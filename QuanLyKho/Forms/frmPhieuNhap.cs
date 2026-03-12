using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using QuanLyKho.Models;
using QuanLyKho.Services;

namespace QuanLyKho.Forms
{
    /// <summary>
    /// frmPhieuNhap - Lập phiếu nhập kho.
    /// Thể hiện Use Case "Lập phiếu nhập kho" và kịch bản kiểm tra số lượng.
    /// </summary>
    public class frmPhieuNhap : Form
    {
        private readonly SanPhamService _spService;
        private readonly DoiTacService _dtService;
        private readonly PhieuKhoService _phieuService;
        private readonly List<ChiTietPhieu> _dsChiTiet = new();

        // Header controls
        private TextBox _txtMaPhieu, _txtNguoiLap;
        private DateTimePicker _dtpNgayLap;
        private ComboBox _cboDoiTac;

        // Chi tiết phiếu
        private ComboBox _cboSanPham;
        private NumericUpDown _nudSoLuong;
        private TextBox _txtDonGia;
        private DataGridView _dgvChiTiet;
        private Label _lblTongTien;

        public frmPhieuNhap(SanPhamService spService, DoiTacService dtService, PhieuKhoService phieuService)
        {
            _spService = spService;
            _dtService = dtService;
            _phieuService = phieuService;
            Text = "📥  Lập Phiếu Nhập Kho";
            BuildUI();
        }

        private void BuildUI()
        {
            BackColor = AppTheme.BgDark;
            ForeColor = AppTheme.TextPrimary;
            Font = AppTheme.FontBody;

            var lblTieuDe = UIHelper.CreateSectionHeader("📥 LẬP PHIẾU NHẬP KHO");

            var split = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Panel1MinSize = 380, // Ép buộc kích thước tối thiểu
                FixedPanel = FixedPanel.Panel1,
                IsSplitterFixed = true,
                BorderStyle = BorderStyle.None
            };

            // ===== CỘT TRÁI: THÔNG TIN & NHẬP LIỆU ===== (CHIÊM 380px)
            var leftPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(16, 20, 10, 16), AutoScroll = true };
            
            var groupInfo = UIHelper.CreateCard();
            groupInfo.Dock = DockStyle.Top; groupInfo.Height = 310; groupInfo.Padding = new Padding(15);

            _txtMaPhieu = UIHelper.CreateInput(""); _txtMaPhieu.Text = _phieuService.SinhMaPhieu(true); _txtMaPhieu.ReadOnly = true;
            _dtpNgayLap = new DateTimePicker { Format = DateTimePickerFormat.Short, Value = DateTime.Now, Font = AppTheme.FontSubtitle, Dock = DockStyle.Top, Height = 35, Enabled = false };
            _txtNguoiLap = UIHelper.CreateInput("Họ tên người lập");
            _cboDoiTac = UIHelper.CreateComboBox();
            foreach (var dt in _dtService.GetAll()) _cboDoiTac.Items.Add(dt);
            if (_cboDoiTac.Items.Count > 0) _cboDoiTac.SelectedIndex = 0;

            groupInfo.Controls.Add(_cboDoiTac); groupInfo.Controls.Add(UIHelper.CreateLabel("Nhà Cung Cấp *"));
            groupInfo.Controls.Add(_txtNguoiLap); groupInfo.Controls.Add(UIHelper.CreateLabel("Người Lập Phiếu"));
            groupInfo.Controls.Add(_dtpNgayLap); groupInfo.Controls.Add(UIHelper.CreateLabel("Ngày Chứng Từ"));
            groupInfo.Controls.Add(_txtMaPhieu); groupInfo.Controls.Add(UIHelper.CreateLabel("Mã Phiếu (Auto)"));
            groupInfo.Controls.Add(new Label { Text = "THÔNG TIN CHUNG", Font = AppTheme.FontLabel, ForeColor = AppTheme.Accent, Dock = DockStyle.Top, Height = 30 });

            var groupItem = UIHelper.CreateCard();
            groupItem.Dock = DockStyle.Top; groupItem.Height = 290; groupItem.Padding = new Padding(15); groupItem.Margin = new Padding(0, 20, 0, 0);

