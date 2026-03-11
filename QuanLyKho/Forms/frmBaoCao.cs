using System;
using System.Drawing;
using System.Windows.Forms;
using QuanLyKho.Services;

namespace QuanLyKho.Forms
{
    /// <summary>
    /// frmBaoCao - Xem báo cáo tồn kho.
    /// Công thức: Tồn cuối = Tồn đầu + Tổng Nhập - Tổng Xuất.
    /// Hỗ trợ lọc theo tên sản phẩm.
    /// </summary>
    public class frmBaoCao : Form
    {
        private readonly PhieuKhoService _phieuService;
        private readonly SanPhamService _spService;

        private DataGridView _dgv;
        private TextBox _txtTimKiem;
        private Label _lblSummary;
        private Button _btnLamMoi;

        public frmBaoCao(PhieuKhoService phieuService, SanPhamService spService)
        {
            _phieuService = phieuService;
            _spService = spService;
            Text = "📊  Báo Cáo Tồn Kho";
            BuildUI();
            LoadData();
        }

        private void BuildUI()
        {
            BackColor = AppTheme.BgDark;
            ForeColor = AppTheme.TextPrimary;
            Font = AppTheme.FontBody;

            var lblTieuDe = UIHelper.CreateSectionHeader("📊  BÁO CÁO THỐNG KÊ TỒN KHO");

            // Toolbar
            var toolbar = new Panel
            {
                Dock = DockStyle.Top, Height = 50,
                BackColor = AppTheme.BgCard,
                Padding = new Padding(12, 6, 12, 6)
            };
            _txtTimKiem = UIHelper.CreateInput("🔍  Tìm kiếm theo tên, mã sản phẩm...");
            _txtTimKiem.Dock = DockStyle.Left;
            _txtTimKiem.Width = 320;
            _txtTimKiem.TextChanged += (s, e) => LoadData(_txtTimKiem.Text);

            _btnLamMoi = UIHelper.CreateButton("🔄 Làm mới", AppTheme.Accent);
            _btnLamMoi.Width = 120;
            _btnLamMoi.Dock = DockStyle.Right;
            _btnLamMoi.Click += (s, e) => { _txtTimKiem.Text = ""; LoadData(); };

            toolbar.Controls.Add(_txtTimKiem);
            toolbar.Controls.Add(_btnLamMoi);

            // DataGridView
            _dgv = UIHelper.CreateDataGridView();
            _dgv.Dock = DockStyle.Fill;
            _dgv.ReadOnly = true;
            _dgv.AllowUserToAddRows = false;

            _dgv.Columns.Add("MaSP",     "Mã SP");
            _dgv.Columns.Add("TenSP",    "Tên Sản Phẩm");
            _dgv.Columns.Add("DonViTinh","Đơn Vị");
            _dgv.Columns.Add("TonDau",   "Tồn Đầu Kỳ");
            _dgv.Columns.Add("TongNhap", "Tổng Nhập");
            _dgv.Columns.Add("TongXuat", "Tổng Xuất");
            _dgv.Columns.Add("TonCuoi",  "Tồn Cuối Kỳ");

            _dgv.Columns["MaSP"].Width = 80;
            _dgv.Columns["TenSP"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            _dgv.Columns["DonViTinh"].Width = 70;
            _dgv.Columns["TonDau"].Width = 90;
            _dgv.Columns["TongNhap"].Width = 90;
            _dgv.Columns["TongXuat"].Width = 90;
            _dgv.Columns["TonCuoi"].Width = 100;

            // Header style đặc biệt cho cột tồn cuối
            _dgv.Columns["TonCuoi"].DefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9.5f, System.Drawing.FontStyle.Bold);

            // Summary bar
            var summaryBar = new Panel
            {
                Dock = DockStyle.Bottom, Height = 38,
                BackColor = AppTheme.BgCard,
                Padding = new Padding(16, 0, 16, 0)
            };
            _lblSummary = new Label
            {
                Font = AppTheme.FontBody,
                ForeColor = AppTheme.TextSecondary,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };
            summaryBar.Controls.Add(_lblSummary);

            Controls.Add(_dgv);
            Controls.Add(toolbar);
            Controls.Add(lblTieuDe);
            Controls.Add(summaryBar);
        }

        private void LoadData(string keyword = "")
        {
            _dgv.Rows.Clear();
            var baoCao = _phieuService.TinhTonKho();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                string kw = keyword.ToLower();
                baoCao = baoCao.FindAll(b =>
                    b.MaSP.ToLower().Contains(kw) ||
                    b.TenSP.ToLower().Contains(kw));
            }

            int tongSP = 0, tongNhap = 0, tongXuat = 0;
            foreach (var item in baoCao)
            {
                int row = _dgv.Rows.Add(
                    item.MaSP, item.TenSP, item.DonViTinh,
                    item.TonDauKy, item.TongNhap,
                    item.TongXuat, item.TonCuoiKy
                );

                // Tô màu theo mức tồn kho
                if (item.TonCuoiKy == 0)
                {
                    // Hết hàng - đỏ
                    _dgv.Rows[row].DefaultCellStyle.BackColor = Color.FromArgb(60, AppTheme.AccentRed);
                    _dgv.Rows[row].Cells["TonCuoi"].Style.ForeColor = AppTheme.AccentRed;
                }
                else if (item.TonCuoiKy < 10)
                {
                    // Sắp hết - vàng
                    _dgv.Rows[row].DefaultCellStyle.BackColor = Color.FromArgb(40, AppTheme.AccentYellow);
                    _dgv.Rows[row].Cells["TonCuoi"].Style.ForeColor = AppTheme.AccentYellow;
                }
                else
                {
                    // Đủ hàng - xanh
                    _dgv.Rows[row].Cells["TonCuoi"].Style.ForeColor = AppTheme.AccentGreen;
                }

                tongSP++;
                tongNhap += item.TongNhap;
                tongXuat += item.TongXuat;
            }

            _lblSummary.Text = $"Tổng sản phẩm: {tongSP}   |   "
                + $"Tổng nhập kỳ này: {tongNhap}   |   "
                + $"Tổng xuất kỳ này: {tongXuat}   |   "
                + $"Cập nhật lúc: {DateTime.Now:HH:mm:ss dd/MM/yyyy}";
        }
    }
}
