using System;
using System.Windows.Forms;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System.Threading;

namespace RigidChips {
	//----------------------------------------------------------------------------------//
	///<summery>
	///すべてのチップの基本クラス
	///このクラスが実装するメソッドの大半は、派生クラス(各チップ)によってオーバーライドされる。
	///</summery>
	public abstract class ChipBase{
		public Environment Environment;
		public string Name;
		protected ChipAttribute angle;
		protected ChipAttribute user1,user2;
		public ChipAttribute ChipColor;
		public ChipBase Parent;
		public ChipBase[] Children;

		//public int RegistID = -1;

		protected JointPosition jointPosition;
		protected Matrix matTranslation;
		protected Matrix matRotation;
		protected Matrix matParent;
		protected long matVersion;

		public string Comment = "";

		/// <summary>
		/// チップの基本的な初期化を行う。
		/// </summary>
		/// <param name="gen">所属する RcData インスタンス</param>
		/// <param name="parent">接続されるチップ</param>
		public ChipBase(Environment gen,ChipBase parent,JointPosition pos){
			Children = new ChipBase[Environment.ChildCapasity];	//	仕様が変わっていなければひとつのチップにつけられるチップの最大数は12
			Environment = gen;
			Attach(parent,pos);
			angle.Const = 0f;
			angle.Val = null;
			angle.IsNegative = false;
			if(parent != null && Environment.EditOption.KeepParentColor)
				this.ChipColor = parent.ChipColor;
			else
				this.ChipColor.SetValue(0xFFFFFF);
		}

		///<summery>
		///コンストラクタ(ヌル)
		///</summery>
		public ChipBase(){}
		
		/// <summary>
		/// チップのワールド座標変換行列を、座標や親チップ、angleをふまえて更新する。
		/// </summary>
		public virtual void UpdateMatrix(){
			// このチップのangle値。
			float a = angle.Value;
			
			// 現在の変換行列の逆行列を取得する。
			Matrix invRotation = matRotation;
			invRotation.Invert();
			
			
			// angleを適用する。
			switch (this.AngleType) {
				case AngleDirection.x:
					matTranslation = invRotation
						* Matrix.Translation(0f, 0f, 0.3f)
						* Matrix.RotationX((float)(a / 180f * Math.PI))
						* Matrix.Translation(0f, 0f, 0.3f)
						* matRotation;
					break;
				case AngleDirection.y:
					matTranslation = invRotation
						* Matrix.Translation(0f, 0f, 0.3f)
						* Matrix.RotationY((float)(a / 180f * Math.PI))
						* Matrix.Translation(0f, 0f, 0.3f)
						* matRotation;
					break;
				case AngleDirection.z:
					matTranslation = invRotation
						* Matrix.Translation(0f, 0f, 0.3f)
						* Matrix.RotationZ((float)(-a / 180f * Math.PI))
						* Matrix.Translation(0f, 0f, 0.3f)
						* matRotation;
					break;
			}

			// 親チップがWheel系だった場合、回転量を反映させる
			WheelChip wh = this.Parent as WheelChip;
			if (Environment.Preview && wh != null) {
				matTranslation *= Matrix.RotationY(wh.PreviewRotation);
			}

			// 親チップの変換行列を取得し、それを掛ける。
			if(Parent != null)	matTranslation *= Parent.Matrix;

			// 行列の生成タイムスタンプを更新。
			matVersion = System.DateTime.Now.Ticks;

			// 子チップが存在する場合、それらも更新。
			foreach(ChipBase cb in Children)if(cb != null)
											   cb.UpdateMatrix();

		}

		/// <summary>
		/// 表示上の位置を表す行列
		/// </summary>
		public virtual Matrix Matrix{
			get{
				// 親の行列がより新しいものになっていたら更新。
				if(this.Parent != null && this.Parent.MatrixVersion > matVersion){
					this.UpdateMatrix();
				}

				return matTranslation;
			}
		}

