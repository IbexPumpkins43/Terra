using SkiaSharp;

namespace Terra.Map.Render
{
    // Unlike Raylib, Skia does not provide a camera class, so I have to implement my own. This
    // camera is slightly different in that it treats the top-left corner as its position rather
    // than the centre of the viewport.
    internal class Camera
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Zoom { get; set; } = 1.0f;

        public void Apply(SKCanvas canvas)
        {
            canvas.Scale(this.Zoom);
            canvas.Translate(-this.X, -this.Y);
        }

        public SKPoint ScreenToWorld(float x, float y) =>
            new SKPoint(x / this.Zoom + this.X, y / this.Zoom + this.Y);
    }
}
