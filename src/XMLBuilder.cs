using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Xml;
using System.Windows.Input;

namespace KineCTRL
{
    class XMLBuilder
    {
        /// <summary>
        /// Class contains methods for generating XML files from objects and vice versa
        /// </summary>
        public XMLBuilder()
        {

        }


        /// <summary>
        /// Converts application preferences to XML file
        /// </summary>
        /// <param name="dictionary">dictionary of all preferences</param>
        /// <param name="path">path to XML file</param>
        public static void SavePreferences(Dictionary<string, string> dictionary, string path)
        {
            // Create XML document
            XmlDocument xml = new XmlDocument();

            // Set declaration for XML file
            XmlDeclaration xmlDeclaration = xml.CreateXmlDeclaration("1.0", Encoding.UTF8.WebName, null);
            xml.AppendChild(xmlDeclaration);

            // Create root element
            XmlElement root = xml.CreateElement("KineCTRL");
            xml.AppendChild(root);

            // Loop over all preferences in dictionary
            foreach (KeyValuePair<string, string> preference in dictionary)
            {
                XmlElement xmlPreference = xml.CreateElement("Preference");

                // Append attribute for this element, which is the key for this preference
                XmlAttribute preferenceKey = xml.CreateAttribute("Key");
                preferenceKey.InnerText = preference.Key;
                xmlPreference.Attributes.Append(preferenceKey);

                // Add value of this preference
                xmlPreference.InnerText = preference.Value;

                // Append this node to the root element
                root.AppendChild(xmlPreference);
            }

            // Write XML to file
            xml.Save(path);
        }


        /// <summary>
        /// Builds a dictionary of application preferences from XML file
        /// </summary>
        /// <param name="path">path to input XML file</param>
        /// <returns>dictionary of preferences</returns>
        public static Dictionary<string, string> LoadPreferences(string path)
        {
            // Create new dictionary
            Dictionary<string, string> Dictionary = new Dictionary<string, string>();

            XmlDocument doc = new XmlDocument();

            doc.Load(path);

            // Get all Preference nodes
            XmlNodeList PreferenceNodeList = doc.SelectNodes("KineCTRL/Preference");

            // Loop over Preference nodes
            foreach (XmlNode preferenceNode in PreferenceNodeList)
            {
                // Get key for this preference
                string PreferenceKey = preferenceNode.Attributes.GetNamedItem("Key").Value;

                // Get value of this preference
                string PreferenceValue = preferenceNode.InnerText;

                Dictionary.Add(PreferenceKey, PreferenceValue);
            }
            return Dictionary;
        }



