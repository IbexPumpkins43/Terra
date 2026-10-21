using System.Numerics;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using Terra.Map.Graph;
using Terra.Map.Render.Features;

namespace Terra.Map.Render
{
    internal class MapControl : SKGLControl
    {
        private readonly RenderData _renderData;
        private readonly GraphData _graphData;

        // Public so that I can move it when I want
        public Camera Camera;

        private bool _isLeftMouseButtonDown;
        private Point _lastMousePosition;

        public bool ShowRoads = true;
        public bool ShowBuildings = true;
        public bool ShowPlaceNames = true;

        public MapControl(RenderData renderData, GraphData graphData)
        {
            this._renderData = renderData;
            this._graphData = graphData;
            this.Camera = new();

            this.Dock = DockStyle.Fill;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (this._isLeftMouseButtonDown)
            {
                this.Camera.X -= (e.Location.X - this._lastMousePosition.X) / this.Camera.Zoom;
                this.Camera.Y -= (e.Location.Y - this._lastMousePosition.Y) / this.Camera.Zoom;
            }

            this._lastMousePosition = e.Location;

            this.Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            this._isLeftMouseButtonDown = e.Button == MouseButtons.Left;
            this.Invalidate();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            this._isLeftMouseButtonDown = !(e.Button == MouseButtons.Left);
            this.Invalidate();
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            this.Camera.Zoom *= MathF.Pow(1.1f, e.Delta / 100);
        }

        protected override void OnPaintSurface(SKPaintGLSurfaceEventArgs e)
        {
            base.OnPaintSurface(e);
            
            SKCanvas canvas = e.Surface.Canvas;

            canvas.Clear(SKColors.White);
            canvas.Save();

            // Update the canvas according to the camera
            this.Camera.Apply(canvas);

            // Shared paint object
            using SKPaint paint = new()
            {
                Color = SKColors.Black,
                Style = SKPaintStyle.Stroke,
                IsAntialias = true
            };

            // Render the map
            this.RenderRoads(canvas, paint);
            this.RenderBuildings(canvas, paint);
            this.RenderPlaces(canvas, paint);

            canvas.Restore();
        }

        private void RenderRoads(SKCanvas canvas, SKPaint paint)
        {
            if (!this.ShowRoads)
            {
                return;
            }

            BoundBox viewBounds = this.GetViewBoundBox(canvas, this.Camera);

            unsafe
            {
                fixed (Vector2* pointsPtr = this._renderData.Points)
                {
                    foreach (DrawableRoad road in this._renderData.Roads)
                    {
                        if (!this.BoundBoxIntersects(road.Bounds, viewBounds))
                        {
                            continue;
                        }

                        RoadStyle style = Road.GetRoadStyle(road.Type);

                        // if (this._camera.Zoom < style.MinimumZoom)
                        // {
                        //    continue;
                        // }

                        int end = road.Start + road.Count - 1;
                        for (int index = road.Start; index < end; index++)
                        {
                            paint.Color = style.Colour;
                            paint.StrokeWidth = style.Thickness;
                            canvas.DrawLine(
                                pointsPtr[index],
                                pointsPtr[index + 1],
                                paint);
                        }
                    }
                }
            }
        }

        private void RenderBuildings(SKCanvas canvas, SKPaint paint) 
        {
            if (!this.ShowBuildings)
            {
                return;
            }

            BoundBox viewBounds = this.GetViewBoundBox(canvas, this.Camera);

            foreach (DrawableBuilding building in this._renderData.Buildings)
            {
                // Don't render if it is out of bounds
                if (!this.BoundBoxIntersects(building.Bounds, viewBounds))
                {
                    continue;
                }

                // Drop any buildings with less than 3 points
                if (building.Count < 3)
                {
                    continue;
                }

                using SKPathBuilder pathBuilder = new();

                Vector2 first = this._renderData.Points[building.Start];
                pathBuilder.MoveTo(first.X, first.Y);

                for (int index = 1; index < building.Count; index++)
                {
                    Vector2 point = this._renderData.Points[building.Start + index];
                    pathBuilder.LineTo(point.X, point.Y);
                }

                pathBuilder.Close();

                using SKPath path = pathBuilder.Detach();
                
                // Fill
                paint.Color = SKColors.Gainsboro;
                paint.Style = SKPaintStyle.Fill;
                canvas.DrawPath(path, paint);

                // Outline
                paint.Color = SKColors.DarkGray;
                paint.Style = SKPaintStyle.Stroke;
                paint.StrokeWidth = 2.0f;
                canvas.DrawPath(path, paint);
            }
        }

        private void RenderPlaces(SKCanvas canvas, SKPaint paint)
        {
            if (!this.ShowPlaceNames)
            {
                return;
            }

            using SKFont font = new()
            {
                Typeface = SKTypeface.Default,
                Size = 16.0f / this.Camera.Zoom,
                Edging = SKFontEdging.Antialias
            };

            paint.Color = SKColors.Black;
            paint.Style = SKPaintStyle.Fill;

            foreach (DrawablePlace place in this._renderData.Places)
            {
                using SKTextBlob blob = SKTextBlob.Create(place.Name, font);
                canvas.DrawText(blob, (float)place.Position.X, (float)place.Position.Y, paint);
            }
        }

        private BoundBox GetViewBoundBox(SKCanvas canvas, Camera camera)
        {
            float halfWidth = canvas.DeviceClipBounds.Width / camera.Zoom;
            float halfHeight = canvas.DeviceClipBounds.Height / camera.Zoom;

            return new(
                MinX: camera.X - halfWidth,
                MaxX: camera.X + halfWidth,
                MinY: camera.Y - halfHeight,
                MaxY: camera.Y + halfHeight);
        }

        private bool BoundBoxIntersects(BoundBox first, BoundBox second)
        {
            return first.MaxX >= second.MinX
                && first.MinX <= second.MaxX
                && first.MaxY >= second.MinY
                && first.MinY <= second.MaxY;
        }
    }
}
