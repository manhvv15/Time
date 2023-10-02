﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Device.Location;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Web.UI.WebControls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using AForge.Imaging.Filters;
using AForge.Video;
using AForge.Video.DirectShow;
using ChamCong365.OOP.ChamCong.CauHinhChamCong;
using ChamCong365.OOP.ChamCong.ChamCongKhuonMat;
using ChamCong365.Popup.ChamCong.ChamCongKhuonMat;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using Newtonsoft.Json;
using static ChamCong365.Popup.ChamCong.ChamCongKhuonMat.Error_NhanDien;

namespace ChamCong365.TimeKeeping
{
    /// <summary>
    /// Interaction logic for ChamCong_Main.xaml
    /// </summary>
    public partial class ChamCong_Main : Page, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public MainWindow Main { get; set; }
        /*EditType type = EditType.DiemDanh*/
        //Type = type;
        public ChamCong_Main(MainWindow main)
        {
            try
            {
                InitializeComponent();
                this.DataContext = this;
                Main = main;
                if (IsWebCamInUse())
                {
                    var popNhanDien = new Error_NhanDien(this, Main);
                    popNhanDien.Message = "Thiết bị ghi hình đang được sử dụng bởi một ứng dụng khác, hãy thử tắt các thiết bị phần mềm đang sử dụng thiết bị ghi hình và thử chấm công lại";
                    popNhanDien.Message2 = "";
                    Popup.NavigationService.Navigate(popNhanDien);
                    PopupChamCong.Visibility = Visibility.Visible;
                }   
                 getComDetail().ContinueWith(tt => this.Dispatcher.Invoke(() =>
                 {
                    if (tt.Result != null)
                    {
                        switch (main.Type)
                        {
                            case 1:
                                if (tt.Result.data.data.type_timekeeping.Contains("4") == false)
                                {
                                    block = true;
                                    if (cam != null && cam.IsRunning) cam.Stop();
                                    if (timer != null) timer.Stop();
                                    var y = new Popup.ChamCong.Comon.Notify1();
                                    y.Type = ChamCong365.Popup.ChamCong.Comon.Notify1.NotifyType.Error;
                                    y.Message = "Chức năng này đang tạm khoá!<br>Vui lòng liên hệ nhân sự để được hướng dẫn thêm";
                                    Popup.NavigationService.Navigate(y);
                                    PopupChamCong.Visibility = Visibility.Visible;
                                }
                                break;
                            case 2:
                                if (tt.Result.data.data.type_timekeeping.Contains("3") == false)
                                {
                                    block = true;
                                    if (cam != null && cam.IsRunning) cam.Stop();
                                    if (timer != null) timer.Stop();
                                    var y = new ChamCong365.Popup.ChamCong.Comon.Notify1();
                                    y.Type = ChamCong365.Popup.ChamCong.Comon.Notify1.NotifyType.Error;
                                    y.Message = "Chức năng này đang tạm khoá!<br>Vui lòng liên hệ nhân sự để được hướng dẫn thêm";
                                    Popup.NavigationService.Navigate(y);
                                    PopupChamCong.Visibility = Visibility.Visible;
                                }
                                break;
                            default:
                                break;
                        }
                    }
                 }));
                 startCapture(false);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        public bool IsWebCamInUse()
        {
            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\CapabilityAccessManager\ConsentStore\webcam\NonPackaged"))
                {
                    if (key != null && key.GetSubKeyNames() != null)
                        foreach (var subKeyName in key.GetSubKeyNames())
                        {
                            using (var subKey = key.OpenSubKey(subKeyName))
                            {
                                if (subKey.GetValueNames().Contains("LastUsedTimeStop"))
                                {
                                    var endTime = subKey.GetValue("LastUsedTimeStop") is long ? (long)subKey.GetValue("LastUsedTimeStop") : -1;
                                    if (endTime <= 0)
                                    {
                                        return true;
                                    }
                                    else
                                    {
                                        return false;
                                    }
                                }
                            }
                        }
                    else return false;
                }
            }
            catch (Exception exception)
            {
                System.Windows.MessageBox.Show(exception.Message);
            }
            return false;
        }
        private async Task<API_Com_ChiTiet> getComDetail()
        {
            try
            {
                string apilink = "http://210.245.108.202:3000/api/qlc/company/info";
                HttpClient httpClient = new HttpClient();
                Dictionary<string, string> form = new Dictionary<string, string>();

                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Properties.Settings.Default.Token);
                form.Add("com_id", Main.IdAcount.ToString());

                int i = 0;
                List<ChildCompany> list = new List<ChildCompany>();

                var respon = await httpClient.PostAsync(apilink, new FormUrlEncodedContent(form));
                API_Com_ChiTiet api = JsonConvert.DeserializeObject<API_Com_ChiTiet>(respon.Content.ReadAsStringAsync().Result);
                if (api.data != null) return api;
                return null;
            }
            catch (Exception ex)
            {

                var x = new ChamCong365.Popup.ChamCong.Comon.Notify1();
                x.Type = ChamCong365.Popup.ChamCong.Comon.Notify1.NotifyType.Error;
                x.Message = ex.Message;
                return null;
            }
        }
        private bool block = false;
        private void Watcher_StatusChanged(object sender, GeoPositionStatusChangedEventArgs e)
        {
            try
            {
                if (e.Status == GeoPositionStatus.Ready)
                {
                    // Display the latitude and longitude.  

                    if (watcher.Position.Location.IsUnknown)
                    {
                        latitude = "0";
                        longitute = "0";
                    }
                    else
                    {
                        latitude = watcher.Position.Location.Latitude.ToString();
                        longitute = watcher.Position.Location.Longitude.ToString();
                    }
                }
                else
                {
                    latitude = "0";
                    longitute = "0";
                }
            }
            catch (Exception)
            {
                latitude = "0";
                longitute = "0";
            }
        }

