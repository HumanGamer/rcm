using System;
using System.Diagnostics;
using System.Globalization;

namespace RigidChips {
	/// <summary>
	/// 変数(RcVal)のリスト
	/// </summary>
	public class ValEntryList{
		public ValEntry[] List;


		public ValEntryList(){
			List = new ValEntry[0];
		}

		public ValEntry this[int idx]{
			get{
				return List[idx];
			}
		}
		public ValEntry this[string valname]{
			get{
				foreach(ValEntry v in List)
					if(string.Compare(v.ValName, valname,true) == 0)return v;
				
				return null;
			}
		}

		public void Swap(ValEntry val1,ValEntry val2){
			int i,j;
			for(i = 0;i < List.Length;i++){
				if(val1.ValName == List[i].ValName)
					break;
			}
			if(i == List.Length)throw new Exception("この変数はリストに存在しません : " + val1.ValName);

			for(j = 0;j < List.Length;j++){
				if(val2.ValName == List[j].ValName){
					List[i] = val2;
					List[j] = val1;
					return;
				}
			}
			throw new Exception("この変数はリストに存在しません : " + val2.ValName);

		}

		/// <summary>
		/// 変数を作成する。
		/// </summary>
		/// <param name="ValName">変数の識別名。</param>
		/// <returns>新たに作成されたRcValへの参照。</returns>
		public ValEntry Add(string ValName){
			if(ValName == null)return null;
			if(this[ValName] != null) throw new Exception("指定された名前の変数はすでに存在します : " + ValName);
			List.CopyTo(List = new ValEntry[List.Length +1],0);
			( List[List.Length-1] = new ValEntry() ).ValName = ValName;
			List[List.Length - 1].List = this;
			return List[List.Length-1];
		}

		/// <summary>
		/// 指定された名前の変数を削除する。
		/// </summary>
		/// <param name="ValName">削除する変数の識別名。</param>
		public void Remove(string ValName){
			bool removed = false;
			ValEntry[] buff = new ValEntry[List.Length - 1];
			for(int i = 0;i < List.Length;i++){
				if(removed)
					buff[i-1] = List[i];
				else if(List[i].ValName == ValName){
					List[i].RefCount = -65536;
					removed = true;
				}
				else
					buff[i] = List[i];
			}
			if(!removed)throw new Exception("指定された名前の変数はリストに存在しません : " + ValName);
			List = buff;
		}

		public int Count{
			get{
				return List.Length;
			}
		}

		public override string ToString() {
			string str = "Val{";
			foreach(ValEntry v in List){
				str += "\r\n\t" + v.ToString();
			}
			str += "\r\n}\r\n";
			return str;
		}
		public void Parse(string data){
			int start,end;
			ValEntry buff;

			start = data.IndexOf('{');
			end = data.LastIndexOf('}');
			if(start >= 0 && end >= 0)
				data = data.Substring(start+1,end-start);
			data = data.Replace(" ","").Replace("\t","").Replace("\n","").Replace("\r","");

			string[] list = data.Split('(',')');
			string[] param;

			for(int i = 0;i < list.Length;i += 2){
				if(list[i] == "")break;
                
				buff = this.Add(list[i]);
				param = list[i+1].Split(',','=');
				for(int j = 0;j < param.Length;j += 2){
					switch(param[j].ToLower()){
						case "default":
							if(param[j+1][0] == '#')
								buff.Default = (float)int.Parse(param[j+1].Substring(1),System.Globalization.NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture);
							else
								buff.Default = float.Parse(param[j+1], CultureInfo.InvariantCulture);
							break;
						case "min":
							if(param[j+1][0] == '#')
								buff.Min = (float)int.Parse(param[j+1].Substring(1),System.Globalization.NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture);
							else
								buff.Min = float.Parse(param[j+1], CultureInfo.InvariantCulture);
							break;
						case "max":
							if(param[j+1][0] == '#')
								buff.Max = (float)int.Parse(param[j+1].Substring(1),System.Globalization.NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture);
							else
								buff.Max = float.Parse(param[j+1], CultureInfo.InvariantCulture);
							break;
						case "step":
							if(param[j+1][0] == '#')
								buff.Step = (float)int.Parse(param[j+1].Substring(1),System.Globalization.NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture);
							else
								buff.Step = float.Parse(param[j+1], CultureInfo.InvariantCulture);
							break;
						case "disp":
							buff.Disp = (float.Parse(param[j+1], CultureInfo.InvariantCulture) != 0f);
							break;
						case "":
							break;
						default:
							throw new Exception("不明な変数パラメータです : " + param[j]);
					}
				}
			}
		}

		internal void Tick() {
			Debug.WriteLine("ValList::Tick");
			foreach (ValEntry v in List)
				if(v != null)
					v.Ticked = true;
		}

		internal void Tack() {
			Debug.WriteLine("ValList::Tack");
			foreach (ValEntry v in List)
				if (v != null)
					v.Ticked = false;
		}
	}
}
