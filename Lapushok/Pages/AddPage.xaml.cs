using Lapushok.Components;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace Lapushok.Pages
{
    /// <summary>
    /// Логика взаимодействия для AddPage.xaml
    /// </summary>
    public partial class AddPage : Page
    {
        private Product _product;
        private byte[] _imageBytes;
        public AddPage(Product product)
        {
            InitializeComponent();
            _product = product;
            DataContext = _product;
            MinPriceTb.MaxLength = 7;
            AmountOfPeopleTb.MaxLength = 3;
            // Загружаем список типов продуктов и цехов
            ProductTypeCb.ItemsSource = App.db.productType.ToList();
            WorkshopCb.ItemsSource = App.db.Workshop.ToList();
            AmountOfPeopleTb.PreviewTextInput += NumberOnly_PreviewTextInput;
            MinPriceTb.PreviewTextInput += NumberOnly_PreviewTextInput;
        }


        private void NumberOnly_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Разрешаем только цифры
            e.Handled = !Regex.IsMatch(e.Text, "^[0-9]+$");
        }


        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем, заполнены ли все поля
            if (string.IsNullOrWhiteSpace(NameTb.Text) ||
                string.IsNullOrWhiteSpace(MinPriceTb.Text) ||
                string.IsNullOrWhiteSpace(AmountOfPeopleTb.Text) ||
                ProductTypeCb.SelectedItem == null ||
                WorkshopCb.SelectedItem == null)
            {
                MessageBox.Show("Заполните все поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Создаём новый продукт
            Product newProduct = new Product
            {
                Name = NameTb.Text,
                MinPriceForAgent = Convert.ToInt32(MinPriceTb.Text),
                CountPeople = Convert.ToInt32(AmountOfPeopleTb.Text),
                ProductType_id = ((productType)ProductTypeCb.SelectedItem).Id,
                WorshopId = ((Workshop)WorkshopCb.SelectedItem).Id,
                photobit = _imageBytes // Теперь изображение сохранится!
            };

            // Добавляем в базу и сохраняем
            App.db.Product.Add(newProduct);
            App.db.SaveChanges();

            // Выводим MessageBox для проверки
            MessageBox.Show("Продукт успешно добавлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

            // Перенаправляем на ProductListPage и обновляем UI
            if (Application.Current.MainWindow is MainWindow main)
            {
                main.MainFrame.Navigate(new ProductListPage()); // Перезагружаем страницу
            }
        }

        private void UploadImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Изображения (*.jpg;*.png)|*.jpg;*.png"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                _imageBytes = File.ReadAllBytes(openFileDialog.FileName);
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = new MemoryStream(_imageBytes);
                bitmap.EndInit();
                ProductImage.Source = bitmap;
            }
        }


        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