        /// <summary>
        /// Converts profile to XML file
        /// </summary>
        /// <param name="dictionary">dictionary of all gestures</param>
        /// <param name="path">path to XML file</param>
        public static void SaveProfile(Profile profile, string path)
        {
            // Create XML document
            XmlDocument xml = new XmlDocument();

            // Set declaration for XML file
            XmlDeclaration xmlDeclaration = xml.CreateXmlDeclaration("1.0", Encoding.UTF8.WebName, null);
            xml.AppendChild(xmlDeclaration);

            // Create root element
            XmlElement root = xml.CreateElement("KineCTRL");
            xml.AppendChild(root);

            // Loop over all gesture lists in dictionary
            foreach (KeyValuePair<string, List<Gesture>> gestures in profile.Gestures)
            {
                // Create main element for list of gestures
                XmlElement xmlgestures = xml.CreateElement("Gestures");

                // Append attribute for this element, which is the type of gesture list
                XmlAttribute gesturesType = xml.CreateAttribute("Type");
                gesturesType.InnerText = gestures.Key;
                xmlgestures.Attributes.Append(gesturesType);

                // Append element
                root.AppendChild(xmlgestures);

                // Loop over all gestures in this list
                foreach (Gesture gesture in gestures.Value)
                {
                    // Create main element for Gesture
                    XmlElement xmlgesture = xml.CreateElement("Gesture");
                    xmlgestures.AppendChild(xmlgesture);

                    // Save gesture name
                    XmlAttribute xmlname = xml.CreateAttribute("Name");
                    xmlname.InnerText = gesture.Name;
                    xmlgesture.Attributes.Append(xmlname);

                    // Save gesture type
                    XmlAttribute xmltype = xml.CreateAttribute("Type");
                    xmltype.InnerText = gesture.Type;
                    xmlgesture.Attributes.Append(xmltype);

                    // Create main element for gesture frames
                    XmlElement xmlframes = xml.CreateElement("Frames");
                    xmlgesture.AppendChild(xmlframes);

                    // Loop through gesture recording
                    foreach (Skeleton skel in gesture.Recording.GetFrames())
                    {
                        XmlElement xmlframe = xml.CreateElement("Frame");

                        // Serialize skeleton
                        BinaryFormatter bf = new BinaryFormatter();
                        MemoryStream ms = new MemoryStream();
                        bf.Serialize(ms, skel);

                        // Convert skeleton to Base64 string
                        string str = Convert.ToBase64String(ms.ToArray());

                        ms.Close();

                        // Save string to file
                        xmlframe.InnerText = str;
                        xmlframes.AppendChild(xmlframe);
                    }

                    // Save gesture action if set
                    XmlElement xmlkeystroke = xml.CreateElement("Action");
                    xmlgesture.AppendChild(xmlkeystroke);

                    if (gesture.Action != null)
                    {
                        XmlElement xmlkey = xml.CreateElement("Key");
                        xmlkey.InnerText = Convert.ToString(KeyInterop.VirtualKeyFromKey(gesture.Action.PressedKey));
                        xmlkeystroke.AppendChild(xmlkey);

                        XmlElement xmlLWinDown = xml.CreateElement("LWinDown");
                        xmlLWinDown.InnerText = Convert.ToString(gesture.Action.LWinDown);
                        xmlkeystroke.AppendChild(xmlLWinDown);

                        XmlElement xmlRWinDown = xml.CreateElement("RWinDown");
                        xmlRWinDown.InnerText = Convert.ToString(gesture.Action.RWinDown);
                        xmlkeystroke.AppendChild(xmlRWinDown);

                        XmlElement xmlLCtrlDown = xml.CreateElement("LCtrlDown");
                        xmlLCtrlDown.InnerText = Convert.ToString(gesture.Action.LCtrlDown);
                        xmlkeystroke.AppendChild(xmlLCtrlDown);

                        XmlElement xmlRCtrlDown = xml.CreateElement("RCtrlDown");
                        xmlRCtrlDown.InnerText = Convert.ToString(gesture.Action.RCtrlDown);
                        xmlkeystroke.AppendChild(xmlRCtrlDown);

                        XmlElement xmlLAltDown = xml.CreateElement("LAltDown");
                        xmlLAltDown.InnerText = Convert.ToString(gesture.Action.LAltDown);
                        xmlkeystroke.AppendChild(xmlLAltDown);

                        XmlElement xmlRAltDown = xml.CreateElement("RAltDown");
                        xmlRAltDown.InnerText = Convert.ToString(gesture.Action.RAltDown);
                        xmlkeystroke.AppendChild(xmlRAltDown);

                        XmlElement xmlLShiftDown = xml.CreateElement("LShiftDown");
                        xmlLShiftDown.InnerText = Convert.ToString(gesture.Action.LShiftDown);
                        xmlkeystroke.AppendChild(xmlLShiftDown);

                        XmlElement xmlRShiftDown = xml.CreateElement("RShiftDown");
                        xmlRShiftDown.InnerText = Convert.ToString(gesture.Action.RShiftDown);
                        xmlkeystroke.AppendChild(xmlRShiftDown);
                    }
                }
            }

            // Create main element for profile settings
            XmlElement xmlSettings = xml.CreateElement("Settings");
            root.AppendChild(xmlSettings);

            // Loop over all settings in dictionary
            foreach (KeyValuePair<string, string> setting in profile.Settings)
            {
                XmlElement xmlSetting = xml.CreateElement("Setting");
                xmlSettings.AppendChild(xmlSetting);

                XmlAttribute xmlSettingKey = xml.CreateAttribute("Key");
                xmlSettingKey.InnerText = setting.Key;
                xmlSetting.Attributes.Append(xmlSettingKey);

                xmlSetting.InnerText = setting.Value;
            }

            // Write XML to file
            xml.Save(path);
        }


