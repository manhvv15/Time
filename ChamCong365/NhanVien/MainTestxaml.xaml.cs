﻿using ChamCong365.OOP.funcQuanLyCongTy;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
using static ChamCong365.NhanVien.MainTestxaml;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Header;

namespace ChamCong365.NhanVien
{
    /// <summary>
    /// Interaction logic for MainTestxaml.xaml
    /// </summary>
    public partial class MainTestxaml : Window
    {
        public MainTestxaml()
        {
            InitializeComponent();
            //getInfor();
        }

        private class VatChat
        {
            public int stt;
            public string hu;
        }
        private ObservableCollection<VatChat> listInfor = new ObservableCollection<VatChat>();
        int stt = 1; // Số thứ tự ban đầu
        public class ItemData
        {
            public int STT { get; set; }
            public string Ten { get; set; }
            public int SoLuong { get; set; }
        }
        ObservableCollection<ItemData> itemList = new ObservableCollection<ItemData>();
        private void getInfor()
        {

        }
        private void Border_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            getInfor();
        }
    }
}
