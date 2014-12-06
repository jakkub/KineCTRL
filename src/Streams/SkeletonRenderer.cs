using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.IO;
using System.Windows.Threading;
using Microsoft.Kinect;

namespace KineCTRL.Streams
{
    public class SkeletonRenderer
    {
        /// <summary>
        /// Width of output drawing
        /// </summary>
        protected double RenderWidth;

        /// <summary>
        /// Height of our output drawing
        /// </summary>
        protected double RenderHeight;

        /// <summary>
        /// Thickness of drawn joint lines
        /// </summary>
        private const double JointThickness = 3;

        /// <summary>
        /// Thickness of body center ellipse
        /// </summary>
        private const double BodyCenterThickness = 10;

        /// <summary>
        /// Thickness of clip edge rectangles
        /// </summary>
        private const double ClipBoundsThickness = 10;

        /// <summary>
        /// Brush used to draw skeleton center point
        /// </summary>
        private readonly Brush centerPointBrush = Brushes.Blue;

        /// <summary>
        /// Brush used for drawing joints that are currently tracked
        /// </summary>
        private readonly Brush backgroundBrush = new SolidColorBrush(Color.FromArgb(255, 20, 20, 20));

        /// <summary>
        /// Brush used for drawing joints that are currently tracked
        /// </summary>
        private readonly Brush trackedJointBrush = new SolidColorBrush(Color.FromArgb(255, 186, 255, 135));

        /// <summary>
        /// Brush used for drawing joints that are currently inferred
        /// </summary>        
        private readonly Brush inferredJointBrush = Brushes.Yellow;

        /// <summary>
        /// Brush used for drawing joints that are currently inactive
        /// </summary>        
        private readonly Brush inactiveJointBrush = Brushes.DarkOrange;

        /// <summary>
        /// Pen used for drawing bones that are currently tracked and focused
        /// </summary>
        private readonly Pen trackedBonePen = new Pen(new SolidColorBrush(Color.FromArgb(255, 84, 193, 4)), 6);

        /// <summary>
        /// Pen used for drawing bones that are currently tracked with no focus
        /// </summary>
        private readonly Pen trackedBonePenNoFocus = new Pen(Brushes.Gray, 4);

        /// <summary>
        /// Pen used for drawing bones that are currently inferred
        /// </summary>        
        private readonly Pen inferredBonePen = new Pen(Brushes.Gray, 1);

        /// <summary>
        /// Pen used for drawing bones that are currently inactive
        /// </summary>
        private readonly Pen inactiveBonePen = new Pen(Brushes.DarkRed, 4);

        /// <summary>
        /// Brush used for drawing joints that are currently tracked
        /// </summary>
        private readonly Brush progressBarBrush = new SolidColorBrush(Color.FromArgb(255, 219, 42, 26));

        /// <summary>
        /// White overlay over whole image
        /// </summary>
        private int whiteOverlayCount = 0;

        /// <summary>
        /// Drawing image for skeleton data rendering
        /// </summary>
        private DrawingImage drawingImage;
        public DrawingImage DrawingImage
        {
            get { return drawingImage; }
        }

        /// <summary>
        /// Drawing group for rendering output
        /// </summary>
        private DrawingGroup drawingGroup = new DrawingGroup();

        /// <summary>
        /// Timer for replays
        /// </summary>
        private DispatcherTimer ReplayTimer;

        /// <summary>
        /// Counter for the timer
        /// </summary>
        private int ReplayFrameCount;

        /// <summary>
        /// Flag for replay repeat
        /// </summary>
        private bool ReplayRepeatEnabled;

        /// <summary>
        /// Type of the recording replay
        /// </summary>
        private string ReplayType;

        /// <summary>
        /// List of frames to be replayed
        /// </summary>
        private List<Skeleton> ReplayFrames;

        /// <summary>
        /// Rendering is enabled/disabled
        /// </summary>
        private bool enabled = true;
        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }


