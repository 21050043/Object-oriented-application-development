using System;
using System.Collections.Generic;
using System.Linq;
using QuanLyKho.Models;

namespace QuanLyKho.Services
{
    /// <summary>
    /// BaoCaoTonKhoItem - DTO (Data Transfer Object) dùng để truyền kết quả báo cáo lên UI.
    /// </summary>
    public class BaoCaoTonKhoItem
    {
        public string MaSP { get; set; } = string.Empty;
        public string TenSP { get; set; } = string.Empty;
        public string DonViTinh { get; set; } = string.Empty;
        public int TonDauKy { get; set; }
        public int TongNhap { get; set; }
        public int TongXuat { get; set; }
        public int TonCuoiKy => TonDauKy + TongNhap - TongXuat;
    }

    /// <summary>
    /// PhieuKhoService - quản lý danh sách phiếu kho và thuật toán tính tồn kho.
    /// Chứa logic nghiệp vụ cốt lõi: gọi CapNhatTonKho() (polymorphism) khi lưu phiếu.
    /// </summary>
    public class PhieuKhoService
    {
        private List<PhieuKho> _dsPhieu;
        private readonly string _filePath;
        private readonly SanPhamService _sanPhamService;

        public PhieuKhoService(string dataDirectory, SanPhamService sanPhamService)
        {
            _filePath = System.IO.Path.Combine(dataDirectory, "phieukho.json");
            _sanPhamService = sanPhamService;
            _dsPhieu = new List<PhieuKho>();
            Load();
        }

        /// <summary>
        /// Lưu phiếu mới và GỌI CapNhatTonKho() — đây là điểm thể hiện tính ĐA HÌNH.
        /// PhieuNhap → TangTonKho() | PhieuXuat → GiamTonKho()
        /// </summary>
        public void LuuPhieu(PhieuKho phieu)
        {
            if (phieu == null) throw new ArgumentNullException(nameof(phieu));
            if (!phieu.DanhSachChiTiet.Any())
                throw new InvalidOperationException("Phiếu phải có ít nhất một dòng chi tiết.");

            // GỌI ĐA HÌNH: tự động chọn đúng phương thức theo loại phiếu
            phieu.CapNhatTonKho();

            _dsPhieu.Add(phieu);
            Save();

        // Đồng bộ tồn kho sản phẩm xuống file
            _sanPhamService.Save();
        }

        /// <summary>
        /// Xóa phiếu và ĐẢO NGƯỢC cập nhật tồn kho.
        /// Phiếu Nhập bị xóa -> Trừ tồn kho | Phiếu Xuất bị xóa -> Cộng tồn kho.
        /// </summary>
        public void XoaPhieu(string maPhieu)
        {
            var phieu = _dsPhieu.FirstOrDefault(p => p.MaPhieu.Equals(maPhieu, StringComparison.OrdinalIgnoreCase));
            if (phieu == null) throw new InvalidOperationException($"Không tìm thấy phiếu '{maPhieu}'.");

            // Đảo ngược tồn kho
            foreach (var ct in phieu.DanhSachChiTiet)
            {
                if (phieu is PhieuNhap)
                    ct.SanPhamGiaoDich.GiamTonKho(ct.SoLuong);
                else if (phieu is PhieuXuat)
                    ct.SanPhamGiaoDich.TangTonKho(ct.SoLuong);
            }

            _dsPhieu.Remove(phieu);
            Save();
            _sanPhamService.Save();
        }

        public List<PhieuKho> GetAll() => new List<PhieuKho>(_dsPhieu);

        public List<PhieuNhap> GetAllPhieuNhap()
            => _dsPhieu.OfType<PhieuNhap>().ToList();

        public List<PhieuXuat> GetAllPhieuXuat()
            => _dsPhieu.OfType<PhieuXuat>().ToList();

        /// <summary>
        /// Thuật toán tính tồn kho động.
        /// Công thức: Tồn cuối kỳ = Tồn đầu kỳ + Tổng nhập - Tổng xuất.
        /// Duyệt toàn bộ danh sách phiếu, group by MaSP.
        /// </summary>
        public List<BaoCaoTonKhoItem> TinhTonKho(DateTime? tuNgay = null, DateTime? denNgay = null)
        {
            var dsSanPham = _sanPhamService.GetAll();
            var dsPhieuLoc = _dsPhieu.AsEnumerable();

            // Lọc theo khoảng thời gian nếu có
            if (tuNgay.HasValue)
                dsPhieuLoc = dsPhieuLoc.Where(p => p.NgayLap.Date >= tuNgay.Value.Date);
            if (denNgay.HasValue)
                dsPhieuLoc = dsPhieuLoc.Where(p => p.NgayLap.Date <= denNgay.Value.Date);

            var dsPhieuFinal = dsPhieuLoc.ToList();

            // Tính tổng nhập và xuất theo từng Mã SP
            var nhapTheoSP = dsPhieuFinal
                .OfType<PhieuNhap>()
                .SelectMany(p => p.DanhSachChiTiet)
                .GroupBy(ct => ct.SanPhamGiaoDich.MaSP)
                .ToDictionary(g => g.Key, g => g.Sum(ct => ct.SoLuong));

            var xuatTheoSP = dsPhieuFinal
                .OfType<PhieuXuat>()
                .SelectMany(p => p.DanhSachChiTiet)
                .GroupBy(ct => ct.SanPhamGiaoDich.MaSP)
                .ToDictionary(g => g.Key, g => g.Sum(ct => ct.SoLuong));

            return dsSanPham.Select(sp => new BaoCaoTonKhoItem
            {
                MaSP = sp.MaSP,
                TenSP = sp.TenSP,
                DonViTinh = sp.DonViTinh,
                TonDauKy = 0,  // Mở rộng: có thể truyền vào từ cấu hình
                TongNhap = nhapTheoSP.ContainsKey(sp.MaSP) ? nhapTheoSP[sp.MaSP] : 0,
                TongXuat = xuatTheoSP.ContainsKey(sp.MaSP) ? xuatTheoSP[sp.MaSP] : 0
            }).ToList();
        }

        /// <summary>Sinh mã phiếu tự động theo định dạng PN/PX + yyyyMMdd + sequence.</summary>
        public string SinhMaPhieu(bool laNhap)
        {
            string prefix = laNhap ? "PN" : "PX";
            string ngay = DateTime.Now.ToString("yyyyMMdd");
            int seq = _dsPhieu.Count(p =>
                p.MaPhieu.StartsWith(prefix + ngay)) + 1;
            return $"{prefix}{ngay}{seq:D3}";
        }

        public void Save() => FileHelper.Save(_filePath, _dsPhieu);

        public void Load() => _dsPhieu = FileHelper.Load<PhieuKho>(_filePath);
    }
}
