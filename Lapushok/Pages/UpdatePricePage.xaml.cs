using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using Lapushok.Components;

namespace Lapushok.Pages
{
    public partial class UpdatePricePage : Page
    {
        private List<Product> selectedProducts = new List<Product>();

        public UpdatePricePage()
        {
            InitializeComponent();
            LoadProducts();
        }

        private void LoadProducts()
        {
            ProductListBox.ItemsSource = App.db.Product.ToList();
        }

        private void UpdatePrice_Click(object sender, RoutedEventArgs e)
        {
            if (!decimal.TryParse(PriceTextBox.Text, out decimal newPrice))
            {
                MessageBox.Show("Введите корректную числовую цену!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            selectedProducts = ProductListBox.SelectedItems.Cast<Product>().ToList();

            if (selectedProducts.Count == 0)
            {
                MessageBox.Show("Выберите хотя бы один продукт!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            foreach (var product in selectedProducts)
            {
                product.MinPriceForAgent = Convert.ToInt32(newPrice);
            }

            App.db.SaveChanges();
            MessageBox.Show("Стоимость успешно обновлена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            NavigationService.GoBack();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void PriceTextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !Regex.IsMatch(e.Text, "^[0-9]*\\.?[0-9]+$"); // Только цифры и точка
        }
    }
}
