namespace Terra.Map.Render
{
    internal readonly record struct BoundBox(
        float MinX,
        float MaxX,
        float MinY,
        float MaxY);
}