            _cboSanPham = UIHelper.CreateComboBox();
            foreach (var sp in _spService.GetAll()) _cboSanPham.Items.Add(sp);
            if (_cboSanPham.Items.Count > 0) _cboSanPham.SelectedIndex = 0;
            _cboSanPham.SelectedIndexChanged += (s, e) => { 
                if (_cboSanPham.SelectedItem is SanPham sp) 
                    _txtDonGia.Text = sp.DonGia.ToString("N0"); 
            };

            _nudSoLuong = new NumericUpDown { Minimum = 1, Maximum = 999999, Font = new Font("Segoe UI", 14f, FontStyle.Bold), Dock = DockStyle.Top, Height = 45, BackColor = AppTheme.BgInput, ForeColor = AppTheme.AccentGreen, TextAlign = HorizontalAlignment.Center };
            _txtDonGia = UIHelper.CreateInput("Đơn giá nhập");
            var btnThemDong = UIHelper.CreateButton("✚ THÊM VÀO PHIẾU", AppTheme.AccentGreen);
            btnThemDong.Dock = DockStyle.Bottom; btnThemDong.Height = 50; btnThemDong.Font = AppTheme.FontSubtitle;
            btnThemDong.Click += BtnThemDong_Click;

            groupItem.Controls.Add(btnThemDong);
            groupItem.Controls.Add(_txtDonGia); groupItem.Controls.Add(UIHelper.CreateLabel("Đơn Giá (đ)"));
            groupItem.Controls.Add(_nudSoLuong); groupItem.Controls.Add(UIHelper.CreateLabel("Số Lượng Nhập"));
            groupItem.Controls.Add(_cboSanPham); groupItem.Controls.Add(UIHelper.CreateLabel("Sản Phẩm *"));
            groupItem.Controls.Add(new Label { Text = "CHI TIẾT MẶT HÀNG", Font = AppTheme.FontLabel, ForeColor = AppTheme.Accent, Dock = DockStyle.Top, Height = 30 });

            leftPanel.Controls.Add(groupItem);
            leftPanel.Controls.Add(UIHelper.CreateSpacer(10));
            leftPanel.Controls.Add(groupInfo);

