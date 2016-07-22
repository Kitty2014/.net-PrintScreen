using System;
using System.Runtime.InteropServices;

namespace Leexsoft.PrintScreen.Listener
{
    public sealed class KeyListener
    {
        //声明键盘钩子处理的初始值
        public static int hKeyboardHook = 0;
        //钩子代理的定义及声明
        public delegate int HookProc(int nCode, int wParam, IntPtr lParam);
        public HookProc KeyboardHookProcedure;

        #region API 定义
        //线程键盘钩子监听鼠标消息设为2，全局键盘监听鼠标消息设为13
        private static readonly int WH_KEYBOARD_LL = 13;
        public static readonly int WM_KEYDOWN = 0x100;     //KEYDOWN:256
        public static readonly int WM_KEYUP = 0x101;        //KEYUP:257
        public static readonly int WM_SYSKEYDOWN = 0x104;  //SYSKEYDOWN:260
        public static readonly int WM_SYSKEYUP = 0x105;    //SYSKEYUP:261

        //安装钩子
        [DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
        private static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, int threadId);
        //卸载钩子
        [DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
        private static extern bool UnhookWindowsHookEx(int idHook);
        //通过信息钩子继续下一个钩子
        [DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
        public static extern int CallNextHookEx(int idHook, int nCode, int wParam, IntPtr lParam);
        //使用WINDOWS API函数代替获取当前实例的函数,防止钩子失效
        [DllImport("kernel32.dll")]
        private static extern IntPtr GetModuleHandle(string name);
        #endregion
        
        public void Start()
        {
            if (hKeyboardHook == 0)
            {
                if (KeyboardHookProcedure != null)
                {
                    hKeyboardHook = SetWindowsHookEx(WH_KEYBOARD_LL, KeyboardHookProcedure, GetModuleHandle(System.Diagnostics.Process.GetCurrentProcess().MainModule.ModuleName), 0);
                }
            }
        }
        public void Stop()
        {
            if (hKeyboardHook != 0)
            {
                if (!UnhookWindowsHookEx(hKeyboardHook))
                {
                    throw new Exception("钩子卸载失败");
                }
                hKeyboardHook = 0;
            }
        }
    }
}
