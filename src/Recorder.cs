using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Kinect;

namespace KineCTRL
{
    public class Recorder
    {
        /// <summary>
        /// Main Application Form
        /// </summary>
        MainWindow Main;

        /// <summary>
        /// Debug Window
        /// </summary>
        DebugWindow Debug;

        /// <summary>
        /// Currently loaded profile
        /// </summary>
        private Profile Profile;

        /// <summary>
        /// Last 90 skeleton frames
        /// </summary>
        public SkeletonHistory skeletonHistory = new SkeletonHistory(60);

        /// <summary>
        /// Size of the skeleton history for gesture detection
        /// </summary>
        public int historySize;

        /// <summary>
        /// Recorded gesture
        /// </summary>
        public SkeletonRecording skeletonRecording;

        /// <summary>
        /// Recording is ready flag
        /// </summary>
        public Boolean recordingReady = false;

        /// <summary>
        /// Recording has been finished flag
        /// </summary>
        public Boolean recordingFinished = false;

        /// <summary>
        /// Threshold when skeleton is standing still 
        /// </summary>
        public double thresholdReady;

        /// <summary>
        /// Threshold when skeleton is performing a gesture
        /// </summary>
        public double thresholdRecord;

        /// <summary>
        /// A gesture to be redefined
        /// </summary>
        public Gesture RedefineGesture;

        /// <summary>
        /// This class is used for recording of skeletal data
        /// </summary>
        /// <param name="main">main window form</param>
        public Recorder(MainWindow main, DebugWindow debug, Profile profile)
        {
            Main = main;
            Debug = debug;
            Profile = profile;

            // Set thresholds for the recorder
            UpdateSettings();

            // Set size of the skeleton history for gesture detection
            historySize = 15;
        }


        /// <summary>
        /// Obtain current thresholds from settings and set these values
        /// </summary>
        public void UpdateSettings()
        {
            thresholdRecord = Convert.ToDouble(Profile.Settings["RecorderDetection"]);
            thresholdReady = Convert.ToDouble(Profile.Settings["RecorderReady"]);
        }


        /// <summary>
        /// Reset recorder
        /// </summary>
        public void ResetRecorder()
        {
            skeletonHistory.Clear();
            skeletonRecording = null;

            recordingReady = false;
            recordingFinished = false;
        }


        /// <summary>
        /// Process skeleton input from sensor
        /// </summary>
        /// <param name="skeleton">skeleton frame</param>
        /// <param name="recordingMode">recording mode</param>
        public void ProcessSkeleton(Skeleton skeleton, string recordingMode)
        {
            // Add skeleton frame to skeleton history
            skeletonHistory.Add(skeleton);

            // Show debug info
            Debug.TbRecorderLongPosChange.Text = skeletonHistory.GetTotalPositionChange(recordingMode).ToString("0.000");
            Debug.TbRecorderShortPosChange.Text = skeletonHistory.GetTotalPositionChange(historySize, recordingMode).ToString("0.000");

            // Check if skeleton history is full
            if (skeletonHistory.IsFull())
            {
                // Wait until total position change in the last 60 frames is under ready threshold, at this point the user is standing still
                if ((!recordingReady) & (skeletonHistory.GetTotalPositionChange(recordingMode) < (thresholdReady)))
                {
                    recordingReady = true;
                    Main.LblRecorderStatus.Content = "PERFORM GESTURE NOW";
                }
                
                // If recording is ready (= user is standing still), wait until total position change in the last 15 frames is over record threshold
                if (recordingReady) 
                {
                    // Record BODY/LH/RH gesture
                    if (recordingMode != Const.POSTURE)
                    {
                        if ((skeletonHistory.GetTotalPositionChange(historySize, recordingMode) > thresholdRecord))
                        {
                            // Start recording
                            Main.LblRecorderStatus.Content = "RECORDING GESTURE";

                            // Add skeleton history to the beginning of the recording if this is the first frame of recording
                            if (skeletonRecording == null)
                            {
                                // Initialize new skeleton recording
                                skeletonRecording = new SkeletonRecording(recordingMode);

                                // Add skeleton history to recording
                                for (int i = historySize-1; i > 0; i--)
                                {
                                    skeletonRecording.AddFrame(skeletonHistory.Get(i));
                                }
                            }

                            // Add skeleton frame to recording
                            skeletonRecording.AddFrame(skeleton);
                        }
                    }
                    // Record POSTURE
                    else
                    {
                        // Initialize new skeleton recording
                        skeletonRecording = new SkeletonRecording(recordingMode);

                        // Add skeleton frame to recording
                        skeletonRecording.AddFrame(skeleton);
                    }
                }

                // If total position change drops under record threshold, finish recording
                if ((skeletonRecording != null) & ((skeletonHistory.GetTotalPositionChange(historySize, recordingMode) <= thresholdRecord) | (recordingMode == Const.POSTURE)))
                {
                    // If this is a new gesture
                    if (RedefineGesture == null)
                    {
                        // Create new gesture object
                        Gesture newGesture = new Gesture("New Gesture", recordingMode, skeletonRecording);

                        // Set different name for posture
                        if (recordingMode == Const.POSTURE)
                        {
                            newGesture.Name = "New Posture";
                        }

                        // Add new gesture to gesture list
                        Profile.Gestures[recordingMode].Add(newGesture);

                        // Refresh gestures listbox
                        Main.LboxGestures.Items.Refresh();

                        // Select new gesture in the listbox
                        Main.LboxGestures.SelectedItem = newGesture;
                    }
                    // Redefining existing gesture
                    else
                    {
                        // Set new recording for this gesture
                        RedefineGesture.Recording = skeletonRecording;

                        // Display redefined gesture;
                        Main.DisplaySavedGesture(RedefineGesture);
                    }

                    // Inform user that gesture was saved
                    if (recordingMode == Const.POSTURE)
                    {
                        Main.LblRecorderStatus.Content = "POSTURE SAVED";
                    }
                    else
                    {
                        Main.LblRecorderStatus.Content = "GESTURE SAVED";
                    }

                    // Finish recording
                    recordingFinished = true;
                }
            }
            else
            {
                Main.LblRecorderStatus.Content = "PLEASE STAND STILL";
            }
                
        }
    }
}
