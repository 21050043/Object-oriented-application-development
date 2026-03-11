using QuanLyKho.Forms;
using QuanLyKho.Services;
using System.IO;

namespace QuanLyKho;

static class Program
{
    /// <summary>
    /// Điểm khởi đầu ứng dụng.
    /// Khởi tạo thư mục Data và tạo các Services theo DI pattern.
    /// </summary>
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        // Xác định thư mục Data (cùng cấp với executable)
        string dataDir = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, "Data");
        FileHelper.EnsureDataDirectory(dataDir);

        // Khởi tạo Services (Dependency Injection thủ công)
        var sanPhamService = new SanPhamService(dataDir);
        var doiTacService  = new DoiTacService(dataDir);
        var phieuKhoService = new PhieuKhoService(dataDir, sanPhamService);

        // Chạy ứng dụng với MainForm nhận Services qua constructor
        Application.Run(new MainForm(sanPhamService, doiTacService, phieuKhoService));
    }
}