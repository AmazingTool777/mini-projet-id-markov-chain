namespace IDMarkovChain.Algorithms.KMeans
{
    public class KMeansCluster(int id, List<IClusterPoint> points, float centroid)
    {
        public int Id { get; set; } = id;

        public List<IClusterPoint> Points { get; set; } = points;

        public float Centroid { get; set; } = centroid;

        // Recalcule et met à-jour le centroïde à partir des points données
        public float ComputeCentroid()
        {
            float sum = 0;
            foreach (IClusterPoint point in Points)
            {
                sum += point.Coordinate;
            }
            // La valeur du centroïde est la moyenne des coordonnées des points de données actuels
            Centroid = sum / Points.Count;

            return Centroid;
        }
    }
}