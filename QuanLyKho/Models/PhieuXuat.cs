using System;
using QuanLyKho.Models;
using Newtonsoft.Json;

namespace QuanLyKho.Models
{
    /// <summary>
    /// Lớp PhieuXuat - KẾ THỪA lớp cha PhieuKho.
    /// Ghi đè (override) CapNhatTonKho(): TRỪ số lượng khỏi tồn kho.
    /// Đây là biểu hiện của TÍNH ĐA HÌNH (Polymorphism).
    /// </summary>
    public class PhieuXuat : PhieuKho
    {
        // ===== THUỘC TÍNH BỔ SUNG (riêng của PhieuXuat) =====
        private DoiTac _doiTacNhan;

        public DoiTac DoiTacNhan
        {
            get => _doiTacNhan;
            set => _doiTacNhan = value;
        }

        // ===== CONSTRUCTORS =====
        [JsonConstructor]
        public PhieuXuat() : base()
        {
            _doiTacNhan = new DoiTac();
        }

        public PhieuXuat(string maPhieu, DateTime ngayLap, string nguoiLap, DoiTac doiTacNhan)
            : base(maPhieu, ngayLap, nguoiLap)
        {
            _doiTacNhan = doiTacNhan ?? new DoiTac();
        }

        // ===== OVERRIDE: Tính đa hình =====

        /// <summary>
        /// Ghi đè phương thức trừu tượng: TRỪ số lượng khỏi từng sản phẩm.
        /// Ném ngoại lệ nếu tồn kho không đủ.
        /// </summary>
        public override void CapNhatTonKho()
        {
            // Kiểm tra trước khi trừ để đảm bảo toàn vẹn dữ liệu
            foreach (var chiTiet in DanhSachChiTiet)
            {
                if (!chiTiet.SanPhamGiaoDich.KiemTraDuHang(chiTiet.SoLuong))
                    throw new InvalidOperationException(
                        $"Không đủ hàng trong kho! Sản phẩm '{chiTiet.SanPhamGiaoDich.TenSP}' " +
                        $"chỉ còn {chiTiet.SanPhamGiaoDich.SoLuongTon} {chiTiet.SanPhamGiaoDich.DonViTinh}, " +
                        $"yêu cầu xuất {chiTiet.SoLuong}.");
            }
            // Sau khi xác nhận đủ hàng, tiến hành trừ
            foreach (var chiTiet in DanhSachChiTiet)
            {
                chiTiet.SanPhamGiaoDich.GiamTonKho(chiTiet.SoLuong);
            }
        }

        /// <summary>Kiểm tra riêng lẻ cho một sản phẩm/số lượng trước khi thêm vào phiếu.</summary>
        public bool KiemTraTonKho(SanPham sanPham, int soLuong)
            => sanPham != null && sanPham.KiemTraDuHang(soLuong);

        public override string LoaiPhieu => "PHIẾU XUẤT";

        public override string ToString() =>
            $"[XUẤT] {base.ToString()} | Đơn vị nhận: {_doiTacNhan?.TenDT ?? "N/A"}";
    }
}
