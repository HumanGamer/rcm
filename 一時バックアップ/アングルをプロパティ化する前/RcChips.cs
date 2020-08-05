using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using DX = Microsoft.DirectX;
using D3D = Microsoft.DirectX.Direct3D;

/* �R�����g�t���͖����� */

/* �����撣���ĕЂ��[����������Ă������� */

namespace RigidChips {
	public class RcChipCore		: RcChipBase{
		RcXFile mesh;
		public RcChipCore(RcData gen,RcChipBase parent,RcJointPosition pos) : base(gen,null,pos){
			this.matTransform = Matrix.RotationY((float)Math.PI);
			mesh = Generics.GetMesh("Core.x");
		}

		public override void Attach(RcChipBase to, RcJointPosition pos) {
			if(to == null)return;
			throw new Exception("Core�͑��̃`�b�v�̔h���`�b�v�ɂ͏o���܂���B");
		}

		public override string AttrTip(string AttrName) {
			return null;
		}

		public override void Detach() {
			return;
		}

		public override void DrawChip() {
			mesh.Draw(Generics.d3ddevice,this.ChipColor,/*DX.Matrix.RotationY((float)Math.PI) * */ Matrix);
		}

		public override string[] GetAttrList() {
			return null;
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

		public override RcAttrValue this[string AttrName] {
			get {
				throw new Exception("Core�ɂ͐ݒ�ł��鍀�ڂ͂���܂���B");
			}
			set {
				throw new Exception("Core�ɂ͐ݒ�ł��鍀�ڂ͂���܂���B");
			}
		}

		public override string ToString(int tabs,int prevDirection) {
			//	�e���v���Ƃ��Ă��g�p�\

			string s = "",st = "";
			for(int i = 0;i < tabs;i++)st += "\t";
			s += st;
			s += "Core(";	//	���`�b�v�̏o�͖�

			//	�����L�q�u���b�N
			if(ChipColor != Color.White)
				s += "Color=#" + ChipColor.R.ToString("X").PadLeft(2,'0') + ChipColor.G.ToString("X").PadLeft(2,'0') + ChipColor.B.ToString("X").PadLeft(2,'0') + ",";
			if(Name != null && Name != "")
				s += "Name=" + Name + ",";

			//	�����Ɋe�`�b�v�̑����L�q�̃R�[�h������
			s = s.TrimEnd(',');

			//	�h���u���b�N�擾
			s += "){";
			foreach(RcChipBase cb in this.Child)if(cb != null)
				s += "\n" + cb.ToString(tabs + 1,0);

			s += "\n" + st + "}";

			return s;
		}

		public override void UpdateMatrix() {
			this.matTransform = Matrix.RotationY((float)Math.PI);
		}

	}

	public class RcChipChip		: RcChipBase{
		protected RcAttrValue damper,spring;
		protected RcXFile mesh;
		public RcChipChip(RcData gen,RcChipBase parent,RcJointPosition pos) : base(gen,parent,pos){
			mesh = Generics.GetMesh("Chip.x");
			damper.Val = null;
			damper.Const = 0.5f;
			spring.Val = null;
			spring.Const = 0.5f;
		}

		public override string AttrTip(string AttrName) {
			switch(AttrName){
				case "Angle":
					return "�܂�Ȃ��p�x";
				case "Damper":
					return "�ڑ����̌���";
				case "Spring":
					return "�ڑ����̒e��";
			}

			return "";
		}

		public override void DrawChip() {
			mesh.Draw(Generics.d3ddevice,ChipColor,this.Matrix);
		}

		public override string[] GetAttrList() {
			string[] s = {"Angle","Damper","Spring"};
			return s;
		}

		public override string ToString(int tabs,int prevDirection) {
			string s = "",st = "";
			for(int i = 0;i < tabs;i++)st += "\t";
			s += st;
			//	�ڑ�����
			int b = (prevDirection + (byte)this.JointPosition) % 4; switch(b){
				case (byte)RcJointPosition.North:
					s += "N:";
					break;
				case (byte)RcJointPosition.South:
					s += "S:";
					break;
				case (byte)RcJointPosition.East:
					s += "E:";
					break;
				case (byte)RcJointPosition.West:
					s += "W:";
					break;
			}

			s += "Chip(";	//	���`�b�v�̏o�͖�

			//	�����L�q�u���b�N
			if(ChipColor != Color.White)
				s += "Color=#" + ChipColor.R.ToString("X").PadLeft(2,'0') + ChipColor.G.ToString("X").PadLeft(2,'0') + ChipColor.B.ToString("X").PadLeft(2,'0') + ",";
			if(angle.Val != null || angle.Const != 0f)
				s += "Angle=" + angle.ToString() + ",";
			if(Name != null && Name != "")
				s += "Name=" + Name + ",";
			if(damper.Val != null || damper.Const != 0.5f)
				s += "Damper=" + damper.ToString() + ",";
			if(spring.Val != null || spring.Const != 0.5f)
				s += "Spring=" + spring.ToString() + ",";

			s = s.TrimEnd(',');

			//	�h���u���b�N�擾
			s += "){";
			foreach(RcChipBase cb in this.Child)if(cb != null)
				s += "\n" + cb.ToString(tabs + 1,b);

			s += "\n" + st + "}";

			return s;
		}

