using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading;
using Newtonsoft.Json;
using System.IO;

namespace set_win
{
    public partial class Form1 : Form
    {
        public struct wininfo
        {
            public string fullname;  //文件路径名
            public IntPtr wndhandle; //暂时用用得handle
            public RECT rect;   //位置记忆 
        }
        public struct wininfo_group
        {
            public string groupname; //分组名称
            public List<wininfo> wininfo_list;//窗口具体位置的list 
        }
        public Form1()
        {
            this.TopMost = true;
            InitializeComponent();
        }
        List<UInt32> PIDs = new List<uint>();
        private List<UInt32> get_pids()
        {
            List<UInt32> PIDs = new List<uint>();
            Process parentProc = null;
            IntPtr handleToSnapshot = IntPtr.Zero;
            try
            {
                PROCESSENTRY32 procEntry = new PROCESSENTRY32();
                procEntry.dwSize = (UInt32)Marshal.SizeOf(typeof(PROCESSENTRY32));
                handleToSnapshot = win32api.CreateToolhelp32Snapshot((uint)SnapshotFlags.Process, 0);
                if (win32api.Process32First(handleToSnapshot, ref procEntry))
                {
                    do
                    {
                        listBox1.Items.Add(string.Format("{0,-10}{1,-10}{2}", procEntry.th32ProcessID, procEntry.th32ParentProcessID, procEntry.szExeFile));
                        PIDs.Add(procEntry.th32ProcessID);

                    } while (win32api.Process32Next(handleToSnapshot, ref procEntry));
                }
                else
                { throw new ApplicationException(string.Format("Failed with win32 error code {0}", Marshal.GetLastWin32Error())); }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Can't get the process.", ex);
            }
            finally
            {
                win32api.CloseHandle(handleToSnapshot);
            }
            return PIDs;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            PIDs.Clear();
            PIDs = get_pids();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            uint pid = 10800;
            pid = (uint)Process.GetCurrentProcess().Id;
            get_fullpath(pid);
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
                    listBox1.Items.Add(string.Format("{0,-10}  fullpath：{1}", mod.th32ProcessID, mod.szExePath));
                    fullpath = mod.szExePath;
                }
                else
                { throw new ApplicationException(string.Format("Failed with win32 error code {0}", Marshal.GetLastWin32Error())); }
            }
            catch (Exception ex)
            {
                //throw new ApplicationException("Can't get the full path name.", ex);
                listBox1.Items.Add(ex.ToString());

            }
            finally
            {
                win32api.CloseHandle(handleToSnapshot);
            }
            return fullpath;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            PIDs.Clear();
            PIDs = get_pids();
            foreach (uint i in PIDs)
            {
                get_fullpath(i);

            }

        }


        public bool my_call_back_function(IntPtr hWnd, IntPtr lParam)
        {

            if (win32api.IsWindowVisible(hWnd) && win32api.IsWindow(hWnd) && win32api.IsWindowEnabled(hWnd))
            {
                StringBuilder strbTitle = new StringBuilder();
                int nLength = win32api.GetWindowText(hWnd, strbTitle, strbTitle.Capacity + 1);
                if (nLength > 1)
                {
                    listBox1.Items.Add(strbTitle.ToString());
                }
            }
            return true;
        }


        private void button4_Click(object sender, EventArgs e)
        {
            WndEnumCallBack my_call_back = new WndEnumCallBack(my_call_back_function);
            win32api.EnumWindows(my_call_back, 0);
        }



        uint the_pid = 0;
        IntPtr the_mainWindHandle = IntPtr.Zero;  //用于获得主窗口的handle

        public bool find_main_window_handle(IntPtr hWnd, IntPtr lParam)
        {

            uint num = 0;
            win32api.GetWindowThreadProcessId(hWnd, ref num);
            if (num == the_pid && IsMainWindow(hWnd))
            {
                the_mainWindHandle = hWnd;
                return false;
            }
            return true;
        }

        private bool IsMainWindow(IntPtr handle)
        {
            return (!(win32api.GetWindow(handle, 4) != IntPtr.Zero) && win32api.IsWindowVisible(handle));
        }

        private void button5_Click(object sender, EventArgs e)
        {

            the_pid = 10800;
            the_pid = (uint)Process.GetCurrentProcess().Id;
            WndEnumCallBack my_call_back = new WndEnumCallBack(find_main_window_handle);
            win32api.EnumWindows(my_call_back, 0);

            //以下这句是没有错误，可以使用的。 
            //IntPtr intptr = win32api.FindWindow(null, "Q-Dir 6.71");

            win32api.MoveWindow(the_mainWindHandle, 20, 20, 400, 400, true);
        }

        mousehook mouse = new mousehook();
        //功能核心函数，重点关注
        void mouse_OnMouseActivity(object sender, MouseEventArgs e)
        {
            string str = "X:" + e.X + "  Y:" + e.Y;
            int x = Cursor.Position.X;
            int y = Cursor.Position.Y;
            this.textBox1.Text = string.Format("({0},{1})", x, y);
            //得到窗口句柄
            Point p = new Point(x, y);
            IntPtr formHandle = win32api.WindowFromPoint(p);

            IntPtr parent_handle = win32api.GetParent(formHandle);
            //确保获得顶级窗口。
            while (parent_handle != IntPtr.Zero)
            {
                formHandle = parent_handle;
                parent_handle = win32api.GetParent(formHandle);
            }
            this.textBox2.Text = string.Format("parent_handle:{0}", formHandle.ToString());
            StringBuilder title = new StringBuilder(256);

            //得到窗口的标题
            win32api.GetWindowText(formHandle, title, title.Capacity);
            this.textBox3.Text = title.ToString();
            if (e.Button == MouseButtons.Left)
            {
                wininfo win_info = new wininfo();
                win_info.rect = get_window_rect(formHandle);
                win_info.wndhandle = formHandle;
                listBox1.Items.Add(string.Format("窗口名称:{0},left:{1,-5},right:{2,-5},top:{3,-5},bottom:{4,-5}", textBox3.Text, win_info.rect.Left, win_info.rect.Right, win_info.rect.Top, win_info.rect.Bottom));
                bool have = false;
                //重复项判断
                foreach (wininfo i in win_info_list)
                {
                    if (i.wndhandle == formHandle)
                    {
                        have = true;
                        break;
                    }
                }
                //添加选择记录
                if (!have)
                {
                    //fullpath获得
                    uint num = 0;
                    win32api.GetWindowThreadProcessId(win_info.wndhandle, ref num);
                    win_info.fullname = get_fullpath(num);
                    win_info_list.Add(win_info);
                    textBox3.Text = win_info.fullname;
                }

            }

        }

        private void button6_Click(object sender, EventArgs e)
        {
            listBox1.Items.Add(string.Format("{0}", button6.Text));
            if (button6.Text.Equals("鼠标全局hook一个,获得窗口句柄"))
            {
                mouse.OnMouseActivity += new MouseEventHandler(mouse_OnMouseActivity);
                mouse.Start();
                button6.Text = "停止hook鼠标";
                return;
            }
            mouse.Stop();
            button6.Text = "鼠标全局hook一个,获得窗口句柄";
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            mouse.Stop();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
        }

        private void button7_Click(object sender, EventArgs e)
        {

        }

        private RECT get_window_rect(IntPtr whandle)
        {
            RECT rect = new RECT();
            win32api.GetWindowRect(whandle, ref rect);
            return rect;
        }

        private void button7_Click_1(object sender, EventArgs e)
        {
            the_pid = (uint)Process.GetCurrentProcess().Id;
            WndEnumCallBack my_call_back = new WndEnumCallBack(find_main_window_handle);
            win32api.EnumWindows(my_call_back, 0);
            RECT rect = new RECT();
            rect = get_window_rect(the_mainWindHandle);
            textBox1.Text = string.Format("left:{0,-5},right:{1,-5},top:{2,-5},bottom:{3,-5}", rect.Left, rect.Right, rect.Top, rect.Bottom);
        }
        List<wininfo> win_info_list = new List<wininfo>();
        private void button9_Click(object sender, EventArgs e)
        {
            if (button9.Text.Equals("记住选中窗口位置大小"))
            {
                mouse.OnMouseActivity += new MouseEventHandler(mouse_OnMouseActivity);
                mouse.Start();
                button9.Text = "停止hook鼠标";
                return;
            }
            mouse.Stop();
            button9.Text = "记住选中窗口位置大小";
        }

        private void button10_Click(object sender, EventArgs e)
        {
            foreach (wininfo i in win_info_list)
            {
                win32api.MoveWindow(i.wndhandle, i.rect.Left, i.rect.Top, i.rect.Right - i.rect.Left, i.rect.Bottom - i.rect.Top, true);
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            foreach (wininfo i in win_info_list)
            {
                listBox1.Items.Add(i.fullname);
                Process my = new Process();
                my = Process.Start(i.fullname);
                Thread.Sleep(3000);
                win32api.MoveWindow(my.MainWindowHandle, i.rect.Left, i.rect.Top, i.rect.Right - i.rect.Left, i.rect.Bottom - i.rect.Top, true);
            }

        }
        private void button12_Click_1(object sender, EventArgs e)
        {
            //创建文件
            // 获取当前程序所在路径，并将要创建的文件命名为info.json 
            string fp = System.Windows.Forms.Application.StartupPath + "\\info.json";
            if (!File.Exists(fp))  // 判断是否已有相同文件 
            {
                FileStream fs1 = new FileStream(fp, FileMode.Create, FileAccess.ReadWrite);
                fs1.Close();
            }

            //序列化对象写入文件
            //wininfo obj = new wininfo();
            //RECT rect = new RECT();
            //rect.Left = 10;
            //rect.Right = 20;
            //rect.Top = 30;
            //rect.Bottom = 40;
            //    obj.fullname = "c:\\abc.exe";
            //obj.wndhandle = (IntPtr)2345;
            //obj.rect = rect;
            //File.WriteAllText(fp, JsonConvert.SerializeObject(obj));

            //直接从文件中反序列化到对象
            //wininfo obj = JsonConvert.DeserializeObject<wininfo>(File.ReadAllText(fp));  // 尖括号<>中填入对象的类名 
            //listBox1.Items.Add(obj.fullname);
            //listBox1.Items.Add(obj.wndhandle.ToString());
            //listBox1.Items.Add(obj.rect.ToString());

            //================Collections的序列化和反序列化==============
            //wininfo obj1 = new wininfo();
            //wininfo obj2 = new wininfo();
            //RECT rect1 = new RECT();
            //RECT rect2 = new RECT();
            //rect1.Left = 10;
            //rect1.Right = 20;
            //rect1.Top = 30;
            //rect1.Bottom = 40;
            //obj1.fullname = "c:\\abc.exe";
            //obj1.wndhandle = (IntPtr)2345;
            //obj1.rect = rect1;

            //rect2.Left = 100;
            //rect2.Right = 200;
            //rect2.Top = 300;
            //rect2.Bottom = 400;
            //obj2.fullname = "c:\\abc2.exe";
            //obj2.wndhandle = (IntPtr)23456;
            //obj2.rect = rect2;

            //List<wininfo> obj_list = new List<wininfo>();
            //obj_list.Add(obj1);
            //obj_list.Add(obj2);
            //File.WriteAllText(fp, JsonConvert.SerializeObject(obj_list, Formatting.Indented));

            //List<wininfo> wininfo_list = JsonConvert.DeserializeObject<List<wininfo>>(File.ReadAllText(fp));
            //listBox1.Items.Add(wininfo_list[0].fullname);
            //listBox1.Items.Add(wininfo_list[1].rect.ToString());
            //listBox1.Items.Add(wininfo_list[0].wndhandle.ToString());

        }

        private void button13_Click(object sender, EventArgs e)
        {
            if (button13.Text == "形状改变鼠标")
            {
                //设置系统鼠标形状
                /*设置系统自带图标 
                 * SetSystemCursor(Cursors.WaitCursor.CopyHandle(), OCR_NORMAL); 
                 */
                /*设置外部图标文件 
                 * IntPtr hcur = LoadCursorFromFile(path + "/click.cur");  
                 * SetSystemCursor(hcur, OCR_NORMAL); 
                 */
                //后面这个参数是要改变的，注意
                //bool abc =win32api.SetSystemCursor(Cursors.Hand.CopyHandle(), win32api.OCR_NORMAL);
                IntPtr hcur = win32api.LoadCursorFromFile("RWHand.cur");
                win32api.SetSystemCursor(hcur, win32api.OCR_NORMAL);
                win32api.SetSystemCursor(Cursors.Hand.CopyHandle(), win32api.OCR_IBEAM);
                button13.Text = "恢复";
            }

            else
            {   //回复为系统默认图标
                win32api.SystemParametersInfo(win32api.SPI_SETCURSORS, 0, IntPtr.Zero, win32api.SPIF_SENDWININICHANGE);
                button13.Text = "形状改变鼠标";
            }

        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                //还原窗体显示    
                WindowState = FormWindowState.Normal;
                //激活窗体并给予它焦点
                this.Activate();
                //任务栏区显示图标
                this.ShowInTaskbar = true;
                //托盘区图标隐藏
                notifyIcon1.Visible = false;
            }
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            //判断是否选择的是最小化按钮
            if (WindowState == FormWindowState.Minimized)
            {
                //隐藏任务栏区图标
                this.ShowInTaskbar = false;
                //图标显示在托盘区
                notifyIcon1.Visible = true;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("是否确认退出程序？", "退出", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                // 关闭所有的线程
                this.Dispose();
                this.Close();
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void 显示ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Normal;
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("是否确认退出程序？", "退出", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                // 关闭所有的线程
                this.Dispose();
                this.Close();
            }
        }

        private void button14_Click(object sender, EventArgs e)
        {
            //要添加的节点名称为空，即文本框是否为空
            if (string.IsNullOrEmpty(txtNodeName.Text.Trim()))
            {
                MessageBox.Show("要添加的节点名称不能为空！");
                return;
            }
            //添加根节点
            treeView1.Nodes.Add(txtNodeName.Text.Trim());
            treeView1.Nodes.Add(txtNodeName.Text.Trim());
            txtNodeName.Text = "";
        }

        private void button15_Click(object sender, EventArgs e)
        {
            //要添加的节点名称为空，即文本框是否为空
            if (string.IsNullOrEmpty(txtNodeName.Text.Trim()))
            {
                MessageBox.Show("要添加的节点名称不能为空！");
                return;
            }
            if (treeView1.SelectedNode == null)
            {
                MessageBox.Show("请选择要添加子节点的节点！");
                return;
            }
            treeView1.SelectedNode.Nodes.Add(txtNodeName.Text.Trim());
            txtNodeName.Text = "";
        }

        private void button16_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode == null)
            {
                MessageBox.Show("请选择要删除的节点！");
                return;
            }
            treeView1.SelectedNode.Remove();
        }

        private void 添加roo节点ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void treeView1_MouseDown(object sender, MouseEventArgs e)
        {
            listBox1.Items.Add("treeview mousedown");
            if (e.Button == MouseButtons.Right)
            {
                Point ClickPoint = new Point(e.X, e.Y);
                int x = e.X;
                int y = e.Y;
                TreeNode CurrentNode = treeView1.GetNodeAt(ClickPoint);
                if (CurrentNode is TreeNode)//判断你点的是不是一个节点
                {
                    treeView1.SelectedNode = CurrentNode;
                }
                else
                {
                    //contextMenuStrip2.
                    添加roo节点ToolStripMenuItem.Enabled = false;
                    //treeView1.ContextMenuStrip = this.contextMenuStrip2;
                    //contextMenuStrip2.Show(MousePosition);
                }
            }
        }

        private void button17_Click(object sender, EventArgs e)
        {

        }

        private void button18_Click(object sender, EventArgs e)
        {
            Rectangle ScreenArea = Screen.GetBounds(this);
            listBox1.Items.Add("全屏宽" + ScreenArea.Width.ToString());
            listBox1.Items.Add("全屏高" + ScreenArea.Height.ToString());

            //不要工具栏的大小
            string currentScreenSize_OutTaskBar = SystemInformation.WorkingArea.Width.ToString() + "," + SystemInformation.WorkingArea.Height.ToString();
            listBox1.Items.Add("无工具栏宽" + SystemInformation.WorkingArea.Width.ToString());
            listBox1.Items.Add("无工具栏高" + SystemInformation.WorkingArea.Height.ToString());


        }

        private void button19_Click(object sender, EventArgs e)
        {
            Graphics g = panel1.CreateGraphics();
            //出现一个画笔
            Pen pen = new Pen(Brushes.Red);
            //因为创建矩形需要point对象与size对象
            Point p = new Point(10, 10);
            Size s = new Size(60, 60);
            Rectangle r = new Rectangle(p, s);
            g.DrawRectangle(pen, r);
            //画线
            Point p1 = new Point(20, 20);
            Point p2 = new Point(20, 50);
            g.DrawLine(new Pen(Color.Red, 1), p1, p2);

            int w = panel1.Size.Width;
            int h = panel1.Size.Height;

            int sw = SystemInformation.WorkingArea.Width;
            int sh = SystemInformation.WorkingArea.Height;
            Double fw =  sw/w;  // 绘图区域/原始图
            Double fh = sh/h;
            Double n = 0;

            textBox1.Text = "aaaaa:"+fw.ToString();
            textBox2.Text = fh.ToString();

            if (fw > fh)  //以小的为放缩比例进行处理。
            {
                n = fw;    
            }
            else
            {
                n = fh;
            }
            int nw =(int)(sw/n);
            int nh = (int)(sh/n);
            int grid_size = 10;
            
            textBox3.Text = n.ToString();

            for(int i = 0; i <= nw; i = i + grid_size)
            {
                listBox1.Items.Add(p1.ToString());
                p1 = new Point(i, 0);
                p2 = new Point(i, nw);
                listBox1.Items.Add(p1.ToString()+"   "+p2.ToString());

                g.DrawLine(new Pen(Color.Red, 1), p1, p2);
            }
            for (int j = 0; j < nh; j = j + grid_size)
            {
                p1 = new Point(0, j);
                p2 = new Point(nw, j);
                g.DrawLine(new Pen(Color.Red, 1), p1, p2);
            }
        }
        PickBox pickbox1 = new PickBox();
        private void button20_Click(object sender, EventArgs e)
        {
            Panel p1 = new Panel();
            p1.Location = new Point(10, 10);
            p1.Size = new Size(20, 20);
            p1.ForeColor = Color.Blue;
            p1.BackColor = Color.Blue;
            p1.Visible = true;
            p1.AllowDrop = true;
            p1.Name = "p1";
            
            panel1.Controls.Add(p1);
            p1.Click += new EventHandler(myclick);
            p1.LostFocus+= new EventHandler(mylostfocus);

            pickbox1.WireControl(p1);



        }
        private void mylostfocus(object sender, EventArgs e)
        {
            pickbox1.Remove();
            textBox2.Text = "remove control";
        }

           private void myclick(object sender,EventArgs e)
        {
           
           
            Panel o = (Panel)sender;
           
            textBox1.Text += o.Name.ToString();
           

        }
    }
}