        private void Start_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            startChamCong();
        }

        public async void startChamCong()
        {
            PopupChamCong.Visibility = Visibility.Collapsed;
            {
                try
                {
                    if (cam == null)
                    {
                        cam = new VideoCaptureDevice(divices[0].MonikerString);
                        cam.NewFrame += new NewFrameEventHandler(Cam_NewFrame);
                    }
                    if (cam != null && !cam.IsRunning) cam.Start();
                    tick = 5;
                    if (timer != null)
                    {
                        timer.Stop();
                    }
                    timer = new DispatcherTimer();
                    PageChamCongStart.Visibility = Visibility.Hidden;
                    PageChamCongMain.Visibility = Visibility.Visible;
                    timer.Interval = new System.TimeSpan(0, 0, 0, 0, 1000);
                    timer.Tick += Timer_Tick;
                    timer.Start();
                    while (true)
                    {
                        ipWifi = "";
                        string externalIpString = await new HttpClient().GetStringAsync("http://checkip.dyndns.org/");
                        externalIpString = externalIpString.Replace("Current IP Address: ", "").Replace("\\r\\n", "").Replace("\\n", "").Trim();

                        var x = externalIpString.Split('.');
                        var x1 = x[0].Substring(x[0].LastIndexOf('>') + 1);
                        var x2 = x[3].Substring(0, x[3].IndexOf("<"));

                        externalIpString = String.Format("{0}:{1}:{2}:{3}", x1, x[1], x[2], x2);
                        ipWifi = (externalIpString);
                        break;
                    }
                }
                catch { }
            }
        }
        public enum EditType { DiemDanh, UpdateFace }

        private EditType _Type;
        public EditType Type
        {
            get { return _Type; }
            set { _Type = value; OnPropertyChanged(); }
        }
        private BitmapImage bitmapFace;

        public BitmapImage BitmapFace
        {
            get { return bitmapFace; }
            set
            {
                bitmapFace = value;

                OnPropertyChanged("BitmapFace");
            }
        }
        
        public Action Success
        {
            get { return (Action)GetValue(SuccessProperty); }
            set { SetValue(SuccessProperty, value); }
        }
        public static readonly DependencyProperty SuccessProperty = DependencyProperty.Register("Success", typeof(Action), typeof(ChamCong_Main));

       
        //

        DispatcherTimer timer;
        public FilterInfoCollection divices;
        VideoCaptureDevice cam;
        int tick = 5;
        public string latitude;
        public string longitute;
        public string ipWifi;
        public GeoCoordinateWatcher watcher = new GeoCoordinateWatcher();
        
        public async Task DiemDanh(string shiftId, APICheckFace infoFace)
        {
            try
            {
                using (MemoryStream bmp = new MemoryStream())
                {
                    /*Main.Type == 1 ? Main.APIStaff.data.data.access_token :*/
                    HttpClient httpClient = new HttpClient();
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",  Properties.Settings.Default.Token);
                    MultipartFormDataContent formData = new MultipartFormDataContent();
                    formData.Add(new StreamContent(new MemoryStream(Convert.FromBase64String(infoFace.image))), "data_img", "image_" + DateTime.Now.Ticks + ".jpg");
                    formData.Add(new StringContent(infoFace.company_id), "company_id");
                    formData.Add(new StringContent(infoFace.user_id), "user_id");
                    if (latitude != null) formData.Add(new StringContent(latitude), "ts_lat");
                    if (longitute != null) formData.Add(new StringContent(longitute), "ts_long");
                    formData.Add(new StringContent(ipWifi.Replace(":", ".")), "wifi_ip");
                    formData.Add(new StringContent(shiftId), "shift_id");
                    formData.Add(new StringContent(Main.Type + ""), "type");

                    var respon = await httpClient.PostAsync("http://210.245.108.202:3000/api/qlc/timekeeping/create/web", formData);
                    APICheckFace result = JsonConvert.DeserializeObject<APICheckFace>(respon.Content.ReadAsStringAsync().Result);
                    if (result.status.Equals("true"))
                    {
                        var x = new DiemDanh_TC_TB(this);
                        x.Avatar = BitmapFace;
                        x.Type = true;
                        Popup.NavigationService.Navigate(x);
                        PopupChamCong.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        var x = new DiemDanh_TC_TB(this);
                        x.Message = result.mess;
                        x.Avatar = BitmapFace;
                        x.Type = false;
                        Popup.NavigationService.Navigate(x);
                        PopupChamCong.Visibility = Visibility.Visible;
                    }
                }
            }
            catch (System.Exception ex)
            {

                System.Windows.MessageBox.Show(ex.Message);
            }
        }
        //
        public void CheckFace(string shiftId, string ep_id, string com_id)
        {
            using (MemoryStream bmp = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(BitmapFace));
                enc.Save(bmp);
                if (Main.Type == 1)
                {
                    try
                    {
                        List<APICheckFace> dataUser = new List<APICheckFace>() { new APICheckFace(com_id, ep_id, Convert.ToBase64String(bmp.ToArray())) };
                        HttpClient httpClient = new HttpClient();
                        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "http://43.239.223.147:5001/verify_web");
                        request.Content = new StringContent(JsonConvert.SerializeObject(dataUser), System.Text.Encoding.UTF8, "application/json");
                        //HttpResponseMessage response = await client.PostAsJsonAsync(path, model1);
                        var response = httpClient.SendAsync(request);
                        APICheckFace result = JsonConvert.DeserializeObject<APICheckFace>(response.Result.Content.ReadAsStringAsync().Result);
                        if (result.message)
                        {
                            DiemDanh(shiftId, dataUser[0]);
                        }
                        else
                        {
                            var x = new DiemDanh_TC_TB(this);
                            x.Message = "Không nhận diện được khuôn mặt";
                            x.Avatar = BitmapFace;
                            x.Type = false;
                            Popup.NavigationService.Navigate(x);
                            PopupChamCong.Visibility = Visibility.Visible;
                        }
                    }
                    catch
                    {
                        var x = new DiemDanh_TC_TB(this);
                        x.Message = "Không nhận diện được khuôn mặt";
                        x.Avatar = BitmapFace;
                        x.Type = false;
                        Popup.NavigationService.Navigate(x);
                        PopupChamCong.Visibility = Visibility.Visible;
                    }
                }
                else
                {
                    try
                    {
                        var image = Convert.ToBase64String(bmp.ToArray());
                        HttpClient httpClient1 = new HttpClient();
                        HttpRequestMessage request1 = new HttpRequestMessage(HttpMethod.Post, "http://43.239.223.5:4321/predict");
                        var content = new MultipartFormDataContent();
                        content.Add(new StringContent(image), "image_url");
                        request1.Content = content;
                        var response1 = httpClient1.SendAsync(request1);
                        try
                        {
                            if (response1.Result.Content.ReadAsStringAsync().Result.Contains("true"))
                            {
                                List<APICheckFace> dataUser = new List<APICheckFace>() { new APICheckFace(com_id, image) };
                                HttpClient httpClient = new HttpClient();
                                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "http://43.239.223.147:5001/verify_web_company");
                                request.Content = new StringContent(JsonConvert.SerializeObject(dataUser), System.Text.Encoding.UTF8, "application/json");
                                var response = httpClient.SendAsync(request);
                                APICheckFace result = JsonConvert.DeserializeObject<APICheckFace>(response.Result.Content.ReadAsStringAsync().Result);
                                int epid;
                                if (!int.TryParse(result.user_id, out epid))
                                {
                                    var x = new Error_NhanDien(this,Main);
                                    x.Message = "Dữ liệu khuôn mặt không hợp lệ! Vui lòng chấm công lại hoặc liên hệ với kỹ thuật để biết thêm !";
                                    //x.Message2 = "Bạn liên hệ với nhân sự công ty để cập nhật lại khuôn mặt";
                                    Popup.NavigationService.Navigate(x);
                                    PopupChamCong.Visibility = Visibility.Visible;
                                }
                                else
                                {
                                    dataUser[0].user_id = result.user_id;
                                    var x = new NhanDienNhanVien(this, result.user_id, Main);
                                    Popup.NavigationService.Navigate(x);
                                    PopupChamCong.Visibility = Visibility.Visible;


                                    x.ChamCong = () =>
                                    {
                                        getInfoChamCong(dataUser[0]);
                                    };

                                    x.ChamLai = () =>
                                    {
                                        Popup.NavigationService.Navigate(null);
                                        startChamCong();
                                    };
                                }
                            }
                            else
                            {
                                var x = new Error_NhanDien(this, Main);
                                x.Message = "Ảnh giả mạo";
                                x.Message2 = "";
                                Popup.NavigationService.Navigate(x);
                                PopupChamCong.Visibility = Visibility.Visible;
                            }
                        }
                        catch (Exception)
                        {
                            var x = new Error_NhanDien(this, Main);
                            x.Message = "Dữ liệu khuôn mặt không hợp lệ! Vui lòng chấm công lại hoặc liên hệ với kỹ thuật để biết thêm !";
                            //x.Message2 = "Bạn liên hệ với nhân sự công ty để cập nhật lại khuôn mặt";
                            Popup.NavigationService.Navigate(x);
                            PopupChamCong.Visibility = Visibility.Visible;
                        }
                    }
                    catch
                    {
                        var x = new Error_NhanDien(this, Main);
                        x.Message = "Dữ liệu khuôn mặt không hợp lệ! Vui lòng chấm công lại hoặc liên hệ với kỹ thuật để biết thêm !";
                        //x.Message2 = "Bạn liên hệ với nhân sự công ty để cập nhật lại khuôn mặt";
                        Popup.NavigationService.Navigate(x);
                        PopupChamCong.Visibility = Visibility.Visible;
                    }
                }
            }
        }
        string listPhoto = "";
        private void Timer_Tick(object sender, System.EventArgs e)
        {
           
            if (tick > 0)
            {
                tblTimer.Visibility = Visibility.Visible;
                tblTimer.Text = "Nhận diện trong " + tick.ToString();
            }
            else if (cam != null)
            {
                if (cam.IsRunning)
                {
                    cam.SignalToStop();
                 
                    CheckFace("", "", Main.IdAcount.ToString());
                }
                else
                {
                    cam.Stop();
                    timer.Stop();
                }
                tblTimer.Visibility = Visibility.Collapsed;
            }
            tick--;
        }
        public void getInfoChamCong(APICheckFace dataEpFace)
        {
            try
            {
                /* Main.Type == 1 ? Main.APIStaff.data.data.access_token :*/
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Properties.Settings.Default.Token);
                Dictionary<string, string> form = new Dictionary<string, string>();
                form.Add("u_id", dataEpFace.user_id);
                form.Add("c_id", dataEpFace.company_id);
                var respon = httpClient.PostAsync("http://210.245.108.202:3000/api/qlc/shift/list_shift_user", new FormUrlEncodedContent(form));
                APIShift result = JsonConvert.DeserializeObject<APIShift>(respon.Result.Content.ReadAsStringAsync().Result);
                if (result.data != null && dataEpFace.user_id != "Unknown")
                {
                    Popup.NavigationService.Navigate(new ThongTinChamCong(this, result.data.shift, dataEpFace, Main));
                }
                PopupChamCong.Visibility = Visibility.Visible;
            }
            catch
            {

            }
        }
       
        public async Task startCapture(bool flag)
        {
            watcher.StatusChanged += Watcher_StatusChanged ;
            watcher.Start();
            divices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            if (divices.Count > 0)
            {
                cam = new VideoCaptureDevice(divices[0].MonikerString);
                cam.NewFrame += new NewFrameEventHandler(Cam_NewFrame);
                cam.Start();
            }
            else
            {
                var x = new Popup.ChamCong.ChamCongKhuonMat.Error_NhanDien(this, Main);
                x.Message = "Không tìm thấy thiết bị ghi hình được kết nối, Hãy thử bật thiết bị ghi hình của bạn hoặc kết nối với thiết bị ghi hình dời và thử chấm công lại";
                x.Message2 = "";
                Popup.NavigationService.Navigate(x);
                PopupChamCong.Visibility = Visibility.Visible;
            }
            if (flag)
            {
                startChamCong();
            }
        }
       
        private void Cam_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                var image = (Bitmap)eventArgs.Frame.Clone();
                if (image != null)
                {
                    var filter = new Mirror(false, true);
                    filter.ApplyInPlace(image);
                    webcamImage.Dispatcher.Invoke(() => webcamImage.Source = BitmapFace = BitmapToImageSource(image));
                }
                else
                {
                    System.Windows.MessageBox.Show("lỗi");
                }
            }
            catch { }

        }
        private static System.Drawing.Image resizeImage(System.Drawing.Image imgToResize, System.Drawing.Size size)
        {
            //Get the image current width  
            int sourceWidth = imgToResize.Width;
            //Get the image current height  
            int sourceHeight = imgToResize.Height;
            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;
            //Calulate  width with new desired size  
            nPercentW = ((float)size.Width / (float)sourceWidth);
            //Calculate height with new desired size  
            nPercentH = ((float)size.Height / (float)sourceHeight);
            if (nPercentH < nPercentW)
                nPercent = nPercentH;
            else
                nPercent = nPercentW;
            //New Width  
            int destWidth = (int)(sourceWidth * nPercent);
            //New Height  
            int destHeight = (int)(sourceHeight * nPercent);
            Bitmap b = new Bitmap(destWidth, destHeight);
            Graphics g = Graphics.FromImage((System.Drawing.Image)b);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            // Draw image with new width and height  
            g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
            g.Dispose();
            return (System.Drawing.Image)b;
        }
        BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            Bitmap bitmap2 = new Crop(new Rectangle((bitmap.Width - bitmap.Height) / 2, 0, bitmap.Height, bitmap.Height)).Apply(bitmap);
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap2.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();
                return bitmapimage;

            }
        }
        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Task k = new Task(() =>
            {
                if (cam != null && cam.IsRunning) cam.Stop();
                if (timer != null)
                {
                    timer.Stop();
                }
            });
            k.Start();
           
        }

        private void ClosePopup(object sender, MouseButtonEventArgs e)
        {
            if (block)
            {
                if (cam != null && cam.IsRunning) cam.SignalToStop();
                if (timer != null)
                {
                    timer.Stop();
                }

            }
            else
            {
                startChamCong();
            }
        }

        private async void updateAllowUpdateFace()
        {
            try
            {
                //HttpClient http = new HttpClient();
                //http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Main.Type == 1 ? Main.APIStaff.data.data.access_token : Main.APICompany.data.data.access_token);
                //Dictionary<string, string> form = new Dictionary<string, string>();
                //form.Add("list_id", Main.APIStaff.data.data.user_info.ep_id);
                //HttpResponseMessage respon = await http.PostAsync("http://210.245.108.202:3000/api/qlc/face/add", new FormUrlEncodedContent(form));
                //API_Respon api = JsonConvert.DeserializeObject<API_Respon>(respon.Content.ReadAsStringAsync().Result);
                //if (api.data != null)
                //{
                //    Main.APIStaff.data.data.user_info.allow_update_face = "0";
                //}
            }
            catch { }
        }

        public async Task NhanDienKhuonMat()
        {
            try
            {
                //HttpClient httpClient = new HttpClient();
                //httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Main.Type == 1 ? Properties.Settings.Default.Token : Properties.Settings.Default.Token);
                //HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, $"http://43.239.223.147:5001/register_web?company_id={Main.IdAcount.ToString()}&user_id={Main.APIStaff.data.data.user_info.ep_id}");
                //request.Content = new StringContent(listPhoto, System.Text.Encoding.UTF8, "application/json");
                //var response = await httpClient.SendAsync(request);
                //API_Response result = JsonConvert.DeserializeObject<API_Response>(response.Content.ReadAsStringAsync().Result);
                //if (result.message)
                //{
                //    this.Dispatcher.Invoke(() =>
                //    {
                //        var x = new Register_Face_Success(this);
                //        Popup.NavigationService.Navigate(x);
                //        PopupChamCong.Visibility = Visibility.Visible;
                //        updateAllowUpdateFace();
                //    });
                //}
                //else
                //{
                //    var x = new Error_NhanDien(this, Error_NhanDien.EditType.UpdateFace);
                //    x.Message = "Đăng ký khuôn mặt thất bại";
                //    x.Message2 = "Không đăng ký được khuôn mặt vui lòng thử lại!";
                //    Popup.NavigationService.Navigate(x);
                //    PopupChamCong.Visibility = Visibility.Visible;
                //}

            }
            catch
            {
                this.Dispatcher.Invoke(() =>
                {
                    var x = new Error_NhanDien(this, Error_NhanDien.EditType.UpdateFace);
                    x.Message = "Đăng ký khuôn mặt thất bại";
                    x.Message2 = "Không đăng ký được khuôn mặt vui lòng thử lại!";
                    Popup.NavigationService.Navigate(x);
                    PopupChamCong.Visibility = Visibility.Visible;
                });
            }
        }
       

    }
}
//switch (Main.Type)
//{
//    case 1:
//        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Main.APIStaff.data.data.access_token);
//        form.Add("com_id", Main.APIStaff.data.data.user_info.com_id);
//        break;
//case 2:
//    break;
//default:
//    break;
//}
//switch (Main.Type)
//{
//    case 1:
//        Main.SideBarIndexNV = 0;
//        break;
//    case 2:
//        Main.SideBarIndex = 0;
//        break;
//    default:
//        break;
//}
//Main.ClosePopup();
//switch (Main.Type)
//{
//    case 1:
//        Main.SideBarIndexNV = 0;
//        break;
//    case 2:
//        Main.SideBarIndex = 0;
//        break;
//    default:
//        break;
//}
//Main.ClosePopup();

