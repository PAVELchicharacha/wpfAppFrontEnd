using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Net.Http;
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
using labFrontEnd.model;

namespace labFrontEnd
{
    /// <summary>
    /// Логика взаимодействия для main.xaml
    /// </summary>
    public partial class main : Window
    {
        private HttpClient httpClient;
        private string? token;
        private Response resp;
        private autorise? autorise=null;

        public main(string response, autorise window)
        {
            InitializeComponent();
            this.autorise = window;
            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + response);
            token = response;
            Task.Run(() => Load());
        }

        private async Task Load()
        {
            List<Precelist>? list = await httpClient.GetFromJsonAsync<List<Precelist>>("http://localhost:24734/api/Pricelist");

            Dispatcher.Invoke(() =>
            {
                ListProducts.ItemsSource = null;
                ListProducts.Items.Clear();
                ListProducts.ItemsSource = list;
            });
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            autorise!.Close();
        }
      

        private async void Button_Click(object sender, RoutedEventArgs e)//save
        {
            Precelist p = new Precelist()
            {
                Coast = double.Parse(Price.Text),
                Name=NameProduct.Text
            };
            JsonContent content = JsonContent.Create(p);
            using var response = await httpClient.PostAsync("http://localhost:24734/api/PriceList", content);
            string responseText = await response.Content.ReadAsStringAsync();
            await Load();
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)//delit
        {
            Precelist? st = ListProducts.SelectedItem as Precelist;
            JsonContent content = JsonContent.Create(st);
            using var response = await httpClient.DeleteAsync("http://localhost:24734/api/PriceList/" + st!.Id);
            string responseText = await response.Content.ReadAsStringAsync();
            await Load();
        }

        private void ListProducts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Precelist? st = ListProducts.SelectedItem as Precelist;
            NameProduct.Text = st?.Name;
            Price.Text = st?.Coast.ToString();
        }

        private async void Button_Click_2(object sender, RoutedEventArgs e)//edit
        {
            Precelist? st = ListProducts.SelectedItem as Precelist;
            st.Name = NameProduct.Text;
            st.Coast = double.Parse(Price.Text);
            JsonContent content = JsonContent.Create(st);
            using var response = await httpClient.PutAsync("http://localhost:24734/api/PriceList", content);
            string responseText = await response.Content.ReadAsStringAsync();
            await Load();
        }
    }
}
