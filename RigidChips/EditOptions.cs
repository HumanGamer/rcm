using System.Globalization;
using System.IO;
using System.Windows.Forms;

namespace RigidChips {
	public class EditOptions{
		public bool UnvisibleNotSelected = true;	//	非表示のゴーストやカウルはマウスで選択されなくする
		public bool ConvertParentAttributes = false;	//	新規接続時、親チップの属性をコピーする
		public int ScrollFrameNum = 20;
		public int AngleViewGrid = 1;
		public bool AttributeAutoApply = true;
		public bool InvertWheel = false;
		public bool InvertRotateX = false;
		public bool InvertRotateY = false;
		public bool KeepParentColor = false;

		public EditOptions(){
			try{
				System.IO.StreamReader file = new StreamReader(Application.StartupPath + "\\edit.cfg");

				UnvisibleNotSelected = bool.Parse(file.ReadLine());
				ConvertParentAttributes = bool.Parse(file.ReadLine());
				ScrollFrameNum = int.Parse(file.ReadLine(), CultureInfo.InvariantCulture);
				AngleViewGrid = int.Parse(file.ReadLine(), CultureInfo.InvariantCulture);
				AttributeAutoApply = bool.Parse(file.ReadLine());
				InvertWheel = bool.Parse(file.ReadLine());
				InvertRotateX = bool.Parse(file.ReadLine());
				InvertRotateY = bool.Parse(file.ReadLine());
				KeepParentColor = bool.Parse(file.ReadLine());
			}
			catch{}
		}

		~EditOptions(){
			System.IO.StreamWriter file = new StreamWriter(Application.StartupPath + "\\edit.cfg");

			file.WriteLine(UnvisibleNotSelected.ToString());
			file.WriteLine(ConvertParentAttributes.ToString());
			file.WriteLine(ScrollFrameNum.ToString());
			file.WriteLine(AngleViewGrid.ToString());
			file.WriteLine(AttributeAutoApply.ToString());
			file.WriteLine(InvertWheel.ToString());
			file.WriteLine(InvertRotateX.ToString());
			file.WriteLine(InvertRotateY.ToString());
			file.WriteLine(KeepParentColor.ToString());

			file.Close();
		}
	}
}
