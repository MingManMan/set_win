using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace  set_win

{
    class mousehook
    {
        public int aaa = 0;
        public int x ;
        public int y ;


        public event MouseEventHandler OnMouseActivity;//全局的事件 
        private HookProc _mouseHookProcedure;
        private static int _hMouseHook = 0; // 鼠标钩子句柄  
        public mousehook() { }
        ~mousehook() { }
        public void Start()
        {
            // 安装鼠标钩子  
            if (_hMouseHook == 0)
            {
                // 生成一个HookProc的实例.  
                _mouseHookProcedure = new HookProc(MouseHookProc);

                _hMouseHook = win32api.SetWindowsHookEx(win32api.WH_MOUSE_LL, _mouseHookProcedure, Marshal.GetHINSTANCE(System.Reflection.Assembly.GetExecutingAssembly().GetModules()[0]), 0);

                //如果装置失败停止钩子  
                if (_hMouseHook == 0)
                {
                    Stop();
                    throw new Exception("SetWindowsHookEx failed.");
                }
            }
        }
        /// <summary>  
        /// 停止全局钩子  
        /// </summary>  
        public void Stop()
        {
            bool retMouse = true;

            if (_hMouseHook != 0)
            {
                retMouse = win32api.UnhookWindowsHookEx(_hMouseHook);
                _hMouseHook = 0;
            }

            // 如果卸下钩子失败  
            if (!(retMouse))
                throw new Exception("UnhookWindowsHookEx failed.");
        }
        /// <summary>  
        /// 鼠标钩子回调函数  
        /// </summary>  
        private int MouseHookProc(int nCode, Int32 wParam, IntPtr lParam)
        {
            // 如果正常运行并且用户要监听鼠标的消息  
            if ((nCode >= 0) && (OnMouseActivity != null))
            {
                MouseButtons button = MouseButtons.None;
                int clickCount = 0;
                
                switch (wParam)
                {
                    case win32api.WM_LBUTTONDOWN:
                        button = MouseButtons.Left;
                        clickCount = 1;
                        break;
                    case win32api.WM_LBUTTONUP:
                        button = MouseButtons.Left;
                        clickCount = 1;
                        break;
                    case win32api.WM_LBUTTONDBLCLK:
                        button = MouseButtons.Left;
                        clickCount = 2;
                        break;
                    case win32api.WM_RBUTTONDOWN:
                        button = MouseButtons.Right;
                        clickCount = 1;
                        break;
                    case win32api.WM_RBUTTONUP:
                        button = MouseButtons.Right;
                        clickCount = 1;
                        break;
                    case win32api.WM_RBUTTONDBLCLK:
                        button = MouseButtons.Right;
                        clickCount = 2;
                        break;
                }

                // 从回调函数中得到鼠标的信息  
                MouseHookStruct MyMouseHookStruct = (MouseHookStruct)Marshal.PtrToStructure(lParam, typeof(MouseHookStruct));
                MouseEventArgs e = new MouseEventArgs(button, clickCount, MyMouseHookStruct.pt.x, MyMouseHookStruct.pt.y, 0);

                // 如果想要限制鼠标在屏幕中的移动区域可以在此处设置  
                // 后期需要考虑实际的x、y的容差  
                if (!Screen.PrimaryScreen.Bounds.Contains(e.X, e.Y))
                {
                    //return 1;  
                }

                OnMouseActivity(this, e);
            }
            // 启动下一次钩子  
            return win32api.CallNextHookEx(_hMouseHook, nCode, wParam, lParam);
        }
       
    }
}
