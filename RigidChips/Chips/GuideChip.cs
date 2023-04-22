using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using DX = Microsoft.DirectX;
using D3D = Microsoft.DirectX.Direct3D;

namespace RigidChips {

	///<summery>
	///チップ追加用 ガイド クラス
	///</summery>
	public class GuideChip : ChipBase{
		XFile mesh;
		public GuideChip(Environment gen,ChipBase parent,JointPosition pos) : base(gen,parent,pos){
			mesh = Environment.GetMesh(Application.StartupPath + "\\Resources\\guide.x",true);
		}

		public override void Add(JointPosition joint, ChipBase chip,bool Registeration) {
			throw new Exception("RcChipGuideに、Add()は無効です。");
		}

		public override string AttrTip(string AttrName) {
			return "!RcChipGuide has no attributes.";
		}

		public override void DrawChip() {
			if(Parent.Parent == null)return;
			switch(this.JointPosition){
				case JointPosition.North:
					ChipColor.SetValue(Environment.DrawOption.NGuideColor.ToArgb());
					break;
				case JointPosition.South:
					ChipColor.SetValue(Environment.DrawOption.SGuideColor.ToArgb());
					break;
				case JointPosition.East:
					ChipColor.SetValue(Environment.DrawOption.EGuideColor.ToArgb());
					break;
				case JointPosition.West:
					ChipColor.SetValue(Environment.DrawOption.WGuideColor.ToArgb());
					break;
				default:
					ChipColor.SetValue(Color.LightGray.ToArgb());
					break;
			}
			//Environment.d3ddevice.Lights[0].Enabled = false;
			//Environment.d3ddevice.Lights[1].Enabled = true;
			Environment.d3ddevice.Lights[0].Update();
			Environment.d3ddevice.Lights[1].Update();
			if(mesh != null)
				mesh.Draw(Environment.d3ddevice,ChipColor.ToColor(),0x7000,matRotation * Matrix);
			//Environment.d3ddevice.Lights[0].Enabled = true;
			//Environment.d3ddevice.Lights[1].Enabled = false;
			Environment.d3ddevice.Lights[0].Update();
			Environment.d3ddevice.Lights[1].Update();
		}

		public override ChipType ChipType => ChipType.SystemGuide;

		//		public override string ToString(int tabs) {
		//			return "";		//	出力されない
		//		}
		//		public override string ToString() {
		//			return "";
		//		}

		public override HitStatus IsHit(int X, int Y, int ScrWidth, int ScrHeight) {
			HitStatus dist = new HitStatus();

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
				viewport, projMat, viewMat, matRotation * this.Matrix);
			Vector3 vFar = Vector3.Unproject(new Vector3(X, Y, viewport.MaxZ), 
				viewport, projMat, viewMat, matRotation * this.Matrix);
			Vector3 vDir = Vector3.Normalize(vFar - vNear);
			
			dist.distance = (mesh.mesh.Intersect(vNear, vDir, out sectinfo)) ? sectinfo.Dist : float.MaxValue; 

			if(dist.distance < float.MaxValue)dist.HitChip = this;


			return dist;
		

		}

		public override JointPosition JointPosition {
			get {
				return base.JointPosition;
			}
			set {
				base.JointPosition = value;
			}
		}



	}
}

