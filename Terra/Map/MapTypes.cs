global using MapNodePositions = System.Collections.Generic.Dictionary<long, Terra.Map.MapPosition>;
global using MapRoads = System.Collections.Generic.List<Terra.Map.Osm.OsmWay>;
global using MapPlaces = System.Collections.Generic.List<Terra.Map.Osm.OsmNode>;
global using MapBuildings = System.Collections.Generic.List<Terra.Map.Osm.OsmWay>;

using Terra.Map.Graph;
using Terra.Map.Render;

namespace Terra.Map
{
    internal readonly record struct MapPosition(
        double X,
        double Y);

    internal class MapData
    {
        public RenderData RenderData { get; private set; } = new();
        public GraphData GraphData { get; private set; } = new();
    }

    internal class MapImporterException : Exception
    {
        public MapImporterException(string path, string message)
            : base($"{path} : {message}")
        {
        }

        public MapImporterException(string path, string message, Exception innerException)
            : base($"{path} : {message}", innerException)
        {
        }
    }
}