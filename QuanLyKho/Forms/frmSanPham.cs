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
        private readonly DoiTacService _dtService;
        private SanPham? _editingSP = null;

        // Controls
        private TextBox _txtMaSP, _txtTenSP, _txtDonViTinh, _txtDonGia, _txtTimKiem, _txtDanhMuc;
        private ComboBox _cboChuSoHuu;
        private DataGridView _dgv;
        private Button _btnThem, _btnCapNhat, _btnXoa, _btnLamMoi;
        private Label _lblTieuDe;

        public frmSanPham(SanPhamService service, DoiTacService dtService)
        {
            _service = service;
            _dtService = dtService;
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
            _txtDanhMuc  = UIHelper.CreateInput("Danh mục (Máy tính, Linh kiện...)");
            _txtDonViTinh= UIHelper.CreateInput("Đơn vị tính (VD: Cái, Hộp)");
            _txtDonGia   = UIHelper.CreateInput("Đơn giá (VD: 150000)");
            
            _cboChuSoHuu = UIHelper.CreateComboBox();
            _cboChuSoHuu.Items.Add(new DoiTac("SYSTEM", "-", "", ""));
            foreach (var dt in _dtService.GetAll()) _cboChuSoHuu.Items.Add(dt);
            _cboChuSoHuu.SelectedIndex = 0;

            var lblMa  = UIHelper.CreateLabel("Mã Sản Phẩm *");
            var lblTen = UIHelper.CreateLabel("Tên Sản Phẩm *");
            var lblDm  = UIHelper.CreateLabel("Danh Mục");
            var lblDvt = UIHelper.CreateLabel("Đơn Vị Tính");
            var lblGia = UIHelper.CreateLabel("Đơn Giá (đ)");
            var lblOwner = UIHelper.CreateLabel("Chủ Sở Hữu (Đối tác)");

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
            formCard.Controls.Add(_cboChuSoHuu);
            formCard.Controls.Add(lblOwner);
            formCard.Controls.Add(_txtDonGia);
            formCard.Controls.Add(lblGia);
            formCard.Controls.Add(_txtDonViTinh);
            formCard.Controls.Add(lblDvt);
            formCard.Controls.Add(_txtDanhMuc);
            formCard.Controls.Add(lblDm);
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
            _dgv.Columns.Add("MaSP",      "Mã SP");
            _dgv.Columns.Add("TenSP",     "Tên Sản Phẩm");
            _dgv.Columns.Add("DanhMuc",   "Danh Mục");
            _dgv.Columns.Add("DonViTinh", "Đơn vị");
            _dgv.Columns.Add("DonGia",    "Đơn giá");
            _dgv.Columns.Add("SoLuongTon","Tồn");
            _dgv.Columns.Add("ChuSoHuu",  "Chủ Sở Hữu");

            _dgv.Columns["MaSP"].Width = 80;
            _dgv.Columns["TenSP"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            _dgv.Columns["DonViTinh"].Width = 70;
            _dgv.Columns["SoLuongTon"].Width = 60;
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
                string ownerName = "-";
                if (!string.IsNullOrEmpty(sp.MaDoiTacChuQuan))
                {
                    var owner = _dtService.GetById(sp.MaDoiTacChuQuan);
                    if (owner != null) ownerName = owner.TenDT;
                }

                int row = _dgv.Rows.Add(sp.MaSP, sp.TenSP, sp.DanhMuc, sp.DonViTinh, sp.DonGia, sp.SoLuongTon, ownerName);
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
                string danhMuc = UIHelper.GetTextValue(_txtDanhMuc);
                string dvt = UIHelper.GetTextValue(_txtDonViTinh);
                string donGiaStr = UIHelper.GetTextValue(_txtDonGia).Replace(",", "").Replace(".", "");

                string maDoiTac = "";
                if (_cboChuSoHuu.SelectedItem is DoiTac dt && dt.MaDT != "SYSTEM")
                    maDoiTac = dt.MaDT;

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

                var sp = new SanPham(maSP, tenSP, dvt, donGia, danhMuc, maDoiTac);
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
                string danhMuc = UIHelper.GetTextValue(_txtDanhMuc);
                string dvt = UIHelper.GetTextValue(_txtDonViTinh);
                string donGiaStr = UIHelper.GetTextValue(_txtDonGia).Replace(",", "").Replace(".", "");

                string maDoiTac = "";
                if (_cboChuSoHuu.SelectedItem is DoiTac dt && dt.MaDT != "SYSTEM")
                    maDoiTac = dt.MaDT;

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
                _editingSP.DanhMuc = danhMuc;
                _editingSP.DonViTinh = dvt;
                _editingSP.DonGia = donGia;
                _editingSP.MaDoiTacChuQuan = maDoiTac;
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
            _txtDanhMuc.Text = _editingSP.DanhMuc;
            _txtDonViTinh.Text = _editingSP.DonViTinh;
            _txtDonGia.Text = _editingSP.DonGia.ToString();

            // Chọn chủ sở hữu trong ComboBox
            _cboChuSoHuu.SelectedIndex = 0; // Mặc định hệ thống
            if (!string.IsNullOrEmpty(_editingSP.MaDoiTacChuQuan))
            {
                for (int i = 0; i < _cboChuSoHuu.Items.Count; i++)
                {
                    if (_cboChuSoHuu.Items[i] is DoiTac dt && dt.MaDT == _editingSP.MaDoiTacChuQuan)
                    {
                        _cboChuSoHuu.SelectedIndex = i;
                        break;
                    }
                }
            }

            _btnThem.Enabled = false;
            _btnCapNhat.Enabled = true;
            _btnXoa.Enabled = true;
        }

        private void ResetForm()
        {
            _editingSP = null;
            UIHelper.ResetInput(_txtMaSP);
            UIHelper.ResetInput(_txtTenSP);
            UIHelper.ResetInput(_txtDanhMuc);
            UIHelper.ResetInput(_txtDonViTinh);
            UIHelper.ResetInput(_txtDonGia);
            if (_cboChuSoHuu.Items.Count > 0) _cboChuSoHuu.SelectedIndex = 0;
            
            _txtMaSP.ReadOnly = false;
            _btnThem.Enabled = true;
            _btnCapNhat.Enabled = false;
            _btnXoa.Enabled = false;
            _dgv.ClearSelection();
        }
    }
}
