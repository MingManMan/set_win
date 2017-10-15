using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using System.Runtime.InteropServices;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Threading;
using set_win;
using System.Text;

namespace set_win_v1
{
    public partial class Form1 : Form
    {
        [Serializable]
        public class wininfo
        {
            public string fullname = "";  //文件路径名
            public IntPtr wndhandle = new IntPtr(); //暂时用用得handle
            public IntPtr pid = new IntPtr();  //对应进程PID
            public RECT rect = new RECT();   //位置记忆 
        }

        [Serializable]
        public class wininfo_group
        {
            public string groupname = ""; //分组名称
            public List<wininfo> win_info_list = new List<wininfo>();//窗口具体位置的list 

        }

        mousehook mouse = new mousehook();
        List<wininfo> win_info_list = new List<wininfo>(); //记录每次的程序内容
        wininfo_group win_group = new wininfo_group();  //记录每次的分组内容 
        List<wininfo_group> win_group_list ; //所有的分组
        string fullpathname = "";
        TreeNode CurrentNode = new TreeNode();  //用来记录在添加的时候的group，在选择过程中选择了其他节点，造成节点添加出错。

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            if (button1.Text.Equals("记录窗口布局"))
            {
                this.TopMost = true;
                treeView1.ExpandAll();
                //添加分组--获得分组数
                int group_num = treeView1.GetNodeCount(false);
                string formatstring = string.Format("group{0}", group_num);

                //添加分组--新建分组
                win_group.groupname = formatstring;

                //添加分组--添加显示节点
                CurrentNode = treeView1.Nodes.Add(formatstring);

                mouse.OnMouseActivity += new MouseEventHandler(mouse_OnMouseActivity);
                mouse.Start();
                button1.Text = "停止记录";

                //添加分组--改变鼠标形状，避免忘记
                IntPtr hcur = win32api.LoadCursorFromFile("RWHand.cur");
                win32api.SetSystemCursor(hcur, win32api.OCR_NORMAL);
                win32api.SetSystemCursor(Cursors.Hand.CopyHandle(), win32api.OCR_IBEAM);

                return;
            }

            //停止添加分组--分组添加到分组list
            win_group_list.Add(Clone<wininfo_group>(win_group));
            //停止添加分组--清除分组group
            win_group.groupname = "";
            win_group.win_info_list.Clear();

            //停止添加分组--停止HOOK
            mouse.Stop();
            button1.Text = "记录窗口布局";
            //停止添加分组--恢复鼠标
            win32api.SystemParametersInfo(win32api.SPI_SETCURSORS, 0, IntPtr.Zero, win32api.SPIF_SENDWININICHANGE);
            //点击过程中，记录相关信息

            save_json_file(Application.StartupPath + "\\info.json");
            this.TopMost = false;
        }

        private RECT get_window_rect(IntPtr whandle)
        {
            RECT rect = new RECT();
            win32api.GetWindowRect(whandle, ref rect);
            return rect;
        }

        private string get_fullpath(UInt32 pid)
        {
            string fullpath = "";
            IntPtr handleToSnapshot = IntPtr.Zero;
            try
            {
                MODULEENTRY32 mod = new MODULEENTRY32();
                mod.dwSize = (UInt32)Marshal.SizeOf(typeof(MODULEENTRY32));
                handleToSnapshot = win32api.CreateToolhelp32Snapshot((uint)SnapshotFlags.Module, pid);
                if (win32api.Module32First(handleToSnapshot, ref mod))
                {
                    fullpath = mod.szExePath;
                }
                else
                { throw new ApplicationException(string.Format("Failed with win32 error code {0}", Marshal.GetLastWin32Error())); }
            }
            catch (Exception ex)
            {
               // throw new ApplicationException("Can't get the full path name.", ex);


            }
            finally
            {
                win32api.CloseHandle(handleToSnapshot);
            }
            return fullpath;
        }

