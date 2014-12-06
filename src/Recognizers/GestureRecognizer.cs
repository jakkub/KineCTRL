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
using System.Windows.Media;
using KineCTRL.Streams;

namespace KineCTRL
{
    class GestureRecognizer
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
        /// Type of the recognizer
        /// </summary>
        String Type;

        /// <summary>
        /// Last N skeleton frames
        /// </summary>
        public SkeletonHistory skeletonHistory;

        /// <summary>
        /// Size of the skeleton history
        /// </summary>
        public int historySize;

        /// <summary>
        /// Recorded gesture
        /// </summary>
        public SkeletonRecording skeletonRecording;

        /// <summary>
        /// Recording should start when total position change reaches over this threshold
        /// </summary>
        public double thresholdRecordStart;

        /// <summary>
        /// Recording should stop when total position change drops under this threshold
        /// </summary>
        public double thresholdRecordStop;

        /// <summary>
        /// Confidence threshold for gesture execution
        /// </summary>
        public double thresholdExecute;

        /// <summary>
        /// Number of frames each keypress should be executed in a row (needed for DirextX applications)
        /// </summary>
        public int keyPressMultiplier;

        /// <summary>
        /// Defines whether defined key presses should be executed
        /// </summary>
        public bool executeKeyPresses;

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
        /// Classified gesture and its confidence
        /// </summary>
        Tuple<Gesture, float> ClassifiedResult;

        /// <summary>
        /// Label for result output
        /// </summary>
        Label LblResult;

        /// <summary>
        /// Alpha channel of the resulting label
        /// </summary>
        int LblResultAlpha = 360;

        /// <summary>
        /// Alpha channel of the resulting label
        /// </summary>
        byte LblResultAlphaByte = 254;



        /// <summary>
        /// Gesture recognizer
        /// </summary>
        /// <param name="main">main window form</param>
        public GestureRecognizer(MainWindow main, DebugWindow debug, Profile profile, String type, Label lblResult)
        {
            Main = main;
            Debug = debug;
            Profile = profile;
            Type = type;

            LblResult = lblResult;

            // Set thresholds for the recognizer
            UpdateSettings();

            // Set size of the skeleton history
            historySize = 15;

            skeletonRecording = new SkeletonRecording(type);
            skeletonHistory = new SkeletonHistory(historySize);
        }


        /// <summary>
        /// Obtain current thresholds from settings and set these values
        /// </summary>
        public void UpdateSettings()
        {
            // Difference between start od the recording threshold and stop of the recording threshold
            double startStopDifference = 0.2;

            if (Type == Const.BODY)
            {
                thresholdRecordStart = Convert.ToDouble(Profile.Settings["BodyRecognizerDetection"]) + startStopDifference;
                thresholdRecordStop = Convert.ToDouble(Profile.Settings["BodyRecognizerDetection"]) - startStopDifference;
                thresholdExecute = Convert.ToDouble(Profile.Settings["BodyRecognizerConfidence"]);
            }
            else if ((Type == Const.LEFT_HAND) | (Type == Const.RIGHT_HAND))
            {
                thresholdRecordStart = Convert.ToDouble(Profile.Settings["HandRecognizerDetection"]) + startStopDifference;
                thresholdRecordStop = Convert.ToDouble(Profile.Settings["HandRecognizerDetection"]) + startStopDifference;
                thresholdExecute = Convert.ToDouble(Profile.Settings["HandRecognizerConfidence"]);
            }

            // Set multiplier for key presses
            if (Boolean.Parse(Profile.Settings["KeyPressMultiplier"]))
            {
                keyPressMultiplier = 30;
            }
            else
            {
                keyPressMultiplier = 1;
            }   
            
            // Set key press execution flag
            executeKeyPresses = Boolean.Parse(Profile.Settings["ExecuteKeyPresses"]);
        }


        /// <summary>
        /// Reset recognizer
        /// </summary>
        public void ResetRecognizer()
        {
            skeletonHistory.Clear();
            skeletonRecording.Clear();
        }


