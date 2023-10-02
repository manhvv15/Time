﻿using ChamCong365.NhanVien.KindOfDon.LichLamViec;
using ChamCong365.NhanVien.Propose;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using Microsoft.Win32;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static ChamCong365.OOP.NhanVien.DonDeXuat.XetDuyetVaTheoDoi;
using ChamCong365.OOP;

namespace ChamCong365.NhanVien.KindOfDon
{
    /// <summary>
    /// Interaction logic for DeXuatLuanChuyenCongTac.xaml
    /// </summary>
    public partial class DeXuatLuanChuyenCongTac : UserControl
    {
        MainChamCong Main;
        OOP.Login.LoginCom.Data Dt;
        public class PhongBan1
        {
            public string PhongBan { get; set; }
        }
        public class ChonChucVu
        {
            public string ChucVu { get; set; }
        }
        List<ChonChucVu> listChucVu = new List<ChonChucVu>();
        List<PhongBan1> listPhongBan = new List<PhongBan1>();
        private void LoadPhongBan()
        {

            listPhongBan.Add(new PhongBan1 { PhongBan = "Kỹ Thuật" });
            listPhongBan.Add(new PhongBan1 { PhongBan = "Biên Tập" });
            listPhongBan.Add(new PhongBan1 { PhongBan = "Nhân viên Chính Thức" });
            listPhongBan.Add(new PhongBan1 { PhongBan = "Kinh Doanh" });
            listPhongBan.Add(new PhongBan1 { PhongBan = "Nhóm Phó" });
            listPhongBan.Add(new PhongBan1 { PhongBan = "Đề Án" });
            listPhongBan.Add(new PhongBan1 { PhongBan = "Phòng Seo" });
            listPhongBan.Add(new PhongBan1 { PhongBan = "Phòng Đào Tạo" });
            listPhongBan.Add(new PhongBan1 { PhongBan = "Phòng Sáng Tạo" });

            lsvPhongBan.ItemsSource = listPhongBan;
        }
        private void LoadChucVu()
        {

            listChucVu.Add(new ChonChucVu { ChucVu = "Sinh Viên Thực Tập " });
            listChucVu.Add(new ChonChucVu { ChucVu = "Sinh viên Part Time" });
            listChucVu.Add(new ChonChucVu { ChucVu = "Nhân viên Chính Thức" });
            listChucVu.Add(new ChonChucVu { ChucVu = "Trưởng Nhóm" });
            listChucVu.Add(new ChonChucVu { ChucVu = "Nhóm Phó" });
            listChucVu.Add(new ChonChucVu { ChucVu = "Tổ Trưởng" });
            listChucVu.Add(new ChonChucVu { ChucVu = "Phó Tổ Trưởng" });
            listChucVu.Add(new ChonChucVu { ChucVu = "Trưởng Ban Dự Án" });
            listChucVu.Add(new ChonChucVu { ChucVu = "Phó Ban Dự Án" });
            listChucVu.Add(new ChonChucVu { ChucVu = "Trưởng Phòng" });
            listChucVu.Add(new ChonChucVu { ChucVu = "Phó Trưởng Phòng" });
            listChucVu.Add(new ChonChucVu { ChucVu = "Giám Đốc" });
            listChucVu.Add(new ChonChucVu { ChucVu = "Phó Giám Đốc" });
            listChucVu.Add(new ChonChucVu { ChucVu = "Phó Ban Dự Án" });
            listChucVu.Add(new ChonChucVu { ChucVu = "Tổng Giám Đốc" });
            listChucVu.Add(new ChonChucVu { ChucVu = "Phó Tổng Giám Đốc" });
            listChucVu.Add(new ChonChucVu { ChucVu = "Tổng Giám Đốc Tập Đoàn" });
            listChucVu.Add(new ChonChucVu { ChucVu = "Phó Tổng Giám Đốc Tập Đoàn" });
            listChucVu.Add(new ChonChucVu { ChucVu = "Chủ Tịch Hội Đồng Quản Trị" });
            listChucVu.Add(new ChonChucVu { ChucVu = "Phó Chủ Tịch Hội Đồng Quản Trị" });
            lsvChonChucVu.ItemsSource = listChucVu;
        }
        
