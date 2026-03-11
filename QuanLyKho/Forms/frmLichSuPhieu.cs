using System;
using System.Drawing;
using System.Windows.Forms;
using QuanLyKho.Models;
using QuanLyKho.Services;

namespace QuanLyKho.Forms
{
    /// <summary>
    /// frmLichSuPhieu - Quản lý danh sách phiếu đã nhập/xuất.
    /// Giải quyết yêu cầu "Xoá" để tránh "Dữ liệu chồng chất".
    /// </summary>
    public class frmLichSuPhieu : Form
    {
        private readonly PhieuKhoService _service;
        private DataGridView _dgv;
        private TextBox _txtTimKiem;

        public frmLichSuPhieu(PhieuKhoService service)
        {
            _service = service;
            Text = "🕰️ Lịch Sử Giao Dịch";
            BuildUI();
            LoadData();
        }

        private void BuildUI()
        {
            BackColor = AppTheme.BgDark;
            ForeColor = AppTheme.TextPrimary;
            Font = AppTheme.FontBody;

            var lblTieuDe = UIHelper.CreateSectionHeader("🕰️ QUẢN LÝ LỊCH SỬ NHẬP / XUẤT KHO");

            // Toolbar
            var toolbar = new Panel { Dock = DockStyle.Top, Height = 50, BackColor = AppTheme.BgCard, Padding = new Padding(12, 6, 12, 6) };
            _txtTimKiem = UIHelper.CreateInput("🔍 Tìm theo mã phiếu, người lập...");
            _txtTimKiem.Dock = DockStyle.Left; _txtTimKiem.Width = 300;
            _txtTimKiem.TextChanged += (s, e) => LoadData(_txtTimKiem.Text);

            var btnXoa = UIHelper.CreateButton("✕ Xóa Phiếu", AppTheme.AccentRed);
            btnXoa.Dock = DockStyle.Right;
            btnXoa.Click += BtnXoa_Click;

            toolbar.Controls.Add(_txtTimKiem);
            toolbar.Controls.Add(btnXoa);

            // GridView
            _dgv = UIHelper.CreateDataGridView();
            _dgv.ReadOnly = true;
            _dgv.Columns.Add("MaPhieu",  "Mã Phiếu");
            _dgv.Columns.Add("NgayLap",  "Ngày Lập");
            _dgv.Columns.Add("Loai",     "Loại Phiếu");
            _dgv.Columns.Add("NguoiLap", "Người Lập");
            _dgv.Columns.Add("TongTien", "Tổng Tiền");
            _dgv.Columns["MaPhieu"].Width = 120;
            _dgv.Columns["NgayLap"].Width = 120;
            _dgv.Columns["TongTien"].DefaultCellStyle.Format = "N0";
            _dgv.Columns["TongTien"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            Controls.Add(_dgv);
            Controls.Add(toolbar);
            Controls.Add(lblTieuDe);
        }

        private void LoadData(string kw = "")
        {
            _dgv.Rows.Clear();
            keyword = kw.ToLower();
            var list = _service.GetAll();
            foreach (var p in list)
            {
                if (!string.IsNullOrEmpty(kw) && 
                    !p.MaPhieu.ToLower().Contains(keyword) && 
                    !p.NguoiLap.ToLower().Contains(keyword)) continue;

                string loai = p is PhieuNhap ? "📥 NHẬP" : "📤 XUẤT";
                int rowIdx = _dgv.Rows.Add(p.MaPhieu, p.NgayLap.ToString("dd/MM/yyyy HH:mm"), loai, p.NguoiLap, p.TinhTongTien());
                
                _dgv.Rows[rowIdx].Cells["Loai"].Style.ForeColor = p is PhieuNhap ? AppTheme.AccentGreen : AppTheme.AccentRed;
            }
        }

        private string keyword = "";

        private void BtnXoa_Click(object? sender, EventArgs e)
        {
            if (_dgv.SelectedRows.Count == 0) { UIHelper.ShowError("Vui lòng chọn phiếu cần xóa!"); return; }
            string maPhieu = _dgv.SelectedRows[0].Cells["MaPhieu"].Value.ToString();

            if (MessageBox.Show($"Bạn có chắc muốn xóa phiếu '{maPhieu}'?\nTồn kho của các sản phẩm trong phiếu sẽ được hệ thống TỰ ĐỘNG ĐẢO NGƯỢC (Hoàn kho).", 
                "Xác nhận xóa phiếu", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                try
                {
                    _service.XoaPhieu(maPhieu);
                    UIHelper.ShowSuccess($"Đã xóa phiếu {maPhieu} và cập nhật lại tồn kho.");
                    LoadData(_txtTimKiem.Text);
                }
                catch (Exception ex) { UIHelper.ShowError(ex.Message); }
            }
        }
    }
}
