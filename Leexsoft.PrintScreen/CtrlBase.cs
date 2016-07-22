using Leexsoft.PrintScreen.Listener;
using System.Windows.Forms;

namespace Leexsoft.PrintScreen
{
    public partial class CtrlBase : UserControl
    {
        internal KeyListenerConfig Config { get; set; }

        public CtrlBase()
        {
            InitializeComponent();
        }

        public virtual void SetValue(KeyListenerConfig config)
        {
            this.Config = config;
        }
    }
}
