using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using Microsoft.Kinect;
using System.Windows.Threading;
using System.Runtime.InteropServices;
using Microsoft.DirectX;
using Microsoft.DirectX.DirectInput;
using System.Windows;

namespace KineCTRL
{
    class HandMouseRecognizer
    {
        /// <summary>
        /// Main Application Form
        /// </summary>
        MainWindow Main;

        /// <summary>
        /// Currently loaded profile
        /// </summary>
        private Profile Profile;

        /// <summary>
        /// Recognizer Active ON/OFF
        /// </summary>
        private bool active = false;
        public bool Active
        {
            get { return active; }
            set { active = value; }
        }

        /// <summary>
        /// Last 3 skeleton frames
        /// </summary>
        public SkeletonHistory skeletonHistory;

        /// <summary>
        /// Mouse sensitivity
        /// </summary>
        private float Sensitivity;

        /// <summary>
        /// HandMouse function recognizer
        /// </summary>
        /// <param name="main">main window form</param>
        public HandMouseRecognizer(MainWindow main, Profile profile)
        {
            Main = main;
            Profile = profile;
            skeletonHistory = new SkeletonHistory(3);

            // Set sensitivity for the recognizer
            UpdateSettings();
        }


        /// <summary>
        /// Obtain current sensitivity from settings and set this value
        /// </summary>
        public void UpdateSettings()
        {
            Sensitivity = Single.Parse(Profile.Settings["HandMouseSensitivity"]);
        }
        

        /// <summary>
        /// Process skeleton input from sensor
        /// </summary>
        /// <param name="skeleton">skeleton frame</param>
        /// <param name="type">type of the HandMouse recognizer (left hand/right hand)</param>
        public void ProcessSkeleton(Skeleton skeleton)
        {
            // Recognize only if this recognizer is active
            if (active)
            {
                // Add skeleton frame to skeleton history
                skeletonHistory.Add(skeleton);

                // Set hand to be recognized according to the type of the recognizer
                JointType hand;
                int zAxisMultiplier;

                if (Main.HandMouseMode == Const.LEFT_HAND)
                {
                    hand = JointType.HandLeft;
                    zAxisMultiplier = 1;
                }
                else
                {
                    hand = JointType.HandRight;
                    zAxisMultiplier = -1;
                }

                // Get current position of the hand
                SkeletonPoint position = skeleton.Joints[hand].Position;
                

                // Check if skeleton history is full and if hand joint was tracked
                if ((skeletonHistory.IsFull()) & (skeleton.Joints[hand].TrackingState == JointTrackingState.Tracked))
                {
                    // Compute dx and dy for mouse movement
                    float dx = -(skeletonHistory.Get(1).Joints[hand].Position.X - position.X) * Sensitivity * 1300;
                    dx += (skeletonHistory.Get(1).Joints[hand].Position.Z - position.Z) * Sensitivity * 1000 * zAxisMultiplier;
                    float dy = (skeletonHistory.Get(1).Joints[hand].Position.Y - position.Y) * Sensitivity * 1000;

                    // Move mouse accordingly
                    MouseInput.MoveMouse(Convert.ToInt32(dx), Convert.ToInt32(dy));
                }
            }
        }
    }
}