        /// <summary>
        /// Builds a dictionary of gesture lists from XML file
        /// </summary>
        /// <param name="path">path to input XML file</param>
        /// <returns>dictionary of gesture lists</returns>
        public static void LoadProfile(Profile profile, string path)
        {
            // Clear old profile data
            profile.Clear();

            // Create new XML document
            XmlDocument doc = new XmlDocument();

            // Load XML file
            doc.Load(path);

            // Get all Gestures nodes
            XmlNodeList GesturesNodeList = doc.SelectNodes("KineCTRL/Gestures");

            // Loop over Gestures nodes
            foreach (XmlNode gesturesNode in GesturesNodeList)
            {
                // Get type of this Gestures list
                string GesturesType = gesturesNode.Attributes.GetNamedItem("Type").Value;

                // Create new list of gestures and add it to the dictionary
                List<Gesture> Gestures = new List<Gesture>();
                profile.Gestures.Add(GesturesType, Gestures);

                // Get all Gesture nodes
                XmlNodeList GestureNodeList = gesturesNode.SelectNodes("Gesture");

                // Loop over Gesture nodes
                foreach (XmlNode gestureNode in GestureNodeList)
                {
                    // Get name and type of the gesture
                    string GestureName = gestureNode.Attributes.GetNamedItem("Name").Value;
                    string GestureType = gestureNode.Attributes.GetNamedItem("Type").Value;

                    // Get Frames node
                    XmlNode framesNode = gestureNode.SelectSingleNode("Frames");

                    // Get all Frame nodes
                    XmlNodeList frameNodeList = framesNode.SelectNodes("Frame");

                    // Create new SkeletonRecording
                    SkeletonRecording GestureSkeletonRecording = new SkeletonRecording(GestureType);
                    
                    // Loop over all Frame nodes
                    foreach (XmlNode frameNode in frameNodeList)
                    {
                        // Convert from Base64 string and deserialize to Skeleton object
                        MemoryStream ms = new MemoryStream(Convert.FromBase64String(frameNode.InnerText));

                        BinaryFormatter bf = new BinaryFormatter();
                        Skeleton newSkeleton = (Skeleton)bf.Deserialize(ms);

                        // Add new skeleton frame
                        GestureSkeletonRecording.AddFrame(newSkeleton);
                    }
                    
                    // Create Gesture object
                    Gesture newGesture = new Gesture(GestureName, GestureType, GestureSkeletonRecording);

                    // Add new gesture to gestures list
                    Gestures.Add(newGesture);

                    // Get Action node
                    XmlNode actionNode = gestureNode.SelectSingleNode("Action");

                    // If Action node is not empty
                    if (actionNode.HasChildNodes)
                    {
                        // Get Key Node
                        XmlNode keyNode = actionNode.SelectSingleNode("Key");

                        // Create new action object
                        KeyInput newAction = new KeyInput();

                        // Set pressedKey for this action
                        newAction.PressedKey = KeyInterop.KeyFromVirtualKey(Convert.ToInt32(keyNode.InnerText));

                        // Read special keys
                        keyNode = actionNode.SelectSingleNode("LWinDown");
                        newAction.LWinDown = Convert.ToBoolean(keyNode.InnerText);

                        keyNode = actionNode.SelectSingleNode("RWinDown");
                        newAction.RWinDown = Convert.ToBoolean(keyNode.InnerText);

                        keyNode = actionNode.SelectSingleNode("LCtrlDown");
                        newAction.LCtrlDown = Convert.ToBoolean(keyNode.InnerText);

                        keyNode = actionNode.SelectSingleNode("RCtrlDown");
                        newAction.RCtrlDown = Convert.ToBoolean(keyNode.InnerText);

                        keyNode = actionNode.SelectSingleNode("LAltDown");
                        newAction.LAltDown = Convert.ToBoolean(keyNode.InnerText);

                        keyNode = actionNode.SelectSingleNode("RAltDown");
                        newAction.RAltDown = Convert.ToBoolean(keyNode.InnerText);

                        keyNode = actionNode.SelectSingleNode("LShiftDown");
                        newAction.LShiftDown = Convert.ToBoolean(keyNode.InnerText);

                        keyNode = actionNode.SelectSingleNode("RShiftDown");
                        newAction.RShiftDown = Convert.ToBoolean(keyNode.InnerText);

                        // Set action for the new gesture
                        newGesture.Action = newAction;
                    }
                }
            }
            
            // Get Settings node
            XmlNode SettingsNode = doc.SelectSingleNode("KineCTRL/Settings");

            // Get all Setting nodes
            XmlNodeList SettingNodeList = SettingsNode.SelectNodes("Setting");

            // Loop over all Setting nodes
            foreach (XmlNode setting in SettingNodeList)
            {
                string settingKey = setting.Attributes.GetNamedItem("Key").Value;
                string settingValue = setting.InnerText;

                profile.Settings.Add(settingKey, settingValue);
            }
        }

    }


