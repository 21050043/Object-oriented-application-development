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
        private Button _btnHome;
        private Button _btnSanPham;
        private Button _btnDoiTac;
        private Button _btnNhapKho;
        private Button _btnXuatKho;
        private Button _btnBaoCao;
        private Button _btnLichSu;
        private Button _btnHuongDan;
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
            _btnHome     = CreateNavButton("🏠  Trang Chủ");
            _btnSanPham  = CreateNavButton("📋  Sản Phẩm");
            _btnDoiTac   = CreateNavButton("🏢  Đối Tác");
            _btnNhapKho  = CreateNavButton("📥  Nhập Kho");
            _btnXuatKho  = CreateNavButton("📤  Xuất Kho");
            _btnBaoCao   = CreateNavButton("📊  Báo Cáo Tồn Kho");
            _btnLichSu   = CreateNavButton("🕰️  Lịch Sử Phiếu");
            _btnHuongDan = CreateNavButton("📖  Hướng Dẫn");

            _btnHome.Click    += (s, e) => { 
                _contentPanel.Controls.Clear(); 
                _contentPanel.Controls.Add(CreateDashboard()); 
                SetActiveButton(_btnHome); 
                _lblTitle.Text = "Trang Chủ"; 
            };
            _btnSanPham.Click += (s, e) => ShowForm(new frmSanPham(_sanPhamService, _doiTacService), _btnSanPham);
            _btnDoiTac.Click  += (s, e) => ShowForm(new frmDoiTac(_doiTacService), _btnDoiTac);
            _btnNhapKho.Click += (s, e) => ShowForm(new frmPhieuNhap(_sanPhamService, _doiTacService, _phieuKhoService), _btnNhapKho);
            _btnXuatKho.Click += (s, e) => ShowForm(new frmPhieuXuat(_sanPhamService, _doiTacService, _phieuKhoService), _btnXuatKho);
            _btnBaoCao.Click  += (s, e) => ShowForm(new frmBaoCao(_phieuKhoService, _sanPhamService), _btnBaoCao);
            _btnLichSu.Click  += (s, e) => ShowForm(new frmLichSuPhieu(_phieuKhoService), _btnLichSu);
            _btnHuongDan.Click += (s, e) => ShowForm(new frmHuongDan(), _btnHuongDan);

            _sidebar.Controls.Add(_btnHuongDan);
            _sidebar.Controls.Add(_btnLichSu);
            _sidebar.Controls.Add(_btnBaoCao);
            _sidebar.Controls.Add(_btnXuatKho);
            _sidebar.Controls.Add(_btnNhapKho);
            _sidebar.Controls.Add(_btnDoiTac);
            _sidebar.Controls.Add(_btnSanPham);
            _sidebar.Controls.Add(_btnHome);
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

            // Dashboard screen (Trang chủ)
            var dashboard = CreateDashboard();
            _contentPanel.Controls.Add(dashboard);
            SetActiveButton(_btnHome);

            // ===== Lắp vào form =====
            Controls.Add(_contentPanel);
            Controls.Add(_header);
            Controls.Add(_sidebar);

            ResumeLayout(true);
            PerformLayout();
        }

        private Panel CreateDashboard()
        {
            var panel = new Panel { Dock = DockStyle.Fill, BackColor = AppTheme.BgDark, Padding = new Padding(10, 40, 10, 0) };

            var lblHello = new Label
            {
                Text = "XIN CHÀO, THỦ KHO! 👋",
                Font = AppTheme.FontSubtitle,
                ForeColor = AppTheme.TextSecondary,
                Dock = DockStyle.Top,
                Height = 30,
                TextAlign = ContentAlignment.TopCenter
            };

            var lblWelcome = new Label
            {
                Text = "Tình Hình Kho Hàng Hôm Nay",
                Font = AppTheme.FontTitle,
                ForeColor = AppTheme.TextPrimary,
                Dock = DockStyle.Top,
                Height = 60,
                TextAlign = ContentAlignment.TopCenter
            };

            // Container cho các cards thống kê
            var statsContainer = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 200,
                FlowDirection = FlowDirection.LeftToRight,
                Padding = new Padding(30, 0, 0, 0),
                BackColor = Color.Transparent
            };

            // Lấy dữ liệu thực tế từ Services
            int totalSP = _sanPhamService.GetAll().Count;
            int totalDT = _doiTacService.GetAll().Count;
            int lowStock = _sanPhamService.GetAll().Count(s => s.SoLuongTon <= 5);

            statsContainer.Controls.Add(CreateStatCard("SẢN PHẨM", totalSP.ToString(), "📦", AppTheme.Accent));
            statsContainer.Controls.Add(CreateStatCard("ĐỐI TÁC", totalDT.ToString(), "🏢", AppTheme.AccentGreen));
            statsContainer.Controls.Add(CreateStatCard("CẦN NHẬP HÀNG", lowStock.ToString(), "⚠️", AppTheme.AccentRed));

            var lblTip = new Label
            {
                Text = "💡 Mẹo: Bạn có thể vào mục 'Lịch Sử Phiếu' để xem lại hoặc xóa các phiếu bị nhập nhầm.",
                Font = AppTheme.FontBody,
                ForeColor = AppTheme.TextSecondary,
                Dock = DockStyle.Bottom,
                Height = 100,
                TextAlign = ContentAlignment.MiddleCenter
            };

            panel.Controls.Add(lblTip);
            panel.Controls.Add(statsContainer);
            panel.Controls.Add(lblWelcome);
            panel.Controls.Add(lblHello);
            return panel;
        }

        private Panel CreateStatCard(string title, string value, string icon, Color color)
        {
            var card = UIHelper.CreateCard();
            card.Size = new Size(250, 150);
            card.Margin = new Padding(0, 0, 25, 0);

            var lblIcon = new Label { Text = icon, Font = new Font("Segoe UI", 28f), ForeColor = color, Location = new Point(20, 20), Size = new Size(60, 60), TextAlign = ContentAlignment.MiddleCenter };
            var lblVal = new Label { Text = value, Font = new Font("Segoe UI", 24f, FontStyle.Bold), ForeColor = AppTheme.TextPrimary, Location = new Point(90, 20), Size = new Size(140, 50), TextAlign = ContentAlignment.MiddleRight };
            var lblTitle = new Label { Text = title, Font = AppTheme.FontLabel, ForeColor = AppTheme.TextSecondary, Location = new Point(20, 90), Size = new Size(210, 30), TextAlign = ContentAlignment.MiddleRight };

            card.Controls.Add(lblIcon);
            card.Controls.Add(lblVal);
            card.Controls.Add(lblTitle);
            return card;
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
                .Replace("📤", "").Replace("📊", "").Replace("🕰️", "").Replace("📖", "").Trim();

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