		///<summery>
		///回転も含めた完全な行列
		///</summery>
		public virtual Matrix FullMatrix{
			get{
				return matRotation * Matrix;
			}
		}

		/// <summary>
		/// 行列のバージョン
		/// </summary>
		public virtual long MatrixVersion{
			get{
				return matVersion;
			}
		}

		/// <summary>
		/// 子チップを追加する。すでに最大数接続されているときには Exception が発生する。
		/// </summary>
		/// <param name="joint">どこに追加するかの RcJointPosition 定数</param>
		/// <param name="chip">追加するチップ</param>
		/// <param name="Registeration">ライブラリに登録するかどうか 省略時はTrue</param>
		public virtual void Add(JointPosition joint,ChipBase chip, bool Registeration){
			for(int i = 0;i < Environment.ChildCapasity;i++){
				if(Children[i] == null){
					Children[i] = chip;
					Children[i].JointPosition = joint;
					Children[i].Parent = this;
					Children[i].UpdateMatrix();
					if(this.GetType() == chip.GetType() && Environment.EditOption.ConvertParentAttributes){
						string[] list = this.AttrNameList;
						foreach(string s in list){
							try{
								if(s == "Angle")continue;
								chip[s] = this[s];
							}
							catch{}
						}
					}
					if (Registeration) {
						Environment.CalcWeightCenter();
					}
					return;
				}
			}
			throw new Exception($"Cannot store more child chips. The maximum number of chips that can be attached to one chip is {Environment.ChildCapasity}.");
		}

		/// <summary>
		/// 子チップを追加する。すでに最大数接続されているときには Exception が発生する。
		/// </summary>
		/// <param name="joint">どこに追加するかの RcJointPosition 定数</param>
		/// <param name="chip">追加するチップ</param>
		public void Add(JointPosition joint, ChipBase chip)
		{
				Add(joint,chip,true);
		}
		

		/// <summary>
		/// すでについているチップを取り外す。存在しないチップを指定したときには Exception が発生する。
		/// </summary>
		/// <param name="chip">取り外したいチップ。</param>
		public void Remove(ChipBase chip){
			bool Removed = false;
			for(int i = 0; i < Environment.ChildCapasity;i++){
				if(Removed){
					Children[i-1] = Children[i];
				}
				else if(Children[i] == chip){
					chip.JointPosition = JointPosition.NULL;
					Environment.CalcWeightCenter();
					Children[i] = null;
					Removed = true;
				}
			}
			if (Removed) {
				Children[Environment.ChildCapasity - 1] = null;
				return;
			}
			else {
				throw new Exception("The specified chip was not found: " + chip.ToString());
			}
		}

		/// <summary>
		/// このチップを他のチップに接続する。
		/// </summary>
		/// <param name="to">接続されるチップ。</param>
		/// <param name="pos">接続位置。</param>
		public virtual void Attach(ChipBase to,JointPosition pos){
			if(to == null)return;
				to.Add(pos,this);
		}

		/// <summary>
		/// 現在接続されているチップから取り外す。
		/// </summary>
		public virtual void Detach(){
			this.Parent.Remove(this);
		}

		/// <summary>
		/// チップを画面に描画する。
		/// </summary>
		public abstract void DrawChip();

		/// <summary>
		/// 使用可能な属性の名前の配列を得る。
		/// </summary>
		public virtual string[] AttrNameList{
			get => throw new NotImplementedException();
		}

		/// <summary>
		/// 現在の全属性の値の配列を得る。インデックスはAttrNameListに対応。
		/// </summary>
		public virtual ChipAttribute[] AttrValList{
			get => throw new NotImplementedException();
		}

		/// <summary>
		/// 属性の規定値を得る。インデックスはAttrNameListに対応。
		/// </summary>
		public virtual float[] AttrDefaultValueList{
			get => throw new NotImplementedException();
		}