        //功能核心函数，重点关注
        void mouse_OnMouseActivity(object sender, MouseEventArgs e)
        {
            //获得当前鼠标位置
            
            int x = Cursor.Position.X;
            int y = Cursor.Position.Y;
            
            //得到鼠标位置窗口句柄
            Point p = new Point(x, y);
            IntPtr formHandle = win32api.WindowFromPoint(p);
            IntPtr parent_handle = win32api.GetParent(formHandle);
            
            //确保获得顶级窗口。
            while (parent_handle != IntPtr.Zero)
            {
                formHandle = parent_handle;
                parent_handle = win32api.GetParent(formHandle);
                
            }

          
            //鼠标左键点击

            if (e.Button == MouseButtons.Left)
            {

                ////得到窗口的标题
                StringBuilder title = new StringBuilder(256);
                win32api.GetWindowText(formHandle, title, title.Capacity);
                string aaa = string.Format("handlw:{0} title:{1}", formHandle, title);
                treeView2.Nodes.Add(aaa);

                //窗口信息获得
                wininfo win_info = new wininfo();
                win_info.rect = get_window_rect(formHandle);
                win_info.wndhandle = formHandle;

                ////重复项判断
                bool have = false;
                foreach (wininfo i in win_group.win_info_list)
                {
                    if (i.wndhandle == formHandle)
                    {
                        have = true;
                        break;
                    }
                }

                ////添加选择记录
                if (!have)
                {
                    //fullpath获得
                    uint num = 0;
                    win32api.GetWindowThreadProcessId(win_info.wndhandle, ref num);
                    win_info.fullname = get_fullpath(num);
                    win_info.pid = (IntPtr)num;
                    win_group.win_info_list.Add(Clone<wininfo>(win_info));
                    fullpathname = win_info.fullname;
                    treeView1.SelectedNode = CurrentNode;
                    string ddd = string.Format("{0},{1}", num, fullpathname);
                    treeView1.SelectedNode.Nodes.Add(fullpathname.Trim());
                    //treeView1.SelectedNode.Nodes.Add(ddd);
                }

            }

        }


        //深度克隆，必须要类型声明为可序列化
        //[Serializable]
        //public class wininfo
        //{
        //    ...
        //}
        //使用：win_group.wininfo_list.Add(Clone<wininfo>(a));
        public static T Clone<T>(T RealObject)
        {
            using (Stream objectStream = new MemoryStream())
            {
                //利用 System.Runtime.Serialization序列化与反序列化完成引用对象的复制  
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(objectStream, RealObject);
                objectStream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(objectStream);
            }
        }

        //获得所有的顶级窗口的全路径
        public List<wininfo> get_top_win_info_way_1()
        {
            List<wininfo> path_list = new List<wininfo>();

            hWnd_list.Clear();
            WndEnumCallBack my_call_back = new WndEnumCallBack(my_call_back_function);
            win32api.EnumWindows(my_call_back, 0);

            foreach (IntPtr i in hWnd_list)
            {
                uint num = 0;
                win32api.GetWindowThreadProcessId(i, ref num);
                string a = string.Format("{0}",get_fullpath(num));
                bool path_list_have_it = false;
                foreach(wininfo j in path_list)
                {
                    if (j.fullname==a)
                    {
                        path_list_have_it = true;
                        break;
                    }

                }
               path_list_have_it = false;
               if (!path_list_have_it)
                {
                    wininfo t = new wininfo();
                    t.fullname = a;
                    t.wndhandle = i;
                    t.pid= (IntPtr)num;
                    t.rect.Top = 0;
                    t.rect.Bottom = 0;
                    t.rect.Left = 0;
                    t.rect.Right = 0;
                    path_list.Add(t);
                }
            }
            return path_list;
        }

        //窗口遍历回调函数
        //调用该函数时需要先
        // hWnd_list.clear();清空列表
        List<IntPtr> hWnd_list = new List<IntPtr>();
        
