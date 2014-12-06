using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.DirectInput;
using System.Runtime.InteropServices;
using System.Windows.Input;

namespace KineCTRL
{
    public class KeyInput
    {
        /// <summary>
        /// Is this Key currently pressed down?
        /// </summary>
        private Boolean isDown;
        public Boolean IsDown
        {
            get { return isDown; }
            set { isDown = value; }
        }

        public System.Windows.Input.Key PressedKey;
        public bool LWinDown;
        public bool RWinDown;
        public bool LCtrlDown;
        public bool RCtrlDown;
        public bool LAltDown;
        public bool RAltDown;
        public bool LShiftDown;
        public bool RShiftDown;

        public KeyInput()
        {
            isDown = false;
        }

        public KeyInput(System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key != System.Windows.Input.Key.LeftShift &
                e.Key != System.Windows.Input.Key.RightShift &
                e.Key != System.Windows.Input.Key.LeftCtrl &
                e.Key != System.Windows.Input.Key.RightCtrl &
                e.Key != System.Windows.Input.Key.LeftAlt &
                e.Key != System.Windows.Input.Key.RightAlt &
                e.Key != System.Windows.Input.Key.LWin & 
                e.Key != System.Windows.Input.Key.RWin)
            {
                LWinDown = e.KeyboardDevice.IsKeyDown(System.Windows.Input.Key.LWin);
                RWinDown = e.KeyboardDevice.IsKeyDown(System.Windows.Input.Key.RWin);
                LCtrlDown = e.KeyboardDevice.IsKeyDown(System.Windows.Input.Key.LeftCtrl);
                RCtrlDown = e.KeyboardDevice.IsKeyDown(System.Windows.Input.Key.RightCtrl);
                LAltDown = e.KeyboardDevice.IsKeyDown(System.Windows.Input.Key.LeftAlt);
                RAltDown = e.KeyboardDevice.IsKeyDown(System.Windows.Input.Key.RightAlt);
                LShiftDown = e.KeyboardDevice.IsKeyDown(System.Windows.Input.Key.LeftShift);
                RShiftDown = e.KeyboardDevice.IsKeyDown(System.Windows.Input.Key.RightShift);
            }
            
            PressedKey = e.Key;
            
        }


        /// <summary>
        /// Execute key press event on this key
        /// </summary>
        public void ExecuteKeyPress()
        {
            SpecialKeysDown();
            SendKeyPress(ConvertKeyCode(PressedKey));
            SpecialKeysUp();
        }

        /// <summary>
        /// Execute key down event on this key
        /// </summary>
        public void ExecuteKeyDown()
        {
            isDown = true;
            SpecialKeysDown();
            SendKeyDown(ConvertKeyCode(PressedKey));
        }

        /// <summary>
        /// Execute key up event on this key
        /// </summary>
        public void ExecuteKeyUp()
        {
            isDown = false;
            SendKeyUp(ConvertKeyCode(PressedKey));
            SpecialKeysUp();
        }

        /// <summary>
        /// Push down special keys if set for this keystroke
        /// </summary>
        private void SpecialKeysDown()
        {
            if (LWinDown)
                SendKeyDown(Microsoft.DirectX.DirectInput.Key.LeftWindows);
            if (RWinDown)
                SendKeyDown(Microsoft.DirectX.DirectInput.Key.RightWindows);
            if (LCtrlDown)
                SendKeyDown(Microsoft.DirectX.DirectInput.Key.LeftControl);
            if (RCtrlDown)
                SendKeyDown(Microsoft.DirectX.DirectInput.Key.RightControl);
            if (LAltDown)
                SendKeyDown(Microsoft.DirectX.DirectInput.Key.LeftAlt);
            if (RAltDown)
                SendKeyDown(Microsoft.DirectX.DirectInput.Key.RightAlt);
            if (LShiftDown)
                SendKeyDown(Microsoft.DirectX.DirectInput.Key.LeftShift);
            if (RShiftDown)
                SendKeyDown(Microsoft.DirectX.DirectInput.Key.RightShift);
        }

