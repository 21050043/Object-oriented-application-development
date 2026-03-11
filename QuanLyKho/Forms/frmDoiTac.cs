using System;
using System.Drawing;
using System.Windows.Forms;
using QuanLyKho.Models;
using QuanLyKho.Services;

namespace QuanLyKho.Forms
{
    /// <summary>frmDoiTac - Quản lý CRUD Đối Tác / Nhà Cung Cấp.</summary>
    public class frmDoiTac : Form
    {
        private readonly DoiTacService _service;
        private DoiTac? _editingDT = null;

        private TextBox _txtMaDT, _txtTenDT, _txtDienThoai, _txtDiaChi, _txtTimKiem;
        private DataGridView _dgv;
        private Button _btnThem, _btnCapNhat, _btnXoa, _btnLamMoi;

        public frmDoiTac(DoiTacService service)
        {
            _service = service;
            Text = "🏢  Quản Lý Đối Tác";
            BuildUI();
            LoadData();
        }

        private void BuildUI()
        {
            BackColor = AppTheme.BgDark;
            ForeColor = AppTheme.TextPrimary;
            Font = AppTheme.FontBody;

            var lblTieuDe = UIHelper.CreateSectionHeader("🏢  QUẢN LÝ DANH MỤC ĐỐI TÁC");

            // Form card bên trái
            var formCard = UIHelper.CreateCard();
            formCard.Width = 300;
            formCard.Dock = DockStyle.Left;
            formCard.Padding = new Padding(16);

            var lblForm = new Label { Text = "Thông Tin Đối Tác", Font = AppTheme.FontLabel, ForeColor = AppTheme.Accent, AutoSize = true, Dock = DockStyle.Top };
            _txtMaDT      = UIHelper.CreateInput("Mã đối tác (VD: NCC001)");
            _txtTenDT     = UIHelper.CreateInput("Tên đối tác");
            _txtDienThoai = UIHelper.CreateInput("Số điện thoại");
            _txtDiaChi    = UIHelper.CreateInput("Địa chỉ");

            var btnPanel = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 50, BackColor = Color.Transparent, Padding = new Padding(0, 6, 0, 0) };
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

            formCard.Controls.Add(btnPanel);
            formCard.Controls.Add(_txtDiaChi);
            formCard.Controls.Add(UIHelper.CreateLabel("Địa Chỉ"));
            formCard.Controls.Add(_txtDienThoai);
            formCard.Controls.Add(UIHelper.CreateLabel("Số Điện Thoại"));
            formCard.Controls.Add(_txtTenDT);
            formCard.Controls.Add(UIHelper.CreateLabel("Tên Đối Tác *"));
            formCard.Controls.Add(_txtMaDT);
            formCard.Controls.Add(UIHelper.CreateLabel("Mã Đối Tác *"));
            formCard.Controls.Add(UIHelper.CreateSpacer(8));
            formCard.Controls.Add(lblForm);

            // Panel danh sách bên phải
            var rightPanel = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent, Padding = new Padding(10, 0, 0, 0) };
            var searchPanel = new Panel { Dock = DockStyle.Top, Height = 40, BackColor = Color.Transparent };
            _txtTimKiem = UIHelper.CreateInput("🔍  Tìm kiếm...");
            _txtTimKiem.Dock = DockStyle.Fill;
            _txtTimKiem.TextChanged += (s, e) => LoadData(_txtTimKiem.Text);
            searchPanel.Controls.Add(_txtTimKiem);

            _dgv = UIHelper.CreateDataGridView();
            _dgv.Columns.Add("MaDT",      "Mã ĐT");
            _dgv.Columns.Add("TenDT",     "Tên Đối Tác");
            _dgv.Columns.Add("DienThoai", "Điện Thoại");
            _dgv.Columns.Add("DiaChi",    "Địa Chỉ");
            _dgv.Columns["MaDT"].Width = 80;
            _dgv.Columns["TenDT"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            _dgv.Columns["DienThoai"].Width = 110;
            _dgv.SelectionChanged += Dgv_SelectionChanged;

            rightPanel.Controls.Add(_dgv);
            rightPanel.Controls.Add(searchPanel);

            Controls.Add(rightPanel);
            Controls.Add(formCard);
            Controls.Add(lblTieuDe);
        }