        public DeXuatLuanChuyenCongTac(MainChamCong main)
        {
            InitializeComponent();
            Main = main;
            LoadPhongBan();
            LoadChucVu();
            getData();
            getData1();
            getDataPhongBan();
            txtName.Text = main.txbNameAccount.Text;
        }

        private async void getDataPhongBan()
        {
            try
            {
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, "http://210.245.108.202:3000/api/qlc/department/list");
                request.Headers.Add("Authorization", "Bearer " + Properties.Settings.Default.Token);
                var content = new MultipartFormDataContent();
                content.Add(new StringContent("1664"), "com_id");
                request.Content = content;
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                var responseContent = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {

                    // Xử lý phản hồi ở đây
                    ChamCong365.OOP.NhanVien.DonDeXuat.API_PhongBan.PhongBan api = JsonConvert.DeserializeObject<ChamCong365.OOP.NhanVien.DonDeXuat.API_PhongBan.PhongBan>(responseContent);
                    if (api.data.items != null)
                    {
                        lsvPhongBan.ItemsSource = api.data.items;
                        //listCaLamViec = api.data.items;
                        //lsvChonCa1.ItemsSource = api.data.items;
                    }
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi,vui lòng kiểm tra lại! " + ex.Message);
            }
        }
        private void Grid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

            if (borPhongBan.Visibility == Visibility.Collapsed)
            {

                borPhongBan.Visibility = Visibility.Visible;

            }
            else
            {
                borPhongBan.Visibility = Visibility.Collapsed;

            }
        }

