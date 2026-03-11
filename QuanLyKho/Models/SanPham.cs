using System;
using Newtonsoft.Json;

namespace QuanLyKho.Models
{
    /// <summary>
    /// Lớp SanPham - thực thể lưu trữ thông tin mặt hàng trong kho.
    /// Áp dụng tính ĐÓNG GÓI (Encapsulation): fields private, 
    /// truy cập qua Properties có validation.
    /// </summary>
    public class SanPham
    {
        // ===== PRIVATE FIELDS (Tính đóng gói) =====
        private string _maSP;
        private string _tenSP;
        private string _donViTinh;
        private double _donGia;
        private int _soLuongTon;

        // ===== PROPERTIES với Validation =====
        public string MaSP
        {
            get => _maSP;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Mã sản phẩm không được để trống.");
                _maSP = value.Trim().ToUpper();
            }
        }

        public string TenSP
        {
            get => _tenSP;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Tên sản phẩm không được để trống.");
                _tenSP = value.Trim();
            }
        }

        public string DonViTinh
        {
            get => _donViTinh;
            set => _donViTinh = string.IsNullOrWhiteSpace(value) ? "Cái" : value.Trim();
        }

        public double DonGia
        {
            get => _donGia;
            set
            {
                if (value < 0)
                    throw new ArgumentException("Đơn giá không được âm.");
                _donGia = value;
            }
        }

        public int SoLuongTon
        {
            get => _soLuongTon;
            set
            {
                if (value < 0)
                    throw new ArgumentException("Số lượng tồn không được âm.");
                _soLuongTon = value;
            }
        }

        // ===== CONSTRUCTORS =====

        /// <summary>Constructor mặc định (bắt buộc cho JSON deserialization)</summary>
        [JsonConstructor]
        public SanPham()
        {
            _maSP = string.Empty;
            _tenSP = string.Empty;
            _donViTinh = "Cái";
            _donGia = 0;
            _soLuongTon = 0;
        }

        /// <summary>Constructor đầy đủ tham số</summary>
        public SanPham(string maSP, string tenSP, string donViTinh, double donGia)
        {
            MaSP = maSP;
            TenSP = tenSP;
            DonViTinh = donViTinh;
            DonGia = donGia;
            _soLuongTon = 0;
        }

        // ===== METHODS NGHIỆP VỤ =====

        /// <summary>Tăng số lượng tồn kho khi nhập hàng.</summary>
        public void TangTonKho(int soLuong)
        {
            if (soLuong <= 0)
                throw new ArgumentException("Số lượng nhập phải lớn hơn 0.");
            _soLuongTon += soLuong;
        }

        /// <summary>Giảm số lượng tồn kho khi xuất hàng. Trả về false nếu không đủ hàng.</summary>
        public bool GiamTonKho(int soLuong)
        {
            if (soLuong <= 0)
                throw new ArgumentException("Số lượng xuất phải lớn hơn 0.");
            if (_soLuongTon < soLuong)
                return false;
            _soLuongTon -= soLuong;
            return true;
        }

        /// <summary>Kiểm tra có đủ hàng để xuất không.</summary>
        public bool KiemTraDuHang(int soLuong) => _soLuongTon >= soLuong;

        public override string ToString() =>
            $"[{MaSP}] {TenSP} | ĐVT: {DonViTinh} | Giá: {DonGia:N0}đ | Tồn: {SoLuongTon}";
    }
}
