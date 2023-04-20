using System;
using System.IO;
using System.Windows.Forms;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Specialized;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System.Runtime.InteropServices.ComTypes;
using System.Collections.Generic;

namespace RigidChips {

	///<summery>
	///ごく単純な、コールバック用デリゲート
	///</summery>
	public delegate void MessageCallback(object param);

	
	///<summery>
	///モデリングに関して総合的なデータを持つクラス。基本的に単一インスタンス。
	///</summery>
	public class Environment{
		public string ResourcePath = "";
		public const int MaxChipCount = 512;
		public const int KeyCount = 17;
		public const int ChildCapasity = 12;

		public Device d3ddevice;
		public Model model;
		public ValEntryList vals;
		public KeyEntryList keys;
		public string headercomment;
		public string script;
		public bool luascript;

		public Mesh imesh;
		public int chipCount;

		List<XFile> meshes;
		public CursorChip Cursor;

		//ChipBase[] ChipLib;

		Vector3 weightCenter;

		bool RegisteringAll = false;

		DrawOptions drawOption;
		OutputOptions outputOption;
		EditOptions editOption;

		bool multiSelect = false;
		int selectedCount = 0;
	//	System.Collections.ArrayList selectedChipArray = new System.Collections.ArrayList();
		List<ChipBase> selectedChipArray = new List<ChipBase>();

		XFile roll;

		/// <summary>
		/// RcData インスタンスを初期化する。
		/// </summary>
		/// <param name="D3DDevice">表示などに用いるMicrosoft.DirectX.Direct3D.Device インスタンス。</param>
		public Environment(Device D3DDevice,DrawOptions draw,OutputOptions output,EditOptions edit,string pathResources){

			//	各インスタンスの初期化
			d3ddevice = D3DDevice;
			drawOption = draw;
			outputOption = output;
			editOption = edit;
			ResourcePath = pathResources;

			vals = new ValEntryList();

			keys = new KeyEntryList(KeyCount);

			meshes = new List<XFile>();

			model = new Model(this);

			headercomment = script = "";

			//ChipLib = new ChipBase[MaxChipCount];
			
			chipCount = 0;

			Cursor = new CursorChip(this,model.root,JointPosition.NULL);
			new GuideChip(this,Cursor,JointPosition.North);
			new GuideChip(this,Cursor,JointPosition.South);
			new GuideChip(this,Cursor,JointPosition.East);
			new GuideChip(this,Cursor,JointPosition.West);
			//RegisterChip(model.root);
			CalcWeightCenter();

			roll = new XFile();
			roll.Load(d3ddevice,Application.StartupPath + "\\Resources\\roll.x");

			ExtendedMaterial[] matbuff;
			imesh = Mesh.FromFile(ResourcePath + @"\Chip.x",MeshFlags.SystemMemory,d3ddevice,out matbuff);
		}

		///<summery>
		///回転方向表示用ガイドモデル
		///</summery>
		public XFile RollMesh{
			get{ return roll; }
		}

		///<summery>
		///描画オプション
		///</summery>
		public DrawOptions DrawOption{
			get{
				return drawOption;
			}
		}

		///<summery>
		///ファイル出力オプション
		///</summery>
		public OutputOptions OutputOption{
			get{
				return outputOption;
			}
		}

		///<summery>
		///編集オプション
		///</summery>
		public EditOptions EditOption{
			get{
				return editOption;
			}
		}

		///<summery>
		///選択されているチップの数。値の代入は、現在0(選択解除)しかできない。
		///</summery>
		public int SelectedChipCount{
			get{
				return (multiSelect ? selectedCount : 0);
			}
			set{
				if(value != 0)
					throw new Exception("SelectedChipCount プロパティに設定できる値は 0 のみです。");
				selectedChipArray.Clear();
			}
		}
		
		///<summery>
		///選択されているチップのリスト。
		///</summery>
		public ChipBase[] SelectedChipList{
			get{
				if(multiSelect)
				return selectedChipArray.ToArray();
				else return null;
			}
		}
		
		/// <summary>
		/// チップ表示用RcXFileクラスへの参照を得る。
		/// </summary>
		/// <param name="FileName">.x ファイルのパス</param>
		/// <returns>ロードしたRcXFileへの参照</returns>
		public XFile GetMesh(string FileName) {

			if (meshes.Find(m => m.FileName == FileName) is XFile mesh) {
				// すでに同一ファイル名のメッシュが読み込まれていたら、それを返す。
				return mesh;
			}

			string tmp = Directory.GetCurrentDirectory();
			Directory.SetCurrentDirectory(ResourcePath);

			var newMesh = new XFile {
				FileName = FileName
			};
			newMesh.Load(this.d3ddevice);
			Directory.SetCurrentDirectory(tmp);

			if (newMesh.mesh != null) {
				meshes.Add(newMesh);
				return newMesh;
			}
			
			return null;
		}