        /// <summary>
        /// This class is used for rendering of skeletal data
        /// </summary>
        /// <param name="width">width of output image</param>
        /// <param name="height">height of output image</param>
        public SkeletonRenderer(Image image)
        {
            // Set width, height and initialize an image to draw to
            RenderWidth = image.Width;
            RenderHeight = image.Height;
            drawingImage = new DrawingImage(drawingGroup);
            image.Source = drawingImage;
            ClearScreen();

            // Initialize timer
            ReplayTimer = new DispatcherTimer(DispatcherPriority.Render);

            // Set timer for 30 fps
            ReplayTimer.Interval = new TimeSpan(0, 0, 0, 0, 33);

            // Set event handler
            ReplayTimer.Tick += new EventHandler(ReplayTimer_Tick); 
        }


        /// <summary>
        /// Clears renderer window
        /// </summary>
        public void ClearScreen()
        {
            using (DrawingContext dc = drawingGroup.Open())
            {
                // Draw a transparent background to set the render size
                dc.DrawRectangle(backgroundBrush, null, new Rect(0.0, 0.0, RenderWidth, RenderHeight));
            }
        }


        /// <summary>
        /// Render selected recording
        /// </summary>
        public void RenderRecording(SkeletonRecording recording, bool playNormalized, bool repeatEnabled = false)
        {
            ReplayRepeatEnabled = repeatEnabled;
            ReplayType = recording.Type;

            // Get frames to be replayed and duplicate them
            if (playNormalized)
            {
                ReplayFrames = new List<Skeleton>(recording.GetNormalizedFrames());
            }
            else
            {
                ReplayFrames = new List<Skeleton>(recording.GetFrames());
            }

            if (ReplayFrames.Count > 0)
            {
                // Set 10 frames to be overlaid by white frame
                whiteOverlayCount = 5;

                // Reset frame counter
                ReplayFrameCount = 0;

                // Start timer
                ReplayTimer.Start();
            }
        }


        /// <summary>
        /// Event handler for timer
        /// </summary>
        private void ReplayTimer_Tick(object sender, EventArgs e)
        {
            // Render single skeleton frame
            RenderSkeleton(ReplayFrames[ReplayFrameCount], ReplayType, ReplayFrameCount, ReplayFrames.Count);
            ReplayFrameCount++;

            if (ReplayFrameCount == ReplayFrames.Count)
            {
                if (ReplayRepeatEnabled)
                {
                    // Play again
                    ReplayFrameCount = 0;
                }
                else
                {
                    // Stop replay
                    ReplayTimer.Stop();

                    // Reset counter
                    ReplayFrameCount = 0;
                }
            }
        }


        /// <summary>
        /// Stop rendering recording
        /// </summary>
        public void RenderRecordingStop()
        {
            ReplayTimer.Stop();
            ClearScreen();
        }


        /// <summary>
        /// Renders an array of skeletons
        /// </summary>
        /// <param name="skeletons">array of skeletons to render</param>
        public void RenderSkeletons(Skeleton[] skeletons, int activeTrackingId, string renderingMode)
        {
            // Render only if rendering is enabled
            if (enabled)
            {
                using (DrawingContext dc = drawingGroup.Open())
                {
                    // Draw a transparent background to set the render size
                    dc.DrawRectangle(backgroundBrush, null, new Rect(0.0, 0.0, RenderWidth, RenderHeight));

                    if (skeletons.Length != 0)
                    {
                        foreach (Skeleton skel in skeletons)
                        {
                            if (skel.TrackingState == SkeletonTrackingState.Tracked)
                            {
                                if (skel.TrackingId == activeTrackingId)
                                {
                                    DrawBonesAndJoints(skel, dc, true, renderingMode);
                                }
                                else
                                {
                                    DrawBonesAndJoints(skel, dc, false, renderingMode);
                                }
                            }
                            else if (skel.TrackingState == SkeletonTrackingState.PositionOnly)
                            {
                                dc.DrawEllipse(
                                centerPointBrush,
                                null,
                                SkeletonPointToScreen(skel.Position),
                                BodyCenterThickness,
                                BodyCenterThickness);
                            }
                        }
                    }

                    // Prevent drawing outside of our render area
                    drawingGroup.ClipGeometry = new RectangleGeometry(new Rect(0.0, 0.0, this.RenderWidth, this.RenderHeight));

                }
            }
        }


