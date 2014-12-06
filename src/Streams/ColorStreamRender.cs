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
    class ColorStreamRender
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
        /// Bitmap that will hold color information
        /// </summary>
        private WriteableBitmap colorBitmap;
        public WriteableBitmap ColorBitmap
        {
            get { return this.colorBitmap; }
        }

        /// <summary>
        /// Temporary storage for the color data received from the sensor
        /// </summary>
        private byte[] colorPixels;

        private int totalFrames = 0;
        private int lastFrames = 0;
        private DateTime lastTime = DateTime.MaxValue;
        Label FPSLabel;

        /// <summary>
        /// This class is used for loading, storing and rendering of color data
        /// </summary>
        /// <param name="framePixelDataLength">length of the pixel data buffer of each image frame</param>
        /// <param name="frameWidth">width of color image frame</param>
        /// <param name="frameHeight">height of color image frame</param>
        public ColorStreamRender(int framePixelDataLength, int frameWidth, int frameHeight, Label fsplabel)
        {
            FramePixelDataLength = framePixelDataLength;
            FrameWidth = frameWidth;
            FrameHeight = frameHeight;
            this.FPSLabel = fsplabel;
            lastTime = DateTime.Now;

            // Allocate space to put the pixels we'll receive
            this.colorPixels = new byte[this.FramePixelDataLength];

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
        /// Event handler for Kinect sensor's ColorFrameReady event
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        public void SensorColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            using (ColorImageFrame colorFrame = e.OpenColorImageFrame())
            {
                if (colorFrame != null)
                {
                    // Copy the pixel data from the image to a temporary array
                    colorFrame.CopyPixelDataTo(this.colorPixels);

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
