namespace Terra.Map.Render.Features
{
    internal readonly record struct DrawableBuilding(
        int Start,
        int Count,
        BoundBox Bounds);
}
