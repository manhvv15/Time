using ChamCong365.OOP.ChamCong.CaiDatBaoMatWifi;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace ChamCong365.Popups.ChamCong.CaiDatWifi
{
    /// <summary>
    /// Interaction logic for ucCreateAddressIP.xaml
    /// </summary>
    public partial class ucThemDiaChiIP : UserControl
    {
        MainWindow Main;
        private ListIP listIP;
        ucDanhSachIP ucDanhSachIP;
        public ucThemDiaChiIP(MainWindow main, ListIP listIP, ucDanhSachIP ucDanhSachIP)
        {
            InitializeComponent();
            Main = main;
            this.listIP = listIP;
            this.ucDanhSachIP = ucDanhSachIP;
           
        }
        
        public async void AddIP()
        {
            try
            {
                //var options = new RestClientOptions("http://210.245.108.202:3000")
                //{
                //    MaxTimeout = -1,
                //};
                //var client = new RestClient(options);
                //var request = new RestRequest("/api/qlc/SetIp/create", Method.Post);
                //request.AddHeader("Content-Type", "application/json");
                //request.AddHeader("Authorization", "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJkYXRhIjp7Il9pZCI6MzgwOTg5LCJpZFRpbVZpZWMzNjUiOjIwMjU4NSwiaWRRTEMiOjE3NjMsImlkUmFvTmhhbmgzNjUiOjAsImVtYWlsIjoiZHVvbmdoaWVwaXQxQGdtYWlsLmNvbSIsInBob25lVEsiOiIiLCJjcmVhdGVkQXQiOjE2MDA2NTg0NzgsInR5cGUiOjEsImNvbV9pZCI6MTc2MywidXNlck5hbWUiOiJDw7RuZyBUeSBUTkhIIEggTSBMIFBwbyJ9LCJpYXQiOjE2OTM3MTQzMDgsImV4cCI6MTY5MzgwMDcwOH0.GGE7-rHXzvpek7y0Sh07kDSFFWGfTcpvMWr-54VM4a8");
                //var body = @"[{""ip_access"": tb_DiaChiIP.Text,""from_site"": "{tb_TenIP}"}]";
                //request.AddStringBody(body, RestSharp.DataFormat.Json);
                //RestResponse response = await client.ExecuteAsync(request);
                //Console.WriteLine(response.Content);
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, APIs.API.create_ip_api);
                request.Headers.Add("Authorization", "Bearer " + Properties.Settings.Default.Token);
                var content = new MultipartFormDataContent();
                content.Add(new StringContent(tb_TenIP.Text), "from_site");
                content.Add(new StringContent(tb_DiaChiIP.Text), "ip_access");
                request.Content = content;
                var response = await client.SendAsync(request);
                if (response.EnsureSuccessStatusCode().IsSuccessStatusCode)
                {
                    var responsIP = await response.Content.ReadAsStringAsync();
                    RootIp loadListWifi = JsonConvert.DeserializeObject<RootIp>(responsIP);
                    this.Visibility = Visibility.Collapsed;
                    ucDanhSachIP.LoadListIP();
                }
                else
                {

                }
            }
            catch (System.Exception)
            {

               
            }
        }

        private bool ValidateAddForm()
        {
            if (String.IsNullOrEmpty(tb_TenIP.Text))
            {
                txtValidateNameIP.Text = "Tên wifi không được để trống!" as string;
                return false;
            }

            else if (String.IsNullOrEmpty(tb_DiaChiIP.Text))
            {
                txtValidateAddressIP.Text = "Địa chỉ Mac không được để trống!" as string;
                return false;
            }


            return true;
        }

        private void btnAddWifi_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (ValidateAddForm())
            {
                AddIP();
            }
        }
        private void ExitCreateWifi_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
        }

        private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
        }

        private void Rectangle_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            this.Visibility= Visibility.Collapsed;
        }

       
    }
}
