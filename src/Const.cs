using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KineCTRL
{
    public static class Const
    {
        // Recognizer modes / gesture types
        public const string ALL = "ALL";
        public const string BODY = "BODY";
        public const string LEFT_HAND = "LEFT_HAND";
        public const string RIGHT_HAND = "RIGHT_HAND";
        public const string POSTURE = "POSTURE";
        public const string POSTURE_DEFAULT = "POSTURE_DEFAULT";
        public const string OFF = "OFF";


        // Renderers
        public const string RENDERER_MAIN = "RENDERER_MAIN";
        public const string RENDERER_RECORD = "RENDERER_RECORD";
        public const string RENDERER_REPLAY = "RENDERER_REPLAY";

        // GUI Image URIs
        public const string URI_IMG_SENSOR_OFF = @"Images/sensor_off.png";
        public const string URI_IMG_SENSOR_ON = @"Images/sensor_on.png";
        public const string URI_IMG_SENSOR_ERROR = @"Images/sensor_error.png";
        public const string URI_IMG_RECOGNITION_OFF = @"Images/recognition_off.png";
        public const string URI_IMG_RECOGNITION_ON = @"Images/recognition_on.png";
        public const string URI_IMG_SPEECH_OFF = @"Images/speech_off.png";
        public const string URI_IMG_SPEECH_ON = @"Images/speech_on.png";
        public const string URI_IMG_HANDMOUSE_OFF = @"Images/handmouse_off.png";
        public const string URI_IMG_HANDMOUSE_ON = @"Images/handmouse_on.png";
        public const string URI_IMG_HANDMOUSE_LEFT_ON = @"Images/handmouse_left_on.png";
        public const string URI_IMG_HANDMOUSE_LEFT_OFF = @"Images/handmouse_left_off.png";
        public const string URI_IMG_HANDMOUSE_RIGHT_ON = @"Images/handmouse_right_on.png";
        public const string URI_IMG_HANDMOUSE_RIGHT_OFF = @"Images/handmouse_right_off.png";

        // Default settings values
        public const double DEFAULT_BODY_RECOGNIZER_DETECTION = 3;
        public const double DEFAULT_BODY_RECOGNIZER_CONFIDENCE = 0.5;
        public const double DEFAULT_HAND_RECOGNIZER_DETECTION = 1.3;
        public const double DEFAULT_HAND_RECOGNIZER_CONFIDENCE = 1;
        public const double DEFAULT_POSTURE_RECOGNIZER_DEFINITION = 0.2;
        public const double DEFAULT_POSTURE_RECOGNIZER_CONFIDENCE = 0.2;
        public const double DEFAULT_RECORDER_DETECTION = 1.3;
        public const double DEFAULT_RECORDER_READY = 6;
        public const double DEFAULT_SPEECH_RECOGNIZER_CONFIDENCE = 0.3;
        public const double DEFAULT_HANDMOUSE_SENSITIVITY = 1.3;
    }
}
