using System;
using System.IO;
using System.Linq;
using System.Windows;
using Lapushok.Pages;

namespace Lapushok
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new ProductListPage());
        }
    }
}