        public bool my_call_back_function(IntPtr hWnd, IntPtr lParam)
        {
            if (win32api.IsWindowVisible(hWnd) && win32api.IsWindow(hWnd) && win32api.IsWindowEnabled(hWnd))
            {
                hWnd_list.Add(hWnd);
            }
            return true;
        }


        //主窗口判断
        private bool IsMainWindow(IntPtr handle)
        {
            return (!(win32api.GetWindow(handle, 4) != IntPtr.Zero) && win32api.IsWindowVisible(handle));
        }


        //获取主窗口handle
        IntPtr the_mainWindHandle = IntPtr.Zero;  //用于获得主窗口的handle
        public bool find_main_window_handle(IntPtr hWnd, IntPtr lParam)
        {
            the_mainWindHandle = IntPtr.Zero;
            uint num = 0;
            win32api.GetWindowThreadProcessId(hWnd, ref num);
            if (num !=0)
            {
                the_mainWindHandle = hWnd;
                return false;
            }
            return true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //获得选择的分组
            TreeNode groupnode = new TreeNode();
            if (treeView1.SelectedNode == null)
            {
                MessageBox.Show("什么都没有搞毛！");
                return;
            }
            else
            {
                if (treeView1.SelectedNode.Parent == null)
                    groupnode = treeView1.SelectedNode;
                else
                {
                    groupnode = treeView1.SelectedNode.Parent;
                }
            }


            //获得当前所有运行中程序的全路径名，以及对应的窗口句柄
            treeView2.Nodes.Clear();
            List<wininfo> path_list = new List<wininfo>();
            path_list = get_top_win_info_way_2();
            foreach (wininfo k in path_list)
            {
                string aaa = string.Format("pid:{0},winhandle:{1},fullname:{2}",k.pid,k.wndhandle,k.fullname);
                treeView2.Nodes.Add(aaa);
            }

            
            //获得
            wininfo_group win_info_group = new wininfo_group();
            win_info_group = win_group_list[groupnode.Index];
            foreach (wininfo i in win_info_group.win_info_list)
            {
                //如果有列表中的名字，直接移动窗口,不存在启动进程移动窗口
                bool movied = false;
                foreach (wininfo j in path_list)
                {
                    if (j.fullname == i.fullname)
                    {
                        //如果存在直接移动窗口
                        //win32api.MoveWindow(j.wndhandle, i.rect.Left, i.rect.Top, i.rect.Right - i.rect.Left, i.rect.Bottom - i.rect.Top, true);
                        win32api.SetWindowPos(j.wndhandle, -1, i.rect.Left, i.rect.Top, i.rect.Right - i.rect.Left, i.rect.Bottom - i.rect.Top, win32api.SWP_DRAWFRAME|win32api.SWP_NOZORDER| win32api.SWP_ASYNCWINDOWPOS);
                        win32api.ShowWindow(j.wndhandle, win32api.SW_SHOWNORMAL);
                        win32api.UpdateWindow(j.wndhandle);

                        movied = true;
                        break;
                    }

                }
                if (!movied)
                {
                    //如果没有，新启动程序，移动窗口。
                    Process my = new Process();
                        my = Process.Start(i.fullname);
                       // win32api.MoveWindow(my.MainWindowHandle, i.rect.Left, i.rect.Top, i.rect.Right - i.rect.Left, i.rect.Bottom - i.rect.Top, true);
                        win32api.SetWindowPos(my.MainWindowHandle, -1, i.rect.Left, i.rect.Top, i.rect.Right - i.rect.Left, i.rect.Bottom - i.rect.Top, win32api.SWP_DRAWFRAME|win32api.SWP_NOZORDER| win32api.SWP_ASYNCWINDOWPOS);
                    win32api.ShowWindow(my.MainWindowHandle, win32api.SW_SHOWNORMAL);
                    win32api.UpdateWindow(my.MainWindowHandle);
                    Thread.Sleep(100);
                }
                //这句也是可以用的。
                // win32api.SetWindowPos(i.wndhandle, -1, i.rect.Left, i.rect.Top, i.rect.Right - i.rect.Left, i.rect.Bottom - i.rect.Top, win32api.SWP_DRAWFRAME|win32api.SWP_NOZORDER| win32api.SWP_ASYNCWINDOWPOS);
            }
        }
        