		/// <summary>
		/// チップ表示用RcXFileクラスへの参照を得る。
		/// </summary>
		/// <param name="FileName">.x ファイルのパス</param>
		/// <param name="IsFullPath">FileNameがフルパスであることを表すフラグ</param>
		/// <returns>ロードしたRcXFileへの参照</returns>
		public XFile GetMesh(string FileName,bool IsFullPath){
			if(IsFullPath){

				if (meshes.Find(m => m.FileName == FileName) is XFile mesh) {
					return mesh;
				}

				//string tmp = Directory.GetCurrentDirectory();
				//Directory.SetCurrentDirectory(Path.GetDirectoryName(FileName));

				var newMesh = new XFile { FileName = FileName };
				newMesh.Load(this.d3ddevice);
				//Directory.SetCurrentDirectory(tmp);

				if (newMesh.mesh != null) {
					meshes.Add(newMesh);
					return newMesh;
				}

				return null;

			}
			else return GetMesh(FileName);
		}

		///<summery>
		///GetMeshによって読み込んだすべてのXファイルデータを解放する。
		///</summery>
		public void DisposeMeshes(){
			meshes.ForEach(m => {
				m.texture?.Dispose();
				m.texture = null;
				m.mesh?.Dispose();
				m.mesh = null;
			});
			meshes.Clear();

			imesh.Dispose();
		}

		///<summery>
		///全メッシュのリロード
		///</summery>
		public void ReloadMeshes(){
			meshes.ForEach(m => m.Load(d3ddevice));
			imesh = Mesh.FromFile(ResourcePath + "\\Chip.x",MeshFlags.SystemMemory,d3ddevice,out ExtendedMaterial[] _);
		}

		///<summery>
		///指定したチップにカーソルを移動する
		///</summery>
		public void SetCursor(ChipBase Target){
			Cursor.Attach(Target,JointPosition.NULL);
		}

		///<summery>
		///カーソルを描画
		///</summery>
		public void DrawCursor(){
			Cursor.SetCursorColor(drawOption.CursorFrontColor,drawOption.CursorBackColor);
			Cursor.DrawChipAll();
		}

		///<summery>
		///チップを描画
		///</summery>
		public void DrawChips(bool enableGuide){
			DrawChips(enableGuide, true);
		}
		public void DrawChips(bool enableGuide, bool enableCursor) {
			Cursor.SetCursorColor(drawOption.CursorFrontColor,drawOption.CursorBackColor);
			//			Cursor.DrawChipAll();

			var cowls = new List<ChipBase>();

			// カウル以外のチップ
			model.ForEach(chip => {
				if (chip is CowlChip) cowls.Add(chip);
				else chip.DrawChip();
			});

			// ガイド
			if(enableGuide){
				foreach(ChipBase g in Cursor.Children){
					if(g == null)break;
					g.DrawChip();
				}
			}

			// カウル
			foreach(ChipBase cowl in cowls) {
				cowl.DrawChip();
			}

			CoreChip core = model.root as CoreChip;
			if (core != null && core.ReservedDrawFunc != null) {
				core.ReservedDrawFunc();
				core.ReservedDrawFunc = null;
			}

			if (enableCursor) {
				Cursor.DrawChip();

				Matrix m = Cursor.Parent.FullMatrix;
				float angle90 = ((float)Math.PI / 2);

				switch (Cursor.Parent.AngleType) {
					case AngleDirection.x:
						m = Matrix.Translation(0, 0, -0.3f) * m;
						roll.Draw(d3ddevice, drawOption.CursorBackColor, 0x8000, m);
						roll.Draw(d3ddevice, drawOption.CursorFrontColor, 0x8000, Matrix.RotationX(angle90) * m);
						roll.Draw(d3ddevice, drawOption.CursorFrontColor, 0x8000, Matrix.RotationX(angle90 * 2) * m);
						roll.Draw(d3ddevice, drawOption.CursorBackColor, 0x8000, Matrix.RotationX(angle90 * 3) * m);
						break;
					case AngleDirection.y:
						m = Matrix.RotationZ(angle90) * Matrix.Translation(0, 0, -0.3f) * m;
						roll.Draw(d3ddevice, drawOption.CursorBackColor, 0x8000, m);
						roll.Draw(d3ddevice, drawOption.CursorFrontColor, 0x8000, Matrix.RotationX(angle90) * m);
						roll.Draw(d3ddevice, drawOption.CursorFrontColor, 0x8000, Matrix.RotationX(angle90 * 2) * m);
						roll.Draw(d3ddevice, drawOption.CursorBackColor, 0x8000, Matrix.RotationX(angle90 * 3) * m);
						break;
					case AngleDirection.z:
						m = Matrix.RotationY(angle90) * m;
						roll.Draw(d3ddevice, drawOption.CursorFrontColor, 0x8000, m);
						roll.Draw(d3ddevice, drawOption.CursorBackColor, 0x8000, Matrix.RotationX(angle90) * m);
						roll.Draw(d3ddevice, drawOption.CursorFrontColor, 0x8000, Matrix.RotationX(angle90 * 2) * m);
						roll.Draw(d3ddevice, drawOption.CursorBackColor, 0x8000, Matrix.RotationX(angle90 * 3) * m);
						break;
				}
			}

		}