        private void LoadData(string keyword = "")
        {
            _dgv.Rows.Clear();
            var list = _service.GetAll();
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                string kw = keyword.ToLower();
                list = list.FindAll(d => d.MaDT.ToLower().Contains(kw) || d.TenDT.ToLower().Contains(kw));
            }
            foreach (var dt in list)
                _dgv.Rows.Add(dt.MaDT, dt.TenDT, dt.DienThoai, dt.DiaChi);
        }

        private void BtnThem_Click(object? sender, EventArgs e)
        {
            try
            {
                string maDT = UIHelper.GetTextValue(_txtMaDT);
                string tenDT = UIHelper.GetTextValue(_txtTenDT);
                string sdt = UIHelper.GetTextValue(_txtDienThoai);
                string diaChi = UIHelper.GetTextValue(_txtDiaChi);

                if (string.IsNullOrEmpty(maDT) || string.IsNullOrEmpty(tenDT))
                {
                    UIHelper.ShowError("Vui lòng nhập đầy đủ Mã và Tên đối tác!");
                    return;
                }

                var dt = new DoiTac(maDT, tenDT, sdt, diaChi);
                _service.Add(dt);
                LoadData(); ResetForm();
                UIHelper.ShowSuccess("Thêm đối tác thành công!");
            }
            catch (Exception ex) { UIHelper.ShowError(ex.Message); }
        }

        private void BtnCapNhat_Click(object? sender, EventArgs e)
        {
            if (_editingDT == null) return;
            try
            {
                string tenDT = UIHelper.GetTextValue(_txtTenDT);
                string sdt = UIHelper.GetTextValue(_txtDienThoai);
                string diaChi = UIHelper.GetTextValue(_txtDiaChi);

                if (string.IsNullOrEmpty(tenDT))
                {
                    UIHelper.ShowError("Tên đối tác không được rỗng!");
                    return;
                }

                _editingDT.TenDT = tenDT;
                _editingDT.DienThoai = sdt;
                _editingDT.DiaChi = diaChi;
                _service.Update(_editingDT);
                LoadData(); ResetForm();
                UIHelper.ShowSuccess("Cập nhật đối tác thành công!");
            }
            catch (Exception ex) { UIHelper.ShowError(ex.Message); }
        }

        private void BtnXoa_Click(object? sender, EventArgs e)
        {
            if (_editingDT == null) return;
            if (MessageBox.Show($"Xóa đối tác '{_editingDT.TenDT}'?", "Xác nhận xóa",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                try { _service.Delete(_editingDT.MaDT); LoadData(); ResetForm(); }
                catch (Exception ex) { UIHelper.ShowError(ex.Message); }
            }
        }

        private void Dgv_SelectionChanged(object? sender, EventArgs e)
        {
            if (_dgv.SelectedRows.Count == 0) return;
            string maDT = _dgv.SelectedRows[0].Cells["MaDT"].Value?.ToString() ?? "";
            _editingDT = _service.GetById(maDT);
            if (_editingDT == null) return;

            _txtMaDT.Text = _editingDT.MaDT; _txtMaDT.ReadOnly = true;
            _txtTenDT.Text = _editingDT.TenDT;
            _txtDienThoai.Text = _editingDT.DienThoai;
            _txtDiaChi.Text = _editingDT.DiaChi;
            _btnThem.Enabled = false; _btnCapNhat.Enabled = true; _btnXoa.Enabled = true;
        }

        private void ResetForm()
        {
            _editingDT = null;
            UIHelper.ResetInput(_txtMaDT);
            UIHelper.ResetInput(_txtTenDT);
            UIHelper.ResetInput(_txtDienThoai);
            UIHelper.ResetInput(_txtDiaChi);
            
            _txtMaDT.ReadOnly = false;
            _btnThem.Enabled = true; _btnCapNhat.Enabled = false; _btnXoa.Enabled = false;
            _dgv.ClearSelection();
        }
    }
}
