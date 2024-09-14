using IDMarkovChain.Utils;

namespace IDMarkovChain.Algorithms.KMeans
{
    public static class KMeansClustering
    {
        // Algorithme de clustering par la méthode des K-Moyennes
        public static KMeansCluster[] Clusterize(int k, IClusterPoint[] dataPoints, int maxIterations = 100, float tolerance = 0.001f)
        {
            KMeansCluster[] clusters = new KMeansCluster[k];

            // Détermination des bornes min et max des points
            float dataPointsMin = float.MaxValue, dataPointsMax = float.MinValue;
            foreach (IClusterPoint dataPoint in dataPoints)
            {
                if (dataPoint.Coordinate < dataPointsMin)
                {
                    dataPointsMin = dataPoint.Coordinate;
                }
                if (dataPoint.Coordinate > dataPointsMax)
                {
                    dataPointsMax = dataPoint.Coordinate;
                }
            }
            // Initilisation des centroides et des clusters asscociés à chaque centroïde
            // avec des valeurs comprises entre les bornes min et max
            float[] centroids = NumbersUtils.GenerateUniqueRandomNumbersInRange(k, dataPointsMin, dataPointsMax);
            for (int i = 0; i < k; i++)
            {
                clusters[i] = new KMeansCluster(i, [], centroids[i]);
            }

            // Mise à jour des clusters jusqu'a avoir convergence ou au maximum d'itération
            for (int iterations = 0; iterations < maxIterations; iterations++)
            {
                // Vider les points dans chaque cluster afin d'en construire des nouvelles listes de points
                foreach (KMeansCluster cluster in clusters)
                {
                    cluster.Points.Clear();
                }

                // Affecter les données dans les clusters correspondants
                foreach (IClusterPoint dataPoint in dataPoints)
                {
                    // Trouver le cluster propriétaire
                    KMeansCluster nearestCluster = FindParentCluster(dataPoint, clusters);
                    // Affecter le point vers ce cluster
                    nearestCluster.Points.Add(dataPoint);
                }

                // Initilisation de la liste des nouveaux centroides à calculer
                float[] nextCentroids = new float[k];

                // Calcul des nouveaux centroides de chaque cluster
                for (int i = 0; i < k; i++)
                {
                    KMeansCluster cluster = clusters[i];
                    float sum = 0;
                    foreach (IClusterPoint point in cluster.Points)
                    {
                        sum += point.Coordinate;
                    }
                    float nextCentroid = sum / cluster.Points.Count;
                    cluster.Centroid = nextCentroid;
                    nextCentroids[i] = nextCentroid;
                }

                // Calcul de la convergence des centroides par la comparaison des distances euclidiennes avec la tolerance
                double centroidsDistance = 0;
                for (int i = 0; i < k; i++)
                {
                    centroidsDistance += Math.Pow(Math.Abs(nextCentroids[i] - centroids[i]), 2);
                }
                centroidsDistance = Math.Sqrt(centroidsDistance);
                // S'il y a convergence, sortir de la boucle
                if (centroidsDistance < tolerance) break;

                // Mise à jour de la liste courante des centroïdes pour la prochaine itération
                centroids = nextCentroids;
            }

            // Tri des clusters par ordre croissant des centroides
            clusters = [.. clusters.OrderBy(c => c.Centroid)];
            // Mise à jour des identifiants de chaque cluster suite au tri précédent
            for (int i = 0; i < k; i++)
            {
                clusters[i].Id = i;
            }

            return clusters;
        }

        // Cherche le cluster appartenant à point parmis une liste de clusters données
        // en déterminant le centroïde le plus proche
        public static KMeansCluster FindParentCluster(IClusterPoint point, KMeansCluster[] clusters)
        {
            int clusterIndex = -1;
            float minDistance = float.MaxValue;
            for (int i = 0; i < clusters.Length; i++)
            {
                // Calculer la distance entre la donnée et le centroïde du cluster
                float distance = Math.Abs(clusters[i].Centroid - point.Coordinate);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    clusterIndex = i;
                }
            }
            return clusters[clusterIndex];
        }
    }
}