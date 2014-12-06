using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KineCTRL
{
    public static class Strings
    {
        // Errors
        public const string ERR_CAPTION = "KineCTRL Error";
        public const string ERR_PROFILE_NOT_FOUND = "Oops! Profile not found!";
        public const string ERR_CANT_LOAD_PROFILE = "Somenthing went terribly wrong! Cannot load profile.";
        public const string ERR_CANNOT_DELETE_PROFILE = "Cannot delete currently loaded profile.";
        public const string ERR_PROFILE_NAME_EXISTS = "Selected profile name already exists.";

        // Tooltips
        public const string TOOLTIP_SENSOR_ON = "Sensor Active";
        public const string TOOLTIP_SENSOR_OFF = "Waiting for sensor...";
        public const string TOOLTIP_SENSOR_ERROR = "Sensor Error";
        public const string TOOLTIP_RECOGNITION_ON = "Gesture Recognition Active";
        public const string TOOLTIP_RECOGNITION_OFF = "Gesture Recognition Off";
        public const string TOOLTIP_SPEECH_ON = "Speech Recognition Active";
        public const string TOOLTIP_SPEECH_OFF = "Speech Recognition Off";
        public const string TOOLTIP_HANDMOUSE_ON = "HandMouse Recognition Active";
        public const string TOOLTIP_HANDMOUSE_OFF = "HandMouse Recognition Off";
    }
}
