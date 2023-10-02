﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChamCong365.OOP.funcQuanLyCongTy
{
    public class ChiTietDeXuat
    {
        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
        public class BoNhiem
        {
            public object thanhviendc_bn { get; set; }
            public object name_ph_bn { get; set; }
            public object chucvu_hientai { get; set; }
            public object chucvu_dx_bn { get; set; }
            public object phong_ban_moi { get; set; }
            public object ly_do { get; set; }
        }

        public class CapPhatTaiSan
        {
            public object danh_sach_tai_san { get; set; }
            public object so_luong_tai_san { get; set; }
            public object ly_do { get; set; }
        }

        public class Data
        {
            public bool result { get; set; }
            public string message { get; set; }
            public List<DetailDeXuat> detailDeXuat { get; set; }
        }

        public class DetailDeXuat
        {
            public string id_de_xuat { get; set; }
            public string ten_de_xuat { get; set; }
            public string nguoi_tao { get; set; }
            public int id_nguoi_tao { get; set; }
            public int nhom_de_xuat { get; set; }
            public double thoi_gian_tao { get; set; }
            public int loai_de_xuat { get; set; }
            public int cap_nhat { get; set; }
            public ThongTinChung thong_tin_chung { get; set; }
            private string Kieu_phe_duyet { get; set; }
            public string kieu_phe_duyet {
                get { switch (Kieu_phe_duyet) {
                        case "0":
                            return "Duyệt lần lượt";
                        default:
                            return "";
                            } }
                set { Kieu_phe_duyet = value; } }
            public List<LanhDaoDuyet> lanh_dao_duyet { get; set; }
            public List<LanhDaoDuyet> nguoi_theo_doi { get; set; }
            public List<object> file_kem { get; set; }
            public int type_duyet { get; set; }
            public double thoi_gian_duyet { get; set; }
            public double thoi_gian_tiep_nhan { get; set; }
            public List<LichSuDuyet> lich_su_duyet { get; set; }
            public int del_type { get; set; }
            public bool qua_han_duyet { get; set; }
            public int tam_ung_status { get; set; }
            public int thanh_toan_status { get; set; }
        }
        public class LichSuDuyet
        {
            public string ng_duyet { get; set; }
            public string Type_duyet { get; set; }
            public string type_duyet {
                get {
                    switch (Type_duyet)
                    {
                        case "1":
                            return "vừa tạo đề xuất";
                        case "2":
                            return "vừa tiếp nhận đề xuất";
                        case "3":
                            return "vừa duyệt đề xuất";
                        default:
                            return "";
                    }
                }
                set { Type_duyet = value; } }
            public DateTime time { get; set; }
        }
        public class DiMuonVeSom
        {
            public object loai_de_xuat { get; set; }
            public object ngay_di_muon_ve_som { get; set; }
            public object time_batdau { get; set; }
            public object time_ketthuc { get; set; }
            public object ca_lam_viec { get; set; }
            public object ly_do { get; set; }
        }

        public class DoiCa
        {
            public object ngay_can_doi { get; set; }
            public object ca_can_doi { get; set; }
            public object ngay_muon_doi { get; set; }
            public object ca_muon_doi { get; set; }
            public object ly_do { get; set; }
        }

        public class HoaHong
        {
            public object chu_ky { get; set; }
            public object time_hh { get; set; }
            public object item_mdt_date { get; set; }
            public object dt_money { get; set; }
            public object name_dt { get; set; }
            public object ly_do { get; set; }
        }

        public class KhieuNai
        {
        }

        public class LanhDaoDuyet
        {
            public string userName { get; set; }
            private string AvatarUser { get; set; }
            public string avatarUser {
                get {
                    if (AvatarUser != null)
                    {
                        return "https://chamcong.24hpay.vn/upload/employee/" + AvatarUser;

                    }
                    else
                    {
                        return "https://tinhluong.timviec365.vn/img/add.png";
                    }

                } 
                set {  AvatarUser = value; } 
            }
            public int idQLC { get; set; }
        }

        public class LichLamViec
        {
            public object lich_lam_viec { get; set; }
            public object thang_ap_dung { get; set; }
            public object ngay_bat_dau { get; set; }
            public object ca_la_viec { get; set; }
            public object ngay_lam_viec { get; set; }
            public object ly_do { get; set; }
        }

        public class LuanChuyenCongTac
        {
            public object cv_nguoi_lc { get; set; }
            public object pb_nguoi_lc { get; set; }
            public object noi_cong_tac { get; set; }
            public object noi_chuyen_den { get; set; }
            public object ly_do { get; set; }
        }

        public class NghiPhep
        {
            public List<object> nd { get; set; }
            public object loai_np { get; set; }
            public object ly_do { get; set; }
        }

        public class NghiPhepRaNgoai
        {
            public object loai_de_xuat { get; set; }
            public object bd_nghi { get; set; }
            public object kt_nghi { get; set; }
            public object ca_nghi { get; set; }
            public object time_bd_nghi { get; set; }
            public object time_kt_nghi { get; set; }
            public object ly_do { get; set; }
        }

        public class NghiThaiSan
        {
            public object ngaybatdau_nghi_ts { get; set; }
            public object ngayketthuc_nghi_ts { get; set; }
            public object ly_do { get; set; }
        }

        public class NhapNgayNhanLuong
        {
            public object ngay_nhan_luong { get; set; }
            public object ly_do { get; set; }
        }

        public class Root
        {
            public Data data { get; set; }
            public object error { get; set; }
        }

        public class SuaChuaCoSoVatChat
        {
            public List<object> noi_dung { get; set; }
        }

        public class SuDungPhongHop
        {
            public object bd_hop { get; set; }
            public object end_hop { get; set; }
            public object phong_hop { get; set; }
            public object ly_do { get; set; }
        }

        public class SuDungXeCong
        {
            public object bd_xe { get; set; }
            public object end_xe { get; set; }
            public object soluong_xe { get; set; }
            public object local_di { get; set; }
            public object local_den { get; set; }
        }

        public class TamUng
        {
            public long ngay_tam_ung { get; set; }
            public int sotien_tam_ung { get; set; }
            public string ly_do { get; set; }
        }

        public class TangCa
        {
            public object time_tc { get; set; }
            public object ly_do { get; set; }
        }

        public class TangLuong
        {
            public object mucluong_ht { get; set; }
            public object mucluong_tang { get; set; }
            public object date_tang_luong { get; set; }
            public object ly_do { get; set; }
        }

        public class ThamGiaDuAn
        {
            public object cv_nguoi_da { get; set; }
            public object pb_nguoi_da { get; set; }
            public object dx_da { get; set; }
            public object ly_do { get; set; }
        }

        public class ThanhToan
        {
            public object so_tien_tt { get; set; }
            public object ly_do { get; set; }
        }

        public class ThoiViec
        {
            public object ngaybatdau_tv { get; set; }
            public object ca_bdnghi { get; set; }
            public object ly_do { get; set; }
        }

        public class ThongTinChung
        {
            public NghiPhep nghi_phep { get; set; }
            public DoiCa doi_ca { get; set; }
            public TamUng tam_ung { get; set; }
            public CapPhatTaiSan cap_phat_tai_san { get; set; }
            public ThoiViec thoi_viec { get; set; }
            public TangLuong tang_luong { get; set; }
            public BoNhiem bo_nhiem { get; set; }
            public LuanChuyenCongTac luan_chuyen_cong_tac { get; set; }
            public ThamGiaDuAn tham_gia_du_an { get; set; }
            public TangCa tang_ca { get; set; }
            public NghiThaiSan nghi_thai_san { get; set; }
            public SuDungPhongHop su_dung_phong_hop { get; set; }
            public SuDungXeCong su_dung_xe_cong { get; set; }
            public SuaChuaCoSoVatChat sua_chua_co_so_vat_chat { get; set; }
            public XacNhanCong xac_nhan_cong { get; set; }
            public LichLamViec lich_lam_viec { get; set; }
            public HoaHong hoa_hong { get; set; }
            public KhieuNai khieu_nai { get; set; }
            public ThanhToan thanh_toan { get; set; }
            public ThuongPhat thuong_phat { get; set; }
            public DiMuonVeSom di_muon_ve_som { get; set; }
            public NghiPhepRaNgoai nghi_phep_ra_ngoai { get; set; }
            public NhapNgayNhanLuong nhap_ngay_nhan_luong { get; set; }
            public XinTaiTaiLieu xin_tai_tai_lieu { get; set; }
        }

        public class ThuongPhat
        {
            public object so_tien_tp { get; set; }
            public object time_tp { get; set; }
            public object nguoi_tp { get; set; }
            public object type_tp { get; set; }
            public object ly_do { get; set; }
        }

        public class XacNhanCong
        {
            public object time_xnc { get; set; }
            public object ca_xnc { get; set; }
            public object id_ca_xnc { get; set; }
            public object ly_do { get; set; }
        }

        public class XinTaiTaiLieu
        {
            public object ten_tai_lieu { get; set; }
            public object ly_do { get; set; }
        }


    }
}
