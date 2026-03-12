using System;
using QuanLyKho.Models;
using Newtonsoft.Json;

namespace QuanLyKho.Models
{
    /// <summary>
    /// Lớp PhieuNhap - KẾ THỪA lớp cha PhieuKho.
    /// Ghi đè (override) CapNhatTonKho(): CỘNG số lượng vào tồn kho.
    /// Đây là biểu hiện của TÍNH ĐA HÌNH (Polymorphism).
    /// </summary>
    public class PhieuNhap : PhieuKho
    {
        // ===== THUỘC TÍNH BỔ SUNG (riêng của PhieuNhap) =====
        private DoiTac _doiTacCungCap;

        public DoiTac DoiTacCungCap
        {
            get => _doiTacCungCap;
            set => _doiTacCungCap = value;
        }

        // ===== CONSTRUCTORS =====
        [JsonConstructor]
        public PhieuNhap() : base()
        {
            _doiTacCungCap = new DoiTac();
        }

        public PhieuNhap(string maPhieu, DateTime ngayLap, string nguoiLap, DoiTac doiTacCungCap)
            : base(maPhieu, ngayLap, nguoiLap)
        {
            _doiTacCungCap = doiTacCungCap ?? new DoiTac();
        }

        // ===== OVERRIDE: Tính đa hình =====

        /// <summary>
        /// Ghi đè phương thức trừu tượng: CỘNG số lượng vào từng sản phẩm trong phiếu.
        /// </summary>
        public override void CapNhatTonKho()
        {
            foreach (var chiTiet in DanhSachChiTiet)
            {
                chiTiet.SanPhamGiaoDich.TangTonKho(chiTiet.SoLuong);
                
                // Cập nhật chủ sở hữu mới cho sản phẩm dựa trên nhà cung cấp của phiếu nhập
                if (_doiTacCungCap != null && !string.IsNullOrEmpty(_doiTacCungCap.MaDT))
                {
                    chiTiet.SanPhamGiaoDich.MaDoiTacChuQuan = _doiTacCungCap.MaDT;
                }
            }
        }

        public override string LoaiPhieu => "PHIẾU NHẬP";

        public override string ToString() =>
            $"[NHẬP] {base.ToString()} | NCC: {_doiTacCungCap?.TenDT ?? "N/A"}";
    }
}
