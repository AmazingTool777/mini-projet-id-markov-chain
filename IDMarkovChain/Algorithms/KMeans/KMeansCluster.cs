namespace IDMarkovChain.Algorithms.KMeans
{
    public class KMeansCluster(int id, List<IClusterPoint> points, float centroid)
    {
        public int Id { get; set; } = id;

        public List<IClusterPoint> Points { get; set; } = points;

        public float Centroid { get; set; } = centroid;
    }
}