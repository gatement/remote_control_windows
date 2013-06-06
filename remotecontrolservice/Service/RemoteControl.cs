using System;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Net;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Windows.Forms;

namespace remotecontrolservice
{
    public class RemoteControl
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, uint wParam, uint lParam);

        [DllImport("user32.dll")]
        static public extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll")]
        static extern void mouse_event(MouseEventFlag flags, int dx, int dy, int data, UIntPtr extraInfo);

        [Flags]
        enum MouseEventFlag : int
        {
            Move = 0x0001,
            LeftDown = 0x0002,
            LeftUp = 0x0004,
            RightDown = 0x0008,
            RightUp = 0x0010,
            MiddleDown = 0x0020,
            MiddleUp = 0x0040,
            XDown = 0x0080,
            XUp = 0x0100,
            Wheel = 0x0800,
            VirtualDesk = 0x4000,
            Absolute = 0x8000
        }

        const uint WM_APPCOMMAND = 0x319;
        const uint APPCOMMAND_VOLUME_UP = 0x0a;
        const uint APPCOMMAND_VOLUME_DOWN = 0x09;
        const uint APPCOMMAND_VOLUME_MUTE = 0x08;

        public static string SendCmd(Command cmd)
        {
            string msg = "";

            switch (cmd.Name)
            {
                case "volumeUp":
                    SendMessage(GetForegroundWindow(), WM_APPCOMMAND, 0x30292, APPCOMMAND_VOLUME_UP * 0x10000);
                    msg = "succeeded.";
                    break;
                case "volumeDown":
                    SendMessage(GetForegroundWindow(), WM_APPCOMMAND, 0x30292, APPCOMMAND_VOLUME_DOWN * 0x10000);
                    msg = "succeeded.";
                    break;
                case "volumeMute":
                    SendMessage(GetForegroundWindow(), WM_APPCOMMAND, 0x200eb0, APPCOMMAND_VOLUME_MUTE * 0x10000);
                    msg = "succeeded.";
                    break;
                case "mouseMove":
                    string[] values = cmd.Value.Split(',');
                    int x = int.Parse(values[0]);
                    int y = int.Parse(values[1]);
                    mouse_event(MouseEventFlag.Move, x, y, 0, UIntPtr.Zero);
                    msg = "succeeded.";
                    break;
                case "mouseLeftClick":
                    mouse_event(MouseEventFlag.LeftDown | MouseEventFlag.LeftUp, 0, 0, 0, UIntPtr.Zero);
                    msg = "succeeded.";
                    break;
                case "mouseRightClick":
                    mouse_event(MouseEventFlag.RightDown | MouseEventFlag.RightUp, 0, 0, 0, UIntPtr.Zero);
                    msg = "succeeded.";
                    break;
                case "mouseWheel":
                    int value = int.Parse(cmd.Value);
                    mouse_event(MouseEventFlag.Wheel, 0, 0, value, UIntPtr.Zero);
                    msg = "succeeded.";
                    break;
                case "textInput":
                    SendKeys.SendWait(cmd.Value);
                    msg = "succeeded.";
                    break;
                case "shutdown":
                    Process.Start("shutdown", "/s /t 0");
                    msg = "succeeded.";
                    break;
                default:
                    msg = "command don't support.";
                    break;
            }

            return msg;
        }
    }
}
