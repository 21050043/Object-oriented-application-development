using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace QuanLyKho.Models
{
    /// <summary>
    /// Lớp TRỪU TƯỢNG PhieuKho - nền tảng cho PhieuNhap và PhieuXuat.
    /// Áp dụng: TÍNH TRỪU TƯỢNG (Abstraction) + TÍNH ĐA HÌNH (Polymorphism).
    /// - Chứa phương thức trừu tượng CapNhatTonKho() buộc lớp con phải ghi đè.
    /// - Chứa thuộc tính chung: MaPhieu, NgayLap, NguoiLap, DanhSachChiTiet.
    /// </summary>
    public abstract class PhieuKho
    {
        // ===== PRIVATE FIELDS =====
        private string _maPhieu;
        private DateTime _ngayLap;
        private string _nguoiLap;
        private List<ChiTietPhieu> _danhSachChiTiet;

        // ===== PROPERTIES =====
        public string MaPhieu
        {
            get => _maPhieu;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Mã phiếu không được để trống.");
                _maPhieu = value.Trim().ToUpper();
            }
        }

        public DateTime NgayLap
        {
            get => _ngayLap;
            set => _ngayLap = value;
        }

        public string NguoiLap
        {
            get => _nguoiLap;
            set => _nguoiLap = string.IsNullOrWhiteSpace(value) ? "Thủ kho" : value.Trim();
        }

        public List<ChiTietPhieu> DanhSachChiTiet
        {
            get => _danhSachChiTiet;
            set => _danhSachChiTiet = value ?? new List<ChiTietPhieu>();
        }

        // ===== CONSTRUCTORS =====
        protected PhieuKho()
        {
            _maPhieu = string.Empty;
            _ngayLap = DateTime.Now;
            _nguoiLap = "Thủ kho";
            _danhSachChiTiet = new List<ChiTietPhieu>();
        }

        protected PhieuKho(string maPhieu, DateTime ngayLap, string nguoiLap)
        {
            MaPhieu = maPhieu;
            NgayLap = ngayLap;
            NguoiLap = nguoiLap;
            _danhSachChiTiet = new List<ChiTietPhieu>();
        }

        // ===== ABSTRACT METHOD (Tính đa hình) =====

        /// <summary>
        /// Phương thức trừu tượng - mỗi lớp con tự định nghĩa cách cập nhật tồn kho.
        /// PhieuNhap → cộng tồn kho | PhieuXuat → trừ tồn kho.
        /// </summary>
        public abstract void CapNhatTonKho();

        // ===== CONCRETE METHODS =====

        /// <summary>Thêm một dòng chi tiết vào phiếu.</summary>
        public void ThemChiTiet(ChiTietPhieu chiTiet)
        {
            if (chiTiet == null)
                throw new ArgumentNullException(nameof(chiTiet));
            _danhSachChiTiet.Add(chiTiet);
        }

        /// <summary>Xóa một dòng chi tiết khỏi phiếu (theo index).</summary>
        public void XoaChiTiet(int index)
        {
            if (index < 0 || index >= _danhSachChiTiet.Count)
                throw new ArgumentOutOfRangeException(nameof(index));
            _danhSachChiTiet.RemoveAt(index);
        }

        /// <summary>Tính tổng tiền của toàn bộ phiếu.</summary>
        public double TinhTongTien() => _danhSachChiTiet.Sum(ct => ct.TinhThanhTien());

        /// <summary>Lấy loại phiếu để hiển thị (override ở lớp con).</summary>
        public abstract string LoaiPhieu { get; }

        public override string ToString() =>
            $"[{LoaiPhieu}] Mã: {MaPhieu} | Ngày: {NgayLap:dd/MM/yyyy} | Người lập: {NguoiLap} | Tổng tiền: {TinhTongTien():N0}đ";
    }
}
