using finiteElementMethod.Models;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using finiteElementMethod.Views;

namespace finiteElementMethod
{
    public partial class MainWindow : Window
    {
        private ExObject obj;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void GenNet_Click(object sender, RoutedEventArgs e)
        {
            Solve.IsEnabled = true;

            double width = WidthSlider.Value;
            double lenght = LenghtSlider.Value;
            double height = HeightSlider.Value;
            int wS = int.Parse(WidthSlices.Text);
            int lS = int.Parse(LenghtSlices.Text);
            int hS = int.Parse(HeightSlices.Text);
            double force = PressureSlider.Value;
            int T = PressureComboBox.SelectedIndex;
            double Pausson = Double.Parse(Puasson.Text);
            obj = new ExObject(width, lenght, height, wS, lS, hS, Pausson, force, T);
            DataContext = new ExObjectView(obj);

            NQP.Text = "Nodes quantity: " + obj.AKT.Count;
            NEL.Text = "Finite elements quantity: " + obj.nel;
        }

        private void Solve_Click(object sender, RoutedEventArgs e)
        {
            FEM fem = new FEM(obj);
            fem.RunSimulation();
            DataContext = new ExObjectView(fem.DefformatedObject, obj.Width, obj.Length, obj.Height);
        }
    }
}