        /// <summary>
        /// Renders a single skeleton
        /// </summary>
        /// <param name="skeleton">skeleton to render</param>
        public void RenderSkeleton(Skeleton skel, string renderingMode, int curFrame, int maxFrame)
        {
            // Render only if rendering is enabled
            if (enabled)
            {
                using (DrawingContext dc = drawingGroup.Open())
                {
                    // Draw a transparent background to set the render size
                    dc.DrawRectangle(backgroundBrush, null, new Rect(0.0, 0.0, RenderWidth, RenderHeight));

                    if (renderingMode != Const.POSTURE)
                    {
                        double progressBarWidth = (RenderWidth / maxFrame) * curFrame;
                        dc.DrawRectangle(Brushes.Black, null, new Rect(0.0, RenderHeight - 10.0, RenderWidth, 10));
                        dc.DrawRectangle(progressBarBrush, null, new Rect(0.0, RenderHeight - 10.0, progressBarWidth, 10));
                    }

                    if (skel.TrackingState == SkeletonTrackingState.Tracked)
                    {
                        DrawBonesAndJoints(skel, dc, true, renderingMode);
                    }
                    else if (skel.TrackingState == SkeletonTrackingState.PositionOnly)
                    {
                        dc.DrawEllipse(
                        centerPointBrush,
                        null,
                        SkeletonPointToScreen(skel.Position),
                        BodyCenterThickness,
                        BodyCenterThickness);
                    }

                    if (whiteOverlayCount > 0)
                    {
                        dc.DrawRectangle(new SolidColorBrush(Color.FromArgb(Convert.ToByte(35*whiteOverlayCount), 255, 255, 255)), null, new Rect(0.0, 0.0, RenderWidth, RenderHeight));
                        whiteOverlayCount--;
                    }


                    // Prevent drawing outside of our render area
                    drawingGroup.ClipGeometry = new RectangleGeometry(new Rect(0.0, 0.0, RenderWidth, RenderHeight));

                }
            }
        }


        /// <summary>
        /// Draws a skeleton's bones and joints
        /// </summary>
        /// <param name="skeleton">skeleton to draw</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        private void DrawBonesAndJoints(Skeleton skeleton, DrawingContext drawingContext, Boolean isActive, string renderingMode)
        {
            Boolean focusBody, focusLH, focusRH;

            switch (renderingMode)
            {
                case Const.ALL:
                    focusBody = true;
                    focusLH = true;
                    focusRH = true;
                    break;
                case Const.BODY:
                    focusBody = true;
                    focusLH = false;
                    focusRH = false;
                    break;
                case Const.LEFT_HAND:
                    focusBody = false;
                    focusLH = true;
                    focusRH = false;
                    break;
                case Const.RIGHT_HAND:
                    focusBody = false;
                    focusLH = false;
                    focusRH = true;
                    break;
                default:
                    focusBody = true;
                    focusLH = true;
                    focusRH = true;
                    break;
            }

            // Render Torso
            DrawBone(skeleton, drawingContext, isActive, focusBody, JointType.Head, JointType.ShoulderCenter);
            DrawBone(skeleton, drawingContext, isActive, focusBody, JointType.ShoulderCenter, JointType.ShoulderLeft);
            DrawBone(skeleton, drawingContext, isActive, focusBody, JointType.ShoulderCenter, JointType.ShoulderRight);
            DrawBone(skeleton, drawingContext, isActive, focusBody, JointType.ShoulderCenter, JointType.Spine);
            DrawBone(skeleton, drawingContext, isActive, focusBody, JointType.Spine, JointType.HipCenter);
            DrawBone(skeleton, drawingContext, isActive, focusBody, JointType.HipCenter, JointType.HipLeft);
            DrawBone(skeleton, drawingContext, isActive, focusBody, JointType.HipCenter, JointType.HipRight);

            // Left Arm
            DrawBone(skeleton, drawingContext, isActive, focusLH, JointType.ShoulderLeft, JointType.ElbowLeft);
            DrawBone(skeleton, drawingContext, isActive, focusLH, JointType.ElbowLeft, JointType.WristLeft);
            DrawBone(skeleton, drawingContext, isActive, focusLH, JointType.WristLeft, JointType.HandLeft);

            // Right Arm
            DrawBone(skeleton, drawingContext, isActive, focusRH, JointType.ShoulderRight, JointType.ElbowRight);
            DrawBone(skeleton, drawingContext, isActive, focusRH, JointType.ElbowRight, JointType.WristRight);
            DrawBone(skeleton, drawingContext, isActive, focusRH, JointType.WristRight, JointType.HandRight);

            // Left Leg
            DrawBone(skeleton, drawingContext, isActive, focusBody, JointType.HipLeft, JointType.KneeLeft);
            DrawBone(skeleton, drawingContext, isActive, focusBody, JointType.KneeLeft, JointType.AnkleLeft);
            DrawBone(skeleton, drawingContext, isActive, focusBody, JointType.AnkleLeft, JointType.FootLeft);

            // Right Leg
            DrawBone(skeleton, drawingContext, isActive, focusBody, JointType.HipRight, JointType.KneeRight);
            DrawBone(skeleton, drawingContext, isActive, focusBody, JointType.KneeRight, JointType.AnkleRight);
            DrawBone(skeleton, drawingContext, isActive, focusBody, JointType.AnkleRight, JointType.FootRight);

            // Render Joints
            foreach (Joint joint in skeleton.Joints)
            {
                Brush drawBrush = null;

                if (joint.TrackingState == JointTrackingState.Tracked)
                {
                    if (isActive)
                    {
                        drawBrush = trackedJointBrush;
                    }
                    else
                    {
                        drawBrush = inactiveJointBrush;
                    }
                }
                else if (joint.TrackingState == JointTrackingState.Inferred)
                {
                    drawBrush = inferredJointBrush;
                }

                if (drawBrush != null)
                {
                    drawingContext.DrawEllipse(drawBrush, null, SkeletonPointToScreen(joint.Position), JointThickness, JointThickness);
                }
            }
        }