		/////<summery>
		/////モデルチップリストに、チップを登録する。
		/////</summery>
		//public int RegisterChip(ChipBase c){
		//	if(c.RegistID > 0)return c.RegistID;
		//	for(int i = 0;i < MaxChipCount;i++){
		//		if(ChipLib[i] == null){
		//			ChipLib[i] = c;
		//			c.RegistID = i;
		//			chipCount++;

		//			string[] attrlist = c.AttrNameList;
		//			ValEntry v;
		//			for(int j = 0;j < attrlist.Length;j++)
		//				if((v = c[attrlist[j]].Val) != null)
		//					v.RefCount++;
		//			CalcWeightCenter();
		//			if(!RegisteringAll && OnChipLibraryChanged != null)OnChipLibraryChanged(null);
		//			return i;
		//		}
		//		else if(ChipLib[i] == c){
		//			c.RegistID = i;
		//			chipCount++;
		//			return i;
		//		}
		//	}
		//	throw new Exception("チップ最大数(512)を超えました。");
		//}

		/////<summery>
		/////登録されていたチップをリストから除去する。
		/////</summery>
		//public void UnregisterChip(ChipBase c){
		//	if(c.RegistID < 0 || c.RegistID > Environment.MaxChipCount){
		//		throw new Exception("指定したチップは登録されていないか、または不正な方法で登録された可能性があります : " + c.ToString());
		//	}
		//	ChipLib[c.RegistID] = null;
		//	string[] attrlist = c.AttrNameList;
		//	ValEntry v;
		//	for(int j = 0;j < attrlist.Length;j++)
		//		if((v = c[attrlist[j]].Val) != null)
		//			v.RefCount--;

		//	SlideLibraryData(c.RegistID);
		//	c.RegistID = -1;
		//	chipCount--;
		//	CalcWeightCenter();
		//	if(!RegisteringAll && OnChipLibraryChanged != null)OnChipLibraryChanged(null);
		//}

		/////<summery>
		/////あるチップと、その派生チップすべてを登録する。
		/////</summery>
		//public void RegisterChipAll(ChipBase c){
		//	RegisteringAll = true; 
		//	RegisterChip(c);
		//	foreach (ChipBase cb in c.Children) {
		//		if (cb != null) {
		//			RegisterChipAll(cb);
		//		}
		//	}
		//	RegisteringAll = false;
		//	CalcWeightCenter();
		//	if(OnChipLibraryChanged != null)OnChipLibraryChanged(null);
		//}

		//public void UnregisterChipAll(ChipBase c){
		//	RegisteringAll = true;
		//	foreach (ChipBase cb in c.Children) {
		//		if (cb != null) {
		//			UnregisterChipAll(cb);
		//		}
		//	}
		//	UnregisterChip(c);
		//	RegisteringAll = false;
		//	CalcWeightCenter();
		//	if(OnChipLibraryChanged != null)OnChipLibraryChanged(null);
		//}

		/////<summery>
		/////リストの途中の空欄を消す。
		/////</summery>
		//public void SlideLibraryData(int startat){
		//	for(int i = startat;i < MaxChipCount -1; i++){
		//		ChipLib[i] = ChipLib[i + 1];
		//		if(ChipLib[i] != null)ChipLib[i].RegistID = i;
		//		else return;
		//	}
		//	ChipLib[MaxChipCount -1] = null;
		//}

		/////<summery>
		/////あるチップの親がそのチップより大きいインデックス番号で登録されていると、保存時に問題になるので
		/////それを除去する。
		/////</summery>
		//public void CheckBackTrack(){
		//	int buff;
		//	ChipBase buffc;