		public override string ToString() {
			string str = "";
			str += "Chip(";

				if(Name != null && Name != "")
					str += "Name=" + Name + ",";
				if(angle.Val != null || angle.Const != 0f)
					str += "Angle=" + angle.ToString() + ",";
				if(this.ChipColor != Color.White)
					str += "Color=#" + ChipColor.R.ToString("X").PadLeft(2,'0') + ChipColor.G.ToString("X").PadLeft(2,'0') + ChipColor.B.ToString("X").PadLeft(2,'0') + ",";
				if(damper.Val != null || damper.Const != 0.5f)
					str += "Damper=" + damper.ToString() + ",";
				if(spring.Val != null || spring.Const != 0.5f)
					str += "Spring=" + spring.ToString() + ",";
			str.TrimEnd(',');
			str += ")";

			return str;

		}

		public override RcAttrValue this[string AttrName] {
			get {
				switch(AttrName){
					case "Angle":
						return this.angle;
					case "Damper":
						return this.damper;
					case "Spring":
						return this.spring;
				}
				throw new Exception("�w�肳�ꂽ���O�̑����͂���܂���B");
			}
			set {
				switch(AttrName){
					case "Angle":
						this.angle = value;
						return;
					case "Damper":
						this.damper = value;
						return;
					case "Spring":
						this.spring = value;
						return;
				}
				throw new Exception("�w�肳�ꂽ���O�̑����͂���܂���B");
			}
		}


	}

	public class RcChipFrame	: RcChipChip{
		bool option;
		RcXFile ghostmesh;
		public RcChipFrame(RcData gen,RcChipBase parent,RcJointPosition pos) : base(gen,parent,pos){
			mesh = Generics.GetMesh("Chip2.x");
			ghostmesh = Generics.GetMesh("Chip.x_");
			option = false;
		}

		public override void DrawChip() {
			if(!option)
				base.DrawChip ();
			else
				ghostmesh.Draw(Generics.d3ddevice,ChipColor,Matrix);
		}

		public override string AttrTip(string AttrName) {
			if(AttrName == "Option")
				return "0�ȊO�ŃS�[�X�g��";
			return base.AttrTip (AttrName);
		}

		public override string[] GetAttrList() {
			string[] s = base.GetAttrList ();
			s.CopyTo(s = new string[s.Length + 1],0);
			s[s.Length-1] = "Option";
			return s;
		}

		public override RcAttrValue this[string AttrName] {
			get {
				RcAttrValue v;
				if(AttrName == "Option"){
					v.Const = option ? 1 : 0;
					v.isNegative = false;
					v.Val = null;
					return v;
				}
				return base[AttrName];
			}
			set {
				if(AttrName == "Option")
					option = (value.Value() != 0f);
				else base[AttrName] = value;
			}
		}

		public override string ToString(int tabs,int prevDirection) {
			string s = "",st = "";
			for(int i = 0;i < tabs;i++)st += "\t";
			s += st;
			//	�ڑ�����
			int b = (prevDirection + (byte)this.JointPosition) % 4; switch(b){
				case (byte)RcJointPosition.North:
					s += "N:";
					break;
				case (byte)RcJointPosition.South:
					s += "S:";
					break;
				case (byte)RcJointPosition.East:
					s += "E:";
					break;
				case (byte)RcJointPosition.West:
					s += "W:";
					break;
			}

			s += "Frame(";	//	���`�b�v�̏o�͖�

			//	�����L�q�u���b�N
			if(ChipColor != Color.White)
				s += "Color=#" + ChipColor.R.ToString("X").PadLeft(2,'0') + ChipColor.G.ToString("X").PadLeft(2,'0') + ChipColor.B.ToString("X").PadLeft(2,'0') + ",";
			if(Name != null && Name != "")
				s += "Name=" + Name + ",";
			if(angle.Value() != 0 || angle.Val != null)
				s += "Angle=" + angle.ToString() + ",";
			if(damper.Val != null || damper.Const != 0.5f)
				s += "Damper=" + damper.ToString() + ",";
			if(spring.Val != null || spring.Const != 0.5f)
				s += "Spring=" + spring.ToString() + ",";
			if(option)
				s += "Option=1,";

			s = s.TrimEnd(',');

			//	�h���u���b�N�擾
			s += "){";
			foreach(RcChipBase cb in this.Child)if(cb != null)
				s += "\n" + cb.ToString(tabs + 1,b);

			s += "\n" + st + "}";

			return s;
		}


	}

	public class RcChipRudder	: RcChipChip{
		public RcChipRudder(RcData gen,RcChipBase parent,RcJointPosition pos) : base(gen,parent,pos){
			mesh = Generics.GetMesh("Rudder.x");
		}

		public override void DrawChip() {
			mesh.Draw(Generics.d3ddevice,ChipColor,DX.Matrix.RotationY((float)Math.PI) * this.Matrix);
		}

		public override string ToString(int tabs,int prevDirection) {
			string s = "",st = "";
			for(int i = 0;i < tabs;i++)st += "\t";
			s += st;
			//	�ڑ�����
			int b = (prevDirection + (byte)this.JointPosition) % 4; 
			switch(b){
				case (byte)RcJointPosition.North:
					s += "N:";
					break;
				case (byte)RcJointPosition.South:
					s += "S:";
					break;
				case (byte)RcJointPosition.East:
					s += "E:";
					break;
				case (byte)RcJointPosition.West:
					s += "W:";
					break;
			}

			s += "Rudder(";	//	���`�b�v�̏o�͖�

			//	�����L�q�u���b�N
			if(ChipColor != Color.White)
				s += "Color=#" + ChipColor.R.ToString("X").PadLeft(2,'0') + ChipColor.G.ToString("X").PadLeft(2,'0') + ChipColor.B.ToString("X").PadLeft(2,'0') + ",";
			if(angle.Val != null || angle.Const != 0f)
				s += "Angle=" + angle.ToString() + ",";
			if(Name != null && Name != "")
				s += "Name=" + Name + ",";
			if(damper.Val != null || damper.Const != 0.5f)
				s += "Damper=" + damper.ToString() + ",";
			if(spring.Val != null || spring.Const != 0.5f)
				s += "Spring=" + spring.ToString() + ",";

			s = s.TrimEnd(',');

			//	�h���u���b�N�擾
			s += "){";
			foreach(RcChipBase cb in this.Child)if(cb != null)
													s += "\n" + cb.ToString(tabs + 1,b);

			s += "\n" + st + "}";

			return s;
		}

