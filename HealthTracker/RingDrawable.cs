using Microsoft.Maui.Graphics;

namespace HealthTracker;

// Calliope - A "drawable": an object that knows how to paint itself. MAUI calls
// Draw with a canvas (the paintbrush) and the area to paint whenever the view
// needs repainting. Progress runs 0.0 (empty) to 1.0 (full ring).
public class RingDrawable : IDrawable
{
    public double Progress { get; set; }
    public Color TrackColor { get; set; } = Colors.LightGray;
    public Color FillColor { get; set; } = Colors.DodgerBlue;
    public float Thickness { get; set; } = 10;

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        canvas.StrokeSize = Thickness;
        canvas.StrokeLineCap = LineCap.Round; // rounded arc ends, not flat cuts

        // Calliope - Inset the circle so the thick stroke isn't clipped at the edges.
        var inset = Thickness / 2 + 1;
        var x = dirtyRect.X + inset;
        var y = dirtyRect.Y + inset;
        var size = dirtyRect.Width - inset * 2;

        // Full background track...
        canvas.StrokeColor = TrackColor;
        canvas.DrawEllipse(x, y, size, size);

        // ...and the progress arc on top.
        var progress = Math.Clamp(Progress, 0, 1);
        canvas.StrokeColor = FillColor;
        if (progress >= 1)
        {
            // Calliope - At exactly 100% the arc math degenerates (a 360° sweep
            // computes as 0°), so just stroke the whole circle.
            canvas.DrawEllipse(x, y, size, size);
        }
        else
        {
            // Calliope - Maui.Graphics measures angles COUNTERclockwise from 3 o'clock,
            // so 12 o'clock is 90°, and sweeping clockwise means DECREASING the angle:
            // 90° down to 90° - sweep. (Verified against the library's own source.)
            var sweep = (float)(360 * progress);
            canvas.DrawArc(x, y, size, size, 90, 90 - sweep, true, false);
        }
    }

}
