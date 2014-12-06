using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace KineCTRL
{
    public static class JointTypes
    {
        /// <summary>
        /// List of all body joints
        /// </summary>
        public static List<JointType> All = new List<JointType>();

        /// <summary>
        /// List of all body joints except hands
        /// </summary>
        public static List<JointType> Body = new List<JointType>();

        /// <summary>
        /// List of Left Hand joints
        /// </summary>
        public static List<JointType> LH = new List<JointType>();

        /// <summary>
        /// List of Right Hand joints
        /// </summary>
        public static List<JointType> RH = new List<JointType>();


        /// <summary>
        /// Generates groups of joints and creates joint lists
        /// </summary>
        static JointTypes()
        {
            foreach (JointType type in (JointType[])Enum.GetValues(typeof(JointType)))
            {
                All.Add(type);
            }

            Body.Add(JointType.AnkleLeft);
            Body.Add(JointType.AnkleRight);
            Body.Add(JointType.FootLeft);
            Body.Add(JointType.FootRight);
            Body.Add(JointType.Head);
            Body.Add(JointType.HipCenter);
            Body.Add(JointType.HipLeft);
            Body.Add(JointType.HipRight);
            Body.Add(JointType.KneeLeft);
            Body.Add(JointType.KneeRight);
            Body.Add(JointType.ShoulderCenter);
            Body.Add(JointType.Spine);

            LH.Add(JointType.HandLeft);
            LH.Add(JointType.WristLeft);
            LH.Add(JointType.ElbowLeft);
            LH.Add(JointType.ShoulderLeft);

            RH.Add(JointType.HandRight);
            RH.Add(JointType.WristRight);
            RH.Add(JointType.ElbowRight);
            RH.Add(JointType.ShoulderRight);
        }


        /// <summary>
        /// Gets list of all joints in selected group
        /// </summary>
        /// <param name="type">type of the group</param>
        /// <returns>list of all joints in the requested group</returns>
        public static List<JointType> GetJoints(String type)
        {
            // Return requested list according to type
            switch (type)
            {
                case Const.ALL:
                    return All;
                case Const.BODY:
                    return Body;
                case Const.LEFT_HAND:
                    return LH;
                case Const.RIGHT_HAND:
                    return RH;
                default:
                    return All;
            }
        }


        /// <summary>
        /// Gets the number of joints in a group
        /// </summary>
        /// <param name="type">type of the group</param>
        /// <returns>number of joints in the requested group</returns>
        public static int GetJointsCount(String type)
        {
            // Return number of joints in selected group
            switch (type)
            {
                case Const.ALL:
                    return All.Count;
                case Const.BODY:
                    return Body.Count;
                case Const.LEFT_HAND:
                    return LH.Count;
                case Const.RIGHT_HAND:
                    return RH.Count;
                default:
                    return All.Count;
            }
        }
        
    }
}
