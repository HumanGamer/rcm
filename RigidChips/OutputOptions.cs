using System.Globalization;
using System.IO;
using System.Windows.Forms;

namespace RigidChips {
	public class OutputOptions{
		public bool ReturnEndChipBracket = false;
		public bool IndentEnable = true;
		public bool IndentBySpace = false;
		public bool OpenBracketWithChipDefinition = true;
		public bool CommaWithSpace = false;
		public uint IndentNum = 2;
		public bool PrintAllAttributes = false;

		public OutputOptions() : this(false){}

		public OutputOptions(bool defaultSetting) {
			if (!defaultSetting) {
				try {
					System.IO.StreamReader file = new StreamReader(Application.StartupPath + "\\output.cfg");

					ReturnEndChipBracket = bool.Parse(file.ReadLine());
					IndentEnable = bool.Parse(file.ReadLine());
					IndentBySpace = bool.Parse(file.ReadLine());
					OpenBracketWithChipDefinition = bool.Parse(file.ReadLine());
					CommaWithSpace = bool.Parse(file.ReadLine());
					PrintAllAttributes = bool.Parse(file.ReadLine());
					IndentNum = uint.Parse(file.ReadLine(), System.Globalization.NumberStyles.Integer, CultureInfo.InvariantCulture);

					file.Close();
				}
				catch { }
			}
		}

		~OutputOptions(){
			System.IO.StreamWriter file = new StreamWriter(Application.StartupPath + "\\output.cfg");

			file.WriteLine(ReturnEndChipBracket.ToString());
			file.WriteLine(IndentEnable.ToString());
			file.WriteLine(IndentBySpace.ToString());
			file.WriteLine(OpenBracketWithChipDefinition.ToString());
			file.WriteLine(CommaWithSpace.ToString());
			file.WriteLine(PrintAllAttributes.ToString());
			file.WriteLine(IndentNum.ToString());

			file.Close();
		}
	}
}