        /// <summary>
        /// Pull up special keys if set for this keystroke
        /// </summary>
        private void SpecialKeysUp()
        {
            if (LWinDown)
                SendKeyUp(Microsoft.DirectX.DirectInput.Key.LeftWindows);
            if (RWinDown)
                SendKeyUp(Microsoft.DirectX.DirectInput.Key.RightWindows);
            if (LCtrlDown)
                SendKeyUp(Microsoft.DirectX.DirectInput.Key.LeftControl);
            if (RCtrlDown)
                SendKeyUp(Microsoft.DirectX.DirectInput.Key.RightControl);
            if (LAltDown)
                SendKeyUp(Microsoft.DirectX.DirectInput.Key.LeftAlt);
            if (RAltDown)
                SendKeyUp(Microsoft.DirectX.DirectInput.Key.RightAlt);
            if (LShiftDown)
                SendKeyUp(Microsoft.DirectX.DirectInput.Key.LeftShift);
            if (RShiftDown)
                SendKeyUp(Microsoft.DirectX.DirectInput.Key.RightShift);
        }

        struct INPUT
        {
            public UInt32 type;
            public ushort wVk;
            public ushort wScan;
            public UInt32 dwFlags;
            public UInt32 time;
            public UIntPtr dwExtraInfo;
            public UInt32 uMsg;
            public ushort wParamL;
            public ushort wParamH;
        }

        enum SendInputFlags
        {
            KEYEVENTF_EXTENDEDKEY = 0x0001,
            KEYEVENTF_KEYUP = 0x0002,
            KEYEVENTF_UNICODE = 0x0004,
            KEYEVENTF_SCANCODE = 0x0008,
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint SendInput(uint numberOfInputs, INPUT[] inputs, int sizeOfInputStructure);

        /// <summary>
        /// Simulate key press
        /// </summary>
        /// <param name="K">Key ScanCode</param>
        public static void SendKeyPress(Microsoft.DirectX.DirectInput.Key K)
        {
            INPUT[] InputData = new INPUT[2];

            InputData[0].type = 1;
            InputData[0].wScan = (ushort)K;
            InputData[0].dwFlags = (uint)SendInputFlags.KEYEVENTF_SCANCODE;
            InputData[0].wVk = 0;

            InputData[1].type = 1;
            InputData[1].wScan = (ushort)K;
            InputData[1].dwFlags = (uint)(SendInputFlags.KEYEVENTF_KEYUP | SendInputFlags.KEYEVENTF_SCANCODE);
            InputData[1].wVk = 0;
            SendInput(2, InputData, Marshal.SizeOf(InputData[0]));
        }

        /// <summary>
        /// Send a key down and hold it down until sendkeyup method is called
        /// </summary>
        /// <param name="K">Key ScanCode</param>
        public static void SendKeyDown(Microsoft.DirectX.DirectInput.Key K)
        {
            INPUT[] InputData = new INPUT[1];
            InputData[0].type = 1;
            InputData[0].wScan = (ushort)K;
            InputData[0].dwFlags = (uint)(SendInputFlags.KEYEVENTF_SCANCODE);
            if (IsExtendedKey(K))
                InputData[0].dwFlags |= (uint)(SendInputFlags.KEYEVENTF_EXTENDEDKEY);
            InputData[0].wVk = 0;
            SendInput(1, InputData, Marshal.SizeOf(InputData[0]));
        }

        /// <summary>
        /// Release a key that is being held down
        /// </summary>
        /// <param name="K">Key ScanCode</param>
        public static void SendKeyUp(Microsoft.DirectX.DirectInput.Key K)
        {
            
            INPUT[] InputData = new INPUT[1];
            InputData[0].type = 1;
            InputData[0].wScan = (ushort)K;
            InputData[0].dwFlags = (uint)(SendInputFlags.KEYEVENTF_KEYUP | SendInputFlags.KEYEVENTF_SCANCODE);
            if (IsExtendedKey(K))
                InputData[0].dwFlags |= (uint)(SendInputFlags.KEYEVENTF_EXTENDEDKEY);
            InputData[0].wVk = 0;
            SendInput(1, InputData, Marshal.SizeOf(InputData[0]));
        }