		public override string ToString() {
			string str = "";
			str += "Rudder(";

				if(Name != null && Name != "")
					str += "Name=" + Name + ",";
				if(angle.Val != null || angle.Const != 0f)
					str += "Angle=" + angle.ToString() + ",";
				if(this.ChipColor != Color.White)
					str += "Color=#" + ChipColor.R.ToString("X").PadLeft(2,'0') + ChipColor.G.ToString("X").PadLeft(2,'0') + ChipColor.B.ToString("X").PadLeft(2,'0') + ",";
				if(damper.Val != null || damper.Const != 0.5f)
					str += "Damper=" + damper.ToString() + ",";
				if(spring.Val != null || spring.Const != 0.5f)
					str += "Spring=" + spring.ToString() + ",";
			str.TrimEnd(',');
			str += ")";

			return str;

		}

		public override void UpdateMatrix() {
			float a = angle.Value();
			matTransform =	  Matrix.Translation(0f,0f,-0.3f)
				* Matrix.RotationY((float)(a / 180f * Math.PI))
				* Matrix.Translation(0f,0f,-0.3f);

			switch(this.JointPosition){
				case RcJointPosition.North:
					break;
				case RcJointPosition.South:
					matTransform *= Matrix.RotationY((float)Math.PI);
					break;
				case RcJointPosition.East:
					matTransform *= Matrix.RotationY((float)(Math.PI * 0.5f));
					break;
				case RcJointPosition.West:
					matTransform *= Matrix.RotationY((float)(Math.PI * 1.5f));
					break;
			}
			matTransform *= Parent.Matrix;
			matVersion = System.DateTime.Now.Ticks;

			foreach(RcChipBase c in Child)if(c != null){
											  c.UpdateMatrix();
										  }
			
		}


	}

	public class RcChipRudderF	: RcChipRudder{
		bool option;
		RcXFile ghostmesh;
		public RcChipRudderF(RcData gen,RcChipBase parent,RcJointPosition pos) : base(gen,parent,pos){
			mesh = Generics.GetMesh("RudderF.x");
			ghostmesh = Generics.GetMesh("Chip.x_");
			option = false;
		}

		public override void DrawChip() {
			if(!option)
				base.DrawChip ();
			else
				ghostmesh.Draw(Generics.d3ddevice,ChipColor,Matrix);
		}

		public override string AttrTip(string AttrName) {
			if(AttrName == "Option")
				return "0�ȊO�ŃS�[�X�g��";
			return base.AttrTip (AttrName);
		}

		public override string[] GetAttrList() {
			string[] s = base.GetAttrList ();
			s.CopyTo(s = new string[s.Length + 1],0);
			s[s.Length-1] = "Option";
			return s;
		}

		public override RcAttrValue this[string AttrName] {
			get {
				RcAttrValue v;
				if(AttrName == "Option"){
					v.Const = option ? 1 : 0;
					v.isNegative = false;
					v.Val = null;
					return v;
				}
				return base[AttrName];
			}
			set {
				if(AttrName == "Option")
					option = (value.Value() != 0f);
				else base[AttrName] = value;
			}
		}

		public override string ToString(int tabs,int prevDirection) {
			string s = "",st = "";
			for(int i = 0;i < tabs;i++)st += "\t";
			s += st;
			//	�ڑ�����
			int b = (prevDirection + (byte)this.JointPosition) % 4; switch(b){
				case (byte)RcJointPosition.North:
					s += "N:";
					break;
				case (byte)RcJointPosition.South:
					s += "S:";
					break;
				case (byte)RcJointPosition.East:
					s += "E:";
					break;
				case (byte)RcJointPosition.West:
					s += "W:";
					break;
			}

			s += "RudderF(";	//	���`�b�v�̏o�͖�

			//	�����L�q�u���b�N
			if(ChipColor != Color.White)
				s += "Color=#" + ChipColor.R.ToString("X").PadLeft(2,'0') + ChipColor.G.ToString("X").PadLeft(2,'0') + ChipColor.B.ToString("X").PadLeft(2,'0') + ",";
			if(Name != null && Name != "")
				s += "Name=" + Name + ",";
			if(angle.Value() != 0 || angle.Val != null)
				s += "Angle=" + angle.ToString() + ",";
			if(damper.Val != null || damper.Const != 0.5f)
				s += "Damper=" + damper.ToString() + ",";
			if(spring.Val != null || spring.Const != 0.5f)
				s += "Spring=" + spring.ToString() + ",";
			if(option)
				s += "Option=1,";

			s = s.TrimEnd(',');

			//	�h���u���b�N�擾
			s += "){";
			foreach(RcChipBase cb in this.Child)if(cb != null)
				s += "\n" + cb.ToString(tabs + 1,b);

			s += "\n" + st + "}";

			return s;
		}

