﻿namespace RigidChips {
	///<summery>
	///アーム チップ クラス
	///</summery>
	public class ArmChip		: ChipBase{
		XFile mesh;
		ChipAttribute power,damper,spring,option;
		public ArmChip(){}
		public ArmChip(Environment gen,ChipBase parent,JointPosition pos) : base(gen,parent,pos){
			mesh = Environment.GetMesh("Arm.x");
			if(!Environment.EditOption.ConvertParentAttributes || parent == null || parent.GetType() != this.GetType()){
				damper.Val = null;
				damper.Const = 0.5f;
				spring.Val = null;
				spring.Const = 0.5f;
			}
		}

		public override ChipType ChipType {
			get {
				return ChipType.Arm;
			}
		}
		public override string AttrTip(string AttrName) {
			switch(AttrName){
				case "Angle":
					return "折り曲げ角度";
				case "Damper":
				case "Dumper":
				case "Danper":
				case "Dunper":
					return "接続部の堅さ";
				case "Spring":
					return "接続部の弾性";
				case "Power":
					return "トリガ";
				case "Option":
					return "威力\nこれに比例した時間をチャージに要する";
				case "User1":
				case "User2":
					return "シナリオ用";
				default:
					return base.AttrTip(AttrName);
			}
		}

		public override void DrawChip() {
			if(mesh == null)
				mesh = Environment.GetMesh("Arm.x");
			if(mesh != null)
				mesh.Draw(Environment.d3ddevice,ChipColor.ToColor(),matRotation * matTranslation);
		}

		public override string[] AttrNameList {
			get{
				string[] s = {"Color","Angle","Damper","Spring","Power","Option","User1","User2"};
				return s;
			}
		}

		public override ChipAttribute this[string AttrName] {
			get {
				switch(AttrName){
					case "Angle":
						return this.angle;
					case "Damper":
					case "Dumper":
					case "Danper":
					case "Dunper":
						return this.damper;
					case "Spring":
						return this.spring;
					case "Power":
						return this.power;
					case "Option":
						return this.option;
					case "User1":
						return this.user1;
					case "User2":
						return this.user2;
					default:
						return base[AttrName];
				}
			}
			set {
				switch(AttrName){
					case "Angle":
						this.angle = value;
						return;
					case "Damper":
					case "Dumper":
					case "Danper":
					case "Dunper":
						this.damper = value;
						return;
					case "Spring":
						this.spring = value;
						return;
					case "Power":
						this.power = value;
						return;
					case "Option":
						this.option = value;
						return;
					case "User1":
						this.user1 = value;
						return;
					case "User2":
						this.user2 = value;
						return;
					default:
						base[AttrName] = value;
						return;
				}
			}
		}

		public override float[] AttrDefaultValueList {
			get {
				return new float[]{(float)0xFFFFFF,0f,0.5f,0.5f,0f,0f,0f,0f};
			}
		}

		public override ChipAttribute[] AttrValList {
			get {
				return new ChipAttribute[]{ChipColor,angle,damper,spring,power,option,user1,user2};
			}
		}



		/*		public override string ToString() {
					string s = "";
					s += "Arm(";	//	←チップの出力名

					//	属性記述ブロック
					if(ChipColor != RcColor.Default)
						s += "Color=" + ChipColor.ToString() + ",";
					if(Name != null && Name != "")
						s += "Name=" + Name + ",";
					if(angle.Val != null || angle.Const != 0f)
						s += "Angle=" + angle.ToString() + ",";
					if(damper.Val != null || damper.Const != 0.5f)
						s += "Damper=" + damper.ToString() + ",";
					if(spring.Val != null || spring.Const != 0.5f)
						s += "Spring=" + spring.ToString() + ",";
					if(power.Val != null || power.Const != 0f)
						s += "Power=" + power.ToString() + ",";
					if(option.Val != null || option.Const != 0f)
						s += "Option=" + option.ToString() + ",";
					if(user1.Val != null || user1.Const != 0f)
						s += "User1=" + user1.ToString() + ",";
					if(user2.Val != null || user2.Const != 0f)
						s += "User2=" + user2.ToString() + ",";

					s = s.TrimEnd(',');

					s += ")";

					return s;
				}
		*/
		public override ChipBase Clone(bool WithChild,ChipBase parent) {
			ArmChip copy = new ArmChip();
			copy.Name = this.Name;
			copy.angle = this.angle;
			copy.ChipColor = this.ChipColor;
			copy.Environment = this.Environment;
			copy.jointPosition = this.jointPosition;
			copy.matVersion = this.matVersion;
			copy.Parent = parent;

			copy.mesh = this.mesh;

			copy.damper = this.damper;
			copy.spring = this.spring;
			copy.power = this.power;
			copy.option = this.option;
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

		public override void ReverseY() {
			base.ReverseY();
			if(this.angle.Val != null)
				this.angle.IsNegative ^= true;
			else
				this.angle.Const = -this.angle.Const;
		}

		public override float Weight {
			get {
				return 2f;
			}
		}

	}
}

