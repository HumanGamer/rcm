using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace rcm
{
    // https://stackoverflow.com/a/23617211
    public static class KeyboardLayout
    {
        const int KL_NAMELENGTH = 9;

        [DllImport("user32.dll")]
		private static extern long GetKeyboardLayoutName(
		  System.Text.StringBuilder pwszKLID);

		public static bool ShouldFlipZY()
		{
            StringBuilder name = new StringBuilder(KL_NAMELENGTH);
            GetKeyboardLayoutName(name);
            String KeyBoardLayout = name.ToString();
            if (KeyBoardLayout == "00000407" || KeyBoardLayout == "00000807")
            {
                //MessageBox.Show("Using QWERTZ");
                return true;
            }
            else if (KeyBoardLayout == "0000040c" || KeyBoardLayout == "0000080c")
            {
                //MessageBox.Show("Using AZERTY");
                return false;
            }
            else if (KeyBoardLayout == "00010409")
            {
                //MessageBox.Show("Using Dvorak");
                return false;
            }
            else
            {
                //MessageBox.Show("Using QWERTY");
                return false;
            }
        }
	}
}