		public override string ToString() {
			string str = "";
			str += "RudderF(";

			if(Name != null && Name != "")
				str += "Name=" + Name + ",";
			if(angle.Val != null || angle.Const != 0f)
				str += "Angle=" + angle.ToString() + ",";
			if(this.ChipColor != Color.White)
				str += "Color=#" + ChipColor.R.ToString("X").PadLeft(2,'0') + ChipColor.G.ToString("X").PadLeft(2,'0') + ChipColor.B.ToString("X").PadLeft(2,'0') + ",";
			if(damper.Val != null || damper.Const != 0.5f)
				str += "Damper=" + damper.ToString() + ",";
			if(spring.Val != null || spring.Const != 0.5f)
				str += "Spring=" + spring.ToString() + ",";
			if(option)
				str += "Option=1,";
			str.TrimEnd(',');
			str += ")";

			return str;

		}

	}

	public class RcChipTrim		: RcChipChip{
		public RcChipTrim(RcData gen,RcChipBase parent,RcJointPosition pos) : base(gen,parent,pos){
			mesh = Generics.GetMesh("Trim.x");
		}

		public override void DrawChip() {
			mesh.Draw(Generics.d3ddevice,ChipColor,DX.Matrix.RotationY((float)Math.PI) * this.Matrix);
		}

		public override string ToString(int tabs,int prevDirection) {
			string s = "",st = "";
			for(int i = 0;i < tabs;i++)st += "\t";
			s += st;
			//	�ڑ�����
			int b = (prevDirection + (byte)this.JointPosition) % 4; 
			switch(b){
				case (byte)RcJointPosition.North:
					s += "N:";
					break;
				case (byte)RcJointPosition.South:
					s += "S:";
					break;
				case (byte)RcJointPosition.East:
					s += "E:";
					break;
				case (byte)RcJointPosition.West:
					s += "W:";
					break;
			}

			s += "Trim(";	//	���`�b�v�̏o�͖�

			//	�����L�q�u���b�N
			if(ChipColor != Color.White)
				s += "Color=#" + ChipColor.R.ToString("X").PadLeft(2,'0') + ChipColor.G.ToString("X").PadLeft(2,'0') + ChipColor.B.ToString("X").PadLeft(2,'0') + ",";
			if(angle.Val != null || angle.Const != 0f)
				s += "Angle=" + angle.ToString() + ",";
			if(Name != null && Name != "")
				s += "Name=" + Name + ",";
			if(damper.Val != null || damper.Const != 0.5f)
				s += "Damper=" + damper.ToString() + ",";
			if(spring.Val != null || spring.Const != 0.5f)
				s += "Spring=" + spring.ToString() + ",";

			s = s.TrimEnd(',');

			//	�h���u���b�N�擾
			s += "){";
			foreach(RcChipBase cb in this.Child)if(cb != null)
													s += "\n" + cb.ToString(tabs + 1,b);

			s += "\n" + st + "}";

			return s;
		}

		public override string ToString() {
			string str = "";
			str += "Trim(";

			if(Name != null && Name != "")
				str += "Name=" + Name + ",";
			if(angle.Val != null || angle.Const != 0f)
				str += "Angle=" + angle.ToString() + ",";
			if(this.ChipColor != Color.White)
				str += "Color=#" + ChipColor.R.ToString("X").PadLeft(2,'0') + ChipColor.G.ToString("X").PadLeft(2,'0') + ChipColor.B.ToString("X").PadLeft(2,'0') + ",";
			if(damper.Val != null || damper.Const != 0.5f)
				str += "Damper=" + damper.ToString() + ",";
			if(spring.Val != null || spring.Const != 0.5f)
				str += "Spring=" + spring.ToString() + ",";
			str.TrimEnd(',');
			str += ")";

			return str;

		}

