using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Lapushok.Components;

namespace Lapushok.Pages
{
    public partial class EditPage : Page
    {
        private Product _product;

        public EditPage(Product product)
        {
            InitializeComponent();
            _product = product;
            DataContext = _product;

            MinPriceTb.MaxLength = 7;
            AmountOfPeopleTb.MaxLength = 3;

            // Загружаем список типов продуктов и цехов
            ProductTypeCb.ItemsSource = App.db.productType.ToList();
            WorkshopCb.ItemsSource = App.db.Workshop.ToList();

            // Ограничение на ввод только цифр
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
            // Сохранение данных в базе
            App.db.SaveChanges();
            NavigationService.GoBack();
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            // Переход назад
            NavigationService.GoBack();
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            // Подтверждение удаления
            if (MessageBox.Show("Вы уверены, что хотите удалить этот продукт?", "Удаление", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                // Удаляем продукт из базы
                App.db.Product.Remove(_product);
                App.db.SaveChanges();

                // Перенаправляем на ProductListPage и обновляем UI
                if (Application.Current.MainWindow is MainWindow main)
                {
                    main.MainFrame.Navigate(new ProductListPage()); // Перезагружаем страницу
                }
            }
        }


    }
}