        private void lsvPhongBan_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            scrollPhongBan.ScrollToVerticalOffset(scrollPhongBan.VerticalOffset - e.Delta);
        }

        private void borTenChonLoai_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            OOP.NhanVien.DonDeXuat.API_PhongBan.Item d = (sender as Border).DataContext as OOP.NhanVien.DonDeXuat.API_PhongBan.Item;
            if (d != null)
            {
                textPhongBan.Text = d.dep_name;

            }
        }

        private void Grid_MouseLeftButtonUp_1(object sender, MouseButtonEventArgs e)
        {
            if (borChucVu.Visibility == Visibility.Collapsed)
            {

                borChucVu.Visibility = Visibility.Visible;

            }
            else
            {
                borChucVu.Visibility = Visibility.Collapsed;

            }
        }

        private void lsvChucVu_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            scrollChucVu.ScrollToVerticalOffset(scrollChucVu.VerticalOffset - e.Delta);
        }

        private void borTenChonLoai_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {

            ChonChucVu d = (sender as Border).DataContext as ChonChucVu;
            if (d != null)
            {
                textChucVu.Text = d.ChucVu;

            }
        }

        public async void getData()
        {
            try
            {
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, "http://210.245.108.202:3005/api/vanthu/dexuat/showadd"); request.Headers.Add("Authorization", "Bearer " + Properties.Settings.Default.Token);
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                var responseContent = await response.Content.ReadAsStringAsync();


                if (response.IsSuccessStatusCode)
                {

                    // Xử lý phản hồi ở đây
                    ChamCong365.OOP.NhanVien.DonDeXuat.XetDuyetVaTheoDoi.XetDuyetTheoDoi api = JsonConvert.DeserializeObject<ChamCong365.OOP.NhanVien.DonDeXuat.XetDuyetVaTheoDoi.XetDuyetTheoDoi>(responseContent);
                    if (api.data.listUsersDuyet != null)
                    {
                        lsvNguoiXetDuyet.ItemsSource = api.data.listUsersDuyet;
                        listUsersDuyets = api.data.listUsersDuyet;
                        //listUsersTheoDoi = lsvNguoiTheoDoi;
                    }
                }
                else
                {

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi,vui lòng kiểm tra lại! " + ex.Message);
            }
        }
        public void lsvNguoiXetDuyet_PreviewMouseWheel_1(object sender, MouseWheelEventArgs e)
        {
            scrollNguoiXetDuyet.ScrollToVerticalOffset(scrollNguoiXetDuyet.VerticalOffset - e.Delta);
        }
        public void lsvNguoiXetDuyet_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lsvNguoiXetDuyet.SelectedItem != null)
            {
                string selectedUserName = ((ListUsersDuyet)lsvNguoiXetDuyet.SelectedItem).userName;
                if (!ListXet.Any(item => item.userName == selectedUserName))
                {
                    ListUsersDuyet infor = new ListUsersDuyet()
                    {
                        userName = ((ListUsersDuyet)lsvNguoiXetDuyet.SelectedItem).userName,
                        idQLC = ((ListUsersDuyet)lsvNguoiXetDuyet.SelectedItem).idQLC
                    };

                    ListXet.Add(infor);
                    ListXet = ListXet.ToList();
                    listXetDuyet.ItemsSource = ListXet;
                    listXetDuyet.Visibility = Visibility.Visible;
                    borNguoiXetDuyet.Visibility = Visibility.Collapsed;

                }
            }
        }
        public List<ListUsersDuyet> listXet = new List<ListUsersDuyet>();
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public List<ListUsersDuyet> ListXet
        {
            get { return listXet; }
            set
            {
                if (listXet != value)
                {
                    listXet = value;
                    OnPropertyChanged(nameof(ListXet));
                }
            }
        }
        List<OOP.NhanVien.DonDeXuat.XetDuyetVaTheoDoi.ListUsersDuyet> listUsersDuyets = new List<OOP.NhanVien.DonDeXuat.XetDuyetVaTheoDoi.ListUsersDuyet>();

        List<OOP.NhanVien.DonDeXuat.XetDuyetVaTheoDoi.ListUsersDuyet> listUsersDuyetsTimKiem = new List<OOP.NhanVien.DonDeXuat.XetDuyetVaTheoDoi.ListUsersDuyet>();
      //  List<OOP.NhanVien.DonDeXuat.CaLamViec.Item> listCaLamViec = new List<OOP.NhanVien.DonDeXuat.CaLamViec.Item>();
        public void textSearchNguoiXetDuyet_TextChanged(object sender, TextChangedEventArgs e)
        {
            List<OOP.NhanVien.DonDeXuat.XetDuyetVaTheoDoi.ListUsersDuyet> listUsersDuyetTimKiem = new List<OOP.NhanVien.DonDeXuat.XetDuyetVaTheoDoi.ListUsersDuyet>();
            string searchText = textSearchNguoiXetDuyet.Text.ToString().ToLower();
            foreach (var str in listUsersDuyets)
            {
                if (str.userName.ToLower().Contains(searchText))
                {
                    listUsersDuyetTimKiem.Add(str);

                }
            }
            lsvNguoiXetDuyet.ItemsSource = listUsersDuyetTimKiem;

            if (textSearchNguoiXetDuyet.Text == "")
            {
                lsvNguoiXetDuyet.ItemsSource = listUsersDuyets;
            }
        }
        public void xoaAnh_MouseEnter(object sender, MouseEventArgs e)
        {
            SolidColorBrush redBrush = new SolidColorBrush(Colors.DarkGray);
            ((Border)sender).Background = redBrush;

        }
        public void xoaAnh_MouseLeave(object sender, MouseEventArgs e)
        {
            SolidColorBrush redBrush = new SolidColorBrush(Colors.Aqua);
            ((Border)sender).Background = redBrush;
        }
        public void xoaAnh_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ListUsersDuyet index = (ListUsersDuyet)listXetDuyet.SelectedItem;
            if (index != null)
            {
                ListXet.Remove(index);
                listXetDuyet.ClearValue(ItemsControl.ItemsSourceProperty);
                listXetDuyet.ItemsSource = ListXet;
                shouldProcessEvent = false;
            }
            shouldProcessEvent = true;
            if (listXet.Count == 0)
            {
                borNhapNgXetD.Visibility = Visibility.Visible;
                listXetDuyet.Visibility = Visibility.Collapsed;
                textNg.Text = "Nhập người xét duyệt";

            }
        }
        public bool shouldProcessEvent = true;
        public void Grid_MouseLeftButtonUp_3(object sender, MouseButtonEventArgs e)
        {
            if (shouldProcessEvent)
            {
                if (borNguoiXetDuyet.Visibility == Visibility.Collapsed)
                {
                    borNguoiXetDuyet.Visibility = Visibility.Visible;
                    //listXetDuyet.Visibility = Visibility.Visible;

                }
                else
                {
                    borNguoiXetDuyet.Visibility = Visibility.Collapsed;

                }
            }
        }
        List<OOP.NhanVien.DonDeXuat.XetDuyetVaTheoDoi.ListUsersTheoDoi> listUsersTheoDoi1 = new List<OOP.NhanVien.DonDeXuat.XetDuyetVaTheoDoi.ListUsersTheoDoi>();
        private async void getData1()
        {
            try
            {
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, "http://210.245.108.202:3005/api/vanthu/dexuat/showadd");
                request.Headers.Add("Authorization", "Bearer " + Properties.Settings.Default.Token);
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                var responseContent = await response.Content.ReadAsStringAsync();


                if (response.IsSuccessStatusCode)
                {

                    // Xử lý phản hồi ở đây
                    ChamCong365.OOP.NhanVien.DonDeXuat.XetDuyetVaTheoDoi.XetDuyetTheoDoi api = JsonConvert.DeserializeObject<ChamCong365.OOP.NhanVien.DonDeXuat.XetDuyetVaTheoDoi.XetDuyetTheoDoi>(responseContent);
                    if (api.data.listUsersTheoDoi != null)
                    {
                        lsvNguoiTheoDoi.ItemsSource = api.data.listUsersTheoDoi;
                        listUsersTheoDoi1 = api.data.listUsersTheoDoi;
                        //listUsersTheoDoi = lsvNguoiTheoDoi;
                    }
                }
                else
                {

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi,vui lòng kiểm tra lại! " + ex.Message);
            }

        }
        public int idTheoDoi;
        private void borTenChonLoai_MouseLeftButtonDown_2(object sender, MouseButtonEventArgs e)
        {
            ListUsersTheoDoi d = (sender as Border).DataContext as ListUsersTheoDoi;
            if (d != null)
            {
                textNguoiTheoDoi.Text = d.userName;
                d.idQLC = d.idQLC;
                SolidColorBrush redBrush = new SolidColorBrush(Colors.Aqua);
                borTheoDoi.Background = redBrush;
                //idTheoDoi = ((ListUsersTheoDoi)lsvNguoiTheoDoi.SelectedItem).idQLC;
            }
        }
        private void Grid_MouseLeftButtonUp_4(object sender, MouseButtonEventArgs e)
        {
            if (borNguoiTheoDoi.Visibility == Visibility.Collapsed)
            {

                borNguoiTheoDoi.Visibility = Visibility.Visible;

            }
            else
            {
                borNguoiTheoDoi.Visibility = Visibility.Collapsed;

            }
        }
        private void lsvNguoiTheoDoi_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            scrollNguoiTheoDoi.ScrollToVerticalOffset(scrollNguoiTheoDoi.VerticalOffset - e.Delta);
        }
        private void textSearchNguoiTheoDoi_TextChanged(object sender, TextChangedEventArgs e)
        {
            List<OOP.NhanVien.DonDeXuat.XetDuyetVaTheoDoi.ListUsersTheoDoi> listUsersTheoDoi1TimKiem = new List<OOP.NhanVien.DonDeXuat.XetDuyetVaTheoDoi.ListUsersTheoDoi>();
            string searchText = textSearchNguoiTheoDoi.Text.ToString().ToLower();
            foreach (var str in listUsersTheoDoi1)
            {
                if (str.userName.ToLower().Contains(searchText))
                {
                    listUsersTheoDoi1TimKiem.Add(str);

                }
            }
            lsvNguoiTheoDoi.ItemsSource = listUsersTheoDoi1TimKiem;

            if (textSearchNguoiTheoDoi.Text == "")
            {
                lsvNguoiTheoDoi.ItemsSource = listUsersTheoDoi1;
            }
        }

        private void Border_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            listTess uc = new listTess(Main);
            Main.dopBody.Children.Clear();
            object Content = uc.Content;
            uc.Content = null;
            Main.dopBody.Children.Add(Content as UIElement);
        }

        private async void Border_MouseLeftButtonUp_1(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, "http://210.245.108.202:3005/api/vanthu/dexuat/De_Xuat_Luan_Chuyen_Cong_Tac");
                request.Headers.Add("Authorization", "Bearer " + Properties.Settings.Default.Token);
                var content = new MultipartFormDataContent();
                content.Add(new StringContent(textNhapTenDeXuat.Text), "name_dx");
                int id;
                List<StringContent> NgXeDuyet = new List<StringContent>();
                List<string> listString = new List<string>();
                foreach (var item in ListXet)
                {
                    id = item.idQLC;
                    string idString = Convert.ToString(id);

                    listString.Add(idString + ",");

                }
                for (int i = 0; i < listString.Count; i++)
                {
                    if (listString[i].EndsWith(",") && i == listString.Count - 1)
                    {
                        listString[i] = listString[i].Substring(0, listString[i].Length - 1);
                    }
                    //listString = Convert.ToString(listString);
                }
                string idXetDuyet = string.Join("", listString);
                //MessageBox.Show(Convert.ToString(idXetDuyet));
                content.Add(new StringContent(Convert.ToString(idXetDuyet)), "id_user_duyet");
                idTheoDoi = ((ListUsersTheoDoi)lsvNguoiTheoDoi.SelectedItem).idQLC;
                content.Add(new StringContent(Convert.ToString(idTheoDoi)), "id_user_theo_doi");
                content.Add(new StringContent(textNhapLyDo.Text), "ly_do");

                content.Add(new StringContent(textChucVu.Text), "cv_nguoi_lc");
                content.Add(new StringContent(textPhongBan.Text), "pb_nguoi_lc");
                content.Add(new StringContent(textNhapNoiCongTac.Text), "noi_cong_tac");
                content.Add(new StringContent(textNhapNoiChuyenDen.Text), "noi_chuyen_den");
                content.Add(new StreamContent(File.OpenRead(TenTep)), "fileKem", tepDinhKem.Text);
                request.Content = content;
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                var responseContent = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    TaoDeXuatThanhCong uc = new TaoDeXuatThanhCong(Main);
                    //Main.dopBody.Children.Clear();
                    object Content = uc.Content;
                    uc.Content = null;
                    Main.dopBody.Children.Add(Content as UIElement);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        string TenTep = "";
        public void Border_MouseLeftButtonUp_2(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Tất cả các tệp|*.*"; // Lọc tất cả các tệp

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;

                try
                {
                    // Đọc nội dung của tệp bằng File.ReadAllText
                    string fileContent = File.ReadAllText(filePath);
                    //  tepDinhKem.Text = filePath;
                    TenTep = filePath;
                    tepDinhKem.Text = System.IO.Path.GetFileName(filePath); ;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Đã xảy ra lỗi khi đọc tệp: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }
        }
    }
}
