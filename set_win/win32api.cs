using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using HANDLE = System.IntPtr;
using HWND = System.IntPtr;
using HDC = System.IntPtr;
using System.Drawing;

namespace set_win
{
    [Flags]
    public enum SnapshotFlags : uint
    {
        HeapList = 0x00000001,
        Process = 0x00000002,
        Thread = 0x00000004,
        Module = 0x00000008,
        Module32 = 0x00000010,
        All = (HeapList | Process | Thread | Module),
        Inherit = 0x80000000,
        NoHeaps = 0x40000000

    }
   
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct PROCESSENTRY32
    {
        const int MAX_PATH = 260;
        internal UInt32 dwSize;
        internal UInt32 cntUsage;
        internal UInt32 th32ProcessID;
        internal IntPtr th32DefaultHeapID;
        internal UInt32 th32ModuleID;
        internal UInt32 cntThreads;
        internal UInt32 th32ParentProcessID;
        internal Int32 pcPriClassBase;
        internal UInt32 dwFlags;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)]
        internal string szExeFile;
    }
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct MODULEENTRY32
    {
        internal uint dwSize;
        internal uint th32ModuleID;
        internal uint th32ProcessID;
        internal uint GlblcntUsage;
        internal uint ProccntUsage;
        internal IntPtr modBaseAddr;
        internal uint modBaseSize;
        internal IntPtr hModule;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        internal string szModule;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        internal string szExePath;
    }

    /// <summary>
    /// 点
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public class POINT
    {
        public int x;
        public int y;
    }


    /// <summary>
    ///  EnumWindows 函数回调定义
    /// </summary>
    public delegate bool WndEnumCallBack(IntPtr hWnd, IntPtr lParam);


    /// <summary>
    /// 钩子结构体
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public class MouseHookStruct
    {
        public POINT pt;
        public int hWnd;
        public int wHitTestCode;
        public int dwExtraInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;                             //最左坐标
        public int Top;                             //最上坐标
        public int Right;                           //最右坐标
        public int Bottom;                        //最下坐标
    }


    

    // 钩子回调定义
    public delegate int HookProc(int nCode, Int32 wParam, IntPtr lParam);
    
    

    public class win32api
    {
        ///改变窗口位置常量定义
        ///
        public const int HWND_BOTTOM = 1; //把窗口放在Z轴的最后，即所有窗口的后面
        public const int HWND_TOP = 0; //将窗口放在Z轴的前面，即所有窗口的前面
        public const int HWND_TOPMOST = -1; //使窗口成为“TopMost”类型的窗口，这种类型的窗口总是在其它窗口的前面，真到它被关闭
        public const int HWND_NOTOPMOST = -2; //将窗口放在所有“TopMost”类型

        public const int SWP_DRAWFRAME = 0x20;  //移动窗口后重画窗口及其上的所有内容
        public const int SWP_HIDEWINDOW = 0x80;  //隐藏窗口，窗口隐藏后既不出现在屏幕上也不出现在任务栏上，但它仍然处于激活状态
        public const int SWP_NOACTIVATE = 0x10;  //窗口移动后不激活窗口，当然，如果窗口在移动前就是激活的则例外
        public const int SWP_NOCOPYBITS = 0x100;  //当窗口移动后，不重画它上面的任何内容
        public const int SWP_NOMOVE = 0x2;  //不移动窗口（即忽略X和Y参数）
        public const int SWP_NOSIZE = 0x1;  //不改变窗口尺寸（即忽略Cx和Cy参数）

        //不重画改变的内容。如果设置了这个标志，则不发生任何重画动作。
        //适用于客户区和非客户区（包括标题栏和滚动条）和任何由于窗回移动而露出的父窗口的所有部分。
        //如果设置了这个标志，应用程序必须明确地使窗口无效并区重画窗口的任何部分和父窗口需要重画的部分。
        public const int SWP_NOREDRAW = 0x8;

        public const int SWP_NOZORDER = 0x4;  //不改变窗口听Z轴位置（即忽略hWndInsertAfter参数）
        public const int SWP_SHOWWINDOW = 0x40;  //显示窗口（之前必须使用过SWP_HIDEWINDOW隐藏窗口）
        public const int SWP_ASYNCWINDOWPOS = 0x4000; //若调用进程不拥有窗口, 系统会向拥有窗口的线程发出需求
        public const int SWP_DEFERERASE = 0x2000; //{防止产生 WM_SYNCPAINT 消息}
        public const int SWP_NOSENDCHANGING = 0x400; //{不发出 WM_WINDOWPOSCHANGING 消息 }


        /// <summary>
        /// 鼠标常量定义
        /// </summary>
        public const int WM_MOUSEMOVE = 0x200;
        public const int WM_LBUTTONDOWN = 0x201;
        public const int WM_RBUTTONDOWN = 0x204;
        public const int WM_MBUTTONDOWN = 0x207;
        public const int WM_LBUTTONUP = 0x202;
        public const int WM_RBUTTONUP = 0x205;
        public const int WM_MBUTTONUP = 0x208;
        public const int WM_LBUTTONDBLCLK = 0x203;
        public const int WM_RBUTTONDBLCLK = 0x206;
        public const int WM_MBUTTONDBLCLK = 0x209;

        public const int WH_MOUSE_LL = 14; // mouse hook constant

        //回复系统鼠标形状设置用
        public const uint SPI_SETCURSORS = 87;
        public const uint SPIF_SENDWININICHANGE = 2;

        //改变系统鼠标形状设置用
        public const uint OCR_NORMAL = 32512; //标准箭头
        public const uint OCR_IBEAM = 32513;  //I形梁
        public const uint OCR_APPSTARTING = 32650; //标准箭头和小的沙漏； 
        public const uint OCR_CROSS = 32515; //交叉十字线光标
        public const uint OCR_HAND = 32649;  //手的形状（WindowsNT5.0和以后版本）
        public const uint OCR_HELP = 32651;  //箭头和向东标记
        public const uint OCR_NO = 32648; //斜的圆
        public const uint OCR_SIZEALL = 32646; //四个方位的箭头分别指向北、南、东、西
        public const uint OCR_SIZENESW = 32643;//双箭头分别指向东北和西南
        public const uint OCR_SIZENS = 32645; //双箭头，分别指向北和南
        public const uint OCR_SIZENWSE = 32642; //双箭头分别指向西北和东南
        public const uint OCR_SIZEWE = 32644;//双箭头分别指向西和东
        public const uint OCR_UP = 32516;
        public const uint OCR_WAIT = 32514;

        //光标形状，通过
        public const uint IDC_ARROW = 32512;
        public const uint IDC_IBEAM = 32513;
        public const uint IDC_WAIT = 32514;
        public const uint IDC_CROSS = 32515;
        public const uint IDC_UPARROW = 32516;
        public const uint IDC_SIZE = 32640;
        public const uint IDC_ICON = 32641;
        public const uint IDC_SIZENWSE = 32642;
        public const uint IDC_SIZENESW = 32643;
        public const uint IDC_SIZEWE = 32644;
        public const uint IDC_SIZENS = 32645;
        public const uint IDC_SIZEALL = 32646;
        public const uint IDC_NO = 32648;
        public const uint IDC_APPSTARTING = 32650;
        public const uint IDC_HELP = 32651;




        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr CreateToolhelp32Snapshot([In]UInt32 dwFlags, [In]UInt32 th32ProcessID);

        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool Process32First([In]IntPtr hSnapshot, ref PROCESSENTRY32 lppe);

        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool Process32Next([In]IntPtr hSnapshot, ref PROCESSENTRY32 lppe);

        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool Module32First(IntPtr hSnapshot, ref MODULEENTRY32 lpme);

        [DllImport("kernel32", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle([In] IntPtr hObject);

        [DllImport("user32", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int EnumWindows(WndEnumCallBack lpEnumFunc, int lParam);

        [DllImport("user32", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool IsWindow(HWND hwnd);

        [DllImport("user32", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool IsWindowEnabled(HWND hwnd);

        [DllImport("user32", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool IsWindowUnicode(HWND hwnd);

        [DllImport("user32", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool IsWindowVisible(HWND hwnd);

        [DllImport("user32", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetWindowText(HWND hWnd, StringBuilder title, int size);

        [DllImport("user32", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetWindowTextLength(HWND hWnd);

        [DllImport("user32", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", EntryPoint = "GetParent", SetLastError = true)]
        public static extern IntPtr GetParent(IntPtr hWnd);

        [DllImport("user32", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool ShowWindow(IntPtr hWnd, uint nCmdShow);

        [DllImport("user32", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int MoveWindow(HWND hwnd, int x, int y, int nWidth, int nHeight, bool bRepaint);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int y, int Width, int Height, int flags);
      



        [DllImport("user32", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern UInt32 GetWindowThreadProcessId(HWND hwnd, ref uint lpdwProcessId);

        [DllImport("user32", CharSet = CharSet.Auto)]
        public static extern IntPtr GetWindow(HWND hwnd, int wCmd);
        //cCmd参数的取值
        //GW_CHILD  5  寻找源窗口的第一个子窗口
        // GW_HWNDFIRST  0  为一个源子窗口寻找第一个兄弟（同级）窗口，或寻找第一个顶级窗口
        // GW_HWNDLAST  1   为一个源子窗口寻找最后一个兄弟（同级）窗口，或寻找最后一个顶级窗口
        //GW_HWNDNEXT   2   为源窗口寻找下一个兄弟窗口
        //GW_HWNDPREV  3 为源窗口寻找前一个兄弟窗口
        //   GW_OWNER  4    寻找窗口的所有者

        // 装置钩子的函数
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, int threadId);

        // 卸下钩子的函数
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern bool UnhookWindowsHookEx(int idHook);

        // 下一个钩挂的函数
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int CallNextHookEx(int idHook, int nCode, Int32 wParam, IntPtr lParam);

        //指定坐标处窗体句柄  
        [DllImport("user32", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr WindowFromPoint(Point point);

        //获取鼠标坐标  
        [DllImport("user32.dll", EntryPoint = "GetCursorPos", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetCursorPos( out POINT lpPoint );

        //获取窗口大小及位置
        [DllImport("user32.dll",SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);

        //改变系统鼠标形状
        [DllImport("User32.DLL", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool SetSystemCursor(IntPtr hcur, uint id);

        //恢复系统鼠标形状设置
        [DllImport("User32.DLL", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool SystemParametersInfo(uint uiAction, uint uiParam, IntPtr pvParam, uint fWinIni);

        /*光标资源加载函数 
         * fileName为加载路径下的.cur文件 
         */
        [DllImport("User32.DLL", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr LoadCursorFromFile(string fileName);


    }
}
