using System.Globalization;
using System.Windows.Forms;

namespace RigidChips {
	/// <summary>
	/// キー(RcKey)のリスト
	/// </summary>
	public class KeyEntryList{
		public KeyEntry[] list;

		public static readonly Keys[] KeyMap = {
			Keys.Up, Keys.Down, Keys.Left, Keys.Right, 
			Keys.Z, Keys.X, Keys.C,
			Keys.A, Keys.S, Keys.D,
			Keys.V, Keys.B,
			Keys.F, Keys.G,
			Keys.Q, Keys.W, Keys.E
		};

		public KeyEntryList(int KeyNum){
			list = new KeyEntry[KeyNum];
			for(int i = 0;i < KeyNum;i++){
				list[i] = new KeyEntry();
				list[i].Key_ID = (byte)i;
			}
		}

		public KeyEntry this[int idx]{
			get{
				return list[idx];
			}
		}

		public override string ToString() {
			string s = "Key{\r\n";
			string t;
			for(int i = 0;i < list.Length;i++){
				t = list[i].ToString();
				if(t == null || t == "")continue;
				s += "\t" + t + "\r\n";
			}

			s += "}\r\n";

			return s;
		}

		public void Parse(string input,ValEntryList vallist){
			input = input.Replace(" ","").Replace("\t","").Replace("\n","").Replace("\r","");
			string[] list = input.Split(':', ',', '(', ')');

			KeyEntry buff;
			ValEntry valbuff;
			string valname;
			string step;
			int i = 0;
			while(i < list.Length -1){
				buff = this.list[int.Parse(list[i++], CultureInfo.InvariantCulture)];
				do{
					if(list[i] == "")i++;

					valname = list[i++];
					step = list[i++];
					if(step.ToLower().StartsWith("step=")){
						step = step.Substring(5);
					}
					valbuff = vallist[valname];
					if(valbuff != null)
						buff.AssignWork(vallist[valname],float.Parse(step, CultureInfo.InvariantCulture));
				}while(i < list.Length - 1 && list[i] == "");
			}
			


		}

	}
}
