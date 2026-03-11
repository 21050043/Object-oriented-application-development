using System;
using System.Drawing;
using System.Windows.Forms;

namespace QuanLyKho.Forms
{
    /// <summary>
    /// UIHelper - Factory methods để tạo controls theo design system AppTheme.
    /// Áp dụng DRY + SRP: một nơi tạo controls, không lặp code.
    /// </summary>
    public static class UIHelper
    {
        // ===== SECTION HEADER =====
        public static Label CreateSectionHeader(string text)
        {
            return new Label
            {
                Text = text,
                Font = AppTheme.FontSubtitle,
                ForeColor = AppTheme.TextPrimary,
                BackColor = AppTheme.BgCard,
                Dock = DockStyle.Top,
                Height = 46,
                Padding = new Padding(16, 0, 0, 0),
                TextAlign = ContentAlignment.MiddleLeft,
                BorderStyle = BorderStyle.None
            };
        }

        // ===== CARD PANEL =====
        public static Panel CreateCard()
        {
            return new Panel
            {
                BackColor = AppTheme.BgCard,
                Margin = new Padding(0, 0, 0, 8)
            };
        }

        // ===== INPUT (TextBox) =====
        public static TextBox CreateInput(string placeholder = "")
        {
            var txt = new TextBox
            {
                Dock = DockStyle.Top,
                Height = 32,
                BackColor = AppTheme.BgInput,
                ForeColor = AppTheme.TextPrimary,
                BorderStyle = BorderStyle.FixedSingle,
                Font = AppTheme.FontBody,
                Margin = new Padding(0, 0, 0, 6),
                Tag = placeholder   // Lưu placeholder vào Tag để GetTextValue kiểm tra
            };
            if (!string.IsNullOrEmpty(placeholder))
            {
                txt.ForeColor = AppTheme.TextSecondary;
                txt.Text = placeholder;
                txt.GotFocus += (s, e) =>
                {
                    if (txt.Text == placeholder)
                    { txt.Text = ""; txt.ForeColor = AppTheme.TextPrimary; }
                };
                txt.LostFocus += (s, e) =>
                {
                    if (string.IsNullOrWhiteSpace(txt.Text))
                    { txt.Text = placeholder; txt.ForeColor = AppTheme.TextSecondary; }
                };
            }
            return txt;
        }

        /// <summary>
        /// Lấy giá trị thực của TextBox - trả về "" nếu đang hiển thị placeholder.
        /// Ngăn chặn lỗi nhập nhầm placeholder text vào data.
        /// </summary>
        public static string GetTextValue(TextBox txt)
        {
            if (txt.Tag is string placeholder && txt.Text == placeholder)
                return string.Empty;
            return txt.Text.Trim();
        }

        /// <summary>Reset TextBox về trạng thái placeholder.</summary>
        public static void ResetInput(TextBox txt)
        {
            if (txt.Tag is string placeholder && !string.IsNullOrEmpty(placeholder))
            {
                txt.Text = placeholder;
                txt.ForeColor = AppTheme.TextSecondary;
            }
            else
            {
                txt.Text = string.Empty;
                txt.ForeColor = AppTheme.TextPrimary;
            }
        }

        // ===== LABEL =====
        public static Label CreateLabel(string text)
        {
            return new Label
            {
                Text = text,
                ForeColor = AppTheme.TextSecondary,
                Font = AppTheme.FontSmall,
                Dock = DockStyle.Top,
                Height = 18,
                TextAlign = ContentAlignment.BottomLeft
            };
        }

        // ===== BUTTON =====
        public static Button CreateButton(string text, Color backColor)
        {
            var btn = new Button
            {
                Text = text,
                BackColor = backColor,
                ForeColor = AppTheme.TextPrimary,
                FlatStyle = FlatStyle.Flat,
                Font = AppTheme.FontBody,
                Height = AppTheme.ButtonHeight,
                Width = 125,
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 0, 8, 8)
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = ControlPaint.Light(backColor, 0.1f);

            // Micro-animation: scale effect via opacity simulation
            btn.MouseEnter += (s, e) =>
            {
                btn.BackColor = ControlPaint.Light(backColor, 0.15f);
            };
            btn.MouseLeave += (s, e) =>
            {
                btn.BackColor = backColor;
            };
            return btn;
        }

        // ===== COMBOBOX =====
        public static ComboBox CreateComboBox()
        {
            return new ComboBox
            {
                Dock = DockStyle.Fill,
                BackColor = AppTheme.BgInput,
                ForeColor = AppTheme.TextPrimary,
                Font = AppTheme.FontBody,
                FlatStyle = FlatStyle.Flat,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Margin = new Padding(0, 0, 0, 0)
            };
        }

        // ===== DATA GRID VIEW =====
        public static DataGridView CreateDataGridView()
        {
            var dgv = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = AppTheme.BgCard,
                ForeColor = AppTheme.TextPrimary,
                GridColor = AppTheme.BorderColor,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                ReadOnly = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None,
                ColumnHeadersHeight = 34,
                RowTemplate = { Height = 30 },
                Font = AppTheme.FontBody,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal
            };

            // Header style
            dgv.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(38, 38, 58),
                ForeColor = AppTheme.Accent,
                Font = AppTheme.FontLabel,
                Alignment = DataGridViewContentAlignment.MiddleLeft,
                Padding = new Padding(4, 0, 0, 0)
            };
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;

            // Row style
            dgv.DefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = AppTheme.BgCard,
                ForeColor = AppTheme.TextPrimary,
                SelectionBackColor = Color.FromArgb(50, AppTheme.Accent),
                SelectionForeColor = AppTheme.TextPrimary,
                Padding = new Padding(4, 0, 0, 0)
            };

            // Alternating row
            dgv.AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(24, 24, 40),
                ForeColor = AppTheme.TextPrimary,
                SelectionBackColor = Color.FromArgb(50, AppTheme.Accent),
                SelectionForeColor = AppTheme.TextPrimary
            };

            dgv.EnableHeadersVisualStyles = false;
            return dgv;
        }

        // ===== SPACER =====
        public static Panel CreateSpacer(int height)
        {
            return new Panel { Dock = DockStyle.Top, Height = height, BackColor = Color.Transparent };
        }

        // ===== MESSAGEBOX HELPERS =====
        public static void ShowSuccess(string message)
        {
            MessageBox.Show(message, "✅ Thành Công", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static void ShowError(string message)
        {
            MessageBox.Show(message, "❌ Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
