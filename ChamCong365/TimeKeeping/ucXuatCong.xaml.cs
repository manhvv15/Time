using ChamCong365.OOP.ChamCong;
using ChamCong365.OOP.ChamCong.CaiDatLichLamViec;
using ChamCong365.OOP.ChamCong.XuatCong;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
//using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace ChamCong365.TimeKeeping
{

    public partial class ucXuatCong : UserControl
    {
        BrushConverter bc = new BrushConverter();
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        int com_id = 0;
        int ep_id = 0;
        int dep_id = 0;
        int pageNumber = 1;
        int pageSize = 10;
        int idQLC = 0;
        public ucXuatCong(MainWindow main)
        {
            InitializeComponent();
            this.Main = main;
            com_id = Main.IdAcount;
           
            dpStart.SelectedDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            dpEnd.SelectedDate = DateTime.Today;
            getData();
            cboLoaiNhanVien.ItemsSource = listCheck;   
        }

        private async Task getData()
        {
            
            List<Task> k = new List<Task>()
            {
                getCom().ContinueWith(tt=>this.Dispatcher.Invoke(()=>{
                    if (tt.Result!=null)
                    {
                        listCom=tt.Result.data.items;
                        cboChonCongTy.ItemsSource = listCom;
                        if (listCom!=null&& listCom.Count>0)
                        {
                            cboChonCongTy.SelectedIndex=listCom.Count -1;
                            getDep().ContinueWith(hh=>this.Dispatcher.Invoke(()=>{
                                if (hh.Result!=null)
                                {
                                    listDep=hh.Result.data.items;
                                    listDep.Insert(0,new Item_PhongBan(){ dep_id = -1,dep_name="Phòng ban (tất cả)"});
                                    listDep=listDep.ToList();
                                    cboChonPhongBan.ItemsSource = listDep;
                                    cboChonPhongBan.SelectedIndex = 0;
                                    

                                }
                            }));
                        }
                    }
                })),
            };
            k.ForEach(t =>
            {
                t.ContinueWith(tt =>
                {
                    var ck = new List<bool>();
                    k.ForEach(h => ck.Add(h.IsCompleted));
                    if (!ck.Contains(false))
                    {
                        this.Dispatcher.Invoke(() =>
                        {
                            getEp(EpPage, 500).ContinueWith(ee => this.Dispatcher.Invoke(() =>
                            {
                                if (ee.Result != null)
                                {
                                    listEp = ee.Result.data.items;
                                    cboNhanVien.ItemsSource = listEp;
                                }
                            }));
                            getBangCong().ContinueWith(zz => this.Dispatcher.Invoke(() =>
                            {
                                if (zz.Result.data != null)
                                {
                                    listCong = zz.Result.data.listUser;
                                    foreach (ListUser item in listCong)
                                    {
                                        if (item.avatarUser == "" || item.avatarUser == null)
                                        {
                                            item.avatarUser = "https://chamcong.timviec365.vn/images/ep_logo.png";
                                        }
                                        else
                                        {
                                            item.avatarUser = "https://chamcong.24hpay.vn/upload/employee/" + item.avatarUser;
                                        }
                                    }
                                    dgvXuatCong.ItemsSource = listCong;
                                    pagin.TotalRecords = zz.Result.data.listUser.Count;

                                }  
                            }));
                        });
                    }
                });
            });

        }
        private async Task ExportExcelDetail(List<ExcelData> data, FileInfo file, string startDate = "", string endDate = "")
        {
            if (file.Exists)
            {
                file.Delete();
            }
            using (var package = new ExcelPackage(file))
            {
                var ws = package.Workbook.Worksheets.Add("Bảng công theo tháng");

                ws.Cells["A3"].Offset(0, 0).Value = "Mã NV";
                ws.Cells["A3"].Offset(0, 1).Value = "Tên nhân viên";
                ws.Cells["A3"].Offset(0, 2).Value = "Ngày";
                ws.Cells["A3"].Offset(0, 3).Value = "Thứ";
                ws.Cells["A3"].Offset(0, 4).Value = "Công";
                ws.Cells["A3"].Offset(0, 5).Value = "Muộn(phút)";
                ws.Cells["A3"].Offset(0, 6).Value = "Sớm(phút)";
                ws.Cells["A3"].Offset(0, 7).Value = "Số giờ";
                ws.Cells["A3"].Offset(0, 8).Value = "Công tăng ca";

                int max = 1;
                data.ForEach(dd =>
                {
                    if (dd.lst_time.Count > max) max = dd.lst_time.Count;
                });

                ws.Cells["A1"].Value = "CHI TIẾT CHẤM CÔNG CÔNG TY " ;
                ws.Cells[1, 1, 1, 9 + max].Merge = true;
                ws.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                ws.Row(1).Style.Font.Bold = true;
                ws.Row(1).Style.Font.Size = 13;

                ws.Cells["A2"].Value = string.Format("Từ ngày {0} đến ngày {1}", startDate, endDate);
                ws.Cells[2, 1, 2, 9 + max].Merge = true;
                ws.Row(2).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                ws.Row(2).Style.Font.Bold = true;
                ws.Row(2).Style.Font.Size = 13;

                for (int i = 0; i < max; i++)
                {
                    ws.Cells["A3"].Offset(0, 9 + i).Value = "Thời gian";
                }


                ws.Row(3).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                ws.Row(3).Style.Font.Bold = true;
                ws.Row(3).Style.Font.Size = 13;

                for (int i = 0; i < data.Count; i++)
                {
                    ws.Row(i + 4).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    ws.Row(i + 4).Style.Font.Size = 13;
                }

                for (int i = 0; i < data.Count; i++)
                {
                    ws.Cells[4 + i, 1].Value = data[i].ep_id;
                    ws.Cells[4 + i, 2].Value = data[i].ep_name;
                    ws.Cells[4 + i, 3].Value = data[i].ts_date;
                    ws.Cells[4 + i, 4].Value = data[i].day_of_week;
                    ws.Cells[4 + i, 5].Value = data[i].num_to_calculate;
                    ws.Cells[4 + i, 6].Value = data[i].late;
                    ws.Cells[4 + i, 7].Value = data[i].early;
                    ws.Cells[4 + i, 8].Value = data[i].total_time;
                    ws.Cells[4 + i, 9].Value = data[i].num_overtime;
                    for (int j = 0; j < data[i].lst_time.Count; j++)
                    {
                        ws.Cells[4 + i, 10 + j].Value = DateTime.Parse(data[i].lst_time[j]).ToString("hh:mm tt");
                    }
                }



                ws.Cells["A3:" + ws.Cells.End.Address].AutoFitColumns();
                package.SaveAsync();
            }
        }

        private void ExportExcel(object sender, MouseButtonEventArgs e)
        {
            var list = new List<BangCong>();
            var start = dpStart.SelectedDate.Value.ToString("dd/MM/yyyy");
            var end = dpEnd.SelectedDate.Value.ToString("dd/MM/yyyy");
            bool flag = true;
            getXuatExcel();
        }
        private void getXuatExcel()
        {
            try
            {
                using (WebClient http = new WebClient())
                {
                    var apilink = APIs.API.XuatCong_Api;
                    var pram = new List<string>();

                   
                    http.Headers.Add("Authorization", "Bearer " + Properties.Settings.Default.Token);
                        
                    

                    if (cboChonCongTy.SelectedItem != null) http.QueryString.Add("com_id", cboChonCongTy.SelectedValue.ToString());
                    if (cboChonPhongBan.SelectedItem != null && cboChonPhongBan.SelectedIndex >= 1) http.QueryString.Add("dep_id", cboChonPhongBan.SelectedValue.ToString());
                    if (cboNhanVien.SelectedItem != null) http.QueryString.Add("idQLC", (cboNhanVien.SelectedItem as Item_NhanVien).ep_id.ToString());

                    if (dpStart.SelectedDate != null) http.QueryString.Add("inputOld", dpStart.SelectedDate.Value.ToString("yyyy-MM-dd"));
                    if (dpEnd.SelectedDate != null) http.QueryString.Add("inputNew", dpEnd.SelectedDate.Value.ToString("yyyy-MM-dd"));
                    http.QueryString.Add("pageNumber", "1");
                    http.QueryString.Add("pageSize", "1000000");
                    http.UploadValuesCompleted += (sender, e) =>
                    {
                        API_XuatExcel api = JsonConvert.DeserializeObject<API_XuatExcel>(UnicodeEncoding.UTF8.GetString(e.Result));
                        if (api.list_total != null)
                        {
                            var start = dpStart.SelectedDate.Value.ToString("dd/MM/yyyy");
                            var end = dpEnd.SelectedDate.Value.ToString("dd/MM/yyyy");
                            Microsoft.Win32.SaveFileDialog sv = new Microsoft.Win32.SaveFileDialog();
                            sv.Filter = "Microsoft Excel Worksheet | *.xlsx";
                            sv.FileName = "data";
                            if (sv.ShowDialog() == true)
                            {
                                var data = new List<ExcelData>();
                                int max = 1;
                                List<ExcelData> list1 = api.list_total;
                                list1.ForEach(i =>
                                {
                                    data.Add(new ExcelData
                                    {
                                        ep_id = i.ep_id,
                                        ep_name = i.ep_name,
                                        ts_date = i.ts_date,
                                        day_of_week = i.day_of_week,
                                        num_to_calculate = i.num_to_calculate,
                                        late = i.late,
                                        early = i.early,
                                        total_time = i.total_time,
                                        num_overtime = i.num_overtime,
                                        lst_time = i.lst_time,
                                    });
                                });

                                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                                var file = new FileInfo(sv.FileName);
                                ExportExcelDetail(data, file, start, end).ContinueWith(zz => this.Dispatcher.Invoke(() =>
                                {
                                   
                                        System.Diagnostics.Process.Start(file.FullName);
                                   
                                }));
                                
                            }
                        }
                    };
                    http.UploadValuesTaskAsync(new Uri(apilink), http.QueryString);
                }
            }
            catch (Exception)
            {
            }
        }
        #region Even


        public void LoadDataNull()
        {
            //foreach (ListUser item in listCong)
            //{
            //    if (item.avatarUser == "" || item.avatarUser == null)
            //    {
            //        item.avatarUser = "https://chamcong.timviec365.vn/images/ep_logo.png";
            //    }
            //    else
            //    {
            //        item.avatarUser = "https://chamcong.24hpay.vn/upload/employee/" + item.avatarUser;
            //    }
            //}
          
            //foreach (ListUser item in listCong)
            //{
            //    if (item.phoneTK == "" || item.phoneTK == null)
            //    {
            //        item.phoneTK = "Chưa cập nhật!";
            //    }
            //    else
            //    {
            //        item.phoneTK = item.phoneTK;
            //    }
            //}
            //foreach (ListUser item in listCong)
            //{
            //    if (item.emailContact == "" || item.emailContact == null)
            //    {
            //        item.emailContact = "Chưa cập nhật!";
            //    }
            //    else
            //    {
            //        item.emailContact = item.emailContact;
            //    }
            //}
        }
        public List<string> listCheck { get; set; } = new List<string> { "Nhân viên có chấm công", "Toàn bộ nhân viên" };
        public MainWindow Main { get; set; }
        private int _IsSmallSize = 0;
        public int IsSmallSize
        {
            get { return _IsSmallSize; }
            set
            {
                _IsSmallSize = value;
                OnPropertyChanged("IsSmallSize");
            }
        }
        private int EpPage = 1;
        private string EpKey = "";

      
        #endregion

        #region CallApi

        private List<ListUser> _listCongUser;

        public List<ListUser> listCongUser
        {
            get { return _listCongUser; }
            set { _listCongUser = value; OnPropertyChanged(); }
        }


        private List<ListUser> _listCong;
        public List<ListUser> listCong
        {
            get { return _listCong; }
            set { _listCong = value; OnPropertyChanged(); }
        }
                //var z = dgvXuatCong.Columns.ToList();
                //z.ForEach(c =>
                //{
                //    int index = z.IndexOf(c);
                //    if (index > 2) dgvXuatCong.Columns.Remove(c);
                //});
                //_listCong = value;
                //if (value != null)
                //{
                //    int max = 1;
                //    value.ForEach(x =>
                //    {
                //        if (x.lst_time.Count > max) max = x.lst_time.Count;
                //    });
                //    for (int i = 0; i < max; i++)
                //    {
                //        System.Windows.Controls.DataGridTextColumn col = new System.Windows.Controls.DataGridTextColumn();
                //        col.Header = "Thời gian";
                //        col.ElementStyle = App.Current.Resources["textRowCenter"] as Style;
                //        col.Binding = new Binding() { Path = new PropertyPath("lst_time[" + i.ToString() + "]") };
                //        dgvXuatCong.Columns.Add(col);
                //    }
      
        //lỗi
        private async Task<Root_TimeKeeping> getBangCong(int page = 1)
        {
            try
            {
                HttpClient http = new HttpClient();
                http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Properties.Settings.Default.Token);
              

                var apilink = APIs.API.XuatCong_Api;
                var pram = new List<string>();
                var form = new Dictionary<string, string>();
                if (cboChonCongTy.SelectedItem != null) form.Add("com_id", cboChonCongTy.SelectedValue.ToString());
                if (cboChonPhongBan.SelectedItem != null && cboChonPhongBan.SelectedIndex >= 1) form.Add("dep_id", cboChonPhongBan.SelectedValue.ToString());
                if (cboNhanVien.SelectedItem != null) form.Add("idQLC", (cboNhanVien.SelectedItem as Item_NhanVien).ep_id.ToString());

                if (dpStart.SelectedDate != null) form.Add("inputOld", dpStart.SelectedDate.Value.ToString("yyyy/MM/dd"));
                if (dpEnd.SelectedDate != null) form.Add("inputNew", dpEnd.SelectedDate.Value.ToString("yyyy/MM/dd"));

                if (page <= 0) page = 1;
                var offset = (page - 1) * 20;
                form.Add("pageNumber", page.ToString());
                form.Add("pageSize", "20");

                HttpResponseMessage respon = await http.PostAsync(apilink, new FormUrlEncodedContent(form));
                var check = respon.Content.ReadAsStringAsync().Result;
                Root_TimeKeeping api = JsonConvert.DeserializeObject<Root_TimeKeeping>(respon.Content.ReadAsStringAsync().Result);
                
                if (api.data != null) return api;
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion

        #region NhanVien
        private List<Item_NhanVien> _listEp1;
        public List<Item_NhanVien> listEp1
        {
            get { return _listEp1; }
            set { _listEp1 = value; OnPropertyChanged(); }
        }

        private List<Item_NhanVien> _listEp;
        public List<Item_NhanVien> listEp
        {
            get { return _listEp; }
            set { _listEp = value; OnPropertyChanged(); }
        }
        private async Task<Root_NhanVien> getEp(int page = 1, int length = 20)
        {
            try
            {
                if (cboChonCongTy.SelectedItem != null)
                {
                    HttpClient http = new HttpClient();
                    var apilink = APIs.API.List_ManagerSaff_Api;
                    var pram = new List<string>();
                    Dictionary<string, string> form = new Dictionary<string, string>();
                   
                    
                        
                            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Properties.Settings.Default.Token);
                            if (cboChonCongTy.SelectedItem != null && cboChonCongTy.SelectedIndex >= 1) form.Add("com_id", cboChonCongTy.SelectedValue.ToString());
                            else form.Add("com_id", Main.IdAcount.ToString());
                        
                    

                    if (cboChonPhongBan.SelectedItem != null && cboChonPhongBan.SelectedIndex >= 1) form.Add("id_com", cboChonPhongBan.SelectedValue.ToString());
                    form.Add("pageNumber", page.ToString());
                    form.Add("pageSize", length.ToString());
                    HttpResponseMessage respon = await http.PostAsync(apilink, new FormUrlEncodedContent(form));


                    Root_NhanVien api = JsonConvert.DeserializeObject<Root_NhanVien>(respon.Content.ReadAsStringAsync().Result);
                    //cboNhanVien.ItemsSource = api.data.items;
                    if (api.data != null) return api;
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private void cboEP_ScrollToEnd()
        {
            EpPage++;
            if (listEp == null) listEp = new List<Item_NhanVien>();
            getEp(EpPage).ContinueWith(tt => this.Dispatcher.Invoke(() =>
            {
                if (tt.Result != null)
                {
                    listEp.AddRange(tt.Result.data.items);
                    listEp = listEp.ToList();
                    cboNhanVien.Refresh();
                }
            }));
        }
        private void cboEP_SearchIsNull(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox)
            {
                EpKey = (sender as TextBox).Text;
                EpPage = 1;
                getEp().ContinueWith(tt => this.Dispatcher.Invoke(() =>
                {
                    if (tt.Result != null)
                    {
                        listEp = tt.Result.data.items;
                        cboNhanVien.Refresh();
                    }
                }));
            }
        }
        #endregion

        #region PhongBan

        private List<Item_PhongBan> _listDep;
        public List<Item_PhongBan> listDep
        {
            get { return _listDep; }
            set { _listDep = value; OnPropertyChanged(); }
        }

        private void cboChonPhongBan_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            getEp().ContinueWith(ep => this.Dispatcher.Invoke(() =>
            {
                if (ep.Result != null)
                {
                    listEp = ep.Result.data.items;
                    cboNhanVien.Refresh();
                    cboNhanVien.ItemsSource = listEp;
                }
            }));
            
        }

        private async Task<Root_PhongBan> getDep(int page = 1)
        {
            try
            {
                if (cboChonCongTy.SelectedItem != null)
                {
                    string apilink = APIs.API.List_Room_Api;
                    HttpClient httpClient = new HttpClient();
                    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Properties.Settings.Default.Token);
                       
                    
                    List<Item_PhongBan> list = new List<Item_PhongBan>();
                    Dictionary<string, string> form = new Dictionary<string, string>();
                    form.Add("com_id", cboChonCongTy.SelectedValue.ToString());
                    var respon = await httpClient.PostAsync(apilink, new FormUrlEncodedContent(form));
                    Root_PhongBan api = JsonConvert.DeserializeObject<Root_PhongBan>(respon.Content.ReadAsStringAsync().Result);
                    //cboChonPhongBan.ItemsSource = api.data.items;
                    if (api.data != null)
                    {
                        return api; 
                    }
                    
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion

        #region CongTy
        private List<Item_CtyCon> _listCom;
        public List<Item_CtyCon> listCom
        {
            get { return _listCom; }
            set { _listCom = value; OnPropertyChanged(); }
        }

         private async Task<Root_CongTyCon> getCom(int page = 1)
        {
            try
            {
                string apilink = "http://210.245.108.202:3000/api/qlc/company/child/list";
                HttpClient httpClient = new HttpClient();
                Dictionary<string, string> form = new Dictionary<string, string>();
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Properties.Settings.Default.Token);
                form.Add("com_id",Main.IdAcount.ToString());
                   
                
                int i = 0;
                List<Item_CtyCon> list = new List<Item_CtyCon>();
                
                var respon = await httpClient.PostAsync(apilink,new FormUrlEncodedContent(form));
                Root_CongTyCon ts = JsonConvert.DeserializeObject<Root_CongTyCon>(respon.Content.ReadAsStringAsync().Result);
                if (ts != null)
                {
                    if (ts.data != null && ts.data.items != null)
                    {
                        ts.data.items.RemoveAll(item => item == null);
                        list = ts.data.items;
                        list.Add(new Item_CtyCon() 
                        { 
                            com_id = Main.IdAcount,
                            com_name = Main.txbNameAccount.Text,
                            
                        });
                        //cboChonCongTy.ItemsSource = ts.data.items;
                    }
                }
                if (ts.data != null) return ts;
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
        private void cboChonCongTy_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cboChonCongTy.SelectedItem != null)
            {
                getDep().ContinueWith(tt => this.Dispatcher.Invoke(() =>
                {
                    if (tt.Result != null)
                    {
                        listDep = tt.Result.data.items;
                        listDep.Insert(0, new Item_PhongBan() { dep_id = -1, dep_name = "Phòng ban (tất cả)" });
                        listDep = listDep.ToList();
                        cboChonPhongBan.ItemsSource = listDep;
                        cboChonPhongBan.SelectedIndex = 0;
                       

                        getEp().ContinueWith(zz => this.Dispatcher.Invoke(() =>
                        {
                            if (zz.Result != null)
                            {
                                listEp = zz.Result.data.items;
                                cboNhanVien.Refresh();
                            }
                        }));
                    }
                }));
            }
        }

        #endregion



        private void pagin_SelectionChange(object sender, SelectionChangedEventArgs e)
        {
            getBangCong(pagin.SelectedPage).ContinueWith(tt => this.Dispatcher.Invoke(() =>
            {
                if (tt.Result.data != null)
                {
                    listCong = tt.Result.data.listUser;
               
                }
            }));
        }

  

        private void LocNhanVienChamCong(object sender, MouseButtonEventArgs e)
        {
            getBangCong().ContinueWith(tt => this.Dispatcher.Invoke(() =>
            {
                if (tt.Result.data != null)
                {
                    listCong = tt.Result.data.listUser;
                    dgvXuatCong.ItemsSource = listCong;
                    pagin.TotalRecords = tt.Result.data.listUser.Count();
                }
            }));
        }

     

       
    }
}
