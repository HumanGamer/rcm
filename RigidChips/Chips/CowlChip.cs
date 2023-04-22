using System;
using System.Windows.Forms;

namespace RigidChips {
	///<summery>
	///カウル チップ クラス
	///</summery>
	public class CowlChip		: ChipBase{
		XFile[] meshes;
		int option;
		ChipAttribute effect;
		public CowlChip(){}
		public CowlChip(Environment gen,ChipBase parent,JointPosition pos) : base(gen,parent,pos){
			meshes = new XFile[6];
			meshes[0] = Environment.GetMesh("Type0.x");
			meshes[1] = Environment.GetMesh("Type1.x");
			meshes[2] = Environment.GetMesh("Type2.x");
			meshes[3] = Environment.GetMesh("Type3.x");
			meshes[4] = Environment.GetMesh("Type4.x");
			meshes[5] = Environment.GetMesh("Type5.x");
			if(!Environment.EditOption.ConvertParentAttributes || parent == null || parent.GetType() != this.GetType()){
				option = 0;
				effect = new ChipAttribute(0x00fb);
			}
			else{
				this.option = ((CowlChip)parent).option;
				effect = ((CowlChip)parent).effect;
			}
		}

		public override ChipType ChipType {
			get {
				return ChipType.Cowl;
			}
		}

		public override void Add(JointPosition joint, ChipBase chip,bool Registeration) {
			if(!(chip is CowlChip)){
				MessageBox.Show("Only Cowl can be connected to Cowl! Remove non-Cowl chip to make this model usable in-game again.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
			base.Add(joint,chip,Registeration);
		}

		public override string AttrTip(string AttrName) {
			switch(AttrName){
				case "Angle":
					return "Bending angle";
				case "Option":
					return "Shape of Cowl";
				case "Effect":
					return "Material details (4-digit hexadecimal number)\n Left ← transparency, luminosity, specular intensity, specular → right";
				case "User1":
				case "User2":
					return "For scenarios";
				default:
					return base.AttrTip(AttrName);
			}
		}

		public override void DrawChip() {
			ReserveDraw(() => {
				if (Environment.DrawOption.ShowCowl && (((int)this.effect.Value) & 0xF000) != 0xF000)
					if (meshes != null) {
						XFile targetMesh = meshes[(option < 0 || option > 5) ? 0 : option];
						targetMesh.Draw(
							Environment.d3ddevice,
							ChipColor.ToColor(),
							(int)effect.Value,
							this.matRotation * this.Matrix);
					}
			});
		}

		public override string[] AttrNameList {
			get{
				string[] s = {"Color","Angle","Option","Effect","User1","User2"};
				return s;
			}
		}

		public override float[] AttrDefaultValueList {
			get {
				return new float[]{(float)0xFFFFFF,0f,0f,0x00fb,0f,0f};
			}
		}

		public override ChipAttribute[] AttrValList {
			get {
				ChipAttribute attropt = new ChipAttribute();
				attropt.Const = option;
				return new ChipAttribute[]{ChipColor,angle,attropt,effect,user1,user2};
			}
		}


		public override ChipAttribute this[string AttrName] {
			get {
				ChipAttribute temp = new ChipAttribute();
				switch(AttrName){
					case  "Angle":
						return angle;
					case "Option":
						temp.Const = (float)option;
						return temp;
					case "Effect":
						return effect;
					case "User1":
						return user1;
					case "User2":
						return user2;
					default:
						return base[AttrName];
				}
			}
			set {
				switch(AttrName){
					case  "Angle":
						angle = value;
						return;
					case "Option":
						option = (int)value.Const;
						return;
					case "Effect":
						effect = value;
						return;
					case "User1":
						user1 = value;
						return;
					case "User2":
						user2 = value;
						return;
					default:
						base[AttrName] = value;
						return;
				}
			}
		}

		/*		public override string ToString() {
					string str = "";
					str += "Cowl(";	//	←チップの出力名

					//	属性記述ブロック
					if(ChipColor != RcColor.Default)
						str += "Color=" + ChipColor.ToString() + ",";
					if(angle.Val != null || angle.Const != 0f)
						str += "Angle=" + angle.ToString() + ",";
					if(Name != null && Name != "")
						str += "Name=" + Name + ",";
					if(option > 1 || option < 5)
						str += "Option=" + option + ",";
					if(user1.Val != null || user1.Const != 0f)
						str += "User1=" + user1.ToString() + ",";
					if(user2.Val != null || user2.Const != 0f)
						str += "User2=" + user2.ToString() + ",";

					str = str.TrimEnd(',');
					str += ")";

					return str;

				}
		*/

		public override ChipBase Clone(bool WithChild,ChipBase parent) {
			CowlChip copy = new CowlChip();
			copy.Name = this.Name;
			copy.angle = this.angle;
			copy.ChipColor = this.ChipColor;
			copy.Environment = this.Environment;
			copy.jointPosition = this.jointPosition;
			copy.matVersion = this.matVersion;
			copy.Parent = parent;

			copy.meshes = new XFile[this.meshes.Length];
			this.meshes.CopyTo(copy.meshes,0);

			copy.option = this.option;
			copy.effect = this.effect;
			copy.user1 = this.user1;
			copy.user2 = this.user2;
			copy.Comment = this.Comment;

			copy.Children = new ChipBase[Environment.ChildCapasity];
			if(WithChild){
				for(int i = 0;i < Environment.ChildCapasity;i++){
					if(this.Children[i] != null)copy.Add(this.Children[i].JointPosition ,this.Children[i].Clone(WithChild,copy),false);
				}
			}
			
			return copy;
		}

		public override void ReverseX() {
			base.ReverseX ();
			if(option == 3)
				option = 4;
			else if(option == 4)
				option = 3;
		}

		public override void ReverseY() {
			base.ReverseY();
			if(this.angle.Val != null)
				this.angle.IsNegative ^= true;
			else
				this.angle.Const = -this.angle.Const;
		}


		public override void ReverseZ() {
			base.ReverseZ ();
			if(option == 3)
				option = 4;
			else if(option == 4)
				option = 3;
		}


		public override float Weight {
			get {
				return 0f;
			}
		}
		public override HitStatus IsHit(int X, int Y, int ScrWidth, int ScrHeight) {
			if(Environment.EditOption.UnvisibleNotSelected && !Environment.DrawOption.ShowCowl || ((int)this.effect.Value & 0xF000) == 0xF000){
				HitStatus dist ,buff;
				dist.distance = float.MaxValue;
				dist.HitChip = null;
				foreach(ChipBase c in Children){
					if(c != null){
						buff = c.IsHit(X,Y,ScrWidth,ScrHeight);
						if(dist.distance > buff.distance){
							dist = buff;
						}
					}
				}
				return dist;
			}
			else 
				return base.IsHit (X, Y, ScrWidth, ScrHeight);

		}


	}
}

