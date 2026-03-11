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

            var lblTieuDe = UIHelper.CreateSectionHeader("📥  LẬP PHIẾU NHẬP KHO");

            // ===== INFO HEADER CARD =====
            var infoCard = UIHelper.CreateCard();
            infoCard.Dock = DockStyle.Top;
            infoCard.Height = 130;
            infoCard.Padding = new Padding(16, 10, 16, 10);

            var tblInfo = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4,
                RowCount = 2,
                BackColor = Color.Transparent
            };
            tblInfo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));
            tblInfo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));
            tblInfo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));
            tblInfo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));

            string maPhieu = _phieuService.SinhMaPhieu(true);
            _txtMaPhieu = UIHelper.CreateInput("");
            _txtMaPhieu.Text = maPhieu;
            _txtMaPhieu.ReadOnly = true;
            _txtMaPhieu.BackColor = AppTheme.BgInput;

            _dtpNgayLap = new DateTimePicker
            {
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Now,
                Font = AppTheme.FontBody,
                BackColor = AppTheme.BgInput,
                ForeColor = AppTheme.TextPrimary,
                Dock = DockStyle.Fill,
                CalendarMonthBackground = AppTheme.BgCard,
                CalendarForeColor = AppTheme.TextPrimary
            };

            _txtNguoiLap = UIHelper.CreateInput("Tên thủ kho");
            _cboDoiTac = UIHelper.CreateComboBox();
            foreach (var dt in _dtService.GetAll())
                _cboDoiTac.Items.Add(dt);
            if (_cboDoiTac.Items.Count > 0) _cboDoiTac.SelectedIndex = 0;

            tblInfo.Controls.Add(UIHelper.CreateLabel("Mã Phiếu"),   0, 0);
            tblInfo.Controls.Add(UIHelper.CreateLabel("Ngày Lập"),   1, 0);
            tblInfo.Controls.Add(UIHelper.CreateLabel("Người Lập"),  2, 0);
            tblInfo.Controls.Add(UIHelper.CreateLabel("Nhà Cung Cấp"), 3, 0);
            tblInfo.Controls.Add(_txtMaPhieu,   0, 1);
            tblInfo.Controls.Add(_dtpNgayLap,   1, 1);
            tblInfo.Controls.Add(_txtNguoiLap,  2, 1);
            tblInfo.Controls.Add(_cboDoiTac,    3, 1);
            infoCard.Controls.Add(tblInfo);

            // ===== CHI TIẾT PHIẾU =====
            var detailCard = UIHelper.CreateCard();
            detailCard.Dock = DockStyle.Fill;
            detailCard.Padding = new Padding(16, 10, 16, 10);

            // Hàng nhập chi tiết
            var inputRow = new TableLayoutPanel
            {
                Dock = DockStyle.Top, Height = 50,
                ColumnCount = 5, RowCount = 1,
                BackColor = Color.Transparent
            };
            inputRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40f));
            inputRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15f));
            inputRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20f));
            inputRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15f));
            inputRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10f));

            _cboSanPham = UIHelper.CreateComboBox();
            foreach (var sp in _spService.GetAll())
                _cboSanPham.Items.Add(sp);
            if (_cboSanPham.Items.Count > 0) _cboSanPham.SelectedIndex = 0;
            _cboSanPham.SelectedIndexChanged += (s, e) =>
            {
                if (_cboSanPham.SelectedItem is SanPham sp)
                    _txtDonGia.Text = sp.DonGia.ToString("N0");
            };

            _nudSoLuong = new NumericUpDown
            {
                Minimum = 1, Maximum = 999999, Value = 1,
                Font = AppTheme.FontBody, Dock = DockStyle.Fill,
                BackColor = AppTheme.BgInput, ForeColor = AppTheme.TextPrimary,
                BorderStyle = BorderStyle.FixedSingle
            };
            _txtDonGia = UIHelper.CreateInput("Đơn giá");

            var btnThemDong = UIHelper.CreateButton("+ Thêm dòng", AppTheme.AccentGreen);
            btnThemDong.Dock = DockStyle.Fill;
            btnThemDong.Click += BtnThemDong_Click;

            var lblSP   = BuildVerticalLabel("Sản Phẩm");
            var lblSL   = BuildVerticalLabel("Số Lượng");
            var lblDG   = BuildVerticalLabel("Đơn Giá");

            inputRow.Controls.Add(_cboSanPham, 0, 0);
            inputRow.Controls.Add(_nudSoLuong, 1, 0);
            inputRow.Controls.Add(_txtDonGia,  2, 0);
            inputRow.Controls.Add(btnThemDong, 3, 0);

            // DataGridView chi tiết phiếu
            _dgvChiTiet = UIHelper.CreateDataGridView();
            _dgvChiTiet.Columns.Add("TenSP",   "Sản Phẩm");
            _dgvChiTiet.Columns.Add("SoLuong", "Số Lượng");
            _dgvChiTiet.Columns.Add("DonGia",  "Đơn Giá");
            _dgvChiTiet.Columns.Add("ThanhTien","Thành Tiền");
            _dgvChiTiet.Columns["TenSP"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            _dgvChiTiet.Columns["ThanhTien"].DefaultCellStyle.Format = "N0";

            // Delete row khi nhấn Delete
            _dgvChiTiet.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Delete && _dgvChiTiet.SelectedRows.Count > 0)
                {
                    int idx = _dgvChiTiet.SelectedRows[0].Index;
                    _dsChiTiet.RemoveAt(idx);
                    _dgvChiTiet.Rows.RemoveAt(idx);
                    CapNhatTongTien();
                }
            };

            // Footer
            var footer = new Panel { Dock = DockStyle.Bottom, Height = 50, BackColor = Color.Transparent };
            _lblTongTien = new Label
            {
                Text = "TỔNG TIỀN: 0 đ",
                Font = AppTheme.FontSubtitle,
                ForeColor = AppTheme.AccentGreen,
                AutoSize = true
            };
            var btnLuu = UIHelper.CreateButton("💾  Lưu Phiếu Nhập", AppTheme.Accent);
            btnLuu.Width = 180;
            btnLuu.Dock = DockStyle.Right;
            btnLuu.Click += BtnLuu_Click;

            footer.Controls.Add(_lblTongTien);
            footer.Controls.Add(btnLuu);

            detailCard.Controls.Add(footer);
            detailCard.Controls.Add(_dgvChiTiet);
            detailCard.Controls.Add(UIHelper.CreateSpacer(6));
            detailCard.Controls.Add(inputRow);
            detailCard.Controls.Add(UIHelper.CreateLabel("Chi Tiết Hàng Hóa Nhập"));

            Controls.Add(detailCard);
            Controls.Add(infoCard);
            Controls.Add(lblTieuDe);
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
