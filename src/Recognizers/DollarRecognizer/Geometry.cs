using System;

namespace KineCTRL.DollarRecognizer
{
    public class Geometry
    {
        /// <summary>
        /// Computes the Squared Euclidean Distance between two points in 3D
        /// </summary>
        public static float SqrEuclideanDistance(Point a, Point b)
        {
            return (a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y) + (a.Z - b.Z) * (a.Z - b.Z);
        }

        /// <summary>
        /// Computes the Euclidean Distance between two points in 3D
        /// </summary>
        public static float EuclideanDistance(Point a, Point b)
        {
            return (float)Math.Sqrt(SqrEuclideanDistance(a, b));
        }
    }
}