/*divices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
    if (divices.Count <= 0)
    {
        var x = new ChamCong365.Popup.ChamCong.ChamCongKhuonMat.Error_NhanDien(this,Main);
        x.Message = "Không tìm thấy thiết bị ghi hình được kết nối";
        x.Message2 = "Hãy thử bật thiết bị ghi hình của bạn hoặc kết nối với thiết bị ghi hình dời và thử chấm công lại";
        Popup.NavigationService.Navigate(x);
        PopupChamCong.Visibility = Visibility.Visible;
    }
    else if (IsWebCamInUse())
    {
        var x = new Error_NhanDien(this, Main);
        x.Message = "Thiết bị ghi hình đang được sử dụng bởi một ứng dụng khác, ";
        x.Message2 = "hãy thử tắt các thiết bị phần mềm đang sử dụng thiết bị ghi hình và thử chấm công lại";
        Popup.NavigationService.Navigate(x);
        PopupChamCong.Visibility = Visibility.Visible;
    }
    else*/



//switch (Type)
//{
//    case EditType.DiemDanh:
//    break;
//case EditType.UpdateFace:
//if (main.APIStaff.data.data.user_info.allow_update_face == "0")
//{
//}
//        break;
//    default:
//        break;
//}

//Cập nhật mặt nhân viên
//block = true;
//if (timer != null) timer.Stop();
//var x = new ChamCong365.Popup.ChamCong.Comon.Notify1();
//x.Type = ChamCong365.Popup.ChamCong.Comon.Notify1.NotifyType.Error;
//x.Message = "Bạn chưa được cấp quyền đăng ký lại khuôn mặt!<br>Vui lòng liên hệ với tài khoản công ty để được cấp quyền đăng ký.";
//x.NotiClosed += () =>
//{
//    Task k = new Task(() =>
//    {
//        if (cam != null && cam.IsRunning) cam.Stop();
//        if (timer != null)
//        {
//            timer.Stop();
//        }
//    });
//    k.Start();
//};
//Popup.NavigationService.Navigate(x);
//PopupChamCong.Visibility = Visibility.Visible;


