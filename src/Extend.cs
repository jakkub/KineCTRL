using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace KineCTRL
{
    public static class Extend
    {
        public static string RemoveExt(string filename)
        {
            return filename.Substring(0, filename.Length - 4);
        }

        public static float SqrEuclideanDistance(SkeletonPoint p1, SkeletonPoint p2)
        {
            return (p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y) + (p1.Z - p2.Z) * (p1.Z - p2.Z);
        }

        public static float EuclideanDistance(SkeletonPoint p1, SkeletonPoint p2)
        {
            return (float)Math.Sqrt(SqrEuclideanDistance(p1, p2));
        }

        public static float ManhattanDistance(SkeletonPoint p1, SkeletonPoint p2)
        {
            return Math.Abs(p1.X - p2.X) + Math.Abs(p1.Y - p2.Y) + Math.Abs(p1.Z - p2.Z);
        }
    }
}
