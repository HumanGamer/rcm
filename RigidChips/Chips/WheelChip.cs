using System;

using Microsoft.DirectX;

namespace RigidChips {
	///<summery>
	///ホイール チップ クラス
	///</summery>
	public class WheelChip	: ChipBase{
		protected XFile mesh;
		protected XFile tire;
		protected ChipAttribute damper,spring,power,brake,effect;
		protected int option;
		public WheelChip(){}
		public WheelChip(Environment gen,ChipBase parent,JointPosition pos) : base(gen,parent,pos){
			mesh = Environment.GetMesh("Wheel.x");
			tire = Environment.GetMesh("WheelT.x");
			if(!Environment.EditOption.ConvertParentAttributes || parent == null || parent.GetType() != this.GetType()){
				damper.Const = spring.Const = 0.5f;
			}
		}

		public override float Fuel {
			get{return 0;}
		}


		public override ChipType ChipType {
			get {
				return ChipType.Wheel;
			}
		}

		public float PreviewRotation {
			get;
			set;
		}
		public override void DrawChip() {
			if(mesh == null)
				mesh = Environment.GetMesh("Wheel.x");

			// 角度反映
			Matrix rot;
			if (Environment.Preview) {
				PreviewRotation += (float)(power.Value * Math.PI / 15000);
				//PreviewRotation += power.Value > 0 ? 0.125f : power.Value < 0 ? -0.125f : 0f;
				rot = Matrix.RotationY(PreviewRotation);
			}
			else {
				PreviewRotation = 0f;
				rot = Matrix.Identity; ;
			}

			if(mesh != null)
				mesh.Draw(Environment.d3ddevice,ChipColor.ToColor(),rot * matRotation * Matrix);

			if((1 <= option && option <= 2) || effect.Value >= 1f){
				float r,w = 1f;
				switch(option){
					case 1:
						r = 0.75f;
						break;
					case 2:
						r = 1f;
						break;
					default:
						r = 0.5f;
						break;
				}
				w = Math.Max(effect.Value,option >0 ? 1f : 0f);
				if(w > 10f)w = 10f;
				if(tire == null)
					tire = Environment.GetMesh("WheelT.x");
				if(tire != null)
					tire.Draw(Environment.d3ddevice,ChipColor.ToColor(),rot * Matrix.Scaling(r,w,r) * matRotation * Matrix);
			}
		}

		public override string AttrTip(string AttrName) {
			switch(AttrName){
				case "Angle":
					return "Bending angle";
				case "Damper":
				case "Dumper":
				case "Danper":
				case "Dunper":
					return "Tightness of connection";
				case "Spring":
					return "Elasticity of connection";
				case "Power":
					return "Rotation output";
				case "Brake":
				case "Break":
					return "Braking force";
				case "Option":
					return "Tire diameter 1:x1.5 2:x2.0";
				case "Effect":
					return "Tire width (1.0-10.0 no physical effect)";
				case "User1":
				case "User2":
					return "For scenarios";
				default:
					return base.AttrTip(AttrName);
			}
		}

		public override string[] AttrNameList {
			get{
				string[] s = {"Color","Angle","Damper","Spring","Power","Brake","Option","Effect","User1","User2"};
				return s;
			}
		}

		public override float[] AttrDefaultValueList {
			get {
				return new float[]{(float)0xFFFFFF,0f,0.5f,0.5f,0f,0f,0f,0f,0f,0f};
			}
		}

		public override ChipAttribute[] AttrValList {
			get {
				ChipAttribute attropt = new ChipAttribute();
				attropt.Const = option;
				return new ChipAttribute[]{ChipColor,angle,damper,spring,power,brake,attropt,effect,user1,user2};
			}
		}

		public override ChipAttribute this[string AttrName] {
			get {
				switch(AttrName){
					case "Angle":
						return angle;
					case "Damper":
					case "Dumper":
					case "Danper":
					case "Dunper":
						return damper;
					case "Spring":
						return spring;
					case "Power":
						return power;
					case "Brake":
					case "Break":
						return brake;
					case "Option":
						ChipAttribute a = new ChipAttribute();
						a.Const = (float)option;
						return a;
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
					case "Angle":
						angle = value;
						return;
					case "Damper":
					case "Dumper":
					case "Danper":
					case "Dunper":
						damper = value;
						return;
					case "Spring":
						spring = value;
						return;
					case "Power":
						power = value;
						return;
					case "Brake":
					case "Break":
						brake = value;
						return;
					case "Option":
						option = (int)value.Value;
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

		public override void UpdateMatrix() {
			if (Environment.Preview) {
				PreviewRotation += power.Value / 8192f;
				//				PreviewRotation += power.Value > 0 ? 0.125f : power.Value < 0 ? -0.125f : 0f;
			}
			base.UpdateMatrix();
		}

		/*		public override string ToString() {
					string s = "";
					s += "Wheel(";	//	←チップの出力名

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
					if(brake.Val != null || brake.Const != 0f)
						s += "brake=" + brake.ToString() + ",";
					if(0 < option && option <= 2)
						s += "Option=" + option.ToString() + ",";
					if(effect.Val != null || effect.Const != 0f)
						s += "Effect=" + effect.ToString() + ",";
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
			WheelChip copy = new WheelChip();
			copy.Name = this.Name;
			copy.angle = this.angle;
			copy.ChipColor = this.ChipColor;
			copy.Environment = this.Environment;
			copy.jointPosition = this.jointPosition;
			copy.matVersion = this.matVersion;
			copy.Parent = parent;

			copy.mesh = this.mesh;
			copy.tire = this.tire;

			copy.damper = this.damper;
			copy.spring = this.spring;
			copy.power = this.power;
			copy.brake = this.brake;
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
			if(this.power.Val != null)
				this.power.IsNegative ^= true;
			else
				this.power.Const = -this.power.Const;
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
				return 3.14f;
			}
		}

		public override Matrix FullMatrix {
			get {
				Matrix rot;
				if (Environment.Preview) {
					rot = Matrix.RotationY(PreviewRotation);
				}
				else {
					rot = Matrix.Identity; ;
				}
				return rot * base.FullMatrix;
			}
		}



	}
}

