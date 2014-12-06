using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using Microsoft.Speech.AudioFormat;
using Microsoft.Speech.Recognition;
using System.IO;

namespace KineCTRL
{
    class SpeechRecognizer
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
        /// Speech recognition trigger on/off
        /// </summary>
        private bool active = true;
        public bool Active
        {
            get { return active; }
            set { active = value; }
        }

        /// <summary>
        /// Speech recognition engine using audio data from Kinect.
        /// </summary>
        private SpeechRecognitionEngine speechEngine;

        /// <summary>
        /// Metadata for the speech recognizer (acoustic model)
        /// </summary>
        public RecognizerInfo ri;

        /// <summary>
        /// Speech confidence threshold
        /// </summary>
        private float ConfidenceThreshold;


        public SpeechRecognizer(MainWindow main, Profile profile)
        {
            Main = main;
            Profile = profile;

            // Set confidence threshold for the recognizer
            UpdateSettings();

            ri = GetKinectRecognizer();

            if (null != ri)
            {
                speechEngine = new SpeechRecognitionEngine(ri.Id);

                // Create a grammar from grammar definition XML file.
                using (var memoryStream = new MemoryStream(Encoding.ASCII.GetBytes(Properties.Resources.SpeechGrammar)))
                {
                    var g = new Grammar(memoryStream);
                    speechEngine.LoadGrammar(g);
                }

                speechEngine.SpeechRecognized += SpeechRecognized;
                speechEngine.SpeechRecognitionRejected += SpeechRejected;

                speechEngine.SetInputToAudioStream(
                    Main.Sensor.AudioSource.Start(), new SpeechAudioFormatInfo(EncodingFormat.Pcm, 16000, 16, 1, 32000, 2, null));
                speechEngine.RecognizeAsync(RecognizeMode.Multiple);
            }
        }


        /// <summary>
        /// Obtain current sensitivity from settings and set this value
        /// </summary>
        public void UpdateSettings()
        {
            ConfidenceThreshold = Single.Parse(Profile.Settings["SpeechRecognizerConfidence"]);
        }


        /// <summary>
        /// Gets the metadata for the speech recognizer (acoustic model) most suitable to
        /// process audio from Kinect device.
        /// </summary>
        /// <returns>
        /// RecognizerInfo if found, <code>null</code> otherwise.
        /// </returns>
        private static RecognizerInfo GetKinectRecognizer()
        {
            foreach (RecognizerInfo recognizer in SpeechRecognitionEngine.InstalledRecognizers())
            {
                string value;
                recognizer.AdditionalInfo.TryGetValue("Kinect", out value);
                if ("True".Equals(value, StringComparison.OrdinalIgnoreCase) && "en-US".Equals(recognizer.Culture.Name, StringComparison.OrdinalIgnoreCase))
                {
                    return recognizer;
                }
            }

            return null;
        }

        /// <summary>
        /// Handler for recognized speech events.
        /// </summary>
        /// <param name="sender">object sending the event.</param>
        /// <param name="e">event arguments.</param>
        private void SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            // Recognize only if recognizer is active
            if (active)
            {
                // Check confidence below which we treat speech as if it hadn't been heard
                if (e.Result.Confidence >= ConfidenceThreshold)
                {
                    switch (e.Result.Semantics.Value.ToString())
                    {
                        case "GESTURE_RECOGNITION_ON":
                            Main.GestureRecognitionActive = true;
                            break;
                        case "GESTURE_RECOGNITION_OFF":
                            Main.GestureRecognitionActive = false;
                            break;
                        case "HANDMOUSE_ON":
                            Main.HandMouseActive = true;
                            break;
                        case "HANDMOUSE_OFF":
                            Main.HandMouseActive = false;
                            break;
                        case "HANDMOUSE_LEFT":
                            Main.HandMouseMode = Const.LEFT_HAND;
                            break;
                        case "HANDMOUSE_RIGHT":
                            Main.HandMouseMode = Const.RIGHT_HAND;
                            break;
                        case "CLICK_LEFT":
                            MouseInput.LeftClick();
                            break;
                        case "CLICK_RIGHT":
                            MouseInput.RightClick();
                            break;
                        case "TILT_UP":
                            Main.SensorTiltUp();
                            break;
                        case "TILT_DOWN":
                            Main.SensorTiltDown();
                            break;
                        case "NEW_GESTURE":
                            Main.TabItemGestureManager.IsSelected = true;
                            Main.DefineGesture();
                            break;
                        case "REDEFINE_GESTURE":
                            if (Main.ActiveGesture != null)
                                Main.DefineGesture(Main.ActiveGesture);
                            break;
                        case "TAB_GESTURE_RECOGNIZER":
                            Main.TabItemGestureRecognizer.IsSelected = true;
                            break;
                        case "TAB_GESTURE_MANAGER":
                            Main.TabItemGestureManager.IsSelected = true;
                            break;
                        case "RADIO_BODY_GESTURES":
                            Main.TabItemGestureManager.IsSelected = true;
                            Main.RbBodyGestures.IsChecked = true;
                            break;
                        case "RADIO_LEFT_HAND_GESTURES":
                            Main.TabItemGestureManager.IsSelected = true;
                            Main.RbLHGestures.IsChecked = true;
                            break;
                        case "RADIO_RIGHT_HAND_GESTURES":
                            Main.TabItemGestureManager.IsSelected = true;
                            Main.RbRHGestures.IsChecked = true;
                            break;
                        case "RADIO_POSTURES":
                            Main.TabItemGestureManager.IsSelected = true;
                            Main.RbPostures.IsChecked = true;
                            break;
                        case "KEY_PRESSES_ON":
                            Main.CbExecuteKeyPresses.IsChecked = true;
                            break;
                        case "KEY_PRESSES_OFF":
                            Main.CbExecuteKeyPresses.IsChecked = false;
                            break;
                        case "DIRECTX_ON":
                            Main.CbDirectXmode.IsChecked = true;
                            break;
                        case "DIRECTX_OFF":
                            Main.CbDirectXmode.IsChecked = false;
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Handler for rejected speech events.
        /// </summary>
        /// <param name="sender">object sending the event.</param>
        /// <param name="e">event arguments.</param>
        private void SpeechRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {

        }
    }
}
