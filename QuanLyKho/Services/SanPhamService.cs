using System;
using System.Collections.Generic;
using System.Linq;
using QuanLyKho.Interfaces;
using QuanLyKho.Models;

namespace QuanLyKho.Services
{
    /// <summary>
    /// SanPhamService - triển khai IRepository<SanPham>.
    /// Quản lý CRUD sản phẩm và persistance qua FileHelper.
    /// Áp dụng SRP: chỉ chịu trách nhiệm quản lý danh sách sản phẩm.
    /// </summary>
    public class SanPhamService : IRepository<SanPham>
    {
        private List<SanPham> _dsSanPham;
        private readonly string _filePath;

        public SanPhamService(string dataDirectory)
        {
            _filePath = System.IO.Path.Combine(dataDirectory, "sanpham.json");
            _dsSanPham = new List<SanPham>();
            Load();
        }

        public List<SanPham> GetAll() => new List<SanPham>(_dsSanPham);

        public SanPham? GetById(string id)
            => _dsSanPham.FirstOrDefault(sp => sp.MaSP.Equals(id, StringComparison.OrdinalIgnoreCase));

        public void Add(SanPham entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            if (GetById(entity.MaSP) != null)
                throw new InvalidOperationException($"Mã sản phẩm '{entity.MaSP}' đã tồn tại.");
            _dsSanPham.Add(entity);
            Save();
        }

        public void Update(SanPham entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            int index = _dsSanPham.FindIndex(sp => sp.MaSP.Equals(entity.MaSP, StringComparison.OrdinalIgnoreCase));
            if (index < 0)
                throw new InvalidOperationException($"Không tìm thấy sản phẩm '{entity.MaSP}'.");
            _dsSanPham[index] = entity;
            Save();
        }

        public void Delete(string id)
        {
            var sp = GetById(id);
            if (sp == null)
                throw new InvalidOperationException($"Không tìm thấy sản phẩm '{id}'.");
            _dsSanPham.Remove(sp);
            Save();
        }

        public void Save() => FileHelper.Save(_filePath, _dsSanPham);

        public void Load() => _dsSanPham = FileHelper.Load<SanPham>(_filePath);

        /// <summary>Tìm kiếm sản phẩm theo tên hoặc mã (cho use case <<include>> trong phiếu).</summary>
        public List<SanPham> TimKiem(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword)) return GetAll();
            string kw = keyword.Trim().ToLower();
            return _dsSanPham.Where(sp =>
                sp.MaSP.ToLower().Contains(kw) ||
                sp.TenSP.ToLower().Contains(kw) ||
                sp.DanhMuc.ToLower().Contains(kw)).ToList();
        }
    }
}