		public override void UpdateMatrix() {
			float a = angle.Value();
			matTransform =	  Matrix.Translation(0f,0f,-0.3f)
				* Matrix.RotationZ((float)(a / 180f * Math.PI))
				* Matrix.Translation(0f,0f,-0.3f);

			switch(this.JointPosition){
				case RcJointPosition.North:
					break;
				case RcJointPosition.South:
					matTransform *= Matrix.RotationY((float)Math.PI);
					break;
				case RcJointPosition.East:
					matTransform *= Matrix.RotationY((float)(Math.PI * 0.5f));
					break;
				case RcJointPosition.West:
					matTransform *= Matrix.RotationY((float)(Math.PI * 1.5f));
					break;
			}
			matTransform *= Parent.Matrix;
			matVersion = System.DateTime.Now.Ticks;

			foreach(RcChipBase c in Child)if(c != null){
											  c.UpdateMatrix();
										  }
			
		}

	}

/**/public class RcChipTrimF	: RcChipTrim{
		bool option;
		RcXFile ghostmesh;
		public RcChipTrimF(RcData gen,RcChipBase parent,RcJointPosition pos) : base(gen,parent,pos){
			mesh = Generics.GetMesh("TrimF.x");
			ghostmesh = Generics.GetMesh("Chip.x_");
			option = false;
		}

		public override void DrawChip() {
			if(!option)
				base.DrawChip ();
			else
				ghostmesh.Draw(Generics.d3ddevice,ChipColor,Matrix);
		}

		public override string AttrTip(string AttrName) {
			if(AttrName == "Option")
				return "0�ȊO�ŃS�[�X�g��";
			return base.AttrTip (AttrName);
		}

		public override string[] GetAttrList() {
			string[] s = base.GetAttrList ();
			s.CopyTo(s = new string[s.Length + 1],0);
			s[s.Length-1] = "Option";
			return s;
		}

		public override RcAttrValue this[string AttrName] {
			get {
				RcAttrValue v;
				if(AttrName == "Option"){
					v.Const = option ? 1 : 0;
					v.isNegative = false;
					v.Val = null;
					return v;
				}
				return base[AttrName];
			}
			set {
				if(AttrName == "Option")
					option = (value.Value() != 0f);
				else base[AttrName] = value;
			}
		}

		public override string ToString(int tabs,int prevDirection) {
			string s = "",st = "";
			for(int i = 0;i < tabs;i++)st += "\t";
			s += st;
			//	�ڑ�����
			int b = (prevDirection + (byte)this.JointPosition) % 4; switch(b){
				case (byte)RcJointPosition.North:
					s += "N:";
					break;
				case (byte)RcJointPosition.South:
					s += "S:";
					break;
				case (byte)RcJointPosition.East:
					s += "E:";
					break;
				case (byte)RcJointPosition.West:
					s += "W:";
					break;
			}

			s += "TrimF(";	//	���`�b�v�̏o�͖�

			//	�����L�q�u���b�N
			if(ChipColor != Color.White)
				s += "Color=#" + ChipColor.R.ToString("X").PadLeft(2,'0') + ChipColor.G.ToString("X").PadLeft(2,'0') + ChipColor.B.ToString("X").PadLeft(2,'0') + ",";
			if(Name != null && Name != "")
				s += "Name=" + Name + ",";
			if(angle.Value() != 0 || angle.Val != null)
				s += "Angle=" + angle.ToString() + ",";
			if(damper.Val != null || damper.Const != 0.5f)
				s += "Damper=" + damper.ToString() + ",";
			if(spring.Val != null || spring.Const != 0.5f)
				s += "Spring=" + spring.ToString() + ",";
			if(option)
				s += "Option=1,";

			s = s.TrimEnd(',');

			//	�h���u���b�N�擾
			s += "){";
			foreach(RcChipBase cb in this.Child)if(cb != null)
				s += "\n" + cb.ToString(tabs + 1,b);

			s += "\n" + st + "}";

			return s;
		}

		public override string ToString() {
			string str = "";
			str += "TrimF(";

			if(Name != null && Name != "")
				str += "Name=" + Name + ",";
			if(angle.Val != null || angle.Const != 0f)
				str += "Angle=" + angle.ToString() + ",";
			if(this.ChipColor != Color.White)
				str += "Color=#" + ChipColor.R.ToString("X").PadLeft(2,'0') + ChipColor.G.ToString("X").PadLeft(2,'0') + ChipColor.B.ToString("X").PadLeft(2,'0') + ",";
			if(damper.Val != null || damper.Const != 0.5f)
				str += "Damper=" + damper.ToString() + ",";
			if(spring.Val != null || spring.Const != 0.5f)
				str += "Spring=" + spring.ToString() + ",";
			if(option)
				str += "Option=1,";
			str.TrimEnd(',');
			str += ")";

			return str;

		}

	}

	public class RcChipJet		: RcChipBase{
		RcXFile jet,baloon;
		RcAttrValue power,damper,spring,effect,option;
		public RcChipJet(RcData gen,RcChipBase parent,RcJointPosition pos) : base(gen,parent,pos){
			jet = Generics.GetMesh("Jet.x");
			baloon = Generics.GetMesh("Jet2.x");
			damper.Val = null;
			damper.Const = 0.5f;
			spring.Val = null;
			spring.Const = 0.5f;
			effect.Const = 0f;
		}

		public override string AttrTip(string AttrName) {
			switch(AttrName){
				case "Angle":
					return "�܂�Ȃ��p�x";
				case "Damper":
					return "�ڑ����̌���";
				case "Spring":
					return "�ڑ����̒e��";
				case "Power":
					return "�W�F�b�g�o��:�o���[���K�X��";
				case "Option":
					return "0:�W�F�b�g 1:���f�o���[�� 2:��C�o���[��";
				case "Effect":
					return "1-4:�X���[�N���o��(�W�F�b�g���̂�)";
			}

			return "";

		}

		public override void DrawChip() {
			float size = (float)Math.Pow(power.Value(),0.3);
			if(option.Const > 0f)
				baloon.Draw(Generics.d3ddevice,ChipColor,DX.Matrix.Scaling(size + 1f,size + 1f,size + 1f) * Matrix);
			else
				jet.Draw(Generics.d3ddevice,ChipColor,Matrix);
		}

		public override string[] GetAttrList() {
			string[] s = {"Angle","Damper","Spring","Power","Option","Effect"};
			return s;
		}

		public override RcAttrValue this[string AttrName] {
			get {
				switch(AttrName){
					case "Angle":
						return this.angle;
					case "Damper":
						return this.damper;
					case "Spring":
						return this.spring;
					case "Power":
						return this.power;
					case "Option":
						return this.option;
					case "Effect":
						return this.effect;
				}
				throw new Exception("�w�肳�ꂽ���O�̑����͂���܂���B");
			}
			set {
				switch(AttrName){
					case "Angle":
						this.angle = value;
						return;
					case "Damper":
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
				}
				throw new Exception("�w�肳�ꂽ���O�̑����͂���܂���B");
			}
		}

		public override string ToString(int tabs,int prevDirection) {
			string s = "",st = "";
			for(int i = 0;i < tabs;i++)st += "\t";
			s += st;
			//	�ڑ�����
			int b = (prevDirection + (byte)this.JointPosition) % 4; switch(b){
				case (byte)RcJointPosition.North:
					s += "N:";
					break;
				case (byte)RcJointPosition.South:
					s += "S:";
					break;
				case (byte)RcJointPosition.East:
					s += "E:";
					break;
				case (byte)RcJointPosition.West:
					s += "W:";
					break;
			}

			s += "Jet(";	//	���`�b�v�̏o�͖�

			//	�����L�q�u���b�N
			if(ChipColor != Color.White)
				s += "Color=#" + ChipColor.R.ToString("X").PadLeft(2,'0') + ChipColor.G.ToString("X").PadLeft(2,'0') + ChipColor.B.ToString("X").PadLeft(2,'0') + ",";
			if(Name != null && Name != "")
				s += "Name=" + Name + ",";
			if(angle.Value() != 0 || angle.Val != null)
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

			s = s.TrimEnd(',');

			//	�h���u���b�N�擾
			s += "){";
			foreach(RcChipBase cb in this.Child)if(cb != null)
													s += "\n" + cb.ToString(tabs + 1,b);

			s += "\n" + st + "}";

			return s;
		}

