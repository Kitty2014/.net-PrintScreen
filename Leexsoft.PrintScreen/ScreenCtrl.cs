using Leexsoft.PrintScreen.Listener;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Leexsoft.PrintScreen
{
    public partial class ScreenCtrl : CtrlBase
    {
        public ScreenCtrl()
        {
            InitializeComponent();
        }

        public override void SetValue(KeyListenerConfig config)
        {
            //下拉框数据初始
            this.comboBox1.SelectedIndexChanged -= this.comboBox1_SelectedIndexChanged;
            this.comboBox1.DataSource = Screen.AllScreens;
            this.comboBox1.SelectedItem = config.WatchScreen;
            this.comboBox1.SelectedIndexChanged += this.comboBox1_SelectedIndexChanged;
            //选定区域初始
            this.SetArea(config);
            base.SetValue(config);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var currentScreen = this.comboBox1.SelectedValue.ToString();
            if (!this.Config.ScreenName.Equals(currentScreen, StringComparison.CurrentCultureIgnoreCase))
            {
                //设置值和当前值不相等则需要重新启动监听
                this.Config.ScreenName = currentScreen;
                this.Config.HasChange = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.button1.Enabled = false;
            using (var overlay = new FormOverlay())
            {
                overlay.CloseEvent += this.OnSetAreaColse;
                overlay.SetAreaFinishEvent += this.OnSetAreaFinish;
                //注意要配合窗体属性的设置才能让窗体显示在不同的显示器上
                overlay.DesktopBounds = this.Config.WatchScreen.Bounds;
                overlay.ShowDialog();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Config.WatchArea = Rectangle.Empty;
            this.Config.HasChange = true;
            this.SetArea(this.Config);
        }

        #region 响应鼠标框选监听区域的事件
        private void OnSetAreaColse()
        {
            this.button1.Enabled = true;
        }

        private void OnSetAreaFinish(Rectangle watchArea)
        {
            if (!this.Config.WatchArea.Equals(watchArea))
            {
                //设置值和当前值不相等则需要重新启动监听
                this.Config.WatchArea = watchArea;
                this.Config.HasChange = true;
                this.SetArea(this.Config);
            }
        }
        #endregion

        private void SetArea(KeyListenerConfig config)
        {
            if (string.IsNullOrEmpty(config.AreaDesc))
            {
                this.label3.Text = string.Empty;
                this.button2.Enabled = false;
            }
            else
            {
                this.label3.Text = string.Format("已设定区域：{0}", config.AreaDesc);
                this.button2.Enabled = true;
            }
        }
    }
}
