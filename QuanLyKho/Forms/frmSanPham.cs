using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using QuanLyKho.Models;
using QuanLyKho.Services;

namespace QuanLyKho.Forms
{
    /// <summary>
    /// frmSanPham - Quản lý CRUD sản phẩm.
    /// Layout: Form nhập liệu (trái) + DataGridView (phải).
    /// </summary>
    public class frmSanPham : Form
    {
        private readonly SanPhamService _service;
        private SanPham? _editingSP = null;

        // Controls
        private TextBox _txtMaSP, _txtTenSP, _txtDonViTinh, _txtDonGia, _txtTimKiem;
        private DataGridView _dgv;
        private Button _btnThem, _btnCapNhat, _btnXoa, _btnLamMoi;
        private Label _lblTieuDe;

        public frmSanPham(SanPhamService service)
        {
            _service = service;
            Text = "🗂️  Quản Lý Sản Phẩm";
            BuildUI();
            LoadData();
        }

        private void BuildUI()
        {
            BackColor = AppTheme.BgDark;
            ForeColor = AppTheme.TextPrimary;
            Font = AppTheme.FontBody;

            // Header
            _lblTieuDe = UIHelper.CreateSectionHeader("📋  QUẢN LÝ DANH MỤC SẢN PHẨM");

            // ===== PANEL NHẬP LIỆU (LEFT) =====
            var formCard = UIHelper.CreateCard();
            formCard.Width = 300;
            formCard.Dock = DockStyle.Left;
            formCard.Padding = new Padding(16);

            var lblForm = new Label { Text = "Thông Tin Sản Phẩm", Font = AppTheme.FontLabel, ForeColor = AppTheme.Accent, AutoSize = true, Dock = DockStyle.Top };

            _txtMaSP     = UIHelper.CreateInput("Mã sản phẩm (VD: SP001)");
            _txtTenSP    = UIHelper.CreateInput("Tên sản phẩm");
            _txtDonViTinh= UIHelper.CreateInput("Đơn vị tính (VD: Cái, Hộp)");
            _txtDonGia   = UIHelper.CreateInput("Đơn giá (VD: 150000)");

            var lblMa  = UIHelper.CreateLabel("Mã Sản Phẩm *");
            var lblTen = UIHelper.CreateLabel("Tên Sản Phẩm *");
            var lblDvt = UIHelper.CreateLabel("Đơn Vị Tính");
            var lblGia = UIHelper.CreateLabel("Đơn Giá (đ)");

            // Buttons
            var btnPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 90,
                FlowDirection = FlowDirection.LeftToRight,
                BackColor = Color.Transparent,
                Padding = new Padding(0, 6, 0, 0)
            };
            _btnThem    = UIHelper.CreateButton("+ Thêm",     AppTheme.AccentGreen);
            _btnCapNhat = UIHelper.CreateButton("✎ Cập nhật", AppTheme.Accent);
            _btnXoa     = UIHelper.CreateButton("✕ Xóa",      AppTheme.AccentRed);
            _btnLamMoi  = UIHelper.CreateButton("↺ Mới",      AppTheme.BgInput);
            _btnCapNhat.Enabled = false;
            _btnXoa.Enabled = false;

            _btnThem.Click    += BtnThem_Click;
            _btnCapNhat.Click += BtnCapNhat_Click;
            _btnXoa.Click     += BtnXoa_Click;
            _btnLamMoi.Click  += (s, e) => ResetForm();

            btnPanel.Controls.AddRange(new Control[] { _btnThem, _btnCapNhat, _btnXoa, _btnLamMoi });

            // Thêm controls vào card (bottom-up vì Dock.Top)
            formCard.Controls.Add(btnPanel);
            formCard.Controls.Add(_txtDonGia);
            formCard.Controls.Add(lblGia);
            formCard.Controls.Add(_txtDonViTinh);
            formCard.Controls.Add(lblDvt);
            formCard.Controls.Add(_txtTenSP);
            formCard.Controls.Add(lblTen);
            formCard.Controls.Add(_txtMaSP);
            formCard.Controls.Add(lblMa);
            formCard.Controls.Add(UIHelper.CreateSpacer(8));
            formCard.Controls.Add(lblForm);

            // ===== PANEL DANH SÁCH (RIGHT) =====
            var rightPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                Padding = new Padding(10, 0, 0, 0)
            };

            // Tìm kiếm
            var searchPanel = new Panel { Dock = DockStyle.Top, Height = 40, BackColor = Color.Transparent };
            _txtTimKiem = UIHelper.CreateInput("🔍  Tìm kiếm theo mã, tên...");
            _txtTimKiem.Dock = DockStyle.Fill;
            _txtTimKiem.TextChanged += (s, e) => LoadData(_txtTimKiem.Text);
            searchPanel.Controls.Add(_txtTimKiem);

