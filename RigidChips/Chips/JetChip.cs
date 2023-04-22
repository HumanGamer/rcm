using System;
using System.Drawing;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System.Diagnostics;

namespace RigidChips {
	///<summery>
	///ジェット/バルーン チップ クラス
	///</summery>
	public class JetChip		: ChipBase{
		XFile jet,baloon,fire;
		ChipAttribute power,damper,spring,effect,option;
		public JetChip(){}
		public JetChip(Environment gen,ChipBase parent,JointPosition pos) : base(gen,parent,pos){
			jet = Environment.GetMesh("Jet.x");
			baloon = Environment.GetMesh("Jet2.x");
			fire = Environment.GetMesh("Fire.x");
			if(!Environment.EditOption.ConvertParentAttributes || parent == null || parent.GetType() != this.GetType()){
				damper.Val = null;
				damper.Const = 0.5f;
				spring.Val = null;
				spring.Const = 0.5f;
				effect.Const = 0f;
			}
		}

		public override float Fuel {
			get{return 0;}
		}


		public override ChipType ChipType {
			get {
				return ChipType.Jet;
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
					return "Jet Output and Balloon gas volume";
				case "Option":
					return "0: Jet 1: Hydrogen balloon 2: Air balloon";
				case "Effect":
					return "1-4: Smoke (Jet only)";
				case "User1":
				case "User2":
					return "For scenarios";
				default:
					return base.AttrTip(AttrName);
			}

		}

		public override void DrawChip() {
			if(jet == null)
				jet = Environment.GetMesh("Jet.x");
			if(baloon == null)
				baloon = Environment.GetMesh("Jet2.x");
			float size = (float)Math.Pow(power.Value,0.3);
			if(option.Const == 0f){
				if(jet != null)
					jet.Draw(Environment.d3ddevice,ChipColor.ToColor(),matRotation * Matrix);
				if (Environment.Preview) {
					// 2000で1チップの長さ
					// 最大で2.5チップ分(5000)
					// それ以上は伸びない
					if (fire == null)
						fire = Environment.GetMesh("Fire.x");
					float scale = Math.Abs(this.power.Value);
					if (scale > 5000) scale = 5000;
					scale *= Math.Sign(this.power.Value) / 2000.0f;
					Debug.WriteLine(scale, "jet scale");
					Matrix m =
						  Matrix.Translation(0f, -0.3f, 0f)
						* Matrix.Scaling(1f, scale, 1f)
						* matRotation 
						* Matrix;
					ReserveDraw(() => {
						//bool lighting = Generics.d3ddevice.RenderState.Lighting;
						//Generics.d3ddevice.RenderState.Lighting = false;
						fire.Draw2(Environment.d3ddevice, Color.Transparent, 0x0FFF, m);
						//Generics.d3ddevice.RenderState.Lighting = lighting;
					});
				}
			}
			
			else if(Environment.DrawOption.BaloonSwelling){
				float rate = 0.5f + Environment.DrawOption.BaloonSwellingRatio * (float)Math.Pow(power.Value * 3 / (4f * Math.PI),1.0/3.0);
				/*
					 * V = 4/3 * Pi *r^3 
					 * r^3 = V*3/4 / PI
					 * 
					 * ax^3 + bx^2 + cx + d = ?
					 * 0	:0.5 d = 0.5
					 * 160	:1
					 * 3340	:2
					 */
				rate = Math.Abs(rate);
				if(baloon != null)
					baloon.Draw(Environment.d3ddevice,ChipColor.ToColor(),Matrix.Scaling(rate,rate,rate) * matRotation * Matrix);
			}
			else
				if(baloon != null)
				baloon.Draw(Environment.d3ddevice,ChipColor.ToColor(),matRotation * Matrix);
		}

		public override string[] AttrNameList {
			get{
				string[] s = {"Color","Angle","Damper","Spring","Power","Option","Effect","User1","User2"};
				return s;
			}
		}

		public override float[] AttrDefaultValueList {
			get {
				return new float[]{(float)0xFFFFFF,0f,0.5f,0.5f,0f,0f,0f,0f,0f};
			}
		}

		public override ChipAttribute[] AttrValList {
			get {
				return new ChipAttribute[]{ChipColor,angle,damper,spring,power,option,effect,user1,user2};
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
					case "Effect":
						return this.effect;
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
					case "Effect":
						this.effect = value;
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

		/*		public override string ToString() {
					string s = "";
					s += "Jet(";	//	←チップの出力名

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
		public override HitStatus IsHit(int X, int Y, int ScrWidth, int ScrHeight) {
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
			// 投影行列
			Matrix projMat = Environment.d3ddevice.Transform.Projection;
			// ビュー行列
			Matrix viewMat = Environment.d3ddevice.Transform.View;
			// ビューポート
			Viewport viewport = new Viewport();

			IntersectInformation sectinfo = new IntersectInformation();

			viewport.Width = ScrWidth; 
			viewport.Height = ScrHeight; 
			viewport.X = viewport.Y = 0;
			viewport.MaxZ = 1.0f; 
			viewport.MinZ = 0.0f;

			// クリックしたスクリーン座標からレイを計算し、対象メッシュとの交差をチェック 
			Vector3 vNear = Vector3.Unproject(new Vector3(X, Y, viewport.MinZ), 
				viewport, projMat, viewMat, this.Matrix /* ここがWorld行列 */);
			Vector3 vFar = Vector3.Unproject(new Vector3(X, Y, viewport.MaxZ), 
				viewport, projMat, viewMat, this.Matrix /* ここがWorld行列 */);
			Vector3 vDir = Vector3.Normalize(vFar - vNear);
			
			if(option.Value != 0f)
				buff.distance = (baloon.mesh.Intersect(vNear, vDir, out sectinfo)) ? sectinfo.Dist : float.MaxValue; 
			else
				buff.distance = (Environment.imesh.Intersect(vNear, vDir, out sectinfo)) ? sectinfo.Dist : float.MaxValue; 

			if(dist.distance > buff.distance){
				dist.distance = buff.distance;
				dist.HitChip = this;
			}

			return dist;
		}

		public override ChipBase Clone(bool WithChild,ChipBase parent) {
			JetChip copy = new JetChip();
			copy.Name = this.Name;
			copy.angle = this.angle;
			copy.ChipColor = this.ChipColor;
			copy.Environment = this.Environment;
			copy.jointPosition = this.jointPosition;
			copy.matVersion = this.matVersion;
			copy.Parent = parent;

			copy.jet = this.jet;
			copy.baloon = this.baloon;

			copy.damper = this.damper;
			copy.spring = this.spring;
			copy.power = this.power;
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

		public override void ReverseY() {
			base.ReverseY();
			if(this.angle.Val != null)
				this.angle.IsNegative ^= true;
			else
				this.angle.Const = -this.angle.Const;

			if(this.power.Val != null)
				this.power.IsNegative ^= true;
			else
				this.power.Const = -this.power.Const;
		}


		public override float Weight {
			get {
				return 1.47f;
			}
		}

	}
}

