namespace Terra.Map.Osm
{
    internal enum PbfBlockType
    {
        OsmHeader,
        OsmData
    }

    internal record PbfBlock(
        long Index,
        ReadOnlyMemory<byte> Bytes,
        PbfBlockType Type);

    internal class PbfReaderException : Exception
    {
        public PbfReaderException(string path, string message) : base($"{path} : {message}")
        {
        }

        public PbfReaderException(string path, string message, Exception innerException)
            : base($"{path} : {message}", innerException)
        {
        }
    }
}
