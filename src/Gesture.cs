using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KineCTRL.DollarRecognizer;


namespace KineCTRL
{
    public class Gesture
    {
        /// <summary>
        /// Name of the gesture
        /// </summary>
        private string name;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// Type of the gesture
        /// </summary>
        private string type;
        public string Type
        {
            get { return type; }
            set { type = value; }
        }

        /// <summary>
        /// Action to execute when the gesture is recognized
        /// </summary>
        private KeyInput action;
        public KeyInput Action
        {
            get { return action; }
            set { action = value; }
        }

        /// <summary>
        /// Number of remaining frames where this gesture's action has to be executed
        /// </summary>
        private int keyPressCount;
        public int KeyPressCount
        {
            get { return keyPressCount; }
            set { keyPressCount = value; }
        }

        /// <summary>
        /// Recording of the gesture
        /// </summary>
        private SkeletonRecording recording;
        public SkeletonRecording Recording
        {
            get { return recording; }
            set { recording = value; }
        }

        /// <summary>
        /// Gesture in normalized representation
        /// </summary>
        private NormalizedGesture normalizedGesture;
        public NormalizedGesture NormalizedGesture
        {
            get { return normalizedGesture; }
            set { normalizedGesture = value; }
        }


        public Gesture()
        {
        }

        /// <summary>
        /// Gesture object
        /// </summary>
        /// <param name="_name">name of the gesture</param>
        /// <param name="_type">type of the gesture</param>
        /// <param name="_recording">SkeletonRecording of the gesture</param>
        public Gesture(string _name, string _type, SkeletonRecording _recording)
        {
            name = _name;
            type = _type;
            recording = _recording;
            NormalizedGesture = new DollarRecognizer.NormalizedGesture(DollarRecognizer.SkeletonRecordingToPointCloud.Convert(recording, type));
        }


        /// <summary>
        /// Executes gesture action if needed
        /// </summary>
        public void ExecuteKeyPress()
        {
            // If there are remaining frames for KeyDown command, execute it
            if (keyPressCount > 0)
            {
                ExecuteKeyDown();
                keyPressCount--;
            }
            // Execute KeyUp command if no more KeyDown frames are left
            else if (keyPressCount == 0)
            {
                ExecuteKeyUp();
                keyPressCount--;
            }
        }


        /// <summary>
        /// Executes gesture action for specified number of frames
        /// </summary>
        /// <param name="n">number of frames this gesture has to be executed for</param>
        public void ExecuteKeyPress(int n)
        {
            // Set number of remaining frames
            keyPressCount = n;

            // Execute action
            ExecuteKeyPress();
        }

        /// <summary>
        /// Executes gesture action key down command
        /// </summary>
        public void ExecuteKeyDown()
        {
            // Execute key down command if action exists
            if (Action != null)
            {
                Action.ExecuteKeyDown();
            }
        }

        /// <summary>
        /// Executes gesture action key up command
        /// </summary>
        public void ExecuteKeyUp()
        {
            // Execute key up command if action exists
            if (Action != null)
            {
                Action.ExecuteKeyUp();
            }
        }
    }
}