		/// <summary>
		/// このチップの種類を得る
		/// </summary>
		public abstract ChipType ChipType { get; }

		///// <summary>
		///// このチップの種類を文字列で得る。
		///// </summary>
		//public abstract string ChipTypeName{ get; }

		/// <summary>
		/// 指定した名前の属性の説明文を得る。
		/// </summary>
		/// <param name="AttrName">使用可能な属性名。</param>
		/// <returns>指定した属性の説明文。</returns>
		public virtual string AttrTip(string AttrName){
			if(AttrName == "Color")	return "Chip color"; else return null;
		}

		/// <summary>
		/// 指定した属性の値を設定・取得する。
		/// </summary>
		public virtual ChipAttribute this[string AttrName]{
			set{
				if(AttrName == "Color")
					ChipColor = value;
				else
					throw new Exception("Attribute with specified name does not exist: " + AttrName);
			}

			get{
				if(AttrName == "Color")
					return ChipColor;

				else
					throw new Exception("Attribute with specified name does not exist: " + AttrName);
			}
		}

		/// <summary>
		/// このチップの情報の文字列を生成する。
		/// </summary>
		/// <returns>このチップの文字列情報。</returns>
		public override string ToString(){
			return ToString(Environment.OutputOption);
		}
		/// <summary>
		/// このチップの情報の文字列を生成する。
		/// </summary>
		/// <returns>このチップの文字列情報。</returns>
		public virtual string ToString(OutputOptions outputOptions) {
			StringBuilder str = new StringBuilder();			
//			string str = "";
			string comma = outputOptions.CommaWithSpace ? ", " : ",";
			str.Append(this.ChipType.ToString());
			str.Append('(');

			string[] attrname = this.AttrNameList;
			float[] attrdef = this.AttrDefaultValueList;
			ChipAttribute[] attrval = this.AttrValList;
			if(this.Name != null && this.Name != ""){
				str.Append("Name=");
				str.Append(Name);
				str.Append(comma);
			}
			for(int i = 0;i < attrname.Length;i++){
				if(outputOptions.PrintAllAttributes || attrval[i].Val != null || attrval[i].Const != attrdef[i]){
					str.Append(attrname[i]);
					str.Append('=');
					str.Append(attrval[i].ToString());
					str.Append(comma);
				}
			}

			while(str[str.Length-1] == ' ' || str[str.Length-1] == ',')str.Length--;

			str.Append(')');


			return str.ToString();
		}

		/// <summary>
		/// このチップとその派生についての完全表現文字列を得る。
		/// </summary>
		/// <param name="tabs">インデントのタブ数</param>
		/// <returns>.rcd内に使用可能な Body ブロック用文字列</returns>
		public string ToString(int tabs) {
			return ToString(tabs, Environment.OutputOption);
		}

