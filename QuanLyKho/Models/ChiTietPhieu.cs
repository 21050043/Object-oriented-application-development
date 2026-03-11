using System;
using Newtonsoft.Json;

namespace QuanLyKho.Models
{
    /// <summary>
    /// Lớp ChiTietPhieu - thực thể trung gian thể hiện từng dòng hàng hóa
    /// trong một phiếu giao dịch (Association giữa PhieuKho và SanPham).
    /// </summary>
    public class ChiTietPhieu
    {
        // ===== PRIVATE FIELDS =====
        private SanPham _sanPhamGiaoDich;
        private int _soLuong;
        private double _donGia;

        // ===== PROPERTIES =====
        public SanPham SanPhamGiaoDich
        {
            get => _sanPhamGiaoDich;
            set => _sanPhamGiaoDich = value ?? throw new ArgumentNullException(nameof(value), "Sản phẩm không được null.");
        }

        public int SoLuong
        {
            get => _soLuong;
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Số lượng phải lớn hơn 0.");
                _soLuong = value;
            }
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

        // ===== CONSTRUCTORS =====
        [JsonConstructor]
        public ChiTietPhieu()
        {
            _sanPhamGiaoDich = new SanPham();
            _soLuong = 1;
            _donGia = 0;
        }

        public ChiTietPhieu(SanPham sanPham, int soLuong, double donGia)
        {
            SanPhamGiaoDich = sanPham;
            SoLuong = soLuong;
            DonGia = donGia;
        }

        // ===== METHODS =====

        /// <summary>Tính thành tiền cho dòng chi tiết này.</summary>
        public double TinhThanhTien() => _soLuong * _donGia;

        public override string ToString() =>
            $"{_sanPhamGiaoDich?.TenSP ?? "N/A"} x{_soLuong} = {TinhThanhTien():N0}đ";
    }
}
