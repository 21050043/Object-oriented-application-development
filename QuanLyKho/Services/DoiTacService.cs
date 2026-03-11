using System;
using System.Collections.Generic;
using System.Linq;
using QuanLyKho.Interfaces;
using QuanLyKho.Models;

namespace QuanLyKho.Services
{
    /// <summary>
    /// DoiTacService - triển khai IRepository<DoiTac>.
    /// Quản lý CRUD đối tác/nhà cung cấp.
    /// </summary>
    public class DoiTacService : IRepository<DoiTac>
    {
        private List<DoiTac> _dsDoiTac;
        private readonly string _filePath;

        public DoiTacService(string dataDirectory)
        {
            _filePath = System.IO.Path.Combine(dataDirectory, "doitac.json");
            _dsDoiTac = new List<DoiTac>();
            Load();
        }

        public List<DoiTac> GetAll() => new List<DoiTac>(_dsDoiTac);

        public DoiTac? GetById(string id)
            => _dsDoiTac.FirstOrDefault(dt => dt.MaDT.Equals(id, StringComparison.OrdinalIgnoreCase));

        public void Add(DoiTac entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            if (GetById(entity.MaDT) != null)
                throw new InvalidOperationException($"Mã đối tác '{entity.MaDT}' đã tồn tại.");
            _dsDoiTac.Add(entity);
            Save();
        }

        public void Update(DoiTac entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            int index = _dsDoiTac.FindIndex(dt => dt.MaDT.Equals(entity.MaDT, StringComparison.OrdinalIgnoreCase));
            if (index < 0)
                throw new InvalidOperationException($"Không tìm thấy đối tác '{entity.MaDT}'.");
            _dsDoiTac[index] = entity;
            Save();
        }

        public void Delete(string id)
        {
            var dt = GetById(id);
            if (dt == null)
                throw new InvalidOperationException($"Không tìm thấy đối tác '{id}'.");
            _dsDoiTac.Remove(dt);
            Save();
        }

        public void Save() => FileHelper.Save(_filePath, _dsDoiTac);
        public void Load() => _dsDoiTac = FileHelper.Load<DoiTac>(_filePath);
    }
}
