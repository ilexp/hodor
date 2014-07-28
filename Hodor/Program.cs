using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Runtime.InteropServices;

using WindowsInput;

using Hodor.Native;

namespace Hodor
{
	public class Program
	{
		private const int WM_KEYDOWN = 0x100;
		private const int WM_KEYUP = 0x101;
		private const int WM_SYSKEYDOWN = 0x104;
		private const int WM_SYSKEYUP = 0x105;

		private static IntPtr hook = IntPtr.Zero;
		private static keyboardHookProc hookFunc = HookCallback;
		private	static int sentFromHodor = 0;
		private static VirtualKeyCode[] hodorKeys = new VirtualKeyCode[]
			{
				VirtualKeyCode.VK_H,
				VirtualKeyCode.VK_O,
				VirtualKeyCode.VK_D,
				VirtualKeyCode.VK_O,
				VirtualKeyCode.VK_R,
				VirtualKeyCode.SPACE
			};
		private	static int hodorIndex = 0;
		private static DateTime lastHodor = DateTime.Now;

		public static void Main(string[] args)
		{
			Application.ApplicationExit += new EventHandler(Application_ApplicationExit);
			Application.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);

			IntPtr hInstance = NativeMethods.LoadLibrary("User32");
			hook = NativeMethods.SetWindowsHookEx(HookType.WH_KEYBOARD_LL, hookFunc, hInstance, 0);
			Application.Run();
		}

		private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
		{
			if (hook != IntPtr.Zero)
			{
				NativeMethods.UnhookWindowsHookEx(hook);
			}
			Application.Exit();
		}
		private static void Application_ApplicationExit(object sender, EventArgs e)
		{
			if (hook != IntPtr.Zero)
			{
				NativeMethods.UnhookWindowsHookEx(hook);
			}
		}

		private static int HookCallback(int code, int wParam, ref keyboardHookStruct lParam)
		{
			if (code >= 0)
			{
				Keys key = (Keys)lParam.vkCode;

				double timeSinceLast = (DateTime.Now - lastHodor).TotalSeconds;
				bool hodorize = false;
				switch (key)
				{
					case Keys.A:
					case Keys.B:
					case Keys.C:
					case Keys.D:
					case Keys.E:
					case Keys.F:
					case Keys.G:
					case Keys.H:
					case Keys.I:
					case Keys.J:
					case Keys.K:
					case Keys.L:
					case Keys.M:
					case Keys.N:
					case Keys.O:
					case Keys.P:
					case Keys.Q:
					case Keys.R:
					case Keys.S:
					case Keys.T:
					case Keys.U:
					case Keys.V:
					case Keys.W:
					case Keys.X:
					case Keys.Y:
					case Keys.Z:
					case Keys.Oem1:
					case Keys.Oem2:
					case Keys.Oem3:
					case Keys.Oem4:
					case Keys.Oem5:
					case Keys.Oem6:
					case Keys.Oem7:
					case Keys.Oem8:
					case Keys.Add:
					case Keys.Subtract:
					case Keys.Multiply:
					case Keys.Divide:
					case Keys.Decimal:
					case Keys.NumPad0:
					case Keys.NumPad1:
					case Keys.NumPad2:
					case Keys.NumPad3:
					case Keys.NumPad4:
					case Keys.NumPad5:
					case Keys.NumPad6:
					case Keys.NumPad7:
					case Keys.NumPad8:
					case Keys.NumPad9:
					case Keys.D0:
					case Keys.D1:
					case Keys.D2:
					case Keys.D3:
					case Keys.D4:
					case Keys.D5:
					case Keys.D6:
					case Keys.D7:
					case Keys.D8:
					case Keys.D9:
						hodorize = true;
						break;
				}

				if (key == Keys.Space && timeSinceLast < 2.0d)
					hodorize = true;

				if (key == Keys.H && Control.ModifierKeys.HasFlag(Keys.Alt))
				{
					Application.Exit();
					hodorize = false;
				}
				if (Control.ModifierKeys.HasFlag(Keys.Control) || Control.ModifierKeys.HasFlag(Keys.Alt))
				{
					hodorize = false;
				}
				if (key == Keys.D1 && Control.ModifierKeys.HasFlag(Keys.Shift))
				{
					hodorize = false;
					hodorIndex = hodorKeys.Length - 1;
				}
				if (key == Keys.OemOpenBrackets && Control.ModifierKeys.HasFlag(Keys.Shift))
				{
					hodorize = false;
					hodorIndex = hodorKeys.Length - 1;
				}
				if (key == Keys.Oemcomma || key == Keys.OemPeriod)
				{
					hodorize = false;
					hodorIndex = hodorKeys.Length - 1;
				}

				if (key == Keys.Return)
					hodorIndex = 0;
				if (key == Keys.Back)
					hodorIndex = (hodorIndex - 1 + hodorKeys.Length) % hodorKeys.Length;

				if (sentFromHodor > 0 || !hodorize)
				{
					return NativeMethods.CallNextHookEx(IntPtr.Zero, code, wParam, ref lParam);         
				}
				else if (code >= 0 && wParam == WM_KEYDOWN)
				{
					sentFromHodor++;

					if (timeSinceLast >= 2.0d)
					{
						hodorIndex = 0;
					}
					lastHodor = DateTime.Now;

					InputSimulator.SimulateKeyPress(hodorKeys[hodorIndex % hodorKeys.Length]);
					hodorIndex++;

					sentFromHodor--;
				}
			}
			return 1;         
		}
	}
}
