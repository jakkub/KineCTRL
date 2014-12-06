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

namespace KineCTRL
{
    class PostureRecognizer
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
        /// Default posture
        /// </summary>
        public Gesture DefaultPosture;

        /// <summary>
        /// Difference threshold for comparison of default and reference skeleton
        /// </summary>
        public double DefaultNeighborhood;

        /// <summary>
        /// Neighborhood value for joint comparison
        /// </summary>
        public double ReferenceNeighborhood;

        /// <summary>
        /// Defines whether defined key presses should be executed
        /// </summary>
        public bool executeKeyPresses;


        /// <summary>
        /// Gesture recognizer
        /// </summary>
        /// <param name="main">main window form</param>
        /// <param name="postures">list of postures</param>
        public PostureRecognizer(MainWindow main, Profile profile)
        {
            Main = main;
            Profile = profile;

            // Set neighborhood thresholds for the recognizer
            UpdateSettings();

            // Set default posture
            SetDefaultPosture();
        }


        /// <summary>
        /// Obtain current thresholds from settings and set these values
        /// </summary>
        public void UpdateSettings()
        {
            DefaultNeighborhood = Convert.ToDouble(Profile.Settings["PostureRecognizerDefinition"]);
            ReferenceNeighborhood = Convert.ToDouble(Profile.Settings["PostureRecognizerConfidence"]);

            // Set key press execution flag
            executeKeyPresses = Boolean.Parse(Profile.Settings["ExecuteKeyPresses"]);
        }

        /// <summary>
        /// Find and set default posture
        /// </summary>
        public void SetDefaultPosture()
        {
            foreach (Gesture g in Profile.Gestures[Const.POSTURE])
            {
                if (g.Type == Const.POSTURE_DEFAULT)
                {
                    DefaultPosture = g;
                    break;
                }
            }
        }


        /// <summary>
        /// Process skeleton input from sensor
        /// </summary>
        /// <param name="skeleton">skeleton frame</param>
        public void ProcessSkeleton(Skeleton skeleton)
        {
            // Recognize only if this recognizer is active
            if (active)
            {
                // Process only if default posture is selected
                if (DefaultPosture != null)
                {
                    string ResultOutput = String.Empty;

                    // Loop through all postures except default posture
                    foreach (Gesture p in Profile.Gestures[Const.POSTURE])
                    {
                        if (p != DefaultPosture)
                        {
                            // If posture was recognized, execute key down command
                            if (RecognizePosture(p, skeleton))
                            {
                                if (executeKeyPresses)
                                {
                                    p.ExecuteKeyDown();
                                }
                                ResultOutput += p.Name + " + ";
                            }
                            // If posture is no longer recognized, execute key up command
                            else
                            {
                                if (executeKeyPresses)
                                {
                                    p.ExecuteKeyUp();
                                }
                            }
                        }
                    }

                    // Output result
                    if (ResultOutput.Length > 0)
                    {
                        ResultOutput = ResultOutput.Substring(0, ResultOutput.Length - 3);
                    }
                    Main.LblPostureRecognizerResult.Content = ResultOutput;
                }
            }
        }


        /// <summary>
        /// Recognizes posture
        /// </summary>
        /// <param name="p">posture to recognize</param>
        /// <param name="curSkeleton">current skeleton frame</param>
        /// <returns>true if posture was recognized in the current skeleton frame, false otherwise</returns>
        public Boolean RecognizePosture(Gesture p, Skeleton curSkeleton)
        {
            // Reference skeleton is a posture to recognize
            // Default skeleton is a default posture
            // Current skeleton is current skeleton frame

            // Set default and reference skeletons
            Skeleton refSkeleton = p.Recording.GetFrames()[0];
            Skeleton defSkeleton = DefaultPosture.Recording.GetFrames()[0];

            // Set main joint type HipCenter
            JointType main = JointType.HipCenter;

            // Loop over all joints except main joint
            foreach (JointType type in (JointType[])Enum.GetValues(typeof(JointType)))
            {
                if (type != main)
                {
                    // Calculate vector from main joint to current joint in the reference skeleton
                    float RX1 = refSkeleton.Joints[type].Position.X - refSkeleton.Joints[main].Position.X;
                    float RY1 = refSkeleton.Joints[type].Position.Y - refSkeleton.Joints[main].Position.Y;
                    float RZ1 = refSkeleton.Joints[type].Position.Z - refSkeleton.Joints[main].Position.Z;

                    // Calculate vector from main joint to current joint in the current skeleton
                    float CX1 = curSkeleton.Joints[type].Position.X - curSkeleton.Joints[main].Position.X;
                    float CY1 = curSkeleton.Joints[type].Position.Y - curSkeleton.Joints[main].Position.Y;
                    float CZ1 = curSkeleton.Joints[type].Position.Z - curSkeleton.Joints[main].Position.Z;

                    // Calculate vector from main joint to current joint in the default skeleton
                    float DX1 = defSkeleton.Joints[type].Position.X - defSkeleton.Joints[main].Position.X;
                    float DY1 = defSkeleton.Joints[type].Position.Y - defSkeleton.Joints[main].Position.Y;
                    float DZ1 = defSkeleton.Joints[type].Position.Z - defSkeleton.Joints[main].Position.Z;

                    // Check if the "reference" joint lies outside of the "default" joint's neighborhood.
                    // If yes, this "reference" joint is positioned differently when compared to "default" joint, therefore
                    // this joint is interesting for us and we have to check in the next step 
                    // if "current" joint is anywhere near "reference" joint.
                    if ((Math.Abs(RX1 - DX1) > DefaultNeighborhood) |
                        (Math.Abs(RY1 - DY1) > DefaultNeighborhood) |
                        (Math.Abs(RZ1 - DZ1) > DefaultNeighborhood))
                    {
                        // If position of the "current" joint lies outside the neighborhood of the "reference" joint, 
                        // posture was not recognized. Otherwise, posture was recognized.
                        if ((Math.Abs(RX1 - CX1) > ReferenceNeighborhood) |
                            (Math.Abs(RY1 - CY1) > ReferenceNeighborhood) |
                            (Math.Abs(RZ1 - CZ1) > ReferenceNeighborhood))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }
    }
}
