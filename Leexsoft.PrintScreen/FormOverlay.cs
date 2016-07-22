using System;
using System.Drawing;
using System.Windows.Forms;

namespace Leexsoft.PrintScreen
{
    public partial class FormOverlay : Form
    {
        /// <summary>
        /// 鼠标框选区域
        /// </summary>
        private Rectangle WatchRect { get; set; }
        /// <summary>
        /// 鼠标是否在框选区域
        /// </summary>
        private bool MouseIsDown { get; set; }
        /// <summary>
        /// 鼠标框选区域
        /// </summary>
        private Rectangle MouseRect { get; set; }
        /// <summary>
        /// GDI+对象
        /// </summary>
        private Graphics Graphic { get; set; }

        #region 事件
        public event Action CloseEvent;
        private void OnClose()
        {
            if (CloseEvent != null)
            {
                CloseEvent();
            }
            this.Hide();
            this.Dispose();
        }

        public event Action<Rectangle> SetAreaFinishEvent;
        private void OnSetAreaFinish(Rectangle rect)
        {
            this.OnClose();
            if (SetAreaFinishEvent != null)
            {
                SetAreaFinishEvent(rect);
            }
        }
        #endregion

        public FormOverlay()
        {
            InitializeComponent();
        }

        private void FormOverlay_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.OnClose();
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.OnClose();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.OnSetAreaFinish(this.WatchRect);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (this.Graphic = this.CreateGraphics())
            {
                this.Graphic.Clear(Color.White);
                this.button1.Visible = false;
                this.button2.Visible = false;
            }
        }

        #region 鼠标选择框  
        private void FormOverlay_MouseDown(object sender, MouseEventArgs e)
        {
            //Log4netHelper.Info(string.Format("StartPoint for x:{0}, y:{1}", e.Location.X, e.Location.Y));
            this.Capture = true;
            this.WatchRect = Rectangle.Empty;
            this.MouseIsDown = true;
            this.MouseRect = new Rectangle(e.Location, new Size(0, 0));
            this.Graphic = this.CreateGraphics();
        }

        private void FormOverlay_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.MouseIsDown)
            {
                ResizeToRectangle(e.Location);
            }
        }

        private void FormOverlay_MouseUp(object sender, MouseEventArgs e)
        {
            this.ShowButtons(e.Location);
            this.DrawRectangle();
            this.Capture = false;
            this.WatchRect = this.RectangleToScreen(this.MouseRect);
            this.MouseIsDown = false;
            this.MouseRect = Rectangle.Empty;
            this.Graphic.Dispose();
        }
        #endregion

        #region 画框辅助方法
        private void ResizeToRectangle(Point p)
        {
            this.DrawRectangle();
            var startPoint = this.MouseRect.Location;
            var size = new Size();
            size.Width = p.X - this.MouseRect.Left;
            size.Height = p.Y - this.MouseRect.Top;
            this.MouseRect = new Rectangle(startPoint, size);
            this.DrawRectangle();
        }

        private void DrawRectangle()
        {
            var rect = this.MouseRect;
            this.Graphic.FillRectangle(Brushes.LightSkyBlue, rect);
            this.Graphic.DrawRectangle(Pens.Blue, rect);
            //Log4netHelper.Info(string.Format("Rectangle for left:{0}, top:{1}, width:{2}, height:{3}", rect.Left, rect.Top, rect.Width, rect.Height));
        }

        private void ShowButtons(Point end)
        {
            var x = this.MouseRect.Location.X;
            var y = end.Y + 10;
            this.button1.Location = new Point(x, y);
            this.button1.Visible = true;

            var x1 = x + this.button1.Size.Width + 10;
            this.button2.Location = new Point(x1, y);
            this.button2.Visible = true;
        }
        #endregion
    }
}
