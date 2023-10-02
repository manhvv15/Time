﻿using AForge.Imaging.Filters;
using AForge.Video;
using AForge.Video.DirectShow;
using ChamCong365.OOP.ChamCong.CauHinhChamCong;
using ChamCong365.OOP.ChamCong.ChamCongKhuonMat;
using ChamCong365.Popup.ChamCong.ChamCongKhuonMat;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

using static ChamCong365.Popup.ChamCong.ChamCongKhuonMat.Error_NhanDien;

namespace ChamCong365.TimeKeeping
{
    /// <summary>
    /// Interaction logic for ucBatdauChamCong.xaml
    /// </summary>
    public partial class ucChamCongKhuonMat : UserControl
    {
        FilterInfoCollection videoDevices;
        VideoCaptureDevice videoSource;
        Bitmap currentFrame;
        int tick = 5;
        DispatcherTimer timer;


        public MainWindow Main;
        BrushConverter bcMain = new BrushConverter();
        Error_NhanDien Error;
        public ucChamCongKhuonMat(MainWindow main, Error_NhanDien error)
        {
            InitializeComponent();
            InitializeCamera();
            Main = main;
            Error = error;
            //startCapture(false);
        }
        private void InitializeCamera()
        {
            videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

            if (videoDevices.Count == 0)
            {
                MessageBox.Show("No webcam found.");
                return;
            }

            videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);
            videoSource.NewFrame += new NewFrameEventHandler(videoSource_NewFrame);
            tick = 5;
            if (timer != null)
            {
                timer.Stop();
            }
            timer = new DispatcherTimer();
            //PageChamCongStart.Visibility = Visibility.Hidden;
            //PageChamCongMain.Visibility = Visibility.Visible;
            timer.Interval = new System.TimeSpan(0, 0, 0, 0, 1000);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, System.EventArgs e)
        {
            if (tick > 0)
            {
                tblTimer.Visibility = Visibility.Visible;
                tblTimer.Text = "Nhận diện trong " + tick.ToString();
                if (tick == 1)
                {
                    if (currentFrame != null)
                    {
                        BitmapImage bitmapImage = BitmapToImageSource(currentFrame);

                        // Thêm ảnh đã chụp vào ObservableCollection
                        capturedImages.Add(bitmapImage);

                        // Cập nhật nguồn dữ liệu của ListBox
                        //imageListBox.ItemsSource = capturedImages;
                        //ChamCongThanhCongAn.Visibility = Visibility.Visible;
                        path.Visibility = Visibility.Collapsed;
                    }

                }
            }
            else
            {
                //MessageBox.Show("jn");
                if (videoSource.IsRunning)
                {
                    videoSource.SignalToStop();
                    ////cam.NewFrame -= Cam_NewFrame;
                }
                else
                {
                    videoSource.Stop();
                    timer.Stop();
                }
                tblTimer.Visibility = Visibility.Collapsed;
            }
            tick--;
        }

        private void videoSource_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            // Lấy khung hình hiện tại từ sự kiện NewFrame
            currentFrame = (Bitmap)eventArgs.Frame.Clone();