        /// <summary>
        /// Convert this KeyStroke to string representation
        /// </summary>
        public string ConvertToString()
        {
            string output;

            output =
              ((LWinDown | RWinDown) ? "Win + " : "") +
              ((LCtrlDown | RCtrlDown) ? "Ctrl + " : "") +
              ((LAltDown | RAltDown) ? "Alt + " : "") +
              ((LShiftDown | RShiftDown) ? "Shift + " : "") +
              PressedKey.ToString();

            return output;
        }


        /// <summary>
        /// Checks if key is extended key
        /// </summary>
        /// <param name="K">Key ScanCode</param>
        /// <returns>true/false</returns>
        private static bool IsExtendedKey(Microsoft.DirectX.DirectInput.Key K)
        {
            switch (K)
            {
                case Microsoft.DirectX.DirectInput.Key.Up:
                    return true;
                case Microsoft.DirectX.DirectInput.Key.Down:
                    return true;
                case Microsoft.DirectX.DirectInput.Key.Left:
                    return true;
                case Microsoft.DirectX.DirectInput.Key.Right:
                    return true;
                case Microsoft.DirectX.DirectInput.Key.Home:
                    return true;
                case Microsoft.DirectX.DirectInput.Key.End:
                    return true;
                case Microsoft.DirectX.DirectInput.Key.PageDown:
                    return true;
                case Microsoft.DirectX.DirectInput.Key.PageUp:
                    return true;
                case Microsoft.DirectX.DirectInput.Key.Insert:
                    return true;
                case Microsoft.DirectX.DirectInput.Key.Delete:
                    return true;
                case Microsoft.DirectX.DirectInput.Key.RightAlt:
                    return true;
                case Microsoft.DirectX.DirectInput.Key.RightControl:
                    return true;
                case Microsoft.DirectX.DirectInput.Key.Numlock:
                    return true;
                case Microsoft.DirectX.DirectInput.Key.SysRq:
                    return true;
                default:
                    return false;
            }
        }


