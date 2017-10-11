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

namespace set_win_v1
{
    public partial class Form1 : Form
    {
        [Serializable]
        public class wininfo
        {
            public string fullname="";  //文件路径名
            public IntPtr wndhandle=new IntPtr(); //暂时用用得handle
            public RECT rect=new RECT();   //位置记忆 
        }

        [Serializable]
        public class wininfo_group
        {
            public string groupname=""; //分组名称
            public List<wininfo> wininfo_list=new List<wininfo>();//窗口具体位置的list 
        
        }

        mousehook mouse = new mousehook();
        List<wininfo> win_info_list = new List<wininfo>(); //记录每次的程序内容
        wininfo_group win_group = new wininfo_group();  //记录每次的分组内容 
        List<wininfo_group> wininfo_group_list = new List<wininfo_group>(); //所有的分组
        string fullpathname = "";

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            


            //添加分组--设置鼠标hook
            if (button1.Text.Equals("记录窗口布局"))
            {

                //添加分组--获得分组数
                int group_num = treeView1.GetNodeCount(false);
                string formatstring = string.Format("group{0}", group_num);

                //添加分组--新建分组
                win_group.groupname = formatstring;

                //添加分组--添加显示节点
                treeView1.SelectedNode = treeView1.Nodes.Add(formatstring);

                mouse.OnMouseActivity += new MouseEventHandler(mouse_OnMouseActivity);
                mouse.Start();
                button1.Text = "停止记录";
                return;
            }
          
            //添加分组--分组添加到分组list
            wininfo_group_list.Add(Clone<wininfo_group>(win_group));
            //添加分组--清除分组group
            win_group.groupname = "";
            win_group.wininfo_list.Clear();
            //添加分组--停止HOOK
            mouse.Stop();
            button1.Text = "记录窗口布局";
          
            //设置鼠标形状
            //点击过程中，记录相关信息
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
                    win_info_list.Add(Clone< wininfo>(win_info));
                    fullpathname = win_info.fullname;

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

            
            wininfo a = new wininfo();
            a.fullname = "aaaa";
            a.wndhandle =(IntPtr)10000;
            a.rect.Left = 10;
            a.rect.Right = 10;
            a.rect.Top = 10;
            a.rect.Bottom = 10;

            win_group.groupname = "abcdefg";
            win_group.wininfo_list.Add(Clone<wininfo>(a));

            a.fullname = "bbbb";
            a.wndhandle = (IntPtr)20000;
            a.rect.Left = 20;
            a.rect.Right = 20;
            a.rect.Top = 20;
            a.rect.Bottom = 20;

            win_group.wininfo_list.Add(Clone<wininfo>(a));

            wininfo_group_list.Add(Clone<wininfo_group>(win_group));

            win_group.groupname = "";
            win_group.wininfo_list.Clear();
            foreach(wininfo_group i in wininfo_group_list)
            {
                listBox1.Items.Add(i.groupname);
                foreach(wininfo j in i.wininfo_list)
                {
                    listBox1.Items.Add(string.Format("{0},{1},{2}", j.fullname,j.wndhandle,j.rect.ToString()));
                }
            }

        }
    }
}
