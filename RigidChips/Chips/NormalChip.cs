namespace RigidChips {
	///<summery>
	///チップ チップ クラス
	///</summery>
	public class NormalChip		: ChipBase{
		protected ChipAttribute damper,spring;
		protected XFile mesh;
		public NormalChip(){}
		public NormalChip(Environment gen,ChipBase parent,JointPosition pos) : base(gen,parent,pos){
			mesh = Environment.GetMesh("Chip.x");
			if(!Environment.EditOption.ConvertParentAttributes || parent == null || parent.GetType() != this.GetType()){
				damper.Val = null;
				damper.Const = 0.5f;
				spring.Val = null;
				spring.Const = 0.5f;
			}
		}



		public override ChipType ChipType {
			get {
				return ChipType.Chip;
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
				case "User1":
				case "User2":
					return "For scenarios";
				default:
					return base.AttrTip(AttrName);
			}
		}

		public override void DrawChip() {
			if(mesh == null)
				mesh = Environment.GetMesh("Chip.x");
			if(mesh != null)
				mesh.Draw(Environment.d3ddevice,ChipColor.ToColor(),this.matRotation * this.Matrix);
		}

		public override string[] AttrNameList {
			get{
				string[] s = {"Color","Angle","Damper","Spring","User1","User2"};
				return s;
			}
		}

		public override float[] AttrDefaultValueList {
			get {
				return new float[]{(float)0xFFFFFF,0f,0.5f,0.5f,0f,0f};
			}
		}

		public override ChipAttribute[] AttrValList {
			get {
				return new ChipAttribute[]{ChipColor,angle,damper,spring,user1,user2};
			}
		}


		/*		public override string ToString() {
					string str = "";
					str += "Chip(";

					if(Name != null && Name != "")
						str += "Name=" + Name + ",";
					if(angle.Val != null || angle.Const != 0f)
						str += "Angle=" + angle.ToString() + ",";
					if(this.ChipColor != RcColor.Default)
						str += "Color=" + ChipColor.ToString() + ",";
					if(damper.Val != null || damper.Const != 0.5f)
						str += "Damper=" + damper.ToString() + ",";
					if(spring.Val != null || spring.Const != 0.5f)
						str += "Spring=" + spring.ToString() + ",";
					if(user1.Val != null || user1.Const != 0f)
						str += "User1=" + user1.ToString() + ",";
					if(user2.Val != null || user2.Const != 0f)
						str += "User2=" + user2.ToString() + ",";
					str = str.TrimEnd(',');
					str += ")";

					return str;

				}
		*/
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


		public override ChipBase Clone(bool WithChild,ChipBase parent) {
			NormalChip copy = new NormalChip();
			copy.angle = this.angle;
			copy.ChipColor = this.ChipColor;
			copy.damper = this.damper;
			copy.Environment = this.Environment;
			copy.jointPosition = this.jointPosition;
			copy.mesh = this.mesh;
			copy.matVersion = this.matVersion;
			copy.Name = this.Name;
			copy.Parent = parent;
			copy.spring = this.spring;
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
				return 1f;
			}
		}



	}
}

