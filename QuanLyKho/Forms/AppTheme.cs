using System.Drawing;

namespace QuanLyKho.Forms
{
    /// <summary>
    /// AppTheme - Design System tập trung cho toàn bộ ứng dụng.
    /// Áp dụng nguyên tắc DRY + SRP: một nơi quản lý tất cả màu sắc và font.
    /// </summary>
    public static class AppTheme
    {
        // ===== COLORS =====
        public static readonly Color BgDark      = Color.FromArgb(18,  18,  30);   // nền chính
        public static readonly Color BgCard      = Color.FromArgb(28,  28,  46);   // card/panel
        public static readonly Color BgSidebar   = Color.FromArgb(22,  22,  38);   // sidebar
        public static readonly Color BgInput     = Color.FromArgb(38,  38,  58);   // textbox
        public static readonly Color Accent      = Color.FromArgb(99,  102, 241);  // indigo-500
        public static readonly Color AccentHover = Color.FromArgb(79,  82,  220);  // indigo-600
        public static readonly Color AccentGreen = Color.FromArgb(52,  211, 153);  // emerald-400
        public static readonly Color AccentRed   = Color.FromArgb(248, 113, 113);  // red-400
        public static readonly Color AccentYellow= Color.FromArgb(251, 191,  36);  // amber-400
        public static readonly Color TextPrimary = Color.FromArgb(241, 245, 249);  // slate-100
        public static readonly Color TextSecondary = Color.FromArgb(148, 163, 184);// slate-400
        public static readonly Color BorderColor = Color.FromArgb(51,  51,  80);   // viền

        // ===== FONTS =====
        public static readonly Font FontTitle   = new Font("Segoe UI", 18f, FontStyle.Bold);
        public static readonly Font FontSubtitle= new Font("Segoe UI", 11f, FontStyle.Bold);
        public static readonly Font FontBody    = new Font("Segoe UI",  9.5f);
        public static readonly Font FontSmall   = new Font("Segoe UI",  8.5f);
        public static readonly Font FontLabel   = new Font("Segoe UI",  9f, FontStyle.Bold);
        public static readonly Font FontMono    = new Font("Consolas",  9f);

        // ===== DIMENSIONS =====
        public const int SidebarWidth   = 210;
        public const int HeaderHeight   = 60;
        public const int CornerRadius   = 10;
        public const int Padding        = 16;
        public const int ButtonHeight   = 38;
    }
}
