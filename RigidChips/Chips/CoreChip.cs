using System;
using System.Windows.Forms;
using Microsoft.DirectX;
using System.Threading;

namespace RigidChips {
	///<summery>
	///コア チップ クラス。
	///</summery>
	public class CoreChip		: ChipBase{
		XFile mesh;
		public CoreChip(Environment gen,ChipBase parent,JointPosition pos) : base(gen,null,pos){
			this.JointPosition = JointPosition.NULL;
			this.matTranslation = Matrix.Identity;
			this.matRotation = Matrix.RotationY((float)Math.PI);
			mesh = Environment.GetMesh("Core.x");
		}

		public override float Fuel {
			get{	return 2000000;			}
		}



		public override ChipType ChipType {
			get {
				return ChipType.Core;
			}
		}

		public override void Attach(ChipBase to, JointPosition pos) {
			if(to == null)return;
			throw new Exception("Coreは他のチップの派生チップには出来ません。");
		}

		public override string AttrTip(string AttrName) {
			switch(AttrName){
				case "User1":
					return "For scenarios";
				case "User2":
					return "For scenarios";
				default:
					return base.AttrTip(AttrName);
			}
		}

		public override void Detach() {
			return;
		}

		public override void DrawChip() {
			if(mesh == null)
				mesh = Environment.GetMesh("Core.x");
			if(mesh != null)
				mesh.Draw(Environment.d3ddevice,this.ChipColor.ToColor(),matRotation * Matrix);
		}

		public ThreadStart ReservedDrawFunc;
		public override void ReserveDraw(System.Threading.ThreadStart fnc) {
			ReservedDrawFunc += fnc;
		}

		public override string[] AttrNameList {
			get{
				string[] s = {"Color","User1","User2"};
				return s;
			}
		}

		public override float[] AttrDefaultValueList {
			get {
				return new float[]{(float)0xFFFFFF,0f,0f};
			}
		}

		public override ChipAttribute[] AttrValList {
			get {
				return new ChipAttribute[]{ChipColor,user1,user2};
			}
		}


		public override Matrix Matrix {
			get {
				return Matrix.Identity;
			}
		}

		public override long MatrixVersion {
			get {
				return long.MinValue;
			}
		}

		public override ChipAttribute this[string AttrName] {
			get {
				if(AttrName == "User1")
					return user1;
				else if(AttrName == "User2")
					return user2;
				else return base[AttrName];
			}
			set {
				if(AttrName == "User1"){
					user1 = value;
				}
				else if(AttrName == "User2"){
					user2 = value;
				}
				else
					base[AttrName] = value;
			}
		}

		/*		public override string ToString() {
					string s = "";
					s += "Core(";	//	←チップの出力名

					//	属性記述ブロック
					if(ChipColor != RcColor.Default)
						s += "Color=" + ChipColor.ToString() + ",";
					if(Name != null && Name != "")
						s += "Name=" + Name + ",";
					if(user1.Val != null || user1.Const != 0f)
						s += "User1=" + user1.ToString() + ",";
					if(user2.Val != null || user2.Const != 0f)
						s += "User2=" + user2.ToString() + ",";

					s = s.TrimEnd(',');

					s += ")";

					return s;
				}

		*/
		public override void UpdateMatrix() {
			this.matTranslation = Matrix.Identity;
			foreach(ChipBase cb in this.Children)
				if(cb != null)cb.UpdateMatrix();
		}


		public override ChipBase Clone(bool WithChild,ChipBase parent) {
			return null;
		}


		public override AngleDirection AngleType {
			get {
				return AngleDirection.None;
			}
		}


		public override float Weight {
			get {
				return 1f;
			}
		}

		public void Read(string data){
			string[] param = data.Split(',',':','(',')','=');
			int index = 1;
			
			ChipAttribute attr = new ChipAttribute();
			try{
				for( ; param[index] != "" ; index += 2){
					param[index] = param[index].ToLower();
					if(param[index] == "name")
						this.Name = param[index+1];
					else if(param[index] == "color")
						this["Color"] = new ChipAttribute(param[index+1],Environment.vals);
					else{
						param[index] = char.ToUpper(param[index][0]).ToString() + param[index].Substring(1);
						attr.SetValue(param[index+1],Environment.vals);
						this[param[index]] = attr;
					}

				}
			}
			catch(IndexOutOfRangeException){
				throw new Exception("不正な属性値指定が存在します。");
			}
			catch(Exception e){
				MessageBox.Show(e.Message);
			}
		}

	}
}

