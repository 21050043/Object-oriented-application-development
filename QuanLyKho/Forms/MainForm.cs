using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using QuanLyKho.Services;

namespace QuanLyKho.Forms
{
    /// <summary>
    /// MainForm - cửa sổ chính với Sidebar navigation và Panel nội dung.
    /// Dark theme gorgeous UI với animation hover sidebar.
    /// </summary>
    public class MainForm : Form
    {
        // ===== SERVICES (Dependency Injection) =====
        private readonly SanPhamService _sanPhamService;
        private readonly DoiTacService _doiTacService;
        private readonly PhieuKhoService _phieuKhoService;

        // ===== UI CONTROLS =====
        private Panel _sidebar;
        private Panel _header;
        private Panel _contentPanel;
        private Label _lblTitle;
        private Label _lblDateTime;
        private System.Windows.Forms.Timer _clockTimer;

        // Sidebar buttons
        private Button _btnSanPham;
        private Button _btnDoiTac;
        private Button _btnNhapKho;
        private Button _btnXuatKho;
        private Button _btnBaoCao;
        private Button _activeBtn;

        public MainForm(SanPhamService sanPhamService, DoiTacService doiTacService, PhieuKhoService phieuKhoService)
        {
            _sanPhamService = sanPhamService;
            _doiTacService = doiTacService;
            _phieuKhoService = phieuKhoService;
            InitializeComponent();
            StartClock();
        }

        private void InitializeComponent()
        {
            SuspendLayout();

            // Form settings
            Text = "🏭  Hệ Thống Quản Lý Kho Hàng  |  OOP Demo";
            Size = new Size(1200, 780);
            MinimumSize = new Size(1000, 680);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = AppTheme.BgDark;
            ForeColor = AppTheme.TextPrimary;
            Font = AppTheme.FontBody;

            // ===== SIDEBAR =====
            _sidebar = new Panel
            {
                Dock = DockStyle.Left,
                Width = AppTheme.SidebarWidth,
                BackColor = AppTheme.BgSidebar,
                Padding = new Padding(0, 10, 0, 10)
            };

            // Logo area trong sidebar
            var logoPanel = new Panel
            {
                Height = 90,
                Dock = DockStyle.Top,
                BackColor = Color.Transparent
            };
            var lblLogo = new Label
            {
                Text = "📦",
                Font = new Font("Segoe UI Emoji", 28f),
                ForeColor = AppTheme.Accent,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill,
                AutoSize = false
            };
            var lblAppName = new Label
            {
                Text = "KHO HÀNG OOP",
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = AppTheme.TextSecondary,
                TextAlign = ContentAlignment.BottomCenter,
                Dock = DockStyle.Bottom,
                Height = 22
            };
            logoPanel.Controls.Add(lblLogo);
            logoPanel.Controls.Add(lblAppName);

            // Separator
            var sep1 = new Panel { Height = 1, Dock = DockStyle.Top, BackColor = AppTheme.BorderColor, Margin = new Padding(10, 0, 10, 0) };

            // Nav buttons
            _btnSanPham = CreateNavButton("📋  Sản Phẩm");
            _btnDoiTac  = CreateNavButton("🏢  Đối Tác");
            _btnNhapKho = CreateNavButton("📥  Nhập Kho");
            _btnXuatKho = CreateNavButton("📤  Xuất Kho");
            _btnBaoCao  = CreateNavButton("📊  Báo Cáo Tồn Kho");

            _btnSanPham.Click += (s, e) => ShowForm(new frmSanPham(_sanPhamService), _btnSanPham);
            _btnDoiTac.Click  += (s, e) => ShowForm(new frmDoiTac(_doiTacService), _btnDoiTac);
            _btnNhapKho.Click += (s, e) => ShowForm(new frmPhieuNhap(_sanPhamService, _doiTacService, _phieuKhoService), _btnNhapKho);
            _btnXuatKho.Click += (s, e) => ShowForm(new frmPhieuXuat(_sanPhamService, _doiTacService, _phieuKhoService), _btnXuatKho);
            _btnBaoCao.Click  += (s, e) => ShowForm(new frmBaoCao(_phieuKhoService, _sanPhamService), _btnBaoCao);

            _sidebar.Controls.Add(_btnBaoCao);
            _sidebar.Controls.Add(_btnXuatKho);
            _sidebar.Controls.Add(_btnNhapKho);
            _sidebar.Controls.Add(_btnDoiTac);
            _sidebar.Controls.Add(_btnSanPham);
            _sidebar.Controls.Add(sep1);
            _sidebar.Controls.Add(logoPanel);

            // ===== HEADER =====
            _header = new Panel
            {
                Dock = DockStyle.Top,
                Height = AppTheme.HeaderHeight,
                BackColor = AppTheme.BgCard,
                Padding = new Padding(AppTheme.Padding, 0, AppTheme.Padding, 0)
            };
            _lblTitle = new Label
            {
                Text = "Trang Chủ",
                Font = AppTheme.FontSubtitle,
                ForeColor = AppTheme.TextPrimary,
                AutoSize = false,
                Dock = DockStyle.Left,
                Width = 400,
                TextAlign = ContentAlignment.MiddleLeft
            };
            _lblDateTime = new Label
            {
                Font = AppTheme.FontBody,
                ForeColor = AppTheme.TextSecondary,
                AutoSize = false,
                Dock = DockStyle.Right,
                Width = 250,
                TextAlign = ContentAlignment.MiddleRight
            };
            _header.Controls.Add(_lblTitle);
            _header.Controls.Add(_lblDateTime);

            // ===== CONTENT PANEL =====
            _contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = AppTheme.BgDark,
                Padding = new Padding(AppTheme.Padding)
            };

