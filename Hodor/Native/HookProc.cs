using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hodor.Native
{
	public delegate int keyboardHookProc(int code, int wParam, ref keyboardHookStruct lParam);
	
	public struct keyboardHookStruct {
		public int vkCode;
		public int scanCode;
		public int flags;
		public int time;
		public int dwExtraInfo;
	}
}