		/// <summary>
		/// このチップとその派生についての完全表現文字列を得る。
		/// </summary>
		/// <param name="tabs">インデントのタブ数</param>
		/// <param name="outputOptions">使用する出力オプション</param>
		/// <returns>.rcd内に使用可能な Body ブロック用文字列</returns>
		public string ToString(int tabs, OutputOptions outputOptions) {
//			string s = "";
			StringBuilder s = new StringBuilder();
			StringBuilder st = new StringBuilder();
			StringBuilder ss = new StringBuilder();
			if(outputOptions.IndentEnable){
				if(outputOptions.IndentBySpace)
					for(int i = 0;i < tabs * outputOptions.IndentNum ;  i++)st.Append(' ');
				else
					for(int i = 0;i < tabs; i++)st.Append('\t');
			}
			s.Append(st);
			//	接続方位
			switch(this.jointPosition){
				case JointPosition.North:
					s.Append("N:");
					break;
				case JointPosition.South:
					s.Append("S:");
					break;
				case JointPosition.East:
					s.Append("E:");
					break;
				case JointPosition.West:
					s.Append("W:");
					break;
			}


			s.Append(this.ToString(outputOptions));

			//	派生ブロック取得
			if(outputOptions.OpenBracketWithChipDefinition){

				if(!outputOptions.ReturnEndChipBracket){
					foreach(ChipBase cb in this.Children)
						if(cb != null){
							ss.Append("\r\n");
							ss.Append(cb.ToString(tabs + 1, outputOptions));
						}
					if(ss.Length == 0){
						s.Append("{}");
						s.Append((this.Comment == null || this.Comment == "") ? "" : ("//" + this.Comment));
					}
					else{
						s.Append("{");
						s.Append((this.Comment == null || this.Comment == "") ? "" : ("//" + this.Comment));
						s.Append(ss.ToString());
						s.Append("\r\n");
						s.Append(st.ToString());
						s.Append("}");
					}
				}
				else{
					s.Append('{');
					s.Append((this.Comment == null || this.Comment == "") ? "" : ("//" + this.Comment));
					foreach(ChipBase cb in this.Children)
						if(cb != null){
							s.Append("\r\n");
							s.Append(cb.ToString(tabs + 1, outputOptions));
						}
					s.Append("\r\n");
					s.Append(st.ToString());
					s.Append('}');
				}

			}
			else{
				s.Append(((this.Comment == null || this.Comment == "") ? "" : ("//" + this.Comment)) + "\r\n" + st.ToString() + "{");
				foreach(ChipBase cb in this.Children)
					if(cb != null){
						s.Append("\r\n");
						s.Append(cb.ToString(tabs + 1, outputOptions));
					}
				if(!outputOptions.ReturnEndChipBracket && s[s.Length-1] == '{')
					s.Append('}');
				else{
					s.Append("\r\n");
					s.Append(st.ToString());
					s.Append('}');
				}

			}

			return s.ToString();
		}

		/// <summary>
		///	このチップ及び派生チップの総数。読み取り専用。
		/// </summary>
		public int ChipCount{
			get{
				int c = 1;
				foreach(ChipBase cb in Children)if(cb != null){
					c += cb.ChipCount;	// 再帰
				}
				return c;
			}
		}

		/// <summary>
		/// マウスポインティングに対し、このオブジェクトが選択されたかどうかを得る。
		/// </summary>
		/// <param name="X">マウスカーソルのX座標</param>
		/// <param name="Y">マウスカーソルのY座標</param>
		/// <param name="ScrWidth">スクリーンの幅</param>
		/// <param name="ScrHeight">スクリーンの高さ</param>
		/// <returns>選択されたチップと距離の情報</returns>
		public virtual HitStatus IsHit(int X,int Y,int ScrWidth,int ScrHeight){
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
			
			buff.distance = (Environment.imesh.Intersect(vNear, vDir, out sectinfo)) ? sectinfo.Dist : float.MaxValue; 
			if(dist.distance > buff.distance){
				dist.distance = buff.distance;
				dist.HitChip = this;
			}

			return dist;
		}

		/// <summary>
		/// 指定された相対インデックス値のチップを得る。
		/// </summary>
		/// <param name="idx">取得するチップのインデックス。この値は変更される。</param>
		/// <returns>要求されたインデックスのチップ</returns>
		public ChipBase GetChildChip(ref int idx){
			ChipBase buff;
			if(idx == 0)
				return this;
			else{
				idx--;
				foreach(ChipBase cld in Children)if(cld != null){
					buff = cld.GetChildChip(ref idx);
					if(buff != null) return buff;
				}
			}
			return null;
		}

		/// <summary>
		/// このチップとその派生チップ全てを描画する。
		/// </summary>
		public void DrawChipAll(){
			DrawChip();
			foreach(ChipBase cld in Children)if(cld != null)
				cld.DrawChipAll();
		}


		public virtual void ReserveDraw(ThreadStart fnc) {
			if (this.Parent != null)
				this.Parent.ReserveDraw(fnc);
			else
				fnc();
		}

