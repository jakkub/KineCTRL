using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using System.Windows.Threading;


using KineCTRL.Streams;

namespace KineCTRL
{
    public class SkeletonRecording
    {
        /// <summary>
        /// List of skeleton frames
        /// </summary>
        private List<Skeleton> Frames;

        /// <summary>
        /// Normalized list of skeleton frames
        /// </summary>
        private List<Skeleton> NormalizedFrames;

        /// <summary>
        /// Type of the recording
        /// </summary>
        private string type;
        public string Type
        {
            get { return type; }
            set { type = value; }
        }

        /// <summary>
        /// This class keeps single recording of a skeleton sequence
        /// </summary>
        public SkeletonRecording(string _type)
        {
            type = _type;

            // Initialize list of frames
            Frames = new List<Skeleton>();
        }


        /// <summary>
        /// Get recorded skeleton frames
        /// </summary>
        /// <returns>list of skeleton frames</returns>
        public List<Skeleton> GetFrames() 
        {
            return Frames;
        }


        /// <summary>
        /// Get normalized skeleton frames
        /// </summary>
        /// <returns>list of skeleton frames</returns>
        public List<Skeleton> GetNormalizedFrames()
        {
            if (Frames.Count > 1)
            {
                // Normalize frames if they haven't been normalized before
                if (NormalizedFrames == null)
                {
                    NormalizeFrames();
                }

                if (NormalizedFrames.Count > 3)
                {
                    return NormalizedFrames;
                }
                else
                {
                    return Frames;
                }
            }
            else
            {
                return Frames;
            }
        }


        /// <summary>
        /// Get number of recorded frames
        /// </summary>
        /// <returns>number of skeleton frames</returns>
        public int Count()
        {
            return Frames.Count;
        }


        /// <summary>
        /// Add a single frame to the recording
        /// </summary>
        /// <param name="skeletons">skeletons frame</param>
        public void AddFrame(Skeleton skeleton)
        {
            Frames.Add(skeleton);
        }


        /// <summary>
        /// Clear the recording
        /// </summary>
        public void Clear()
        {
            Frames.Clear();
            NormalizedFrames = null;
        }


        /// <summary>
        /// Normalizes recording by removing frames that are too similar
        /// </summary>
        public void NormalizeFrames()
        {
            NormalizedFrames = new List<Skeleton>();
            for (int i = 0; i < Frames.Count - 1; i++)
            {
                if (GetPositionChange(Frames[i], Frames[i+1]) > 0.07)
                {
                    NormalizedFrames.Add(Frames[i]);
                }
            }
        }


        /// <summary>
        /// Calculates total change in position of two skeletons
        /// </summary>
        /// <param name="skel1">first skeleton</param>
        /// <param name="skel2">second skeleton</param>
        /// <returns>total position change</returns>
        public float GetPositionChange(Skeleton skel1, Skeleton skel2)
        {
            float total = 0;
            foreach (JointType jointType in JointTypes.GetJoints(type))
            {
                total += Math.Abs(skel1.Joints[jointType].Position.X - skel2.Joints[jointType].Position.X);
                total += Math.Abs(skel1.Joints[jointType].Position.Y - skel2.Joints[jointType].Position.Y);
                total += Math.Abs(skel1.Joints[jointType].Position.Z - skel2.Joints[jointType].Position.Z);
            }
            return total;
        }
    }
}