            // Hiển thị khung hình trên giao diện người dùng
            webcamImage.Dispatcher.Invoke(() =>
            {
                webcamImage.Source = BitmapToImageSource(currentFrame);
            });

        }

        ObservableCollection<BitmapImage> capturedImages = new ObservableCollection<BitmapImage>();
        //System.Drawing.Bitmap anhChamCong = new System.Drawing.Bitmap(filePath);

        // Thêm danh sách các tệp ảnh đã chụp vào ListBox hoặc ListView

        public FilterInfoCollection divices;
        public void startChamCong()
        {
            videoSource.Start();

        }
        public bool IsWebCamInUse()
        {
            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\CapabilityAccessManager\ConsentStore\webcam\NonPackaged"))
                {
                    if (key != null && key.GetSubKeyNames() != null)
                    {
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
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
            return false;
        }

        private BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            Bitmap bitmap2 = new Crop(new System.Drawing.Rectangle((bitmap.Width - bitmap.Height) / 2, 0, bitmap.Height, bitmap.Height)).Apply(bitmap);
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

        private void btnStart_MouseUp(object sender, MouseButtonEventArgs e)
        {
            anhHien.Visibility = Visibility.Collapsed;
            anhAn.Visibility = Visibility.Visible;
            startChamCong();

        }

        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

            if (currentFrame != null)
            {
                // Chuyển đổi Bitmap thành BitmapImage
                int numberOfImages = capturedImages.Count;
                if (numberOfImages < 5)
                {
                    BitmapImage bitmapImage = BitmapToImageSource(currentFrame);

                    // Thêm ảnh đã chụp vào ObservableCollection
                    capturedImages.Add(bitmapImage);

                    // Cập nhật nguồn dữ liệu của ListBox
                    //imageListBox.ItemsSource = capturedImages;
                }
                if (numberOfImages == 5)
                {
                    //capNhatThanhCong uc = new capNhatThanhCong(Main);
                    // Main.grShowPopup.Children.Clear();
                    //object Content = uc.Content;
                    //uc.Content = null;
                    Main.dopBody.Children.Add(Content as UIElement);
                    //videoSource.Stop(); 
                    if (videoSource != null && videoSource.IsRunning) videoSource.Stop();
                }
            }
        }

        private void Border_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //ChamCongThanhCongAn.Visibility = Visibility.Collapsed;
            //ThongTinAn.Visibility = Visibility.Visible;
        }

        private void Border_MouseLeftButtonUp_1(object sender, MouseButtonEventArgs e)
        {
            //ThongTinAn.Visibility = Visibility.Collapsed;
            //ChamCongHoanTatAn.Visibility = Visibility.Visible;
        }
        #region Cham cong sai
        //public enum EditType { DiemDanh, UpdateFace }

        //private EditType _Type;
        //public EditType Type
        //{
        //    get { return _Type; }
        //    set { _Type = value; OnPropertyChanged(); }
        //}

        //private BitmapImage bitmapFace;

        //public BitmapImage BitmapFace
        //{
        //    get { return bitmapFace; }
        //    set
        //    {
        //        bitmapFace = value;

        //        OnPropertyChanged("BitmapFace");
        //    }
        //}

        ////BƯỚC 1
        //public async Task startCapture(bool flag)
        //{
        //    watcher.StatusChanged += Watcher_StatusChanged; ;
        //    watcher.Start();
        //    divices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
        //    if (divices.Count > 0)
        //    {
        //        cam = new VideoCaptureDevice(divices[0].MonikerString);
        //        cam.NewFrame += Cam_NewFrame;
        //        cam.Start();
        //    }
        //    else
        //    {
        //        var x = new Popup.ChamCong.ChamCongKhuonMat.ucErrorSaff(this);
        //        x.Message = "Không tìm thấy thiết bị ghi hình được kết nối";
        //        x.Message2 = "Hãy thử bật thiết bị ghi hình của bạn hoặc kết nối với thiết bị ghi hình dời và thử chấm công lại";
        //        Popup.NavigationService.Navigate(x);
        //        PopupChamCong.Visibility = Visibility.Visible;

        //    }
        //    if (flag)
        //    {
        //        startChamCong();
        //    }
        //}

        ////BƯỚC 2
        //private void Start_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    startChamCong();
        //}

        ////BƯỚC 3
        //public async void startChamCong()
        //{
        //    PopupChamCong.Visibility = Visibility.Collapsed;
        //    divices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
        //    if (divices.Count <= 0)
        //    {
        //        var x = new Popup.ChamCong.ChamCongKhuonMat.ucErrorSaff(this);
        //        x.Message = "Không tìm thấy thiết bị ghi hình được kết nối";
        //        x.Message2 = "Hãy thử bật thiết bị ghi hình của bạn hoặc kết nối với thiết bị ghi hình dời và thử chấm công lại";
        //        Popup.NavigationService.Navigate(x);
        //        PopupChamCong.Visibility = Visibility.Visible;
        //    }
        //    else if (IsWebCamInUse())
        //    {
        //        var x = new ucErrorSaff(this);
        //        x.Message = "Thiết bị ghi hình đang được sử dụng bởi một ứng dụng khác, ";
        //        x.Message2 = "hãy thử tắt các thiết bị phần mềm đang sử dụng thiết bị ghi hình và thử chấm công lại";
        //        Popup.NavigationService.Navigate(x);
        //        PopupChamCong.Visibility = Visibility.Visible;
        //    }
        //    else
        //    {
        //        try
        //        {
        //            if (cam == null)
        //            {
        //                cam = new VideoCaptureDevice(divices[0].MonikerString);
        //                cam.NewFrame += Cam_NewFrame;
        //            }
        //            if (cam != null && !cam.IsRunning) cam.Start();
        //            tick = 5;
        //            if (timer != null)
        //            {
        //                timer.Stop();
        //            }
        //            timer = new DispatcherTimer();
        //            PageChamCongStart.Visibility = Visibility.Hidden;
        //            PageChamCongMain.Visibility = Visibility.Visible;
        //            timer.Interval = new System.TimeSpan(0, 0, 0, 0, 1000);
        //            timer.Tick += Timer_Tick;
        //            timer.Start();
        //            while (true)
        //            {
        //                ipWifi = "";
        //                string externalIpString = await new HttpClient().GetStringAsync("http://checkip.dyndns.org/");
        //                externalIpString = externalIpString.Replace("Current IP Address: ", "").Replace("\\r\\n", "").Replace("\\n", "").Trim();

        //                var x = externalIpString.Split('.');
        //                var x1 = x[0].Substring(x[0].LastIndexOf('>') + 1);
        //                var x2 = x[3].Substring(0, x[3].IndexOf("<"));

        //                externalIpString = String.Format("{0}:{1}:{2}:{3}", x1, x[1], x[2], x2);
        //                ipWifi = (externalIpString);
        //                break;
        //            }
        //        }
        //        catch { }
        //    }
        //}


        ////BƯỚC 4
        //public void CheckFace(string shiftId, string ep_id, string com_id)
        //{
        //    using (MemoryStream bmp = new MemoryStream())
        //    {
        //        BitmapEncoder enc = new BmpBitmapEncoder();
        //        enc.Frames.Add(BitmapFrame.Create(BitmapFace));
        //        enc.Save(bmp);
        //        try
        //        {
        //            var image = Convert.ToBase64String(bmp.ToArray());
        //            HttpClient httpClient1 = new HttpClient();
        //            HttpRequestMessage request1 = new HttpRequestMessage(HttpMethod.Post, "http://43.239.223.5:4321/predict");
        //            var content = new MultipartFormDataContent();
        //            content.Add(new StringContent(image), "image_url");
        //            request1.Content = content;
        //            var response1 = httpClient1.SendAsync(request1);
        //            try
        //            {
        //                if (response1.Result.Content.ReadAsStringAsync().Result.Contains("true"))
        //                {
        //                    List<APICheckFace> dataUser = new List<APICheckFace>() { new APICheckFace(com_id, image) };
        //                    HttpClient httpClient = new HttpClient();
        //                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "http://43.239.223.147:5001/verify_web_company");
        //                    request.Content = new StringContent(JsonConvert.SerializeObject(dataUser), System.Text.Encoding.UTF8, "application/json");
        //                    var response = httpClient.SendAsync(request);
        //                    APICheckFace result = JsonConvert.DeserializeObject<APICheckFace>(response.Result.Content.ReadAsStringAsync().Result);
        //                    int epid;
        //                    if (!int.TryParse(result.user_id, out epid))
        //                    {
        //                        var x = new ucErrorSaff(this);
        //                        x.Message = "Dữ liệu khuôn mặt không hợp lệ! Vui lòng chấm công lại hoặc liên hệ với kỹ thuật để biết thêm !";
        //                        x.Message2 = "";
        //                        Popup.NavigationService.Navigate(x);
        //                        PopupChamCong.Visibility = Visibility.Visible;
        //                    }
        //                    else
        //                    {
        //                        dataUser[0].user_id = result.user_id;
        //                        var x = new ucCheckSaff(Main, this, result.user_id);
        //                        Popup.NavigationService.Navigate(x);
        //                        PopupChamCong.Visibility = Visibility.Visible;


        //                        x.ChamCongStart = () =>
        //                        {
        //                            getInfoChamCong(dataUser[0]);
        //                        };

        //                        x.ChamLai = () =>
        //                        {
        //                            Popup.NavigationService.Navigate(null);
        //                            startChamCong();
        //                        };
        //                    }
        //                }
        //                else
        //                {
        //                    var x = new ucErrorSaff(this);
        //                    x.Message = "Ảnh giả mạo";
        //                    x.Message2 = "";
        //                    Popup.NavigationService.Navigate(x);
        //                    PopupChamCong.Visibility = Visibility.Visible;
        //                }
        //            }
        //            catch (Exception)
        //            {
        //                var x = new ucErrorSaff(this);
        //                x.Message = "Dữ liệu khuôn mặt không hợp lệ! Vui lòng chấm công lại hoặc liên hệ với kỹ thuật để biết thêm !";
        //                x.Message2 = "";
        //                Popup.NavigationService.Navigate(x);
        //                PopupChamCong.Visibility = Visibility.Visible;
        //            }
        //        }
        //        catch
        //        {
        //            var x = new ucErrorSaff(this);
        //            x.Message = "Dữ liệu khuôn mặt không hợp lệ! Vui lòng chấm công lại hoặc liên hệ với kỹ thuật để biết thêm !";
        //            x.Message2 = "";
        //            Popup.NavigationService.Navigate(x);
        //            PopupChamCong.Visibility = Visibility.Visible;
        //        }

        //    }
        //}

        //private async Task<API_Com_ChiTiet> getComDetail()
        //{
        //    try
        //    {
        //        string apilink = "http://210.245.108.202:3000/api/qlc/company/info";
        //        HttpClient httpClient = new HttpClient();
        //        Dictionary<string, string> form = new Dictionary<string, string>();

        //        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Properties.Settings.Default.Token);
        //        form.Add("com_id", Main.IdAcount.ToString());

        //        //int i = 0;
        //        List<ChildCompany> list = new List<ChildCompany>();

        //        var respon = await httpClient.PostAsync(apilink, new FormUrlEncodedContent(form));
        //        API_Com_ChiTiet api = JsonConvert.DeserializeObject<API_Com_ChiTiet>(respon.Content.ReadAsStringAsync().Result);
        //        if (api.data != null) return api;
        //        return null;
        //    }
        //    catch (Exception)
        //    {
        //        return null;
        //    }
        //}

        //public async Task DiemDanh(string shiftId, APICheckFace infoFace)
        //{
        //    try
        //    {
        //        using (MemoryStream bmp = new MemoryStream())
        //        {
        //            HttpClient httpClient = new HttpClient();
        //            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Properties.Settings.Default.Token);
        //            MultipartFormDataContent formData = new MultipartFormDataContent();
        //            formData.Add(new StreamContent(new MemoryStream(Convert.FromBase64String(infoFace.image))), "data_img", "image_" + DateTime.Now.Ticks + ".jpg");
        //            formData.Add(new StringContent(infoFace.company_id), "company_id");
        //            formData.Add(new StringContent(infoFace.user_id), "user_id");
        //            if (latitude != null) formData.Add(new StringContent(latitude), "ts_lat");
        //            if (longitute != null) formData.Add(new StringContent(longitute), "ts_long");
        //            formData.Add(new StringContent(ipWifi.Replace(":", ".")), "wifi_ip");
        //            formData.Add(new StringContent(shiftId), "shift_id");
        //            //formData.Add(new StringContent(Main.Type + ""), "type");

        //            var respon = await httpClient.PostAsync("http://210.245.108.202:3000/api/qlc/timekeeping/create/web", formData);

        //            APICheckFace result = JsonConvert.DeserializeObject<APICheckFace>(respon.Content.ReadAsStringAsync().Result);

        //            if (result.status.Equals("true"))
        //            {
        //                var x = new ucCheck_Face_Fail(this);
        //                x.Avatar = BitmapFace;
        //                x.Type = true;
        //                Popup.NavigationService.Navigate(x);
        //                PopupChamCong.Visibility = Visibility.Visible;
        //            }
        //            else
        //            {
        //                var x = new ucCheck_Face_Fail(this);
        //                x.Message = result.mess;
        //                x.Avatar = BitmapFace;
        //                x.Type = false;
        //                Popup.NavigationService.Navigate(x);
        //                PopupChamCong.Visibility = Visibility.Visible;
        //            }
        //        }
        //    }
        //    catch (System.Exception ex)
        //    {

        //        MessageBox.Show(ex.Message);
        //    }
        //}

        //string listPhoto = "";


        //private void Watcher_StatusChanged(object sender, GeoPositionStatusChangedEventArgs e)
        //{
        //    try
        //    {
        //        if (e.Status == GeoPositionStatus.Ready)
        //        {
        //            // Display the latitude and longitude.  

        //            if (watcher.Position.Location.IsUnknown)
        //            {
        //                latitude = "0";
        //                longitute = "0";
        //            }
        //            else
        //            {
        //                latitude = watcher.Position.Location.Latitude.ToString();
        //                longitute = watcher.Position.Location.Longitude.ToString();
        //            }
        //        }
        //        else
        //        {
        //            latitude = "0";
        //            longitute = "0";
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        latitude = "0";
        //        longitute = "0";
        //    }
        //}
        //DispatcherTimer timer;
        //public FilterInfoCollection divices;
        //VideoCaptureDevice cam;
        //int tick = 5;
        //public string latitude;
        //public string longitute;
        //public string ipWifi;
        //public GeoCoordinateWatcher watcher = new GeoCoordinateWatcher();



        //#region Cham Cong Khuon Mat



        //public bool IsWebCamInUse()
        //{
        //    try
        //    {
        //        using (var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\CapabilityAccessManager\ConsentStore\webcam\NonPackaged"))
        //        {
        //            if (key != null && key.GetSubKeyNames() != null)
        //                foreach (var subKeyName in key.GetSubKeyNames())
        //                {
        //                    using (var subKey = key.OpenSubKey(subKeyName))
        //                    {
        //                        if (subKey.GetValueNames().Contains("LastUsedTimeStop"))
        //                        {
        //                            var endTime = subKey.GetValue("LastUsedTimeStop") is long ? (long)subKey.GetValue("LastUsedTimeStop") : -1;
        //                            if (endTime <= 0)
        //                            {
        //                                return true;
        //                            }
        //                            else
        //                            {
        //                                return false;
        //                            }
        //                        }
        //                    }
        //                }
        //            else return false;
        //        }
        //    }
        //    catch (Exception exception)
        //    {
        //        MessageBox.Show(exception.Message);
        //    }
        //    return false;
        //}

        //private void Cam_NewFrame(object sender, NewFrameEventArgs eventArgs)
        //{
        //    try
        //    {
        //        var image = (Bitmap)eventArgs.Frame.Clone();
        //        if (image != null)
        //        {
        //            //Lỗi ở đây
        //            var filter = new Mirror(false, true);
        //            filter.ApplyInPlace(image);
        //            this.Dispatcher.Invoke(() => BitmapFace = BitmapToImageSource(image));
        //        }
        //        else
        //        {
        //            MessageBox.Show("lỗi");
        //        }
        //    }
        //    catch { }

        //}
        //#endregion
        //private void Timer_Tick(object sender, System.EventArgs e)
        //{
        //    switch (Type)
        //    {
        //        case EditType.DiemDanh:
        //            if (!IsWebCamInUse() && BitmapFace == null)
        //            {
        //                if (cam != null) cam.Stop();
        //                if (timer != null) timer.Stop();

        //                var x = new ucErrorSaff(this);
        //                x.Message = "Thiết bị ghi hình đang được sử dụng bởi một ứng dụng khác, ";
        //                x.Message2 = "hãy thử tắt các thiết bị phần mềm đang sử dụng thiết bị ghi hình và thử chấm công lại";
        //                Popup.NavigationService.Navigate(x);
        //                PopupChamCong.Visibility = Visibility.Visible;
        //            }
        //            else if (tick > 0)
        //            {
        //                tblTimer.Visibility = Visibility.Visible;
        //                tblTimer.Text = "Nhận diện trong " + tick.ToString();
        //            }
        //            else if (cam != null)
        //            {
        //                if (cam.IsRunning)
        //                {
        //                    cam.SignalToStop();
        //                    CheckFace("", "", Main.IdAcount.ToString());
        //                }
        //                else
        //                {
        //                    cam.Stop();
        //                    timer.Stop();
        //                }
        //                tblTimer.Visibility = Visibility.Collapsed;
        //            }

        //            break;
        //        case EditType.UpdateFace:
        //            if (tick > 0)
        //            {
        //                tblTimer.Visibility = Visibility.Visible;
        //                tblTimer.Text = "Nhận diện trong " + tick.ToString();
        //                if (tick == 2)
        //                {
        //                    listPhoto = "";
        //                    Thread threa = new Thread(() =>
        //                    {
        //                        for (int i = 0; i < 10; i++)
        //                        {
        //                            using (MemoryStream bmp = new MemoryStream())
        //                            {
        //                                BitmapEncoder enc = new BmpBitmapEncoder();
        //                                enc.Frames.Add(BitmapFrame.Create(BitmapFace));
        //                                enc.Save(bmp);
        //                                if (i == 0)
        //                                {
        //                                    listPhoto += "[{\"image\":\"" + Convert.ToBase64String(bmp.ToArray()) + "\"},";
        //                                }
        //                                else if (i == 9)
        //                                {
        //                                    listPhoto += "{\"image\":\"" + Convert.ToBase64String(bmp.ToArray()) + "\"}]";
        //                                }
        //                                else
        //                                {
        //                                    listPhoto += "{\"image\":\"" + Convert.ToBase64String(bmp.ToArray()) + "\"},";
        //                                }
        //                                Thread.Sleep(100);
        //                            }
        //                        }
        //                        //if (Main.IdAcount.ToString() != null)
        //                        //{
        //                        //    NhanDienKhuonMat();
        //                        //}
        //                    });
        //                    threa.Start();
        //                }
        //            }
        //            else
        //            {
        //                if (cam.IsRunning)
        //                {
        //                    cam.SignalToStop();
        //                }
        //                else
        //                {
        //                    cam.Stop();
        //                    timer.Stop();
        //                }
        //                tblTimer.Visibility = Visibility.Collapsed;
        //            }
        //            break;
        //        default:
        //            break;
        //    }
        //    tick--;
        //}

        //public void getInfoChamCong(APICheckFace dataEpFace)
        //{
        //    try
        //    {
        //        HttpClient httpClient = new HttpClient();
        //        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Properties.Settings.Default.Token);
        //        Dictionary<string, string> form = new Dictionary<string, string>();
        //        form.Add("u_id", dataEpFace.user_id);
        //        form.Add("c_id", dataEpFace.company_id);
        //        var respon = httpClient.PostAsync("http://210.245.108.202:3000/api/qlc/shift/list_shift_user", new FormUrlEncodedContent(form));
        //        APIShift result = JsonConvert.DeserializeObject<APIShift>(respon.Result.Content.ReadAsStringAsync().Result);
        //        if (result.data != null && dataEpFace.user_id != "Unknown")
        //        {
        //            Popup.NavigationService.Navigate(new ucThongTinChamCong(this, result.data.shift, dataEpFace));
        //        }
        //        PopupChamCong.Visibility = Visibility.Visible;
        //    }
        //    catch
        //    {

        //    }
        //}






        //public OOP.Login.LoginCom.Root APICompany { get; set; }
        //public OOP.Login.InfoEp.Root APIStaff { get; set; }
        //public OOP.Login.InfoEp.Employee APIEp { get; set; }



        //BitmapImage BitmapToImageSource(Bitmap bitmap)
        //{
        //    Bitmap bitmap2 = new Crop(new System.Drawing.Rectangle((bitmap.Width - bitmap.Height) / 2, 0, bitmap.Height, bitmap.Height)).Apply(bitmap);
        //    using (MemoryStream memory = new MemoryStream())
        //    {
        //        bitmap2.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
        //        memory.Position = 0;
        //        BitmapImage bitmapimage = new BitmapImage();
        //        bitmapimage.BeginInit();
        //        bitmapimage.StreamSource = memory;
        //        bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
        //        bitmapimage.EndInit();
        //        return bitmapimage;

        //    }
        //}

        //private static System.Drawing.Image resizeImage(System.Drawing.Image imgToResize, System.Drawing.Size size)
        //{
        //    //Get the image current width  
        //    int sourceWidth = imgToResize.Width;
        //    //Get the image current height  
        //    int sourceHeight = imgToResize.Height;
        //    float nPercent = 0;
        //    float nPercentW = 0;
        //    float nPercentH = 0;
        //    //Calulate  width with new desired size  
        //    nPercentW = ((float)size.Width / (float)sourceWidth);
        //    //Calculate height with new desired size  
        //    nPercentH = ((float)size.Height / (float)sourceHeight);
        //    if (nPercentH < nPercentW)
        //        nPercent = nPercentH;
        //    else
        //        nPercent = nPercentW;
        //    //New Width  
        //    int destWidth = (int)(sourceWidth * nPercent);
        //    //New Height  
        //    int destHeight = (int)(sourceHeight * nPercent);
        //    Bitmap b = new Bitmap(destWidth, destHeight);
        //    Graphics g = Graphics.FromImage((System.Drawing.Image)b);
        //    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
        //    // Draw image with new width and height  
        //    g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
        //    g.Dispose();
        //    return (System.Drawing.Image)b;
        //}
        //private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{

        //}

        //private void ClosePopup(object sender, MouseButtonEventArgs e)
        //{

        //}

        #endregion
    }
}
