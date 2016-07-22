using Leexsoft.PrintScreen.Listener;
using System;
using System.Windows.Forms;

namespace Leexsoft.PrintScreen
{
    public partial class FolderCtrl : CtrlBase
    {
        public FolderCtrl()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                var currentPath = this.folderBrowserDialog1.SelectedPath;
                this.textBox1.Text = currentPath;
                if (!string.IsNullOrEmpty(currentPath))
                {
                    if (!this.Config.SavePath.Equals(currentPath, StringComparison.CurrentCultureIgnoreCase))
                    {
                        //设置值和当前值不相等则需要重新启动监听
                        this.Config.SavePath = currentPath;
                        this.Config.HasChange = true;
                    }
                }
            }
        }

        public override void SetValue(KeyListenerConfig config)
        {
            this.textBox1.Text = config.SavePath;
            base.SetValue(config);
        }
    }
}
