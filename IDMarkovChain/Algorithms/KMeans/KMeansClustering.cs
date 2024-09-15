using IDMarkovChain.Utils;

namespace IDMarkovChain.Algorithms.KMeans
{
    public static class KMeansClustering
    {
        // Algorithme de clustering par la méthode des K-Moyennes
        public static KMeansCluster[] Clusterize(int k, IClusterPoint[] dataPoints, int maxIterations = 100, double tolerance = 0.001f)
        {
            // Initialisation de la liste de clusters
            KMeansCluster[] clusters = new KMeansCluster[k];

            // Création de K centroïdes
            double[] centroids = CreateCentroids(k, dataPoints);
            // Initialisation de chaque cluster avec les centroïdes créés
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
                    KMeansCluster parentCluster = FindParentCluster(dataPoint, clusters);
                    // Affecter le point vers ce cluster
                    parentCluster.Points.Add(dataPoint);
                }

                // Initilisation de la liste des nouveaux centroides à calculer
                double[] nextCentroids = new double[k];

                // Calcul des nouveaux centroides de chaque cluster
                for (int i = 0; i < k; i++)
                {
                    double nextCentroid = clusters[i].ComputeCentroid();
                    // Memorisation du nouveau centroïde
                    nextCentroids[i] = nextCentroid;
                }

                // SORTIR s'il y a convergence
                if (CheckConvergence(centroids, nextCentroids, tolerance)) break;

                // Mise à jour de la liste courante des centroïdes pour la prochaine itération
                centroids = nextCentroids;
            }

            // Tri des clusters par ordre croissant des centroides
            SortClusters(clusters);

            return clusters;
        }

        // Cherche le cluster appartenant à point parmis une liste de clusters données
        // en déterminant le centroïde le plus proche
        public static KMeansCluster FindParentCluster(IClusterPoint point, KMeansCluster[] clusters)
        {
            int clusterIndex = -1;
            double minDistance = double.MaxValue;
            for (int i = 0; i < clusters.Length; i++)
            {
                // Calculer la distance entre la donnée et le centroïde du cluster
                double distance = Math.Abs(clusters[i].Centroid - point.Coordinate);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    clusterIndex = i;
                }
            }
            return clusters[clusterIndex];
        }

        // Crée K centroïdes à partir d'une liste des points de données
        private static double[] CreateCentroids(int k, IClusterPoint[] dataPoints)
        {
            // Détermination des bornes min et max des points
            double dataPointsMin = double.MaxValue, dataPointsMax = double.MinValue;
            foreach (IClusterPoint dataPoint in dataPoints)
            {
                dataPointsMin = Math.Min(dataPoint.Coordinate, dataPointsMin);
                dataPointsMax = Math.Max(dataPoint.Coordinate, dataPointsMax);
            }
            // Initilisation des centroides et des clusters asscociés à chaque centroïde
            // avec des valeurs comprises entre les bornes min et max
            return NumbersUtils.GenerateUniqueRandomNumbersInRange(k, dataPointsMin, dataPointsMax);
        }

        // Vérification de la convergence de 2 liste de centroïdes étant donnée un valeur de tolérance
        private static bool CheckConvergence(double[] prevCentroids, double[] centroids, double tolerance = 0.001f)
        {
            // Calcul de la distance euclidienne entre les chaque centroïde des 2 listes de centroïdes
            double centroidsDistance = 0;
            for (int i = 0; i < centroids.Length; i++)
            {
                centroidsDistance += Math.Pow(Math.Abs(centroids[i] - prevCentroids[i]), 2);
            }
            centroidsDistance = Math.Sqrt(centroidsDistance);

            // Il y a convergence si la distance euclidienne est inférieure à la tolérance donnée
            return centroidsDistance < tolerance;
        }

        // Trie une liste de clusters par ordre croissant des centroïdes
        private static KMeansCluster[] SortClusters(KMeansCluster[] clusters)
        {
            clusters = [.. clusters.OrderBy(c => c.Centroid)];
            // Mise à jour des identifiants de chaque cluster suite au tri précédent
            for (int i = 0; i < clusters.Length; i++)
            {
                clusters[i].Id = i;
            }

            return clusters;
        }
    }
}