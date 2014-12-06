using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace KineCTRL
{
    public class SkeletonHistory
    {
        /// <summary>
        /// Size of a skeleton list
        /// </summary>
        private int size;

        /// <summary>
        /// Position of the newest frame in the list
        /// </summary>
        private int position = -1;

        /// <summary>
        /// List of skeletons
        /// </summary>
        private List<Skeleton> skeletons;


        /// <summary>
        /// Keeps a specified number of past skeleton frames
        /// </summary>
        /// <param name="_size">length of a list</param>
        /// <param name="_type">type of a list</param>
        public SkeletonHistory(int _size)
        {
            size = _size;
            skeletons = new List<Skeleton>();
        }


        /// <summary>
        /// Clear skeleton history
        /// </summary>
        public void Clear()
        {
            skeletons.Clear();
            position = -1;
        }


        /// <summary>
        /// Add a single skeleton frame to history
        /// </summary>
        /// <param name="skel">skeleton frame</param>
        public void Add(Skeleton skel)
        {
            if (skeletons.Count < size)
            {
                skeletons.Add(skel);
            }
            else
            {
                position++;
                if (position == size)
                    position = 0;
                skeletons[position] = skel;
            }
        }


        /// <summary>
        /// Get n-th newest skeleton frame
        /// </summary>
        /// <param name="index">index of a requested skeleton frame, 0 = newest</param>
        /// <returns>n-th newest skeleton frame</returns>
        public Skeleton Get(int index)
        {
            if (skeletons.Count < size)
            {
                if (index < 0)
                    index = 0;

                if (index > (skeletons.Count - 1))
                    index = skeletons.Count - 1;

                return skeletons[skeletons.Count - 1 - index];
            }
            else
            {
                if ((index < 0) | (index >= size))
                    index = 0;

                int i = position - index;
                if (i < 0)
                    i += size;

                return skeletons[i];
            }
        }


        /// <summary>
        /// Get number of frames in history
        /// </summary>
        /// <returns>number of frames</returns>
        public int Count()
        {
            return skeletons.Count;
        }


        /// <summary>
        /// Check if skeleton history is full
        /// </summary>
        /// <returns>true = full, false = not full</returns>
        public Boolean IsFull()
        {
            if (skeletons.Count == size)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// Calculates total change in position of all joints in all frames
        /// </summary>
        /// <param name="type">part of the skeleton to be computed</param>
        /// <returns>total position change</returns>
        public float GetTotalPositionChange(string type)
        {
            float total = 0;
            if (skeletons.Count > 1)
            {
                for (int i = 0; i < skeletons.Count - 1; i++)
                {
                    foreach (JointType jointType in JointTypes.GetJoints(type))
                    {
                        // Manhattan distance
                        //total += Extend.ManhattanDistance(skeletons[i].Joints[jointType].Position, skeletons[i + 1].Joints[jointType].Position);

                        // Euclidean distance
                        total += Extend.EuclideanDistance(skeletons[i].Joints[jointType].Position, skeletons[i + 1].Joints[jointType].Position);
                    }
                }
            }
            return total;
        }


        /// <summary>
        /// Calculates total change in position of all joints in last n frames
        /// </summary>
        /// <param name="n">number of last frames to computed</param>
        /// <param name="type">part of the skeleton to be computed</param>
        /// <returns>total position change in the last n frames</returns>
        public float GetTotalPositionChange(int n, string type)
        {
            float total = 0;

            if (n < size)
            {
                for (int i = 0; i < n; i++)
                {
                    foreach (JointType jointType in JointTypes.GetJoints(type))
                    {
                        // Manhattan distance
                        //total += Extend.ManhattanDistance(Get(i).Joints[jointType].Position, Get(i + 1).Joints[jointType].Position);
                        
                        // Euclidean distance
                        total += Extend.EuclideanDistance(Get(i).Joints[jointType].Position, Get(i + 1).Joints[jointType].Position);
                    }
                }
            }
            else
            {
                total = GetTotalPositionChange(type);
            }

            return total;
        }
    }
}