		///<summery>
		///接続方向を得る。
		///</summery>
		public virtual JointPosition JointPosition{
			get{
				return jointPosition;
			}
			set{
				jointPosition = value;
				switch(jointPosition){
					case JointPosition.South:
						matRotation = Matrix.RotationY((float)Math.PI);
						break;
					case JointPosition.East:
						matRotation = Matrix.RotationY((float)(Math.PI * 0.5f));
						break;
					case JointPosition.West:
						matRotation = Matrix.RotationY((float)(Math.PI * 1.5f));
						break;
					case JointPosition.North:
					default:
						matRotation = Matrix.Identity;
						break;
				}
			}
		}

		/// <summary>
		/// このチップと同一の内容のRcChipBaseインスタンスを作成する。
		/// </summary>
		/// <param name="WithChild">派生チップ情報のクローンも作成するかどうかのフラグ</param>
		/// <returns>同一の内容を保持した新しいRcChipBaseへの参照</returns>
		public virtual ChipBase Clone(bool WithChild,ChipBase parent) => throw new NotImplementedException();

		public virtual void RotateLeft(){
			ChipBase[] cld = new ChipBase[Environment.ChildCapasity];
			JointPosition jp;
			this.Children.CopyTo(cld,0);

			foreach(ChipBase cb in cld){
				if(cb == null)continue;
				jp = cb.JointPosition;
				cb.RotateLeft();
				cb.Detach();
				cb.Attach(this,(JointPosition)(((int)jp + 3) % 4));
			}
		}
		public virtual void RotateRight(){
			ChipBase[] cld = new ChipBase[Environment.ChildCapasity];
			JointPosition jp;
			this.Children.CopyTo(cld,0);

			foreach(ChipBase cb in cld){
				if(cb == null)continue;
				jp = cb.JointPosition;
				cb.RotateRight();
				cb.Detach();
				cb.Attach(this,(JointPosition)(((int)jp + 1) % 4));
			}

		}

		public virtual void ReverseX(){
			ChipBase[] cld = new ChipBase[Environment.ChildCapasity];
			JointPosition jp;
			this.Children.CopyTo(cld,0);

			foreach(ChipBase cb in cld){
				if(cb == null)continue;
				jp = cb.JointPosition;
				cb.ReverseX();
				if(((int)jp & 1) > 0){
					cb.Detach();
					cb.Attach(this,(JointPosition)(((int)jp + 2) % 4));
				}
			}
		}
		public virtual void ReverseY(){
			ChipBase[] cld = new ChipBase[Environment.ChildCapasity];
			JointPosition jp;
			this.Children.CopyTo(cld,0);

			foreach(ChipBase cb in cld){
				if(cb == null)continue;
				jp = cb.JointPosition;
				cb.ReverseY();
			}
			
		}
		public virtual void ReverseZ(){
			ChipBase[] cld = new ChipBase[Environment.ChildCapasity];
			JointPosition jp;
			this.Children.CopyTo(cld,0);

			foreach(ChipBase cb in cld){
				if(cb == null)continue;
				jp = cb.JointPosition;
				cb.ReverseZ();
				if(((int)jp % 2) == 0){
					cb.Detach();
					cb.Attach(this,(JointPosition)(((int)jp + 2) % 4));
				}
			}
		}

