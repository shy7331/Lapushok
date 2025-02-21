using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using Lapushok.Components;

namespace Lapushok.Pages
{
    public partial class ProductListUserControl : UserControl
    {
        private Product _product;  // Добавляем приватное поле
        public event Action<Product> OnEditRequested;
        public ProductListUserControl(Product product)
        {
            InitializeComponent();
            _product = product;  // Сохраняем переданный продукт
            DataContext = _product;
            DataContext = new ProductViewModel(_product);
        }

        private void EditBtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (Application.Current.MainWindow is MainWindow main)
            {
                main.MainFrame.Navigate(new EditPage(_product));
            }
        }

        public class ProductViewModel
        {
            public Product Product { get; set; }
            public string Name => Product.Name;
            public string allmaterials => Product.allmaterials;
            public decimal MinPriceForAgent => Convert.ToInt64(Product.MinPriceForAgent);
            public byte[] photobit => Product.photobit;
            public productType ProductType => Product.productType;
            public int Id => Product.Id;

            // Определяем цвет фона в зависимости от DaysForProduction
            public Brush ProductBackground
            {
                get
                {
                    // Получаем последний заказ для данного продукта
                    var lastOrder = App.db.AgentOrder
                        .Where(order => order.ProductId == Product.Id)
                        .OrderByDescending(order => order.Id) // Берем последний заказ
                        .FirstOrDefault();

                    if (lastOrder != null && lastOrder.DaysForProduction > 30)
                    {
                        return new SolidColorBrush(Colors.LightCoral); // Светло-красный
                    }

                    return new SolidColorBrush(Colors.White); // Обычный белый
                }
            }

            public ProductViewModel(Product product)
            {
                Product = product;
            }
        }
    }
}

