using System;
using Newtonsoft.Json;

namespace QuanLyKho.Models
{
    /// <summary>
    /// Lớp DoiTac - quản lý thông tin nhà cung cấp/đối tác giao dịch.
    /// Áp dụng tính ĐÓNG GÓI (Encapsulation).
    /// </summary>
    public class DoiTac
    {
        // ===== PRIVATE FIELDS =====
        private string _maDT;
        private string _tenDT;
        private string _dienThoai;
        private string _diaChi;

        // ===== PROPERTIES với Validation =====
        public string MaDT
        {
            get => _maDT;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Mã đối tác không được để trống.");
                _maDT = value.Trim().ToUpper();
            }
        }

        public string TenDT
        {
            get => _tenDT;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Tên đối tác không được để trống.");
                _tenDT = value.Trim();
            }
        }

        public string DienThoai
        {
            get => _dienThoai;
            set => _dienThoai = value?.Trim() ?? string.Empty;
        }

        public string DiaChi
        {
            get => _diaChi;
            set => _diaChi = value?.Trim() ?? string.Empty;
        }

        // ===== CONSTRUCTORS =====

        [JsonConstructor]
        public DoiTac()
        {
            _maDT = string.Empty;
            _tenDT = string.Empty;
            _dienThoai = string.Empty;
            _diaChi = string.Empty;
        }

        public DoiTac(string maDT, string tenDT, string dienThoai, string diaChi)
        {
            MaDT = maDT;
            TenDT = tenDT;
            DienThoai = dienThoai;
            DiaChi = diaChi;
        }

        public override string ToString() => $"[{MaDT}] {TenDT}";
    }
}
