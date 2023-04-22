using System.Windows.Forms;

namespace RigidChips {
	///<summery>
	///ラダーフレーム チップ クラス
	///</summery>
	public class RudderFrameChip	: RudderChip{
		bool option;
		XFile mesh2;
		public RudderFrameChip(){}
		public RudderFrameChip(Environment gen,ChipBase parent,JointPosition pos) : base(gen,parent,pos){
			mesh2 = Environment.GetMesh("RudderF.x");
			if(!Environment.EditOption.ConvertParentAttributes || parent == null || parent.GetType() != this.GetType()){
				option = false;
			}
		}

		public override float Fuel {
get{			return base.Fuel / 2;}
		}


		public override ChipType ChipType {
			get {
				return ChipType.RudderF;
			}
		}
		public override void DrawChip() {
			if(mesh2 == null)
				mesh2 = Environment.GetMesh("RudderF.x");
			if(option){
				if(Environment.DrawOption.FrameGhostShow){
					switch(Environment.DrawOption.FrameGhostView){
						case 0:
							if(mesh != null)
								mesh.Draw(Environment.d3ddevice,ChipColor.ToColor(),0xA022,matRotation * matTranslation);
							break;
						case 1:
							if(mesh2 != null)
								mesh2.Draw(Environment.d3ddevice,ChipColor.ToColor(),matRotation * matTranslation);
							break;
						case 2:
							XFile x = Environment.GetMesh(Application.StartupPath + "\\Resources\\RudderG.x",true);
							if(x != null)x.Draw(Environment.d3ddevice,ChipColor.ToColor(),matRotation * matTranslation);
							break;
					}
				}
			}
			else{
				if(mesh2 != null)
					mesh2.Draw(Environment.d3ddevice,ChipColor.ToColor(),matRotation * matTranslation);
			}
		}

		public override string AttrTip(string AttrName) {
			if(AttrName == "Option")
				return "Set 1 for invisible chip";
			return base.AttrTip (AttrName);
		}

		public override string[] AttrNameList {
			get{
				string[] s = base.AttrNameList;
				s.CopyTo(s = new string[s.Length + 1],0);
				s[s.Length -1] = s[s.Length - 2];
				s[s.Length -2] = s[s.Length - 3];
				s[s.Length -3] = "Option";
				return s;
			}
		}

		public override float[] AttrDefaultValueList {
			get {
				return new float[]{(float)0xFFFFFF,0f,0.5f,0.5f,0f,0f,0f};
			}
		}

		public override ChipAttribute[] AttrValList {
			get {
				ChipAttribute attropt = new ChipAttribute();
				attropt.Const = (option ? 1 : 0);
				return new ChipAttribute[]{ChipColor,angle,damper,spring,attropt,user1,user2};
			}
		}

		public override ChipAttribute this[string AttrName] {
			get {
				ChipAttribute v;
				if(AttrName == "Option"){
					v.Const = new Constant(option ? 1f : 0f, false);
					v.IsNegative = false;
					v.Val = null;
					return v;
				}
				return base[AttrName];
			}
			set {
				if(AttrName == "Option")
					option = (value.Value != 0f);
				else base[AttrName] = value;
			}
		}

		/*		public override string ToString() {
					string str = "";
					str += "RudderF(";

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
					if(option)
						str += "Option=1,";
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
			RudderFrameChip copy = new RudderFrameChip();
			copy.Name = this.Name;
			copy.angle = this.angle;
			copy.ChipColor = this.ChipColor;
			copy.Environment = this.Environment;
			copy.jointPosition = this.jointPosition;
			copy.matVersion = this.matVersion;
			copy.Parent = parent;

			copy.damper = this.damper;
			copy.spring = this.spring;
			copy.mesh = this.mesh;
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
				return 0.5f;
			}
		}

		public override HitStatus IsHit(int X, int Y, int ScrWidth, int ScrHeight) {
			if(option && !Environment.DrawOption.FrameGhostShow){
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