            // Welcome screen
            var welcomePanel = CreateWelcomePanel();
            _contentPanel.Controls.Add(welcomePanel);

            // ===== Lắp vào form =====
            Controls.Add(_contentPanel);
            Controls.Add(_header);
            Controls.Add(_sidebar);

            ResumeLayout(true);
            PerformLayout();
        }

        private Panel CreateWelcomePanel()
        {
            var panel = new Panel { Dock = DockStyle.Fill, BackColor = AppTheme.BgDark };

            var lblWelcome = new Label
            {
                Text = "📦  QUẢN LÝ KHO HÀNG",
                Font = AppTheme.FontTitle,
                ForeColor = AppTheme.Accent,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill,
                AutoSize = false
            };
            var lblSub = new Label
            {
                Text = "Chọn chức năng từ thanh bên trái để bắt đầu\n\nHệ thống áp dụng Lập trình Hướng Đối Tượng (OOP)\n" +
                       "✅ Encapsulation  ✅ Inheritance  ✅ Polymorphism  ✅ Abstraction",
                Font = AppTheme.FontBody,
                ForeColor = AppTheme.TextSecondary,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Bottom,
                Height = 80,
                AutoSize = false
            };
            panel.Controls.Add(lblWelcome);
            panel.Controls.Add(lblSub);
            return panel;
        }

        private Button CreateNavButton(string text)
        {
            var btn = new Button
            {
                Text = text,
                Dock = DockStyle.Top,
                Height = 48,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                ForeColor = AppTheme.TextSecondary,
                Font = AppTheme.FontBody,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(16, 0, 0, 0),
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 2, 0, 2)
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(40, AppTheme.Accent);
            btn.MouseEnter += (s, e) => { if (btn != _activeBtn) btn.ForeColor = AppTheme.TextPrimary; };
            btn.MouseLeave += (s, e) => { if (btn != _activeBtn) btn.ForeColor = AppTheme.TextSecondary; };
            return btn;
        }

        private void SetActiveButton(Button btn)
        {
            if (_activeBtn != null)
            {
                _activeBtn.ForeColor = AppTheme.TextSecondary;
                _activeBtn.BackColor = Color.Transparent;
            }
            _activeBtn = btn;
            _activeBtn.ForeColor = AppTheme.Accent;
            _activeBtn.BackColor = Color.FromArgb(30, AppTheme.Accent);
        }

        private void ShowForm(Form childForm, Button navBtn)
        {
            SetActiveButton(navBtn);
            _lblTitle.Text = childForm.Text.Replace("🗂️", "").Replace("🏢", "").Replace("📥", "")
                .Replace("📤", "").Replace("📊", "").Trim();

            // Xóa content cũ
            _contentPanel.Controls.Clear();

            // Embed form con vào content panel
            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;
            childForm.BackColor = AppTheme.BgDark;
            childForm.Show();

            _contentPanel.Controls.Add(childForm);
        }

        private void StartClock()
        {
            _clockTimer = new System.Windows.Forms.Timer { Interval = 1000 };
            _clockTimer.Tick += (s, e) =>
                _lblDateTime.Text = DateTime.Now.ToString("dddd, dd/MM/yyyy  HH:mm:ss");
            _clockTimer.Start();
            _lblDateTime.Text = DateTime.Now.ToString("dddd, dd/MM/yyyy  HH:mm:ss");
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            // Vẽ đường phân cách sidebar
            using var pen = new Pen(AppTheme.BorderColor, 1);
            e.Graphics.DrawLine(pen, AppTheme.SidebarWidth, 0, AppTheme.SidebarWidth, Height);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _clockTimer?.Dispose();
            base.Dispose(disposing);
        }
    }
}
