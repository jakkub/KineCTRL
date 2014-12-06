using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using Microsoft.Kinect;

namespace KineCTRL.Streams
{
    class SkeletonLoader
    {
        /// <summary>
        /// Main Application Form
        /// </summary>
        MainWindow Main;

        /// <summary>
        /// Debug window
        /// </summary>
        DebugWindow Debug;

        /// <summary>
        /// Currently loaded profile
        /// </summary>
        private Profile Profile;

        /// <summary>
        /// Stream recording trigger on/off
        /// </summary>
        private bool recordingEnabled = false;
        public bool RecordingEnabled
        {
            get { return recordingEnabled; }
            set { recordingEnabled = value; }
        }

        /// <summary>
        /// Active skeleton ID
        /// </summary>
        private int activeTrackingID = -1;
        public int ActiveTrackingID
        {
            get { return activeTrackingID; }
            set { activeTrackingID = value; }
        }

        /// <summary>
        /// Gesture body recognizer
        /// </summary>
        public GestureRecognizer RecognizerBody;

        /// <summary>
        /// Gesture left hand recognizer
        /// </summary>
        public GestureRecognizer RecognizerLH;

        /// <summary>
        /// Gesture right hand recognizer
        /// </summary>
        public GestureRecognizer RecognizerRH;

        /// <summary>
        /// Posture recognizer
        /// </summary>
        public PostureRecognizer PostureRecognizer;

        /// <summary>
        /// MouseHand recognizer
        /// </summary>
        public HandMouseRecognizer HandMouseRecognizer;

        /// <summary>
        /// Gesture recorder
        /// </summary>
        public Recorder Recorder;

        /// <summary>
        /// Main skeleton renderer
        /// </summary>
        public SkeletonRenderer RendererMain;

        /// <summary>
        /// Skeleton renderer for gesture recordings
        /// </summary>
        public SkeletonRenderer RendererManager;


        /// <summary>
        /// FPS counter
        /// </summary>
        //public FPSCounter FPSCounter;


        /// <summary>
        /// This class is used for loading of skeletal data
        /// </summary>
        /// <param name="main">main application form</param>
        public SkeletonLoader(MainWindow main, DebugWindow debug, Profile profile)
        {
            Main = main;
            Debug = debug;
            Profile = profile;
            
            // Initialize Gesture Recognizers
            RecognizerBody = new GestureRecognizer(Main, Debug, Profile, Const.BODY, Main.LblBodyRecognizerResult);
            RecognizerLH = new GestureRecognizer(Main, Debug, Profile, Const.LEFT_HAND, Main.LblLHRecognizerResult);
            RecognizerRH = new GestureRecognizer(Main, Debug, Profile, Const.RIGHT_HAND, Main.LblRHRecognizerResult);

            // Initialize HandMouse Recognizer
            HandMouseRecognizer = new HandMouseRecognizer(Main, Profile);

            // Initialize Posture Recognizer
            PostureRecognizer = new PostureRecognizer(Main, Profile);

            // Initialize Gesture Recorder
            Recorder = new Recorder(Main, Debug, Profile);

            // Initialize Skeleton renderers
            RendererMain = new SkeletonRenderer(Main.ImageOutputMain);
            RendererManager = new SkeletonRenderer(Main.ImageOutputManager);
            
            // Initialize FPS Counter
            //FPSCounter = new FPSCounter(Main);
        }


        /// <summary>
        /// Event handler for Kinect sensor's SkeletonFrameReady event
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        public void SensorSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            // Inform the FPS counter that new frame has been received
            //FPSCounter.AddFrame();

            // Initialize skeletons list
            Skeleton[] skeletons = new Skeleton[0];

            // Build skeletons list
            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {
                    skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    skeletonFrame.CopySkeletonDataTo(skeletons);
                }
            }
            
            // Initialize active skeleton object
            Skeleton activeSkeleton = new Skeleton();
            bool activeSkeletonFound = false;

            int numberOfTrackedSkeletons = 0;
            // Extract active skeleton
            if (skeletons.Length != 0)
            {
                // Loop over all received skeletons
                foreach (Skeleton skel in skeletons)
                {
                    if (skel.TrackingState == SkeletonTrackingState.Tracked)
                    {
                        numberOfTrackedSkeletons++;

                        if ((skel.Joints[JointType.HandLeft].Position.Y > skel.Joints[JointType.Head].Position.Y) &
                            (skel.Joints[JointType.HandRight].Position.Y > skel.Joints[JointType.Head].Position.Y))
                        {
                            // Register skeleton
                            activeSkeleton = skel;
                            activeTrackingID = skel.TrackingId;
                            activeSkeletonFound = true;
                            break;
                        }
                        if ((skel.TrackingId == activeTrackingID) | (activeTrackingID == -1))
                        {
                            // Register skeleton
                            activeSkeleton = skel;
                            activeTrackingID = skel.TrackingId;
                            activeSkeletonFound = true;
                        }
                    }
                }

                // Show tip about how to become active skeleton
                if (numberOfTrackedSkeletons >= 2)
                {
                    Main.TbTipBecomeActive.Visibility = Visibility.Visible;
                }
                else
                {
                    Main.TbTipBecomeActive.Visibility = Visibility.Hidden;
                }


                // Active skeleton not found, check for other skeletons in the next frame
                if (!activeSkeletonFound)
                {
                    activeTrackingID = -1;
                }
            }
            else
            {
                // No skeletons found, wait for skeletons
                activeTrackingID = -1;
            }

            // Render all received skeletons, highlight active skeleton
            RendererMain.RenderSkeletons(skeletons, activeTrackingID, Const.ALL);

            // Render skeletons to manager if recording is enabled
            if (recordingEnabled)
            {
                RendererManager.RenderSkeletons(skeletons, activeTrackingID, Main.RecordingMode);
            }

            // Check if some active skeleton was found
            if (activeSkeletonFound)
            {
                // Pass skeleton to the gesture recognizers
                RecognizerBody.ProcessSkeleton(activeSkeleton);
                RecognizerLH.ProcessSkeleton(activeSkeleton);
                RecognizerRH.ProcessSkeleton(activeSkeleton);

                // Pass skeleton to the handmouse recognizer
                HandMouseRecognizer.ProcessSkeleton(activeSkeleton);
                    
                // Pass skeleton to the posture recognizer
                PostureRecognizer.ProcessSkeleton(activeSkeleton);
            }


            // Recording is on
            if (recordingEnabled)
            {
                // Check if some active skeleton was found
                if (activeSkeletonFound)
                {
                    // Recording is in progress
                    if (!Recorder.recordingFinished)
                    {
                        // Pass skeleton to the recorder
                        Recorder.ProcessSkeleton(activeSkeleton, Main.RecordingMode);
                    }
                    // Recording was completed
                    else
                    {
                        recordingEnabled = false;
                    }
                }
                else
                {
                    // Switch recorder status to waiting
                    Main.LblRecorderStatus.Content = "WAITING FOR USER";
                }
            }
        }
    }
}
