﻿
using ChamCong365.TimeKeeping;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ChamCong365.Popup.ChamCong.ChamCongKhuonMat
{
    /// <summary>
    /// Interaction logic for Register_Face_Success.xaml
    /// </summary>
    public partial class DangKyKhuonMatThanhCong : Page
    {
        public DangKyKhuonMatThanhCong(ChamCong_Main ChamCong_Main)
        {
            InitializeComponent();
            ChamCongMain = ChamCong_Main;
        }
        private ChamCong_Main ChamCongMain;
        private void TrangChu_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //switch (ChamCongMain.Main.Type)
            //{
            //    case 1:
            //        ChamCongMain.Main.SideBarIndexNV = 0;
            //        break;
            //    case 2:
            //        ChamCongMain.Main.SideBarIndex = 0;
            //        break;
            //    default:
            //        break;
            //}
            //ChamCongMain.Main.ClosePopup();
        }
    }
}