        /// <summary>
        /// Draws a bone line between two joints
        /// </summary>
        /// <param name="skeleton">skeleton to draw bones from</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        /// <param name="isActive">drawing type</param>
        /// <param name="jointType0">joint to start drawing from</param>
        /// <param name="jointType1">joint to end drawing at</param>
        private void DrawBone(Skeleton skeleton, DrawingContext drawingContext, Boolean isActive, Boolean focus, JointType jointType0, JointType jointType1)
        {
            Joint joint0 = skeleton.Joints[jointType0];
            Joint joint1 = skeleton.Joints[jointType1];

            // If we can't find either of these joints, exit
            if (joint0.TrackingState == JointTrackingState.NotTracked ||
                joint1.TrackingState == JointTrackingState.NotTracked)
            {
                return;
            }

            // Don't draw if both points are inferred
            if (joint0.TrackingState == JointTrackingState.Inferred &&
                joint1.TrackingState == JointTrackingState.Inferred)
            {
                return;
            }

            // We assume all drawn bones are inferred unless BOTH joints are tracked
            Pen drawPen = inferredBonePen;
            if (joint0.TrackingState == JointTrackingState.Tracked && joint1.TrackingState == JointTrackingState.Tracked)
            {
                if (isActive)
                {
                    if (focus)
                    {
                        drawPen = trackedBonePen;
                    }
                    else
                    {
                        drawPen = trackedBonePenNoFocus;
                    }
                }
                else
                {
                    drawPen = inactiveBonePen;
                }
            }

            drawingContext.DrawLine(drawPen, SkeletonPointToScreen(joint0.Position), SkeletonPointToScreen(joint1.Position));
        }


        /// <summary>
        /// Maps a SkeletonPoint to lie within our render space and converts to Point
        /// </summary>
        /// <param name="skelpoint">point to map</param>
        /// <returns>mapped point</returns>
        private Point SkeletonPointToScreen(SkeletonPoint skelPoint)
        {
            // Convert point to image space.  
            return new Point(ScaleVector(RenderWidth, skelPoint.X), ScaleVector(RenderHeight, -skelPoint.Y));
        }


        /// <summary>
        /// Scales SkeletonPoint vector to specific value
        /// </summary>
        /// <param name="length">scale length</param>
        /// <param name="position">origin of the vector</param>
        /// <returns>scaled vector</returns>
        private double ScaleVector(double length, float position)
        {
            double value = ((((length) / 1f) / 2f) * position) + (length / 2);
            if (value > length)
            {
                return length;
            }
            if (value < 0f)
            {
                return 0f;
            }
            return value;
        }





    }
}
