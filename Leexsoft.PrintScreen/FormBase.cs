using Leexsoft.PrintScreen.Listener;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Leexsoft.PrintScreen
{
    public partial class FormBase : Form
    {
        /// <summary>
        /// 监听器
        /// </summary>
        private KeyListener Listener { get; set; }
        /// <summary>
        /// 监听配置
        /// </summary>
        private KeyListenerConfig Config { get; set; }
        /// <summary>
        /// 截屏保存
        /// </summary>
        private FileSaver PicSaver { get; set; }
        /// <summary>
        /// 配置控件
        /// </summary>
        private CtrlBase LoadCtrl { get; set; }

        public FormBase()
        {
            InitializeComponent();
            this.Listener = new KeyListener();
            this.Config = new KeyListenerConfig();
            this.PicSaver = new FileSaver();
        }

        private void FormBase_Load(object sender, EventArgs e)
        {
            this.Hide();
            this.notifyIcon1.ShowBalloonTip(1000);
            //开启钩子
            if (this.Listener.KeyboardHookProcedure == null)
            {
                this.Listener.KeyboardHookProcedure = this.OnKeyDown;
            }
            this.Listener.Start();
        }

        private void FormBase_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e != null)
            {
                e.Cancel = true;
            }
            this.HideForm();
        }

        #region 菜单事件
        private void menuItemSetFolder_Click(object sender, EventArgs e)
        {
            this.LoadCtrl = new FolderCtrl();
            this.ShowForm();
        }

        private void menuItemArea_Click(object sender, EventArgs e)
        {
            this.LoadCtrl = new ScreenCtrl();
            this.ShowForm();
        }

        private void menuItemExit_Click(object sender, EventArgs e)
        {
            //卸载钩子
            this.Listener.Stop();
            //隐藏托盘图标
            this.notifyIcon1.Visible = false;
            //退出应用
            Environment.Exit(1);
        }
        #endregion

        #region 托盘显示和隐藏
        private void HideForm()
        {
            if (this.WindowState != FormWindowState.Minimized)
            {
                this.LoadCtrl.Dispose();
                this.LoadCtrl = null;
                this.Hide();
                this.WindowState = FormWindowState.Minimized;
                //隐藏配置窗体时，重启按键监听
                if (this.Config.HasChange)
                {                    
                    this.ReStartListner();
                }
            }
        }

        private void ShowForm()
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Controls.Clear();
                if (this.LoadCtrl != null)
                {
                    this.LoadCtrl.SetValue(this.Config);
                    this.Height = this.LoadCtrl.Height + 40;
                    this.Width = this.LoadCtrl.Width + 5;
                    this.LoadCtrl.Dock = DockStyle.Fill;
                    this.Controls.Add(LoadCtrl);
                }
                this.Location = new Point(400, 300);
                this.Show();
                this.WindowState = FormWindowState.Normal;
            }
        }

        private void ReStartListner()
        {
            this.Config.SaveConfig();
            this.Listener.Stop();
            this.Listener.Start();
        }
        #endregion

        #region 按键监听及截屏实现
        private int OnKeyDown(int nCode, int wParam, IntPtr lParam)
        {
            var currentKey = (Key)Marshal.PtrToStructure(lParam, typeof(Key));
            if (wParam == KeyListener.WM_KEYDOWN)
            {
                if (currentKey.vkCode == Keys.PrintScreen.GetHashCode())
                {
                    this.SaveImg();
                }
            }

            if (currentKey.vkCode == Keys.PrintScreen.GetHashCode())
            {
                //如果返回1，则结束消息，这个消息到此为止，不再传递。
                return 1;
            }
            else
            {
                //如果返回0或调用CallNextHookEx函数则消息出了这个钩子继续往下传递，也就是传给消息真正的接受者 
                return KeyListener.CallNextHookEx(KeyListener.hKeyboardHook, nCode, wParam, lParam);
            }
        }
        private void SaveImg()
        {
            //截屏
            var rect = this.Config.WatchArea == Rectangle.Empty ? this.Config.WatchScreen.WorkingArea : this.Config.WatchArea;
            var bmp = new Bitmap(rect.Width, rect.Height);
            var gph = Graphics.FromImage(bmp);
            gph.CopyFromScreen(rect.X, rect.Y, 0, 0, new Size(rect.Width, rect.Height));
            //保存
            this.PicSaver.SaveImg(bmp, this.Config.SavePath);
        }
        #endregion
    }
}