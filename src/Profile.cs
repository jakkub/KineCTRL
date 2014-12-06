using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KineCTRL
{
    public class Profile
    {
        /// <summary>
        /// Dictionary of all gesture lists
        /// </summary>
        private Dictionary<string, List<Gesture>> gestures = new Dictionary<string, List<Gesture>>();
        public Dictionary<string, List<Gesture>> Gestures
        {
            get { return gestures; }
            set { gestures = value; }
        }

        /// <summary>
        /// Settings for this profile
        /// </summary>
        private Dictionary<string, string> settings = new Dictionary<string, string>();
        public Dictionary<string, string> Settings
        {
            get { return settings; }
            set { settings = value; }
        }

        /// <summary>
        /// Is this profile saved?
        /// </summary>
        private bool isSaved = true;
        public bool IsSaved
        {
            get { return isSaved; }
            set { isSaved = value; }
        }

        public Profile()
        {
        }

        /// <summary>
        /// Clears all profile data
        /// </summary>
        public void Clear()
        {
            gestures.Clear();
            settings.Clear();
        }
    }
}
