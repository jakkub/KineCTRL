using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace KineCTRL
{
    class FPSCounter
    {
        /// <summary>
        /// Main Application Form
        /// </summary>
        MainWindow Main;

        /// <summary>
        /// Total number of received frames
        /// </summary>
        private int totalFrames = 0;

        /// <summary>
        /// Frames count on the last update of FPS counter
        /// </summary>
        private int lastFrames = 0;

        /// <summary>
        /// Time of the last update of FPS counter
        /// </summary>
        private DateTime lastTime = DateTime.Now;

        /// <summary>
        /// This class is used for calculation and output of the FPS rate
        /// </summary>
        /// <param name="main">main application form</param>
        public FPSCounter(MainWindow main)
        {
            Main = main;
        }

        /// <summary>
        /// Adds a new frame to FPS counter
        /// </summary>
        public void AddFrame()
        {
            ++totalFrames;

            var cur = DateTime.Now;

            if (cur.Subtract(lastTime) > TimeSpan.FromSeconds(1))
            {
                int frameDiff = totalFrames - lastFrames;
                lastFrames = totalFrames;
                lastTime = cur;
                Main.LblSkeletonStreamFPS.Content = frameDiff.ToString() + " fps";
            }
        }
    }
}
