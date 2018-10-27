using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ProcessManager
{
    /// <summary>
    /// Interaction logic for Intro.xaml
    /// </summary>
    public partial class Intro : Window
    {
        public Intro()
        {
            InitializeComponent();
        }

        private void DoubleAnimationUsingKeyFrames_Completed(object sender, EventArgs e)
        {
            Thread.Sleep(1000);
            var mainWindow = new MainWindow {Owner = this};
            mainWindow.Show();
            mainWindow.Closed += MainWindow_Closed;
            this.Hide();
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
           this.Close();
        }
    }
}
