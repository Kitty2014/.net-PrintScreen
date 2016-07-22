using System.Runtime.InteropServices;

namespace Leexsoft.PrintScreen.Listener
{
    [StructLayout(LayoutKind.Sequential)]
    public class Key
    {
        public int vkCode;      //定一个虚拟键码。该代码必须有一个价值的范围1至254
        public int scanCode;    // 指定的硬件扫描码的关键
        public int flags;       // 键标志
        public int time;        // 指定的时间戳记的这个讯息
        public int dwExtraInfo; // 指定额外信息相关的信息
    }
}