		public override string ToString() {
			string s = "";
			s += "Jet(";	//	���`�b�v�̏o�͖�

			//	�����L�q�u���b�N
			if(ChipColor != Color.White)
				s += "Color=#" + ChipColor.R.ToString("X").PadLeft(2,'0') + ChipColor.G.ToString("X").PadLeft(2,'0') + ChipColor.B.ToString("X").PadLeft(2,'0') + ",";
			if(Name != null && Name != "")
				s += "Name=" + Name + ",";
			if(angle.Value() != 0 || angle.Val != null)
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

			s = s.TrimEnd(',');

			s += ")";

			return s;
		}

		public override RcHitStatus IsHit(int X, int Y, int ScrWidth, int ScrHeight) {
			RcHitStatus dist ,buff;
			dist.distance = float.MaxValue;
			dist.HitChip = null;
			foreach(RcChipBase c in Child){
				if(c != null){
					buff = c.IsHit(X,Y,ScrWidth,ScrHeight);
					if(dist.distance > buff.distance){
						dist = buff;
					}
				}
			}
			// ���e�s��
			Matrix projMat = Generics.d3ddevice.Transform.Projection;
			// �r���[�s��
			Matrix viewMat = Generics.d3ddevice.Transform.View;
			// �r���[�|�[�g
			Viewport viewport = new Viewport();

			IntersectInformation sectinfo = new IntersectInformation();

			viewport.Width = ScrWidth; 
			viewport.Height = ScrHeight; 
			viewport.X = viewport.Y = 0;
			viewport.MaxZ = 1.0f; 
			viewport.MinZ = 0.0f;

			// �N���b�N�����X�N���[�����W���烌�C���v�Z���A�Ώۃ��b�V���Ƃ̌������`�F�b�N 
			Vector3 vNear = Vector3.Unproject(new Vector3(X, Y, viewport.MinZ), 
				viewport, projMat, viewMat, this.Matrix /* ������World�s�� */);
			Vector3 vFar = Vector3.Unproject(new Vector3(X, Y, viewport.MaxZ), 
				viewport, projMat, viewMat, this.Matrix /* ������World�s�� */);
			Vector3 vDir = Vector3.Normalize(vFar - vNear);
			
			if(option.Value() != 0f)
				buff.distance = (baloon.mesh.Intersect(vNear, vDir, ref sectinfo)) ? sectinfo.Dist : float.MaxValue; 
			else
				buff.distance = (Generics.imesh.Intersect(vNear, vDir, ref sectinfo)) ? sectinfo.Dist : float.MaxValue; 

			if(dist.distance > buff.distance){
				dist.distance = buff.distance;
				dist.HitChip = this;
			}

			return dist;
		}


	}

/**/public class RcChipWheel	: RcChipBase{
		public RcChipWheel(RcData gen,RcChipBase parent,RcJointPosition pos) : base(gen,parent,pos){
		}
	}

/**/public class RcChipRLW		: RcChipWheel{
		public RcChipRLW(RcData gen,RcChipBase parent,RcJointPosition pos) : base(gen,parent,pos){
		}
	}

/**/public class RcChipWeight	: RcChipChip{
		public RcChipWeight(RcData gen,RcChipBase parent,RcJointPosition pos) : base(gen,parent,pos){
			mesh = Generics.GetMesh("ChipH.x");
		}

		public override string ToString(int tabs,int prevDirection) {
			string s = "",st = "";
			for(int i = 0;i < tabs;i++)st += "\t";
			s += st;
			//	�ڑ�����
			int b = (prevDirection + (byte)this.JointPosition) % 4; switch(b){
																		case (byte)RcJointPosition.North:
																			s += "N:";
																			break;
																		case (byte)RcJointPosition.South:
																			s += "S:";
																			break;
																		case (byte)RcJointPosition.East:
																			s += "E:";
																			break;
																		case (byte)RcJointPosition.West:
																			s += "W:";
																			break;
																	}

			s += "Weight(";	//	���`�b�v�̏o�͖�

			//	�����L�q�u���b�N
			if(ChipColor != Color.White)
				s += "Color=#" + ChipColor.R.ToString("X").PadLeft(2,'0') + ChipColor.G.ToString("X").PadLeft(2,'0') + ChipColor.B.ToString("X").PadLeft(2,'0') + ",";
			if(angle.Val != null || angle.Const != 0f)
				s += "Angle=" + angle.ToString() + ",";
			if(Name != null && Name != "")
				s += "Name=" + Name + ",";
			if(damper.Val != null || damper.Const != 0.5f)
				s += "Damper=" + damper.ToString() + ",";
			if(spring.Val != null || spring.Const != 0.5f)
				s += "Spring=" + spring.ToString() + ",";

			s = s.TrimEnd(',');

			//	�h���u���b�N�擾
			s += "){";
			foreach(RcChipBase cb in this.Child)if(cb != null)
													s += "\n" + cb.ToString(tabs + 1,b);

			s += "\n" + st + "}";

			return s;
		}

		public override string ToString() {
			string str = "";
			str += "Weight(";

			if(Name != null && Name != "")
				str += "Name=" + Name + ",";
			if(angle.Val != null || angle.Const != 0f)
				str += "Angle=" + angle.ToString() + ",";
			if(this.ChipColor != Color.White)
				str += "Color=#" + ChipColor.R.ToString("X").PadLeft(2,'0') + ChipColor.G.ToString("X").PadLeft(2,'0') + ChipColor.B.ToString("X").PadLeft(2,'0') + ",";
			if(damper.Val != null || damper.Const != 0.5f)
				str += "Damper=" + damper.ToString() + ",";
			if(spring.Val != null || spring.Const != 0.5f)
				str += "Spring=" + spring.ToString() + ",";
			str.TrimEnd(',');
			str += ")";

			return str;

		}


	}

/**/public class RcChipCowl		: RcChipBase{
		RcXFile[] meshes;
		int option;
		public RcChipCowl(RcData gen,RcChipBase parent,RcJointPosition pos) : base(gen,parent,pos){
			meshes = new RcXFile[6];
			meshes[0] = Generics.GetMesh("Type0.x");
			meshes[1] = Generics.GetMesh("Type1.x");
			meshes[2] = Generics.GetMesh("Type2.x");
			meshes[3] = Generics.GetMesh("Type3.x");
			meshes[4] = Generics.GetMesh("Type4.x");
			meshes[5] = Generics.GetMesh("Type5.x");
			option = 0;
		}

		public override void Add(RcJointPosition joint, RcChipBase chip) {
			if(!(chip is RcChipCowl)){
				throw new Exception("�J�E���ɂ̓J�E�������ڑ��ł��܂���B");
			}
			base.Add(joint,chip);
		}

		public override string AttrTip(string AttrName) {
			if(AttrName == "Angle")
				return "�܂�Ȃ��p�x";
			else if(AttrName == "Option")
				return "�`��\n1:�g 2:�~ 3,4:���p�O�p�` 5:���~ ��:�l�p";
			throw new Exception("�w�肳�ꂽ�����͑��݂��܂���B");
		}

		public override void DrawChip() {
			meshes[(option < 0 || option > 5) ? 0 : option].Draw(Generics.d3ddevice,ChipColor,DX.Matrix.RotationY((float)Math.PI) * this.Matrix);
		}

		public override string[] GetAttrList() {
			string[] s = {"Angle","Option"};
			return s;
		}

		public override RcAttrValue this[string AttrName] {
			get {
				RcAttrValue temp = new RcAttrValue();
				if(AttrName == "Angle"){
					return angle;
				}
				else if(AttrName == "Option"){
					temp.Const = (float)option;
					return temp;
				}
				throw new Exception("�w�肳�ꂽ�����͑��݂��܂���B");
			}
			set {
				if(AttrName == "Angle"){
					angle = value;
					return;
				}
				else if(AttrName == "Option"){
					option = (int)value.Const;
					return;
				}
				throw new Exception("�w�肳�ꂽ�����͑��݂��܂���B");
			}
		}

		public override string ToString(int tabs,int prevDirection) {
			string s = "",st = "";
			for(int i = 0;i < tabs;i++)st += "\t";
			s += st;
			//	�ڑ�����
			int b = (prevDirection + (byte)this.JointPosition) % 4; switch(b){
																		case (byte)RcJointPosition.North:
																			s += "N:";
																			break;
																		case (byte)RcJointPosition.South:
																			s += "S:";
																			break;
																		case (byte)RcJointPosition.East:
																			s += "E:";
																			break;
																		case (byte)RcJointPosition.West:
																			s += "W:";
																			break;
																	}

			s += "Cowl(";	//	���`�b�v�̏o�͖�

			//	�����L�q�u���b�N
			if(ChipColor != Color.White)
				s += "Color=#" + ChipColor.R.ToString("X").PadLeft(2,'0') + ChipColor.G.ToString("X").PadLeft(2,'0') + ChipColor.B.ToString("X").PadLeft(2,'0') + ",";
			if(angle.Val != null || angle.Const != 0f)
				s += "Angle=" + angle.ToString() + ",";
			if(Name != null && Name != "")
				s += "Name=" + Name + ",";
			if(option > 1 || option < 5){
				s += "Option=" + option + ",";
			}

			s = s.TrimEnd(',');

			//	�h���u���b�N�擾
			s += "){";
			foreach(RcChipBase cb in this.Child)if(cb != null)
													s += "\n" + cb.ToString(tabs + 1,b);

			s += "\n" + st + "}";

			return s;
		}

		public override string ToString() {
			string str = "";
			str += "Cowl(";	//	���`�b�v�̏o�͖�

			//	�����L�q�u���b�N
			if(ChipColor != Color.White)
				str += "Color=#" + ChipColor.R.ToString("X").PadLeft(2,'0') + ChipColor.G.ToString("X").PadLeft(2,'0') + ChipColor.B.ToString("X").PadLeft(2,'0') + ",";
			if(angle.Val != null || angle.Const != 0f)
				str += "Angle=" + angle.ToString() + ",";
			if(Name != null && Name != "")
				str += "Name=" + Name + ",";
			if(option > 1 || option < 5){
				str += "Option=" + option + ",";
			}

			str.TrimEnd(',');
			str += ")";

			return str;

		}

	}

/**/public class RcChipArm		: RcChipBase{
		public RcChipArm(RcData gen,RcChipBase parent,RcJointPosition pos) : base(gen,parent,pos){
		}
	}

