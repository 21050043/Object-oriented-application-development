using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace QuanLyKho.Services
{
    /// <summary>
    /// Lớp FileHelper - tiện ích tĩnh xử lý JSON Serialization/Deserialization.
    /// Tách biệt hoàn toàn logic lưu trữ khỏi logic nghiệp vụ (nguyên tắc SRP - SOLID).
    /// </summary>
    public static class FileHelper
    {
        private static readonly JsonSerializerSettings _settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,   // Hỗ trợ polymorphic (PhieuNhap / PhieuXuat)
            Formatting = Formatting.Indented,           // JSON dễ đọc
            NullValueHandling = NullValueHandling.Ignore
        };

        /// <summary>
        /// Lưu danh sách đối tượng xuống file JSON.
        /// TypeNameHandling.Auto đảm bảo đa hình được giữ nguyên khi serialize.
        /// </summary>
        public static void Save<T>(string filePath, List<T> data)
        {
            try
            {
                string? directory = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                string json = JsonConvert.SerializeObject(data, _settings);
                File.WriteAllText(filePath, json, System.Text.Encoding.UTF8);
            }
            catch (Exception ex)
            {
                throw new IOException($"Lỗi khi lưu file '{filePath}': {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Tải danh sách đối tượng từ file JSON.
        /// Trả về danh sách rỗng nếu file chưa tồn tại (lần đầu chạy ứng dụng).
        /// </summary>
        public static List<T> Load<T>(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                    return new List<T>();

                string json = File.ReadAllText(filePath, System.Text.Encoding.UTF8);
                return JsonConvert.DeserializeObject<List<T>>(json, _settings) ?? new List<T>();
            }
            catch (Exception ex)
            {
                throw new IOException($"Lỗi khi đọc file '{filePath}': {ex.Message}", ex);
            }
        }

        /// <summary>Đảm bảo thư mục Data tồn tại khi khởi động ứng dụng.</summary>
        public static void EnsureDataDirectory(string dataPath)
        {
            if (!Directory.Exists(dataPath))
                Directory.CreateDirectory(dataPath);
        }
    }
}