        /// <summary>
        /// Converts System.Windows.Input.Key to DirectInput.Key
        /// </summary>
        /// <param name="K">Key ScanCode</param>
        /// <returns>DirectInput.Key</returns>
        private Microsoft.DirectX.DirectInput.Key ConvertKeyCode(System.Windows.Input.Key K)
        {
            switch (K)
            {
                case System.Windows.Input.Key.A:
                    return Microsoft.DirectX.DirectInput.Key.A;
                    
                case System.Windows.Input.Key.B:
                    return Microsoft.DirectX.DirectInput.Key.B;
                    
                case System.Windows.Input.Key.C:
                    return Microsoft.DirectX.DirectInput.Key.C;
                    
                case System.Windows.Input.Key.D:
                    return Microsoft.DirectX.DirectInput.Key.D;
                    
                case System.Windows.Input.Key.E:
                    return Microsoft.DirectX.DirectInput.Key.E;
                    
                case System.Windows.Input.Key.F:
                    return Microsoft.DirectX.DirectInput.Key.F;
                    
                case System.Windows.Input.Key.G:
                    return Microsoft.DirectX.DirectInput.Key.G;
                    
                case System.Windows.Input.Key.H:
                    return Microsoft.DirectX.DirectInput.Key.H;
                    
                case System.Windows.Input.Key.I:
                    return Microsoft.DirectX.DirectInput.Key.I;
                    
                case System.Windows.Input.Key.J:
                    return Microsoft.DirectX.DirectInput.Key.J;
                    
                case System.Windows.Input.Key.K:
                    return Microsoft.DirectX.DirectInput.Key.K;
                    
                case System.Windows.Input.Key.L:
                    return Microsoft.DirectX.DirectInput.Key.L;
                    
                case System.Windows.Input.Key.M:
                    return Microsoft.DirectX.DirectInput.Key.M;
                    
                case System.Windows.Input.Key.N:
                    return Microsoft.DirectX.DirectInput.Key.N;
                    
                case System.Windows.Input.Key.O:
                    return Microsoft.DirectX.DirectInput.Key.O;
                    
                case System.Windows.Input.Key.P:
                    return Microsoft.DirectX.DirectInput.Key.P;
                    
                case System.Windows.Input.Key.Q:
                    return Microsoft.DirectX.DirectInput.Key.Q;
                    
                case System.Windows.Input.Key.R:
                    return Microsoft.DirectX.DirectInput.Key.R;
                    
                case System.Windows.Input.Key.S:
                    return Microsoft.DirectX.DirectInput.Key.S;
                    
                case System.Windows.Input.Key.T:
                    return Microsoft.DirectX.DirectInput.Key.T;
                    
                case System.Windows.Input.Key.U:
                    return Microsoft.DirectX.DirectInput.Key.U;
                    
                case System.Windows.Input.Key.V:
                    return Microsoft.DirectX.DirectInput.Key.V;
                    
                case System.Windows.Input.Key.W:
                    return Microsoft.DirectX.DirectInput.Key.W;
                    
                case System.Windows.Input.Key.X:
                    return Microsoft.DirectX.DirectInput.Key.X;
                    
                case System.Windows.Input.Key.Y:
                    return Microsoft.DirectX.DirectInput.Key.Y;
                    
                case System.Windows.Input.Key.Z:
                    return Microsoft.DirectX.DirectInput.Key.Z;
                    
                case System.Windows.Input.Key.Up:
                    return Microsoft.DirectX.DirectInput.Key.Up;
                    
                case System.Windows.Input.Key.Down:
                    return Microsoft.DirectX.DirectInput.Key.Down;
                    
                case System.Windows.Input.Key.Left:
                    return Microsoft.DirectX.DirectInput.Key.Left;
                    
                case System.Windows.Input.Key.Right:
                    return Microsoft.DirectX.DirectInput.Key.Right;
                    
                case System.Windows.Input.Key.D0:
                    return Microsoft.DirectX.DirectInput.Key.D0;
                    
                case System.Windows.Input.Key.D1:
                    return Microsoft.DirectX.DirectInput.Key.D1;
                    
                case System.Windows.Input.Key.D2:
                    return Microsoft.DirectX.DirectInput.Key.D2;
                    
                case System.Windows.Input.Key.D3:
                    return Microsoft.DirectX.DirectInput.Key.D3;
                    
                case System.Windows.Input.Key.D4:
                    return Microsoft.DirectX.DirectInput.Key.D4;
                    
                case System.Windows.Input.Key.D5:
                    return Microsoft.DirectX.DirectInput.Key.D5;
                    
                case System.Windows.Input.Key.D6:
                    return Microsoft.DirectX.DirectInput.Key.D6;
                    
                case System.Windows.Input.Key.D7:
                    return Microsoft.DirectX.DirectInput.Key.D7;
                    
                case System.Windows.Input.Key.D8:
                    return Microsoft.DirectX.DirectInput.Key.D8;
                    
                case System.Windows.Input.Key.D9:
                    return Microsoft.DirectX.DirectInput.Key.D9;
                    
                case System.Windows.Input.Key.NumPad0:
                    return Microsoft.DirectX.DirectInput.Key.NumPad0;
                    
                case System.Windows.Input.Key.NumPad1:
                    return Microsoft.DirectX.DirectInput.Key.NumPad1;
                    
                case System.Windows.Input.Key.NumPad2:
                    return Microsoft.DirectX.DirectInput.Key.NumPad2;
                    
                case System.Windows.Input.Key.NumPad3:
                    return Microsoft.DirectX.DirectInput.Key.NumPad3;
                    
                case System.Windows.Input.Key.NumPad4:
                    return Microsoft.DirectX.DirectInput.Key.NumPad4;
                    
                case System.Windows.Input.Key.NumPad5:
                    return Microsoft.DirectX.DirectInput.Key.NumPad5;
                    
                case System.Windows.Input.Key.NumPad6:
                    return Microsoft.DirectX.DirectInput.Key.NumPad6;
                    
                case System.Windows.Input.Key.NumPad7:
                    return Microsoft.DirectX.DirectInput.Key.NumPad7;
                    
                case System.Windows.Input.Key.NumPad8:
                    return Microsoft.DirectX.DirectInput.Key.NumPad8;
                    
                case System.Windows.Input.Key.NumPad9:
                    return Microsoft.DirectX.DirectInput.Key.NumPad9;
                    
                case System.Windows.Input.Key.LeftCtrl:
                    return Microsoft.DirectX.DirectInput.Key.LeftControl;
                    
                case System.Windows.Input.Key.RightCtrl:
                    return Microsoft.DirectX.DirectInput.Key.RightControl;
                    
                case System.Windows.Input.Key.LeftShift:
                    return Microsoft.DirectX.DirectInput.Key.LeftShift;
                    
                case System.Windows.Input.Key.RightShift:
                    return Microsoft.DirectX.DirectInput.Key.RightShift;
                    
                case System.Windows.Input.Key.LeftAlt:
                    return Microsoft.DirectX.DirectInput.Key.LeftAlt;
                    
                case System.Windows.Input.Key.RightAlt:
                    return Microsoft.DirectX.DirectInput.Key.RightAlt;
                    
                case System.Windows.Input.Key.RWin:
                    return Microsoft.DirectX.DirectInput.Key.RightWindows;
                    
                case System.Windows.Input.Key.LWin:
                    return Microsoft.DirectX.DirectInput.Key.LeftWindows;
                    
                case System.Windows.Input.Key.Return:
                    return Microsoft.DirectX.DirectInput.Key.Return;
                    
                case System.Windows.Input.Key.Home:
                    return Microsoft.DirectX.DirectInput.Key.Home;
                    
                case System.Windows.Input.Key.End:
                    return Microsoft.DirectX.DirectInput.Key.End;
                    
                case System.Windows.Input.Key.PageUp:
                    return Microsoft.DirectX.DirectInput.Key.PageUp;
                    
                case System.Windows.Input.Key.PageDown:
                    return Microsoft.DirectX.DirectInput.Key.PageDown;
                    
                case System.Windows.Input.Key.Escape:
                    return Microsoft.DirectX.DirectInput.Key.Escape;
                    
                case System.Windows.Input.Key.PrintScreen:
                    return Microsoft.DirectX.DirectInput.Key.SysRq;
                    
                case System.Windows.Input.Key.Insert:
                    return Microsoft.DirectX.DirectInput.Key.Insert;
                    
                case System.Windows.Input.Key.Delete:
                    return Microsoft.DirectX.DirectInput.Key.Delete;
                    
                case System.Windows.Input.Key.Multiply:
                    return Microsoft.DirectX.DirectInput.Key.Multiply;
                    
                case System.Windows.Input.Key.Add:
                    return Microsoft.DirectX.DirectInput.Key.Add;
                    
                case System.Windows.Input.Key.Subtract:
                    return Microsoft.DirectX.DirectInput.Key.Subtract;
                    
                case System.Windows.Input.Key.Divide:
                    return Microsoft.DirectX.DirectInput.Key.Divide;
                    
                case System.Windows.Input.Key.Separator:
                    return Microsoft.DirectX.DirectInput.Key.NumPadComma;
                    
                case System.Windows.Input.Key.NumLock:
                    return Microsoft.DirectX.DirectInput.Key.Numlock;
                    
                case System.Windows.Input.Key.Scroll:
                    return Microsoft.DirectX.DirectInput.Key.Scroll;
                    
                case System.Windows.Input.Key.CapsLock:
                    return Microsoft.DirectX.DirectInput.Key.CapsLock;
                    
                case System.Windows.Input.Key.Pause:
                    return Microsoft.DirectX.DirectInput.Key.Pause;
                    
                case System.Windows.Input.Key.Space:
                    return Microsoft.DirectX.DirectInput.Key.Space;
                    
                case System.Windows.Input.Key.Tab:
                    return Microsoft.DirectX.DirectInput.Key.Tab;
                    
                case System.Windows.Input.Key.Back:
                    return Microsoft.DirectX.DirectInput.Key.BackSpace;
                    
                case System.Windows.Input.Key.Oem1:
                    return Microsoft.DirectX.DirectInput.Key.Colon;
                    
                case System.Windows.Input.Key.Oem2:
                    return Microsoft.DirectX.DirectInput.Key.Slash;
                    
                case System.Windows.Input.Key.Oem3:
                    return Microsoft.DirectX.DirectInput.Key.Grave;
                    
                case System.Windows.Input.Key.Oem4:
                    return Microsoft.DirectX.DirectInput.Key.LeftBracket;
                    
                case System.Windows.Input.Key.Oem5:
                    return Microsoft.DirectX.DirectInput.Key.BackSlash;
                    
                case System.Windows.Input.Key.Oem6:
                    return Microsoft.DirectX.DirectInput.Key.RightBracket;
                    
                case System.Windows.Input.Key.Oem7:
                    return Microsoft.DirectX.DirectInput.Key.Apostrophe;
                    
                case System.Windows.Input.Key.Oem8:
                    return Microsoft.DirectX.DirectInput.Key.Slash;
                    
                case System.Windows.Input.Key.Oem102:
                    return Microsoft.DirectX.DirectInput.Key.OEM102;
                    
                case System.Windows.Input.Key.OemPeriod:
                    return Microsoft.DirectX.DirectInput.Key.Period;
                    
                case System.Windows.Input.Key.OemComma:
                    return Microsoft.DirectX.DirectInput.Key.Comma;
                    
                case System.Windows.Input.Key.OemPlus:
                    return Microsoft.DirectX.DirectInput.Key.Equals;
                    
                case System.Windows.Input.Key.OemMinus:
                    return Microsoft.DirectX.DirectInput.Key.Minus;
                    
                case System.Windows.Input.Key.F1:
                    return Microsoft.DirectX.DirectInput.Key.F1;
                    
                case System.Windows.Input.Key.F2:
                    return Microsoft.DirectX.DirectInput.Key.F2;
                    
                case System.Windows.Input.Key.F3:
                    return Microsoft.DirectX.DirectInput.Key.F3;
                    
                case System.Windows.Input.Key.F4:
                    return Microsoft.DirectX.DirectInput.Key.F4;
                    
                case System.Windows.Input.Key.F5:
                    return Microsoft.DirectX.DirectInput.Key.F5;
                    
                case System.Windows.Input.Key.F6:
                    return Microsoft.DirectX.DirectInput.Key.F6;
                    
                case System.Windows.Input.Key.F7:
                    return Microsoft.DirectX.DirectInput.Key.F7;
                    
                case System.Windows.Input.Key.F8:
                    return Microsoft.DirectX.DirectInput.Key.F8;
                    
                case System.Windows.Input.Key.F9:
                    return Microsoft.DirectX.DirectInput.Key.F9;
                    
                case System.Windows.Input.Key.F10:
                    return Microsoft.DirectX.DirectInput.Key.F10;
                    
                case System.Windows.Input.Key.F11:
                    return Microsoft.DirectX.DirectInput.Key.F11;
                    
                case System.Windows.Input.Key.F12:
                    return Microsoft.DirectX.DirectInput.Key.F12;
                
                default:
                    return Microsoft.DirectX.DirectInput.Key.A;
            }

        }
    }
}