	//	------------------------------------------------------------------------�ȉ��A�V�X�e���p

	public class RcChipCursor : RcChipBase{
		RcXFile mesh;

		public RcChipCursor(RcData gen,RcChipBase parent,RcJointPosition pos) : base(gen,parent,pos){
			this.Attach(parent,RcJointPosition.NULL);
			mesh = Generics.GetMesh("Cursor2.x");
			ChipColor = Color.FromArgb(255,0,0);
		}

		public override void Add(RcJointPosition joint, RcChipBase chip) {
			if(chip is RcChipGuide){
				base.Add(joint,chip);
				return;
			}
			throw new Exception("RcChipCursor��Add���s���Ȍ`�ŌĂяo����܂����B");
		}

		public override void Attach(RcChipBase to, RcJointPosition pos) {
			this.Parent = to;
			UpdateMatrix();
		}


		public override string AttrTip(string AttrName) {
			return "!RcChipCursor has no attributes.";
		}

		public override void DrawChip() {
			if(Parent == null)return;
			mesh.Draw(Generics.d3ddevice,Color.FromArgb(127,ChipColor),matTransform);
		}

		public override string[] GetAttrList() {
			return null;
		}

		public override void UpdateMatrix() {
			if(Parent == null)return;
			this.matTransform = Parent.Matrix;
			this.matVersion = System.DateTime.Now.Ticks;
		}

