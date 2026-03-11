using System;
using System.Drawing;
using System.Windows.Forms;

namespace QuanLyKho.Forms
{
    /// <summary>
    /// frmHuongDan - Hiển thị hướng dẫn sử dụng ngay trên ứng dụng.
    /// Thiết kế theo phong cách hiện đại, trực quan.
    /// </summary>
    public class frmHuongDan : Form
    {
        public frmHuongDan()
        {
            Text = "📖 Hướng Dẫn Sử Dụng";
            BuildUI();
        }

        private void BuildUI()
        {
            base.BackColor = AppTheme.BgDark;
            base.ForeColor = AppTheme.TextPrimary;
            base.Font = AppTheme.FontBody;

            var lblTieuDe = UIHelper.CreateSectionHeader("📖 HƯỚNG DẪN SỬ DỤNG HỆ THỐNG");

            var container = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(30),
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                BackColor = Color.Transparent
            };

            // Nội dung hướng dẫn theo thứ tự 1 -> 5
            AddGuideStep(container, "1", "📦", "Quản lý Danh mục", 
                "Quản lý thông tin gốc của Sản phẩm và Đối tác.\n" +
                "• Sử dụng form bên trái để Nhập/Sửa.\n" +
                "• Sử dụng bảng bên phải để xem và chọn dữ liệu.\n" +
                "• Tìm kiếm thông minh theo mã hoặc tên ngay khi gõ.");

            AddGuideStep(container, "2", "📥", "Lập Phiếu Nhập Kho", 
                "Đưa hàng hóa vào kho và tăng số lượng tồn.\n" +
                "• Chọn nhà cung cấp và sản phẩm.\n" +
                "• Hệ thống tự tính thành tiền và tổng trị giá phiếu.\n" +
                "• Tồn kho sẽ được cập nhật ngay sau khi Lưu.");

            AddGuideStep(container, "3", "📤", "Lập Phiếu Xuất Kho", 
                "Xuất hàng cho đối tác nhận.\n" +
                "• **Tính năng Hero**: Tự động kiểm tra tồn kho.\n" +
                "• Cảnh báo đỏ sẽ hiện nếu bạn xuất quá số lượng hiện có.\n" +
                "• Ngăn chặn tuyệt đối việc để kho bị âm.");

            AddGuideStep(container, "4", "🕰️", "Lịch Sử & Xóa Phiếu", 
                "Xử lý dữ liệu nhập nhầm để tránh chồng chất.\n" +
                "• Xem lại toàn bộ các giao dịch đã thực hiện.\n" +
                "• Khi xóa phiếu, hệ thống tự động **Hoàn Kho** (đảo ngược số lượng).\n" +
                "• Đảm bảo tính nhất quán dữ liệu 100%.");

            AddGuideStep(container, "5", "📊", "Báo Cáo Tồn Kho", 
                "Xem tình hình hàng hóa dynamic.\n" +
                "• Màu xanh: Hàng còn nhiều.\n" +
                "• Màu vàng: Sắp hết hàng (Dưới 10).\n" +
                "• Màu đỏ: Đã hết hàng (0).");

            Controls.Add(container);
            Controls.Add(lblTieuDe);
        }

        private void AddGuideStep(FlowLayoutPanel parent, string step, string icon, string title, string detail)
        {
            var card = UIHelper.CreateCard();
            card.Width = 850;
            card.Height = 140;
            card.Padding = new Padding(20);
            card.Margin = new Padding(0, 0, 0, 20);

            var lblStep = new Label
            {
                Text = step,
                Font = new Font("Segoe UI", 24f, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, AppTheme.Accent),
                Size = new Size(50, 60),
                Location = new Point(15, 15)
            };

            var lblIcon = new Label
            {
                Text = icon,
                Font = new Font("Segoe UI Emoji", 26f),
                Size = new Size(60, 60),
                Location = new Point(70, 15),
                TextAlign = ContentAlignment.MiddleCenter
            };

            var lblTitle = new Label
            {
                Text = title.ToUpper(),
                Font = AppTheme.FontSubtitle,
                ForeColor = AppTheme.Accent,
                AutoSize = true,
                Location = new Point(140, 20)
            };

            var lblDetail = new Label
            {
                Text = detail,
                Font = AppTheme.FontBody,
                ForeColor = AppTheme.TextPrimary,
                Size = new Size(680, 80),
                Location = new Point(140, 50)
            };

            card.Controls.Add(lblDetail);
            card.Controls.Add(lblTitle);
            card.Controls.Add(lblIcon);
            card.Controls.Add(lblStep);
            
            parent.Controls.Add(card);
        }
    }
}
