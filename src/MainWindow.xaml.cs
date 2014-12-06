using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.IO;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Microsoft.Kinect;
using System.Diagnostics;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Windows.Threading;


using KineCTRL.Streams;


namespace KineCTRL
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Active Kinect sensor
        /// </summary>
        public KinectSensor Sensor;

        /// <summary>
        /// Skeleton data for rendering
        /// </summary>
        private SkeletonLoader SkeletonLoader;

        /// <summary>
        /// Color data for rendering
        /// </summary>
        //private ColorStreamRender ColorStreamRender;

        /// <summary>
        /// Depth data for rendering
        /// </summary>
        //private DepthStreamRender DepthStreamRender;

        /// <summary>
        /// Speech recognizer
        /// </summary>
        private SpeechRecognizer SpeechRecognizer;

        /// <summary>
        /// Currently loaded profile
        /// </summary>
        private Profile Profile = new Profile();

        /// <summary>
        /// Application preferences
        /// </summary>
        private Dictionary<string, string> Preferences = new Dictionary<string,string>();

        /// <summary>
        /// Recording mode
        /// </summary>
        public String RecordingMode = Const.BODY;

        /// <summary>
        /// Waiting for key press when setting action for a gesture
        /// </summary>
        private Boolean WaitForKeyStroke = false;

        /// <summary>
        /// Currently selected gesture from the gesture list
        /// </summary>
        public Gesture ActiveGesture;

        /// <summary>
        /// Special window for debugging purposes
        /// </summary>
        DebugWindow Debug = new DebugWindow();

        public MainWindow()
        {
            InitializeComponent();
        }


        /// <summary>
        /// Execute loading of data
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void Main_Initialized(object sender, EventArgs e)
        {
            // Load application preferences
            Preferences = XMLBuilder.LoadPreferences(@"Preferences.xml");

            // Load recently loaded profile from file
            LoadProfile(Preferences["LastProfile"]);

        }

        /// <summary>
        /// Execute startup tasks
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Look through all sensors and start the first connected one
            foreach (var potentialSensor in KinectSensor.KinectSensors)
            {
                if (potentialSensor.Status == KinectStatus.Connected)
                {
                    Sensor = potentialSensor;
                    break;
                }
            }

            // Try to initialize Sensor
            InitializeSensor();

            // Initialize GUI elements
            InitializeGUI();

            // Set event handler for sensor starus changes
            KinectSensor.KinectSensors.StatusChanged += KinectSensorsStatusChanged; 
        }


        /// <summary>
        /// Event handler for sensor status changes
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        void KinectSensorsStatusChanged(object sender, StatusChangedEventArgs e)
        {
            Sensor = e.Sensor;
            InitializeSensor();
        }
        

        /// <summary>
        /// Try to initialize Kinect sensor
        /// </summary>
        public void InitializeSensor()
        {
            if (Sensor != null)
            {
                if (Sensor.Status == KinectStatus.Connected)
                {
                    //GridMain.Visibility = Visibility.Visible;
                    GridSensorError.Visibility = Visibility.Hidden;

                    // Turn on the skeleton stream to receive skeleton frames
                    Sensor.SkeletonStream.Enable();

                    // Turn on the color stream to receive color frames
                    Sensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);

                    // Turn on the depth stream to receive depth frames
                    Sensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);


                    // Start the sensor!
                    try
                    {
                        Sensor.Start();
                    }
                    catch (IOException)
                    {
                        Sensor = null;
                    }

                    // Set message for user
                    ImgSensorStatus.Source = new BitmapImage(new Uri(Const.URI_IMG_SENSOR_ON, UriKind.Relative));
                    ImgSensorStatus.ToolTip = Strings.TOOLTIP_SENSOR_ON;

                    // Output information about sensor tilt
                    LblTilt.Content = Sensor.ElevationAngle + "°";


                    // Initialize Skeleton Stream
                    SkeletonLoader = new SkeletonLoader(Main, Debug, Profile);

                    // Initialize Color Stream
                    //ColorStreamRender = new ColorStreamRender(Sensor.ColorStream.FramePixelDataLength, Sensor.ColorStream.FrameWidth, Sensor.ColorStream.FrameHeight, LblColorStreamFPS);

                    // Initialize Depth Stream
                    //DepthStreamRender = new DepthStreamRender(Sensor.DepthStream.FramePixelDataLength, Sensor.DepthStream.FrameWidth, Sensor.DepthStream.FrameHeight, LblDepthStreamFPS);

                    // Add an event handler to be called whenever there is new color frame data
                    Sensor.SkeletonFrameReady += SkeletonLoader.SensorSkeletonFrameReady;

                    // Add an event handler to be called whenever there is new color frame data
                    //Sensor.ColorFrameReady += ColorStreamRender.SensorColorFrameReady;

                    // Add an event handler to be called whenever there is new depth frame data
                    //Sensor.DepthFrameReady += DepthStreamRender.SensorDepthFrameReady;

                    // Initialize Speech Recognizer
                    SpeechRecognizer = new SpeechRecognizer(Main, Profile);
                }
                else
                {
                    GridSensorError.Visibility = Visibility.Visible;
                }
            }

            if (Sensor == null)
            {
                GridSensorError.Visibility = Visibility.Visible;
            }
        }


        /// <summary>
        /// Execute shutdown tasks
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (null != Sensor)
            {
                Sensor.Stop();
            }

            // Save current profile and preferences
            SaveProfile();
            XMLBuilder.SavePreferences(Preferences, @"Preferences.xml");

            // Close all windows
            Application.Current.Shutdown();
        }


        /// <summary>
        /// Initialize GUI elements
        /// </summary>
        private void InitializeGUI()
        {
            // Hide all edit gesture controls
            HideEditGestureControls();

            // Set focus to Tab Item element
            FocusManager.SetFocusedElement(Main, TabItemGestureRecognizer);

            // Check radio button and append gestures to listbox
            RbBodyGestures.IsChecked = true;

            // Get all available profiles
            GetAllProfiles();

            // Turn functions on/off
            GestureRecognitionActive = false;
            SpeechRecognitionActive = true;
            HandMouseActive = false;
            HandMouseMode = Const.RIGHT_HAND;
        }



        #region Gestures List Box Control

        /// <summary>
        /// List box with all saved gestures
        /// </summary>
        /// <param name="e">event arguments</param>
        private void LboxGestures_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // If user changed selection
            if (e.RemovedItems.Count > 0)
            {
                // Stop displaying removed item
                StopDisplayingSavedGesture((Gesture)e.RemovedItems[0]);

                // Hide controls for gesture edit
                HideEditGestureControls();
            }

            // If user changed selection
            if (e.AddedItems.Count > 0)
            {
                // Set active gesture to currently selected gesture
                ActiveGesture = (Gesture)e.AddedItems[0];

                // Show controls for gesture edit
                ShowEditGestureControls();

                // Fill in gesture name
                TboxGestureName.Text = ActiveGesture.Name;

                // If active gesture has already defined action, show it to user, else show text Undefined
                if (ActiveGesture.Action != null)
                {
                    TbSetAction.Text = ActiveGesture.Action.ConvertToString();
                }
                else
                {
                    TbSetAction.Text = "Undefined";
                }

                // Display active gesture
                DisplaySavedGesture(ActiveGesture);
            }
        }


        /// <summary>
        /// Display a replay of saved gesture
        /// </summary>
        /// <param name="gesture">gesture to replay</param>
        public void DisplaySavedGesture(Gesture gesture)
        {
            // Replay recording
            SkeletonLoader.RendererManager.RenderRecording(gesture.Recording, false, true);
        }


        /// <summary>
        /// Stop displaying the replay of a saved gesture
        /// </summary>
        /// <param name="gesture">gesture to stop replaying</param>
        public void StopDisplayingSavedGesture(Gesture gesture)
        {
            // Stop replaying
            SkeletonLoader.RendererManager.RenderRecordingStop();
        }


        /// <summary>
        /// Refreshes Gestures listbox with currently loaded gestures
        /// </summary>
        private void RefreshLboxGestures()
        {
            LboxGestures.ItemsSource = Profile.Gestures[RecordingMode];
            LboxGestures.DisplayMemberPath = "Name";
            LboxGestures.Items.Refresh();
        }

        #endregion

        #region Gesture Control: Define, Redefine, Save, Delete, Set Action, Remove Action, Show Controls, Hide Controls


        /// <summary>
        /// Definition of gesture
        /// </summary>
        public void DefineGesture(Gesture gestureToRedefine = null)
        {
            // Stop replays
            SkeletonLoader.RendererManager.RenderRecordingStop();

            // Reset recorder
            SkeletonLoader.Recorder.ResetRecorder();

            // Set gesture to be redefined
            SkeletonLoader.Recorder.RedefineGesture = gestureToRedefine;

            // Enable recording
            SkeletonLoader.RecordingEnabled = true;
        }

        /// <summary>
        /// Button for new body gesture definition
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void BtnNewGesture_Click(object sender, RoutedEventArgs e)
        {
            DefineGesture();
        }


        /// <summary>
        /// Button for redefining of gesture
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void BtnRedefineGesture_Click(object sender, RoutedEventArgs e)
        {
            DefineGesture(ActiveGesture);
        }

        /// <summary>
        /// Save gesture button
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void BtnSaveGestureName_Click(object sender, RoutedEventArgs e)
        {
            // Check if gesture name is not empty
            if (TboxGestureName.Text.Length > 0)
            {
                // Save name
                ActiveGesture.Name = TboxGestureName.Text;

                // Refresh listbox
                LboxGestures.Items.Refresh();
            }
        }


        /// <summary>
        /// Delete gesture button
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void BtnDeleteGesture_Click(object sender, RoutedEventArgs e)
        {
            StopDisplayingSavedGesture(ActiveGesture);
            Profile.Gestures[ActiveGesture.Type].Remove(ActiveGesture);
            LboxGestures.Items.Refresh();
            HideEditGestureControls();
        }


        /// <summary>
        /// Set action for a gesture
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void TbSetAction_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            WaitForKeyStroke = true;
            TbSetAction.Focus();
            TbSetAction.Text = "press a key...";
        }


        /// <summary>
        /// Remove action from a gesture
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void TbSetAction_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            ActiveGesture.Action = null;
            TbSetAction.Text = "Undefined";
        }


        /// <summary>
        /// Show controls for editing saved gesture
        /// </summary>
        private void ShowEditGestureControls()
        {
            GboxGestureInfo.Visibility = Visibility.Visible;
            BtnRedefineGesture.Visibility = Visibility.Visible;
            BtnDeleteGesture.Visibility = Visibility.Visible;

            if (ActiveGesture.Type != Const.POSTURE_DEFAULT)
            {
                LblGestureName.Visibility = Visibility.Visible;
                TboxGestureName.Visibility = Visibility.Visible;
                BtnSaveGestureName.Visibility = Visibility.Visible;
                LblGestureAction.Visibility = Visibility.Visible;
                TbSetAction.Visibility = Visibility.Visible;
                BtnDeleteGesture.IsEnabled = true;
            }
            else
            {
                TbDefaultPostureInfo.Visibility = Visibility.Visible;
                BtnDeleteGesture.IsEnabled = false;
            }

        }


        /// <summary>
        /// Hide controls for editing saved gesture
        /// </summary>
        private void HideEditGestureControls()
        {
            GboxGestureInfo.Visibility = Visibility.Hidden;
            LblGestureName.Visibility = Visibility.Hidden;
            TboxGestureName.Visibility = Visibility.Hidden;
            BtnSaveGestureName.Visibility = Visibility.Hidden;
            LblGestureAction.Visibility = Visibility.Hidden;
            TbSetAction.Visibility = Visibility.Hidden;
            BtnDeleteGesture.Visibility = Visibility.Hidden;
            BtnRedefineGesture.Visibility = Visibility.Hidden;
            TbDefaultPostureInfo.Visibility = Visibility.Hidden;
        }


        #endregion

        #region Gesture Type Selection

        private void RbBodyGestures_Checked(object sender, RoutedEventArgs e)
        {
            RbLHGestures.IsChecked = false;
            RbRHGestures.IsChecked = false;
            RbPostures.IsChecked = false;
            RecordingMode = Const.BODY;
            RefreshLboxGestures();
            HideEditGestureControls();
        }

        private void RbLHGestures_Checked(object sender, RoutedEventArgs e)
        {
            RbBodyGestures.IsChecked = false;
            RbRHGestures.IsChecked = false;
            RbPostures.IsChecked = false;
            RecordingMode = Const.LEFT_HAND;
            RefreshLboxGestures();
            HideEditGestureControls();
        }

        private void RbRHGestures_Checked(object sender, RoutedEventArgs e)
        {
            RbBodyGestures.IsChecked = false;
            RbLHGestures.IsChecked = false;
            RbPostures.IsChecked = false;
            RecordingMode = Const.RIGHT_HAND;
            RefreshLboxGestures();
            HideEditGestureControls();
        }

        private void RbPostures_Checked(object sender, RoutedEventArgs e)
        {
            RbBodyGestures.IsChecked = false;
            RbLHGestures.IsChecked = false;
            RbRHGestures.IsChecked = false;
            RecordingMode = Const.POSTURE;
            RefreshLboxGestures();
            HideEditGestureControls();
        }

        #endregion

        #region Tabs Control

        /// <summary>
        /// Change application tab
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is System.Windows.Controls.TabControl)
            {
                // Gesture Manager is selected
                if (TabItemGestureManager.IsSelected)
                {
                    if (SkeletonLoader != null)
                    {
                        SkeletonLoader.RendererMain.Enabled = false;
                        SkeletonLoader.RendererManager.Enabled = true;
                    }
                }

                // Gesture Recognizer is selected
                if (TabItemGestureRecognizer.IsSelected)
                {
                    if (SkeletonLoader != null)
                    {
                        SkeletonLoader.RendererMain.Enabled = true;
                        SkeletonLoader.RendererManager.Enabled = false;
                    }
                }

                // Advanced Settings is selected
                if (TabItemAdvancedSettings.IsSelected)
                {
                    SetAdvancedSettingsValues();
                }
            }
        }


        #endregion

        #region Profiles Control

        /// <summary>
        /// Create new profile
        /// </summary>
        private void CreateNewProfile()
        {
            // Set file name for the new profile
            string name = "New Profile";
            string path = @"Profiles\" + name + ".xml";

            // Check if this file name already exists
            int i = 1;
            while (File.Exists(path))
            {
                name = "New Profile (" + i + ")";
                path = @"Profiles\" + name + ".xml";
                i++;
            }

            // Write blank profile from Resources to file
            File.WriteAllBytes(path, Properties.Resources.DefaultProfile);

            // Refresh Profiles ListBox
            GetAllProfiles();
        
            // Load new profile
            LoadProfile(name);
        }


        /// <summary>
        /// Save profile to XML file
        /// </summary>
        private void SaveProfile()
        {
            XMLBuilder.SaveProfile(Profile, @"Profiles\" + Preferences["LastProfile"] + ".xml");
        }


        /// <summary>
        /// Load all saved gestures from XML file
        /// </summary>
        private void LoadProfile(string profile)
        {
            // Generate path to profile file
            string path = @"Profiles\" + profile + ".xml";

            // Check if this profile file exists
            if (File.Exists(path))
            {
                // Save profile to preferences as last loaded profile
                Preferences["LastProfile"] = profile;

                // Build profile from XML file
                XMLBuilder.LoadProfile(Profile, path);

                // Set label for currently loaded profile
                TbCurrentProfile.Text = Preferences["LastProfile"];

                // Refresh Gestures listbox
                RefreshLboxGestures();

                // Refresh default posture
                if (SkeletonLoader != null)
                {
                    SkeletonLoader.PostureRecognizer.SetDefaultPosture();
                }

                // Set settings
                SetAdvancedSettingsValues();
            }
            // Profile file doesn't exist
            else
            {
                // If this is at the start of the application, create new profile
                if (Profile.Gestures.Count == 0)
                {
                    CreateNewProfile();
                }
                else
                {
                    // Show error message
                    MessageBox.Show(Strings.ERR_PROFILE_NOT_FOUND, Strings.ERR_CAPTION);
                }
            }
        }


        /// <summary>
        /// Get all available profiles in the Profiles folder
        /// </summary>
        private void GetAllProfiles()
        {
            LboxProfiles.Items.Clear();
            DirectoryInfo d = new DirectoryInfo(@"Profiles\");
            foreach (var file in d.GetFiles("*.xml"))
            {
                LboxProfiles.Items.Add(Path.GetFileNameWithoutExtension(file.Name));
            }
        }


        /// <summary>
        /// Load profile by double clicking on its name
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void LboxProfiles_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (LboxProfiles.SelectedItem != null)
            {
                SaveProfile();
                LoadProfile((string)LboxProfiles.SelectedItem);
            }
        }


        /// <summary>
        /// Load profile by clicking on a button
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void BtnLoadProfile_Click(object sender, RoutedEventArgs e)
        {
            if (LboxProfiles.SelectedItem != null)
            {
                SaveProfile();
                LoadProfile((string)LboxProfiles.SelectedItem);
            }
        }


        /// <summary>
        /// Click on the New profile button
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void BtnNewProfile_Click(object sender, RoutedEventArgs e)
        {
            SaveProfile();
            CreateNewProfile();
        }


        /// <summary>
        /// Create a copy of a profile
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void BtnCopyProfile_Click(object sender, RoutedEventArgs e)
        {
            if (LboxProfiles.SelectedItem != null)
            {
                string oldName = (string)LboxProfiles.SelectedItem;
                string oldPath = @"Profiles\" + oldName + ".xml";

                // Set file name for the new profile
                string newName = oldName + " Copy";
                string newPath = @"Profiles\" + newName + ".xml";

                // Check if this file name already exists
                int i = 1;
                while (File.Exists(newPath))
                {
                    newName = oldName + " Copy (" + i + ")";
                    newPath = @"Profiles\" + newName + ".xml";
                    i++;
                }

                // Copy
                File.Copy(oldPath, newPath, true);

                // Refresh Profiles ListBox
                GetAllProfiles();

                SaveProfile();
                LoadProfile(newName);
            }
        }


        /// <summary>
        /// Delete selected profile
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void BtnDeleteProfile_Click(object sender, RoutedEventArgs e)
        {
            // Check if profile was selected
            if (LboxProfiles.SelectedItem != null)
            {
                string name = (string)LboxProfiles.SelectedItem;
                string path = @"Profiles\" + name + ".xml";

                // Check if selected profile is not loaded
                if (name != Preferences["LastProfile"])
                {
                    // Delete profile file
                    File.Delete(path);

                    // Refresh Profiles ListBox
                    GetAllProfiles();
                }
                else
                {
                    // Show error message
                    MessageBox.Show(Strings.ERR_CANNOT_DELETE_PROFILE, Strings.ERR_CAPTION);
                }
            }
        }


        private void LboxProfiles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Check if profile was selected
            if (LboxProfiles.SelectedItem != null)
            {
                TboxRenameProfile.Text = (string)LboxProfiles.SelectedItem;
            }
        }

        private void BtnRenameProfile_Click(object sender, RoutedEventArgs e)
        {
            // Check if profile was selected
            if (LboxProfiles.SelectedItem != null)
            {
                string oldName = (string)LboxProfiles.SelectedItem;
                string oldPath = @"Profiles\" + oldName + ".xml";

                string newName = TboxRenameProfile.Text;
                string newPath = @"Profiles\" + newName + ".xml";

                // Check if profile with that name already exists
                if (!File.Exists(newPath))
                {
                    // Rename file
                    File.Move(oldPath, newPath);

                    // Check if renamed profile was loaded
                    if (Preferences["LastProfile"] == oldName)
                    {
                        Preferences["LastProfile"] = newName;
                        TbCurrentProfile.Text = newName;
                    }

                    // Refresh Profiles ListBox
                    GetAllProfiles();
                }
                else
                {
                    // Show error message
                    MessageBox.Show(Strings.ERR_PROFILE_NAME_EXISTS, Strings.ERR_CAPTION);
                }

            }
        }

        #endregion

        #region Recognition Control Buttons

        private bool gestureRecognitionActive = false;
        public bool GestureRecognitionActive
        {
            get 
            {
                return gestureRecognitionActive; 
            }
            set 
            {
                gestureRecognitionActive = value;

                if (SkeletonLoader != null)
                {
                    SkeletonLoader.RecognizerBody.Active = gestureRecognitionActive;
                    SkeletonLoader.RecognizerLH.Active = gestureRecognitionActive;
                    SkeletonLoader.RecognizerRH.Active = gestureRecognitionActive;
                    SkeletonLoader.PostureRecognizer.Active = gestureRecognitionActive;

                    if (gestureRecognitionActive)
                    {
                        ImgRecognitionStatus.Source = new BitmapImage(new Uri(Const.URI_IMG_RECOGNITION_ON, UriKind.Relative));
                        ImgRecognitionStatus.ToolTip = Strings.TOOLTIP_RECOGNITION_ON;
                    }
                    else
                    {
                        ImgRecognitionStatus.Source = new BitmapImage(new Uri(Const.URI_IMG_RECOGNITION_OFF, UriKind.Relative));
                        ImgRecognitionStatus.ToolTip = Strings.TOOLTIP_RECOGNITION_OFF;
                    }
                }
            }
        }

        private void ImgRecognitionStatus_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            GestureRecognitionActive = !GestureRecognitionActive;
        }


        private bool speechRecognitionActive = false;
        public bool SpeechRecognitionActive
        {
            get
            {
                return speechRecognitionActive;
            }
            set
            {
                speechRecognitionActive = value;

                if (SpeechRecognizer != null)
                {
                    SpeechRecognizer.Active = speechRecognitionActive;

                    if (speechRecognitionActive)
                    {
                        ImgSpeechStatus.Source = new BitmapImage(new Uri(Const.URI_IMG_SPEECH_ON, UriKind.Relative));
                        ImgSpeechStatus.ToolTip = Strings.TOOLTIP_SPEECH_ON;
                    }
                    else
                    {
                        ImgSpeechStatus.Source = new BitmapImage(new Uri(Const.URI_IMG_SPEECH_OFF, UriKind.Relative));
                        ImgSpeechStatus.ToolTip = Strings.TOOLTIP_SPEECH_OFF;
                    }
                }
            }
        }

        private void ImgSpeechStatus_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            SpeechRecognitionActive = !SpeechRecognitionActive;
        }


        private bool handMouseActive = false;
        public bool HandMouseActive
        {
            get
            {
                return handMouseActive;
            }
            set
            {
                handMouseActive = value;

                if (SkeletonLoader != null)
                {
                    SkeletonLoader.HandMouseRecognizer.Active = handMouseActive;

                    if (handMouseActive)
                    {
                        ImgHandMouseStatus.Source = new BitmapImage(new Uri(Const.URI_IMG_HANDMOUSE_ON, UriKind.Relative));
                        ImgHandMouseStatus.ToolTip = Strings.TOOLTIP_HANDMOUSE_ON;
                    }
                    else
                    {
                        ImgHandMouseStatus.Source = new BitmapImage(new Uri(Const.URI_IMG_HANDMOUSE_OFF, UriKind.Relative));
                        ImgHandMouseStatus.ToolTip = Strings.TOOLTIP_HANDMOUSE_OFF;
                    }
                }
            }
        }



        private void ImgHandMouseStatus_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            HandMouseActive = !HandMouseActive;
        }

        #endregion

        #region Tilt Control

        /// <summary>
        /// Tilt sensor up
        /// </summary>
        public void SensorTiltUp()
        {
            // Change elevation angle if possible
            if ((Sensor.ElevationAngle + 5) <= Sensor.MaxElevationAngle)
            {
                try
                {
                    Sensor.ElevationAngle += 5;
                }
                catch (InvalidOperationException e)
                {
                    Console.Out.WriteLine("Sensor Elevation Angle exception has been handled: " + e.Message);
                }
            }
            else
            {
                try
                {
                    Sensor.ElevationAngle = Sensor.MaxElevationAngle;
                }
                catch (InvalidOperationException e)
                {
                    Console.Out.WriteLine("Sensor Elevation Angle exception has been handled: " + e.Message);
                }
            }

            // Show current elevation angle to user
            LblTilt.Content = Sensor.ElevationAngle + "°";
        }


        /// <summary>
        /// Tilt sensor down
        /// </summary>
        public void SensorTiltDown()
        {
            // Change elevation angle if possible
            if ((Sensor.ElevationAngle - 5) >= Sensor.MinElevationAngle)
            {
                try
                {
                    Sensor.ElevationAngle -= 5;
                }
                catch (InvalidOperationException e) 
                {
                    Console.Out.WriteLine("Sensor Elevation Angle exception has been handled: " + e.Message);
                }
            }
            else
            {
                try
                {
                    Sensor.ElevationAngle = Sensor.MinElevationAngle;
                }
                catch (InvalidOperationException e) 
                {
                    Console.Out.WriteLine("Sensor Elevation Angle exception has been handled: " + e.Message);
                }
            }

            // Show current elevation angle to user
            LblTilt.Content = Sensor.ElevationAngle + "°";
        }

        private void ImgTiltUp_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SensorTiltUp();
        }

        private void ImgTiltDown_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SensorTiltDown();
        }

        #endregion

        #region HandMouse Control


        /// <summary>
        /// Get/Set value of the HandMouse mode
        /// </summary>
        private string handMouseMode = Const.RIGHT_HAND;
        public string HandMouseMode
        {
            get
            {
                return handMouseMode;
            }
            set
            {
                handMouseMode = value;

                if (handMouseMode == Const.LEFT_HAND)
                {
                    ImgHandMouseModeLeft.Source = new BitmapImage(new Uri(Const.URI_IMG_HANDMOUSE_LEFT_ON, UriKind.Relative));
                    ImgHandMouseModeRight.Source = new BitmapImage(new Uri(Const.URI_IMG_HANDMOUSE_RIGHT_OFF, UriKind.Relative));
                }
                else
                {
                    ImgHandMouseModeLeft.Source = new BitmapImage(new Uri(Const.URI_IMG_HANDMOUSE_LEFT_OFF, UriKind.Relative));
                    ImgHandMouseModeRight.Source = new BitmapImage(new Uri(Const.URI_IMG_HANDMOUSE_RIGHT_ON, UriKind.Relative));
                }
            }
        }


        /// <summary>
        /// Set HandMouse mode to Left
        /// </summary>
        private void ImgHandMouseModeLeft_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            HandMouseMode = Const.LEFT_HAND;
        }


        /// <summary>
        /// Set HandMouse mode to Right
        /// </summary>
        private void ImgHandMouseModeRight_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            HandMouseMode = Const.RIGHT_HAND;
        }


        #endregion

        #region Key Events Handling

        /// <summary>
        /// Handling key presses when defining an action for a gesture
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            // Check if user wants to define new action
            if (WaitForKeyStroke)
            {

                // Return if user pressed a special key, we'll handle them in the KeyUp event
                if (e.Key == Key.LeftShift || e.Key == Key.RightShift || e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl || e.Key == Key.LeftAlt || e.Key == Key.RightAlt || e.Key == Key.LWin || e.Key == Key.RWin)
                {
                    return;
                }

                // Set new action for currently selected gesture
                ActiveGesture.Action = new KeyInput(e);

                // Show text representation of the action
                TbSetAction.Text = ActiveGesture.Action.ConvertToString();

                // Stop waiting for key press
                WaitForKeyStroke = false;
            }
        }


        /// <summary>
        /// Handling special keys when defining an action for a gesture
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void Window_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            // Check if user wants to define new action
            if (WaitForKeyStroke)
            {
                // If user pressed a special key and nothing else, save new action for active gesture
                if (e.Key == Key.LeftShift || e.Key == Key.RightShift || e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl || e.Key == Key.LeftAlt || e.Key == Key.RightAlt || e.Key == Key.LWin || e.Key == Key.RWin)
                {
                    ActiveGesture.Action = new KeyInput(e);
                }

                // Convert action to text
                TbSetAction.Text = ActiveGesture.Action.ConvertToString();

                // Stop waiting for key press
                WaitForKeyStroke = false;
            }
        }

        #endregion

        #region Output Stream Control

        /// <summary>
        /// ComboBox for switching image output
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void ComboBoxOutputStream_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (null != Sensor)
            {
                if (comboBoxOutputStream.SelectedIndex == 0)
                {
                    // Set the output image to skeleton stream data
                    ImageOutputMain.Source = SkeletonLoader.RendererMain.DrawingImage;
                }
                else if (comboBoxOutputStream.SelectedIndex == 1)
                {
                    // Set the output image to color stream data
                    //ImageOutputMain.Source = ColorStreamRender.ColorBitmap;
                }
                else if (comboBoxOutputStream.SelectedIndex == 2)
                {
                    // Set the output image to depth stream data
                    //ImageOutputMain.Source = DepthStreamRender.ColorBitmap;
                }
                else
                {
                    ImageOutputMain.Source = null;
                }
            }
        }

        #endregion

        #region Settings Control

        private void BtnShowDebugWindow_Click(object sender, RoutedEventArgs e)
        {
            Debug.Show();
        }

        private void UpdateAllSettings()
        {
            if (SkeletonLoader != null)
            {
                SkeletonLoader.RecognizerBody.UpdateSettings();
                SkeletonLoader.RecognizerLH.UpdateSettings();
                SkeletonLoader.RecognizerRH.UpdateSettings();
                SkeletonLoader.PostureRecognizer.UpdateSettings();
                SkeletonLoader.Recorder.UpdateSettings();
                SkeletonLoader.HandMouseRecognizer.UpdateSettings();
            }

            if (SpeechRecognizer != null)
            {
                SpeechRecognizer.UpdateSettings();
            }
        }

        private void BtnResetSettingsToDefault_Click(object sender, RoutedEventArgs e)
        {
            SldrBodyRecognizerDetection.Value = Const.DEFAULT_BODY_RECOGNIZER_DETECTION;
            SldrBodyRecognizerConfidence.Value = Const.DEFAULT_BODY_RECOGNIZER_CONFIDENCE;
            SldrHandRecognizerDetection.Value = Const.DEFAULT_HAND_RECOGNIZER_DETECTION;
            SldrHandRecognizerConfidence.Value = Const.DEFAULT_HAND_RECOGNIZER_CONFIDENCE;
            SldrPostureRecognizerDefinition.Value = Const.DEFAULT_POSTURE_RECOGNIZER_DEFINITION;
            SldrPostureRecognizerConfidence.Value = Const.DEFAULT_POSTURE_RECOGNIZER_CONFIDENCE;
            SldrRecorderDetection.Value = Const.DEFAULT_RECORDER_DETECTION;
            SldrRecorderReady.Value = Const.DEFAULT_RECORDER_READY;
            SldrSpeechRecognizerConfidence.Value = Const.DEFAULT_SPEECH_RECOGNIZER_CONFIDENCE;
            SldrHandMouseSensitivity.Value = Const.DEFAULT_HANDMOUSE_SENSITIVITY;
            CbExecuteKeyPresses.IsChecked = true;
            CbDirectXmode.IsChecked = false;
        }

        private void SetAdvancedSettingsValues()
        {
            // Set values to sliders
            SldrBodyRecognizerDetection.Value = Convert.ToDouble(Profile.Settings["BodyRecognizerDetection"]);
            SldrBodyRecognizerConfidence.Value = Convert.ToDouble(Profile.Settings["BodyRecognizerConfidence"]);
            SldrHandRecognizerDetection.Value = Convert.ToDouble(Profile.Settings["HandRecognizerDetection"]);
            SldrHandRecognizerConfidence.Value = Convert.ToDouble(Profile.Settings["HandRecognizerConfidence"]);
            SldrPostureRecognizerDefinition.Value = Convert.ToDouble(Profile.Settings["PostureRecognizerDefinition"]);
            SldrPostureRecognizerConfidence.Value = Convert.ToDouble(Profile.Settings["PostureRecognizerConfidence"]);
            SldrRecorderDetection.Value = Convert.ToDouble(Profile.Settings["RecorderDetection"]);
            SldrRecorderReady.Value = Convert.ToDouble(Profile.Settings["RecorderReady"]);
            SldrHandMouseSensitivity.Value = Convert.ToDouble(Profile.Settings["HandMouseSensitivity"]);
            SldrSpeechRecognizerConfidence.Value = Convert.ToDouble(Profile.Settings["SpeechRecognizerConfidence"]);

            // Set value for DirectX mode 
            if (Profile.Settings["KeyPressMultiplier"] == "true")
            {
                CbDirectXmode.IsChecked = true;
            }
            else
            {
                CbDirectXmode.IsChecked = false;
            }

            // Set value for Key Presses execution
            if (Profile.Settings["ExecuteKeyPresses"] == "true")
            {
                CbExecuteKeyPresses.IsChecked = true;
            }
            else
            {
                CbExecuteKeyPresses.IsChecked = false;
            }
        }

        private void SldrBodyRecognizerDetection_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Profile.Settings["BodyRecognizerDetection"] = Convert.ToString(e.NewValue);
            UpdateAllSettings();
        }

        private void SldrBodyRecognizerConfidence_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Profile.Settings["BodyRecognizerConfidence"] = Convert.ToString(e.NewValue);
            UpdateAllSettings();
        }

        private void SldrHandRecognizerDetection_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Profile.Settings["HandRecognizerDetection"] = Convert.ToString(e.NewValue);
            UpdateAllSettings();
        }

        private void SldrHandRecognizerConfidence_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Profile.Settings["HandRecognizerConfidence"] = Convert.ToString(e.NewValue);
            UpdateAllSettings();
        }

        private void SldrPostureRecognizerDefinition_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Profile.Settings["PostureRecognizerDefinition"] = Convert.ToString(e.NewValue);
            UpdateAllSettings();
        }

        private void SldrPostureRecognizerConfidence_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Profile.Settings["PostureRecognizerConfidence"] = Convert.ToString(e.NewValue);
            UpdateAllSettings();
        }

        private void SldrRecorderDetection_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Profile.Settings["RecorderDetection"] = Convert.ToString(e.NewValue);
            UpdateAllSettings();
        }

        private void SldrRecorderReady_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Profile.Settings["RecorderReady"] = Convert.ToString(e.NewValue);
            UpdateAllSettings();
        }

        private void SldrSpeechRecognizerConfidence_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Profile.Settings["SpeechRecognizerConfidence"] = Convert.ToString(e.NewValue);
            UpdateAllSettings();
        }

        private void SldrHandMouseSensitivity_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Profile.Settings["HandMouseSensitivity"] = Convert.ToString(e.NewValue);
            UpdateAllSettings();
        }

        private void CbDirectXmode_Checked(object sender, RoutedEventArgs e)
        {
            Profile.Settings["KeyPressMultiplier"] = "true";
            UpdateAllSettings();
        }

        private void CbDirectXmode_Unchecked(object sender, RoutedEventArgs e)
        {
            Profile.Settings["KeyPressMultiplier"] = "false";
            UpdateAllSettings();
        }

        private void CbExecuteKeyPresses_Checked(object sender, RoutedEventArgs e)
        {
            Profile.Settings["ExecuteKeyPresses"] = "true";
            UpdateAllSettings();
        }

        private void CbExecuteKeyPresses_Unchecked(object sender, RoutedEventArgs e)
        {
            Profile.Settings["ExecuteKeyPresses"] = "false";
            UpdateAllSettings();
        }

        #endregion

        #region About

        private void ImgLogo_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("KineCTRL v1.0" + Environment.NewLine + "© Jakub Kosnar 2014", "About");
        }

        #endregion



    }
}