            // DataGridView
            _dgv = UIHelper.CreateDataGridView();
            _dgv.Columns.Add("MaSP",     "Mã SP");
            _dgv.Columns.Add("TenSP",    "Tên Sản Phẩm");
            _dgv.Columns.Add("DonViTinh","Đơn vị tính");
            _dgv.Columns.Add("DonGia",   "Đơn giá (đ)");
            _dgv.Columns.Add("SoLuongTon","Tồn kho");
            _dgv.Columns["MaSP"].Width = 90;
            _dgv.Columns["TenSP"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            _dgv.Columns["DonGia"].DefaultCellStyle.Format = "N0";
            _dgv.SelectionChanged += Dgv_SelectionChanged;

            rightPanel.Controls.Add(_dgv);
            rightPanel.Controls.Add(searchPanel);

            // ===== Gắn vào form =====
            Controls.Add(rightPanel);
            Controls.Add(formCard);
            Controls.Add(_lblTieuDe);
        }

        private void LoadData(string keyword = "")
        {
            _dgv.Rows.Clear();
            var list = string.IsNullOrWhiteSpace(keyword)
                ? _service.GetAll()
                : _service.TimKiem(keyword);

            foreach (var sp in list)
            {
                int row = _dgv.Rows.Add(sp.MaSP, sp.TenSP, sp.DonViTinh, sp.DonGia, sp.SoLuongTon);
                // Tô màu dòng tồn thấp
                if (sp.SoLuongTon == 0)
                    _dgv.Rows[row].DefaultCellStyle.BackColor = Color.FromArgb(60, AppTheme.AccentRed);
                else if (sp.SoLuongTon < 10)
                    _dgv.Rows[row].DefaultCellStyle.BackColor = Color.FromArgb(40, AppTheme.AccentYellow);
            }
        }

        private void BtnThem_Click(object? sender, EventArgs e)
        {
            try
            {
                string maSP = UIHelper.GetTextValue(_txtMaSP);
                string tenSP = UIHelper.GetTextValue(_txtTenSP);
                string dvt = UIHelper.GetTextValue(_txtDonViTinh);
                string donGiaStr = UIHelper.GetTextValue(_txtDonGia).Replace(",", "").Replace(".", "");

                if (string.IsNullOrEmpty(maSP) || string.IsNullOrEmpty(tenSP))
                {
                    UIHelper.ShowError("Vui lòng nhập đầy đủ Mã và Tên sản phẩm!");
                    return;
                }

                if (!double.TryParse(donGiaStr, out double donGia))
                {
                    UIHelper.ShowError("Đơn giá không hợp lệ!");
                    return;
                }

                var sp = new SanPham(maSP, tenSP, dvt, donGia);
                _service.Add(sp);
                LoadData();
                ResetForm();
                UIHelper.ShowSuccess("Thêm sản phẩm thành công!");
            }
            catch (Exception ex) { UIHelper.ShowError(ex.Message); }
        }

        private void BtnCapNhat_Click(object? sender, EventArgs e)
        {
            if (_editingSP == null) return;
            try
            {
                string tenSP = UIHelper.GetTextValue(_txtTenSP);
                string dvt = UIHelper.GetTextValue(_txtDonViTinh);
                string donGiaStr = UIHelper.GetTextValue(_txtDonGia).Replace(",", "").Replace(".", "");

                if (string.IsNullOrEmpty(tenSP))
                {
                    UIHelper.ShowError("Tên sản phẩm không được rỗng!");
                    return;
                }

                if (!double.TryParse(donGiaStr, out double donGia))
                {
                    UIHelper.ShowError("Đơn giá không hợp lệ!");
                    return;
                }

                _editingSP.TenSP = tenSP;
                _editingSP.DonViTinh = dvt;
                _editingSP.DonGia = donGia;
                _service.Update(_editingSP);
                LoadData();
                ResetForm();
                UIHelper.ShowSuccess("Cập nhật thành công!");
            }
            catch (Exception ex) { UIHelper.ShowError(ex.Message); }
        }

        private void BtnXoa_Click(object? sender, EventArgs e)
        {
            if (_editingSP == null) return;
            if (MessageBox.Show($"Xóa sản phẩm '{_editingSP.TenSP}'?", "Xác nhận xóa",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                try { _service.Delete(_editingSP.MaSP); LoadData(); ResetForm(); }
                catch (Exception ex) { UIHelper.ShowError(ex.Message); }
            }
        }

        private void Dgv_SelectionChanged(object? sender, EventArgs e)
        {
            if (_dgv.SelectedRows.Count == 0) return;
            var row = _dgv.SelectedRows[0];
            string maSP = row.Cells["MaSP"].Value?.ToString() ?? "";
            _editingSP = _service.GetById(maSP);
            if (_editingSP == null) return;

            _txtMaSP.Text = _editingSP.MaSP;
            _txtMaSP.ReadOnly = true;
            _txtTenSP.Text = _editingSP.TenSP;
            _txtDonViTinh.Text = _editingSP.DonViTinh;
            _txtDonGia.Text = _editingSP.DonGia.ToString();

            _btnThem.Enabled = false;
            _btnCapNhat.Enabled = true;
            _btnXoa.Enabled = true;
        }

        private void ResetForm()
        {
            _editingSP = null;
            UIHelper.ResetInput(_txtMaSP);
            UIHelper.ResetInput(_txtTenSP);
            UIHelper.ResetInput(_txtDonViTinh);
            UIHelper.ResetInput(_txtDonGia);
            
            _txtMaSP.ReadOnly = false;
            _btnThem.Enabled = true;
            _btnCapNhat.Enabled = false;
            _btnXoa.Enabled = false;
            _dgv.ClearSelection();
        }
    }
}