        /// <summary>
        /// Process skeleton input from sensor
        /// </summary>
        /// <param name="skeleton">skeleton frame</param>
        public void ProcessSkeleton(Skeleton skeleton)
        {
            // Recognize only if this recognizer is active and when there are gestures to recognize
            if ((active) & (Profile.Gestures[Type].Count > 0))
            {
                // First, execute key commands on all gestures

                foreach (Gesture g in Profile.Gestures[Type])
                {
                    g.ExecuteKeyPress();
                }

                // Add skeleton frame to skeleton history
                skeletonHistory.Add(skeleton);

                // Check if skeleton history is full
                if (skeletonHistory.IsFull())
                {
                    // If total position change reaches over recording threshold
                    if (skeletonHistory.GetTotalPositionChange(Type) > thresholdRecordStart)
                    {
                        // Add skeleton history to the beginning of the recording if this is the first frame of recording
                        if (skeletonRecording.Count() == 0)
                        {
                            // Add skeleton history to recording
                            for (int i = historySize-1; i > 0; i--)
                            {
                                skeletonRecording.AddFrame(skeletonHistory.Get(i));
                            }
                        }

                        // Add skeleton frame to recording
                        skeletonRecording.AddFrame(skeleton);

                        // Check if Debugger requests info about this recognizer
                        if (Type == Debug.RecognizerInfoMode)
                        {
                            Debug.TbStatus.Text = "RECORDING";
                        }
                    }

                    // If total position change drops under recording threshold while recording is in progress, finish recording
                    if ((skeletonRecording.Count() > 0) & (skeletonHistory.GetTotalPositionChange(Type) <= thresholdRecordStop))
                    {
                        // Create candidate gesture object
                        Gesture candidateGesture = new Gesture("Candidate Gesture", Type, skeletonRecording);

                        // Classify gesture using Dollar Recognizer
                        ClassifiedResult = DollarRecognizer.PointCloudRecognizer.Classify(candidateGesture, Profile.Gestures[Type]);

                        // Execute gesture action if distance is below threshold
                        if (ClassifiedResult.Item2 < thresholdExecute)
                        {
                            if (executeKeyPresses)
                            {
                                ClassifiedResult.Item1.ExecuteKeyPress(keyPressMultiplier);
                            }

                            // Output result to Main Window
                            LblResultAlpha = 360;
                            LblResult.Content = ClassifiedResult.Item1.Name;
                        }

                        // Check if Debugger requests info about this recognizer
                        if (Type == Debug.RecognizerInfoMode)
                        {
                            // Output gesture name and distance
                            Debug.TbStatus.Text = "STAND BY";
                            Debug.TbResult.Text = ClassifiedResult.Item1.Name;
                            Debug.TbConfidence.Text = ClassifiedResult.Item2.ToString("0.00");

                            // Set color of the result
                            if (ClassifiedResult.Item2 < thresholdExecute)
                            {
                                Debug.TbResult.Foreground = Brushes.Green;
                                Debug.TbConfidence.Foreground = Brushes.Green;
                            }
                            else
                            {
                                Debug.TbResult.Foreground = Brushes.Red;
                                Debug.TbConfidence.Foreground = Brushes.Red;
                            }

                            // Render current recording and classified gesture
                            Debug.RendererDebugPerformed.RenderRecording(skeletonRecording, true, true);
                            Debug.RendererDebugOriginal.RenderRecording(ClassifiedResult.Item1.Recording, true, true);
                        }

                        // Reset recognizer
                        ResetRecognizer();
                    }
                }

                // Check if Debugger requests info about this recognizer
                if (Type == Debug.RecognizerInfoMode)
                {
                    Debug.TbPositionChange.Text = skeletonHistory.GetTotalPositionChange(Type).ToString("0.000");
                }
            }


            // Update alpha channel of the result
            if (LblResultAlpha > 255)
            {
                LblResultAlphaByte = 255;
            }
            else
            {
                LblResultAlphaByte = Convert.ToByte(LblResultAlpha);
            }

            LblResult.Foreground = new SolidColorBrush(Color.FromArgb(LblResultAlphaByte, 30, 30, 30));

            if (LblResultAlpha > 0)
            {
                LblResultAlpha -= 2;
            }
        }
    }
}