		//	for(int i = 0;i < MaxChipCount-1;i++){
		//		if(ChipLib[i] is CowlChip){
		//			bool flag = false;
		//			for(int j = chipCount - 1;j != i;j--){
		//				if(! (ChipLib[j] is CowlChip)){
		//					buffc = ChipLib[i];
		//					ChipLib[i] = ChipLib[j];
		//					ChipLib[j] = buffc;
		//					ChipLib[i].RegistID = i;
		//					ChipLib[j].RegistID = j;
		//					flag = true;
		//					break;
		//				}
		//			}
		//			if(!flag)break;
		//		}
		//	}
			
		//	for(int i = 0;i < MaxChipCount -1; i++){
		//		if(ChipLib[i] is CoreChip)continue;
		//		if(ChipLib[i] == null || ChipLib[i].Parent == null)return;
		//		if(ChipLib[i].Parent.RegistID < 0){
		//			try{
		//				UnregisterChipAll(ChipLib[i].Parent);
		//			}catch{}
		//			continue;
		//		}
		//		if(ChipLib[i].RegistID < ChipLib[i].Parent.RegistID){
		//			buff = ChipLib[i].Parent.RegistID;
		//			buffc = ChipLib[i].Parent;
		//			ChipLib[buff] = ChipLib[i];
		//			ChipLib[i] = buffc;
		//			ChipLib[buff].RegistID = buff;
		//			ChipLib[i].RegistID = i;
		//			i--;
		//		}
		//	}

		//}

		/////<summery>
		/////指定したインデックスで登録されているチップを得る。
		/////</summery>
		//public ChipBase GetChipFromLib(int id){
		//	return ChipLib[id];
		//}

		///<summery>
		///現在カーソルがあるチップを取得する。複数選択時にはnull。
		///</summery>
		public ChipBase SelectedChip{
			get{
				if(multiSelect)return null;
				return Cursor.Parent;
			}
			set{
				SetCursor(value);
				multiSelect = false;
				selectedChipArray.Clear();
				SelectedChipCount = 0;
				if(OnSelectedChipChanged != null)
					OnSelectedChipChanged(value);
			}
		}

		///<summery>
		///選択されているチップを増減させる。chipがすでに選択されていれば解除し、されていなければ選択する。
		///</summery>
		public void AssignSelectedChips(ChipBase chip){
			if(chip == null)return;
			if(!multiSelect)
				selectedChipArray.Add(Cursor.Parent);
			if(selectedChipArray.Contains(chip))
				selectedChipArray.Remove(chip);
			else
				selectedChipArray.Add(chip);

			selectedCount = selectedChipArray.Count;
			if(selectedCount != 0){
				SetCursor(null);
				multiSelect = true;
			}
			else{
				SetCursor(chip);
				multiSelect = false;
			}

			if(OnSelectedChipChanged != null)OnSelectedChipChanged(null);
		}

		public void AssignSelectedChips(ChipBase[] chips){
			foreach(ChipBase chip in chips){
				if(chip == null)break;
				if(!multiSelect)
					selectedChipArray.Add(Cursor.Parent);
				if(selectedChipArray.Contains(chip))
					selectedChipArray.Remove(chip);
				else
					selectedChipArray.Add(chip);

				selectedCount = selectedChipArray.Count;
				if(selectedCount != 0){
					SetCursor(null);
					multiSelect = true;
				}
				else{
					SetCursor(chip);
					multiSelect = false;
				}
			}
			if(OnSelectedChipChanged != null)OnSelectedChipChanged(null);
		}

		///<summery>
		///選択されているチップのリストを、chipsに変更する。
		///</summery>
		public void AssignSelectedChips(ChipBase[] chips,bool SetList){
			if(SetList)selectedChipArray.Clear();
			AssignSelectedChips(chips);
		}

		public void AssignSelectedChips(object[] chips,bool SetList){
			if(SetList)selectedChipArray.Clear();
			AssignSelectedChips(chips);
		}

		public ChipBase[] AllChip{
			get{
				List<ChipBase> list = new List<ChipBase>();
				model.ForEach(chip => list.Add(chip));
				return list.ToArray();
			}
		}

		public void AssignSelectedChips(object[] chips){
			foreach(object o in chips){
				ChipBase chip = o as ChipBase;
				if(chip == null)break;
				if(!multiSelect)
					selectedChipArray.Add(Cursor.Parent);
				if(selectedChipArray.Contains(chip))
					selectedChipArray.Remove(chip);
				else
					selectedChipArray.Add(chip);

				selectedCount = selectedChipArray.Count;
				if(selectedCount != 0){
					SetCursor(null);
					multiSelect = true;
				}
				else{
					SetCursor(chip);
					multiSelect = false;
				}
			}
			if(OnSelectedChipChanged != null)OnSelectedChipChanged(null);
		}

