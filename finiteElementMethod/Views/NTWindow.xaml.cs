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

namespace finiteElementMethod.Views
{
    /// <summary>
    /// Interaction logic for NTWindow.xaml
    /// </summary>
    public partial class NTWindow : Window
    {
        public NTWindow()
        {
            InitializeComponent();
        }

        public NTWindow(ref List<List<uint>> NT)
        {
            InitializeComponent();
        }
    }
}
