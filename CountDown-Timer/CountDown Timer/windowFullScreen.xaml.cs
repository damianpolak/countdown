using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CountDown_Timer
{
    /// <summary>
    /// Interaction logic for windowFullScreen.xaml
    /// </summary>
    public partial class windowFullScreen
    {

        private bool bDragMoveIsEnable;
        public event EventHandler Disactive;

        public windowFullScreen()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(Window_SizeChanged);
            ChangeResizeMode();
        }

        protected void WhenDisactive()
        {
            if(this.Disactive != null)
            {
                this.Disactive(this, EventArgs.Empty);
            }
        }

        public void ChangeResizeMode()
        {

            if (this.ResizeMode == ResizeMode.CanResize)
            {
                this.ResizeMode = ResizeMode.NoResize;
                bDragMoveIsEnable = false;
                this.Topmost = true;
            }
            else
            {
                this.ResizeMode = ResizeMode.CanResize;
                bDragMoveIsEnable = true;
                this.Topmost = false;

            }

        }

        private void lbMain_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Size newSize = e.NewSize;
            Size prevSize = e.PreviousSize;
            double l = newSize.Width / prevSize.Width;
            if (l != double.PositiveInfinity)
            {
                lbMain.FontSize = lbMain.FontSize * l;
            }
        }

        private void Window_Closing_1(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private void lbMain_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ChangeResizeMode();

        }

        private void lbMain_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (bDragMoveIsEnable == true)
            {
                this.DragMove();
            }

        }

        private void Window_StateChanged_1(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                bDragMoveIsEnable = false;
                this.ResizeMode = ResizeMode.NoResize;
                this.Topmost = true;
            }
        }

        private void Window_SizeChanged(object sender, RoutedEventArgs e)
        {
            this.WhenDisactive();
        }

        private void Window_LocationChanged_1(object sender, EventArgs e)
        {
            this.WhenDisactive();
        }
    }
}