		///<summery>
		///文字列からチップを生成する。
		///</summery>
		public static ChipBase Parse(Environment generics,string data){
			string[] param = data.Split(',',':','(',')','=');
			int index = 0;
			JointPosition jp = JointPosition.NULL;
			ChipBase newchip;

			switch(param[index]){
				case "n":
				case "N":
					jp = JointPosition.North;
					index++;
					break;
				case "s":
				case "S":
					jp = JointPosition.South;
					index++;
					break;
				case "e":
				case "E":
					jp = JointPosition.East;
					index++;
					break;
				case "w":
				case "W":
					jp = JointPosition.West;
					index++;
					break;
			}

			switch(param[index].ToLower()){
				case "core":
					newchip = new CoreChip(generics,null,JointPosition.NULL);
					index++;
					break;
				case "chip":
					newchip = new NormalChip(generics,null,JointPosition.NULL);
					index++;
					break;
				case "frame":
					newchip = new FrameChip(generics,null,JointPosition.NULL);
					index++;
					break;
				case "rudder":
					newchip = new RudderChip(generics,null,JointPosition.NULL);
					index++;
					break;
				case "rudderf":
					newchip = new RudderFrameChip(generics,null,JointPosition.NULL);
					index++;
					break;
				case "trim":
					newchip = new TrimChip(generics,null,JointPosition.NULL);
					index++;
					break;
				case "trimf":
					newchip = new TrimFrameChip(generics,null,JointPosition.NULL);
					index++;
					break;
				case "wheel":
					newchip = new WheelChip(generics,null,JointPosition.NULL);
					index++;
					break;
				case "rlw":
					newchip = new RLWChip(generics,null,JointPosition.NULL);
					index++;
					break;
				case "jet":
					newchip = new JetChip(generics,null,JointPosition.NULL);
					index++;
					break;
				case "weight":
					newchip = new WeightChip(generics,null,JointPosition.NULL);
					index++;
					break;
				case "cowl":
					newchip = new CowlChip(generics,null,JointPosition.NULL);
					index++;
					break;
				case "arm":
					newchip = new ArmChip(generics,null,JointPosition.NULL);
					index++;
					break;
				default:
					throw new Exception("Unknown chip type detected: " + param[index]);
			}
			newchip.JointPosition = jp;
			ChipAttribute attr = new ChipAttribute();
			try{
				for( ; param[index] != "" ; index += 2){
					param[index] = param[index].ToLower();
					if(param[index] == "name")
						newchip.Name = param[index+1];
					else if(param[index] == "color")
						newchip.ChipColor.SetValue(param[index+1],generics.vals);
					else{
						param[index] = char.ToUpper(param[index][0]).ToString() + param[index].Substring(1);
						attr.SetValue(param[index+1],generics.vals);
						newchip[param[index]] = attr;
					}

				}
			}
			catch(IndexOutOfRangeException){
				throw new Exception("Illegal attribute value specification exists.");
			}
			catch(Exception e){
				MessageBox.Show(e.Message);
			}

			return newchip;

		}

		///<summery>
		///チップの種類を得る。
		///</summery>
		public static ChipType CheckType(ChipBase chip){
			if(chip is CoreChip){
				return RigidChips.ChipType.Core;
			}
			else if(chip is NormalChip){
				if(chip is FrameChip){
					return RigidChips.ChipType.Frame;
				}
				else if(chip is RudderChip){
					if(chip is RudderFrameChip){
						return RigidChips.ChipType.RudderF;
					}
					else{
						return RigidChips.ChipType.Rudder;
					}
				}
				else if(chip is TrimChip){
					if(chip is TrimFrameChip){
						return RigidChips.ChipType.TrimF;
					}
					else{
						return RigidChips.ChipType.Trim;
					}
				}
				else if(chip is WeightChip){
					return RigidChips.ChipType.Weight;
				}
				else{
					return RigidChips.ChipType.Chip;
				}
			}
			else if(chip is WheelChip){
				if(chip is RLWChip){
					return RigidChips.ChipType.RLW;
				}
				else{
					return RigidChips.ChipType.Wheel;
				}
			}
			else if(chip is JetChip){
				return RigidChips.ChipType.Jet;
			}
			else if(chip is ArmChip){
				return RigidChips.ChipType.Arm;
			}
			else if(chip is CowlChip){
				return RigidChips.ChipType.Cowl;
			}
			else{
				return RigidChips.ChipType.Unknown;
			}
		}