    public class ProgressStreamWrapper : Stream, IDisposable
    {
        public ProgressStreamWrapper(Stream innerStream)
        {
            InnerStream = innerStream;
        }

        public Stream InnerStream { get; private set; }

        public override void Close()
        {
            InnerStream.Close();
        }

        void IDisposable.Dispose()
        {
            base.Dispose();
            InnerStream.Dispose();
        }

        public override void Flush()
        {
            InnerStream.Flush();
        }

        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            return InnerStream.BeginRead(buffer, offset, count, callback, state);
        }

        public override int EndRead(IAsyncResult asyncResult)
        {
            int endRead = InnerStream.EndRead(asyncResult);
            OnPositionChanged();
            return endRead;
        }

        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            return InnerStream.BeginWrite(buffer, offset, count, callback, state);
        }

        public override void EndWrite(IAsyncResult asyncResult)
        {
            InnerStream.EndWrite(asyncResult);
            OnPositionChanged(); ;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            long seek = InnerStream.Seek(offset, origin);
            OnPositionChanged();
            return seek;
        }

        public override void SetLength(long value)
        {
            InnerStream.SetLength(value);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int read = InnerStream.Read(buffer, offset, count);
            OnPositionChanged();
            return read;
        }

        public override int ReadByte()
        {
            int readByte = InnerStream.ReadByte();
            OnPositionChanged();
            return readByte;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            InnerStream.Write(buffer, offset, count);
            OnPositionChanged();
        }

        public override void WriteByte(byte value)
        {
            InnerStream.WriteByte(value);
            OnPositionChanged();
        }

        public override bool CanRead
        {
            get { return InnerStream.CanRead; }
        }

        public override bool CanSeek
        {
            get { return InnerStream.CanSeek; }
        }

        public override bool CanTimeout
        {
            get { return InnerStream.CanTimeout; }
        }

        public override bool CanWrite
        {
            get { return InnerStream.CanWrite; }
        }

        public override long Length
        {
            get { return InnerStream.Length; }
        }

        public override long Position
        {
            get { return InnerStream.Position; }
            set
            {
                InnerStream.Position = value;
                OnPositionChanged();
            }
        }

        public event EventHandler PositionChanged;

        protected virtual void OnPositionChanged()
        {
            if (PositionChanged != null)
            {
                PositionChanged(this, EventArgs.Empty);
            }
        }

        public override int ReadTimeout
        {
            get { return InnerStream.ReadTimeout; }
            set { InnerStream.ReadTimeout = value; }
        }

        public override int WriteTimeout
        {
            get { return InnerStream.WriteTimeout; }
            set { InnerStream.WriteTimeout = value; }
        }
    }
}
