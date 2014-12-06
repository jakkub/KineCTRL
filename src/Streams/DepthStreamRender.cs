using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using Microsoft.Kinect;

namespace KineCTRL.Streams
{
    class DepthStreamRender
    {
        /// <summary>
        /// Length of the pixel data buffer of each image frame
        /// </summary>
        private int FramePixelDataLength;

        /// <summary>
        /// Width of color image frame
        /// </summary>
        private int FrameWidth;

        /// <summary>
        /// Height of color image frame
        /// </summary>
        private int FrameHeight;

        /// <summary>
        /// Temporary storage for the depth data received from the camera
        /// </summary>
        private DepthImagePixel[] depthPixels;

        /// <summary>
        /// Temporary storage for the depth data converted to color
        /// </summary>
        private byte[] colorPixels;

        /// <summary>
        /// Bitmap that will hold color information
        /// </summary>
        private WriteableBitmap colorBitmap;
        public WriteableBitmap ColorBitmap
        {
            get { return this.colorBitmap; }
        }

        private int totalFrames = 0;
        private int lastFrames = 0;
        private DateTime lastTime = DateTime.MaxValue;
        Label FPSLabel;


        /// <summary>
        /// This class is used for loading, storing and rendering of depth data
        /// </summary>
        /// <param name="framePixelDataLength">length of the pixel data buffer of each image frame</param>
        /// <param name="frameWidth">width of color image frame</param>
        /// <param name="frameHeight">height of color image frame</param>
        public DepthStreamRender(int framePixelDataLength, int frameWidth, int frameHeight, Label fsplabel)
        {
            FramePixelDataLength = framePixelDataLength;
            FrameWidth = frameWidth;
            FrameHeight = frameHeight;

            this.FPSLabel = fsplabel;
            lastTime = DateTime.Now;

            // Allocate space to put the depth pixels we'll receive
            this.depthPixels = new DepthImagePixel[this.FramePixelDataLength];

            // Allocate space to put the color pixels we'll create
            this.colorPixels = new byte[this.FramePixelDataLength * sizeof(int)];

            // This is the bitmap we'll display on-screen
            this.colorBitmap = new WriteableBitmap(this.FrameWidth, this.FrameHeight, 96.0, 96.0, PixelFormats.Bgr32, null);
        }

        private void CalculateFps()
        {
            ++this.totalFrames;

            var cur = DateTime.Now;
            if (cur.Subtract(this.lastTime) > TimeSpan.FromSeconds(1))
            {
                int frameDiff = this.totalFrames - this.lastFrames;
                this.lastFrames = this.totalFrames;
                this.lastTime = cur;
                this.FPSLabel.Content = frameDiff.ToString() + " fps";
            }

        }

        /// <summary>
        /// Event handler for Kinect sensor's DepthFrameReady event
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        public void SensorDepthFrameReady(object sender, DepthImageFrameReadyEventArgs e)
        {
            using (DepthImageFrame depthFrame = e.OpenDepthImageFrame())
            {
                if (depthFrame != null)
                {
                    // Copy the pixel data from the image to a temporary array
                    depthFrame.CopyDepthImagePixelDataTo(this.depthPixels);

                    // Get the min and max reliable depth for the current frame
                    int minDepth = depthFrame.MinDepth;
                    int maxDepth = depthFrame.MaxDepth;

                    // Convert the depth to RGB
                    int colorPixelIndex = 0;
                    for (int i = 0; i < this.depthPixels.Length; ++i)
                    {
                        // Get the depth for this pixel
                        short depth = depthPixels[i].Depth;

                        // To convert to a byte, we're discarding the most-significant
                        // rather than least-significant bits.
                        // We're preserving detail, although the intensity will "wrap."
                        // Values outside the reliable depth range are mapped to 0 (black).

                        // Note: Using conditionals in this loop could degrade performance.
                        // Consider using a lookup table instead when writing production code.
                        // See the KinectDepthViewer class used by the KinectExplorer sample
                        // for a lookup table example.

                        byte intensity = (byte)(depth >= minDepth && depth <= maxDepth ? depth : 0);
                        
                        // Write out blue byte
                        this.colorPixels[colorPixelIndex++] = intensity;

                        // Write out green byte
                        this.colorPixels[colorPixelIndex++] = 0;

                        // Write out red byte                        
                        this.colorPixels[colorPixelIndex++] = (byte)(255 - intensity);

                        // We're outputting BGR, the last byte in the 32 bits is unused so skip it
                        // If we were outputting BGRA, we would write alpha here.
                        ++colorPixelIndex;
                    }

                    // Write the pixel data into our bitmap
                    this.colorBitmap.WritePixels(
                        new Int32Rect(0, 0, this.colorBitmap.PixelWidth, this.colorBitmap.PixelHeight),
                        this.colorPixels,
                        this.colorBitmap.PixelWidth * sizeof(int),
                        0);
                }
            }

            this.CalculateFps();
        }
    }
}