		///<summery>
		///このチップはどの方向へ折れ曲がるチップかを得る。
		///</summery>
		public virtual AngleDirection AngleType{
			get => AngleDirection.x;
		}

		///<summery>
		///このチップの重量。
		///</summery>
		public virtual float Weight{
			get => throw new NotImplementedException();
		}

		///<summery>
		///重量による重み付きベクトルを得る。
		///</summery>
		public Vector3 WeightedVector{
			get{
				Vector3 v = new Vector3();
				v.TransformCoordinate(matTranslation);
				return Weight * v;
			}
		}

		///<summery>
		///このチップと同一の属性を持ち、種類が違う新しいチップを生成する。
		///</summery>
		public ChipBase ChangeType(ChipType type){
			ChipBase buff;

			if(type != RigidChips.ChipType.Cowl && this.Parent is CowlChip)
				throw new Exception("Parent is a cowl. Can't change chip to non-cowl chip.");
			
			switch(type) {
				case RigidChips.ChipType.Arm:
					buff = new ArmChip(Environment, null, JointPosition.NULL);
					break;
				case RigidChips.ChipType.Chip:
					buff = new NormalChip(Environment, null, JointPosition.NULL);
					break;
				case RigidChips.ChipType.Frame:
					buff = new FrameChip(Environment, null, JointPosition.NULL);
					break;
				case RigidChips.ChipType.Rudder:
					buff = new RudderChip(Environment, null, JointPosition.NULL);
					break;
				case RigidChips.ChipType.RudderF:
					buff = new RudderFrameChip(Environment, null, JointPosition.NULL);
					break;
				case RigidChips.ChipType.Trim:
					buff = new TrimChip(Environment, null, JointPosition.NULL);
					break;
				case RigidChips.ChipType.TrimF:
					buff = new TrimFrameChip(Environment, null, JointPosition.NULL);
					break;
				case RigidChips.ChipType.Wheel:
					buff = new WheelChip(Environment, null, JointPosition.NULL);
					break;
				case RigidChips.ChipType.RLW:
					buff = new RLWChip(Environment, null, JointPosition.NULL);
					break;
				case RigidChips.ChipType.Weight:
					buff = new WeightChip(Environment, null, JointPosition.NULL);
					break;
				case RigidChips.ChipType.Cowl:
					buff = new CowlChip(Environment, null, JointPosition.NULL);
					break;
				case RigidChips.ChipType.Jet:
					buff = new JetChip(Environment, null, JointPosition.NULL);
					break;
				default:
					return this;
			}
			JointPosition jp;
			ChipBase parent = this.Parent;

			string[] attrlist = this.AttrNameList;
			foreach(string attr in attrlist){
				try{
					buff[attr] = this[attr];
				}
				catch{}
			}

			if (type == RigidChips.ChipType.Cowl) {
				switch (ChipBase.CheckType(this)) {
					case RigidChips.ChipType.Jet:
					case RigidChips.ChipType.RLW:
					case RigidChips.ChipType.Wheel:
						buff["Option"] = new ChipAttribute(2);
						break;
					case RigidChips.ChipType.Frame:
					case RigidChips.ChipType.RudderF:
					case RigidChips.ChipType.TrimF:
						buff["Option"] = new ChipAttribute(1);
						break;
					default:
						buff["Option"] = new ChipAttribute(0);
						break;
				}

			}

			ChipBase c;
			do{
				c = this.Children[0];
				if(c == null)break;
				jp = c.JointPosition;
				c.Detach();
				buff.Add(jp,c,false);
			}while(true);

			jp = this.JointPosition;
			this.Detach();
			buff.Attach(parent,jp);

			return buff;
		}

		public virtual float Fuel{
			get { return 1000000; }
		}

		public ChipBase Ancestor {
			get {
				if (Parent == null) return null;
				if (Parent.Parent == null) return Parent;
				return Parent.Ancestor;
			}
		}
	}
}