		///<summery>
		///チップの選択を解除する。
		///</summery>
		public void ResetSelectedChips(){
			SelectedChipCount = 0;
		}

#if false
		///<summery>
		///.rcdファイルの記述内容から、データを読み取る。
		///</summery>
		public bool Parse(string filedata){
			filedata = Regex.Replace(filedata,"//.*\r\n","\r\n");
			filedata = Regex.Replace(filedata,"//.*\r","\r");
			filedata = Regex.Replace(filedata,"//.*\n","\n");
			Match blocks = Regex.Match(filedata,@"val\s*\{(?<vals>[^}]*)}\s*key\s*\{(?<keys>[^}]*)}\s*body\s*\{(?<body>.*)}\s*(?<scripttype>script|lua)\s*\{(?<script>.*)}",RegexOptions.Singleline | RegexOptions.IgnoreCase);
			if(!blocks.Success){
				blocks = Regex.Match(filedata,@"val\s*\{(?<vals>[^}]*)}\s*key\s*\{(?<keys>[^}]*)}\s*body\s*\{(?<body>.*)}",RegexOptions.Singleline | RegexOptions.IgnoreCase);
			}

			string buffer;
			if(blocks.Success){
				vals.Parse(blocks.Result("${vals}"));
				keys.Parse(blocks.Result("${keys}"),vals);
				model.Parse(blocks.Result("${body}"));
				buffer = blocks.Result("${scripttype}");
				luascript = (string.Compare(buffer,"lua",true) == 0);
				script = blocks.Result("${script}");
				if(script == "${script}")script = "";
				CheckBackTrack();
				return false;
			}
			else
				return true;

		}
#else //次期パース関数
		class SourceIterator : IEnumerable<char> {
			public SourceIterator(string s) {
				str = s;
			}
			string str;
			public IEnumerator<char> GetEnumerator() {
				string _str = str;
				bool inComment = false;
				bool inString = false;
				int blockLevel = 0;
				int i;
				for (i = 0; i < _str.Length; i++) {
					if (blockLevel == 0 && !inComment && !inString &&
						(_str[i + 0] == 'l' || _str[i + 0] == 'L') &&
						(_str[i + 1] == 'u' || _str[i + 1] == 'U') &&
						(_str[i + 2] == 'a' || _str[i + 2] == 'A'))
						break;	// luaはコメントなどの処理は行わない


					if (!inString && _str[i] == '/' && _str[i + 1] == '/') {
						inComment = true;
					}
					if (!inComment && _str[i] == '"') {
						inString = !inString;
					}
					if (_str[i] == '\n') {
						inComment = false;
					}

					if (!inComment) {
						if (_str[i] == '{') blockLevel++;
						else if (_str[i] == '}') blockLevel--;
						yield return _str[i];
					}
				}

				while (i < _str.Length) {
					yield return _str[i++];
				}
			}

			#region IEnumerable メンバ

			IEnumerator IEnumerable.GetEnumerator() {
				throw new NotImplementedException();
			}

			#endregion
		}

