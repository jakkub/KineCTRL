using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace KineCTRL.DollarRecognizer
{
    class SkeletonRecordingToPointCloud
    {
        /// <summary>
        /// Convert skeleton recording to point cloud
        /// </summary>
        /// <param name="skeletonRecording">skeleton recording</param>
        /// <returns>point cloud</returns>
        public static Point[] Convert(SkeletonRecording skeletonRecording, String type)
        {
            Point[] points = new Point[skeletonRecording.GetNormalizedFrames().Count * JointTypes.GetJointsCount(type)];

            int jointId = 0;
            int i = 0;

            // Loop through each joint in the skeleton
            foreach (JointType jointType in JointTypes.GetJoints(type))
            {
                // Loop through skeleton frames
                for (int frameId = 0; frameId < skeletonRecording.GetNormalizedFrames().Count; frameId++)
                {
                    float x = skeletonRecording.GetNormalizedFrames()[frameId].Joints[jointType].Position.X;
                    float y = skeletonRecording.GetNormalizedFrames()[frameId].Joints[jointType].Position.Y;
                    float z = skeletonRecording.GetNormalizedFrames()[frameId].Joints[jointType].Position.Z;

                    // Add point to point cloud
                    points[i++] = new Point(x, y, z, jointId);
                }

                // Increase jointId for each joint
                jointId++;
            }

            return points;
        }
    }
}
