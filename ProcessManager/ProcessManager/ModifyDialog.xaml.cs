using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ProcessManager.ViewModel;

namespace ProcessManager
{
    /// <summary>
    /// Interaction logic for ModifyDialog.xaml
    /// </summary>
    public partial class ModifyDialog : Window
    {
        public ModifyDialog(ProcessViewModel process)
        {
            this.DataContext = process;
            InitializeComponent();
        }
    }
}
