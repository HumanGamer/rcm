using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace RigidChips {

	//	TODO: �`�b�v�x�[�X�N���X�����A���ꂩ��e��`�b�v��h�������邱��
	//	TODO: ���ۂɎ����I�Ƀ`�b�v��g�ݗ��āA�\���ł��邩�ǂ���������

	//----------------------------------------------------------------------------------//
	public class RcChipBase{
		public RcData Generics;
		public string Name;
		protected RcAttrValue angle;
//		public RcChipType refFormat;
		public Color ChipColor;
		public RcJointPosition JointPosition;
		public RcChipBase Parent;
		public RcChipBase[] Child;

		protected Matrix matTransform;
		protected long matVersion;

		/// <summary>
		/// �`�b�v�̊�{�I�ȏ��������s���B
		/// </summary>
		/// <param name="gen">�������� RcData �C���X�^���X</param>
		/// <param name="parent">�ڑ������`�b�v</param>
		public RcChipBase(RcData gen,RcChipBase parent,RcJointPosition pos){
			Child = new RcChipBase[12];	//	�d�l���ς���Ă��Ȃ���΂ЂƂ̃`�b�v�ɂ�����`�b�v�̍ő吔��12
			Generics = gen;
			Attach(parent,pos);
			angle.Const = 0f;
			angle.Val = null;
			angle.isNegative = false;
			this.ChipColor = Color.White;
		}
		/// <summary>
		/// ���[���h���W�ϊ��s����X�V����B
		/// </summary>
		public virtual void UpdateMatrix(){
			float a = angle.Value();
			matTransform =	  Matrix.Translation(0f,0f,-0.3f)
							* Matrix.RotationX((float)(-a / 180f * Math.PI))
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

		/// <summary>
		/// �\����̈ʒu��\���s��
		/// </summary>
		public virtual Matrix Matrix{
			get{
				if(this.Parent.MatrixVersion > matVersion){
					this.UpdateMatrix();
				}

				return matTransform;
			}
		}

		/// <summary>
		/// �s��̃o�[�W����
		/// </summary>
		public virtual long MatrixVersion{
			get{
				return matVersion;
			}
		}

		/// <summary>
		/// �q�`�b�v��ǉ�����B���łɍő吔�ڑ�����Ă���Ƃ��ɂ� Exception ����������B
		/// </summary>
		/// <param name="joint">�ǂ��ɒǉ����邩�� RcJointPosition �萔</param>
		/// <param name="chip">�ǉ�����`�b�v</param>
		public virtual void Add(RcJointPosition joint,RcChipBase chip){
			for(int i = 0;i < 12;i++){
				if(Child[i] == null){
					Child[i] = chip;
					Child[i].JointPosition = joint;
					Child[i].Parent = this;
					Child[i].UpdateMatrix();
					Generics.RegisterChipAll(chip);
					return;
				}
			}
			throw new Exception("����ȏ�q�`�b�v���i�[�ł��܂���B�ЂƂ̃`�b�v�Ɏ��t������`�b�v�̐���12�܂łł��B");
		}

		/// <summary>
		/// ���łɂ��Ă���`�b�v�����O���B���݂��Ȃ��`�b�v���w�肵���Ƃ��ɂ� Exception ����������B
		/// </summary>
		/// <param name="chip">���O�������`�b�v�B</param>
		public virtual void Remove(RcChipBase chip){
			bool Removed = false;
			for(int i = 0; i < 12;i++){
				if(Removed){
					Child[i-1] = Child[i];
				}
				else if(Child[i] == chip){
					chip.JointPosition = RcJointPosition.NULL;
					Generics.UnregisterChipAll(chip);
					Removed = true;
				}
			}
			if(Removed)
				return;
			else
				throw new Exception("�w�肳�ꂽ�`�b�v�͌�����܂���ł����B");
		}

		/// <summary>
		/// ���̃`�b�v�𑼂̃`�b�v�ɐڑ�����B
		/// </summary>
		/// <param name="to">�ڑ������`�b�v�B</param>
		/// <param name="pos">�ڑ��ʒu�B</param>
		public virtual void Attach(RcChipBase to,RcJointPosition pos){
				to.Add(pos,this);
		}

		/// <summary>
		/// ���ݐڑ�����Ă���`�b�v������O���B
		/// </summary>
		public virtual void Detach(){
				this.Parent.Remove(this);
		}

		/// <summary>
		/// �`�b�v����ʂɕ`�悷��B
		/// </summary>
		public virtual void DrawChip(){
			MessageBox.Show("RcChipBase::DrawChip() Was Called.");
		}

		/// <summary>
		/// �g�p�\�ȑ����̖��O�̔z��𓾂�B
		/// </summary>
		/// <returns>�������̔z��B</returns>
		public virtual string[] GetAttrList(){
			return null;
		}

		/// <summary>
		/// �w�肵�����O�̑����̐������𓾂�B
		/// </summary>
		/// <param name="AttrName">�g�p�\�ȑ������B</param>
		/// <returns>�w�肵�������̐������B</returns>
		public virtual string AttrTip(string AttrName){
			return null;
		}

		/// <summary>
		/// �w�肵�������̒l��ݒ�E�擾����B
		/// </summary>
		public virtual RcAttrValue this[string AttrName]{
			set{
			}

			get{
				return new RcAttrValue();
			}
		}

		/// <summary>
		/// ���̃`�b�v�̏��̕�����𐶐�����B
		/// </summary>
		/// <returns>���̃`�b�v�̕�������B</returns>
		public override string ToString(){
			string str = "";
			str += "Dummy(";

				if(Name != "")str += "Name=" + Name + ",";
				if(JointPosition != RcJointPosition.NULL)str += "Angle=" + angle.ToString() + ",";
				if(this.ChipColor != Color.White)
					str += "Color=#" + ChipColor.R.ToString("X") + ChipColor.G.ToString("X") + ChipColor.B.ToString("X");
			str.TrimEnd(',');
			str += ")";

			return str;
		}

		/// <summary>
		/// ���̃`�b�v�Ƃ��̔h���ɂ��Ă̊��S�\��������𓾂�B
		/// </summary>
		/// <param name="tabs">�C���f���g�̃^�u��</param>
		/// <param name="prevDirection">���O�̃`�b�v�̕��p</param>
		/// <returns>.rcd���Ɏg�p�\�� Body �u���b�N�p������</returns>
		public virtual string ToString(int tabs,int prevDirection) {
			return base.ToString ();	//	������
		}

		/// <summary>
		///	���̃`�b�v�y�єh���`�b�v�̑����B�ǂݎ���p�B
		/// </summary>
		public int ChipCount{
			get{
				int c = 1;
				foreach(RcChipBase cb in Child)if(cb != null){
					c += cb.ChipCount;
				}
				return c;
			}
		}

		/// <summary>
		/// ���̃��f�����I�����ꂽ���ǂ����𓾂�B
		/// </summary>
		/// <param name="X">�}�E�X�J�[�\����X���W</param>
		/// <param name="Y">�}�E�X�J�[�\����Y���W</param>
		/// <param name="ScrWidth">�X�N���[���̕�</param>
		/// <param name="ScrHeight">�X�N���[���̍���</param>
		/// <returns>�I�����ꂽ�`�b�v�Ƌ����̏��</returns>
		public virtual RcHitStatus IsHit(int X,int Y,int ScrWidth,int ScrHeight){
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
			
			buff.distance = (Generics.imesh.Intersect(vNear, vDir, ref sectinfo)) ? sectinfo.Dist : float.MaxValue; 
			if(dist.distance > buff.distance){
				dist.distance = buff.distance;
				dist.HitChip = this;
			}

			return dist;
		}

		/// <summary>
		/// �w�肳�ꂽ���΃C���f�b�N�X�l�̃`�b�v�𓾂�B
		/// </summary>
		/// <param name="idx">�擾����`�b�v�̃C���f�b�N�X�B���̒l�͕ύX�����B</param>
		/// <returns>�v�����ꂽ�C���f�b�N�X�̃`�b�v</returns>
		public RcChipBase GetChildChip(ref int idx){
			RcChipBase buff;
			if(idx == 0)
				return this;
			else{
				idx--;
				foreach(RcChipBase cld in Child)if(cld != null){
					buff = cld.GetChildChip(ref idx);
					if(buff != null) return buff;
				}
			}
			return null;
		}

		/// <summary>
		/// ���̃`�b�v�Ƃ��̔h���`�b�v�S�Ă�`�悷��B
		/// </summary>
		public void DrawChipAll(){
			DrawChip();
			foreach(RcChipBase cld in Child)if(cld != null)
				cld.DrawChipAll();
		}

		public virtual byte GetRealAngle(){
			byte b;
			if(Parent != null)
				b = Parent.GetRealAngle();
			else
				b = 0;

			return (byte)((b + (int)this.JointPosition) % 4);
		}
	}

	public class RcData{
		public const int MaxChipCount = 512;
		public const int ChipMeshCount = 64;
		public const int KeyCount = 17;

		public Device d3ddevice;
		public RcModel model;
		public RcValList vals;
		public RcKeyList keys;
		public Mesh imesh;

		public int chipCount;
		RcXFile[] meshes;
		public RcChipCursor Cursor;
		RcChipGuide[] Guides;	//	�ʒu�\���p�̉��`�b�v

		RcChipBase[] ChipLib;

		/// <summary>
		/// RcData �C���X�^���X������������B
		/// </summary>
		/// <param name="D3DDevice">�\���Ȃǂɗp����Microsoft.DirectX.Direct3D.Device �C���X�^���X�B</param>
		public RcData(Device D3DDevice){

			//	�e�C���X�^���X�̏�����
			d3ddevice = D3DDevice;

			vals = new RcValList();

			keys = new RcKeyList(KeyCount);

			meshes = new RcXFile[ChipMeshCount];

			model = new RcModel(this);

			ChipLib = new RcChipBase[MaxChipCount];
			
			chipCount = 1;

			Guides = new RcChipGuide[4];
			Cursor = new RcChipCursor(this,model.root,RcJointPosition.NULL);
			Guides[0] = new RcChipGuide(this,Cursor,RcJointPosition.North);
			Guides[1] = new RcChipGuide(this,Cursor,RcJointPosition.South);
			Guides[2] = new RcChipGuide(this,Cursor,RcJointPosition.East);
			Guides[3] = new RcChipGuide(this,Cursor,RcJointPosition.West);

			ExtendedMaterial[] matbuff;
			imesh = Mesh.FromFile("Chip.x",MeshFlags.SystemMemory,d3ddevice,out matbuff);
		}

		/// <summary>
		/// �`�b�v�\���pRcXFile�N���X�ւ̎Q�Ƃ𓾂�B
		/// </summary>
		/// <param name="FileName">.x �t�@�C���̃p�X</param>
		/// <returns>���[�h����RcXFile�ւ̎Q��</returns>
		public RcXFile GetMesh(string FileName){
			for(int i = 0;i < ChipMeshCount;i++){
				if(meshes[i] == null){
					meshes[i] = new RcXFile();
					meshes[i].FileName = FileName;
					meshes[i].Load(this.d3ddevice);
					return meshes[i];
				}else if(meshes[i].FileName == FileName)
					return meshes[i];
			}
			return null;
		}

		public void DisposeMeshes(){
			for(int i = 0;i < ChipMeshCount;i++){
				if(meshes[i] != null && meshes[i].FileName != ""){
					if(meshes[i].texture != null){
						meshes[i].texture.Dispose();
						meshes[i].texture = null;
					}
					if(meshes[i].mesh != null){
						meshes[i].mesh.Dispose();
						meshes[i].mesh = null;
					}
				}
			}

			imesh.Dispose();
		}

		public void ReloadMeshes(){
			for(int i = 0;i < ChipMeshCount;i++){
				if(meshes[i] != null && meshes[i].FileName != ""){
					meshes[i].Load(this.d3ddevice);
				}
			}
			ExtendedMaterial[] matbuff;
			imesh = Mesh.FromFile("Chip.x",MeshFlags.SystemMemory,d3ddevice,out matbuff);
		}

		public void SetCursor(RcChipBase Target){
			Cursor.Attach(Target,RcJointPosition.NULL);
		}

		public void DrawCursor(){
			Cursor.DrawChipAll();
		}

		public void DrawCursor(Color CursorColor){
			Cursor.ChipColor = CursorColor;
			Cursor.DrawChipAll();
		}

		public int RegisterChip(RcChipBase c){
			for(int i = 0;i < MaxChipCount;i++){
				if(ChipLib[i] == null){
					ChipLib[i] = c;
					return i;
				}
				else if(ChipLib[i] == c)
					return i;
			}
			throw new Exception("�`�b�v�ő吔(512)�𒴂��܂����B");
		}

		public void UnregisterChip(RcChipBase c){
			for(int i = 0;i < MaxChipCount;i++){
				if(ChipLib[i] == c){
					ChipLib[i] = null;
					return;
				}
			}
			throw new Exception("�w�肳�ꂽ�`�b�v�͌�����܂���ł����B");
		}

		public void RegisterChipAll(RcChipBase c){
			RegisterChip(c);
			foreach(RcChipBase cb in c.Child)if(cb != null)
				RegisterChipAll(cb);
		}

		public void UnregisterChipAll(RcChipBase c){
			foreach(RcChipBase cb in c.Child)if(cb != null)
				UnregisterChipAll(cb);
			UnregisterChip(c);
		}

		public RcChipBase GetChipFromLib(int id){
			return ChipLib[id];
		}
												  
	}

	/// <summary>
	/// RigidChips���f���f�[�^
	/// </summary>
	public class RcModel{
		RcData gen;
		public RcChipBase root;
		public RcModel(RcData gen){
			this.gen = gen;
			root = new RcChipCore(gen,null,RcJointPosition.NULL);
		}
		/// <summary>
		/// ���f�����̓���̃`�b�v�𓾂�B
		/// </summary>
		public RcChipBase this[int idx]{
			get{
				RcChipBase buff = root.GetChildChip(ref idx);
				if(buff != null)
					return buff;
				else
					throw new Exception("�C���f�b�N�X�l���͈͂𒴂��Ă��܂��B");
			}
/*			set{
				RcChipBase t = root.GetChildChip(ref idx),p;
				if(t == null)
					throw new Exception("�C���f�b�N�X�l���͈͂𒴂��Ă��܂��B");

				int sabun = 0;
				p = t.Parent;
				for(int i = 0;i < 12;i++){
					if(p.Child[i] == t){
						sabun = value.ChipCount - p.Child[i].ChipCount;
						gen.chipCount += sabun;
						p.Child[i] = value;
						return;
					}
				}
				
				throw new Exception("RcChipBase[]�ŗ\�����ʃG���[���������܂����B");
			}	*/
		}
		/// <summary>
		/// ���f�����́A����̖��O���t�����`�b�v�𓾂�B������Ȃ��ꍇ��null.
		/// </summary>
		public RcChipBase this[string name]{
			get{
				return null;
			}
		}
	}

	/// <summary>
	/// �ϐ��f�[�^
	/// </summary>
	public class RcVal{
		public string ValName;
		public float Default;
		public float Min;
		public float Max;
		public float Step;
		public bool Disp;

		public RcVal(){
			Default = Min = Step = 0f;
			Max = 99999999f;
			Disp = true;
		}

		/// <summary>
		/// Val���ڂ̕�������쐬
		/// </summary>
		/// <returns>���������ꂽ Val �u���b�N�̍���</returns>
		public override string ToString() {
			string buff = "";
			buff += ValName + "(";
			if(Default != 0f)buff += "default=" + Default.ToString();
			if(Min != 0f)buff += ",min=" + Min.ToString();
			if(Max != 99999999f)buff += ",max=" + Max.ToString();
			if(Step != 0)buff += ",step=" + Step.ToString();
			if(!Disp)buff += ",disp=0";
			buff += ")";
			return buff;
		}
	}

	/// <summary>
	/// �ϐ�(RcVal)�̃��X�g
	/// </summary>
	public class RcValList{
		RcVal[] list;

		public RcVal this[int idx]{
			get{
				return list[idx];
			}
		}
		public RcVal this[string valname]{
			get{
				foreach(RcVal v in list)
					if(v.ValName == valname)return v;
				
				return null;
			}
		}

		/// <summary>
		/// �ϐ����쐬����B
		/// </summary>
		/// <param name="ValName">�ϐ��̎��ʖ��B</param>
		/// <returns>�V���ɍ쐬���ꂽRcVal�ւ̎Q�ƁB</returns>
		public RcVal Add(string ValName){
			if(this[ValName] != null) return null;
			list.CopyTo(list = new RcVal[list.Length +1],0);
			( list[list.Length-1] = new RcVal() ).ValName = ValName;
			return list[list.Length-1];
		}

		/// <summary>
		/// �w�肳�ꂽ���O�̕ϐ����폜����B
		/// </summary>
		/// <param name="ValName">�폜����ϐ��̎��ʖ��B</param>
		public void Remove(string ValName){
			bool removed = false;
			for(int i = 0;i < list.Length;i++){
				if(removed)
					list[i-1] = list[i];

				else if(list[i].ValName == ValName)
					removed = true;
			}

			list.CopyTo(list = new RcVal[list.Length -1],0);
		}
	}

	/// <summary>
	/// �L�[���͏��
	/// </summary>
	public class RcKey{
		public struct RcKeyWork{
			public RcVal Target;
			public float Step;
		}

		public byte Key_ID;
		public RcKeyWork[] Works;

		/// <summary>
		/// �L�[�ɕϐ����֘A�Â���B���łɊ֘A�Â����Ă���ꍇ�̓X�e�b�v��ύX����B
		/// </summary>
		/// <param name="Target">�^�[�Q�b�g�ƂȂ�RcVal�ւ̎Q�ƁB</param>
		/// <param name="Step">�X�e�b�v�l�B</param>
		public void AssignWork(RcVal Target,float Step){
			for(int i = 0;i < Works.Length;i++){
				if(Works[i].Target == Target){
					Works[i].Step = Step;
					return;
				}
			}
			Works.CopyTo(Works = new RcKeyWork[Works.Length + 1],0);
			Works[Works.Length-1].Target = Target;
			Works[Works.Length-1].Step = Step;
		}

		/// <summary>
		/// �ϐ��̃L�[�ւ̊֘A�Â����폜����B
		/// </summary>
		/// <param name="Target">�폜����RcVal�ւ̎Q�ƁB</param>
		public void DeleteWork(RcVal Target){
			bool removed = false;
			for(int i = 0;i < Works.Length;i++){
				if(removed)
					Works[i-1] = Works[i];

				else if(Works[i].Target == Target)
					removed = true;
			}

			Works.CopyTo(Works = new RcKeyWork[Works.Length -1],0);

		}

		/// <summary>
		/// �L�[�̓����\��������𓾂�B
		/// </summary>
		/// <returns>Key�u���b�N�Ŏg�p�\�Ȃ��̃C���X�^���X�̊��S������B</returns>
		public override string ToString() {
			if(Works.Length == 0)return null;
			string str;
			str = Key_ID.ToString() + ":";
			foreach(RcKeyWork kw in Works){
				str += kw.Target.ValName + "(step=" + kw.Step.ToString() + "),";
			}
			str = str.Substring(0,str.Length-1);
			return str;
		}

	}

	/// <summary>
	/// �L�[(RcKey)�̃��X�g
	/// </summary>
	public class RcKeyList{
		RcKey[] Keys;

		public RcKeyList(int KeyNum){
			Keys = new RcKey[KeyNum];
		}

		public RcKey this[int idx]{
			get{
				return Keys[idx];
			}
		}
	}

	/// <summary>
	/// �}�e���A�������A�e�N�X�`���ꖇ�̊ȈՃ��b�V��
	/// </summary>
	public class RcXFile{
		public string FileName = null;
		public Mesh mesh = null;
		public Texture texture = null;

		/// <summary>
		/// ���b�V����`�悷��B
		/// </summary>
		/// <param name="d3ddevice">�`�悷�� Microsoft.DirectX.Direct3D.Device �ւ̎Q�ƁB</param>
		/// <param name="color">�}�e���A���F�B</param>
		/// <param name="world">�z�u�s��B</param>
		public void Draw(Device d3ddevice,Color color,Matrix world){
			d3ddevice.Transform.World = world;
			d3ddevice.SetTexture(0,texture);
			
			Material mat = new Material();
			mat.Ambient = mat.Diffuse = color;
			color = Color.FromArgb(color.A * 3 / 4,	color.R *3 / 4,color.G * 3 / 4,color.B * 3 / 4);
			mat.Emissive = color;

			d3ddevice.Material = mat;

			mesh.DrawSubset(0);
		}

		/// <summary>
		/// FileName �Ɏw�肳�ꂽ���b�V����ǂݍ��ށB
		/// </summary>
		/// <param name="d3ddevice">Microsoft.DirectX.Direct3D.Device �ւ̎Q�ƁB</param>
		public void Load(Device d3ddevice){
			ExtendedMaterial[] matbuff;
			mesh = Mesh.FromFile(this.FileName,MeshFlags.SystemMemory,d3ddevice,out matbuff);
			
			if(matbuff[0].TextureFilename != null){
				texture = TextureLoader.FromFile(d3ddevice, matbuff[0].TextureFilename);
			}
		}
		/// <summary>
		/// ���b�V����ǂݍ��ށB
		/// </summary>
		/// <param name="d3ddevice">Microsoft.DirectX.Direct3D.Device �ւ̎Q�ƁB</param>
		/// <param name="FileName">�ǂݍ��ރ��b�V���̃t�@�C�����B</param>
		public void Load(Device d3ddevice,string FileName){
			this.FileName = FileName;
			Load(d3ddevice);
		}
	}
	//----------------------------------------------------------------------------------//
	/// <summary>
	/// �`�b�v�����̒l�A�ϐ�(RcVal)�ƒ萔(float)��2����
	/// </summary>
	public struct RcAttrValue{
		public float Const;
		public RcVal Val;
		public bool isNegative;
		/// <summary>
		/// ������Ԃɒu���邱�̕ϐ��̒l�𓾂�B
		/// </summary>
		/// <returns>�ϐ��̒l��\��float�l�B</returns>
		public float Value(){
			if(Val != null)return isNegative ? -Val.Default : Val.Default;
			else           return Const;
		}

		/// <summary>
		/// �ϐ����A�������͒萔�𕶎���œ���B
		/// </summary>
		/// <returns>.rcd�Ŏg�p�\�ȕϐ��܂��͒萔�̕�����B</returns>
		public override string ToString() {
			if(Val != null){
				if(isNegative){
					return "-" + Val.ValName;
				}
				else{
					return Val.ValName;
				}
			}
			else
				return Const.ToString();
		}

	}

	public struct RcHitStatus{
		public float distance;
		public RcChipBase HitChip;
	}
	//----------------------------------------------------------------------------------//
	/// <summary>
	/// �A���ꏊ
	/// </summary>
	public enum RcJointPosition : byte{
		NULL = 255,
		North = 0,
		East,
		South,
		West 
	}
	/// <summary>
	/// �܂�Ȃ�����
	/// </summary>
	public enum RcAngle : byte{
		NULL = 255,
		x = 0,
		y,
		z
	}

}