//switch (Type)
//   {
//       case EditType.DiemDanh:
//if (!IsWebCamInUse())
//{
//    if (cam != null) cam.Stop();
//    if (timer != null) timer.Stop();

//    var x = new Error_NhanDien(this, Main);
//    x.Message = "Thiết bị ghi hình đang được sử dụng bởi một ứng dụng khác, ";
//    x.Message2 = "hãy thử tắt các thiết bị phần mềm đang sử dụng thiết bị ghi hình và thử chấm công lại";
//    Popup.NavigationService.Navigate(x);
//    PopupChamCong.Visibility = Visibility.Visible;
//}
//else
//                     break;
//                case EditType.UpdateFace:
//                    if (tick > 0)
//                    {
//                        tblTimer.Visibility = Visibility.Visible;
//                        tblTimer.Text = "Nhận diện trong " + tick.ToString();
//                        if (tick == 2)
//                        {
//                            listPhoto = "";
//                            Thread threa = new Thread(() =>
//                            {
//                                for (int i = 0; i < 10; i++)
//                                {
//                                    using (MemoryStream bmp = new MemoryStream())
//                                    {
//                                        BitmapEncoder enc = new BmpBitmapEncoder();
//                                        enc.Frames.Add(BitmapFrame.Create(BitmapFace));
//                                        enc.Save(bmp);
//                                        if (i == 0)
//                                        {
//                                            listPhoto += "[{\"image\":\"" + Convert.ToBase64String(bmp.ToArray()) + "\"},";
//                                        }
//                                        else if (i == 9)
//                                        {
//                                            listPhoto += "{\"image\":\"" + Convert.ToBase64String(bmp.ToArray()) + "\"}]";
//                                        }
//                                        else
//                                        {
//                                            listPhoto += "{\"image\":\"" + Convert.ToBase64String(bmp.ToArray()) + "\"},";
//                                        }
//                                        Thread.Sleep(100);
//                                    }
//                                }
//                                if (Main.Type == 1)
//                                {
//                                    NhanDienKhuonMat();
//                                }
//                            });
//        threa.Start();
//                        }
//}
//                    else
//{
//    if (cam.IsRunning)
//    {
//        cam.SignalToStop();
//        ////cam.NewFrame -= Cam_NewFrame;
//    }
//    else
//    {
//        cam.Stop();
//        timer.Stop();
//    }
//    tblTimer.Visibility = Visibility.Collapsed;
//}

//break;
//default:
//                    break;
//            }
//if (Main.Type == 1)
//{
//    getInfoChamCong(new APICheckFace() { user_id = Main.APIStaff.data.data.user_info.ep_id, company_id = Main.APIStaff.data.data.user_info.com_id });
//}
//else
//{
