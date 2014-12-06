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
    class MouseInput
    {
        /// <summary>
        /// Moves mouse cursor by dx and dy values
        /// </summary>
        /// <param name="dx"># of pixels on x-axis</param>
        /// <param name="dy"># of pixels on y-axis</param>
        public static void MoveMouse(int dx, int dy)
        {
            INPUT input = new INPUT();
            MOUSEINPUT mi = new MOUSEINPUT();
            input.dwType = InputType.Mouse;
            input.mi = mi;
            input.mi.dwExtraInfo = IntPtr.Zero;
            // mouse co-ords: top left is (0,0), bottom right is (65535, 65535)
            // convert screen co-ord to mouse co-ords...
            input.mi.dx = dx;
            input.mi.dy = dy;
            input.mi.time = 0;
            input.mi.mouseData = 0;
            // can be used for WHEEL event see msdn
            input.mi.dwFlags = MOUSEEVENTF.MOVE;
            int cbSize = Marshal.SizeOf(typeof(INPUT));
            int result = SendInput(1, ref input, cbSize);
            if (result == 0)
                Console.WriteLine("DoMouse Error:" + Marshal.GetLastWin32Error());
        }

        public static void LeftClick()
        {
            INPUT input = new INPUT();
            MOUSEINPUT mi = new MOUSEINPUT();
            input.dwType = InputType.Mouse;
            input.mi = mi;
            input.mi.dwExtraInfo = IntPtr.Zero;
            // mouse co-ords: top left is (0,0), bottom right is (65535, 65535)
            // convert screen co-ord to mouse co-ords...
            input.mi.dx = 0;
            input.mi.dy = 0;
            input.mi.time = 0;
            input.mi.mouseData = 0;
            // can be used for WHEEL event see msdn
            input.mi.dwFlags = MOUSEEVENTF.LEFTDOWN | MOUSEEVENTF.LEFTUP;
            int cbSize = Marshal.SizeOf(typeof(INPUT));
            int result = SendInput(1, ref input, cbSize);
            if (result == 0)
                Console.WriteLine("DoMouse Error:" + Marshal.GetLastWin32Error());
        }

        public static void RightClick()
        {
            INPUT input = new INPUT();
            MOUSEINPUT mi = new MOUSEINPUT();
            input.dwType = InputType.Mouse;
            input.mi = mi;
            input.mi.dwExtraInfo = IntPtr.Zero;
            // mouse co-ords: top left is (0,0), bottom right is (65535, 65535)
            // convert screen co-ord to mouse co-ords...
            input.mi.dx = 0;
            input.mi.dy = 0;
            input.mi.time = 0;
            input.mi.mouseData = 0;
            // can be used for WHEEL event see msdn
            input.mi.dwFlags = MOUSEEVENTF.RIGHTDOWN | MOUSEEVENTF.RIGHTUP;
            int cbSize = Marshal.SizeOf(typeof(INPUT));
            int result = SendInput(1, ref input, cbSize);
            if (result == 0)
                Console.WriteLine("DoMouse Error:" + Marshal.GetLastWin32Error());
        }

        /*
         * Native Methods 
        */

        [DllImport("user32.dll", SetLastError = true)]
        static internal extern Int32 SendInput(Int32 cInputs, ref INPUT pInputs, Int32 cbSize);

        [DllImport("user32.dll")]
        public static extern bool GetAsyncKeyState(Int32 vKey);

        [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 28)]
        internal struct INPUT
        {
            [FieldOffset(0)]
            public InputType dwType;
            [FieldOffset(4)]
            public MOUSEINPUT mi;
            [FieldOffset(4)]
            public KEYBDINPUT ki;
            [FieldOffset(4)]
            public HARDWAREINPUT hi;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct MOUSEINPUT
        {
            public Int32 dx;
            public Int32 dy;
            public Int32 mouseData;
            public MOUSEEVENTF dwFlags;
            public Int32 time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct KEYBDINPUT
        {
            public Int16 wVk;
            public Int16 wScan;
            public KEYEVENTF dwFlags;
            public Int32 time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct HARDWAREINPUT
        {
            public Int32 uMsg;
            public Int16 wParamL;
            public Int16 wParamH;
        }

        internal enum InputType : int
        {
            Mouse = 0,
            Keyboard = 1,
            Hardware = 2
        }

        [Flags()]
        internal enum MOUSEEVENTF : int
        {
            MOVE = 0x1,
            LEFTDOWN = 0x2,
            LEFTUP = 0x4,
            RIGHTDOWN = 0x8,
            RIGHTUP = 0x10,
            MIDDLEDOWN = 0x20,
            MIDDLEUP = 0x40,
            XDOWN = 0x80,
            XUP = 0x100,
            VIRTUALDESK = 0x400,
            WHEEL = 0x800,
            ABSOLUTE = 0x8000
        }

        [Flags()]
        internal enum KEYEVENTF : int
        {
            EXTENDEDKEY = 1,
            KEYUP = 2,
            UNICODE = 4,
            SCANCODE = 8
        }
    }
}
