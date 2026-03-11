using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using QuanLyKho.Models;
using QuanLyKho.Services;

namespace QuanLyKho.Forms
{
    /// <summary>
    /// frmPhieuXuat - Lập phiếu xuất kho với kiểm tra tồn kho realtime.
    /// Kịch bản: nếu số lượng yêu cầu > tồn kho → cảnh báo đỏ và vô hiệu [Lưu].
    /// </summary>
    public class frmPhieuXuat : Form
    {
        private readonly SanPhamService _spService;
        private readonly DoiTacService _dtService;
        private readonly PhieuKhoService _phieuService;
        private readonly List<ChiTietPhieu> _dsChiTiet = new();

        private TextBox _txtMaPhieu, _txtNguoiLap;
        private DateTimePicker _dtpNgayLap;
        private ComboBox _cboDoiTac, _cboSanPham;
        private NumericUpDown _nudSoLuong;
        private TextBox _txtDonGia;
        private DataGridView _dgvChiTiet;
        private Label _lblTongTien, _lblCanhBao, _lblTonHienTai;
        private Button _btnLuu;

        public frmPhieuXuat(SanPhamService spService, DoiTacService dtService, PhieuKhoService phieuService)
        {
            _spService = spService;
            _dtService = dtService;
            _phieuService = phieuService;
            Text = "📤  Lập Phiếu Xuất Kho";
            BuildUI();
        }