            // ===== CỘT PHẢI: CHI TIẾT HÓA ĐƠN =====
            var rightPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10, 20, 20, 16) };
            var invoiceCard = UIHelper.CreateCard();
            invoiceCard.Dock = DockStyle.Fill;
            invoiceCard.Padding = new Padding(1);

            _dgvChiTiet = UIHelper.CreateDataGridView();
            _dgvChiTiet.Columns.Add("TenSP", "MẶT HÀNG");
            _dgvChiTiet.Columns.Add("SoLuong", "SL");
            _dgvChiTiet.Columns.Add("DonGia", "ĐƠN GIÁ");
            _dgvChiTiet.Columns.Add("ThanhTien", "THÀNH TIỀN");
            _dgvChiTiet.Columns["TenSP"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            _dgvChiTiet.Columns["SoLuong"].Width = 60;
            _dgvChiTiet.Columns["ThanhTien"].DefaultCellStyle.Format = "N0";
            _dgvChiTiet.Columns["ThanhTien"].Width = 120;
            
            _dgvChiTiet.KeyDown += (s, e) => {
                if (e.KeyCode == Keys.Delete && _dgvChiTiet.SelectedRows.Count > 0) {
                    int idx = _dgvChiTiet.SelectedRows[0].Index;
                    _dsChiTiet.RemoveAt(idx); _dgvChiTiet.Rows.RemoveAt(idx); CapNhatTongTien();
                }
            };

            var bottomAction = new TableLayoutPanel { Dock = DockStyle.Bottom, Height = 110, RowCount = 1, ColumnCount = 2, BackColor = Color.FromArgb(22, 22, 38), Padding = new Padding(20, 10, 20, 10) };
            bottomAction.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60f));
            bottomAction.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40f));

            _lblTongTien = new Label { Text = "TỔNG: 0 đ", Font = new Font("Segoe UI", 22f, FontStyle.Bold), ForeColor = AppTheme.AccentGreen, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft };
            var btnSavePhieu = UIHelper.CreateButton("💾 LƯU PHIẾU", AppTheme.Accent);
            btnSavePhieu.Dock = DockStyle.Fill; btnSavePhieu.Font = AppTheme.FontSubtitle;
            btnSavePhieu.Click += BtnLuu_Click;

            bottomAction.Controls.Add(_lblTongTien, 0, 0);
            bottomAction.Controls.Add(btnSavePhieu, 1, 0);

            invoiceCard.Controls.Add(_dgvChiTiet);
            invoiceCard.Controls.Add(new Label { Text = "DANH SÁCH CHI TIẾT PHIẾU NHẬP", Font = AppTheme.FontLabel, ForeColor = AppTheme.TextSecondary, Dock = DockStyle.Top, Height = 45, TextAlign = ContentAlignment.MiddleCenter });
            invoiceCard.Controls.Add(bottomAction);

            rightPanel.Controls.Add(invoiceCard);

            split.Panel1.Controls.Add(leftPanel);
            split.Panel2.Controls.Add(rightPanel);
            Controls.Add(split);
            Controls.Add(lblTieuDe);

            split.SplitterDistance = 380; // Ép buộc giá trị sau khi đã add vào Controls
        }

        private Label BuildVerticalLabel(string text)
            => new Label { Text = text, ForeColor = AppTheme.TextSecondary, Font = AppTheme.FontSmall, AutoSize = true };

        private void BtnThemDong_Click(object? sender, EventArgs e)
        {
            if (_cboSanPham.SelectedItem is not SanPham sp) { UIHelper.ShowError("Chọn sản phẩm!"); return; }
            int soLuong = (int)_nudSoLuong.Value;
            string donGiaStr = UIHelper.GetTextValue(_txtDonGia).Replace(",", "").Replace(".", "");
            
            if (!double.TryParse(donGiaStr, out double donGia) || donGia < 0)
            { UIHelper.ShowError("Đơn giá không hợp lệ!"); return; }

            var ct = new ChiTietPhieu(sp, soLuong, donGia);
            _dsChiTiet.Add(ct);
            _dgvChiTiet.Rows.Add(sp.TenSP, soLuong, donGia.ToString("N0"), ct.TinhThanhTien().ToString("N0"));
            CapNhatTongTien();
        }

        private void BtnLuu_Click(object? sender, EventArgs e)
        {
            if (_dsChiTiet.Count == 0) { UIHelper.ShowError("Vui lòng thêm ít nhất một dòng hàng hóa!"); return; }
            if (_cboDoiTac.SelectedItem is not DoiTac doiTac) { UIHelper.ShowError("Chọn nhà cung cấp!"); return; }

            try
            {
                string maPhieu = UIHelper.GetTextValue(_txtMaPhieu);
                string nguoiLap = UIHelper.GetTextValue(_txtNguoiLap);
                
                var phieu = new PhieuNhap(
                    maPhieu,
                    _dtpNgayLap.Value,
                    string.IsNullOrEmpty(nguoiLap) ? "Thủ kho" : nguoiLap,
                    doiTac
                );
                foreach (var ct in _dsChiTiet)
                    phieu.ThemChiTiet(ct);

                _phieuService.LuuPhieu(phieu); // Gọi CapNhatTonKho() qua polymorphism!

                UIHelper.ShowSuccess($"✅ Lưu phiếu nhập {phieu.MaPhieu} thành công!\nĐã cập nhật tồn kho.");
                ResetForm();
            }
            catch (Exception ex) { UIHelper.ShowError(ex.Message); }
        }

        private void CapNhatTongTien()
        {
            double tong = 0;
            foreach (var ct in _dsChiTiet) tong += ct.TinhThanhTien();
            _lblTongTien.Text = $"TỔNG TIỀN: {tong:N0} đ";
        }

        private void ResetForm()
        {
            _dsChiTiet.Clear();
            _dgvChiTiet.Rows.Clear();
            _lblTongTien.Text = "TỔNG TIỀN: 0 đ";
            _txtMaPhieu.Text = _phieuService.SinhMaPhieu(true);
            UIHelper.ResetInput(_txtNguoiLap);
            _dtpNgayLap.Value = DateTime.Now;
            if (_cboDoiTac.Items.Count > 0) _cboDoiTac.SelectedIndex = 0;

            // Reload danh sách SP (tồn kho có thể đã thay đổi)
            _cboSanPham.Items.Clear();
            foreach (var sp in _spService.GetAll())
                _cboSanPham.Items.Add(sp);
            if (_cboSanPham.Items.Count > 0) _cboSanPham.SelectedIndex = 0;
        }
    }
}