		///<summery>
		///.rcdファイルの記述内容から、データを読み取る。
		///<returns>エラーが発生した場合はtrue</returns>
		///</summery>
		public bool Parse(string filedata) {
			var tokenBuffer = new StringBuilder();
			var val = new StringBuilder();
			var key = new StringBuilder();
			var body = new StringBuilder();
			var scr = new StringBuilder();

			var itr = new SourceIterator(filedata);
			foreach (char c in itr) {
				tokenBuffer.Append(c);
			}
			// ここから先は従来の処理

			Match blocks = Regex.Match(filedata, @"val\s*\{(?<vals>[^}]*)}\s*key\s*\{(?<keys>[^}]*)}\s*body\s*\{(?<body>.*)}\s*(?<scripttype>script|lua)\s*\{(?<script>.*)}", RegexOptions.Singleline | RegexOptions.IgnoreCase);
			if (!blocks.Success) {
				blocks = Regex.Match(filedata, @"val\s*\{(?<vals>[^}]*)}\s*key\s*\{(?<keys>[^}]*)}\s*body\s*\{(?<body>.*)}", RegexOptions.Singleline | RegexOptions.IgnoreCase);
			}

			string buffer;
			if (blocks.Success) {
				vals.Parse(blocks.Result("${vals}"));
				keys.Parse(blocks.Result("${keys}"), vals);
				model.Parse(blocks.Result("${body}"));
				buffer = blocks.Result("${scripttype}");
				luascript = (string.Compare(buffer, "lua", true) == 0);
				script = blocks.Result("${script}");
				if (script == "${script}") script = "";
				//CheckBackTrack();
				return false;
			}
			else
				return true;
//			throw new Exception("Test finished.");
		}
#endif
		public void Save(string FileName,System.Collections.Specialized.NameValueCollection others){
			StreamWriter file = new StreamWriter(FileName,false,System.Text.Encoding.GetEncoding(932));
			
			file.WriteLine("RCMFORMAT VERSION 1.1");
			
			file.WriteLine("VALS");
			foreach(ValEntry v in vals.List){
				file.WriteLine("{0},{1},{2},{3},{4},{5}",v.ValName.ToString(),v.Default.ToString(),v.Min.ToString(),v.Max.ToString(),v.Step.ToString(),v.Disp.ToString());
			}
			file.WriteLine("VALS END");
			
			file.WriteLine("KEYS");
			for(int i = 0;i < Environment.KeyCount;i++){
				foreach(KeyEntry.KeyEntryWork w in keys[i].Works){
					file.WriteLine(keys[i].Key_ID.ToString() + "," + w.Target.ValName + "," + w.Step.ToString("G9"));
				}
			}
			file.WriteLine("KEYS END");

			file.WriteLine("BODY");

			{ // シリアル化
				Queue<KeyValuePair<ChipBase, int>> procQueue = new Queue<KeyValuePair<ChipBase, int>>();
				procQueue.Enqueue(new KeyValuePair<ChipBase, int>(model.root, -1));

				int index = 0;
				while(procQueue.Count > 0){
					var c = procQueue.Dequeue();

					foreach (var ch in c.Key.Children) {
						if (ch != null) {
							procQueue.Enqueue(new KeyValuePair<ChipBase, int>(ch, index));
						}
					}

					var target = c.Key;

					var cols = new List<string>() {
						target.ChipType.ToString(),
						target.JointPosition.ToString(),
						target.Name,
					};
					foreach(var attr in target.AttrNameList) {
						cols.Add(target[attr].ToString());
					}
					cols.Add(c.Value.ToString());
					if (target.Comment != null) {
						foreach (char ch in target.Comment)
							cols.Add(((int)ch).ToString());
					}

					file.WriteLine(string.Join(",", cols.ToArray()));

					index++;
				}
			}
			file.WriteLine("BODY END");

			file.WriteLine(luascript ? "SCRIPT LUA" : "SCRIPT");
			string[] lines = script.Split('\n');
			file.WriteLine(lines.Length.ToString());
			for(int i = 0;i < lines.Length;i++)
				file.WriteLine(lines[i].Replace("\r",""));
			file.WriteLine("SCRIPT END");

			file.WriteLine("OTHER");
			for(int i = 0;i < others.Count;i++)
				file.WriteLine("{0}\t{1}",others.GetKey(i),others.Get(i));
			file.WriteLine("OTHER END");
			file.Close();
		}

		
		public NameValueCollection Load(string FileName){
			System.Globalization.NumberFormatInfo formatinfo = System.Globalization.NumberFormatInfo.InvariantInfo;
			StreamReader file = new StreamReader(FileName,System.Text.Encoding.GetEncoding(932));
			NameValueCollection opts = new NameValueCollection();
			string version;
			string input;
			string[] parameters,parts;

			/*
			 * ブロック概要
			 *	VALS
			 *		変数
			 *	KEYS
			 *		キー動作
			 *	BODY
			 *		モデル定義
			 *	SCRIPT
			 *		スクリプト部
			 *	OTHER
			 *		その他、固有の設定など
			 */

			version = file.ReadLine();
			input = file.ReadLine();

			ValEntry tmpV;
			ChipBase chip;
			ChipBase[] array = {	
									 new CoreChip(this,null,JointPosition.NULL),
									 new NormalChip(this,null,JointPosition.NULL),
									 new FrameChip(this,null,JointPosition.NULL),
									 new RudderChip(this,null,JointPosition.NULL),
									 new RudderFrameChip(this,null,JointPosition.NULL),
									 new TrimChip(this,null,JointPosition.NULL),
									 new TrimFrameChip(this,null,JointPosition.NULL),
									 new WheelChip(this,null,JointPosition.NULL),
									 new RLWChip(this,null,JointPosition.NULL),
									 new JetChip(this,null,JointPosition.NULL),
									 new WeightChip(this,null,JointPosition.NULL),
									 new CowlChip(this,null,JointPosition.NULL),
									 new ArmChip(this,null,JointPosition.NULL)
								 };
			int i;
			ChipAttribute attr = new ChipAttribute();


			try{
				var chips = new List<ChipBase>();
				switch (version){
					case "RCMFORMAT VERSION 1.0":
						#region Version 1.0
						while(input != null && input != "VALS"){
							input = file.ReadLine();
						}
						while(input != null && input != "VALS END"){
							input = file.ReadLine();
							parameters = input.Split(',');
							if(parameters.Length != 6)
								continue;
							try{
								tmpV = this.vals.Add(parameters[0]);
								tmpV.Default = float.Parse(parameters[1]);
								tmpV.Min = float.Parse(parameters[2]);
								tmpV.Max = float.Parse(parameters[3]);
								tmpV.Step = float.Parse(parameters[4]);
								tmpV.Disp = bool.Parse(parameters[5]);
							}
							catch{
								continue;
							}
						}

						while(input != null && input != "KEYS"){
							input = file.ReadLine();
						}
						i = 0;
						try{
							while(input != null && input != "KEYS END"){
								input = file.ReadLine();
								parameters = input.Split(',');
								foreach(string s in parameters){
									parts = s.Split(':');
									if(parts.Length < 2)continue;
									keys[i++].AssignWork(vals[parts[0]],float.Parse(parts[1]));
								}
							}
						}catch{}

						while(input != null && input != "BODY"){
							input = file.ReadLine();
						}
						input = file.ReadLine();
						while(input != null && input != "BODY END"){
							parameters = input.Split(',');
							if(parameters[0] == "0"){
								//コアだったときの処理
								model.root.Name = parameters[2];
								model.root.ChipColor.SetValue(parameters[3],vals);
								attr = new ChipAttribute();
								attr.SetValue(parameters[4],vals);
								model.root["User1"] = attr;
								attr = new ChipAttribute();
								attr.SetValue(parameters[5],vals);
								model.root["User2"] = attr;

								chips.Add(model.root);
							}
							else{
								i = 4;
								chip = array[int.Parse(parameters[0])].Clone(false,null);
								chip.JointPosition = (JointPosition)(byte.Parse(parameters[1]));
								chip.Name = parameters[2];
								chip.ChipColor.SetValue(parameters[3],vals);
								parts = chip.AttrNameList;
								for(int j = 0;j < parts.Length;j++){
									attr = new ChipAttribute();
									attr.SetValue(parameters[i++],vals);
									chip[parts[j]] = attr;
								}
								chip.Attach(chips[int.Parse(parameters[i])],chip.JointPosition);

								chips.Add(chip);
							}
							input = file.ReadLine();
						}

						#endregion
						break;
					case "RCMFORMAT VERSION 1.1":
						#region Version 1.1
						while(input != null && input != "VALS"){
							input = file.ReadLine();
						}
						while(input != null && input != "VALS END"){
							input = file.ReadLine();
							parameters = input.Split(',');
							if(parameters.Length != 6)
								continue;
							try{
								tmpV = this.vals.Add(parameters[0]);
								tmpV.Default = float.Parse(parameters[1],formatinfo);
								tmpV.Min = float.Parse(parameters[2],formatinfo);
								tmpV.Max = float.Parse(parameters[3],formatinfo);
								tmpV.Step = float.Parse(parameters[4],formatinfo);
								tmpV.Disp = bool.Parse(parameters[5]);
							}
							catch{
								continue;
							}
						}

						while(input != null && input != "KEYS"){
							input = file.ReadLine();
						}
						i = 0;
					
						input = file.ReadLine();
						while(input != null && input != "KEYS END"){
							parameters = input.Split(',');
							keys[int.Parse(parameters[0])].AssignWork(vals[parameters[1]],float.Parse(parameters[2]));
							input = file.ReadLine();
						}


						while (input != null && input != "BODY") {
							input = file.ReadLine();
						}
						input = file.ReadLine();
						while (input != null && input != "BODY END") {
							parameters = input.Split(',');
							if (parameters[0] == "0") {
								//コアだったときの処理
								model.root.Name = parameters[2];
								model.root.ChipColor.SetValue(parameters[3], vals);
								attr = new ChipAttribute();
								attr.SetValue(parameters[4], vals);
								model.root["User1"] = attr;
								attr = new ChipAttribute();
								attr.SetValue(parameters[5], vals);
								model.root["User2"] = attr;
								if (parameters.Length > 7)
									for (int j = 7; j < parameters.Length; j++)
										model.root.Comment += ((char)int.Parse(parameters[j])).ToString();
								chips.Add(model.root);
							}
							else {
								i = 3;
								chip = array[int.Parse(parameters[0])].Clone(false, null);
								chip.JointPosition = (JointPosition)(byte.Parse(parameters[1]));
								chip.Name = parameters[2];
								parts = chip.AttrNameList;
								for (int j = 0; j < parts.Length; j++) {
									attr = new ChipAttribute();
									attr.SetValue(parameters[i++], vals);
									chip[parts[j]] = attr;
								}
								if (parameters.Length > i + 1)
									for (int j = i + 1; j < parameters.Length; j++)
										chip.Comment += ((char)int.Parse(parameters[j])).ToString();

								chip.Attach(chips[int.Parse(parameters[i])], chip.JointPosition);

								chips.Add(model.root);
							}
							input = file.ReadLine();
						}

						while (input != null && !input.StartsWith("SCRIPT")){
							input = file.ReadLine();
						}
						if(input.EndsWith("LUA"))
							this.luascript = true;
						input = file.ReadLine();
						while(input != null && input != "SCRIPT END"){
							i = int.Parse(input);
							for(int j = 0;j < i;j++)
								script += (input = file.ReadLine()) + "\r\n";
							input = file.ReadLine();
						}

						while(input != null && input != "OTHER"){
							input = file.ReadLine();
						}
						while(input != null && input != "OTHER END"){
							parameters = (input = file.ReadLine()).Split('\t');
							if(parameters.Length != 2)continue;
							opts.Add(parameters[0],parameters[1]);
						}


						#endregion
						break;
					default:
						break;
				}

			}
			catch{
				opts = null;
			}
			finally{
				file.Close();

				//CheckBackTrack();
				SetCursor(model.root);
			}
			return opts;

		}