        private void BuildUI()
        {
            BackColor = AppTheme.BgDark;
            ForeColor = AppTheme.TextPrimary;
            Font = AppTheme.FontBody;

            var lblTieuDe = UIHelper.CreateSectionHeader("📤  LẬP PHIẾU XUẤT KHO");

            // ===== INFO CARD =====
            var infoCard = UIHelper.CreateCard();
            infoCard.Dock = DockStyle.Top;
            infoCard.Height = 130;
            infoCard.Padding = new Padding(16, 10, 16, 10);

            var tblInfo = new TableLayoutPanel
            {
                Dock = DockStyle.Fill, ColumnCount = 4, RowCount = 2,
                BackColor = Color.Transparent
            };
            tblInfo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));
            tblInfo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));
            tblInfo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));
            tblInfo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));

            _txtMaPhieu = UIHelper.CreateInput(""); _txtMaPhieu.Text = _phieuService.SinhMaPhieu(false); _txtMaPhieu.ReadOnly = true;
            _dtpNgayLap = new DateTimePicker { Format = DateTimePickerFormat.Short, Value = DateTime.Now, Font = AppTheme.FontBody, Dock = DockStyle.Fill };
            _txtNguoiLap = UIHelper.CreateInput("Tên thủ kho");
            _cboDoiTac = UIHelper.CreateComboBox();
            foreach (var dt in _dtService.GetAll()) _cboDoiTac.Items.Add(dt);
            if (_cboDoiTac.Items.Count > 0) _cboDoiTac.SelectedIndex = 0;

            tblInfo.Controls.Add(UIHelper.CreateLabel("Mã Phiếu"),   0, 0); tblInfo.Controls.Add(_txtMaPhieu,  0, 1);
            tblInfo.Controls.Add(UIHelper.CreateLabel("Ngày Xuất"),  1, 0); tblInfo.Controls.Add(_dtpNgayLap, 1, 1);
            tblInfo.Controls.Add(UIHelper.CreateLabel("Người Lập"),  2, 0); tblInfo.Controls.Add(_txtNguoiLap, 2, 1);
            tblInfo.Controls.Add(UIHelper.CreateLabel("Đơn Vị Nhận"),3, 0); tblInfo.Controls.Add(_cboDoiTac,  3, 1);
            infoCard.Controls.Add(tblInfo);

            // ===== DETAIL CARD =====
            var detailCard = UIHelper.CreateCard();
            detailCard.Dock = DockStyle.Fill;
            detailCard.Padding = new Padding(16, 10, 16, 10);

            // Input row
            var inputRow = new TableLayoutPanel
            {
                Dock = DockStyle.Top, Height = 55,
                ColumnCount = 5, RowCount = 2,
                BackColor = Color.Transparent
            };
            inputRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 38f));
            inputRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16f));
            inputRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16f));
            inputRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20f));
            inputRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10f));

            _cboSanPham = UIHelper.CreateComboBox();
            LoadComboSanPham();
            _cboSanPham.SelectedIndexChanged += CboSanPham_Changed;

            _nudSoLuong = new NumericUpDown
            {
                Minimum = 1, Maximum = 999999, Value = 1,
                Font = AppTheme.FontBody, Dock = DockStyle.Fill,
                BackColor = AppTheme.BgInput, ForeColor = AppTheme.TextPrimary,
                BorderStyle = BorderStyle.FixedSingle
            };
            _nudSoLuong.ValueChanged += KiemTraTonKho;

            _txtDonGia = UIHelper.CreateInput("Đơn giá");

            // Label tồn hiện tại + cảnh báo
            _lblTonHienTai = new Label
            {
                Text = "", ForeColor = AppTheme.AccentGreen,
                Font = AppTheme.FontSmall, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft
            };
            _lblCanhBao = new Label
            {
                Text = "", ForeColor = AppTheme.AccentRed,
                Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft
            };

            var btnThemDong = UIHelper.CreateButton("+ Thêm", AppTheme.AccentGreen);
            btnThemDong.Dock = DockStyle.Fill;
            btnThemDong.Click += BtnThemDong_Click;

            inputRow.Controls.Add(_cboSanPham,   0, 0);
            inputRow.Controls.Add(_nudSoLuong,   1, 0);
            inputRow.Controls.Add(_txtDonGia,    2, 0);
            inputRow.Controls.Add(btnThemDong,   3, 0);
            inputRow.Controls.Add(_lblTonHienTai,0, 1);
            inputRow.Controls.Add(_lblCanhBao,   1, 1);
            inputRow.SetColumnSpan(_lblCanhBao, 2);

            // DataGridView
            _dgvChiTiet = UIHelper.CreateDataGridView();
            _dgvChiTiet.Columns.Add("TenSP", "Sản Phẩm");
            _dgvChiTiet.Columns.Add("SoLuong","Số Lượng");
            _dgvChiTiet.Columns.Add("DonGia", "Đơn Giá");
            _dgvChiTiet.Columns.Add("ThanhTien","Thành Tiền");
            _dgvChiTiet.Columns["TenSP"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            _dgvChiTiet.Columns["ThanhTien"].DefaultCellStyle.Format = "N0";
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
            _btnLuu = UIHelper.CreateButton("💾  Lưu Phiếu Xuất", AppTheme.AccentRed);
            _btnLuu.Width = 180; _btnLuu.Dock = DockStyle.Right;
            _btnLuu.Click += BtnLuu_Click;

            footer.Controls.Add(_lblTongTien);
            footer.Controls.Add(_btnLuu);

            detailCard.Controls.Add(footer);
            detailCard.Controls.Add(_dgvChiTiet);
            detailCard.Controls.Add(UIHelper.CreateSpacer(6));
            detailCard.Controls.Add(inputRow);
            detailCard.Controls.Add(UIHelper.CreateLabel("Chi Tiết Hàng Hóa Xuất"));

            Controls.Add(detailCard);
            Controls.Add(infoCard);
            Controls.Add(lblTieuDe);

            // Init trạng thái ban đầu
            if (_cboSanPham.Items.Count > 0) _cboSanPham.SelectedIndex = 0;
        }

        private void LoadComboSanPham()
        {
            _cboSanPham.Items.Clear();
            foreach (var sp in _spService.GetAll())
                _cboSanPham.Items.Add(sp);
        }

        private void CboSanPham_Changed(object? sender, EventArgs e)
        {
            if (_cboSanPham.SelectedItem is SanPham sp)
            {
                _txtDonGia.Text = sp.DonGia.ToString("N0");
                _lblTonHienTai.Text = $"Tồn kho hiện tại: {sp.SoLuongTon} {sp.DonViTinh}";
                _lblTonHienTai.ForeColor = sp.SoLuongTon > 0 ? AppTheme.AccentGreen : AppTheme.AccentRed;
                KiemTraTonKho(sender, e);
            }
        }

        private void KiemTraTonKho(object? sender, EventArgs e)
        {
            if (_cboSanPham.SelectedItem is not SanPham sp) return;
            int yeuCau = (int)_nudSoLuong.Value;
            if (sp.SoLuongTon < yeuCau)
            {
                _lblCanhBao.Text = $"⚠ Không đủ hàng! Tồn: {sp.SoLuongTon}, Yêu cầu: {yeuCau}";
                _lblCanhBao.ForeColor = AppTheme.AccentRed;
            }
            else
            {
                _lblCanhBao.Text = "✓ Đủ số lượng xuất";
                _lblCanhBao.ForeColor = AppTheme.AccentGreen;
            }
        }

        private void BtnThemDong_Click(object? sender, EventArgs e)
        {
            if (_cboSanPham.SelectedItem is not SanPham sp) { UIHelper.ShowError("Chọn sản phẩm!"); return; }
            int soLuong = (int)_nudSoLuong.Value;
            string donGiaStr = UIHelper.GetTextValue(_txtDonGia).Replace(",", "").Replace(".", "");

            if (!sp.KiemTraDuHang(soLuong))
            {
                UIHelper.ShowError($"Không đủ hàng!\nTồn kho '{sp.TenSP}': {sp.SoLuongTon} {sp.DonViTinh}\nYêu cầu xuất: {soLuong}");
                return;
            }
            if (!double.TryParse(donGiaStr, out double donGia) || donGia < 0)
            { UIHelper.ShowError("Đơn giá không hợp lệ!"); return; }

            var ct = new ChiTietPhieu(sp, soLuong, donGia);
            _dsChiTiet.Add(ct);
            _dgvChiTiet.Rows.Add(sp.TenSP, soLuong, donGia.ToString("N0"), ct.TinhThanhTien().ToString("N0"));
            CapNhatTongTien();
        }

        private void BtnLuu_Click(object? sender, EventArgs e)
        {
            if (_dsChiTiet.Count == 0) { UIHelper.ShowError("Thêm ít nhất một dòng hàng hóa!"); return; }
            if (_cboDoiTac.SelectedItem is not DoiTac doiTac) { UIHelper.ShowError("Chọn đơn vị nhận!"); return; }
            try
            {
                string maPhieu = UIHelper.GetTextValue(_txtMaPhieu);
                string nguoiLap = UIHelper.GetTextValue(_txtNguoiLap);

                var phieu = new PhieuXuat(
                    maPhieu,
                    _dtpNgayLap.Value,
                    string.IsNullOrEmpty(nguoiLap) ? "Thủ kho" : nguoiLap,
                    doiTac
                );
                foreach (var ct in _dsChiTiet)
                    phieu.ThemChiTiet(ct);

                _phieuService.LuuPhieu(phieu); // Gọi PhieuXuat.CapNhatTonKho() qua polymorphism!
                UIHelper.ShowSuccess($"✅ Lưu phiếu xuất {phieu.MaPhieu} thành công!\nĐã trừ tồn kho.");
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
            _dsChiTiet.Clear(); _dgvChiTiet.Rows.Clear();
            _lblTongTien.Text = "TỔNG TIỀN: 0 đ";
            _lblCanhBao.Text = ""; _lblTonHienTai.Text = "";
            _txtMaPhieu.Text = _phieuService.SinhMaPhieu(false);
            UIHelper.ResetInput(_txtNguoiLap);
            _dtpNgayLap.Value = DateTime.Now;
            LoadComboSanPham();
            if (_cboSanPham.Items.Count > 0) _cboSanPham.SelectedIndex = 0;
        }
    }
}
