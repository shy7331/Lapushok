using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Lapushok.Components;

namespace Lapushok.Pages
{
    public partial class ProductListPage : Page
    {
        private List<Product> allProducts;
        private List<Product> filteredProducts;
        private int currentPage = 1;
        private int itemsPerPage = 20;
        private string selectedSort = "NameAsc";
        private string selectedFilter = "All";
        private string searchQuery = "";

        public ProductListPage()
        {
            InitializeComponent();
            LoadProducts();
            LoadFilterOptions();
        }

        public void LoadProducts()
        {
            allProducts = App.db.Product.ToList();

            if (allProducts == null)
            {
                MessageBox.Show("Ошибка: allProducts == null");
                return;
            }
            if (allProducts.Count == 0)
            {
                MessageBox.Show("В базе нет продуктов!");
            }

            filteredProducts = allProducts.ToList(); // Убедимся, что список инициализирован
            UpdateProductList();
        }


        private void OpenEditPage(Product product)
        {
            if (NavigationService != null)
            {
                NavigationService.Navigate(new EditPage(product));
            }
            else
            {
                MainWindow main = (MainWindow)Application.Current.MainWindow;
                main.MainFrame.Navigate(new EditPage(product));
            }
        }


        private void LoadFilterOptions()
        {
            // Получаем список типов из базы
            var types = App.db.productType.ToList();

            if (types == null || types.Count == 0)
            {
                MessageBox.Show("Ошибка: В базе нет типов продуктов!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Создаем новый список и добавляем "Все типы"
            var allTypes = new List<productType>
    {
        new productType { Id = 0, Name = "Все типы" }
    };
            allTypes.AddRange(types); // Добавляем остальные типы из БД

            // Привязываем данные к ComboBox
            FilterComboBox.ItemsSource = allTypes;
            FilterComboBox.SelectedIndex = 0; // Выбираем "Все типы" по умолчанию
        }






        private void ApplyFilters()
        {
            if (allProducts == null)
                return;

            filteredProducts = allProducts
                .Where(p =>
                    // Фильтр по типу продукта (ID)
                    (selectedFilter == "All" || (p.ProductType_id.ToString() == selectedFilter)) &&

                    // Фильтр по поиску
                    (string.IsNullOrEmpty(searchQuery) ||
                     (p.Name != null && p.Name.ToLower().Contains(searchQuery)) ||
                     (p.Description != null && p.Description.ToLower().Contains(searchQuery))))
                .ToList();

            
            ApplySorting(); // Применяем сортировку
        }


        private void ApplySorting()
        {
            switch (selectedSort)
            {
                case "NameAsc":
                    filteredProducts = filteredProducts.OrderBy(p => p.Name).ToList();
                    break;
                case "NameDesc":
                    filteredProducts = filteredProducts.OrderByDescending(p => p.Name).ToList();
                    break;
                case "WorkshopAsc":
                    filteredProducts = filteredProducts.OrderBy(p => p.WorshopId).ToList();
                    break;
                case "WorkshopDesc":
                    filteredProducts = filteredProducts.OrderByDescending(p => p.WorshopId).ToList();
                    break;
                case "PriceAsc":
                    filteredProducts = filteredProducts.OrderBy(p => p.MinPriceForAgent).ToList();
                    break;
                case "PriceDesc":
                    filteredProducts = filteredProducts.OrderByDescending(p => p.MinPriceForAgent).ToList();
                    break;
            }

            UpdateProductList();
        }

        private void UpdateProductList()
        {
            var items = filteredProducts.Skip((currentPage - 1) * itemsPerPage).Take(itemsPerPage).ToList();
            ProductList.Items.Clear();

            foreach (var product in items)
            {
                ProductList.Items.Add(new ProductListUserControl(product));
            }

            PageNumberText.Text = $"{currentPage} / {Math.Max(1, (filteredProducts.Count + itemsPerPage - 1) / itemsPerPage)}";
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            searchQuery = SearchTextBox.Text.ToLower();
            ApplyFilters();
        }

        private void SortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedSort = ((ComboBoxItem)SortComboBox.SelectedItem).Tag.ToString();
            ApplySorting();
        }

        private void FilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FilterComboBox.SelectedItem is productType selectedType)
            {
                // Если выбран "Все типы", то фильтр не применяется
                if (selectedType.Id == 0)
                {
                    selectedFilter = "All";
                }
                else
                {
                    selectedFilter = selectedType.Id.ToString();
                }

                ApplyFilters();
            }
        }



        private void NextPage_Click(object sender, RoutedEventArgs e)
        {
            if (filteredProducts == null || filteredProducts.Count == 0)
            {
                MessageBox.Show("Нет данных для перехода!");
                return;
            }

            int totalPages = (int)Math.Ceiling((double)filteredProducts.Count / itemsPerPage);
            if (currentPage < totalPages)
            {
                currentPage++;
                UpdateProductList();
            }
        }

        private void PrevPage_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage > 1)
            {
                currentPage--;
                UpdateProductList();
            }
        }


        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
                NavigationService.Navigate(new AddPage(new Product()));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new UpdatePricePage());
        }
    }
}