        private void Form1_Load(object sender, EventArgs e)
        {
            //读取窗口分组信息
            road_json_file(Application.StartupPath + "\\info.json");
            treeView1.HideSelection = false;
            this.treeView1.DrawMode = TreeViewDrawMode.OwnerDrawText;
            this.treeView1.DrawNode += new DrawTreeNodeEventHandler(treeView1_DrawNode);
        }

        private void road_json_file(string fp)
        {
            
            if (!File.Exists(fp))  // 判断是否已有相同文件 
            {
                FileStream fs1 = new FileStream(fp, FileMode.Create, FileAccess.ReadWrite);
                fs1.Close();
            }
            string aaa = File.ReadAllText(fp);
            win_group_list = JsonConvert.DeserializeObject<List<wininfo_group>>(aaa);
            if (win_group_list == null)
            {
                win_group_list = new List<wininfo_group>();
            }
            else
            {
                foreach (wininfo_group i in win_group_list)
                {
                    treeView1.SelectedNode = treeView1.Nodes.Add(i.groupname);
                    foreach (wininfo j in i.win_info_list)
                    {
                        treeView1.SelectedNode.Nodes.Add(j.fullname);
                    }

                }
                if (win_group_list.Count > 0) {
                    treeView1.SelectedNode = treeView1.Nodes[0];
                    treeView1.ExpandAll();
                }
                
            }
        }

    

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //你还处于鼠标hook状态，
            //鼠标手势还原
            win32api.SystemParametersInfo(win32api.SPI_SETCURSORS, 0, IntPtr.Zero, win32api.SPIF_SENDWININICHANGE);
            //停止鼠标hook
            mouse.Stop();
        }

        private void treeView1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Point ClickPoint = new Point(e.X, e.Y);
                int x = e.X;
                int y = e.Y;
                TreeNode CurrentNode = treeView1.GetNodeAt(ClickPoint);
                if (CurrentNode!=null)//判断你点的是不是一个节点
                {
                    treeView1.SelectedNode = CurrentNode;
                    contextMenuStrip1.Show(MousePosition);
                }
                
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            win32api.SystemParametersInfo(win32api.SPI_SETCURSORS, 0, IntPtr.Zero, win32api.SPIF_SENDWININICHANGE);
            mouse.Stop();
        }

        private void 删除该节点ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode == null)
            {
                MessageBox.Show("请选择要删除的节点！");
                return;
            }

            //找到他的父节点
            TreeNode groupnode = new TreeNode();
            if (treeView1.SelectedNode.Parent == null) //本身就是group节点
            {
                groupnode = treeView1.SelectedNode;
                win_group_list.RemoveAt(groupnode.Index);  //直接删除group 节点
                treeView1.SelectedNode.Remove();
            }
            else
            {
                groupnode = treeView1.SelectedNode.Parent;
                foreach (wininfo i in win_group_list[groupnode.Index].win_info_list)
                {
                    if (i.fullname == treeView1.SelectedNode.Text)
                    {
                        win_group_list[groupnode.Index].win_info_list.Remove(i);
                        break;
                    }
                }
                treeView1.SelectedNode.Remove();

                if (win_group_list[groupnode.Index].win_info_list.Count == 0)
                {
                    win_group_list.RemoveAt(groupnode.Index);
                    treeView1.Nodes.RemoveAt(groupnode.Index);
                }
            }

            save_json_file(Application.StartupPath + "\\info.json");
           
            win_group_list.Clear();
            treeView1.Nodes.Clear();
            //重新载入 
            
            road_json_file(Application.StartupPath + "\\info.json");
        }

       

        private void save_json_file(string fp)
        {
            
            if (!File.Exists(fp))  // 判断是否已有相同文件 
            {
                FileStream fs1 = new FileStream(fp, FileMode.Create, FileAccess.ReadWrite);
                fs1.Close();
            }
            File.WriteAllText(fp, JsonConvert.SerializeObject(win_group_list, Formatting.Indented));
        }

        private void button4_Click(object sender, EventArgs e)
        {

            List<wininfo> path_list = new List<wininfo>();
            path_list = get_top_win_info_way_2();
            foreach (wininfo i in path_list)
            {
                StringBuilder title = new StringBuilder(256);
                win32api.GetWindowText(i.wndhandle, title, title.Capacity);
                string ddd = string.Format("pid:{0}-wndle:{1}--{2}", i.pid, i.wndhandle.ToString(), i.fullname);
                //if (title.Length > 0)
                    treeView1.Nodes.Add(ddd);
                
            }
            

        }


        //获得所有顶级窗口的 进程id，窗口进程，全路径名
        private List<wininfo> get_top_win_info_way_2() 
        {
            List<wininfo> path_list = new List<wininfo>();

            //通过desktop窗口反向找的次级窗口为实际应用窗口的顶级窗口
            //1、获取桌面窗口的句柄
            IntPtr desktopPtr = win32api.GetDesktopWindow();
            textBox1.Text = desktopPtr.ToString();
            //2、获得一个子窗口（这通常是一个顶层窗口，当前活动的窗口）
            IntPtr winPtr = win32api.GetWindow(desktopPtr, 5);

            //3、循环取得桌面下的所有子窗口
            List<IntPtr> winPtr_list = new List<IntPtr>();
            while (winPtr != IntPtr.Zero)
            {
                if (win32api.IsWindowVisible(winPtr) && win32api.IsWindow(winPtr) && win32api.IsWindowEnabled(winPtr))
                {
                    
                    uint num = 0;
                    win32api.GetWindowThreadProcessId(winPtr, ref num);
                    string a = string.Format("{0}", get_fullpath(num));

                    bool path_list_have_it = false;
                    foreach (wininfo j in path_list)
                    {
                        if (j.fullname == a)
                        {
                            path_list_have_it = true;
                            break;
                        }

                    }
                    path_list_have_it = false;
                    if (!path_list_have_it )
                    {
                        wininfo t = new wininfo();
                        t.fullname = a;
                        t.wndhandle = winPtr;
                        t.pid = (IntPtr)num;
                        t.rect.Top = 0;
                        t.rect.Bottom = 0;
                        t.rect.Left = 0;
                        t.rect.Right = 0;
                        path_list.Add(t);
                    }
                }
                //4、继续获取下一个子窗口
                winPtr = win32api.GetWindow(winPtr, 2);
            }
            return path_list;
        }

        //treeview在绘制节点事件中，按自已想的绘制
        private void treeView1_DrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            e.DrawDefault = true; //我这里用默认颜色即可，只需要在TreeView失去焦点时选中节点仍然突显
            return;

            if ((e.State & TreeNodeStates.Selected) != 0)
            {
                //演示为绿底白字
                e.Graphics.FillRectangle(Brushes.DarkBlue, e.Node.Bounds);

                Font nodeFont = e.Node.NodeFont;
                if (nodeFont == null) nodeFont = ((TreeView)sender).Font;
                e.Graphics.DrawString(e.Node.Text, nodeFont, Brushes.White, Rectangle.Inflate(e.Bounds, 2, 0));
            }
            else
            {
                e.DrawDefault = true;
            }

            if ((e.State & TreeNodeStates.Focused) != 0)
            {
                using (Pen focusPen = new Pen(Color.Black))
                {
                    focusPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
                    Rectangle focusBounds = e.Node.Bounds;
                    focusBounds.Size = new Size(focusBounds.Width - 1,
                    focusBounds.Height - 1);
                    e.Graphics.DrawRectangle(focusPen, focusBounds);
                }
            }

        }


    }
}