		public override string ToString(int tabs,int prevDirection){
			string s = "";
			for(int i = 0;i < tabs;i++)s += "\t";
			return s + "//C:Guide(){}\n";		//	�h���`�b�v�͎����Ȃ�
		}

		public override string ToString() {
			return "//" + "Guide()";
		}

		public override RcAttrValue this[string AttrName] {
			get {
				throw new Exception("RcChipCursor�͑����l�������܂���B\nRcAttrValue�v���p�e�B�͎g�p�ł��܂���B");
			}
			set {
				throw new Exception("RcChipCursor�͑����l�������܂���B\nRcAttrValue�v���p�e�B�͎g�p�ł��܂���B");
			}
		}

	}

	public class RcChipGuide : RcChipBase{
		RcXFile mesh;
		public RcChipGuide(RcData gen,RcChipBase parent,RcJointPosition pos) : base(gen,parent,pos){
			mesh = Generics.GetMesh("guide.x");
		}

		public override void Add(RcJointPosition joint, RcChipBase chip) {
			throw new Exception("RcChipGuide�ɁAAdd()�͖����ł��B");
		}

		public override string AttrTip(string AttrName) {
			return "!RcChipGuide has no attributes.";
		}

		public override void DrawChip() {
			if(Parent.Parent == null)return;
			switch(this.JointPosition){
				case RcJointPosition.North:
					ChipColor = Color.Blue;
					break;
				case RcJointPosition.South:
					ChipColor = Color.Yellow;
					break;
				case RcJointPosition.East:
					ChipColor = Color.Red;
					break;
				case RcJointPosition.West:
					ChipColor = Color.Green;
					break;
				default:
					ChipColor = Color.LightGray;
					break;
			}
			ChipColor = Color.FromArgb(0x7F,ChipColor);
			mesh.Draw(Generics.d3ddevice,ChipColor,this.Matrix);
		}

		public override string[] GetAttrList() {
			return null;
		}

		public override RcAttrValue this[string AttrName] {
			get {
				throw new Exception("RcChipGuide�͑����l�������܂���B\nRcAttrValue�v���p�e�B�͎g�p�ł��܂���B");
			}
			set {
				throw new Exception("RcChipGuide�͑����l�������܂���B\nRcAttrValue�v���p�e�B�͎g�p�ł��܂���B");
			}
		}

		public override string ToString(int tabs,int prevDirection) {
			return "";		//	�o�͂���Ȃ�
		}
		public override string ToString() {
			return "";
		}

		public override RcHitStatus IsHit(int X, int Y, int ScrWidth, int ScrHeight) {
			RcHitStatus dist ,buff;
			dist.distance = float.MaxValue;
			dist.HitChip = null;
			foreach(RcChipBase c in Child){
				if(c != null){
					buff = c.IsHit(X,Y,ScrWidth,ScrHeight);
					if(dist.distance > buff.distance){
						dist = buff;
					}
				}
			}
			// ���e�s��
			Matrix projMat = Generics.d3ddevice.Transform.Projection;
			// �r���[�s��
			Matrix viewMat = Generics.d3ddevice.Transform.View;
			// �r���[�|�[�g
			Viewport viewport = new Viewport();

			IntersectInformation sectinfo = new IntersectInformation();

			viewport.Width = ScrWidth; 
			viewport.Height = ScrHeight; 
			viewport.X = viewport.Y = 0;
			viewport.MaxZ = 1.0f; 
			viewport.MinZ = 0.0f;

			// �N���b�N�����X�N���[�����W���烌�C���v�Z���A�Ώۃ��b�V���Ƃ̌������`�F�b�N 
			Vector3 vNear = Vector3.Unproject(new Vector3(X, Y, viewport.MinZ), 
				viewport, projMat, viewMat, this.Matrix /* ������World�s�� */);
			Vector3 vFar = Vector3.Unproject(new Vector3(X, Y, viewport.MaxZ), 
				viewport, projMat, viewMat, this.Matrix /* ������World�s�� */);
			Vector3 vDir = Vector3.Normalize(vFar - vNear);
			
			buff.distance = (mesh.mesh.Intersect(vNear, vDir, ref sectinfo)) ? sectinfo.Dist : float.MaxValue; 

			

			if(dist.distance > buff.distance){
				dist.distance = buff.distance;
				dist.HitChip = this;
			}

			return dist;
		

		}


	}

}
