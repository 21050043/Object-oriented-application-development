using System.Collections.Generic;

namespace QuanLyKho.Interfaces
{
    /// <summary>
    /// Interface IRepository - áp dụng nguyên tắc SOLID:
    /// - ISP (Interface Segregation Principle): tách biệt các thao tác CRUD.
    /// - DIP (Dependency Inversion Principle): tầng trên phụ thuộc abstraction, không phụ thuộc concretion.
    /// </summary>
    public interface IRepository<T>
    {
        /// <summary>Lấy toàn bộ danh sách.</summary>
        List<T> GetAll();

        /// <summary>Lấy một phần tử theo ID.</summary>
        T? GetById(string id);

        /// <summary>Thêm mới một phần tử.</summary>
        void Add(T entity);

        /// <summary>Cập nhật một phần tử đã có.</summary>
        void Update(T entity);

        /// <summary>Xóa một phần tử theo ID.</summary>
        void Delete(string id);

        /// <summary>Lưu toàn bộ dữ liệu xuống file.</summary>
        void Save();

        /// <summary>Tải toàn bộ dữ liệu từ file lên bộ nhớ.</summary>
        void Load();
    }
}
