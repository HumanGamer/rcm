namespace RigidChips {
	///<summery>
	///ウェイト チップ クラス
	///</summery>
	public class WeightChip	: NormalChip{
		ChipAttribute option;

		public WeightChip(){}
		public WeightChip(Environment gen,ChipBase parent,JointPosition pos) : base(gen,parent,pos){
			option.Const = 1f;
			mesh = Environment.GetMesh("ChipH.x");
		}

		public override float Fuel {
			get { return 6 * base.Fuel * option.Value; }
		}


		public override ChipType ChipType {
			get {
				return ChipType.Weight;
			}
		}

		/*		public override string ToString() {
					string str = "";
					str += "Weight(";

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
					str = str.TrimEnd(',');
					str += ")";

					return str;

				}
		*/
		public override ChipBase Clone(bool WithChild,ChipBase parent) {
			WeightChip copy = new WeightChip();
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
			copy.option = this.option;
			copy.user1 = this.user1;
			copy.user2 = this.user2;
			copy.Children = new ChipBase[Environment.ChildCapasity];
			copy.Comment = this.Comment;
			if(WithChild){
				for(int i = 0;i < Environment.ChildCapasity;i++){
					if(this.Children[i] != null)copy.Add(this.Children[i].JointPosition ,this.Children[i].Clone(WithChild,copy),false);
				}
			}
			
			return copy;
		}


		public override float Weight {
			get {
				return 4f * option.Value;
			}
		}

		public override ChipAttribute[] AttrValList {
			get {
				return new ChipAttribute[]{ChipColor,angle,damper,spring,option,user1,user2};
			}
		}

		public override ChipAttribute this[string AttrName] {
			get {
				if(AttrName == "Option")
					return option;
				else
					return base[AttrName];
			}
			set {
				if(AttrName == "Option")
					option = value;
				else
					base[AttrName] = value;
			}
		}

		public override float[] AttrDefaultValueList {
			get {
				return new float[]{(float)0xFFFFFF,0f,0.5f,0.5f,1f,0f,0f};
			}
		}

		public override string[] AttrNameList {
			get {
				string[] s = base.AttrNameList;
				s.CopyTo(s = new string[s.Length + 1],0);
				s[s.Length -1] = s[s.Length - 2];
				s[s.Length -2] = s[s.Length - 3];
				s[s.Length -3] = "Option";
				return s;
			}
		}

		public override string AttrTip(string AttrName) {
			if(AttrName == "Option")
				return "Weight scale (1-8)";
			else
				return base.AttrTip (AttrName);
		}

	}
}

