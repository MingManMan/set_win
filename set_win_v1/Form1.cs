using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using set_win;
using System.Runtime.InteropServices;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Threading;

namespace set_win_v1
{
    public partial class Form1 : Form
    {
        [Serializable]
        public class wininfo
        {
            public string fullname = "";  //文件路径名
            public IntPtr wndhandle = new IntPtr(); //暂时用用得handle
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
        List<wininfo_group> win_group_list = new List<wininfo_group>(); //所有的分组
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

            string fp = System.Windows.Forms.Application.StartupPath + "\\info.json";
            if (!File.Exists(fp))  // 判断是否已有相同文件 
            {
                FileStream fs1 = new FileStream(fp, FileMode.Create, FileAccess.ReadWrite);
                fs1.Close();
            }
            File.WriteAllText(fp, JsonConvert.SerializeObject(win_group_list, Formatting.Indented));
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
                throw new ApplicationException("Can't get the full path name.", ex);


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

            ////得到窗口的标题
            //StringBuilder title = new StringBuilder(256);
            //win32api.GetWindowText(formHandle, title, title.Capacity);

            //鼠标左键点击
            if (e.Button == MouseButtons.Left)
            {
                //窗口信息获得
                wininfo win_info = new wininfo();
                win_info.rect = get_window_rect(formHandle);
                win_info.wndhandle = formHandle;

                //重复项判断
                bool have = false;
                foreach (wininfo i in win_group.win_info_list)
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
                    win_group.win_info_list.Add(Clone<wininfo>(win_info));
                    fullpathname = win_info.fullname;
                    treeView1.SelectedNode = CurrentNode;
                    treeView1.SelectedNode.Nodes.Add(fullpathname.Trim());
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
            //获得
            wininfo_group win_info_group = new wininfo_group();
            win_info_group = win_group_list[groupnode.Index];
            foreach (wininfo i in win_info_group.win_info_list)
            {

                // win32api.SetWindowPos(i.wndhandle, -1, i.rect.Left, i.rect.Top, i.rect.Right - i.rect.Left, i.rect.Bottom - i.rect.Top, win32api.SWP_DRAWFRAME|win32api.SWP_NOZORDER| win32api.SWP_ASYNCWINDOWPOS);
                int aaa = win32api.MoveWindow(i.wndhandle, i.rect.Left, i.rect.Top, i.rect.Right - i.rect.Left, i.rect.Bottom - i.rect.Top, true);
                if (aaa == 0)
                {
                    Process my = new Process();
                    my = Process.Start(i.fullname);
                    Thread.Sleep(3000);
                    win32api.MoveWindow(my.MainWindowHandle, i.rect.Left, i.rect.Top, i.rect.Right - i.rect.Left, i.rect.Bottom - i.rect.Top, true);
                }



            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //读取窗口分组信息
            string fp = System.Windows.Forms.Application.StartupPath + "\\info.json";
            if (!File.Exists(fp))  // 判断是否已有相同文件 
            {
                FileStream fs1 = new FileStream(fp, FileMode.Create, FileAccess.ReadWrite);
                fs1.Close();
            }
            //List<wininfo_group>

            win_group_list = JsonConvert.DeserializeObject<List<wininfo_group>>(File.ReadAllText(fp));
            foreach (wininfo_group i in win_group_list)
            {
                treeView1.SelectedNode = treeView1.Nodes.Add(i.groupname);
                foreach (wininfo j in i.win_info_list)
                {
                    treeView1.SelectedNode.Nodes.Add(j.fullname);
                }

            }
            if (treeView1.Nodes.Count > 0)
            {
                treeView1.SelectedNode = treeView1.Nodes[0];
            }
            treeView1.ExpandAll();

        }

        public void AddTree(TreeNode parentNode)
        {

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //你还处于鼠标hook状态，

            mouse.Stop();
        }
    }
}
