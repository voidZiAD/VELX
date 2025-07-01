using System.Drawing.Drawing2D;

using System.Drawing;
using System.Windows.Forms;

namespace VELX.Custom_UI_Elements
{
    public class GradientTextLabel : Label
    {
        public Color GradientStart { get; set; } = Color.Red;
        public Color GradientEnd { get; set; } = Color.Blue;
        public float GradientAngle { get; set; } = 90f;

        public GradientTextLabel()
        {
            // Enable proper transparency support
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            SetStyle(ControlStyles.Opaque, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, false);
            this.BackColor = Color.Transparent;
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x20; // WS_EX_TRANSPARENT
                return cp;
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // Do nothing - this prevents background drawing
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // Get the graphics object
            Graphics g = e.Graphics;

            // Set high quality rendering
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Measure text size
            SizeF textSize = g.MeasureString(Text, Font);

            // Calculate text position (centered)
            PointF textLocation = new PointF(
                (ClientSize.Width - textSize.Width) / 2,
                (ClientSize.Height - textSize.Height) / 2);

            // Create gradient rectangle for the text
            RectangleF textRect = new RectangleF(textLocation, textSize);

            // Create gradient brush
            using (var brush = new LinearGradientBrush(textRect, GradientStart, GradientEnd, GradientAngle))
            {
                // Draw text with gradient
                g.DrawString(Text, Font, brush, textLocation);
            }
        }
    }
}