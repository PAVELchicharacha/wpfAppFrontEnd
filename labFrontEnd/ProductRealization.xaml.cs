using labFrontEnd.model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reflection.Metadata;
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

namespace labFrontEnd
{
    /// <summary>
    /// Логика взаимодействия для ProductRealization.xaml
    /// </summary>
    public partial class ProductRealization : Window
    {
        private HttpClient _httpProduct;
        private Product? product;
        private autorise auto;
        private string token;
        public ProductRealization ( string token, autorise a )
        {
            InitializeComponent();
            _httpProduct = new HttpClient();
            _httpProduct.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);//возможна ошибка в названии\
            auto = a;
            this.token = token;
            Task.Run(() => Load());
        }
        private async Task Load ()
        {
            List<Product>? list = await _httpProduct.GetFromJsonAsync<List<Product>>("http://localhost:24734/api/Product");
            //for
            Dispatcher.Invoke(() =>
            {
                ProductList.ItemsSource = null;
                ProductList.Items.Clear();
                ProductList.ItemsSource = list;
            });
        }
        private async Task Save ()
        {
            Product product = new Product
            {
                //Id = int.Parse(ProductID.Text),
                SaleDate = DateTime.Parse(PoductDataSale.Text),
                Quantity = int.Parse(Quantity.Text),
                ProductCoast = int.Parse(ProductCoast.Text),
                ProductSales = int.Parse(ProductSalesS.Text)

            };
            JsonContent content = JsonContent.Create(product);
            using var response = await _httpProduct.PostAsync("http://localhost:24734/api/Product", content);
            string responseText = await response.Content.ReadAsStringAsync();
            await Load();
        }
        
        private async Task Delite ()
        {
            using var response = await _httpProduct.DeleteAsync("http://localhost:24734/api/Product" + product.Id);
            string responseText = await response.Content.ReadAsStringAsync();
            await Load();
        }
        private async Task Edit ()
        {
            product!.Id = int.Parse(ProductSalesS?.Text);
            product!.SaleDate = DateTime.Parse(PoductDataSale.Text);
            product!.Quantity = int.Parse(Quantity.Text);
            product!.ProductCoast = int.Parse(ProductCoast.Text);
            JsonContent content = JsonContent.Create(product);
            using var response = await _httpProduct.PutAsync("http://localhost:24734/api/product/", content);
            string responseText = await response.Content.ReadAsStringAsync();
        }


        private async void Button_Click_1 ( object sender, RoutedEventArgs e )//edit
        {
            await Edit();
        }
        private async void Button_Click_2 ( object sender, RoutedEventArgs e )//delit
        {
            await Delite();
        }
        private async void Button_Click ( object sender, RoutedEventArgs e )//save
        {
            await Save();
        }
        private void Product_SelectionChanged ( object sender, SelectionChangedEventArgs e )
        {
            product = ProductList.SelectedItem as Product;
            //  ProductID.Text = ProductID.Text;
            PoductDataSale.Text = PoductDataSale.Text;
            product!.Quantity = int.Parse(Quantity.Text);
            product!.ProductCoast = int.Parse(ProductCoast.Text);
            product!.ProductSales = int.Parse(ProductSalesS.Text);
        }

        private void Window_Closed ( object sender, EventArgs e )
        {
            auto.Close();
        }

        private void Button_Click_3 ( object sender, RoutedEventArgs e )
        {
            main window = new main(token, auto);
            window.Show();
        }
    }
}