		///<summery>
		///重心を計算する。
		///</summery>
		public void CalcWeightCenter(){
			if(RegisteringAll)return;
			double totalWeight = 0.0;
			Vector3 totalVector = new Vector3();

			this.model.ForEach(c => {
				totalWeight += c.Weight;
				totalVector += c.WeightedVector;
			});

			//for(int i = 0;i < MaxChipCount;i++){
			//	if(ChipLib[i] == null)break;
			//	totalWeight += ChipLib[i].Weight;
			//	totalVector += ChipLib[i].WeightedVector;
			//}

			totalVector.Scale((float)(1 / totalWeight));
			weightCenter = totalVector;
		}

		///<summery>
		///最後に計算された重心を得る。
		///</summery>
		public Vector3 WeightCenter{
			get{
				return weightCenter;
			}
		}


		public MessageCallback OnSelectedChipChanged;
		public MessageCallback OnChipLibraryChanged;

		/// <summary>
		/// RCDデータ用の数値としてパースする
		/// </summary>
		/// <param name="text">パースする文字列</param>
		/// <exception>NumberFormatException</exception>
		/// <returns>パース結果</returns>
		public static float ParseNumber(string text) {
			text = text.Trim();
			if (text[0] == '#') {
				// 十六進数
				return (float)int.Parse(text.Substring(1), System.Globalization.NumberStyles.HexNumber);
			}
			else {
				return float.Parse(text);
			}
		}

		/// <summary>
		/// RCDデータ用の数値としてパースする
		/// </summary>
		/// <param name="text"></param>
		/// <returns>成功すればTrue</returns>
		public static bool TryParseNumber(string text) {
			float dummy;
			try
			{
				return TryParseNumber(text, out dummy);
			} catch
			{
				return false;
			}
		}
		/// <summary>
		/// RCDデータ用の数値としてパースする
		/// </summary>
		/// <param name="text"></param>
		/// <returns>成功すればTrue</returns>
		public static bool TryParseNumber(string text, out float result) {
			text = text.Trim();
			if (text[0] == '#') {
				int buffer;
				bool ret = int.TryParse(text.Substring(1), System.Globalization.NumberStyles.HexNumber, null, out buffer);
				result = (float)buffer;
				return ret;
			}
			else {
				return float.TryParse(text, out result);
			}
		}

		bool preview;
		public bool Preview {
			get { return preview; }
			set {
				preview = value;
				foreach (var v in vals.List) {
					if (v != null) {
						v.OnPreview = value;
						v.Now = v.Default;
					}
				}
			}
		}

	}

	//class IterationHelper {
	//	delegate bool NextFinder<T>(T item, out T nextItem);

	//}
}
