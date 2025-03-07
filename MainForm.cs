using System;
using System.Drawing;
using System.Collections;
using System.Collections.Specialized;
//using System.ComponentModel;
using System.Windows.Forms;
//using System.Data;
using System.IO;
using System.Runtime.InteropServices;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

using RigidChips;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Globalization;

namespace rcm {

	///<summery>
	///メインフォーム。
	///</summery>
	public class MainForm : System.Windows.Forms.Form {
		struct DragSign {
			public bool Draging;
			public int StartX, StartY;
			public int PrevX, PrevY;
		}
		enum UndoType {
			None,
			Added,
			Removed,
			Modified,
			LeftRotated,
			RightRotated,
			HorizonalReversed,
			VerticalReversed,
		}

		class UndoInfo {
			public UndoType type = UndoType.None;
			public ChipBase[] chips = null;
			public UndoInfo next = null;
		}

		Device device = null;              // 1. Create rendering device
		PresentParameters presentParams = new PresentParameters();
		Caps caps;

		CustomVertex.PositionOnly[] lineCV;

		bool CameraOrtho = false;
		float CamTheta = -(float)Math.PI / 4f;
		float CamPhi = (float)Math.PI * 7f / 8f;
		float CamDepth = 8f;
		VertexBuffer vbGuide = null;
		VertexBuffer vbWeight = null;

		DragSign draging;
		int MouseX, MouseY;
		bool LeastIsLeftButton = false;
		bool angledrag = false;
		bool palettedrag = false;

		Vector3 CamNow;
		Vector3 CamNext;
		int ScrollCount = 0;

		//ToolBarButton selected; // 古い仕様
		ToolStripButton selectedButton;

		bool Pause = false;
		bool deviceLost = false;

		UndoInfo undo = new UndoInfo();

		const double RadToDeg = 180.0 / Math.PI;
		const double DegToRad = Math.PI / 180.0;

		ChipBase clipboard = null;
		JointPosition jointPositionBuffer = JointPosition.NULL;

		public RigidChips.Environment rcdata;
		DrawOptions drawOption = new DrawOptions();
		OutputOptions outputOption = new OutputOptions();
		EditOptions editOption = new EditOptions();
		RigidChips.XFile weightBall;
		RigidChips.XFile multiSelCursor;

		ConfigForm configwindow;

		bool modified;
		bool parameterChanged;
		string editingFileName;

		string[] Arguments;

		private System.Windows.Forms.TabControl tabGI;
		private System.Windows.Forms.TabPage tpAngle;
		private System.Windows.Forms.TabPage tpPalette;
		private System.Windows.Forms.Label labelPaletteError;
		private System.Windows.Forms.ContextMenu ctmPalette;
		private System.Windows.Forms.MenuItem miPaletteAllPaint;
		private System.Windows.Forms.MenuItem miPaletteChildPaint;
		private System.Windows.Forms.MenuItem miFile;
		private System.Windows.Forms.MenuItem miFileNew;
		private System.Windows.Forms.MenuItem miFileOpen;
		private System.Windows.Forms.MenuItem miFileSave;
		private System.Windows.Forms.MenuItem miFileSaveAs;
		private System.Windows.Forms.MenuItem miFileImport;
		private System.Windows.Forms.MenuItem miFileExport;
		private System.Windows.Forms.ContextMenu ctmAngles;
		private System.Windows.Forms.MenuItem miListCut;
		private System.Windows.Forms.MenuItem miListCopy;
		private System.Windows.Forms.MenuItem miListAdd;
		private System.Windows.Forms.MenuItem menuItem10;
		private System.Windows.Forms.MenuItem miListSelect;
		private System.Windows.Forms.MenuItem miChipComment;
		private System.Windows.Forms.MenuItem menuItem7;
		private System.Windows.Forms.MenuItem miCommentEdit;
		private System.Windows.Forms.MenuItem miCommentDelete;
		private System.Windows.Forms.MenuItem miTool;
		private System.Windows.Forms.MenuItem miToolVal;
		private System.Windows.Forms.MenuItem miToolKey;
		private System.Windows.Forms.MenuItem miToolScript;
		private System.Windows.Forms.MenuItem miConfig;
		private System.Windows.Forms.MenuItem miConfigDraw;
		private System.Windows.Forms.MenuItem miConfigOutput;
		private System.Windows.Forms.MenuItem miHelp;
		private System.Windows.Forms.MenuItem miHelpOpen;
		private System.Windows.Forms.MenuItem miHelpReadme;
		private System.Windows.Forms.MenuItem miHelpVersion;
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.MenuItem menuItem3;
		private System.Windows.Forms.MenuItem miToolComment;
		private System.Windows.Forms.MenuItem miToolPrev;
		private System.Windows.Forms.MenuItem miToolSend;
		private System.Windows.Forms.MenuItem miToolTree;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem miFileQuit;
		private System.Windows.Forms.MenuItem miConfigEdit;
		private System.Windows.Forms.MenuItem miAngleGrid0;
		private System.Windows.Forms.MenuItem miAngleGrid1;
		private System.Windows.Forms.MenuItem miAngleGrid5;
		private System.Windows.Forms.MenuItem miAngleGrid15;
		private System.Windows.Forms.MenuItem miAngleGrid30;
		private System.Windows.Forms.MenuItem miAngleGrid90;
		private System.Windows.Forms.MenuItem miEdit;
		private System.Windows.Forms.MenuItem menuItem6;
		private System.Windows.Forms.MenuItem miEditCut;
		private System.Windows.Forms.MenuItem miEditCopy;
		private System.Windows.Forms.MenuItem miEditDelete;
		private System.Windows.Forms.MenuItem miEditView;
		private System.Windows.Forms.MenuItem miEditViewTop;
		private System.Windows.Forms.MenuItem miEditSelectParent;
		private System.Windows.Forms.MenuItem miEditSelectCore;
		private System.Windows.Forms.MenuItem menuItem14;
		private System.Windows.Forms.MenuItem miEditViewRight;
		private System.Windows.Forms.MenuItem miEditViewLeft;
		private System.Windows.Forms.MenuItem miEditViewBottom;
		private System.Windows.Forms.MenuItem miEditVierFront;
		private System.Windows.Forms.MenuItem miEditViewRear;
		private System.Windows.Forms.MenuItem miEditViewUser;
		private System.Windows.Forms.MenuItem miEditInfo;
		private System.Windows.Forms.MenuItem miColor;
		private System.Windows.Forms.MenuItem miColorToAll;
		private System.Windows.Forms.MenuItem miColorToChild;
		private System.Windows.Forms.MenuItem miEditSelect;
		private System.Windows.Forms.MenuItem miEditSelectChild;
		private System.Windows.Forms.MenuItem miEditSelectAll;
		private System.Windows.Forms.Button btnListAdd;
		private System.Windows.Forms.Button btnEditPanel;
		private ToolStripContainer toolStripContainer1;
		private MenuStrip menuStrip1;
		private ToolStripMenuItem ファイルFToolStripMenuItem;
		private ToolStripMenuItem 新規作成ToolStripMenuItem;
		private ToolStripMenuItem 開くToolStripMenuItem;
		private ToolStripMenuItem 上書き保存ToolStripMenuItem;
		private ToolStripMenuItem 名前をつけて保存ToolStripMenuItem;
		private ToolStripSeparator toolStripSeparator1;
		private ToolStripMenuItem 編集EToolStripMenuItem;
		private ToolStripMenuItem ツールTToolStripMenuItem;
		private ToolStripMenuItem 設定CToolStripMenuItem;
		private ToolStripMenuItem ヘルプHToolStripMenuItem;
		private ToolStripMenuItem rCDTXTを開くToolStripMenuItem;
		private ToolStripMenuItem rCDで保存ToolStripMenuItem;
		private ToolStripSeparator toolStripSeparator2;
		private ToolStripMenuItem 終了ToolStripMenuItem;
		private ToolStrip toolStrip1;
		private ToolStripButton tsbSelectMode;
		private ToolStripButton tsbCut;
		private ToolStripButton tsbCopy;
		private ToolStripSeparator toolStripSeparator3;
		private ToolStripButton tsbInsert;
		private ToolStripButton tsbRemove;
		private ToolStripSeparator toolStripSeparator4;
		private ToolStripButton tsbZoom;
		private ToolStripButton tsbMooz;
		private ToolStripButton tsbCameraMode;
		private ToolStripButton tsbAutoCamera;
		private ToolStripSeparator toolStripSeparator5;
		private ToolStripButton tsbChipMode;
		private ToolStripButton tsbFrameMode;
		private ToolStripButton tsbRudderMode;
		private ToolStripButton tsbRudderFMode;
		private ToolStripButton tsbTrimMode;
		private ToolStripButton tsbTrimFMode;
		private ToolStripButton tsbWheelMode;
		private ToolStripButton tsbRLWMode;
		private ToolStripSeparator toolStripSeparator6;
		private ToolStripButton tsbJetMode;
		private ToolStripButton tsbWeightMode;
		private ToolStripButton tsbCowlMode;
		private ToolStripButton tsbArmMode;
		private ToolStripMenuItem 切り取りTToolStripMenuItem;
		private ToolStripMenuItem コピーCToolStripMenuItem;
		private ToolStripMenuItem 貼り付けPToolStripMenuItem;
		private ToolStripSeparator toolStripMenuItem1;
		private ToolStripMenuItem 視点変更VToolStripMenuItem;
		private ToolStripMenuItem 選択SToolStripMenuItem;
		private ToolStripMenuItem モデル情報SToolStripMenuItem;
		private MenuItem menuItem2;
		private MenuItem miPaletteShowDlg;
		private ColorDialog dlgColor;
		private MenuItem miListDelete;
		private Panel panelAttr;
		private Panel panelAttrValue;
		private ComboBox cmbAttrItem9;
		private ComboBox cmbAttrItem8;
		private ComboBox cmbAttrItem7;
		private ComboBox cmbAttrItem6;
		private ComboBox cmbAttrItem5;
		private ComboBox cmbAttrItem4;
		private ComboBox cmbAttrItem3;
		private ComboBox cmbAttrItem2;
		private ComboBox cmbAttrItem1;
		private ComboBox cmbAttrItem0;
		private Label labelAttrValue;
		private Splitter splAttr;
		private Panel panelAttrName;
		private Label labelAttrItem9;
		private Label labelAttrItem8;
		private Label labelAttrItem7;
		private Label labelAttrItem6;
		private Label labelAttrItem5;
		private Label labelAttrItem4;
		private Label labelAttrItem3;
		private Label labelAttrItem2;
		private Label labelAttrItem1;
		private Label labelAttrItem0;
		private Label labelAttrName;
		private ToolStripButton tsbPasteMode;
		bool Initialized = false;

		[DllImport("gdi32.dll")]
		static extern int GetPixel(IntPtr hDC, int dwX, int dwY);
		[DllImport("user32.dll")]
		static extern int RegisterWindowMessageA(string msgId);
		[DllImport("user32.dll")]
		static extern int PostMessageA(int hwd, int msg, int wparam, int lparam);
		const int hWnd_Broadcast = -1;

		const int Msg_RcLoadStart = 0;
		const int Msg_RcLoadChar = 1;
		const int Msg_RcLoadEnd = 2;
		int WM_RIGIDCHIP_LOAD;

		private rcm.TreeForm treeview;
		private rcm.ScriptForm scriptform;
		private System.Windows.Forms.Panel panelCtrl;
		private System.Windows.Forms.ListBox lstSouth;
		private System.Windows.Forms.ListBox lstNorth;
		private System.Windows.Forms.ListBox lstEast;
		private System.Windows.Forms.ListBox lstWest;
		private System.Windows.Forms.MainMenu mnuMain;
		private System.Windows.Forms.Label labelTip;
		private System.Windows.Forms.TextBox txtName;
		private System.Windows.Forms.Label labelName;
		private System.Windows.Forms.Button buttonSelChip;
		private System.Windows.Forms.Button btnVal;
		private System.Windows.Forms.Button btnKey;
		private System.Windows.Forms.Timer tmr;
		private System.Windows.Forms.ImageList imgIcons;
		private System.Windows.Forms.ToolTip ttMain;

		private System.Windows.Forms.Label[] labelAttrItems;
		private System.Windows.Forms.Timer tmrScroll;
		private System.Windows.Forms.Button btnRootChip;
		private System.Windows.Forms.ContextMenu ctmChipType;
		private System.Windows.Forms.MenuItem miChangeChip;
		private System.Windows.Forms.MenuItem miChangeFrame;
		private System.Windows.Forms.MenuItem miChangeRudder;
		private System.Windows.Forms.MenuItem miChangeRudderF;
		private System.Windows.Forms.MenuItem miChangeTrim;
		private System.Windows.Forms.MenuItem miChangeTrimF;
		private System.Windows.Forms.MenuItem ctmSeparator;
		private System.Windows.Forms.MenuItem miChangeWeight;
		private System.Windows.Forms.MenuItem miChangeCowl;
		private System.Windows.Forms.MenuItem miChangeWheel;
		private System.Windows.Forms.MenuItem miChangeRLW;
		private System.Windows.Forms.MenuItem miChangeJet;
		private System.Windows.Forms.MenuItem miChangeArm;
		private System.Windows.Forms.MenuItem menuItem8;
		private System.Windows.Forms.ImageList imgIconsL;
		private System.Windows.Forms.ContextMenu ctmChildList;
		private System.Windows.Forms.MenuItem menuItem13;
		private System.Windows.Forms.MenuItem menuItem19;
		private System.Windows.Forms.MenuItem miCut;
		private System.Windows.Forms.MenuItem miCopy;
		private System.Windows.Forms.MenuItem miDelete;
		private System.Windows.Forms.MenuItem miChange;
		private System.Windows.Forms.MenuItem miRotateRight;
		private System.Windows.Forms.MenuItem miRotateLeft;
		private System.Windows.Forms.MenuItem menuItem5;
		private System.Windows.Forms.MenuItem miReverseX;
		private System.Windows.Forms.MenuItem miReverseY;
		private System.Windows.Forms.MenuItem miReverseZ;
		private PanelEx pictTarget;
		//private System.Windows.Forms.PictureBox pictTarget;
		private System.Windows.Forms.PictureBox pictAngle;
		private System.Windows.Forms.OpenFileDialog dlgOpen;
		private System.Windows.Forms.SaveFileDialog dlgSave;
		private System.Windows.Forms.PictureBox pictPalette;
		private System.Windows.Forms.ComboBox[] cmbAttrItems;
		private System.Windows.Forms.MenuItem[] miChangeTypeList;

		public MainForm(string[] args) {
			//
			// Windows フォーム デザイナ サポートに必要です。
			//
			InitializeComponent();

			labelAttrItems = new Label[10] {
				labelAttrItem0,
				labelAttrItem1,
				labelAttrItem2,
				labelAttrItem3,
				labelAttrItem4,
				labelAttrItem5,
				labelAttrItem6,
				labelAttrItem7,
				labelAttrItem8,
				labelAttrItem9,
			};
			cmbAttrItems = new ComboBox[10] {
				cmbAttrItem0,
				cmbAttrItem1,
				cmbAttrItem2,
				cmbAttrItem3,
				cmbAttrItem4,
				cmbAttrItem5,
				cmbAttrItem6,
				cmbAttrItem7,
				cmbAttrItem8,
				cmbAttrItem9, };
			miChangeTypeList = new MenuItem[13] {
				null,
				miChangeChip,
				miChangeRudder,
				miChangeTrim,
				miChangeFrame,
				miChangeRudderF,
				miChangeTrimF,
				miChangeWheel,
				miChangeRLW,
				miChangeJet,
				miChangeWeight,
				miChangeCowl,
				miChangeArm,
			};
			//Color clr = Color.FromArgb(0);
			//for (KnownColor i = KnownColor.ActiveBorder;
			//    i <= KnownColor.YellowGreen; i++) {
			//    clr = Color.FromKnownColor(i);
			//    if (clr.IsSystemColor == false) {
			//        cmbColor.Items.Add(i);
			//    }
			//}

			Modified = false;
			EditingFileName = "";

			Arguments = args;
		}

		/// <summary>
		/// 使用されているリソースに後処理を実行します。
		/// </summary>
		protected override void Dispose(bool disposing) {
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		#region Windows フォーム デザイナで生成されたコード
		/// <summary>
		/// デザイナ サポートに必要なメソッドです。このメソッドの内容を
		/// コード エディタで変更しないでください。
		/// </summary>
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.mnuMain = new System.Windows.Forms.MainMenu(this.components);
			this.miFile = new System.Windows.Forms.MenuItem();
			this.miFileNew = new System.Windows.Forms.MenuItem();
			this.miFileOpen = new System.Windows.Forms.MenuItem();
			this.miFileSave = new System.Windows.Forms.MenuItem();
			this.miFileSaveAs = new System.Windows.Forms.MenuItem();
			this.menuItem19 = new System.Windows.Forms.MenuItem();
			this.miFileImport = new System.Windows.Forms.MenuItem();
			this.miFileExport = new System.Windows.Forms.MenuItem();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.miFileQuit = new System.Windows.Forms.MenuItem();
			this.miEdit = new System.Windows.Forms.MenuItem();
			this.miEditCut = new System.Windows.Forms.MenuItem();
			this.miEditCopy = new System.Windows.Forms.MenuItem();
			this.miEditDelete = new System.Windows.Forms.MenuItem();
			this.menuItem6 = new System.Windows.Forms.MenuItem();
			this.miEditView = new System.Windows.Forms.MenuItem();
			this.miEditViewRight = new System.Windows.Forms.MenuItem();
			this.miEditViewLeft = new System.Windows.Forms.MenuItem();
			this.miEditViewTop = new System.Windows.Forms.MenuItem();
			this.miEditViewBottom = new System.Windows.Forms.MenuItem();
			this.miEditVierFront = new System.Windows.Forms.MenuItem();
			this.miEditViewRear = new System.Windows.Forms.MenuItem();
			this.menuItem14 = new System.Windows.Forms.MenuItem();
			this.miEditViewUser = new System.Windows.Forms.MenuItem();
			this.miEditSelect = new System.Windows.Forms.MenuItem();
			this.miEditSelectParent = new System.Windows.Forms.MenuItem();
			this.miEditSelectCore = new System.Windows.Forms.MenuItem();
			this.miEditSelectChild = new System.Windows.Forms.MenuItem();
			this.miEditSelectAll = new System.Windows.Forms.MenuItem();
			this.miEditInfo = new System.Windows.Forms.MenuItem();
			this.miTool = new System.Windows.Forms.MenuItem();
			this.miToolVal = new System.Windows.Forms.MenuItem();
			this.miToolKey = new System.Windows.Forms.MenuItem();
			this.miToolScript = new System.Windows.Forms.MenuItem();
			this.miToolComment = new System.Windows.Forms.MenuItem();
			this.menuItem3 = new System.Windows.Forms.MenuItem();
			this.miToolTree = new System.Windows.Forms.MenuItem();
			this.miToolPrev = new System.Windows.Forms.MenuItem();
			this.miToolSend = new System.Windows.Forms.MenuItem();
			this.miConfig = new System.Windows.Forms.MenuItem();
			this.miConfigDraw = new System.Windows.Forms.MenuItem();
			this.miConfigOutput = new System.Windows.Forms.MenuItem();
			this.miConfigEdit = new System.Windows.Forms.MenuItem();
			this.miHelp = new System.Windows.Forms.MenuItem();
			this.miHelpOpen = new System.Windows.Forms.MenuItem();
			this.miHelpReadme = new System.Windows.Forms.MenuItem();
			this.menuItem13 = new System.Windows.Forms.MenuItem();
			this.miHelpVersion = new System.Windows.Forms.MenuItem();
			this.labelTip = new System.Windows.Forms.Label();
			this.imgIcons = new System.Windows.Forms.ImageList(this.components);
			this.panelCtrl = new System.Windows.Forms.Panel();
			this.btnListAdd = new System.Windows.Forms.Button();
			this.btnRootChip = new System.Windows.Forms.Button();
			this.btnKey = new System.Windows.Forms.Button();
			this.btnVal = new System.Windows.Forms.Button();
			this.panelAttr = new System.Windows.Forms.Panel();
			this.panelAttrValue = new System.Windows.Forms.Panel();
			this.cmbAttrItem9 = new System.Windows.Forms.ComboBox();
			this.cmbAttrItem8 = new System.Windows.Forms.ComboBox();
			this.cmbAttrItem7 = new System.Windows.Forms.ComboBox();
			this.cmbAttrItem6 = new System.Windows.Forms.ComboBox();
			this.cmbAttrItem5 = new System.Windows.Forms.ComboBox();
			this.cmbAttrItem4 = new System.Windows.Forms.ComboBox();
			this.cmbAttrItem3 = new System.Windows.Forms.ComboBox();
			this.cmbAttrItem2 = new System.Windows.Forms.ComboBox();
			this.cmbAttrItem1 = new System.Windows.Forms.ComboBox();
			this.cmbAttrItem0 = new System.Windows.Forms.ComboBox();
			this.labelAttrValue = new System.Windows.Forms.Label();
			this.splAttr = new System.Windows.Forms.Splitter();
			this.panelAttrName = new System.Windows.Forms.Panel();
			this.labelAttrItem9 = new System.Windows.Forms.Label();
			this.labelAttrItem8 = new System.Windows.Forms.Label();
			this.labelAttrItem7 = new System.Windows.Forms.Label();
			this.labelAttrItem6 = new System.Windows.Forms.Label();
			this.labelAttrItem5 = new System.Windows.Forms.Label();
			this.labelAttrItem4 = new System.Windows.Forms.Label();
			this.labelAttrItem3 = new System.Windows.Forms.Label();
			this.labelAttrItem2 = new System.Windows.Forms.Label();
			this.labelAttrItem1 = new System.Windows.Forms.Label();
			this.labelAttrItem0 = new System.Windows.Forms.Label();
			this.labelAttrName = new System.Windows.Forms.Label();
			this.txtName = new System.Windows.Forms.TextBox();
			this.labelName = new System.Windows.Forms.Label();
			this.lstSouth = new System.Windows.Forms.ListBox();
			this.ctmChildList = new System.Windows.Forms.ContextMenu();
			this.miListAdd = new System.Windows.Forms.MenuItem();
			this.miListSelect = new System.Windows.Forms.MenuItem();
			this.menuItem10 = new System.Windows.Forms.MenuItem();
			this.miListCut = new System.Windows.Forms.MenuItem();
			this.miListCopy = new System.Windows.Forms.MenuItem();
			this.miListDelete = new System.Windows.Forms.MenuItem();
			this.lstNorth = new System.Windows.Forms.ListBox();
			this.lstEast = new System.Windows.Forms.ListBox();
			this.lstWest = new System.Windows.Forms.ListBox();
			this.buttonSelChip = new System.Windows.Forms.Button();
			this.imgIconsL = new System.Windows.Forms.ImageList(this.components);
			this.tabGI = new System.Windows.Forms.TabControl();
			this.tpAngle = new System.Windows.Forms.TabPage();
			this.pictAngle = new System.Windows.Forms.PictureBox();
			this.ctmAngles = new System.Windows.Forms.ContextMenu();
			this.miAngleGrid0 = new System.Windows.Forms.MenuItem();
			this.miAngleGrid1 = new System.Windows.Forms.MenuItem();
			this.miAngleGrid5 = new System.Windows.Forms.MenuItem();
			this.miAngleGrid15 = new System.Windows.Forms.MenuItem();
			this.miAngleGrid30 = new System.Windows.Forms.MenuItem();
			this.miAngleGrid90 = new System.Windows.Forms.MenuItem();
			this.tpPalette = new System.Windows.Forms.TabPage();
			this.pictPalette = new System.Windows.Forms.PictureBox();
			this.ctmPalette = new System.Windows.Forms.ContextMenu();
			this.miPaletteShowDlg = new System.Windows.Forms.MenuItem();
			this.menuItem2 = new System.Windows.Forms.MenuItem();
			this.miPaletteChildPaint = new System.Windows.Forms.MenuItem();
			this.miPaletteAllPaint = new System.Windows.Forms.MenuItem();
			this.labelPaletteError = new System.Windows.Forms.Label();
			this.tmr = new System.Windows.Forms.Timer(this.components);
			this.ttMain = new System.Windows.Forms.ToolTip(this.components);
			this.tmrScroll = new System.Windows.Forms.Timer(this.components);
			this.ctmChipType = new System.Windows.Forms.ContextMenu();
			this.miCut = new System.Windows.Forms.MenuItem();
			this.miCopy = new System.Windows.Forms.MenuItem();
			this.miDelete = new System.Windows.Forms.MenuItem();
			this.menuItem8 = new System.Windows.Forms.MenuItem();
			this.miChange = new System.Windows.Forms.MenuItem();
			this.miChangeChip = new System.Windows.Forms.MenuItem();
			this.miChangeFrame = new System.Windows.Forms.MenuItem();
			this.miChangeRudder = new System.Windows.Forms.MenuItem();
			this.miChangeRudderF = new System.Windows.Forms.MenuItem();
			this.miChangeTrim = new System.Windows.Forms.MenuItem();
			this.miChangeTrimF = new System.Windows.Forms.MenuItem();
			this.ctmSeparator = new System.Windows.Forms.MenuItem();
			this.miChangeWheel = new System.Windows.Forms.MenuItem();
			this.miChangeRLW = new System.Windows.Forms.MenuItem();
			this.miChangeJet = new System.Windows.Forms.MenuItem();
			this.miChangeWeight = new System.Windows.Forms.MenuItem();
			this.miChangeCowl = new System.Windows.Forms.MenuItem();
			this.miChangeArm = new System.Windows.Forms.MenuItem();
			this.miChipComment = new System.Windows.Forms.MenuItem();
			this.miCommentEdit = new System.Windows.Forms.MenuItem();
			this.miCommentDelete = new System.Windows.Forms.MenuItem();
			this.miColor = new System.Windows.Forms.MenuItem();
			this.miColorToAll = new System.Windows.Forms.MenuItem();
			this.miColorToChild = new System.Windows.Forms.MenuItem();
			this.menuItem7 = new System.Windows.Forms.MenuItem();
			this.miRotateRight = new System.Windows.Forms.MenuItem();
			this.miRotateLeft = new System.Windows.Forms.MenuItem();
			this.menuItem5 = new System.Windows.Forms.MenuItem();
			this.miReverseX = new System.Windows.Forms.MenuItem();
			this.miReverseY = new System.Windows.Forms.MenuItem();
			this.miReverseZ = new System.Windows.Forms.MenuItem();
			this.dlgOpen = new System.Windows.Forms.OpenFileDialog();
			this.dlgSave = new System.Windows.Forms.SaveFileDialog();
			this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.ファイルFToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.新規作成ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.開くToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.上書き保存ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.名前をつけて保存ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.rCDTXTを開くToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.rCDで保存ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.終了ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.編集EToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.切り取りTToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.コピーCToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.貼り付けPToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
			this.視点変更VToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.選択SToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.モデル情報SToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ツールTToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.設定CToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ヘルプHToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this.tsbSelectMode = new System.Windows.Forms.ToolStripButton();
			this.tsbCut = new System.Windows.Forms.ToolStripButton();
			this.tsbCopy = new System.Windows.Forms.ToolStripButton();
			this.tsbPasteMode = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.tsbInsert = new System.Windows.Forms.ToolStripButton();
			this.tsbRemove = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
			this.tsbZoom = new System.Windows.Forms.ToolStripButton();
			this.tsbMooz = new System.Windows.Forms.ToolStripButton();
			this.tsbCameraMode = new System.Windows.Forms.ToolStripButton();
			this.tsbAutoCamera = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
			this.tsbChipMode = new System.Windows.Forms.ToolStripButton();
			this.tsbFrameMode = new System.Windows.Forms.ToolStripButton();
			this.tsbRudderMode = new System.Windows.Forms.ToolStripButton();
			this.tsbRudderFMode = new System.Windows.Forms.ToolStripButton();
			this.tsbTrimMode = new System.Windows.Forms.ToolStripButton();
			this.tsbTrimFMode = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
			this.tsbWheelMode = new System.Windows.Forms.ToolStripButton();
			this.tsbRLWMode = new System.Windows.Forms.ToolStripButton();
			this.tsbJetMode = new System.Windows.Forms.ToolStripButton();
			this.tsbWeightMode = new System.Windows.Forms.ToolStripButton();
			this.tsbCowlMode = new System.Windows.Forms.ToolStripButton();
			this.tsbArmMode = new System.Windows.Forms.ToolStripButton();
			this.dlgColor = new System.Windows.Forms.ColorDialog();
			this.pictTarget = new rcm.PanelEx();
			this.btnEditPanel = new System.Windows.Forms.Button();
			this.panelCtrl.SuspendLayout();
			this.panelAttr.SuspendLayout();
			this.panelAttrValue.SuspendLayout();
			this.panelAttrName.SuspendLayout();
			this.tabGI.SuspendLayout();
			this.tpAngle.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictAngle)).BeginInit();
			this.tpPalette.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictPalette)).BeginInit();
			this.toolStripContainer1.ContentPanel.SuspendLayout();
			this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
			this.toolStripContainer1.SuspendLayout();
			this.menuStrip1.SuspendLayout();
			this.toolStrip1.SuspendLayout();
			this.pictTarget.SuspendLayout();
			this.SuspendLayout();
			// 
			// mnuMain
			// 
			this.mnuMain.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.miFile,
            this.miEdit,
            this.miTool,
            this.miConfig,
            this.miHelp});
			// 
			// miFile
			// 
			this.miFile.Index = 0;
			this.miFile.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.miFileNew,
            this.miFileOpen,
            this.miFileSave,
            this.miFileSaveAs,
            this.menuItem19,
            this.miFileImport,
            this.miFileExport,
            this.menuItem1,
            this.miFileQuit});
			this.miFile.Text = "File (&F)";
			this.miFile.Click += new System.EventHandler(this.miFile_Click);
			// 
			// miFileNew
			// 
			this.miFileNew.Index = 0;
			this.miFileNew.Shortcut = System.Windows.Forms.Shortcut.CtrlN;
			this.miFileNew.Text = "New Model (&N)";
			this.miFileNew.Click += new System.EventHandler(this.miFileNew_Click);
			// 
			// miFileOpen
			// 
			this.miFileOpen.Index = 1;
			this.miFileOpen.Shortcut = System.Windows.Forms.Shortcut.CtrlO;
			this.miFileOpen.Text = "Open Model (&O)";
			this.miFileOpen.Click += new System.EventHandler(this.miFileOpen_Click);
			// 
			// miFileSave
			// 
			this.miFileSave.Index = 2;
			this.miFileSave.Shortcut = System.Windows.Forms.Shortcut.CtrlS;
			this.miFileSave.Text = "Save Model (&S)";
			this.miFileSave.Click += new System.EventHandler(this.miFileSave_Click);
			// 
			// miFileSaveAs
			// 
			this.miFileSaveAs.Index = 3;
			this.miFileSaveAs.Shortcut = System.Windows.Forms.Shortcut.CtrlA;
			this.miFileSaveAs.Text = "Save Model As (&A)";
			this.miFileSaveAs.Click += new System.EventHandler(this.miFileSaveAs_Click);
			// 
			// menuItem19
			// 
			this.menuItem19.Index = 4;
			this.menuItem19.Text = "-";
			this.menuItem19.Visible = false;
			// 
			// miFileImport
			// 
			this.miFileImport.Index = 5;
			this.miFileImport.Shortcut = System.Windows.Forms.Shortcut.CtrlI;
			this.miFileImport.Text = "Open from RCD/TXT く(&I)";
			this.miFileImport.Visible = false;
			this.miFileImport.Click += new System.EventHandler(this.miFileImport_Click);
			// 
			// miFileExport
			// 
			this.miFileExport.Index = 6;
			this.miFileExport.Shortcut = System.Windows.Forms.Shortcut.CtrlE;
			this.miFileExport.Text = "Save in RCD format (&E)";
			this.miFileExport.Visible = false;
			this.miFileExport.Click += new System.EventHandler(this.miFileExport_Click);
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 7;
			this.menuItem1.Text = "-";
			// 
			// miFileQuit
			// 
			this.miFileQuit.Index = 8;
			this.miFileQuit.Text = "Exit RCM (&Q)";
			this.miFileQuit.Click += new System.EventHandler(this.miFileQuit_Click);
			// 
			// miEdit
			// 
			this.miEdit.Index = 1;
			this.miEdit.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.miEditCut,
            this.miEditCopy,
            this.miEditDelete,
            this.menuItem6,
            this.miEditView,
            this.miEditSelect,
            this.miEditInfo});
			this.miEdit.Text = "Edit (&E)";
			// 
			// miEditCut
			// 
			this.miEditCut.Index = 0;
			this.miEditCut.Shortcut = System.Windows.Forms.Shortcut.CtrlX;
			this.miEditCut.Text = "Cut (&T)";
			this.miEditCut.Click += new System.EventHandler(this.miCut_Click);
			// 
			// miEditCopy
			// 
			this.miEditCopy.Index = 1;
			this.miEditCopy.Text = "Copy (&C)";
			this.miEditCopy.Click += new System.EventHandler(this.miCopy_Click);
			// 
			// miEditDelete
			// 
			this.miEditDelete.Index = 2;
			this.miEditDelete.Shortcut = System.Windows.Forms.Shortcut.CtrlDel;
			this.miEditDelete.Text = "Delete (&D)";
			this.miEditDelete.Click += new System.EventHandler(this.miDelete_Click);
			// 
			// menuItem6
			// 
			this.menuItem6.Index = 3;
			this.menuItem6.Text = "-";
			// 
			// miEditView
			// 
			this.miEditView.Index = 4;
			this.miEditView.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.miEditViewRight,
            this.miEditViewLeft,
            this.miEditViewTop,
            this.miEditViewBottom,
            this.miEditVierFront,
            this.miEditViewRear,
            this.menuItem14,
            this.miEditViewUser});
			this.miEditView.Text = "Viewpoint change (&V)";
			this.miEditView.Visible = false;
			// 
			// miEditViewRight
			// 
			this.miEditViewRight.Index = 0;
			this.miEditViewRight.Text = "Right (&R)";
			// 
			// miEditViewLeft
			// 
			this.miEditViewLeft.Index = 1;
			this.miEditViewLeft.Text = "Left (&L)";
			// 
			// miEditViewTop
			// 
			this.miEditViewTop.Index = 2;
			this.miEditViewTop.Text = "Top (&T)";
			// 
			// miEditViewBottom
			// 
			this.miEditViewBottom.Index = 3;
			this.miEditViewBottom.Text = "Bottom (&B)";
			// 
			// miEditVierFront
			// 
			this.miEditVierFront.Index = 4;
			this.miEditVierFront.Text = "Front (&F)";
			// 
			// miEditViewRear
			// 
			this.miEditViewRear.Index = 5;
			this.miEditViewRear.Text = "Behind (&R)";
			// 
			// menuItem14
			// 
			this.menuItem14.Index = 6;
			this.menuItem14.Text = "-";
			// 
			// miEditViewUser
			// 
			this.miEditViewUser.Index = 7;
			this.miEditViewUser.Text = "Numeric Input... (&U)";
			// 
			// miEditSelect
			// 
			this.miEditSelect.Index = 5;
			this.miEditSelect.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.miEditSelectParent,
            this.miEditSelectCore,
            this.miEditSelectChild,
            this.miEditSelectAll});
			this.miEditSelect.Text = "Select... (&S)";
			// 
			// miEditSelectParent
			// 
			this.miEditSelectParent.Index = 0;
			this.miEditSelectParent.Shortcut = System.Windows.Forms.Shortcut.CtrlShiftP;
			this.miEditSelectParent.Text = "Parent Chip (&P)";
			this.miEditSelectParent.Click += new System.EventHandler(this.miEditSelectParent_Click);
			// 
			// miEditSelectCore
			// 
			this.miEditSelectCore.Index = 1;
			this.miEditSelectCore.Shortcut = System.Windows.Forms.Shortcut.CtrlShiftC;
			this.miEditSelectCore.Text = "Core Chip (&C)";
			this.miEditSelectCore.Click += new System.EventHandler(this.miEditSelectCore_Click);
			// 
			// miEditSelectChild
			// 
			this.miEditSelectChild.Index = 2;
			this.miEditSelectChild.Shortcut = System.Windows.Forms.Shortcut.CtrlShiftD;
			this.miEditSelectChild.Text = "All Connecected chips (&O)";
			this.miEditSelectChild.Click += new System.EventHandler(this.miEditSelectChild_Click);
			// 
			// miEditSelectAll
			// 
			this.miEditSelectAll.Index = 3;
			this.miEditSelectAll.Shortcut = System.Windows.Forms.Shortcut.CtrlShiftA;
			this.miEditSelectAll.Text = "All Chips (&A)";
			this.miEditSelectAll.Click += new System.EventHandler(this.miEditSelectAll_Click);
			// 
			// miEditInfo
			// 
			this.miEditInfo.Index = 6;
			this.miEditInfo.Text = "Model Information... (&I)";
			this.miEditInfo.Click += new System.EventHandler(this.miEditInfo_Click);
			// 
			// miTool
			// 
			this.miTool.Index = 2;
			this.miTool.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.miToolVal,
            this.miToolKey,
            this.miToolScript,
            this.miToolComment,
            this.menuItem3,
            this.miToolTree,
            this.miToolPrev,
            this.miToolSend});
			this.miTool.Text = "Tools (&T)";
			this.miTool.Click += new System.EventHandler(this.menuItem2_Click);
			// 
			// miToolVal
			// 
			this.miToolVal.Index = 0;
			this.miToolVal.Shortcut = System.Windows.Forms.Shortcut.F2;
			this.miToolVal.Text = "&Val{...} Edit";
			this.miToolVal.Click += new System.EventHandler(this.btnVal_Click);
			// 
			// miToolKey
			// 
			this.miToolKey.Index = 1;
			this.miToolKey.Shortcut = System.Windows.Forms.Shortcut.F3;
			this.miToolKey.Text = "&Key{...} Edit";
			this.miToolKey.Click += new System.EventHandler(this.btnKey_Click);
			// 
			// miToolScript
			// 
			this.miToolScript.Index = 2;
			this.miToolScript.Shortcut = System.Windows.Forms.Shortcut.F4;
			this.miToolScript.Text = "&Script/Lua{...} Edit";
			this.miToolScript.Click += new System.EventHandler(this.miToolScript_Click);
			// 
			// miToolComment
			// 
			this.miToolComment.Enabled = false;
			this.miToolComment.Index = 3;
			this.miToolComment.Shortcut = System.Windows.Forms.Shortcut.ShiftF4;
			this.miToolComment.Text = "Edit File Beginning Comment (&C)";
			// 
			// menuItem3
			// 
			this.menuItem3.Index = 4;
			this.menuItem3.Text = "-";
			// 
			// miToolTree
			// 
			this.miToolTree.Index = 5;
			this.miToolTree.Shortcut = System.Windows.Forms.Shortcut.F6;
			this.miToolTree.Text = "Tree View (&T)";
			this.miToolTree.Click += new System.EventHandler(this.miToolTree_Click);
			// 
			// miToolPrev
			// 
			this.miToolPrev.Index = 6;
			this.miToolPrev.Shortcut = System.Windows.Forms.Shortcut.F8;
			this.miToolPrev.Text = "Simple Motion Preview (&P)";
			this.miToolPrev.Click += new System.EventHandler(this.btnEditPanel_Click);
			// 
			// miToolSend
			// 
			this.miToolSend.Index = 7;
			this.miToolSend.Shortcut = System.Windows.Forms.Shortcut.F5;
			this.miToolSend.Text = "Preview on RigidChips (&R)";
			this.miToolSend.Click += new System.EventHandler(this.miToolSend_Click);
			// 
			// miConfig
			// 
			this.miConfig.Index = 3;
			this.miConfig.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.miConfigDraw,
            this.miConfigOutput,
            this.miConfigEdit});
			this.miConfig.Text = "Settings (&C)";
			// 
			// miConfigDraw
			// 
			this.miConfigDraw.Index = 0;
			this.miConfigDraw.Text = "Display Settings (&R)";
			this.miConfigDraw.Click += new System.EventHandler(this.miConfigDraw_Click);
			// 
			// miConfigOutput
			// 
			this.miConfigOutput.Index = 1;
			this.miConfigOutput.Text = "Format Settings (&O)";
			this.miConfigOutput.Click += new System.EventHandler(this.miConfigOutput_Click);
			// 
			// miConfigEdit
			// 
			this.miConfigEdit.Index = 2;
			this.miConfigEdit.Text = "Edit Operation Settings (&E)";
			this.miConfigEdit.Click += new System.EventHandler(this.miConfigEdit_Click);
			// 
			// miHelp
			// 
			this.miHelp.Index = 4;
			this.miHelp.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.miHelpOpen,
            this.miHelpReadme,
            this.menuItem13,
            this.miHelpVersion});
			this.miHelp.Text = "Help (&H)";
			// 
			// miHelpOpen
			// 
			this.miHelpOpen.Enabled = false;
			this.miHelpOpen.Index = 0;
			this.miHelpOpen.Text = "Show help";
			// 
			// miHelpReadme
			// 
			this.miHelpReadme.Index = 1;
			this.miHelpReadme.Text = "RCM manual.txt";
			this.miHelpReadme.Click += new System.EventHandler(this.miHelpReadme_Click);
			// 
			// menuItem13
			// 
			this.menuItem13.Index = 2;
			this.menuItem13.Text = "-";
			// 
			// miHelpVersion
			// 
			this.miHelpVersion.Index = 3;
			this.miHelpVersion.Text = "Version info";
			this.miHelpVersion.Click += new System.EventHandler(this.menuItem14_Click);
			// 
			// labelTip
			// 
			this.labelTip.Dock = System.Windows.Forms.DockStyle.Top;
			this.labelTip.Location = new System.Drawing.Point(0, 0);
			this.labelTip.Name = "labelTip";
			this.labelTip.Size = new System.Drawing.Size(795, 16);
			this.labelTip.TabIndex = 8;
			this.labelTip.Text = "Welcome to RCM (English)";
			this.labelTip.Click += new System.EventHandler(this.labelTip_Click);
			// 
			// imgIcons
			// 
			this.imgIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgIcons.ImageStream")));
			this.imgIcons.TransparentColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
			this.imgIcons.Images.SetKeyName(0, "");
			this.imgIcons.Images.SetKeyName(1, "");
			this.imgIcons.Images.SetKeyName(2, "");
			this.imgIcons.Images.SetKeyName(3, "");
			this.imgIcons.Images.SetKeyName(4, "");
			this.imgIcons.Images.SetKeyName(5, "");
			this.imgIcons.Images.SetKeyName(6, "");
			this.imgIcons.Images.SetKeyName(7, "");
			this.imgIcons.Images.SetKeyName(8, "");
			this.imgIcons.Images.SetKeyName(9, "");
			this.imgIcons.Images.SetKeyName(10, "");
			this.imgIcons.Images.SetKeyName(11, "");
			this.imgIcons.Images.SetKeyName(12, "");
			this.imgIcons.Images.SetKeyName(13, "");
			this.imgIcons.Images.SetKeyName(14, "");
			this.imgIcons.Images.SetKeyName(15, "");
			this.imgIcons.Images.SetKeyName(16, "");
			this.imgIcons.Images.SetKeyName(17, "");
			this.imgIcons.Images.SetKeyName(18, "");
			this.imgIcons.Images.SetKeyName(19, "");
			this.imgIcons.Images.SetKeyName(20, "");
			this.imgIcons.Images.SetKeyName(21, "");
			// 
			// panelCtrl
			// 
			this.panelCtrl.Controls.Add(this.btnListAdd);
			this.panelCtrl.Controls.Add(this.btnRootChip);
			this.panelCtrl.Controls.Add(this.btnKey);
			this.panelCtrl.Controls.Add(this.btnVal);
			this.panelCtrl.Controls.Add(this.panelAttr);
			this.panelCtrl.Controls.Add(this.txtName);
			this.panelCtrl.Controls.Add(this.labelName);
			this.panelCtrl.Controls.Add(this.lstSouth);
			this.panelCtrl.Controls.Add(this.lstNorth);
			this.panelCtrl.Controls.Add(this.lstEast);
			this.panelCtrl.Controls.Add(this.lstWest);
			this.panelCtrl.Controls.Add(this.buttonSelChip);
			this.panelCtrl.Controls.Add(this.tabGI);
			this.panelCtrl.Dock = System.Windows.Forms.DockStyle.Right;
			this.panelCtrl.Location = new System.Drawing.Point(795, 0);
			this.panelCtrl.Name = "panelCtrl";
			this.panelCtrl.Size = new System.Drawing.Size(270, 701);
			this.panelCtrl.TabIndex = 10;
			this.panelCtrl.Paint += new System.Windows.Forms.PaintEventHandler(this.panelB_Paint);
			// 
			// btnListAdd
			// 
			this.btnListAdd.Location = new System.Drawing.Point(32, 139);
			this.btnListAdd.Name = "btnListAdd";
			this.btnListAdd.Size = new System.Drawing.Size(16, 17);
			this.btnListAdd.TabIndex = 12;
			this.btnListAdd.Text = "+";
			this.btnListAdd.Visible = false;
			this.btnListAdd.Click += new System.EventHandler(this.btnListAdd_Click);
			// 
			// btnRootChip
			// 
			this.btnRootChip.Location = new System.Drawing.Point(11, 3);
			this.btnRootChip.Name = "btnRootChip";
			this.btnRootChip.Size = new System.Drawing.Size(60, 58);
			this.btnRootChip.TabIndex = 1;
			this.btnRootChip.Text = "Core Chip";
			this.btnRootChip.Click += new System.EventHandler(this.btnRootChip_Click);
			// 
			// btnKey
			// 
			this.btnKey.Location = new System.Drawing.Point(187, 154);
			this.btnKey.Name = "btnKey";
			this.btnKey.Size = new System.Drawing.Size(60, 26);
			this.btnKey.TabIndex = 8;
			this.btnKey.Text = "Key{...} ";
			this.btnKey.Click += new System.EventHandler(this.btnKey_Click);
			// 
			// btnVal
			// 
			this.btnVal.Location = new System.Drawing.Point(187, 122);
			this.btnVal.Name = "btnVal";
			this.btnVal.Size = new System.Drawing.Size(60, 26);
			this.btnVal.TabIndex = 7;
			this.btnVal.Text = "Val{...} ";
			this.btnVal.Click += new System.EventHandler(this.btnVal_Click);
			// 
			// panelAttr
			// 
			this.panelAttr.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.panelAttr.Controls.Add(this.panelAttrValue);
			this.panelAttr.Controls.Add(this.splAttr);
			this.panelAttr.Controls.Add(this.panelAttrName);
			this.panelAttr.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panelAttr.Location = new System.Drawing.Point(0, 459);
			this.panelAttr.Name = "panelAttr";
			this.panelAttr.Size = new System.Drawing.Size(270, 242);
			this.panelAttr.TabIndex = 10;
			// 
			// panelAttrValue
			// 
			this.panelAttrValue.Controls.Add(this.cmbAttrItem9);
			this.panelAttrValue.Controls.Add(this.cmbAttrItem8);
			this.panelAttrValue.Controls.Add(this.cmbAttrItem7);
			this.panelAttrValue.Controls.Add(this.cmbAttrItem6);
			this.panelAttrValue.Controls.Add(this.cmbAttrItem5);
			this.panelAttrValue.Controls.Add(this.cmbAttrItem4);
			this.panelAttrValue.Controls.Add(this.cmbAttrItem3);
			this.panelAttrValue.Controls.Add(this.cmbAttrItem2);
			this.panelAttrValue.Controls.Add(this.cmbAttrItem1);
			this.panelAttrValue.Controls.Add(this.cmbAttrItem0);
			this.panelAttrValue.Controls.Add(this.labelAttrValue);
			this.panelAttrValue.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panelAttrValue.Location = new System.Drawing.Point(104, 0);
			this.panelAttrValue.Name = "panelAttrValue";
			this.panelAttrValue.Size = new System.Drawing.Size(162, 238);
			this.panelAttrValue.TabIndex = 2;
			// 
			// cmbAttrItem9
			// 
			this.cmbAttrItem9.Dock = System.Windows.Forms.DockStyle.Top;
			this.cmbAttrItem9.Font = new System.Drawing.Font("MS Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.cmbAttrItem9.ImeMode = System.Windows.Forms.ImeMode.Off;
			this.cmbAttrItem9.Items.AddRange(new object[] {
            "(no variables)"});
			this.cmbAttrItem9.Location = new System.Drawing.Point(0, 197);
			this.cmbAttrItem9.Name = "cmbAttrItem9";
			this.cmbAttrItem9.Size = new System.Drawing.Size(162, 20);
			this.cmbAttrItem9.TabIndex = 20;
			this.cmbAttrItem9.Text = "comboBox1";
			this.cmbAttrItem9.TextChanged += new System.EventHandler(this.ChipInfo_TextChanged);
			this.cmbAttrItem9.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.cmbAttrItems_KeyPress);
			this.cmbAttrItem9.Leave += new System.EventHandler(this.cmbAttrItems_Leave);
			// 
			// cmbAttrItem8
			// 
			this.cmbAttrItem8.Dock = System.Windows.Forms.DockStyle.Top;
			this.cmbAttrItem8.Font = new System.Drawing.Font("MS Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.cmbAttrItem8.ImeMode = System.Windows.Forms.ImeMode.Off;
			this.cmbAttrItem8.Items.AddRange(new object[] {
            "(no variables)"});
			this.cmbAttrItem8.Location = new System.Drawing.Point(0, 177);
			this.cmbAttrItem8.Name = "cmbAttrItem8";
			this.cmbAttrItem8.Size = new System.Drawing.Size(162, 20);
			this.cmbAttrItem8.TabIndex = 19;
			this.cmbAttrItem8.Text = "comboBox1";
			this.cmbAttrItem8.TextChanged += new System.EventHandler(this.ChipInfo_TextChanged);
			this.cmbAttrItem8.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.cmbAttrItems_KeyPress);
			this.cmbAttrItem8.Leave += new System.EventHandler(this.cmbAttrItems_Leave);
			// 
			// cmbAttrItem7
			// 
			this.cmbAttrItem7.Dock = System.Windows.Forms.DockStyle.Top;
			this.cmbAttrItem7.Font = new System.Drawing.Font("MS Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.cmbAttrItem7.ImeMode = System.Windows.Forms.ImeMode.Off;
			this.cmbAttrItem7.Items.AddRange(new object[] {
            "(no variables)"});
			this.cmbAttrItem7.Location = new System.Drawing.Point(0, 157);
			this.cmbAttrItem7.Name = "cmbAttrItem7";
			this.cmbAttrItem7.Size = new System.Drawing.Size(162, 20);
			this.cmbAttrItem7.TabIndex = 18;
			this.cmbAttrItem7.Text = "comboBox1";
			this.cmbAttrItem7.TextChanged += new System.EventHandler(this.ChipInfo_TextChanged);
			this.cmbAttrItem7.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.cmbAttrItems_KeyPress);
			this.cmbAttrItem7.Leave += new System.EventHandler(this.cmbAttrItems_Leave);
			// 
			// cmbAttrItem6
			// 
			this.cmbAttrItem6.Dock = System.Windows.Forms.DockStyle.Top;
			this.cmbAttrItem6.Font = new System.Drawing.Font("MS Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.cmbAttrItem6.ImeMode = System.Windows.Forms.ImeMode.Off;
			this.cmbAttrItem6.Items.AddRange(new object[] {
            "(no variables)"});
			this.cmbAttrItem6.Location = new System.Drawing.Point(0, 137);
			this.cmbAttrItem6.Name = "cmbAttrItem6";
			this.cmbAttrItem6.Size = new System.Drawing.Size(162, 20);
			this.cmbAttrItem6.TabIndex = 17;
			this.cmbAttrItem6.Text = "cmbAttrItem6";
			this.cmbAttrItem6.TextChanged += new System.EventHandler(this.ChipInfo_TextChanged);
			this.cmbAttrItem6.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.cmbAttrItems_KeyPress);
			this.cmbAttrItem6.Leave += new System.EventHandler(this.cmbAttrItems_Leave);
			// 
			// cmbAttrItem5
			// 
			this.cmbAttrItem5.Dock = System.Windows.Forms.DockStyle.Top;
			this.cmbAttrItem5.Font = new System.Drawing.Font("MS Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.cmbAttrItem5.ImeMode = System.Windows.Forms.ImeMode.Off;
			this.cmbAttrItem5.Items.AddRange(new object[] {
            "(no variables)"});
			this.cmbAttrItem5.Location = new System.Drawing.Point(0, 117);
			this.cmbAttrItem5.Name = "cmbAttrItem5";
			this.cmbAttrItem5.Size = new System.Drawing.Size(162, 20);
			this.cmbAttrItem5.TabIndex = 16;
			this.cmbAttrItem5.Text = "cmbAttrItem5";
			this.cmbAttrItem5.TextChanged += new System.EventHandler(this.ChipInfo_TextChanged);
			this.cmbAttrItem5.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.cmbAttrItems_KeyPress);
			this.cmbAttrItem5.Leave += new System.EventHandler(this.cmbAttrItems_Leave);
			// 
			// cmbAttrItem4
			// 
			this.cmbAttrItem4.Dock = System.Windows.Forms.DockStyle.Top;
			this.cmbAttrItem4.Font = new System.Drawing.Font("MS Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.cmbAttrItem4.ImeMode = System.Windows.Forms.ImeMode.Off;
			this.cmbAttrItem4.Items.AddRange(new object[] {
            "(no variables)"});
			this.cmbAttrItem4.Location = new System.Drawing.Point(0, 97);
			this.cmbAttrItem4.Name = "cmbAttrItem4";
			this.cmbAttrItem4.Size = new System.Drawing.Size(162, 20);
			this.cmbAttrItem4.TabIndex = 15;
			this.cmbAttrItem4.Text = "cmbAttrItem4";
			this.cmbAttrItem4.TextChanged += new System.EventHandler(this.ChipInfo_TextChanged);
			this.cmbAttrItem4.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.cmbAttrItems_KeyPress);
			this.cmbAttrItem4.Leave += new System.EventHandler(this.cmbAttrItems_Leave);
			// 
			// cmbAttrItem3
			// 
			this.cmbAttrItem3.Dock = System.Windows.Forms.DockStyle.Top;
			this.cmbAttrItem3.Font = new System.Drawing.Font("MS Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.cmbAttrItem3.ImeMode = System.Windows.Forms.ImeMode.Off;
			this.cmbAttrItem3.Items.AddRange(new object[] {
            "(no variables)"});
			this.cmbAttrItem3.Location = new System.Drawing.Point(0, 77);
			this.cmbAttrItem3.Name = "cmbAttrItem3";
			this.cmbAttrItem3.Size = new System.Drawing.Size(162, 20);
			this.cmbAttrItem3.TabIndex = 14;
			this.cmbAttrItem3.Text = "cmbAttrItem3";
			this.cmbAttrItem3.TextChanged += new System.EventHandler(this.ChipInfo_TextChanged);
			this.cmbAttrItem3.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.cmbAttrItems_KeyPress);
			this.cmbAttrItem3.Leave += new System.EventHandler(this.cmbAttrItems_Leave);
			// 
			// cmbAttrItem2
			// 
			this.cmbAttrItem2.Dock = System.Windows.Forms.DockStyle.Top;
			this.cmbAttrItem2.Font = new System.Drawing.Font("MS Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.cmbAttrItem2.ImeMode = System.Windows.Forms.ImeMode.Off;
			this.cmbAttrItem2.Items.AddRange(new object[] {
            "(no variables)"});
			this.cmbAttrItem2.Location = new System.Drawing.Point(0, 57);
			this.cmbAttrItem2.Name = "cmbAttrItem2";
			this.cmbAttrItem2.Size = new System.Drawing.Size(162, 20);
			this.cmbAttrItem2.TabIndex = 13;
			this.cmbAttrItem2.Text = "cmbAttrItem2";
			this.cmbAttrItem2.TextChanged += new System.EventHandler(this.ChipInfo_TextChanged);
			this.cmbAttrItem2.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.cmbAttrItems_KeyPress);
			this.cmbAttrItem2.Leave += new System.EventHandler(this.cmbAttrItems_Leave);
			// 
			// cmbAttrItem1
			// 
			this.cmbAttrItem1.Dock = System.Windows.Forms.DockStyle.Top;
			this.cmbAttrItem1.Font = new System.Drawing.Font("MS Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.cmbAttrItem1.ImeMode = System.Windows.Forms.ImeMode.Off;
			this.cmbAttrItem1.Items.AddRange(new object[] {
            "(no variables)"});
			this.cmbAttrItem1.Location = new System.Drawing.Point(0, 37);
			this.cmbAttrItem1.Name = "cmbAttrItem1";
			this.cmbAttrItem1.Size = new System.Drawing.Size(162, 20);
			this.cmbAttrItem1.TabIndex = 12;
			this.cmbAttrItem1.Text = "cmbAttrItem1";
			this.cmbAttrItem1.TextChanged += new System.EventHandler(this.ChipInfo_TextChanged);
			this.cmbAttrItem1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.cmbAttrItems_KeyPress);
			this.cmbAttrItem1.Leave += new System.EventHandler(this.cmbAttrItems_Leave);
			// 
			// cmbAttrItem0
			// 
			this.cmbAttrItem0.Dock = System.Windows.Forms.DockStyle.Top;
			this.cmbAttrItem0.Font = new System.Drawing.Font("MS Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.cmbAttrItem0.ImeMode = System.Windows.Forms.ImeMode.Off;
			this.cmbAttrItem0.Items.AddRange(new object[] {
            "(no variables)"});
			this.cmbAttrItem0.Location = new System.Drawing.Point(0, 17);
			this.cmbAttrItem0.Name = "cmbAttrItem0";
			this.cmbAttrItem0.Size = new System.Drawing.Size(162, 20);
			this.cmbAttrItem0.TabIndex = 21;
			this.cmbAttrItem0.Text = "comboBox1";
			this.cmbAttrItem0.TextChanged += new System.EventHandler(this.ChipInfo_TextChanged);
			this.cmbAttrItem0.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.cmbAttrItems_KeyPress);
			this.cmbAttrItem0.Leave += new System.EventHandler(this.cmbAttrItems_Leave);
			// 
			// labelAttrValue
			// 
			this.labelAttrValue.BackColor = System.Drawing.SystemColors.ActiveCaption;
			this.labelAttrValue.Dock = System.Windows.Forms.DockStyle.Top;
			this.labelAttrValue.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
			this.labelAttrValue.Location = new System.Drawing.Point(0, 0);
			this.labelAttrValue.Name = "labelAttrValue";
			this.labelAttrValue.Size = new System.Drawing.Size(162, 17);
			this.labelAttrValue.TabIndex = 1;
			this.labelAttrValue.Text = "value";
			this.labelAttrValue.Click += new System.EventHandler(this.labelAttrValue_Click);
			// 
			// splAttr
			// 
			this.splAttr.Location = new System.Drawing.Point(100, 0);
			this.splAttr.Name = "splAttr";
			this.splAttr.Size = new System.Drawing.Size(4, 238);
			this.splAttr.TabIndex = 1;
			this.splAttr.TabStop = false;
			// 
			// panelAttrName
			// 
			this.panelAttrName.Controls.Add(this.labelAttrItem9);
			this.panelAttrName.Controls.Add(this.labelAttrItem8);
			this.panelAttrName.Controls.Add(this.labelAttrItem7);
			this.panelAttrName.Controls.Add(this.labelAttrItem6);
			this.panelAttrName.Controls.Add(this.labelAttrItem5);
			this.panelAttrName.Controls.Add(this.labelAttrItem4);
			this.panelAttrName.Controls.Add(this.labelAttrItem3);
			this.panelAttrName.Controls.Add(this.labelAttrItem2);
			this.panelAttrName.Controls.Add(this.labelAttrItem1);
			this.panelAttrName.Controls.Add(this.labelAttrItem0);
			this.panelAttrName.Controls.Add(this.labelAttrName);
			this.panelAttrName.Dock = System.Windows.Forms.DockStyle.Left;
			this.panelAttrName.Location = new System.Drawing.Point(0, 0);
			this.panelAttrName.Name = "panelAttrName";
			this.panelAttrName.Size = new System.Drawing.Size(100, 238);
			this.panelAttrName.TabIndex = 0;
			// 
			// labelAttrItem9
			// 
			this.labelAttrItem9.BackColor = System.Drawing.SystemColors.Window;
			this.labelAttrItem9.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.labelAttrItem9.Dock = System.Windows.Forms.DockStyle.Top;
			this.labelAttrItem9.Font = new System.Drawing.Font("MS Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.labelAttrItem9.Location = new System.Drawing.Point(0, 197);
			this.labelAttrItem9.Name = "labelAttrItem9";
			this.labelAttrItem9.Size = new System.Drawing.Size(100, 20);
			this.labelAttrItem9.TabIndex = 9;
			this.labelAttrItem9.Text = "label2";
			this.labelAttrItem9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelAttrItem8
			// 
			this.labelAttrItem8.BackColor = System.Drawing.SystemColors.Window;
			this.labelAttrItem8.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.labelAttrItem8.Dock = System.Windows.Forms.DockStyle.Top;
			this.labelAttrItem8.Font = new System.Drawing.Font("MS Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.labelAttrItem8.Location = new System.Drawing.Point(0, 177);
			this.labelAttrItem8.Name = "labelAttrItem8";
			this.labelAttrItem8.Size = new System.Drawing.Size(100, 20);
			this.labelAttrItem8.TabIndex = 8;
			this.labelAttrItem8.Text = "labelAttrItem8";
			this.labelAttrItem8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelAttrItem7
			// 
			this.labelAttrItem7.BackColor = System.Drawing.SystemColors.Window;
			this.labelAttrItem7.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.labelAttrItem7.Dock = System.Windows.Forms.DockStyle.Top;
			this.labelAttrItem7.Font = new System.Drawing.Font("MS Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.labelAttrItem7.Location = new System.Drawing.Point(0, 157);
			this.labelAttrItem7.Name = "labelAttrItem7";
			this.labelAttrItem7.Size = new System.Drawing.Size(100, 20);
			this.labelAttrItem7.TabIndex = 7;
			this.labelAttrItem7.Text = "labelAttrItem7";
			this.labelAttrItem7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelAttrItem6
			// 
			this.labelAttrItem6.BackColor = System.Drawing.SystemColors.Window;
			this.labelAttrItem6.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.labelAttrItem6.Dock = System.Windows.Forms.DockStyle.Top;
			this.labelAttrItem6.Font = new System.Drawing.Font("MS Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.labelAttrItem6.Location = new System.Drawing.Point(0, 137);
			this.labelAttrItem6.Name = "labelAttrItem6";
			this.labelAttrItem6.Size = new System.Drawing.Size(100, 20);
			this.labelAttrItem6.TabIndex = 6;
			this.labelAttrItem6.Text = "labelAttrItem6";
			this.labelAttrItem6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelAttrItem5
			// 
			this.labelAttrItem5.BackColor = System.Drawing.SystemColors.Window;
			this.labelAttrItem5.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.labelAttrItem5.Dock = System.Windows.Forms.DockStyle.Top;
			this.labelAttrItem5.Font = new System.Drawing.Font("MS Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.labelAttrItem5.Location = new System.Drawing.Point(0, 117);
			this.labelAttrItem5.Name = "labelAttrItem5";
			this.labelAttrItem5.Size = new System.Drawing.Size(100, 20);
			this.labelAttrItem5.TabIndex = 5;
			this.labelAttrItem5.Text = "labelAttrItem5";
			this.labelAttrItem5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelAttrItem4
			// 
			this.labelAttrItem4.BackColor = System.Drawing.SystemColors.Window;
			this.labelAttrItem4.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.labelAttrItem4.Dock = System.Windows.Forms.DockStyle.Top;
			this.labelAttrItem4.Font = new System.Drawing.Font("MS Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.labelAttrItem4.Location = new System.Drawing.Point(0, 97);
			this.labelAttrItem4.Name = "labelAttrItem4";
			this.labelAttrItem4.Size = new System.Drawing.Size(100, 20);
			this.labelAttrItem4.TabIndex = 4;
			this.labelAttrItem4.Text = "labelAttrItem4";
			this.labelAttrItem4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelAttrItem3
			// 
			this.labelAttrItem3.BackColor = System.Drawing.SystemColors.Window;
			this.labelAttrItem3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.labelAttrItem3.Dock = System.Windows.Forms.DockStyle.Top;
			this.labelAttrItem3.Font = new System.Drawing.Font("MS Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.labelAttrItem3.Location = new System.Drawing.Point(0, 77);
			this.labelAttrItem3.Name = "labelAttrItem3";
			this.labelAttrItem3.Size = new System.Drawing.Size(100, 20);
			this.labelAttrItem3.TabIndex = 3;
			this.labelAttrItem3.Text = "labelAttrItem3";
			this.labelAttrItem3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelAttrItem2
			// 
			this.labelAttrItem2.BackColor = System.Drawing.SystemColors.Window;
			this.labelAttrItem2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.labelAttrItem2.Dock = System.Windows.Forms.DockStyle.Top;
			this.labelAttrItem2.Font = new System.Drawing.Font("MS Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.labelAttrItem2.Location = new System.Drawing.Point(0, 57);
			this.labelAttrItem2.Name = "labelAttrItem2";
			this.labelAttrItem2.Size = new System.Drawing.Size(100, 20);
			this.labelAttrItem2.TabIndex = 2;
			this.labelAttrItem2.Text = "labelAttrItem2";
			this.labelAttrItem2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelAttrItem1
			// 
			this.labelAttrItem1.BackColor = System.Drawing.SystemColors.Window;
			this.labelAttrItem1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.labelAttrItem1.Dock = System.Windows.Forms.DockStyle.Top;
			this.labelAttrItem1.Font = new System.Drawing.Font("MS Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.labelAttrItem1.Location = new System.Drawing.Point(0, 37);
			this.labelAttrItem1.Name = "labelAttrItem1";
			this.labelAttrItem1.Size = new System.Drawing.Size(100, 20);
			this.labelAttrItem1.TabIndex = 1;
			this.labelAttrItem1.Text = "labelAttrItem1";
			this.labelAttrItem1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelAttrItem0
			// 
			this.labelAttrItem0.BackColor = System.Drawing.SystemColors.Window;
			this.labelAttrItem0.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.labelAttrItem0.Dock = System.Windows.Forms.DockStyle.Top;
			this.labelAttrItem0.Font = new System.Drawing.Font("MS Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.labelAttrItem0.Location = new System.Drawing.Point(0, 17);
			this.labelAttrItem0.Name = "labelAttrItem0";
			this.labelAttrItem0.Size = new System.Drawing.Size(100, 20);
			this.labelAttrItem0.TabIndex = 10;
			this.labelAttrItem0.Text = "label2";
			this.labelAttrItem0.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelAttrName
			// 
			this.labelAttrName.BackColor = System.Drawing.SystemColors.ActiveCaption;
			this.labelAttrName.Dock = System.Windows.Forms.DockStyle.Top;
			this.labelAttrName.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
			this.labelAttrName.Location = new System.Drawing.Point(0, 0);
			this.labelAttrName.Name = "labelAttrName";
			this.labelAttrName.Size = new System.Drawing.Size(100, 17);
			this.labelAttrName.TabIndex = 0;
			this.labelAttrName.Text = "attribute name";
			// 
			// txtName
			// 
			this.txtName.Font = new System.Drawing.Font("MS Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.txtName.Location = new System.Drawing.Point(32, 203);
			this.txtName.Name = "txtName";
			this.txtName.Size = new System.Drawing.Size(215, 19);
			this.txtName.TabIndex = 10;
			this.txtName.TextChanged += new System.EventHandler(this.ChipInfo_TextChanged);
			this.txtName.Enter += new System.EventHandler(this.txtAttrs_Enter);
			this.txtName.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtName_KeyPress);
			this.txtName.Leave += new System.EventHandler(this.txtAttrs_Leave);
			// 
			// labelName
			// 
			this.labelName.Location = new System.Drawing.Point(112, 185);
			this.labelName.Name = "labelName";
			this.labelName.Size = new System.Drawing.Size(48, 17);
			this.labelName.TabIndex = 8;
			this.labelName.Text = "Name";
			this.labelName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// lstSouth
			// 
			this.lstSouth.ContextMenu = this.ctmChildList;
			this.lstSouth.Font = new System.Drawing.Font("MS Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.lstSouth.IntegralHeight = false;
			this.lstSouth.ItemHeight = 12;
			this.lstSouth.Location = new System.Drawing.Point(77, 121);
			this.lstSouth.Name = "lstSouth";
			this.lstSouth.Size = new System.Drawing.Size(104, 61);
			this.lstSouth.TabIndex = 4;
			this.lstSouth.SelectedIndexChanged += new System.EventHandler(this.lstChild_SelectedIndexChanged);
			this.lstSouth.DoubleClick += new System.EventHandler(this.lstChild_DoubleClick);
			this.lstSouth.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.lstChild_KeyPress);
			this.lstSouth.MouseLeave += new System.EventHandler(this.lstEast_MouseLeave);
			this.lstSouth.MouseHover += new System.EventHandler(this.lstSouth_MouseHover);
			// 
			// ctmChildList
			// 
			this.ctmChildList.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.miListAdd,
            this.miListSelect,
            this.menuItem10,
            this.miListCut,
            this.miListCopy,
            this.miListDelete});
			this.ctmChildList.Popup += new System.EventHandler(this.ctmChildList_Popup);
			// 
			// miListAdd
			// 
			this.miListAdd.Index = 0;
			this.miListAdd.Text = "Add Here (&A)";
			this.miListAdd.Click += new System.EventHandler(this.miListAdd_Click);
			// 
			// miListSelect
			// 
			this.miListSelect.Index = 1;
			this.miListSelect.Text = "Selection (&S)";
			this.miListSelect.Click += new System.EventHandler(this.miListSelect_Click);
			// 
			// menuItem10
			// 
			this.menuItem10.Index = 2;
			this.menuItem10.Text = "-";
			// 
			// miListCut
			// 
			this.miListCut.Index = 3;
			this.miListCut.Text = "Cut (&T)";
			this.miListCut.Click += new System.EventHandler(this.miListCut_Click);
			// 
			// miListCopy
			// 
			this.miListCopy.Index = 4;
			this.miListCopy.Text = "Copy (&C)";
			this.miListCopy.Click += new System.EventHandler(this.miListCopy_Click);
			// 
			// miListDelete
			// 
			this.miListDelete.Index = 5;
			this.miListDelete.Text = "Delete (&D)";
			this.miListDelete.Click += new System.EventHandler(this.miListDelete_Click);
			// 
			// lstNorth
			// 
			this.lstNorth.ContextMenu = this.ctmChildList;
			this.lstNorth.Font = new System.Drawing.Font("MS Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.lstNorth.IntegralHeight = false;
			this.lstNorth.ItemHeight = 12;
			this.lstNorth.Location = new System.Drawing.Point(77, 0);
			this.lstNorth.Name = "lstNorth";
			this.lstNorth.Size = new System.Drawing.Size(104, 61);
			this.lstNorth.TabIndex = 2;
			this.lstNorth.SelectedIndexChanged += new System.EventHandler(this.lstChild_SelectedIndexChanged);
			this.lstNorth.DoubleClick += new System.EventHandler(this.lstChild_DoubleClick);
			this.lstNorth.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.lstChild_KeyPress);
			this.lstNorth.MouseLeave += new System.EventHandler(this.lstEast_MouseLeave);
			this.lstNorth.MouseHover += new System.EventHandler(this.lstSouth_MouseHover);
			// 
			// lstEast
			// 
			this.lstEast.ContextMenu = this.ctmChildList;
			this.lstEast.Font = new System.Drawing.Font("MS Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.lstEast.IntegralHeight = false;
			this.lstEast.ItemHeight = 12;
			this.lstEast.Location = new System.Drawing.Point(160, 61);
			this.lstEast.Name = "lstEast";
			this.lstEast.Size = new System.Drawing.Size(104, 60);
			this.lstEast.TabIndex = 3;
			this.lstEast.SelectedIndexChanged += new System.EventHandler(this.lstChild_SelectedIndexChanged);
			this.lstEast.DoubleClick += new System.EventHandler(this.lstChild_DoubleClick);
			this.lstEast.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.lstChild_KeyPress);
			this.lstEast.MouseLeave += new System.EventHandler(this.lstEast_MouseLeave);
			this.lstEast.MouseHover += new System.EventHandler(this.lstSouth_MouseHover);
			// 
			// lstWest
			// 
			this.lstWest.ContextMenu = this.ctmChildList;
			this.lstWest.Font = new System.Drawing.Font("MS Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.lstWest.IntegralHeight = false;
			this.lstWest.ItemHeight = 12;
			this.lstWest.Location = new System.Drawing.Point(0, 61);
			this.lstWest.Name = "lstWest";
			this.lstWest.Size = new System.Drawing.Size(104, 60);
			this.lstWest.TabIndex = 5;
			this.lstWest.SelectedIndexChanged += new System.EventHandler(this.lstChild_SelectedIndexChanged);
			this.lstWest.DoubleClick += new System.EventHandler(this.lstChild_DoubleClick);
			this.lstWest.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.lstChild_KeyPress);
			this.lstWest.MouseLeave += new System.EventHandler(this.lstEast_MouseLeave);
			this.lstWest.MouseHover += new System.EventHandler(this.lstSouth_MouseHover);
			// 
			// buttonSelChip
			// 
			this.buttonSelChip.ImageIndex = 0;
			this.buttonSelChip.ImageList = this.imgIconsL;
			this.buttonSelChip.Location = new System.Drawing.Point(104, 61);
			this.buttonSelChip.Name = "buttonSelChip";
			this.buttonSelChip.Size = new System.Drawing.Size(56, 60);
			this.buttonSelChip.TabIndex = 6;
			this.ttMain.SetToolTip(this.buttonSelChip, "No comment");
			this.buttonSelChip.Click += new System.EventHandler(this.buttonSelChip_Click);
			// 
			// imgIconsL
			// 
			this.imgIconsL.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgIconsL.ImageStream")));
			this.imgIconsL.TransparentColor = System.Drawing.Color.White;
			this.imgIconsL.Images.SetKeyName(0, "");
			this.imgIconsL.Images.SetKeyName(1, "");
			this.imgIconsL.Images.SetKeyName(2, "");
			this.imgIconsL.Images.SetKeyName(3, "");
			this.imgIconsL.Images.SetKeyName(4, "");
			this.imgIconsL.Images.SetKeyName(5, "");
			this.imgIconsL.Images.SetKeyName(6, "");
			this.imgIconsL.Images.SetKeyName(7, "");
			this.imgIconsL.Images.SetKeyName(8, "");
			this.imgIconsL.Images.SetKeyName(9, "");
			this.imgIconsL.Images.SetKeyName(10, "");
			this.imgIconsL.Images.SetKeyName(11, "");
			this.imgIconsL.Images.SetKeyName(12, "");
			// 
			// tabGI
			// 
			this.tabGI.Alignment = System.Windows.Forms.TabAlignment.Bottom;
			this.tabGI.Controls.Add(this.tpAngle);
			this.tabGI.Controls.Add(this.tpPalette);
			this.tabGI.Location = new System.Drawing.Point(11, 228);
			this.tabGI.Multiline = true;
			this.tabGI.Name = "tabGI";
			this.tabGI.SelectedIndex = 0;
			this.tabGI.Size = new System.Drawing.Size(250, 265);
			this.tabGI.TabIndex = 9;
			// 
			// tpAngle
			// 
			this.tpAngle.Controls.Add(this.pictAngle);
			this.tpAngle.Location = new System.Drawing.Point(4, 4);
			this.tpAngle.Name = "tpAngle";
			this.tpAngle.Size = new System.Drawing.Size(242, 239);
			this.tpAngle.TabIndex = 0;
			this.tpAngle.Text = "Angle";
			this.tpAngle.UseVisualStyleBackColor = true;
			// 
			// pictAngle
			// 
			this.pictAngle.BackColor = System.Drawing.Color.Navy;
			this.pictAngle.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.pictAngle.ContextMenu = this.ctmAngles;
			this.pictAngle.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pictAngle.Location = new System.Drawing.Point(0, 0);
			this.pictAngle.Name = "pictAngle";
			this.pictAngle.Size = new System.Drawing.Size(242, 239);
			this.pictAngle.TabIndex = 16;
			this.pictAngle.TabStop = false;
			this.pictAngle.Click += new System.EventHandler(this.pictAngle_Click);
			this.pictAngle.Paint += new System.Windows.Forms.PaintEventHandler(this.pictAngle_Paint);
			this.pictAngle.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictAngle_MouseDown);
			this.pictAngle.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictAngle_MouseMove);
			this.pictAngle.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictAngle_MouseUp);
			// 
			// ctmAngles
			// 
			this.ctmAngles.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.miAngleGrid0,
            this.miAngleGrid1,
            this.miAngleGrid5,
            this.miAngleGrid15,
            this.miAngleGrid30,
            this.miAngleGrid90});
			this.ctmAngles.Popup += new System.EventHandler(this.ctmAngles_Popup);
			// 
			// miAngleGrid0
			// 
			this.miAngleGrid0.Index = 0;
			this.miAngleGrid0.Text = "No Grid (&0)";
			this.miAngleGrid0.Click += new System.EventHandler(this.miAngleGrid_Click);
			// 
			// miAngleGrid1
			// 
			this.miAngleGrid1.Index = 1;
			this.miAngleGrid1.Text = "1° Grid Snapping (&1)";
			this.miAngleGrid1.Click += new System.EventHandler(this.miAngleGrid_Click);
			// 
			// miAngleGrid5
			// 
			this.miAngleGrid5.Index = 2;
			this.miAngleGrid5.Text = "5° Grid Snapping (&2)";
			this.miAngleGrid5.Click += new System.EventHandler(this.miAngleGrid_Click);
			// 
			// miAngleGrid15
			// 
			this.miAngleGrid15.Index = 3;
			this.miAngleGrid15.Text = "15° Grid Snapping (&3)";
			this.miAngleGrid15.Click += new System.EventHandler(this.miAngleGrid_Click);
			// 
			// miAngleGrid30
			// 
			this.miAngleGrid30.Index = 4;
			this.miAngleGrid30.Text = "30° Grid Snapping (&4)";
			this.miAngleGrid30.Click += new System.EventHandler(this.miAngleGrid_Click);
			// 
			// miAngleGrid90
			// 
			this.miAngleGrid90.Index = 5;
			this.miAngleGrid90.Text = "90° Grid Snapping (&5)";
			this.miAngleGrid90.Click += new System.EventHandler(this.miAngleGrid_Click);
			// 
			// tpPalette
			// 
			this.tpPalette.Controls.Add(this.pictPalette);
			this.tpPalette.Controls.Add(this.labelPaletteError);
			this.tpPalette.Location = new System.Drawing.Point(4, 4);
			this.tpPalette.Name = "tpPalette";
			this.tpPalette.Size = new System.Drawing.Size(242, 239);
			this.tpPalette.TabIndex = 1;
			this.tpPalette.Text = "Color";
			this.tpPalette.UseVisualStyleBackColor = true;
			// 
			// pictPalette
			// 
			this.pictPalette.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.pictPalette.ContextMenu = this.ctmPalette;
			this.pictPalette.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pictPalette.InitialImage = null;
			this.pictPalette.Location = new System.Drawing.Point(0, 0);
			this.pictPalette.Name = "pictPalette";
			this.pictPalette.Size = new System.Drawing.Size(242, 239);
			this.pictPalette.TabIndex = 0;
			this.pictPalette.TabStop = false;
			this.pictPalette.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictPalette_MouseDown);
			this.pictPalette.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictPalette_MouseMove);
			this.pictPalette.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictPalette_MouseUp);
			// 
			// ctmPalette
			// 
			this.ctmPalette.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.miPaletteShowDlg,
            this.menuItem2,
            this.miPaletteChildPaint,
            this.miPaletteAllPaint});
			this.ctmPalette.Popup += new System.EventHandler(this.ctmPalette_Popup);
			// 
			// miPaletteShowDlg
			// 
			this.miPaletteShowDlg.Index = 0;
			this.miPaletteShowDlg.Text = "Color Dialog...";
			this.miPaletteShowDlg.Click += new System.EventHandler(this.miPaletteShowDlg_Click);
			// 
			// menuItem2
			// 
			this.menuItem2.Index = 1;
			this.menuItem2.Text = "-";
			// 
			// miPaletteChildPaint
			// 
			this.miPaletteChildPaint.Index = 2;
			this.miPaletteChildPaint.Text = "Make the derived chip this color";
			this.miPaletteChildPaint.Click += new System.EventHandler(this.miPaletteChildPaint_Click);
			// 
			// miPaletteAllPaint
			// 
			this.miPaletteAllPaint.Index = 3;
			this.miPaletteAllPaint.Text = "Make all chips this color";
			this.miPaletteAllPaint.Click += new System.EventHandler(this.miPaletteAllPaint_Click);
			// 
			// labelPaletteError
			// 
			this.labelPaletteError.Dock = System.Windows.Forms.DockStyle.Fill;
			this.labelPaletteError.Location = new System.Drawing.Point(0, 0);
			this.labelPaletteError.Name = "labelPaletteError";
			this.labelPaletteError.Size = new System.Drawing.Size(242, 239);
			this.labelPaletteError.TabIndex = 1;
			this.labelPaletteError.Text = "Palette.bmp Can\'t be found, Won\'t use.";
			// 
			// tmr
			// 
			this.tmr.Enabled = true;
			this.tmr.Interval = 250;
			this.tmr.Tick += new System.EventHandler(this.tmr_Tick);
			// 
			// tmrScroll
			// 
			this.tmrScroll.Interval = 1;
			this.tmrScroll.Tick += new System.EventHandler(this.tmrScroll_Tick);
			// 
			// ctmChipType
			// 
			this.ctmChipType.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.miCut,
            this.miCopy,
            this.miDelete,
            this.menuItem8,
            this.miChange,
            this.miChipComment,
            this.miColor,
            this.menuItem7,
            this.miRotateRight,
            this.miRotateLeft,
            this.menuItem5,
            this.miReverseX,
            this.miReverseY,
            this.miReverseZ});
			this.ctmChipType.Popup += new System.EventHandler(this.ctmChipType_Popup);
			// 
			// miCut
			// 
			this.miCut.Index = 0;
			this.miCut.Text = "Cut (&T)";
			this.miCut.Click += new System.EventHandler(this.miCut_Click);
			// 
			// miCopy
			// 
			this.miCopy.Index = 1;
			this.miCopy.Text = "Copy (&C)";
			this.miCopy.Click += new System.EventHandler(this.miCopy_Click);
			// 
			// miDelete
			// 
			this.miDelete.Index = 2;
			this.miDelete.Text = "Delete (&D)";
			this.miDelete.Click += new System.EventHandler(this.miDelete_Click);
			// 
			// menuItem8
			// 
			this.menuItem8.Index = 3;
			this.menuItem8.Text = "-";
			// 
			// miChange
			// 
			this.miChange.Index = 4;
			this.miChange.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.miChangeChip,
            this.miChangeFrame,
            this.miChangeRudder,
            this.miChangeRudderF,
            this.miChangeTrim,
            this.miChangeTrimF,
            this.ctmSeparator,
            this.miChangeWheel,
            this.miChangeRLW,
            this.miChangeJet,
            this.miChangeWeight,
            this.miChangeCowl,
            this.miChangeArm});
			this.miChange.Text = "Change Chip to... (&H)";
			// 
			// miChangeChip
			// 
			this.miChangeChip.Index = 0;
			this.miChangeChip.Text = "Chip";
			this.miChangeChip.Click += new System.EventHandler(this.miChangeType);
			// 
			// miChangeFrame
			// 
			this.miChangeFrame.Index = 1;
			this.miChangeFrame.Text = "Frame";
			this.miChangeFrame.Click += new System.EventHandler(this.miChangeType);
			// 
			// miChangeRudder
			// 
			this.miChangeRudder.Index = 2;
			this.miChangeRudder.Text = "Rudder";
			this.miChangeRudder.Click += new System.EventHandler(this.miChangeType);
			// 
			// miChangeRudderF
			// 
			this.miChangeRudderF.Index = 3;
			this.miChangeRudderF.Text = "Rudder Frame";
			this.miChangeRudderF.Click += new System.EventHandler(this.miChangeType);
			// 
			// miChangeTrim
			// 
			this.miChangeTrim.Index = 4;
			this.miChangeTrim.Text = "Trim";
			this.miChangeTrim.Click += new System.EventHandler(this.miChangeType);
			// 
			// miChangeTrimF
			// 
			this.miChangeTrimF.Index = 5;
			this.miChangeTrimF.Text = "Trim Frame";
			this.miChangeTrimF.Click += new System.EventHandler(this.miChangeType);
			// 
			// ctmSeparator
			// 
			this.ctmSeparator.Index = 6;
			this.ctmSeparator.Text = "-";
			// 
			// miChangeWheel
			// 
			this.miChangeWheel.Index = 7;
			this.miChangeWheel.Text = "Wheel";
			this.miChangeWheel.Click += new System.EventHandler(this.miChangeType);
			// 
			// miChangeRLW
			// 
			this.miChangeRLW.Index = 8;
			this.miChangeRLW.Text = "RLW";
			this.miChangeRLW.Click += new System.EventHandler(this.miChangeType);
			// 
			// miChangeJet
			// 
			this.miChangeJet.Index = 9;
			this.miChangeJet.Text = "Jet";
			this.miChangeJet.Click += new System.EventHandler(this.miChangeType);
			// 
			// miChangeWeight
			// 
			this.miChangeWeight.Index = 10;
			this.miChangeWeight.Text = "Weight";
			this.miChangeWeight.Click += new System.EventHandler(this.miChangeType);
			// 
			// miChangeCowl
			// 
			this.miChangeCowl.Index = 11;
			this.miChangeCowl.Text = "Cowl";
			this.miChangeCowl.Click += new System.EventHandler(this.miChangeType);
			// 
			// miChangeArm
			// 
			this.miChangeArm.Index = 12;
			this.miChangeArm.Text = "Arm";
			this.miChangeArm.Click += new System.EventHandler(this.miChangeType);
			// 
			// miChipComment
			// 
			this.miChipComment.Index = 5;
			this.miChipComment.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.miCommentEdit,
            this.miCommentDelete});
			this.miChipComment.Text = "Comment(&O)";
			this.miChipComment.Popup += new System.EventHandler(this.miChipComment_Popup);
			this.miChipComment.Select += new System.EventHandler(this.miChipComment_Popup);
			// 
			// miCommentEdit
			// 
			this.miCommentEdit.Index = 0;
			this.miCommentEdit.Text = "Edit Comment";
			this.miCommentEdit.Click += new System.EventHandler(this.miCommentEdit_Click);
			// 
			// miCommentDelete
			// 
			this.miCommentDelete.Index = 1;
			this.miCommentDelete.Text = "Remove Comment";
			this.miCommentDelete.Click += new System.EventHandler(this.miCommentDelete_Click);
			// 
			// miColor
			// 
			this.miColor.Index = 6;
			this.miColor.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.miColorToAll,
            this.miColorToChild});
			this.miColor.Text = "Color (&I)";
			// 
			// miColorToAll
			// 
			this.miColorToAll.Index = 0;
			this.miColorToAll.Text = "Copy to all chips (&A)";
			this.miColorToAll.Click += new System.EventHandler(this.miPaletteAllPaint_Click);
			// 
			// miColorToChild
			// 
			this.miColorToChild.Index = 1;
			this.miColorToChild.Text = "Copy to children (&C)";
			this.miColorToChild.Click += new System.EventHandler(this.miPaletteChildPaint_Click);
			// 
			// menuItem7
			// 
			this.menuItem7.Index = 7;
			this.menuItem7.Text = "-";
			// 
			// miRotateRight
			// 
			this.miRotateRight.Index = 8;
			this.miRotateRight.Text = "Rotate right around this chip (&R)";
			this.miRotateRight.Click += new System.EventHandler(this.miRotateRight_Click);
			// 
			// miRotateLeft
			// 
			this.miRotateLeft.Index = 9;
			this.miRotateLeft.Text = "Rotate left around this chip (&L)";
			this.miRotateLeft.Click += new System.EventHandler(this.miRotateLeft_Click);
			// 
			// menuItem5
			// 
			this.menuItem5.Index = 10;
			this.menuItem5.Text = "-";
			// 
			// miReverseX
			// 
			this.miReverseX.Index = 11;
			this.miReverseX.Text = "Flip horizontal (&X)";
			this.miReverseX.Click += new System.EventHandler(this.miReverseX_Click);
			// 
			// miReverseY
			// 
			this.miReverseY.Index = 12;
			this.miReverseY.Text = "Upside Down (&Y)";
			this.miReverseY.Click += new System.EventHandler(this.miReverseY_Click);
			// 
			// miReverseZ
			// 
			this.miReverseZ.Index = 13;
			this.miReverseZ.Text = "Flip forward/backward (&Z)";
			this.miReverseZ.Click += new System.EventHandler(this.miReverseZ_Click);
			// 
			// toolStripContainer1
			// 
			// 
			// toolStripContainer1.ContentPanel
			// 
			this.toolStripContainer1.ContentPanel.Controls.Add(this.pictTarget);
			this.toolStripContainer1.ContentPanel.Controls.Add(this.labelTip);
			this.toolStripContainer1.ContentPanel.Controls.Add(this.panelCtrl);
			this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(1065, 701);
			this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
			this.toolStripContainer1.Name = "toolStripContainer1";
			this.toolStripContainer1.Size = new System.Drawing.Size(1065, 732);
			this.toolStripContainer1.TabIndex = 13;
			this.toolStripContainer1.Text = "toolStripContainer1";
			// 
			// toolStripContainer1.TopToolStripPanel
			// 
			this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.menuStrip1);
			this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.toolStrip1);
			this.toolStripContainer1.TopToolStripPanel.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
			// 
			// menuStrip1
			// 
			this.menuStrip1.Dock = System.Windows.Forms.DockStyle.None;
			this.menuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ファイルFToolStripMenuItem,
            this.編集EToolStripMenuItem,
            this.ツールTToolStripMenuItem,
            this.設定CToolStripMenuItem,
            this.ヘルプHToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
			this.menuStrip1.Size = new System.Drawing.Size(315, 24);
			this.menuStrip1.TabIndex = 14;
			this.menuStrip1.Text = "menuStrip1";
			this.menuStrip1.Visible = false;
			// 
			// ファイルFToolStripMenuItem
			// 
			this.ファイルFToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.新規作成ToolStripMenuItem,
            this.開くToolStripMenuItem,
            this.上書き保存ToolStripMenuItem,
            this.名前をつけて保存ToolStripMenuItem,
            this.toolStripSeparator1,
            this.rCDTXTを開くToolStripMenuItem,
            this.rCDで保存ToolStripMenuItem,
            this.toolStripSeparator2,
            this.終了ToolStripMenuItem});
			this.ファイルFToolStripMenuItem.Name = "ファイルFToolStripMenuItem";
			this.ファイルFToolStripMenuItem.Size = new System.Drawing.Size(54, 20);
			this.ファイルFToolStripMenuItem.Text = "File (&F)";
			// 
			// 新規作成ToolStripMenuItem
			// 
			this.新規作成ToolStripMenuItem.Name = "新規作成ToolStripMenuItem";
			this.新規作成ToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
			this.新規作成ToolStripMenuItem.Text = "Create New (&N)";
			this.新規作成ToolStripMenuItem.Click += new System.EventHandler(this.miFileNew_Click);
			// 
			// 開くToolStripMenuItem
			// 
			this.開くToolStripMenuItem.Name = "開くToolStripMenuItem";
			this.開くToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
			this.開くToolStripMenuItem.Text = "Open く(&O)";
			this.開くToolStripMenuItem.Click += new System.EventHandler(this.miFileOpen_Click);
			// 
			// 上書き保存ToolStripMenuItem
			// 
			this.上書き保存ToolStripMenuItem.Name = "上書き保存ToolStripMenuItem";
			this.上書き保存ToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
			this.上書き保存ToolStripMenuItem.Text = "overwrite save (&S)";
			this.上書き保存ToolStripMenuItem.Click += new System.EventHandler(this.miFileSave_Click);
			// 
			// 名前をつけて保存ToolStripMenuItem
			// 
			this.名前をつけて保存ToolStripMenuItem.Name = "名前をつけて保存ToolStripMenuItem";
			this.名前をつけて保存ToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
			this.名前をつけて保存ToolStripMenuItem.Text = "Save As (&A)";
			this.名前をつけて保存ToolStripMenuItem.Click += new System.EventHandler(this.miFileSaveAs_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(164, 6);
			// 
			// rCDTXTを開くToolStripMenuItem
			// 
			this.rCDTXTを開くToolStripMenuItem.Name = "rCDTXTを開くToolStripMenuItem";
			this.rCDTXTを開くToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
			this.rCDTXTを開くToolStripMenuItem.Text = "Open RCD/TXT (&I)";
			this.rCDTXTを開くToolStripMenuItem.Click += new System.EventHandler(this.miFileImport_Click);
			// 
			// rCDで保存ToolStripMenuItem
			// 
			this.rCDで保存ToolStripMenuItem.Name = "rCDで保存ToolStripMenuItem";
			this.rCDで保存ToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
			this.rCDで保存ToolStripMenuItem.Text = "Save in RCD(&E)";
			this.rCDで保存ToolStripMenuItem.Click += new System.EventHandler(this.miFileExport_Click);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(164, 6);
			// 
			// 終了ToolStripMenuItem
			// 
			this.終了ToolStripMenuItem.Name = "終了ToolStripMenuItem";
			this.終了ToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
			this.終了ToolStripMenuItem.Text = "Exit RCM (&Q)";
			this.終了ToolStripMenuItem.Click += new System.EventHandler(this.miFileQuit_Click);
			// 
			// 編集EToolStripMenuItem
			// 
			this.編集EToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.切り取りTToolStripMenuItem,
            this.コピーCToolStripMenuItem,
            this.貼り付けPToolStripMenuItem,
            this.toolStripMenuItem1,
            this.視点変更VToolStripMenuItem,
            this.選択SToolStripMenuItem,
            this.モデル情報SToolStripMenuItem});
			this.編集EToolStripMenuItem.Name = "編集EToolStripMenuItem";
			this.編集EToolStripMenuItem.Size = new System.Drawing.Size(56, 20);
			this.編集EToolStripMenuItem.Text = "Edit (&E)";
			// 
			// 切り取りTToolStripMenuItem
			// 
			this.切り取りTToolStripMenuItem.Name = "切り取りTToolStripMenuItem";
			this.切り取りTToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
			this.切り取りTToolStripMenuItem.Text = "Cut (&T)";
			// 
			// コピーCToolStripMenuItem
			// 
			this.コピーCToolStripMenuItem.Name = "コピーCToolStripMenuItem";
			this.コピーCToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
			this.コピーCToolStripMenuItem.Text = "Copy (&C)";
			// 
			// 貼り付けPToolStripMenuItem
			// 
			this.貼り付けPToolStripMenuItem.Name = "貼り付けPToolStripMenuItem";
			this.貼り付けPToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
			this.貼り付けPToolStripMenuItem.Text = "Delete (&D)";
			// 
			// toolStripMenuItem1
			// 
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			this.toolStripMenuItem1.Size = new System.Drawing.Size(191, 6);
			// 
			// 視点変更VToolStripMenuItem
			// 
			this.視点変更VToolStripMenuItem.Name = "視点変更VToolStripMenuItem";
			this.視点変更VToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
			this.視点変更VToolStripMenuItem.Text = "perspective change (&V)";
			// 
			// 選択SToolStripMenuItem
			// 
			this.選択SToolStripMenuItem.Name = "選択SToolStripMenuItem";
			this.選択SToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
			this.選択SToolStripMenuItem.Text = "Selection (&S)";
			// 
			// モデル情報SToolStripMenuItem
			// 
			this.モデル情報SToolStripMenuItem.Name = "モデル情報SToolStripMenuItem";
			this.モデル情報SToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
			this.モデル情報SToolStripMenuItem.Text = "model information (&I)";
			// 
			// ツールTToolStripMenuItem
			// 
			this.ツールTToolStripMenuItem.Name = "ツールTToolStripMenuItem";
			this.ツールTToolStripMenuItem.Size = new System.Drawing.Size(58, 20);
			this.ツールTToolStripMenuItem.Text = "Tool (&T)";
			// 
			// 設定CToolStripMenuItem
			// 
			this.設定CToolStripMenuItem.Name = "設定CToolStripMenuItem";
			this.設定CToolStripMenuItem.Size = new System.Drawing.Size(75, 20);
			this.設定CToolStripMenuItem.Text = "Setting (&C)";
			// 
			// ヘルプHToolStripMenuItem
			// 
			this.ヘルプHToolStripMenuItem.Name = "ヘルプHToolStripMenuItem";
			this.ヘルプHToolStripMenuItem.Size = new System.Drawing.Size(64, 20);
			this.ヘルプHToolStripMenuItem.Text = "Help (&H)";
			// 
			// toolStrip1
			// 
			this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
			this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.toolStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbSelectMode,
            this.tsbCut,
            this.tsbCopy,
            this.tsbPasteMode,
            this.toolStripSeparator3,
            this.tsbInsert,
            this.tsbRemove,
            this.toolStripSeparator4,
            this.tsbZoom,
            this.tsbMooz,
            this.tsbCameraMode,
            this.tsbAutoCamera,
            this.toolStripSeparator5,
            this.tsbChipMode,
            this.tsbFrameMode,
            this.tsbRudderMode,
            this.tsbRudderFMode,
            this.tsbTrimMode,
            this.tsbTrimFMode,
            this.toolStripSeparator6,
            this.tsbWheelMode,
            this.tsbRLWMode,
            this.tsbJetMode,
            this.tsbWeightMode,
            this.tsbCowlMode,
            this.tsbArmMode});
			this.toolStrip1.Location = new System.Drawing.Point(3, 0);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
			this.toolStrip1.Size = new System.Drawing.Size(643, 31);
			this.toolStrip1.TabIndex = 15;
			// 
			// tsbSelectMode
			// 
			this.tsbSelectMode.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbSelectMode.Image = ((System.Drawing.Image)(resources.GetObject("tsbSelectMode.Image")));
			this.tsbSelectMode.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbSelectMode.Name = "tsbSelectMode";
			this.tsbSelectMode.Size = new System.Drawing.Size(28, 28);
			this.tsbSelectMode.Text = "Selection";
			this.tsbSelectMode.Click += new System.EventHandler(this.tsbModeChange_Click);
			// 
			// tsbCut
			// 
			this.tsbCut.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbCut.Image = ((System.Drawing.Image)(resources.GetObject("tsbCut.Image")));
			this.tsbCut.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbCut.Name = "tsbCut";
			this.tsbCut.Size = new System.Drawing.Size(28, 28);
			this.tsbCut.Text = "Cut the selected chip";
			this.tsbCut.Click += new System.EventHandler(this.tsbCut_Click);
			// 
			// tsbCopy
			// 
			this.tsbCopy.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbCopy.Image = ((System.Drawing.Image)(resources.GetObject("tsbCopy.Image")));
			this.tsbCopy.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbCopy.Name = "tsbCopy";
			this.tsbCopy.Size = new System.Drawing.Size(28, 28);
			this.tsbCopy.Text = "Copy selected chip";
			this.tsbCopy.Click += new System.EventHandler(this.tsbCopy_Click);
			// 
			// tsbPasteMode
			// 
			this.tsbPasteMode.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbPasteMode.Image = ((System.Drawing.Image)(resources.GetObject("tsbPasteMode.Image")));
			this.tsbPasteMode.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbPasteMode.Name = "tsbPasteMode";
			this.tsbPasteMode.Size = new System.Drawing.Size(28, 28);
			this.tsbPasteMode.Text = "Paste";
			this.tsbPasteMode.Click += new System.EventHandler(this.tsbModeChange_Click);
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new System.Drawing.Size(6, 31);
			// 
			// tsbInsert
			// 
			this.tsbInsert.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbInsert.Image = ((System.Drawing.Image)(resources.GetObject("tsbInsert.Image")));
			this.tsbInsert.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbInsert.Name = "tsbInsert";
			this.tsbInsert.Size = new System.Drawing.Size(28, 28);
			this.tsbInsert.Text = "Insert Chip";
			this.tsbInsert.Click += new System.EventHandler(this.tsbInsert_Click);
			// 
			// tsbRemove
			// 
			this.tsbRemove.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbRemove.Image = ((System.Drawing.Image)(resources.GetObject("tsbRemove.Image")));
			this.tsbRemove.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbRemove.Name = "tsbRemove";
			this.tsbRemove.Size = new System.Drawing.Size(28, 28);
			this.tsbRemove.Text = "Delete selected chip";
			this.tsbRemove.Click += new System.EventHandler(this.tsbRemove_Click);
			// 
			// toolStripSeparator4
			// 
			this.toolStripSeparator4.Name = "toolStripSeparator4";
			this.toolStripSeparator4.Size = new System.Drawing.Size(6, 31);
			// 
			// tsbZoom
			// 
			this.tsbZoom.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbZoom.Image = ((System.Drawing.Image)(resources.GetObject("tsbZoom.Image")));
			this.tsbZoom.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbZoom.Name = "tsbZoom";
			this.tsbZoom.Size = new System.Drawing.Size(28, 28);
			this.tsbZoom.Text = "Zoom in";
			this.tsbZoom.Click += new System.EventHandler(this.tsbZoom_Click);
			// 
			// tsbMooz
			// 
			this.tsbMooz.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbMooz.Image = ((System.Drawing.Image)(resources.GetObject("tsbMooz.Image")));
			this.tsbMooz.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbMooz.Name = "tsbMooz";
			this.tsbMooz.Size = new System.Drawing.Size(28, 28);
			this.tsbMooz.Text = "Zoom out";
			this.tsbMooz.Click += new System.EventHandler(this.tsbMooz_Click);
			// 
			// tsbCameraMode
			// 
			this.tsbCameraMode.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbCameraMode.Image = ((System.Drawing.Image)(resources.GetObject("tsbCameraMode.Image")));
			this.tsbCameraMode.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbCameraMode.Name = "tsbCameraMode";
			this.tsbCameraMode.Size = new System.Drawing.Size(28, 28);
			this.tsbCameraMode.Text = "Perspective";
			this.tsbCameraMode.Click += new System.EventHandler(this.tsbModeChange_Click);
			// 
			// tsbAutoCamera
			// 
			this.tsbAutoCamera.CheckOnClick = true;
			this.tsbAutoCamera.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbAutoCamera.Image = ((System.Drawing.Image)(resources.GetObject("tsbAutoCamera.Image")));
			this.tsbAutoCamera.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbAutoCamera.Name = "tsbAutoCamera";
			this.tsbAutoCamera.Size = new System.Drawing.Size(28, 28);
			this.tsbAutoCamera.Text = "Cursor auto-tracking";
			this.tsbAutoCamera.Click += new System.EventHandler(this.tsbAutoCamera_Click);
			// 
			// toolStripSeparator5
			// 
			this.toolStripSeparator5.Name = "toolStripSeparator5";
			this.toolStripSeparator5.Size = new System.Drawing.Size(6, 31);
			// 
			// tsbChipMode
			// 
			this.tsbChipMode.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbChipMode.Image = ((System.Drawing.Image)(resources.GetObject("tsbChipMode.Image")));
			this.tsbChipMode.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbChipMode.Name = "tsbChipMode";
			this.tsbChipMode.Size = new System.Drawing.Size(28, 28);
			this.tsbChipMode.Text = "Chip";
			this.tsbChipMode.Click += new System.EventHandler(this.tsbModeChange_Click);
			// 
			// tsbFrameMode
			// 
			this.tsbFrameMode.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbFrameMode.Image = ((System.Drawing.Image)(resources.GetObject("tsbFrameMode.Image")));
			this.tsbFrameMode.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbFrameMode.Name = "tsbFrameMode";
			this.tsbFrameMode.Size = new System.Drawing.Size(28, 28);
			this.tsbFrameMode.Text = "Frame";
			this.tsbFrameMode.Click += new System.EventHandler(this.tsbModeChange_Click);
			// 
			// tsbRudderMode
			// 
			this.tsbRudderMode.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbRudderMode.Image = ((System.Drawing.Image)(resources.GetObject("tsbRudderMode.Image")));
			this.tsbRudderMode.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbRudderMode.Name = "tsbRudderMode";
			this.tsbRudderMode.Size = new System.Drawing.Size(28, 28);
			this.tsbRudderMode.Text = "Rudder";
			this.tsbRudderMode.Click += new System.EventHandler(this.tsbModeChange_Click);
			// 
			// tsbRudderFMode
			// 
			this.tsbRudderFMode.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbRudderFMode.Image = ((System.Drawing.Image)(resources.GetObject("tsbRudderFMode.Image")));
			this.tsbRudderFMode.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbRudderFMode.Name = "tsbRudderFMode";
			this.tsbRudderFMode.Size = new System.Drawing.Size(28, 28);
			this.tsbRudderFMode.Text = "Rudder frame";
			this.tsbRudderFMode.Click += new System.EventHandler(this.tsbModeChange_Click);
			// 
			// tsbTrimMode
			// 
			this.tsbTrimMode.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbTrimMode.Image = ((System.Drawing.Image)(resources.GetObject("tsbTrimMode.Image")));
			this.tsbTrimMode.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbTrimMode.Name = "tsbTrimMode";
			this.tsbTrimMode.Size = new System.Drawing.Size(28, 28);
			this.tsbTrimMode.Text = "Trim";
			this.tsbTrimMode.Click += new System.EventHandler(this.tsbModeChange_Click);
			// 
			// tsbTrimFMode
			// 
			this.tsbTrimFMode.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbTrimFMode.Image = ((System.Drawing.Image)(resources.GetObject("tsbTrimFMode.Image")));
			this.tsbTrimFMode.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbTrimFMode.Name = "tsbTrimFMode";
			this.tsbTrimFMode.Size = new System.Drawing.Size(28, 28);
			this.tsbTrimFMode.Text = "Trim frame";
			this.tsbTrimFMode.Click += new System.EventHandler(this.tsbModeChange_Click);
			// 
			// toolStripSeparator6
			// 
			this.toolStripSeparator6.Name = "toolStripSeparator6";
			this.toolStripSeparator6.Size = new System.Drawing.Size(6, 31);
			// 
			// tsbWheelMode
			// 
			this.tsbWheelMode.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbWheelMode.Image = ((System.Drawing.Image)(resources.GetObject("tsbWheelMode.Image")));
			this.tsbWheelMode.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbWheelMode.Name = "tsbWheelMode";
			this.tsbWheelMode.Size = new System.Drawing.Size(28, 28);
			this.tsbWheelMode.Text = "Wheel";
			this.tsbWheelMode.Click += new System.EventHandler(this.tsbModeChange_Click);
			// 
			// tsbRLWMode
			// 
			this.tsbRLWMode.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbRLWMode.Image = ((System.Drawing.Image)(resources.GetObject("tsbRLWMode.Image")));
			this.tsbRLWMode.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbRLWMode.Name = "tsbRLWMode";
			this.tsbRLWMode.Size = new System.Drawing.Size(28, 28);
			this.tsbRLWMode.Text = "Recoilless wheel";
			this.tsbRLWMode.Click += new System.EventHandler(this.tsbModeChange_Click);
			// 
			// tsbJetMode
			// 
			this.tsbJetMode.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbJetMode.Image = ((System.Drawing.Image)(resources.GetObject("tsbJetMode.Image")));
			this.tsbJetMode.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbJetMode.Name = "tsbJetMode";
			this.tsbJetMode.Size = new System.Drawing.Size(28, 28);
			this.tsbJetMode.Text = "Jet";
			this.tsbJetMode.Click += new System.EventHandler(this.tsbModeChange_Click);
			// 
			// tsbWeightMode
			// 
			this.tsbWeightMode.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbWeightMode.Image = ((System.Drawing.Image)(resources.GetObject("tsbWeightMode.Image")));
			this.tsbWeightMode.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbWeightMode.Name = "tsbWeightMode";
			this.tsbWeightMode.Size = new System.Drawing.Size(28, 28);
			this.tsbWeightMode.Text = "Weight";
			this.tsbWeightMode.Click += new System.EventHandler(this.tsbModeChange_Click);
			// 
			// tsbCowlMode
			// 
			this.tsbCowlMode.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbCowlMode.Image = ((System.Drawing.Image)(resources.GetObject("tsbCowlMode.Image")));
			this.tsbCowlMode.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbCowlMode.Name = "tsbCowlMode";
			this.tsbCowlMode.Size = new System.Drawing.Size(28, 28);
			this.tsbCowlMode.Text = "Cowl";
			this.tsbCowlMode.Click += new System.EventHandler(this.tsbModeChange_Click);
			// 
			// tsbArmMode
			// 
			this.tsbArmMode.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbArmMode.Image = ((System.Drawing.Image)(resources.GetObject("tsbArmMode.Image")));
			this.tsbArmMode.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbArmMode.Name = "tsbArmMode";
			this.tsbArmMode.Size = new System.Drawing.Size(28, 28);
			this.tsbArmMode.Text = "Arm";
			this.tsbArmMode.Click += new System.EventHandler(this.tsbModeChange_Click);
			// 
			// pictTarget
			// 
			this.pictTarget.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.pictTarget.Controls.Add(this.btnEditPanel);
			this.pictTarget.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pictTarget.Location = new System.Drawing.Point(0, 16);
			this.pictTarget.Name = "pictTarget";
			this.pictTarget.Size = new System.Drawing.Size(795, 685);
			this.pictTarget.TabIndex = 11;
			this.pictTarget.KeyUp += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyUp);
			this.pictTarget.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
			this.pictTarget.Click += new System.EventHandler(this.pictTarget_Click);
			this.pictTarget.Paint += new System.Windows.Forms.PaintEventHandler(this.pictTarget_Paint);
			this.pictTarget.GotFocus += new System.EventHandler(this.pictTarget_FocusChanged);
			this.pictTarget.LostFocus += new System.EventHandler(this.pictTarget_FocusChanged);
			this.pictTarget.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictTarget_MouseDown);
			this.pictTarget.MouseEnter += new System.EventHandler(this.pictTarget_FocusChanged);
			this.pictTarget.MouseLeave += new System.EventHandler(this.pictTarget_FocusChanged);
			this.pictTarget.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictTarget_MouseMove);
			this.pictTarget.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictTarget_MouseUp);
			this.pictTarget.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.pictTarget_MouseWheel);
			this.pictTarget.Resize += new System.EventHandler(this.pictTarget_Resize);
			// 
			// btnEditPanel
			// 
			this.btnEditPanel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnEditPanel.Location = new System.Drawing.Point(429, 77);
			this.btnEditPanel.Name = "btnEditPanel";
			this.btnEditPanel.Size = new System.Drawing.Size(32, 26);
			this.btnEditPanel.TabIndex = 12;
			this.btnEditPanel.TabStop = false;
			this.btnEditPanel.Text = ">>";
			this.btnEditPanel.Click += new System.EventHandler(this.btnEditPanel_Click);
			// 
			// MainForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(1065, 732);
			this.Controls.Add(this.toolStripContainer1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.KeyPreview = true;
			this.MainMenuStrip = this.menuStrip1;
			this.Menu = this.mnuMain;
			this.MinimumSize = new System.Drawing.Size(600, 542);
			this.Name = "MainForm";
			this.Text = "RCM (RigicChip Modeler)";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.MainForm_Closing);
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
			this.Load += new System.EventHandler(this.MainForm_Load);
			this.Click += new System.EventHandler(this.MainForm_Click);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
			this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.MainForm_KeyPress);
			this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyUp);
			this.Resize += new System.EventHandler(this.MainForm_Resize);
			this.panelCtrl.ResumeLayout(false);
			this.panelCtrl.PerformLayout();
			this.panelAttr.ResumeLayout(false);
			this.panelAttrValue.ResumeLayout(false);
			this.panelAttrName.ResumeLayout(false);
			this.tabGI.ResumeLayout(false);
			this.tpAngle.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pictAngle)).EndInit();
			this.tpPalette.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pictPalette)).EndInit();
			this.toolStripContainer1.ContentPanel.ResumeLayout(false);
			this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
			this.toolStripContainer1.TopToolStripPanel.PerformLayout();
			this.toolStripContainer1.ResumeLayout(false);
			this.toolStripContainer1.PerformLayout();
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			this.pictTarget.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// アプリケーションのメイン エントリ ポイントです。
		/// </summary>
		[STAThread]
		static void Main(string[] args) {
			// 先にこれをやっておくと後々ちょっと速い(何というバッドノウハウ)
			try {
				throw new Exception();
			}
			catch { }

			/*			try{
							Application.Run(new frmMain());
						}catch(Exception e){
							MessageBox.Show(e.Message);
						}
			*/
			MainForm mainform = new MainForm(args);
#if !DEBUG
			try{
				Application.Run(mainform);
			}
			catch(Exception e){
				//	TODO:	ここにレジュームファイルを生成するコードを書く。
				try{
					//mainform.rcdata.Save(Application.StartupPath + "\\_resume.$cm",mainform.GenerateOptList());
					////				MessageBox.Show("The following unexpected error occurred。\nWe apologize for the inconvenience...\n\n" + e.Message + "\n\nRecovery file created.","RCM",MessageBoxButtons.OK,MessageBoxIcon.Error);
					//StreamWriter dmp = new StreamWriter(Application.StartupPath + "\\error.log",false);
					//dmp.Write(e.ToString());
					//dmp.Flush();
					//dmp.Close();

					string output = mainform.rcdata.vals.ToString() + mainform.rcdata.keys.ToString() + mainform.rcdata.model.ToString();
					if (mainform.rcdata.script != "")
					{
						output += (mainform.rcdata.luascript ? "Lua" : "Script") + "{\r\n" + mainform.rcdata.script + "}\r\n";
					}
					StreamWriter tw = new StreamWriter(Application.StartupPath + "\\_resume.$cm", false, System.Text.Encoding.Default);
					//				MessageBox.Show(output);
					//				MessageBox.Show(rcdata.chipCount.ToString());
					tw.Write(output);
					tw.Flush();
					tw.Close();

					MessageBox.Show("The following unexpected error occurred。\nWe apologize for the inconvenience...\n\n" + e.Message + "\n\nRecovery file created.", "RCM", MessageBoxButtons.OK, MessageBoxIcon.Error);

				}
				catch (Exception e2){
					throw new IOException("error dump failed: " + e2.Message, e);
				}
				throw;
			}
#else
			// デバッグ時は、例外は外まで飛ばす。
			Application.Run(mainform);
#endif
		}

		///<summery>
		///編集中のモデルが変更されたことを示すフラグを取得または設定する。
		///</summery>
		public bool Modified {
			get {
				return modified;
			}
			set {
				if (modified == value) return;
				modified = value;
				miFileSave.Enabled = modified;
				SetWindowTitle();
			}
		}

		///<summery>
		///現在編集中のファイルの名前。タイトルバーに表示される。
		///</summery>
		public string EditingFileName {
			get {
				return editingFileName;
			}
			set {
				if (editingFileName == value) return;
				editingFileName = value;
				SetWindowTitle();
			}
		}

		///<summery>
		///タイトルバーに編集中のファイル名、未保存状態表示(*)、ソフト名を設定する。
		///</summery>
		private void SetWindowTitle() {
			this.SuspendLayout();
			this.Text = System.IO.Path.GetFileName(editingFileName);
			if (this.Text == "") this.Text = "(Untitled)";
			this.Text += (modified ? "*" : "") + " - RCM (English)";
			this.ResumeLayout();
		}

		private void MainForm_Load(object sender, System.EventArgs e) {
#if DEBUG
#if TEST
			labelTip.Text += "(Test)";
#else
			labelTip.Text += "(Debug)";
#endif
#endif
			NameValueCollection opts;
			draging.Draging = false;

			// ツールボックスの選択状態を初期状態(選択)に設定する。
			//selected = tbbCursor;
			//selectedButton = tsbSelectMode;
			tsbSelectMode.PerformClick();

			InitializeGraphics();

			string resourcepath;

			StreamReader infile;
			StreamWriter outfile;
			try {
				infile = new StreamReader(Application.StartupPath + "\\ResourcePath.txt");
				resourcepath = infile.ReadLine();
				infile.Close();
				if (!File.Exists(resourcepath + "\\Core.x"))
					throw new Exception();
			}
			catch {
				dlgOpen.Filter = "Core.x in Resource folder|Core.x";
				dlgOpen.AddExtension = false;
				dlgOpen.CheckFileExists = true;
				dlgOpen.CheckPathExists = true;
				dlgOpen.DereferenceLinks = false;
				dlgOpen.Multiselect = false;
				dlgOpen.RestoreDirectory = true;
				dlgOpen.Title = "Please specify Core.x in RigidChips' resource folder.";

				if (dlgOpen.ShowDialog() == DialogResult.Cancel)
					Application.Exit();

				resourcepath = Path.GetDirectoryName(dlgOpen.FileName);

				outfile = new StreamWriter(Application.StartupPath + "\\ResourcePath.txt");
				outfile.Write(resourcepath);
				outfile.Close();

			}

			try {
				rcdata = new RigidChips.Environment(device, drawOption, outputOption, editOption, resourcepath);
				weightBall = new RigidChips.XFile();
				weightBall.Load(device, Application.StartupPath + @"\Resources\weight.x");
				multiSelCursor = new RigidChips.XFile();
				multiSelCursor.Load(device, Application.StartupPath + @"\Resources\cursor3.x");
			}
			catch (FileNotFoundException ex) {
				MessageBox.Show("Exiting due to resource loading failure。\nFile: " + ex.FileName, "FileNotFountException", MessageBoxButtons.OK, MessageBoxIcon.Error);
				this.Dispose(true);
				Application.Exit();
			}

			rcdata.OnSelectedChipChanged += new MessageCallback(RcData_SelectedChipChanged);
			rcdata.SelectedChip = rcdata.model.root;


			Initialized = true;
			try {
				pictPalette.Image = Image.FromFile("content/Palette.bmp");
				pictPalette.SizeMode = PictureBoxSizeMode.StretchImage;
			}
			catch {
				// パレット用ビットマップが見つからない場合は、パレット不使用。
				pictPalette.Visible = false;
			}

			UpdateCameraPosition(0, 0, 0, Matrix.Identity);
			configwindow = new ConfigForm(this);

			bool buffer = editOption.ConvertParentAttributes;
			editOption.ConvertParentAttributes = false;

			// 引数がある場合、そのファイルを読み込む。
			if (Arguments.Length > 0) {
				if (System.IO.Path.GetExtension(Arguments[0]).ToLower() == ".rcm") {
					opts = rcdata.Load(Arguments[0]);
					EditingFileName = Arguments[0];
					ApplyOptList(opts);
				}
				else {
					rcdata.headercomment = rcdata.script = "";
					rcdata.SelectedChip = rcdata.model.root;
					//rcdata.RegisterChip(rcdata.model.root);
					infile = new StreamReader(Arguments[0], System.Text.Encoding.Default);

					rcdata.Parse(infile.ReadToEnd());
					infile.Close();
				}
				rcdata.SelectedChip = rcdata.model.root;
				SetValDropList();
			}

			// 引数が無く、なおかつ途中終了のレジュームデータがあった場合は、それを読み込むことを試みる。
			else if (System.IO.File.Exists(Application.StartupPath + "\\_resume.$cm")) {
				if (MessageBox.Show("Recovery file exists. do you want to load?", "RCM", MessageBoxButtons.YesNo) == DialogResult.Yes) {
					try {
						//opts = rcdata.Load(Application.StartupPath + "\\_resume.$cm");
						//EditingFileName = Application.StartupPath + "\\_resume.$cm";
						//ApplyOptList(opts);

						//rcdata.UnregisterChipAll(rcdata.model.root);
						//rcdata.keys = new KeyEntryList(RigidChips.Environment.KeyCount);
						//rcdata.vals = new ValEntryList();
						//rcdata.model = new Model(rcdata);

						rcdata.headercomment = rcdata.script = "";
						rcdata.SelectedChip = rcdata.model.root;
						//rcdata.RegisterChip(rcdata.model.root);

						//opts = rcdata.Load(dlgOpen.FileName);
						//ApplyOptList(opts);


						infile = new StreamReader(Application.StartupPath + "\\_resume.$cm", System.Text.Encoding.Default);

						rcdata.Parse(infile.ReadToEnd());

						rcdata.SelectedChip = rcdata.model.root;
						SetValDropList();

						infile.Close();

						Modified = true;
					}
					catch {
						MessageBox.Show("Failed to read. File might be corrupted.", "RCM", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					}
				}
				System.IO.File.Delete(Application.StartupPath + "\\_resume.$cm");
			}

			editOption.ConvertParentAttributes = buffer;
			SetListBackColor();
			SetWindowTitle();

			//tbbAutoCamera.Pushed = drawOption.AutoCamera;
			tsbAutoCamera.Checked = drawOption.AutoCamera;
		}

		///<summery>
		///子チップリストボックスの背景色を設定。
		///</summery>
		public void SetListBackColor() {
			this.lstNorth.BackColor = Color.FromArgb(drawOption.NGuideColor.R / 2 + 128, drawOption.NGuideColor.G / 2 + 128, drawOption.NGuideColor.B / 2 + 128);
			this.lstSouth.BackColor = Color.FromArgb(drawOption.SGuideColor.R / 2 + 128, drawOption.SGuideColor.G / 2 + 128, drawOption.SGuideColor.B / 2 + 128);
			this.lstEast.BackColor = Color.FromArgb(drawOption.EGuideColor.R / 2 + 128, drawOption.EGuideColor.G / 2 + 128, drawOption.EGuideColor.B / 2 + 128);
			this.lstWest.BackColor = Color.FromArgb(drawOption.WGuideColor.R / 2 + 128, drawOption.WGuideColor.G / 2 + 128, drawOption.WGuideColor.B / 2 + 128);

		}
		private void MainForm_Click(object sender, System.EventArgs e) {
			pictTarget_Paint(this, null);
		}

		private bool GuideEnabled {
			get {
				return rcdata.DrawOption.ShowGuideAlways ||  (!tsbSelectMode.Checked && !tsbCameraMode.Checked && panelCtrl.Visible);
			}
		}

		private void pictTarget_Paint(object sender, PaintEventArgs e) {
			if (deviceLost) {
				try {
					ReleaseGraphics();
					device.Reset(presentParams);
					deviceLost = false;
					ResetGraphics();
				}
				catch (DeviceLostException) {
					// まだリセットできない
					return;
				}
			}

			if (   !Initialized 
				|| Pause
				|| device == null
				|| this.WindowState == FormWindowState.Minimized
				|| NowClosing) {
				//				labelTip.Text = "painting is stopped.";
				return;
			}

			//			try{

			Material m = new Material();
			if (CameraOrtho != drawOption.CameraOrtho) {
				CameraOrtho = drawOption.CameraOrtho;
				if (CameraOrtho)
					device.Transform.Projection = Matrix.OrthoLH(CamDepth / 2f, CamDepth / 2f, -256f, 256f);
				else
					device.Transform.Projection = Matrix.PerspectiveFovLH(0.5f, (float)pictTarget.ClientSize.Width / (float)pictTarget.ClientSize.Height, 0.5f, 100.0f);
			}

			try {
				device.BeginScene();
				device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, drawOption.BackColor, 1.0f, 0);
				//rcdata.model.root.DrawChipAll();

				if (drawOption.ShowGuideAlways || !draging.Draging) {
					if (rcdata.SelectedChip != null)
						// rcdata.DrawChips(!tbbCursor.Pushed && !tbbCamera.Pushed && panelB.Visible);
						rcdata.DrawChips(GuideEnabled);
					else {
						ChipBase[] selectedChips = rcdata.SelectedChipList;
						//for (int i = 0; i < rcdata.chipCount; i++) {
						//    rcdata.GetChipFromLib(i).DrawChip();
						//}
						rcdata.DrawChips(false, false);
						if (rcdata.SelectedChipCount > 0) {
							foreach (ChipBase c in selectedChips) {
								multiSelCursor.Draw(device, drawOption.CursorFrontColor, 0x7000, c.FullMatrix);
							}
						}
					}
					device.VertexFormat = CustomVertex.PositionOnly.Format;
					device.SetStreamSource(0, vbGuide, 0);
					device.Transform.World = Matrix.Identity;

					if (drawOption.XAxisEnable) {
						m.Ambient = m.Diffuse = m.Emissive = m.Specular = drawOption.XAxisColor;
						device.Material = m;
						device.DrawPrimitives(PrimitiveType.LineList, 0, 1);
					}

					if (drawOption.XNegAxisEnable) {
						m.Ambient = m.Diffuse = m.Emissive = m.Specular = drawOption.XNegAxisColor;
						device.Material = m;
						device.DrawPrimitives(PrimitiveType.LineList, 1, 1);
					}

					if (drawOption.YAxisEnable) {
						m.Ambient = m.Diffuse = m.Emissive = m.Specular = drawOption.YAxisColor;
						device.Material = m;
						device.DrawPrimitives(PrimitiveType.LineList, 3, 1);
					}

					if (drawOption.YNegAxisEnable) {
						m.Ambient = m.Diffuse = m.Emissive = m.Specular = drawOption.YNegAxisColor;
						device.Material = m;
						device.DrawPrimitives(PrimitiveType.LineList, 4, 1);
					}

					if (drawOption.ZAxisEnable) {
						m.Ambient = m.Diffuse = m.Emissive = m.Specular = drawOption.ZAxisColor;
						device.Material = m;
						device.DrawPrimitives(PrimitiveType.LineList, 6, 1);
					}

					if (drawOption.ZNegAxisEnable) {
						m.Ambient = m.Diffuse = m.Emissive = m.Specular = drawOption.ZNegAxisColor;
						device.Material = m;
						device.DrawPrimitives(PrimitiveType.LineList, 7, 1);
					}

					if (drawOption.WeightEnable) {
						device.SetStreamSource(0, vbWeight, 0);
						device.Transform.World = Matrix.Translation(rcdata.WeightCenter);

						m.Ambient = m.Diffuse = m.Emissive = m.Specular = Color.FromArgb(127, drawOption.WeightColor);
						device.Material = m;
						device.DrawPrimitives(PrimitiveType.LineList, 0, 3);

						if (drawOption.WeightBallEnable) {
							weightBall.DrawTransparented(
								rcdata.d3ddevice,
								Color.FromArgb((int)(255 * drawOption.WeightBallAlpha), drawOption.WeightBallColor),
								Matrix.Scaling(drawOption.WeightBallSize, drawOption.WeightBallSize, drawOption.WeightBallSize) * device.Transform.World);
						}
					}
					else if (drawOption.WeightBallEnable) {
						weightBall.DrawTransparented(
							rcdata.d3ddevice,
							Color.FromArgb((int)(255 * drawOption.WeightBallAlpha), 0, 0, 0),
							Matrix.Scaling(drawOption.WeightBallSize, drawOption.WeightBallSize, drawOption.WeightBallSize) * Matrix.Translation(rcdata.WeightCenter));
					}


				}
				else {
					//for (int i = 0; i < rcdata.chipCount; i++) {
					//    RcChipBase b = rcdata.GetChipFromLib(i);
					//    if (b != null) b.DrawChip();
					//}
					rcdata.DrawChips(false, false);
				}
			}

			catch (DirectXException) {
#if DEBUG
				//					throw new Exception("フェイズ1での例外:" + exc.ToString(),exc);
				System.Threading.Thread.Sleep(200);
				ResetGraphics();
				//					throw;
#else

#endif
			}


			//			rcx.Draw(device,Color.FromArgb(40,255,0,0) ,Matrix.Identity);

			device.EndScene();

			try {
				device.Present();
				deviceLost = false;
			}
			catch (DeviceLostException) {
				// 何もしない。何もできない。
				deviceLost = true;
			}
			catch (DeviceNotResetException) {
				device.Reset(presentParams);
				ResetGraphics();
				deviceLost = false;
			}
			//			}
			//			catch(DirectXException dx){
			//				throw new Exception("描画中の例外 : " + dx.Message ,dx);
			//				System.Threading.Thread.Sleep(200);
			//				throw;
			//			}
		}

		private void pictTarget_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e) {
			MouseX = e.X;
			MouseY = e.Y;
			if (e.Button == MouseButtons.Right) {
				LeastIsLeftButton = false;

				HitStatus hit = rcdata.model.root.IsHit(MouseX, MouseY, pictTarget.ClientRectangle.Width, pictTarget.ClientRectangle.Height);
				draging.StartX = draging.PrevX = e.X;
				draging.StartY = draging.PrevY = e.Y;
				draging.Draging = (hit.HitChip == null);
			}
			else if (e.Button == MouseButtons.Left)
				LeastIsLeftButton = true;
			CamDepth += e.Delta * 0.5f;

		}

		private void pictTarget_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e) {
			if (e.Button != MouseButtons.Right) return;
			draging.Draging = false;
			pictTarget_Paint(this, null);
		}

		private void pictTarget_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e) {
			MouseX = e.X;
			MouseY = e.Y;

			if (draging.Draging) {
				if ((e.Button & MouseButtons.Left) > 0)
					UpdateCameraPosition(0, 0, draging.PrevX + draging.PrevY - e.X - e.Y, CamNow);
				else
					UpdateCameraPosition(
						(editOption.InvertRotateX ? -1 : 1) * (e.X - draging.PrevX),
						(editOption.InvertRotateY ? -1 : 1) * (draging.PrevY - e.Y),
						e.Delta,
						CamNow
					);

				draging.PrevX = e.X;
				draging.PrevY = e.Y;

				return;
			}

		}

		private void pictTarget_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e) {
			UpdateCameraPosition(0, 0, (editOption.InvertWheel ? -1 : 1) * e.Delta / 60, CamNow);
		}

		private void tmr_Tick(object sender, System.EventArgs e) {
			if (PreviewMode) {
				rcdata.vals.Tick();
				for (int i = 0; i < previewKeys.Length; i++) {
					bool btn = previewKeys[i];
					if (btn) {
						KeyEntry k = rcdata.keys[i];
						foreach (var v in k.Works) {
							v.Target.Tick(v.Step);
						}
						Debug.WriteLine(i, "Preview");
					}
				}
				foreach(var v in rcdata.vals.List)
					if(v != null)
						v.ApplyDefault();
				rcdata.model.root.UpdateMatrix();
				rcdata.CalcWeightCenter();
				rcdata.vals.Tack();
			}
			pictTarget_Paint(sender, null);

		}

		///<summery>
		/// 一時ファイルに現在のデータを保存し、それを起動済みRigidChipsに読み込ませる。
		///</summery>
		private void miToolSend_Click(object sender, System.EventArgs e) {
			string output = rcdata.vals.ToString() + rcdata.keys.ToString() + rcdata.model.ToString();
			if (rcdata.script != "") {
				output += (rcdata.luascript ? "Lua" : "Script") + "{\r\n" + rcdata.script + "}\r\n";
			}
			//			string filename = Application.StartupPath + "\\out" + (DateTime.Now.Ticks & 0xFFFF).ToString("X") + ".rcd_";
			string filename = Application.StartupPath + "\\out.rcd_";
			byte[] filenameArray;
			StreamWriter tw = new StreamWriter(filename);
			tw.Write(output);
			tw.Flush();
			tw.Close();

			filenameArray = System.Text.Encoding.Convert(System.Text.Encoding.Unicode, System.Text.Encoding.GetEncoding(932 /* Shift-JIS */), System.Text.Encoding.Unicode.GetBytes(filename));

			WM_RIGIDCHIP_LOAD = RegisterWindowMessageA("WM_RIGIDCHIP_LOAD");

			PostMessageA(hWnd_Broadcast, WM_RIGIDCHIP_LOAD, Msg_RcLoadStart, 0);

			for (int i = 0; i < filenameArray.Length; i++) {
				PostMessageA(hWnd_Broadcast, WM_RIGIDCHIP_LOAD, Msg_RcLoadChar, filenameArray[i]);
			}

			PostMessageA(hWnd_Broadcast, WM_RIGIDCHIP_LOAD, Msg_RcLoadEnd, 0);

			labelTip.Text = "Sent a message to RigidChips";

		}

		private void MoveCursor(ChipBase x) {
			if (x != rcdata.SelectedChip) {
				if ((Control.ModifierKeys & Keys.Control) == 0) {
					rcdata.SelectedChip = x;
					labelTip.Text = "Moved To cursor: " + x.ToString() +
						((x.Comment != null && x.Comment != "") ?
						" (" + x.Comment + ")" : "");
				}
				else {
					rcdata.AssignSelectedChips(x);
				}
			}
		}

		private void pictTarget_Click(object sender, System.EventArgs e) {
			pictTarget.Focus();
			HitStatus cursors, models;
			cursors = rcdata.Cursor.IsHit(MouseX, MouseY, pictTarget.ClientRectangle.Width, pictTarget.ClientRectangle.Height, GuideEnabled);
			models = rcdata.model.root.IsHit(MouseX, MouseY, pictTarget.ClientRectangle.Width, pictTarget.ClientRectangle.Height);

			ChipBase clickedChip = cursors.distance < models.distance ? cursors.HitChip : models.HitChip;


			if (!LeastIsLeftButton || draging.Draging) {
				if (draging.StartX != draging.PrevX || draging.StartY != draging.PrevY) return;
				if (panelCtrl.Visible && models.HitChip != null) {
					// コンテキストメニュー表示(他のものより優先)
					draging.Draging = false;
					rcdata.SelectedChip = models.HitChip;
					ctmChipType.Show(pictTarget, new Point(MouseX, MouseY));
				}
				return;
			}

			// 虚空をクリックしていたときにはここ以降の処理は行わない
			if (clickedChip == null) return;

			if (clickedChip is CursorChip || clickedChip == rcdata.SelectedChip) {
				// カーソルか、選択中チップの場合は視点移動
				StartScrollCameraPosition(cursors.HitChip.Matrix);
				return;
			}
			if (!(clickedChip is GuideChip) && clickedChip == models.HitChip) {
				// ガイドでなければモデル中のチップのはず => 選択(視点移動モードの時は視点移動)
				if (selectedButton.Text == "Perspective")
					StartScrollCameraPosition(clickedChip.Matrix);
				else
					MoveCursor(clickedChip);
				return;
			}

			// ここからガイド選択時の追加などの処理
			if (!panelCtrl.Visible) {
				// 広角モードの時はここから先の処理はしない
				return;
			}

			if (editOption.AttributeAutoApply)
				ApplyChipInfo();

			void add(ChipBase x) {
				try {
					rcdata.SelectedChip = x;
					Modified = true;
					if (treeview != null && !treeview.IsDisposed) treeview.UpdateTree(rcdata.SelectedChip);
					labelTip.Text = selectedButton.Text + " added.";
				}
				catch (Exception err) {
					MessageBox.Show(err.Message, "Failed adding.");
					return;
				}
			}

			switch (selectedButton.Text) {
				case "Select":
				case "Perspective":
					// 何もしない
					return;
				case "Paste":
					try {
						ChipBase buffer = clipboard.Clone(true, null);
						buffer.Attach(rcdata.SelectedChip, cursors.HitChip.JointPosition);
						rcdata.SelectedChip = buffer;
						Modified = true;
						if (treeview != null && !treeview.IsDisposed) treeview.UpdateTree(rcdata.SelectedChip);
						labelTip.Text = "Pasted.";
					}
					catch (NullReferenceException) {
						MessageBox.Show("Clipboard is empty.", "Failed adding.");
						return;
					}
					catch (Exception err) {
						MessageBox.Show(err.Message, "Failed adding.");
						return;
					}
					return;
				case "Chip":
					add(new NormalChip(rcdata, rcdata.SelectedChip, cursors.HitChip.JointPosition));
					break;
				case "Frame":
					add(new FrameChip(rcdata, rcdata.SelectedChip, cursors.HitChip.JointPosition));
					break;
				case "Weight":
					add(new WeightChip(rcdata, rcdata.SelectedChip, cursors.HitChip.JointPosition));
					break;
				case "Rudder":
					add(new RudderChip(rcdata, rcdata.SelectedChip, cursors.HitChip.JointPosition));
					break;
				case "Rudder frame":
					add(new RudderFrameChip(rcdata, rcdata.SelectedChip, cursors.HitChip.JointPosition));
					break;
				case "Trim":
					add(new TrimChip(rcdata, rcdata.SelectedChip, cursors.HitChip.JointPosition));
					break;
				case "Trim frame":
					add(new TrimFrameChip(rcdata, rcdata.SelectedChip, cursors.HitChip.JointPosition));
					break;
				case "Wheel":
					add(new WheelChip(rcdata, rcdata.SelectedChip, cursors.HitChip.JointPosition));
					break;
				case "Recoilless wheel":
					add(new RLWChip(rcdata, rcdata.SelectedChip, cursors.HitChip.JointPosition));
					break;
				case "Jet":
					add(new JetChip(rcdata, rcdata.SelectedChip, cursors.HitChip.JointPosition));
					break;
				case "Arm":
					add(new ArmChip(rcdata, rcdata.SelectedChip, cursors.HitChip.JointPosition));
					break;
				case "Cowl":
					try {
						rcdata.SelectedChip = new CowlChip(rcdata, rcdata.SelectedChip, cursors.HitChip.JointPosition);
						Modified = true;
						if (treeview != null && !treeview.IsDisposed) treeview.UpdateTree(rcdata.SelectedChip);
						labelTip.Text = selectedButton.Text + (drawOption.ShowCowl ? " added." : " added. (Currently hidden)");
					}
					catch (Exception err) {
						MessageBox.Show(err.Message, "Additional error");
						labelTip.Text = "Moved the cursor : " + rcdata.SelectedChip.ToString() +
							((rcdata.SelectedChip.Comment != null && rcdata.SelectedChip.Comment != "") ?
							" (" + rcdata.SelectedChip.Comment + ")" : "");
						return;
					}

					break;
				default:
					break;
			}
			rcdata.CalcWeightCenter();
			//rcdata.CheckBackTrack();
			pictTarget_Paint(this, null);


			//			UpdateCameraPosition(0,0,0,rcdata.Cursor.Matrix);

		}

		private void actionCut() {
			if (rcdata.SelectedChipCount > 0)
				return;
			actionCut(rcdata.SelectedChip);
		}
		private void actionCut(ChipBase targetChip) {
			if (targetChip == null)
				return;
			if (targetChip is CoreChip) {
				labelTip.Text = "You can't cut the core";
				return;
			}
			
			if (targetChip.Ancestor != rcdata.model.root) {
				// このチップは登録されていないチップ -> 切り取れない
				return;
			}
			clipboard = targetChip;
			if(rcdata.SelectedChip == targetChip)
				rcdata.SelectedChip = clipboard.Parent;
			targetChip.Detach();
			Modified = true;

			if (treeview != null && !treeview.IsDisposed) treeview.GenerateTree();
			labelTip.Text = "Made a cut.";
		}

		private void actionCopy() {
			actionCopy(rcdata.SelectedChip);
		}
		private void actionCopy(ChipBase targetChip) {
			//	コピー動作
			if (targetChip == null)
				return;
			if (targetChip is CoreChip) {
				labelTip.Text = "Core cannot be copied.";
				return;
			}
			clipboard = targetChip.Clone(true, null);
			labelTip.Text = "Copied.";
		}

		private void actionDelete() {
			if (rcdata.SelectedChip is CoreChip) {
				labelTip.Text = "Core cannot be deleted";
				return;
			}
			if (rcdata.SelectedChipCount > 0) {
				// 複数選択時
				if (MessageBox.Show("Delete all selected chips。\nIf any of the chips have children, they will also be deleted.", "Multiple selection deletion confirmation", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK) {
					ChipBase[] list = rcdata.SelectedChipList;
					foreach (ChipBase c in list)
						try {
							if (!(c is CoreChip))
								c.Detach();
						}
						catch { }
					rcdata.SelectedChip = rcdata.model.root;
				}
				return;
			}
			foreach (ChipBase cb in rcdata.SelectedChip.Children) {
				if (cb != null)
					if (MessageBox.Show("This chip has children. Are you sure you want to delete this?", "Delete confirmation", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
						== DialogResult.Cancel)
						return;
					else
						break;
			}
			ChipBase buff = rcdata.SelectedChip.Parent;
			try { rcdata.SelectedChip.Detach(); }
			catch { };
			rcdata.SelectedChip = buff;
			Modified = true;
			if (treeview != null && !treeview.IsDisposed) treeview.GenerateTree();
			labelTip.Text = "Deleted.";
		}
		private void actionDelete(ChipBase targetChip){
			if (targetChip is CoreChip) {
				labelTip.Text = "Core cannot be deleted.";
				return;
			}
			foreach (ChipBase cb in targetChip.Children) {
				if (cb != null)
					if (MessageBox.Show("This chip has children. Are you sure you want to delete this?", "Delete confirmation", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
						== DialogResult.Cancel)
						return;
					else
						break;
			}
			ChipBase buff = null;
			if (targetChip == rcdata.SelectedChip) buff = targetChip.Parent;

			try { targetChip.Detach(); }
			catch { };
			if (buff != null)
				rcdata.SelectedChip = buff;
			else
				LoadChipInfo();

			Modified = true;
			if (treeview != null && !treeview.IsDisposed) treeview.GenerateTree();
			labelTip.Text = "Deleted.";

		}
#if false
		private void tbMain_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e) {
			if(e.Button == tbbCut){
				actionCut();
			}
			else if(e.Button == tbbCopy){
				actionCopy();
			}
			else if(e.Button == tbbRemove){
				actionRemove();
			}
			else if(e.Button == tbbInsert){
				if(rcdata.SelectedChip is RcChipCore){
					labelTip.Text = "挿入する場所がありません。";
					return;
				}
				RcChipBase prevParent = rcdata.SelectedChip.Parent;
				RcJointPosition prevJP = rcdata.SelectedChip.JointPosition;

				RcChipBase buffer;

				switch(selected.Text){
					case "貼り付け":
						labelTip.Text = "貼り付けモードの時は挿入はできません。";
						return;
					case "視点":
						labelTip.Text = "視点モードの時は挿入はできません。";
						return;
					case "チップ":
						try{
							buffer = new RcChipChip(rcdata,prevParent,prevJP);

						}
						catch(Exception err){
							MessageBox.Show(err.Message,"追加エラー");
							return;
						}
					
						break;
					case "フレーム":
						try{
							buffer = new RcChipFrame(rcdata,prevParent,prevJP);

						}
						catch(Exception err){
							MessageBox.Show(err.Message,"追加エラー");
							return;
						}
					

						break;
					case "ウェイト":
						try{
							buffer = new RcChipWeight(rcdata,prevParent,prevJP);

						}
						catch(Exception err){
							MessageBox.Show(err.Message,"追加エラー");
							return;
						}
					

						break;
					case "カウル":
						try{
							buffer = new RcChipCowl(rcdata,prevParent,prevJP);
						}
						catch(Exception err){
							MessageBox.Show(err.Message,"追加エラー");
							return;
						}
					
						break;
					case "ラダー":
						try{
							buffer = new RcChipRudder(rcdata,prevParent,prevJP);

						}
						catch(Exception err){
							MessageBox.Show(err.Message,"追加エラー");
							return;
						}
					

						break;
					case "ラダーフレーム":
						try{
							buffer = new RcChipRudderF(rcdata,prevParent,prevJP);

						}
						catch(Exception err){
							MessageBox.Show(err.Message,"追加エラー");
							return;
						}
					

						break;
					case "トリム":
						try{
							buffer = new RcChipTrim(rcdata,prevParent,prevJP);

						}
						catch(Exception err){
							MessageBox.Show(err.Message,"追加エラー");
							return;
						}
					

						break;
					case "トリムフレーム":
						try{
							buffer = new RcChipTrimF(rcdata,prevParent,prevJP);

						}
						catch(Exception err){
							MessageBox.Show(err.Message,"追加エラー");
							return;
						}
					

						break;
					case "ホイール":
						try{
							buffer = new RcChipWheel(rcdata,prevParent,prevJP);

						}
						catch(Exception err){
							MessageBox.Show(err.Message,"追加エラー");
							return;
						}
					
						break;
					case "無反動ホイール":
						try{
							buffer = new RcChipRLW(rcdata,prevParent,prevJP);

						}
						catch(Exception err){
							MessageBox.Show(err.Message,"追加エラー");
							return;
						}
					

						break;
					case "ジェット":
						try{
							buffer = new RcChipJet(rcdata,prevParent,prevJP);

						}
						catch(Exception err){
							MessageBox.Show(err.Message,"追加エラー");
							return;
						}
					
						break;
					case "アーム":
						try{
							buffer = new RcChipArm(rcdata,prevParent,prevJP);

						}
						catch(Exception err){
							MessageBox.Show(err.Message,"追加エラー");
							return;
						}
					

						break;
					default:
						labelTip.Text = "不明のモードです。挿入はできません。";
						return;
				}

				try{
					rcdata.SelectedChip.Detach();
					rcdata.SelectedChip.Attach(buffer,prevJP);
				}catch(Exception err){
					MessageBox.Show(err.Message,"挿入エラー",MessageBoxButtons.OK,MessageBoxIcon.Error);
					try{
						buffer.Detach();
					}catch{}
					try{
						rcdata.SelectedChip.Attach(prevParent,prevJP);
					}catch{}
				}

				
				LoadChipInfo();
				ProcessViewPoint(rcdata.Cursor.Matrix);
				Modified = true;
				if(treeview != null && !treeview.IsDisposed)treeview.GenerateTree();
				labelTip.Text = selected.Text + "を挿入しました。";
			}
			else if(e.Button == tbbZoom){
				CamDepth /= 1.5f;
				UpdateCameraPosition(0,0,0,CamNow);
			}
			else if(e.Button == tbbMooz){
				CamDepth *= 1.5f;
				UpdateCameraPosition(0,0,0,CamNow);
			}
			else if(e.Button == tbbAutoCamera){
				StartScrollCameraPosition(rcdata.Cursor.Matrix);
				labelTip.Text = "カーソル自動追尾 : " + (e.Button.Pushed ? "ON" : "OFF");
				drawOption.AutoCamera = e.Button.Pushed;
			}
			else{
				//	各種モード変更
				foreach(ToolBarButton tbb in tbMain.Buttons)
					if(tbb != tbbAutoCamera) tbb.Pushed = false;
				e.Button.Pushed = true;
				selected = e.Button;
				labelTip.Text = "追加モード : " + e.Button.Text;
			}

			pictTarget_Paint(this,null);
		}
#endif
		///<summery>
		///Direct3Dとかの初期化。
		///</summery>
		private bool InitializeGraphics() {
			AdapterInformation a = Manager.Adapters.Default;
			caps = Manager.GetDeviceCaps(a.Adapter, DeviceType.Hardware);

			presentParams.Windowed = true;
			//				presentParams.BackBufferCount = 1;
			//				presentParams.BackBufferHeight = presentParams.BackBufferWidth = 0;
			presentParams.SwapEffect = SwapEffect.Discard;
			presentParams.EnableAutoDepthStencil = true;
			presentParams.AutoDepthStencilFormat = DepthFormat.Unknown;

			DepthFormat[] formats = new DepthFormat[]{
				DepthFormat.D24X8,
				DepthFormat.D24X4S4,
				DepthFormat.D24SingleS8,
				DepthFormat.D24S8,
				DepthFormat.L16,
				DepthFormat.D16,
				DepthFormat.D15S1,
			};

			for (int i = 0; i < formats.Length; i++) {
				if (Manager.CheckDepthStencilMatch(a.Adapter, DeviceType.Hardware, a.CurrentDisplayMode.Format, a.CurrentDisplayMode.Format, formats[i])) {
					presentParams.AutoDepthStencilFormat = formats[i];
#if DEBUG
					Console.Write(formats[i].ToString());
#endif
					break;
				}
			}

			if (presentParams.AutoDepthStencilFormat == DepthFormat.Unknown)
				throw new ApplicationException("DepthFormatの設定に失敗。");


			try {
				device = new Device(
					0,
					DeviceType.Hardware,
					this.pictTarget,
					CreateFlags.SoftwareVertexProcessing,
					presentParams);
			}
			catch {
				try {
					device = new Device(
						0,
						DeviceType.Software,
						this.pictTarget,
						CreateFlags.SoftwareVertexProcessing,
						presentParams);
				}
				catch (Exception e) {
					MessageBox.Show("Failed to initialize Direct3D device.", e.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}


			vbGuide = new VertexBuffer(
				typeof(CustomVertex.PositionOnly),
				9,
				device,
				0,
				CustomVertex.PositionOnly.Format,
				Pool.Default);

			//				defaultMat = device.Material;

			GraphicsStream gs;
			gs = vbGuide.Lock(0, 0, 0);
			lineCV = new Microsoft.DirectX.Direct3D.CustomVertex.PositionOnly[9];	//	PositionColored

			lineCV[0].Position = new Vector3(100f, 0f, 0f);
			lineCV[1].Position = new Vector3(0f, 0f, 0f);
			lineCV[2].Position = new Vector3(-100f, 0f, 0f);
			lineCV[3].Position = new Vector3(0f, 100f, 0f);
			lineCV[4].Position = new Vector3(0f, 0f, 0f);
			lineCV[5].Position = new Vector3(0f, -100f, 0f);
			lineCV[6].Position = new Vector3(0f, 0f, 100f);
			lineCV[7].Position = new Vector3(0f, 0f, 0f);
			lineCV[8].Position = new Vector3(0f, 0f, -100f);

			gs.Write(lineCV);
			vbGuide.Unlock();

			vbWeight = new VertexBuffer(
				typeof(CustomVertex.PositionOnly),
				6,
				device,
				0,
				CustomVertex.PositionOnly.Format,
				Pool.Default);

			gs = vbWeight.Lock(0, 0, 0);

			lineCV = new CustomVertex.PositionOnly[6];

			lineCV[0].Position = new Vector3(100f, 0f, 0f);
			lineCV[1].Position = new Vector3(-100f, 0f, 0f);
			lineCV[2].Position = new Vector3(0f, 100f, 0f);
			lineCV[3].Position = new Vector3(0f, -100f, 0f);
			lineCV[4].Position = new Vector3(0f, 0f, 100f);
			lineCV[5].Position = new Vector3(0f, 0f, -100f);

			gs.Write(lineCV);
			vbWeight.Unlock();

			device.RenderState.ZBufferEnable = true;
			device.Transform.World = Matrix.Identity;
			device.Transform.View = Matrix.LookAtLH(
				new Vector3(0.0f, 0.0f, 0.0f),
				new Vector3(0.0f, 0.0f, 0.0f),
				new Vector3(0.0f, 1.0f, 0.0f));
			//				device.Transform.Projection = Matrix.PerspectiveFovLH(90/180*(float)Math.PI ,1.0f,0.5f,100.0f);
			device.Transform.Projection = Matrix.PerspectiveFovLH(0.5f, (float)pictTarget.ClientSize.Width / (float)pictTarget.ClientSize.Height, 0.5f, 100.0f);

			/*device.Lights[0].Ambient = Color.Gray;
			device.Lights[0].Diffuse = Color.White;
			device.Lights[0].Specular = Color.White;
			device.Lights[0].Type = LightType.Directional;
			device.Lights[0].Direction = new Vector3(1f, -1f, 1f);
			device.Lights[0].Enabled = true;
			device.Lights[0].Update();

			device.Lights[1].Ambient = Color.White;
			device.Lights[1].Diffuse = Color.White;
			device.Lights[1].Type = LightType.Directional;
			device.Lights[1].Direction = new Vector3(0, 1, 0);
			device.Lights[1].Enabled = false;
			device.Lights[1].Update();*/

			//device.RenderState.Lighting = true;
						device.RenderState.Ambient = Color.White;
			device.RenderState.CullMode = Cull.CounterClockwise;

			//				device.RenderState.AlphaBlendOperation = BlendOperation.Add;
			device.RenderState.SourceBlend = Blend.SourceAlpha;
			device.RenderState.DestinationBlend = Blend.InvSourceAlpha;
			device.RenderState.AlphaBlendEnable = true;
			//device.RenderState.SpecularEnable = true;
			device.RenderState.AntiAliasedLineEnable = true;
			/*			device.RenderState.DiffuseMaterialSource = ColorSource.Material;
						device.RenderState.EmissiveMaterialSource = ColorSource.Material;
						device.RenderState.SpecularMaterialSource = ColorSource.Material;
			*/
			//				rcx = new RcXFile();
			//				rcx.FileName = "cursor3.x";
			//				rcx.Load(device);

			device.DeviceLost += new EventHandler(Device_DeviceLost);
			//			device.DeviceReset += new EventHandler(Device_DeviceReset);

			return true;
		}

		/*
		  なんかもう、この辺↓どうしたらいいかさっぱりです
		*/

		private void Device_DeviceReset(object sender, EventArgs e) {
			Pause = true;
			int result;
			if (!device.CheckCooperativeLevel(out result) && this.WindowState != FormWindowState.Minimized)
				device.Reset(presentParams);
			Pause = false;
		}

		private void Device_DeviceLost(object sender, EventArgs e) {
			//			deviceLost = true;
			//			tmr.Enabled = false;
			System.Threading.Thread.Sleep(200);

			//			ResetGraphics();



			//			Pause = true;
			//			throw new Exception("デバイスが消失しました。");
			/*			if(Pause)return;
			Pause = true;
			try{
				device.Reset(presentParams);
			}
			catch{}
*/
		}
		#region 残骸
		/*
		private void Device_DeviceLost(object sender,EventArgs e){
			rcdata.DisposeMeshes();
			vbGuide.Dispose();

			try{
				device.Reset(presentParams);
			}catch(DeviceNotResetException exc){
				MessageBox.Show(exc.Message);
			}

			vbGuide = new VertexBuffer(
				typeof(CustomVertex.PositionColored),
				6,
				device,
				0,
				CustomVertex.PositionColored.Format,
				Pool.Default);

			GraphicsStream gs;
			gs = vbGuide.Lock(0,0,0);
			lineCV = new Microsoft.DirectX.Direct3D.CustomVertex.PositionColored[6];

			lineCV[0].SetPosition(new Vector3(0f,0f,0f));
			lineCV[0].Color = 0xFF0000;
			lineCV[1].SetPosition(new Vector3(-100f,0f,0f));
			lineCV[1].Color = 0xFF0000;
			lineCV[2].SetPosition(new Vector3(0f,0f,0f));
			lineCV[2].Color = 0x00FF00;
			lineCV[3].SetPosition(new Vector3(0f,100f,0f));
			lineCV[3].Color = 0x00FF00;
			lineCV[4].SetPosition(new Vector3(0f,0f,0f));
			lineCV[4].Color = 0x0000FF;
			lineCV[5].SetPosition(new Vector3(0f,0f,-100f));
			lineCV[5].Color = 0x0000FF;

			gs.Write(lineCV);

			vbGuide.Unlock();
			device.RenderState.ZBufferEnable = true;
			device.Transform.World = Matrix.Identity;
			device.Transform.View = Matrix.LookAtLH( 
				new Vector3( 0.0f, 0.0f, 0.0f ),
				new Vector3( 0.0f, 0.0f, 0.0f ),
				new Vector3( 0.0f, 1.0f, 0.0f ) );
			device.Transform.Projection = Matrix.PerspectiveFovLH(0.5f ,1.0f,0.5f,100.0f);

			device.Lights[0].Ambient = Color.White;
			device.Lights[0].Diffuse = Color.White;
			device.Lights[0].Specular = Color.White;
			device.Lights[0].Type = LightType.Directional;
			device.Lights[0].Direction = new Vector3(1f,1f,1f);
			device.Lights[0].Enabled = true;
			device.RenderState.Lighting = true;
			device.RenderState.CullMode = Cull.CounterClockwise;

			device.RenderState.SourceBlend = Blend.SourceAlpha;
			device.RenderState.DestinationBlend = Blend.InvSourceAlpha;
			device.RenderState.AlphaBlendEnable = true;


			rcdata.ReloadMeshes();
		}
		*/
		#endregion

		/// <summary>
		/// 現在選択されているチップの情報を右パネルに表示する。
		/// </summary>
		public void LoadChipInfo() {
			if (rcdata.SelectedChipCount > 0) {
				panelCtrl.Enabled = true;
				ChipBase[] chips = rcdata.SelectedChipList;
				string[] s = chips[0].AttrNameList;
				bool[,] enables = new bool[s.Length, 2];
				ChipAttribute[] values = chips[0].AttrValList;
				ChipAttribute opp;
				int attrindex = 0;
				panelAttr.SuspendLayout();
				for (int i = 0; i < enables.Length / 2; i++)
					enables[i, 0] = enables[i, 1] = true;
				foreach (ComboBox box in cmbAttrItems)
					box.Visible = false;
				foreach (Label l in labelAttrItems)
					l.Visible = false;

				ChipBase target;
				for (int i = 0; i < s.Length; i++) {
					foreach (ChipBase c in chips) {
						target = c;
						try {
							if (Array.Find(target.AttrNameList, x => x == s[i]) == null)
								enables[i, 0] = false;
							else {
								opp = target[s[i]];
								//if(opp.Const != values[i].Const || (opp.Val != values[i].Val && opp.isNegative == values[i].isNegative))
								if (opp != values[i])		// この判定がうまくいっていないから直す
									enables[i, 1] = false;
							}
						}
						catch {
							enables[i, 0] = false;
							break;
						}
					}

					if (enables[i, 0]) {
						// 共通属性
						labelAttrItems[attrindex].Text = s[i];
						labelAttrItems[attrindex].Visible = cmbAttrItems[attrindex].Visible = true;
						if (enables[i, 1]) {
							// 同一値
							cmbAttrItems[attrindex].Text = values[i].ToString();
							if (values[i].Val != null) {
								cmbAttrItems[attrindex].BackColor = Color.FromKnownColor(KnownColor.Info);
								cmbAttrItems[attrindex].ForeColor = Color.FromKnownColor(KnownColor.InfoText);
							}
							else {
								cmbAttrItems[attrindex].BackColor = Color.FromKnownColor(KnownColor.Window);
								cmbAttrItems[attrindex].ForeColor = Color.FromKnownColor(KnownColor.WindowText);
							}
							//foreach(object c in chips)
							//    ((RcChipBase)c)[s[i]] = values[i];
							ttMain.SetToolTip(cmbAttrItems[attrindex], s[i]);
						}
						else
							cmbAttrItems[attrindex].Text = "";

						attrindex++;
					}
				}

				lstNorth.Items.Clear(); lstNorth.Enabled = false;
				lstSouth.Items.Clear(); lstSouth.Enabled = false;
				lstWest.Items.Clear(); lstWest.Enabled = false;
				lstEast.Items.Clear(); lstEast.Enabled = false;

				buttonSelChip.ImageIndex = -1;
//				cmbColor.Text = "";

				txtName.Text = "";
				txtName.Enabled = false;

				btnRootChip.Text = "Parent Chip";
				btnRootChip.Enabled = false;
				buttonSelChip.Enabled = false;

				pictAngle.Refresh();

				ttMain.SetToolTip(buttonSelChip, null);

				panelCtrl.ResumeLayout();
				parameterChanged = false;

			}
			else if (rcdata.SelectedChip != null) {
				panelCtrl.Enabled = true;
				ChipBase target = rcdata.SelectedChip;
				string[] s = target.AttrNameList;
				ChipAttribute attr;
				if (s == null)
					s = new string[0];
				for (int i = 0; i < labelAttrItems.Length; i++) {
					if (s.Length > i) {
						labelAttrItems[i].Text = s[i];
						attr = target[s[i]];
						cmbAttrItems[i].Text = attr.ToString();
						if (attr.Val != null) {
							cmbAttrItems[i].BackColor = Color.FromKnownColor(KnownColor.Info);
							cmbAttrItems[i].ForeColor = Color.FromKnownColor(KnownColor.InfoText);
						}
						else {
							cmbAttrItems[i].BackColor = Color.FromKnownColor(KnownColor.Window);
							cmbAttrItems[i].ForeColor = Color.FromKnownColor(KnownColor.WindowText);
						}
						target[s[i]] = attr;
						string tooltip = target.AttrTip(s[i]);
						if (attr.Val != null) {
							string valinfo = attr.Val.ToString();
							tooltip = tooltip + "\n" + valinfo;
						}
						ttMain.SetToolTip(cmbAttrItems[i], tooltip);
						labelAttrItems[i].Visible = true;
						cmbAttrItems[i].Visible = true;
					}
					else {
						labelAttrItems[i].Visible = false;
						cmbAttrItems[i].Visible = false;
					}
				}

				lstNorth.Items.Clear(); lstNorth.Enabled = true;
				lstSouth.Items.Clear(); lstSouth.Enabled = true;
				lstWest.Items.Clear(); lstWest.Enabled = true;
				lstEast.Items.Clear(); lstEast.Enabled = true;
				buttonSelChip.Enabled = true;

				foreach (ChipBase cb in target.Children) {
					if (cb != null) {
						switch (cb.JointPosition) {
							case JointPosition.North:
								lstNorth.Items.Add(cb);
								break;
							case JointPosition.East:
								lstEast.Items.Add(cb);
								break;
							case JointPosition.South:
								lstSouth.Items.Add(cb);
								break;
							case JointPosition.West:
								lstWest.Items.Add(cb);
								break;
						}
					}
				}

				buttonSelChip.ImageIndex = (int)(sbyte)ChipBase.CheckType(target);
				//cmbColor.Text = target.ChipColor.ToString();

				txtName.Text = target.Name;
				txtName.Enabled = true;


				if (btnRootChip.Enabled = (target.Parent != null))
					btnRootChip.Text = "Parent Chip\n[" + target.Parent.ToString() + "]";
				else
					btnRootChip.Text = "Parent Chip";
				//				btnRootChip.Enabled = true;

				pictAngle.Refresh();

				if (target.Comment == null || target.Comment == "")
					ttMain.SetToolTip(buttonSelChip, "No Comment");
				else
					ttMain.SetToolTip(buttonSelChip, "Comment：\n" + target.Comment);

				parameterChanged = false;
			}
			else {
				panelCtrl.Enabled = false;
			}
			panelAttr.ResumeLayout();
		}

		/// <summary>
		/// 右パネルの情報を選択されているチップに適用する。
		/// </summary>
		public void ApplyChipInfo() {
			if (!parameterChanged) return;
			if (rcdata.SelectedChipCount > 0) {
				// 複数選択時
				ChipBase[] array = rcdata.SelectedChipList;
				ChipAttribute attr;
				ChipBase target;
				for (int i = 0; i < labelAttrItems.Length; i++) {
					if (cmbAttrItems[i].Text == "")
					{
						continue;
					}
					else if (labelAttrItems[i].Visible)
					{
						foreach (ChipBase o in array)
						{
							target = o;
							try
							{
								attr = target[labelAttrItems[i].Text];
								attr.SetValue(cmbAttrItems[i].Text, rcdata.vals);
								if (target[labelAttrItems[i].Text].Val != null)
									target[labelAttrItems[i].Text].Val.RefCount--;
								target[labelAttrItems[i].Text] = attr;
								if (attr.Val != null)
									attr.Val.RefCount++;
							}
							catch (Exception e)
							{
								labelTip.Text = e.Message;
							}
						}

					}
					else
						break;
				}

				/*				foreach(object o in array){
									target = (RcChipBase)o;
									if(cmbColor.Text != "")
										target.ChipColor.Read(cmbColor.Text);
									target.UpdateMatrix();
								}
				*/
				rcdata.model.root.UpdateMatrix();
				rcdata.CalcWeightCenter();

				pictTarget_Paint(this, null);
				Modified = true;
				//if (treeview != null && !treeview.IsDisposed) treeview.GenerateTree();
				if (treeview != null && !treeview.IsDisposed) {
					foreach (var c in rcdata.SelectedChipList)
						treeview.UpdateTree(c);
				}

			}
			else {
				ChipBase target = rcdata.SelectedChip;
				ChipAttribute attr;
				for (int i = 0; i < labelAttrItems.Length; i++) {
					/*if (cmbAttrItems[i].Text == "")
					{
						continue;
					}
					else */if (labelAttrItems[i].Visible)
					{
						try
						{
							attr = target[labelAttrItems[i].Text];
							attr.SetValue(cmbAttrItems[i].Text, rcdata.vals);
							if (target[labelAttrItems[i].Text].Val != null)
								target[labelAttrItems[i].Text].Val.RefCount--;
							target[labelAttrItems[i].Text] = attr;
							if (attr.Val != null)
								attr.Val.RefCount++;
						}
						catch (Exception e)
						{
							labelTip.Text = e.Message;
						}

					}
					else
						break;
				}
				target.Name = txtName.Text;
				//				target.ChipColor.Read(cmbColor.Text);

				LoadChipInfo();
				target.UpdateMatrix();
				rcdata.Cursor.UpdateMatrix();
				rcdata.CalcWeightCenter();

				ProcessViewPoint(rcdata.Cursor.Matrix);
				pictTarget_Paint(this, null);

				Modified = true;
				//if (treeview != null && !treeview.IsDisposed) treeview.GenerateTree();
				if (treeview != null && !treeview.IsDisposed) treeview.UpdateTree(rcdata.SelectedChip);
			}
		}

		private void UpdateCameraPosition(int dX, int dY, int dZ, Matrix viewPointMatrix) {
			tmrScroll.Enabled = false;
			CamPhi += dX / 128f;
			CamTheta += dY / 128f;
			CamDepth += dZ * 0.5f;

			if (CamTheta >= Math.PI / 2) CamTheta = (float)(Math.PI / 2) - float.Epsilon;
			else if (CamTheta <= -Math.PI / 2) CamTheta = (float)(-Math.PI / 2) + float.Epsilon;
			CamPhi %= (float)(Math.PI * 2);

			Vector3 V = new Vector3(0f, 0f, CamDepth);
			Vector3 W = new Vector3(0f, 1f, 0f);
			Vector3 P = new Vector3(0f, 0f, 0f);
			P.TransformCoordinate(viewPointMatrix);

			V.TransformCoordinate(Matrix.RotationX(CamTheta) * Matrix.RotationY(CamPhi));
			V += P;
			W.TransformCoordinate(Matrix.RotationX(CamTheta) * Matrix.RotationY(CamPhi));

			device.Transform.View = Matrix.LookAtLH(
				V,
				P,
				W
				);

			CamNow = P;
			ScrollCount = editOption.ScrollFrameNum;
			if (CameraOrtho)
				device.Transform.Projection = Matrix.OrthoLH(CamDepth / 2f, CamDepth / 2f, -256f, 256f);
			pictTarget_Paint(this, null);
		}
		private void UpdateCameraPosition(int dX, int dY, int dZ, Vector3 viewPointVector) {
			CamPhi += dX / 128f;
			CamTheta += dY / 128f;
			CamDepth += dZ * 0.5f;

			if (CamTheta >= Math.PI / 2) CamTheta = (float)(Math.PI / 2) - float.Epsilon;
			else if (CamTheta <= -Math.PI / 2) CamTheta = (float)(-Math.PI / 2) + float.Epsilon;
			CamPhi %= (float)(Math.PI * 2);

			Vector3 V = new Vector3(0f, 0f, CamDepth);
			Vector3 W = new Vector3(0f, 1f, 0f);

			V.TransformCoordinate(Matrix.RotationX(CamTheta) * Matrix.RotationY(CamPhi));
			V += viewPointVector;
			W.TransformCoordinate(Matrix.RotationX(CamTheta) * Matrix.RotationY(CamPhi));

			device.Transform.View = Matrix.LookAtLH(
				V,
				viewPointVector,
				W
				);

			if (CameraOrtho)
				device.Transform.Projection = Matrix.OrthoLH(CamDepth / 2f, CamDepth / 2f, -256f, 256f);
			pictTarget_Paint(this, null);
		}

		public void ProcessViewPoint(Matrix nextPointMatrix) {
			if (!drawOption.AutoCamera)
				pictTarget_Paint(this, null);
			else
				StartScrollCameraPosition(nextPointMatrix);
		}

		private void StartScrollCameraPosition(Matrix nextPointMatrix) {
			CamNow = CamNext * (ScrollCount / (float)editOption.ScrollFrameNum) + CamNow * (1f - ScrollCount / (float)editOption.ScrollFrameNum);
			CamNext = new Vector3();
			CamNext.TransformCoordinate(nextPointMatrix);
			ScrollCount = 0;
			tmrScroll.Enabled = true;
		}

		private void tmrScroll_Tick(object sender, System.EventArgs e) {
			if (ScrollCount < editOption.ScrollFrameNum) {
				UpdateCameraPosition(0, 0, 0, CamNext * (ScrollCount / (float)editOption.ScrollFrameNum) + CamNow * (1f - ScrollCount / (float)editOption.ScrollFrameNum));
				ScrollCount++;
				//				labelTip.Text = ScrollCount.ToString();
			}
			else {
				CamNow = CamNext;
				UpdateCameraPosition(0, 0, 0, CamNext);
				tmrScroll.Enabled = false;
			}
		}

		private void MainForm_Resize(object sender, System.EventArgs e) {
			if (this.WindowState != FormWindowState.Minimized)
				pictTarget.Dock = DockStyle.Fill;
			//			Pause = (this.WindowState == FormWindowState.Minimized) || (pictTarget.ClientSize.Width <= 0 || pictTarget.ClientSize.Height <= 0);
			if (Initialized && !Pause) {

				//				device.Reset(presentParams);
				//				UpdateCameraPosition(0,0,0,CamNext);
				ResetGraphics();
			}
		}

		private void ReleaseGraphics() {
			vbGuide.Dispose();
			vbWeight.Dispose();
		}

		private void ResetGraphics() {
			device.RenderState.ZBufferEnable = true;
			/*device.Lights[0].Ambient = Color.Gray;
			device.Lights[0].Diffuse = Color.White;
			device.Lights[0].Specular = Color.White;
			device.Lights[0].Type = LightType.Directional;
			device.Lights[0].Direction = new Vector3(1f, -1f, 1f);
			device.Lights[0].Enabled = true;
			device.Lights[0].Update();

			device.Lights[1].Ambient = Color.White;
			device.Lights[1].Diffuse = Color.White;
			device.Lights[1].Type = LightType.Directional;
			device.Lights[1].Direction = new Vector3(0, 1, 0);
			device.Lights[1].Enabled = false;
			device.Lights[1].Update();*/

			//device.RenderState.Lighting = true;
						device.RenderState.Ambient = Color.White;
			device.RenderState.CullMode = Cull.CounterClockwise;

			//				device.RenderState.AlphaBlendOperation = BlendOperation.Add;
			device.RenderState.SourceBlend = Blend.SourceAlpha;
			device.RenderState.DestinationBlend = Blend.InvSourceAlpha;
			device.RenderState.AlphaBlendEnable = true;
			//device.RenderState.SpecularEnable = true;
			device.RenderState.AntiAliasedLineEnable = true;

			device.Transform.Projection = Matrix.PerspectiveFovLH(0.5f, (float)pictTarget.ClientSize.Width / (float)pictTarget.ClientSize.Height, 0.5f, 100.0f);
			// pictTarget_Paint(this,null);

			vbGuide = new VertexBuffer(
				typeof(CustomVertex.PositionOnly),
				9,
				device,
				0,
				CustomVertex.PositionOnly.Format,
				Pool.Default);
			GraphicsStream gs;
			gs = vbGuide.Lock(0, 0, 0);
			lineCV = new Microsoft.DirectX.Direct3D.CustomVertex.PositionOnly[9];	//	PositionColored

			lineCV[0].Position = new Vector3(100f, 0f, 0f);
			lineCV[1].Position = new Vector3(0f, 0f, 0f);
			lineCV[2].Position = new Vector3(-100f, 0f, 0f);
			lineCV[3].Position = new Vector3(0f, 100f, 0f);
			lineCV[4].Position = new Vector3(0f, 0f, 0f);
			lineCV[5].Position = new Vector3(0f, -100f, 0f);
			lineCV[6].Position = new Vector3(0f, 0f, 100f);
			lineCV[7].Position = new Vector3(0f, 0f, 0f);
			lineCV[8].Position = new Vector3(0f, 0f, -100f);

			gs.Write(lineCV);
			vbGuide.Unlock();

			vbWeight = new VertexBuffer(
				typeof(CustomVertex.PositionOnly),
				6,
				device,
				0,
				CustomVertex.PositionOnly.Format,
				Pool.Default);

			gs = vbWeight.Lock(0, 0, 0);

			lineCV = new CustomVertex.PositionOnly[6];

			lineCV[0].Position = new Vector3(100f, 0f, 0f);
			lineCV[1].Position = new Vector3(-100f, 0f, 0f);
			lineCV[2].Position = new Vector3(0f, 100f, 0f);
			lineCV[3].Position = new Vector3(0f, -100f, 0f);
			lineCV[4].Position = new Vector3(0f, 0f, 100f);
			lineCV[5].Position = new Vector3(0f, 0f, -100f);

			gs.Write(lineCV);
			vbWeight.Unlock();


			UpdateCameraPosition(0, 0, 0, CamNext);
		}

		///<summery>
		///子チップリストボックスで、何かが選択されたときのイベント。他のリストボックスの選択を解除する動作。
		///</summery>
		private void lstChild_SelectedIndexChanged(object sender, System.EventArgs e) {
			ListBox targetlist = (ListBox)sender;
			if (targetlist.SelectedIndex != -1) {
				if (targetlist == lstNorth) {
					lstEast.SelectedIndex = -1;
					lstSouth.SelectedIndex = -1;
					lstWest.SelectedIndex = -1;
				}
				else if (targetlist == lstEast) {
					lstNorth.SelectedIndex = -1;
					lstSouth.SelectedIndex = -1;
					lstWest.SelectedIndex = -1;
				}
				else if (targetlist == lstSouth) {
					lstNorth.SelectedIndex = -1;
					lstEast.SelectedIndex = -1;
					lstWest.SelectedIndex = -1;
				}
				else if (targetlist == lstWest) {
					lstNorth.SelectedIndex = -1;
					lstEast.SelectedIndex = -1;
					lstSouth.SelectedIndex = -1;
				}
			}
		}

		private void lstChild_DoubleClick(object sender, System.EventArgs e) {
			ListBox listtarget = (ListBox)sender;
			if (listtarget.SelectedIndex != -1) {
				rcdata.SelectedChip = (ChipBase)listtarget.SelectedItem;
				labelTip.Text = "Moved the cursor: " + rcdata.SelectedChip.ToString() +
					((rcdata.SelectedChip.Comment != null && rcdata.SelectedChip.Comment != "") ?
					" (" + rcdata.SelectedChip.Comment + ")" : "");
			}

		}

		private void btnRootChip_Click(object sender, System.EventArgs e) {
			if (rcdata.SelectedChip.Parent != null) {
				rcdata.SelectedChip = rcdata.SelectedChip.Parent;
				labelTip.Text = "Go to Parent: " + rcdata.SelectedChip.ToString() +
					((rcdata.SelectedChip.Comment != null && rcdata.SelectedChip.Comment != "") ?
					" (" + rcdata.SelectedChip.Comment + ")" : "");
			}
		}

		private void btnVal_Click(object sender, System.EventArgs e) {
			ValsForm valform = new ValsForm(rcdata.vals);
			Pause = true;
			Modified = (valform.ShowDialog() == DialogResult.Yes);
			Pause = false;
			SetValDropList();
			rcdata.model.root.UpdateMatrix();
			rcdata.Cursor.UpdateMatrix();
			LoadChipInfo();

		}

		private void SetValDropList() {
			foreach (ComboBox acb in cmbAttrItems) {
				acb.Items.Clear();
				foreach (ValEntry v in rcdata.vals.List) {
					acb.Items.Add(v.ValName);
					acb.Items.Add("-" + v.ValName);
				}
				if (acb.Items.Count == 0)
					acb.Items.Add("(使用可能な変数はありません)");
			}
		}

		private void buttonSelChip_Click(object sender, System.EventArgs e) {
			ctmChipType.Show((Control)sender, new Point(((Control)sender).Width / 2, ((Control)sender).Height / 2));
		}

		private void SetUndo(UndoType type, params ChipBase[] chips) {
			undo.chips = chips;
			undo.type = type;
		}

		private void Undo() {
			switch (undo.type) {
				case UndoType.Added:
					#region Added処理
					#endregion
					break;
				case UndoType.Removed:
					#region Removed処理
					#endregion
					break;
				case UndoType.Modified:
					#region Modified処理
					#endregion
					break;
				default:
					break;
			}
		}

		private void menuItem2_Click(object sender, System.EventArgs e) {
		}

		private void pictAngle_Paint(object sender, System.Windows.Forms.PaintEventArgs e) {
			bool selfget = false;
			Graphics grp;

			if (e == null) {
				grp = pictAngle.CreateGraphics();
				selfget = true;
			}
			else
				grp = e.Graphics;

			Rectangle target = pictAngle.ClientRectangle;

			//labelTip.Text = target.ToString();

			bool hasAngle = rcdata.SelectedChip?.AttrNameList.Contains("Angle") ?? false;
			if (!hasAngle) {
				grp.FillRectangle(Brushes.Gray, target);
				if (selfget) grp.Dispose();
				return;
			}
			ChipAttribute attr;
			attr = rcdata.SelectedChip["Angle"];
			grp.FillRectangle(Brushes.Navy, target);

			Pen p = new Pen(Color.Red, 1f);
			p.EndCap = System.Drawing.Drawing2D.LineCap.DiamondAnchor;
			SolidBrush gp = new SolidBrush(drawOption.CursorFrontColor);
			SolidBrush gn = new SolidBrush(drawOption.CursorBackColor);

			grp.FillRectangle(gp, target.Width / 2, target.Height / 2, target.Width / 2, target.Height / 2);
			grp.FillRectangle(gn, target.Width / 2, 0, target.Width / 2, target.Height / 2);
			grp.FillEllipse(Brushes.Navy, target.X, target.Y, target.Width - 1, target.Height - 1);


			grp.DrawEllipse(Pens.White, target.X, target.Y, target.Width - 1, target.Height - 1);
			grp.DrawLine(Pens.White, new Point(0, target.Height / 2), new Point(target.Width - 1, target.Height / 2));
			grp.DrawLine(Pens.White, new Point(target.Width / 2, 0), new Point(target.Width / 2, target.Height - 1));

			System.Drawing.Font f = new System.Drawing.Font("ＭＳ ゴシック", 8f);
			if (attr.Val != null) {
				p.Color = Color.Yellow;
				grp.DrawPie(p,
					target,
					attr.IsNegative ? -attr.Val.Min.Value : attr.Val.Min.Value,
					attr.IsNegative ? attr.Val.Min - attr.Val.Max : attr.Val.Max - attr.Val.Min
					);
				p.Color = Color.LightGreen;
			}
			else if (editOption.AngleViewGrid > 0)
				grp.DrawString(editOption.AngleViewGrid.ToString(), f, Brushes.Yellow, 0f, 0f);


			grp.DrawLine(p,
				new Point(target.Width / 2, target.Height / 2),
				new Point(
				(int)Math.Ceiling(target.Width / 2 * (1 + Math.Cos(attr.Value * DegToRad))),
				(int)Math.Ceiling(target.Height / 2 * (1 + Math.Sin(attr.Value * DegToRad)))
				)
				);



			if (selfget)
				grp.Dispose();

			p.Dispose();
		}

		private void pictAngle_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e) {
			if (e.Button == MouseButtons.Left) {
				angledrag = true;
				pictAngle_MouseMove(sender, e);
			}
		}

		private void pictAngle_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e) {
			if (e.Button == MouseButtons.Left) {
				angledrag = false;
			}
		}

		private void pictAngle_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e) {
			if (rcdata.SelectedChipCount < 2)
            {
				if (!angledrag) return;
				ChipAttribute buff;

				if (!Array.Exists(rcdata.SelectedChip.AttrNameList, x => x.Equals("Angle", StringComparison.CurrentCultureIgnoreCase)))
					return;

				buff = rcdata.SelectedChip["Angle"];

				if (buff.Val == null)
				{
					buff.Const = (float)(Math.Atan2(e.Y - pictAngle.ClientRectangle.Height / 2, e.X - pictAngle.ClientRectangle.Width / 2) * RadToDeg);
					if (editOption.AngleViewGrid > 0)
					{
						buff.Const = ((int)(buff.Const / editOption.AngleViewGrid + 180.5f) - 180) * editOption.AngleViewGrid;
					}
					if (buff.Const == -180f) buff.Const = 180f;
					rcdata.SelectedChip["Angle"] = buff;
					rcdata.SelectedChip.UpdateMatrix();
					rcdata.CalcWeightCenter();
					LoadChipInfo();
					ProcessViewPoint(rcdata.Cursor.Matrix);
					Modified = true;
					//if (treeview != null && !treeview.IsDisposed) treeview.GenerateTree();
					if (treeview != null && !treeview.IsDisposed) treeview.UpdateTree(rcdata.SelectedChip);
				}

			} else
            {
				return;
            }

		}

		private void miRotateRight_Click(object sender, System.EventArgs e) {
			rcdata.SelectedChip.RotateRight();
			rcdata.SelectedChip.UpdateMatrix();
			LoadChipInfo();
			if (treeview != null && !treeview.IsDisposed) treeview.UpdateTree(rcdata.SelectedChip);
		}

		private void miRotateLeft_Click(object sender, System.EventArgs e) {
			rcdata.SelectedChip.RotateLeft();
			rcdata.SelectedChip.UpdateMatrix();
			LoadChipInfo();
			if (treeview != null && !treeview.IsDisposed) treeview.UpdateTree(rcdata.SelectedChip);
		}

		private void miReverseX_Click(object sender, System.EventArgs e) {
			rcdata.SelectedChip.ReverseX();
			rcdata.SelectedChip.UpdateMatrix();
			LoadChipInfo();
			if (treeview != null && !treeview.IsDisposed) treeview.UpdateTree(rcdata.SelectedChip);
		}

		private void miReverseY_Click(object sender, System.EventArgs e) {
			rcdata.SelectedChip.ReverseY();
			rcdata.SelectedChip.UpdateMatrix();
			LoadChipInfo();
			if (treeview != null && !treeview.IsDisposed) treeview.UpdateTree(rcdata.SelectedChip);
		}

		private void miReverseZ_Click(object sender, System.EventArgs e) {
			rcdata.SelectedChip.ReverseZ();
			rcdata.SelectedChip.UpdateMatrix();
			LoadChipInfo();
			if (treeview != null && !treeview.IsDisposed) treeview.UpdateTree(rcdata.SelectedChip);
		}

		private void miCut_Click(object sender, System.EventArgs e) {
			actionCut();
		}

		private void miCopy_Click(object sender, System.EventArgs e) {
			actionCopy();
		}

		private void miDelete_Click(object sender, System.EventArgs e) {
			actionDelete();
		}

		private void ctmChipType_Popup(object sender, System.EventArgs e) {
			foreach (MenuItem m in miChange.MenuItems)
				m.Enabled = true;
			miChange.Enabled = true;

			switch (ChipBase.CheckType(rcdata.SelectedChip)) {
				case ChipType.Core: //	コア
					miChange.Enabled = false;
					miCut.Enabled = false;
					miCopy.Enabled = false;
					miDelete.Enabled = false;
					break;
				case ChipType.Chip: //	チップ
					miChangeChip.Enabled = false;
					goto default;
				case ChipType.Frame:    //	フレーム
					miChangeFrame.Enabled = false;
					goto default;
				case ChipType.Rudder:   //	ラダー
					miChangeRudder.Enabled = false;
					goto default;
				case ChipType.RudderF:  //	ラダーフレーム
					miChangeRudderF.Enabled = false;
					goto default;
				case ChipType.Trim: //	トリム
					miChangeTrim.Enabled = false;
					goto default;
				case ChipType.TrimF:    //	トリムフレーム
					miChangeTrimF.Enabled = false;
					goto default;
				case ChipType.Wheel: //	ホイール
					miChangeWheel.Enabled = false;
					goto default;
				case ChipType.RLW:  //	無反動ホイール
					miChangeRLW.Enabled = false;
					goto default;
				case ChipType.Jet:  //	ジェット
					miChangeJet.Enabled = false;
					goto default;
				case ChipType.Weight:   //	ウェイト
					miChangeWeight.Enabled = false;
					goto default;
				case ChipType.Cowl: //	カウル
					miChangeCowl.Enabled = false;
					goto default;
				case ChipType.Arm:  //	アーム
					miChangeArm.Enabled = false;
					goto default;
				default:
					miCut.Enabled = true;
					miCopy.Enabled = true;
					miDelete.Enabled = true;
					break;
			}

		}

		private void btnKey_Click(object sender, System.EventArgs e) {
			Pause = true;
			KeysForm keyform = new KeysForm(rcdata);
			Modified = (keyform.ShowDialog() == DialogResult.Yes);
			Pause = false;
			SetValDropList();
			rcdata.model.root.UpdateMatrix();
			rcdata.Cursor.UpdateMatrix();
			LoadChipInfo();
		}

		private void cmbAttrItems_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e) {
			if (e.KeyChar == 13) {
				e.Handled = true;
				string valname = ((ComboBox)sender).Text;
				if (valname.Length > 0 && !RigidChips.Environment.TryParseNumber(valname)) {
					if (valname[0] == '-') {
						valname = valname.Substring(1);
					}
					if (Array.Find(rcdata.vals.List, x => x.ValName == valname) == null) {
						if (MessageBox.Show(string.Format("New Val : {0} Do you want to create?", valname), "RCM", MessageBoxButtons.YesNo) != DialogResult.Yes)
							return;
						ValsForm valform = new ValsForm(rcdata.vals, valname);
						Pause = true;
						bool result = (valform.ShowDialog() == DialogResult.Yes);
						Pause = false;
						if (!result || Array.Find(rcdata.vals.List, x => x.ValName == valname) == null) {
							LoadChipInfo();
							return;
						}
						SetValDropList();
						rcdata.model.root.UpdateMatrix();
						rcdata.Cursor.UpdateMatrix();
					}
				}
				ApplyChipInfo();
				labelTip.Text = "changed the attributes.";
			}
			/*			else if(!char.IsDigit(e.KeyChar) && e.KeyChar != '.' && e.KeyChar != '-' && !char.IsControl(e.KeyChar)){
							e.Handled = true;
						}
			*/
			//			labelTip.Text = e.KeyChar.ToString() + "(" + (int)e.KeyChar + ")";


		}

		private void cmbAttrItems_TextChanged(object sender, System.EventArgs e) {
			if (sender is ComboBox) {
				ComboBox attrcombo = (ComboBox)sender;
				if (attrcombo.Text == "(使用可能な変数はありません)")
					LoadChipInfo();
			}
		}

		private void txtName_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e) {
			if (e.KeyChar == 13) {
				ApplyChipInfo();
				e.Handled = true;
			}
			else if (!char.IsLetter(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '_' && !char.IsControl(e.KeyChar))
				e.Handled = true;
			else if (char.IsDigit(e.KeyChar) && txtName.Text.Length < 1)
				e.Handled = true;

		}

		private void cmbColor_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e) {
			if ('0' <= e.KeyChar && e.KeyChar <= '9' ||
				'A' <= e.KeyChar && e.KeyChar <= 'F' ||
				'a' <= e.KeyChar && e.KeyChar <= 'f' ||
				e.KeyChar == '#') {
				return;
			}
			if (char.IsControl(e.KeyChar)) {
				if (e.KeyChar == 13) {
					ApplyChipInfo();
					e.Handled = true;
				}
				return;
			}
			e.Handled = true;

		}

		private void pictPalette_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e) {
			if (e.Button != MouseButtons.Left) return;
			palettedrag = true;
			pictPalette_MouseMove(sender, e);
		}

		private void pictPalette_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e) {
			if (!palettedrag) return;
			Graphics g = pictPalette.CreateGraphics();
			IntPtr hdc = g.GetHdc();
			int i = GetPixel(hdc, e.X, e.Y);

			g.ReleaseHdc(hdc);
			g.Dispose();

			if (rcdata.SelectedChipCount > 0) {
				foreach (object c in rcdata.SelectedChipList)
					((ChipBase)c)["Color"] = new ChipAttribute((i & 0xFF) << 16 | (i & 0xFF00) | (i & 0xFF0000) >> 16);
			}
			else {
				rcdata.SelectedChip["Color"] = new ChipAttribute((i & 0xFF) << 16 | (i & 0xFF00) | (i & 0xFF0000) >> 16);
			}
			//	バイトオーダーが逆向きなのにはワラタ
			LoadChipInfo();
			Modified = true;
			if (treeview != null && !treeview.IsDisposed) treeview.UpdateTree(rcdata.SelectedChip);
		}

		private void pictPalette_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e) {
			palettedrag = false;
		}

		private void pictAngle_Click(object sender, System.EventArgs e) {

		}

		private NameValueCollection GenerateOptList() {
			NameValueCollection c = new NameValueCollection();

			c.Add("CamPhi", CamPhi.ToString());
			c.Add("CamTheta", CamTheta.ToString());
			c.Add("CamDepth", CamDepth.ToString());
			//c.Add("Selected", rcdata.SelectedChip == null ? "-1" : rcdata.SelectedChip.RegistID.ToString());

			return c;
		}

		private void ApplyOptList(NameValueCollection opts) {
			foreach (string s in opts.Keys) {
				switch (s) {
					case "CamPhi":
						CamPhi = float.Parse(opts[s], CultureInfo.InvariantCulture);
						break;
					case "CamTheta":
						CamTheta = float.Parse(opts[s], CultureInfo.InvariantCulture);
						break;
					case "CamDepth":
						CamDepth = float.Parse(opts[s], CultureInfo.InvariantCulture);
						break;
					//case "Selected":
					//	rcdata.SelectedChip = rcdata.GetChipFromLib((int)Math.Max(float.Parse(opts[s]), 0));
					//	break;
				}
			}
		}

		private void miFileOpen_Click(object sender, System.EventArgs e) {
			/*			StreamReader test = new StreamReader(@"mmi.txt");
						string buff = test.ReadToEnd();

						rcdata.Parse(buff);
			*/
			if (!Modified || MessageBox.Show("Unsaved changes will be lost.", "Open", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK) {
				if (scriptform != null) {
					scriptform.Hide();
					scriptform.Close();
					if (!scriptform.Disposing) scriptform.Dispose();
					scriptform = null;
				}
				dlgOpen.Filter = "Model File(*.rcm;*.rcd;*.txt)|*.rcm;*.rcd;*.txt";
				dlgOpen.Multiselect = false;
				dlgOpen.ShowReadOnly = false;
				bool buffer = editOption.ConvertParentAttributes;
				editOption.ConvertParentAttributes = false;
				if (dlgOpen.ShowDialog() == DialogResult.OK) {

					Pause = true;

					switch (Path.GetExtension(dlgOpen.FileName).ToLower()) {
						case ".rcm":
							//rcdata.UnregisterChipAll(rcdata.model.root);
							rcdata.keys = new KeyEntryList(RigidChips.Environment.KeyCount);
							rcdata.vals = new ValEntryList();
							rcdata.model = new Model(rcdata);

							rcdata.headercomment = rcdata.script = "";
							rcdata.SelectedChip = rcdata.model.root;
							//rcdata.RegisterChip(rcdata.model.root);

							NameValueCollection opts = rcdata.Load(dlgOpen.FileName);
							ApplyOptList(opts);

							break;
						case ".rcd":
						case ".txt":
						default:
							//rcdata.UnregisterChipAll(rcdata.model.root);
							rcdata.keys = new KeyEntryList(RigidChips.Environment.KeyCount);
							rcdata.vals = new ValEntryList();
							rcdata.model = new Model(rcdata);


							rcdata.headercomment = rcdata.script = "";
							rcdata.SelectedChip = rcdata.model.root;
							//rcdata.RegisterChip(rcdata.model.root);


							StreamReader file = new StreamReader(dlgOpen.FileName, System.Text.Encoding.Default);
							rcdata.Parse(file.ReadToEnd());
							file.Close();

							break;
					}
					GC.Collect();
					Modified = false;
					if (treeview != null && !treeview.IsDisposed) treeview.GenerateTree();
					EditingFileName = dlgOpen.FileName;
					SetValDropList();
					Pause = false;
					labelTip.Text = "Opened File: " + System.IO.Path.GetFileNameWithoutExtension(EditingFileName);
				}
				editOption.ConvertParentAttributes = buffer;
			}

		}

		private void panelB_Paint(object sender, System.Windows.Forms.PaintEventArgs e) {

		}

		private void miPaletteAllPaint_Click(object sender, System.EventArgs e) {
			if (rcdata.SelectedChipCount < 2)
            {
				rcdata.model.ForEach(chip => chip.ChipColor = rcdata.SelectedChip.ChipColor);
				Modified = true;
				if (treeview != null && !treeview.IsDisposed) treeview.UpdateTree(rcdata.SelectedChip);
				labelTip.Text = "Changed color.";
			}
		}

		private void miPaletteChildPaint_Click(object sender, System.EventArgs e) {
			System.Collections.Queue paintqueue = new System.Collections.Queue();
			ChipBase buff;

			if (rcdata.SelectedChipCount == 0)
				paintqueue.Enqueue(rcdata.SelectedChip);
			else
				foreach (ChipBase selected in rcdata.SelectedChipList) paintqueue.Enqueue(selected);

			while (paintqueue.Count > 0) {
				buff = (ChipBase)paintqueue.Dequeue();
				foreach (ChipBase c in buff.Children) {
					if (c == null) continue;
					c.ChipColor = buff.ChipColor;
					paintqueue.Enqueue(c);
				}
			}
			Modified = true;
			if (treeview != null && !treeview.IsDisposed) treeview.UpdateTree(rcdata.SelectedChip);
			labelTip.Text = "Changed color.";
		}

		private void miFileSave_Click(object sender, System.EventArgs e) {
			if (EditingFileName == "") {
				miFileSaveAs_Click(sender, e);
				return;
			}
			SaveFile(editingFileName);
		}

		/*		private void miAngle_Click(object sender, System.EventArgs e) {
					RcAttrValue buff;
					try{
						buff = rcdata.SelectedChip["Angle"];
						if(buff.Val != null)throw new Exception();
					}catch{
						return;
					}
					buff.Const = float.Parse(((MenuItem)sender).Text);
					rcdata.SelectedChip["Angle"] = buff;
					rcdata.SelectedChip.UpdateMatrix();
					rcdata.CalcWeightCenter();
					LoadChipInfo();
					ProcessViewPoint(rcdata.Cursor.Matrix);
					Modified = true;
					if(treeview != null && !treeview.IsDisposed)treeview.GenerateTree();
				}
		*/
		private void ctmAngles_Popup(object sender, System.EventArgs e) {
			ContextMenu menu = (ContextMenu)sender;

		}

		private void miFileNew_Click(object sender, System.EventArgs e) {
			if (!Modified || MessageBox.Show("Unsaved changes will be lost.", "New", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK) {
				//rcdata.UnregisterChipAll(rcdata.model.root);
				rcdata.keys = new KeyEntryList(RigidChips.Environment.KeyCount);
				rcdata.vals = new ValEntryList();
				rcdata.model = new Model(rcdata);
				rcdata.headercomment = rcdata.script = "";
				rcdata.SelectedChip = rcdata.model.root;
				//rcdata.RegisterChip(rcdata.model.root);

				GC.Collect();
				EditingFileName = "";
				Modified = false;
				if (treeview != null && !treeview.IsDisposed) treeview.GenerateTree();
				labelTip.Text = "discarded the information and started a new model data.";
			}
		}

#if false 
		private void ctmChildList_Popup(object sender, System.EventArgs e) {
			ListBox list = (ListBox)((ContextMenu)sender).SourceControl;

			miListCopy.Enabled = miListCut.Enabled = miListSelect.Enabled = ( list.SelectedIndex >= 0);
			miListAdd.Enabled = (selected.Text != "選択") && (selected.Text != "視点") ;
			switch(list.Name){
				case "lstNorth":
					jointPositionBuffer = RcJointPosition.North;
					break;
				case "lstSouth":
					jointPositionBuffer = RcJointPosition.South;
					break;
				case "lstEast":
					jointPositionBuffer = RcJointPosition.East;
					break;
				case "lstWest":
					jointPositionBuffer = RcJointPosition.West;
					break;
				default:
					jointPositionBuffer = RcJointPosition.NULL;
					break;
			}

		}
		private void miListAdd_Click(object sender, System.EventArgs e) {


			switch(selected.Text){
				case "貼り付け":
					try{
						RcChipBase buffer = clipboard.Clone(true,null);
						buffer.Attach(rcdata.SelectedChip,jointPositionBuffer);
						rcdata.SelectedChip = buffer;
						labelTip.Text = "貼り付けを行いました。";
					}
					catch(Exception err){
						MessageBox.Show(err.Message,"追加エラー");
						return;
					}
					
					rcdata.CheckBackTrack();
					break;
				case "チップ":
					try{
						rcdata.SelectedChip = new RcChipChip(rcdata,rcdata.SelectedChip,jointPositionBuffer);
						labelTip.Text = selected.Text + "を追加しました。";
					}
					catch(Exception err){
						MessageBox.Show(err.Message,"追加エラー");
						return;
					}
					
					break;
				case "フレーム":
					try{
						rcdata.SelectedChip = new RcChipFrame(rcdata,rcdata.SelectedChip,jointPositionBuffer);
						labelTip.Text = selected.Text + "を追加しました。";
					}
					catch(Exception err){
						MessageBox.Show(err.Message,"追加エラー");
						return;
					}
					

					break;
				case "ウェイト":
					try{
						rcdata.SelectedChip = new RcChipWeight(rcdata,rcdata.SelectedChip,jointPositionBuffer);
						labelTip.Text = selected.Text + "を追加しました。";
					}
					catch(Exception err){
						MessageBox.Show(err.Message,"追加エラー");
						return;
					}
					

					break;
				case "カウル":
					try{
						rcdata.SelectedChip = new RcChipCowl(rcdata,rcdata.SelectedChip,jointPositionBuffer);
						labelTip.Text = selected.Text + "を追加しました。";
					}
					catch(Exception err){
						MessageBox.Show(err.Message,"追加エラー");
						return;
					}
					
					break;
				case "ラダー":
					try{
						rcdata.SelectedChip = new RcChipRudder(rcdata,rcdata.SelectedChip,jointPositionBuffer);
						labelTip.Text = selected.Text + "を追加しました。";
					}
					catch(Exception err){
						MessageBox.Show(err.Message,"追加エラー");
						return;
					}
					

					break;
				case "ラダーフレーム":
					try{
						rcdata.SelectedChip = new RcChipRudderF(rcdata,rcdata.SelectedChip,jointPositionBuffer);
						labelTip.Text = selected.Text + "を追加しました。";
					}
					catch(Exception err){
						MessageBox.Show(err.Message,"追加エラー");
						return;
					}
					

					break;
				case "トリム":
					try{
						rcdata.SelectedChip = new RcChipTrim(rcdata,rcdata.SelectedChip,jointPositionBuffer);
						labelTip.Text = selected.Text + "を追加しました。";
					}
					catch(Exception err){
						MessageBox.Show(err.Message,"追加エラー");
						return;
					}
					

					break;
				case "トリムフレーム":
					try{
						rcdata.SelectedChip = new RcChipTrimF(rcdata,rcdata.SelectedChip,jointPositionBuffer);
						labelTip.Text = selected.Text + "を追加しました。";
					}
					catch(Exception err){
						MessageBox.Show(err.Message,"追加エラー");
						return;
					}
					

					break;
				case "ホイール":
					try{
						rcdata.SelectedChip = new RcChipWheel(rcdata,rcdata.SelectedChip,jointPositionBuffer);
						labelTip.Text = selected.Text + "を追加しました。";
					}
					catch(Exception err){
						MessageBox.Show(err.Message,"追加エラー");
						return;
					}
					
					break;
				case "無反動ホイール":
					try{
						rcdata.SelectedChip = new RcChipRLW(rcdata,rcdata.SelectedChip,jointPositionBuffer);
						labelTip.Text = selected.Text + "を追加しました。";
					}
					catch(Exception err){
						MessageBox.Show(err.Message,"追加エラー");
						return;
					}
					

					break;
				case "ジェット":
					try{
						rcdata.SelectedChip = new RcChipJet(rcdata,rcdata.SelectedChip,jointPositionBuffer);
						labelTip.Text = selected.Text + "を追加しました。";
					}
					catch(Exception err){
						MessageBox.Show(err.Message,"追加エラー");
						return;
					}
					
					break;
				case "アーム":
					try{
						rcdata.SelectedChip = new RcChipArm(rcdata,rcdata.SelectedChip,jointPositionBuffer);
						labelTip.Text = selected.Text + "を追加しました。";
					}
					catch(Exception err){
						MessageBox.Show(err.Message,"追加エラー");
						return;
					}
					

					break;
				default:
					break;
			}

			//			UpdateCameraPosition(0,0,0,rcdata.Cursor.Matrix);
			rcdata.CheckBackTrack();
			rcdata.CalcWeightCenter();
			pictTarget_Paint(this,null);

			Modified = true;
			if(treeview != null && !treeview.IsDisposed)treeview.GenerateTree();
		}
#else
		private void ctmChildList_Popup(object sender, System.EventArgs e) {
			ListBox list = (ListBox)((ContextMenu)sender).SourceControl;

			miListDelete.Enabled = miListCopy.Enabled = miListCut.Enabled = miListSelect.Enabled = (list.SelectedIndex >= 0);
			miListAdd.Enabled = (selectedButton.Text != "Selection") && (selectedButton.Text != "Perspective");
			switch (list.Name) {
				case "lstNorth":
					jointPositionBuffer = JointPosition.North;
					break;
				case "lstSouth":
					jointPositionBuffer = JointPosition.South;
					break;
				case "lstEast":
					jointPositionBuffer = JointPosition.East;
					break;
				case "lstWest":
					jointPositionBuffer = JointPosition.West;
					break;
				default:
					jointPositionBuffer = JointPosition.NULL;
					break;
			}

		}
		private void miListAdd_Click(object sender, System.EventArgs e) {


			switch (selectedButton.Text) {
				case "Paste":
					try {
						ChipBase buffer = clipboard.Clone(true, null);
						buffer.Attach(rcdata.SelectedChip, jointPositionBuffer);
						labelTip.Text = "Pasted.";
					}
					catch (Exception err) {
						MessageBox.Show(err.Message, "Adding failed");
						return;
					}

					//rcdata.CheckBackTrack();
					break;
				case "Chip":
					try {
						new NormalChip(rcdata, rcdata.SelectedChip, jointPositionBuffer);
						labelTip.Text = selectedButton.Text + "Added.";
					}
					catch (Exception err) {
						MessageBox.Show(err.Message, "Adding failed");
						return;
					}

					break;
				case "Frame":
					try {
						new FrameChip(rcdata, rcdata.SelectedChip, jointPositionBuffer);
						labelTip.Text = selectedButton.Text + "Added.";
					}
					catch (Exception err) {
						MessageBox.Show(err.Message, "Adding failed");
						return;
					}


					break;
				case "Weight":
					try {
						new WeightChip(rcdata, rcdata.SelectedChip, jointPositionBuffer);
						labelTip.Text = selectedButton.Text + "Added.";
					}
					catch (Exception err) {
						MessageBox.Show(err.Message, "Adding failed");
						return;
					}


					break;
				case "Cowl":
					try {
						new CowlChip(rcdata, rcdata.SelectedChip, jointPositionBuffer);
						labelTip.Text = selectedButton.Text + "Added.";
					}
					catch (Exception err) {
						MessageBox.Show(err.Message, "Adding failed");
						return;
					}

					break;
				case "Rudder":
					try {
						new RudderChip(rcdata, rcdata.SelectedChip, jointPositionBuffer);
						labelTip.Text = selectedButton.Text + "Added.";
					}
					catch (Exception err) {
						MessageBox.Show(err.Message, "Adding failed");
						return;
					}


					break;
				case "Rudder frame":
					try {
						new RudderFrameChip(rcdata, rcdata.SelectedChip, jointPositionBuffer);
						labelTip.Text = selectedButton.Text + "Added.";
					}
					catch (Exception err) {
						MessageBox.Show(err.Message, "Adding failed");
						return;
					}


					break;
				case "Trim":
					try {
						new TrimChip(rcdata, rcdata.SelectedChip, jointPositionBuffer);
						labelTip.Text = selectedButton.Text + "Added.";
					}
					catch (Exception err) {
						MessageBox.Show(err.Message, "Adding failed");
						return;
					}


					break;
				case "Trim frame":
					try {
						new TrimFrameChip(rcdata, rcdata.SelectedChip, jointPositionBuffer);
						labelTip.Text = selectedButton.Text + "Added.";
					}
					catch (Exception err) {
						MessageBox.Show(err.Message, "Adding failed");
						return;
					}


					break;
				case "Wheel":
					try {
						new WheelChip(rcdata, rcdata.SelectedChip, jointPositionBuffer);
						labelTip.Text = selectedButton.Text + "Added.";
					}
					catch (Exception err) {
						MessageBox.Show(err.Message, "Adding failed");
						return;
					}

					break;
				case "Recoilless wheel":
					try {
						new RLWChip(rcdata, rcdata.SelectedChip, jointPositionBuffer);
						labelTip.Text = selectedButton.Text + "added.";
					}
					catch (Exception err) {
						MessageBox.Show(err.Message, "Adding failed");
						return;
					}


					break;
				case "Jet":
					try {
						new JetChip(rcdata, rcdata.SelectedChip, jointPositionBuffer);
						labelTip.Text = selectedButton.Text + "Added.";
					}
					catch (Exception err) {
						MessageBox.Show(err.Message, "Adding failed");
						return;
					}

					break;
				case "Arm":
					try {
						new ArmChip(rcdata, rcdata.SelectedChip, jointPositionBuffer);
						labelTip.Text = selectedButton.Text + "Added.";
					}
					catch (Exception err) {
						MessageBox.Show(err.Message, "Adding failed");
						return;
					}


					break;
				default:
					break;
			}

			//			UpdateCameraPosition(0,0,0,rcdata.Cursor.Matrix);
			//rcdata.CheckBackTrack();
			rcdata.CalcWeightCenter();
			pictTarget_Paint(this, null);

			Modified = true;
			if (treeview != null && !treeview.IsDisposed) treeview.UpdateTree(rcdata.SelectedChip);
			LoadChipInfo();
		}
#endif

		private void miFileImport_Click(object sender, System.EventArgs e) {
			if (!Modified || MessageBox.Show("Destroy the current model。", "Initialization confirmation", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK) {
				if (scriptform != null) scriptform.Hide();
				dlgOpen.Filter = "RigidChips Data File(*.rcd;*.txt)|*.rcd;*.txt|All files(*.*)|*.*";
				dlgOpen.Multiselect = false;
				dlgOpen.ShowReadOnly = false;
				if (dlgOpen.ShowDialog() == DialogResult.OK) {
					//rcdata.UnregisterChipAll(rcdata.model.root);
					rcdata.keys = new KeyEntryList(RigidChips.Environment.KeyCount);
					rcdata.vals = new ValEntryList();
					rcdata.model = new Model(rcdata);


					rcdata.headercomment = rcdata.script = "";
					//rcdata.RegisterChip(rcdata.model.root);
					StreamReader file = new StreamReader(dlgOpen.FileName, System.Text.Encoding.Default);
					rcdata.Parse(file.ReadToEnd());
					file.Close();

					rcdata.SelectedChip = rcdata.model.root;
					SetValDropList();

					GC.Collect();
					Modified = false;
					if (treeview != null && !treeview.IsDisposed) treeview.GenerateTree();
					labelTip.Text = "Loaded model data: " + System.IO.Path.GetFileNameWithoutExtension(dlgOpen.FileName);
				}
			}
		}

		private void menuItem14_Click(object sender, System.EventArgs e) {
			MessageBox.Show(string.Format("RCM -RigidChips Modeler-\n\tVersion : {0}.{1}\n\tAssembly : {2}-{3}", Application.ProductVersion.Split('.')), "Version Infomation", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		private void miFileExport_Click(object sender, System.EventArgs e) {
			dlgSave.Filter = "RigidChips Data File(*.rcd)|*.rcd";
			//			dlgSave.DefaultExt = "rcd";
			dlgSave.OverwritePrompt = true;
			dlgSave.AddExtension = true;
			string output;
			if (dlgSave.ShowDialog() == DialogResult.OK) {
				output = rcdata.vals.ToString() + rcdata.keys.ToString() + rcdata.model.ToString();
				if (rcdata.script != "") {
					output += (rcdata.luascript ? "Lua" : "Script") + "{\r\n" + rcdata.script + "}\r\n";
				}
				StreamWriter tw = new StreamWriter(dlgSave.FileName, false, System.Text.Encoding.Default);
				//				MessageBox.Show(output);
				//				MessageBox.Show(rcdata.chipCount.ToString());
				tw.Write(output);
				tw.Flush();
				tw.Close();

				labelTip.Text = "Output model data: " + System.IO.Path.GetFileNameWithoutExtension(dlgSave.FileName);

			}
		}
#if false
		/* 旧処理 */
		private void miFileSaveAs_Click(object sender, System.EventArgs e) {
			dlgSave.Filter = ".rcd File(*.rcd)|*.rcd|RCM File(*.rcm)|*.rcm";
			dlgSave.DefaultExt = "rcm";
			dlgSave.OverwritePrompt = true;

			if(dlgSave.ShowDialog() == DialogResult.OK){
				rcdata.Save(dlgSave.FileName,GenerateOptList());
				EditingFileName = dlgSave.FileName;
				labelTip.Text = "RCDファイルを保存しました : " + System.IO.Path.GetFileNameWithoutExtension(dlgSave.FileName);
				Modified = false;
				EditingFileName = dlgSave.FileName;
			}

		}
#else
		/* rcd形式との統合 */
		private void miFileSaveAs_Click(object sender, System.EventArgs e) {
			dlgSave.Filter = ".rcd File(*.rcd)|*.rcd|RCM File(*.rcm)|*.rcm";
			dlgSave.DefaultExt = "rcd";
			dlgSave.OverwritePrompt = true;


			if (dlgSave.ShowDialog() == DialogResult.OK) {
				SaveFile(dlgSave.FileName);
			}

		}

		/// <summary>
		/// 現在のモデルをファイルへ保存する
		/// </summary>
		/// <param name="filename">保存するファイル名</param>
		private void SaveFile(string filename) {
			if (string.IsNullOrEmpty(filename)) throw new ArgumentException("ファイル名が利用できません", "filename");
			string output;
			try
			{
				switch (Path.GetExtension(filename).ToLower())
				{
					case ".rcm":
						rcdata.Save(filename, GenerateOptList());
						break;
					// case ".rcd":
					default:
						output = rcdata.vals.ToString() + rcdata.keys.ToString() + rcdata.model.ToString();
						if (rcdata.script != "")
						{
							output += (rcdata.luascript ? "Lua" : "Script") + "{\r\n" + rcdata.script + "}\r\n";
						}
						StreamWriter tw = new StreamWriter(filename, false, System.Text.Encoding.Default);
						//				MessageBox.Show(output);
						//				MessageBox.Show(rcdata.chipCount.ToString());
						tw.Write(output);
						tw.Flush();
						tw.Close();

						break;
				}
			} catch (IOException ex)
			{
				MessageBox.Show(this, "Failed to save:\n\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			EditingFileName = filename;
			labelTip.Text = "Files saved: " + System.IO.Path.GetFileNameWithoutExtension(filename);
			Modified = false;
		}

#endif
		private void miListSelect_Click(object sender, System.EventArgs e) {
			ListBox list;
			switch (jointPositionBuffer) {
				case JointPosition.North:
					list = lstNorth;
					break;
				case JointPosition.South:
					list = lstSouth;
					break;
				case JointPosition.East:
					list = lstEast;
					break;
				case JointPosition.West:
					list = lstWest;
					break;
				default:
					return;
			}
			if (list.SelectedIndex < 0) return;

			rcdata.SelectedChip = (ChipBase)list.SelectedItem;
			labelTip.Text = "Moved the cursor: " + rcdata.SelectedChip.ToString() +
				((rcdata.SelectedChip.Comment != null && rcdata.SelectedChip.Comment != "") ?
				" (" + rcdata.SelectedChip.Comment + ")" : "");

		}

		private void miListCut_Click(object sender, System.EventArgs e) {
			ListBox list;
			switch (jointPositionBuffer) {
				case JointPosition.North:
					list = lstNorth;
					break;
				case JointPosition.South:
					list = lstSouth;
					break;
				case JointPosition.East:
					list = lstEast;
					break;
				case JointPosition.West:
					list = lstWest;
					break;
				default:
					return;
			}
			if (list.SelectedIndex < 0) return;

			//clipboard = (RcChipBase)list.SelectedItem;
			//rcdata.SelectedChip = clipboard.Parent;
			//clipboard.Detach();
			actionCut((ChipBase)list.SelectedItem);
			LoadChipInfo();
		}

		private void miListCopy_Click(object sender, System.EventArgs e) {
			ListBox list;
			switch (jointPositionBuffer) {
				case JointPosition.North:
					list = lstNorth;
					break;
				case JointPosition.South:
					list = lstSouth;
					break;
				case JointPosition.East:
					list = lstEast;
					break;
				case JointPosition.West:
					list = lstWest;
					break;
				default:
					return;
			}
			if (list.SelectedIndex < 0) return;

			actionCopy((ChipBase)list.SelectedItem);

		}

		private void miToolTree_Click(object sender, System.EventArgs e) {
			if (treeview == null)
				treeview = new TreeForm(rcdata, ctmChipType);
			//else {
			//    treeview.Dispose();
			//    treeview = null;
			//    miToolTree_Click(sender, e);
			//    return;
			//}
			else if(!treeview.Created) {	// 2010/03/16 変更：余計なtreeviewの再生成を抑制
				treeview.Dispose();
				treeview = null;
				miToolTree_Click(sender, e);
				return;
			}

			treeview.Show();
			treeview.Focus();	// 2010/03/16 追加：treeviewを前面に出す
		}

		private void RcData_SelectedChipChanged(object param) {
			LoadChipInfo();
			if (param != null) ProcessViewPoint(((ChipBase)param).Matrix);
			bool multiSelected = (rcdata.SelectedChipCount != 0);

			//tbbCut.Enabled = tbbCopy.Enabled = tbbInsert.Enabled = tbbChip.Enabled = tbbFrame.Enabled = 
			//    tbbRudder.Enabled = tbbRudderF.Enabled = tbbTrim.Enabled = tbbTrimF.Enabled = tbbPaste.Enabled =
			//    tbbWheel.Enabled = tbbRLW.Enabled = tbbArm.Enabled = tbbJet.Enabled = tbbWeight.Enabled = tbbCowl.Enabled = 
			//    miEditCut.Enabled = miEditCopy.Enabled = !multiSelected;
			//if(multiSelected)tbMain_ButtonClick(this,new ToolBarButtonClickEventArgs(tbbCursor));

			tsbCut.Enabled = tsbCopy.Enabled = tsbInsert.Enabled = tsbChipMode.Enabled = tsbFrameMode.Enabled =
				tsbRudderMode.Enabled = tsbRudderFMode.Enabled = tsbTrimMode.Enabled = tsbTrimFMode.Enabled = tsbPasteMode.Enabled =
				tsbWheelMode.Enabled = tsbRLWMode.Enabled = tsbArmMode.Enabled = tsbJetMode.Enabled = tsbWeightMode.Enabled = tsbCowlMode.Enabled =
				miEditCut.Enabled = miEditCopy.Enabled = !multiSelected;
			if (multiSelected) tsbModeChange_Click(tsbSelectMode, new EventArgs());
		}

		private void miCommentEdit_Click(object sender, System.EventArgs e) {
			string s = TextInputDialog.ShowDialog(rcdata.SelectedChip.Comment, "Enter a comment (Leave blank for no comment)。", 0);
			if (s != null)
				rcdata.SelectedChip.Comment = s;
		}

		private void miChipComment_Popup(object sender, System.EventArgs e) {

		}

		private void miCommentDisplay_Popup(object sender, System.EventArgs e) {
		}

		private void miFile_Click(object sender, System.EventArgs e) {

		}

		private void miHelpReadme_Click(object sender, System.EventArgs e) {
			System.Diagnostics.Process p = new System.Diagnostics.Process();

			string helpFileName = "RCM説明書.txt";
			string helpFile = "content/" + helpFileName;

			if (!File.Exists(helpFile)) {
				MessageBox.Show(helpFileName + " Was not found.");
				return;
			}

			string path = Path.GetFullPath(helpFile);

			p.StartInfo = new System.Diagnostics.ProcessStartInfo(path);
			p.Start();
		}

		private void miConfigDraw_Click(object sender, System.EventArgs e) {
			configwindow.NowTabPage = 0;
			configwindow.Show();
			configwindow.Focus();
		}

		private void miChangeType(object sender, System.EventArgs e) {
			MenuItem item = (MenuItem)sender;
			ChipBase target = rcdata.SelectedChip;

			try {
				if (item == miChangeCowl) {
					ChipBase itr = null;
					foreach (ChipBase c in target.Children) {
						if (c == null) break;
						if (!(c is CowlChip)) {
							if (MessageBox.Show("Are you sure you want to change this chip and it's children to Cowl?", "Type change error", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
								== DialogResult.Yes) {

								//	派生チップをすべてカウルに変更する再帰処理
								System.Collections.Stack s = new System.Collections.Stack();
								System.Collections.Queue q = new System.Collections.Queue();

								q.Enqueue(target);

								while (q.Count > 0) {
									s.Push(q.Dequeue());
									itr = (ChipBase)s.Peek();
									foreach (ChipBase cld in itr.Children) {
										if (cld != null) q.Enqueue(cld);
									}
								}

								while (s.Count > 0) {
									itr = (ChipBase)s.Pop();
									itr = itr.ChangeType(ChipType.Cowl);
								}
								rcdata.SelectedChip = itr;
								labelTip.Text = "Change to Cowl。";
								return;

							}
							else {
								labelTip.Text = "Change to Cowl。";
								return;
							}
						}
					}
					rcdata.SelectedChip = target.ChangeType(ChipType.Cowl);
					labelTip.Text = "Change to Cowl.";
				}
				else if (item == miChangeChip) {
					rcdata.SelectedChip = target.ChangeType(ChipType.Chip);
					labelTip.Text = "Change to Chip.";
				}
				else if (item == miChangeFrame) {
					rcdata.SelectedChip = target.ChangeType(ChipType.Frame);
					labelTip.Text = "Change to Frame.";
				}
				else if (item == miChangeRudder) {
					rcdata.SelectedChip = target.ChangeType(ChipType.Rudder);
					labelTip.Text = "Change to Rudder.";
				}
				else if (item == miChangeRudderF) {
					rcdata.SelectedChip = target.ChangeType(ChipType.RudderF);
					labelTip.Text = "Change to Rudder frame.";
				}
				else if (item == miChangeTrim) {
					rcdata.SelectedChip = target.ChangeType(ChipType.Trim);
					labelTip.Text = "Change to Trim.";
				}
				else if (item == miChangeTrimF) {
					rcdata.SelectedChip = target.ChangeType(ChipType.TrimF);
					labelTip.Text = "Change to Trim frame.";
				}
				else if (item == miChangeWheel) {
					rcdata.SelectedChip = target.ChangeType(ChipType.Wheel);
					labelTip.Text = "Change to Wheel.";
				}
				else if (item == miChangeRLW) {
					rcdata.SelectedChip = target.ChangeType(ChipType.RLW);
					labelTip.Text = "Change to RWL.";
				}
				else if (item == miChangeJet) {
					rcdata.SelectedChip = target.ChangeType(ChipType.Jet);
					labelTip.Text = "Change to Jet.";
				}
				else if (item == miChangeWeight) {
					rcdata.SelectedChip = target.ChangeType(ChipType.Weight);
					labelTip.Text = "Change to Weight";
				}
				else if (item == miChangeArm) {
					rcdata.SelectedChip = target.ChangeType(ChipType.Arm);
					labelTip.Text = "Change to Arm.";
				}
				else
					throw new Exception("Change to unknown chip type: " + item.Text);
			}
			catch (Exception exc) {
				MessageBox.Show(exc.Message + "\nFailed to change types.");
			}
			//rcdata.CheckBackTrack();

		}

		private void miCommentDelete_Click(object sender, System.EventArgs e) {
			if (MessageBox.Show("Remove the comment below。\n\n", "Comment deletion confirmation", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK) {
				rcdata.SelectedChip.Comment = "";
			}
		}

		private void miToolScript_Click(object sender, System.EventArgs e) {
			if (scriptform == null)
				scriptform = new ScriptForm(this, rcdata);
			else
				scriptform.Activate();

			scriptform.Show();
		}

		private void miFileQuit_Click(object sender, System.EventArgs e) {
			//Application.Exit();
			if (Modified)
			{
				switch (MessageBox.Show("Do you want to save?", "RCM", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
				{
					case DialogResult.Yes:
						miFileSave_Click(sender, EventArgs.Empty);
						// ここでModified == trueになるのは
						// 保存ダイアログでキャンセルされたとき
						if (Modified)
							return;
						break;
					case DialogResult.No:
						break;
					default:
						return;
				}
			}

			Application.Exit();
			this.NowClosing = true;
		}

		private void MainForm_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
			if (Modified) {
				switch (MessageBox.Show("Do you want to save?", "RCM", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question)) {
					case DialogResult.Yes:
						miFileSave_Click(sender, EventArgs.Empty);
						// ここでModified == trueになるのは
						// 保存ダイアログでキャンセルされたとき
						if (Modified) {
							e.Cancel = Modified;
							return;
						}
						break;
					case DialogResult.No:
						break;
					default:
						e.Cancel = true;
						return;
				}
			}
			this.NowClosing = true;
		}

		public bool NowClosing { private set; get; }

		private void miConfigOutput_Click(object sender, System.EventArgs e) {
			configwindow.NowTabPage = 1;
			configwindow.Show();
			configwindow.Focus();
		}


		private void threadPreview() {

		}

		private void miConfigEdit_Click(object sender, System.EventArgs e) {
			configwindow.NowTabPage = 2;
			configwindow.Show();
			configwindow.Focus();
		}

		private void miAngleGrid_Click(object sender, System.EventArgs e) {
			MenuItem item = (MenuItem)sender;
			if (item == miAngleGrid1)
				editOption.AngleViewGrid = 1;
			else if (item == miAngleGrid5)
				editOption.AngleViewGrid = 5;
			else if (item == miAngleGrid15)
				editOption.AngleViewGrid = 15;
			else if (item == miAngleGrid30)
				editOption.AngleViewGrid = 30;
			else if (item == miAngleGrid90)
				editOption.AngleViewGrid = 90;
			else
				editOption.AngleViewGrid = 0;

			pictAngle_Paint(sender, null);
		}

		private void MainForm_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e) {
			if (!PreviewMode 
				&& Control.ModifierKeys == Keys.None
				&& !txtName.Focused
//				&& !cmbColor.Focused
				&& !char.IsNumber(e.KeyChar)
				&& e.KeyChar != '-'
				&& rcdata.SelectedChipCount == 0
				&& Array.Find(cmbAttrItems, x => x.Focused) == null) {
				//				labelTip.Text = ((int)e.KeyChar).ToString() + "[" + e.KeyChar.ToString() + "]";

				char key = e.KeyChar;
				if (key == 'z' && KeyboardLayout.ShouldFlipZY())
					key = 'y';
				else if (key == 'y' && KeyboardLayout.ShouldFlipZY())
					key = 'z';

				switch (key) {
					case 'q':
						//tbMain_ButtonClick(this,new ToolBarButtonClickEventArgs(tbbChip));
						tsbModeChange_Click(tsbChipMode, new EventArgs());
						break;
					case 'w':
						//tbMain_ButtonClick(this,new ToolBarButtonClickEventArgs(tbbFrame));
						tsbModeChange_Click(tsbFrameMode, new EventArgs());
						break;
					case 'e':
						//tbMain_ButtonClick(this,new ToolBarButtonClickEventArgs(tbbRudder));
						tsbModeChange_Click(tsbRudderMode, new EventArgs());
						break;
					case 'r':
						//tbMain_ButtonClick(this,new ToolBarButtonClickEventArgs(tbbRudderF));
						tsbModeChange_Click(tsbRudderFMode, new EventArgs());
						break;
					case 't':
						//tbMain_ButtonClick(this,new ToolBarButtonClickEventArgs(tbbTrim));
						tsbModeChange_Click(tsbTrimMode, new EventArgs());
						break;
					case 'y':
						//tbMain_ButtonClick(this,new ToolBarButtonClickEventArgs(tbbTrimF));
						tsbModeChange_Click(tsbTrimFMode, new EventArgs());
						break;
					case 'a':
						//tbMain_ButtonClick(this,new ToolBarButtonClickEventArgs(tbbWheel));
						tsbModeChange_Click(tsbWheelMode, new EventArgs());
						break;
					case 's':
						//tbMain_ButtonClick(this,new ToolBarButtonClickEventArgs(tbbRLW));
						tsbModeChange_Click(tsbRLWMode, new EventArgs());
						break;
					case 'd':
						//tbMain_ButtonClick(this,new ToolBarButtonClickEventArgs(tbbJet));
						tsbModeChange_Click(tsbJetMode, new EventArgs());
						break;
					case 'f':
						//tbMain_ButtonClick(this,new ToolBarButtonClickEventArgs(tbbWeight));
						tsbModeChange_Click(tsbWeightMode, new EventArgs());
						break;
					case 'g':
						//tbMain_ButtonClick(this,new ToolBarButtonClickEventArgs(tbbCowl));
						tsbModeChange_Click(tsbCowlMode, new EventArgs());
						break;
					case 'h':
						//tbMain_ButtonClick(this,new ToolBarButtonClickEventArgs(tbbArm));
						tsbModeChange_Click(tsbArmMode, new EventArgs());
						break;
					case 'z':
						//tbMain_ButtonClick(this,new ToolBarButtonClickEventArgs(tbbCursor));
						tsbModeChange_Click(tsbSelectMode, new EventArgs());
						break;
					case 'x':
						//tbMain_ButtonClick(this,new ToolBarButtonClickEventArgs(tbbCamera));
						tsbModeChange_Click(tsbCameraMode, new EventArgs());
						break;
					case 'c':
						//tbMain_ButtonClick(this,new ToolBarButtonClickEventArgs(tbbAutoCamera));
						tsbAutoCamera_Click(tsbAutoCamera, new EventArgs());
						break;
					case 'v':
						//tbMain_ButtonClick(this,new ToolBarButtonClickEventArgs(tbbPaste));
						tsbModeChange_Click(tsbPasteMode, new EventArgs());
						break;

					/*					case 'b':
											tbMain_ButtonClick(this,new ToolBarButtonClickEventArgs(tbbChip));
											break;
										case 'n':
											tbMain_ButtonClick(this,new ToolBarButtonClickEventArgs(tbbChip));
											break;
										default:
											labelTip.Text = ((int)e.KeyChar).ToString();
											break;
					*/
				}
			}
		}

		private void cmbAttrItems_Leave(object sender, System.EventArgs e) {
			if (editOption.AttributeAutoApply)
				ApplyChipInfo();

		}

		private void txtAttrs_Enter(object sender, System.EventArgs e) {
			((Control)sender).BackColor = Color.FromArgb(drawOption.CursorFrontColor.R / 2 + 128, drawOption.CursorFrontColor.G / 2 + 128, drawOption.CursorFrontColor.B / 2 + 128);
		}

		private void txtAttrs_Leave(object sender, System.EventArgs e) {
			((Control)sender).BackColor = Color.FromKnownColor(KnownColor.Window);
			cmbAttrItems_Leave(this, e);
		}

		bool[] previewKeys = new bool[17];
		private void MainForm_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e) {
			if (PreviewMode) {
				int i = Array.IndexOf(KeyEntryList.KeyMap, e.KeyCode);
				if (i < 0) return;
				previewKeys[i] = true;
				e.Handled = true;
			}
		}

		private void MainForm_KeyUp(object sender, KeyEventArgs e) {
			if (PreviewMode) {
				int i = Array.IndexOf(KeyEntryList.KeyMap, e.KeyCode);
				if (i < 0) return;
				previewKeys[i] = false;
				e.Handled = true;
			}
		}

		private void ChipInfo_TextChanged(object sender, System.EventArgs e) {
			parameterChanged = true;
		}

		private void lstChild_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e) {
			if (e.KeyChar == 13) {
				lstChild_DoubleClick(sender, null);
			}
		}

		private void miEditDelete_Click(object sender, System.EventArgs e) {
			actionDelete();
		}

		private void miEditCut_Click(object sender, System.EventArgs e) {
			actionCut();
		}

		private void miEditCopy_Click(object sender, System.EventArgs e) {
			actionCopy();
		}

		private void miEditInfo_Click(object sender, System.EventArgs e) {
			System.Text.StringBuilder builder = new System.Text.StringBuilder();
			builder.Append("Model information:");
			builder.Append(EditingFileName);
			builder.Append("\n\n");


			int[] ChipCounts = new int[13];
			decimal fuel = 0;

			Stack<ChipBase> stack = new Stack<ChipBase>();
			int depthTotal = 0, currentDepth = 0;
			stack.Push(rcdata.model.root);
			while (stack.Count > 0) {
				ChipBase c = stack.Pop();
				if (c != null) {
					ChipType type = c.ChipType;
					ChipCounts[(int)type]++;
					fuel += (decimal)c.Fuel;
					depthTotal += currentDepth;

					stack.Push(null);
					foreach (ChipBase child in c.Children) {
						if (child != null) stack.Push(child);
					}
					currentDepth++;
				}
				else {
					currentDepth--;
				}

			}
			int sum = 0;
			for (int i = 0; i < 13; i++) {
				if (ChipCounts[i] != 0) {
					builder.Append(Enum.GetName(typeof(ChipType), (byte)i));
					builder.Append(":\t");
					builder.Append(ChipCounts[i].ToString().PadLeft(4, ' '));
					builder.Append('\n');
					sum += ChipCounts[i];
				}
			}
			builder.Append("Total chips:\t");
			builder.Append(sum.ToString().PadLeft(4, ' '));

			builder.Append("\n\nTotal amount of fuel:\n\t");
			builder.Append(fuel);

			builder.Append("\nAverage connection distance from Core:\n\t");
			builder.Append(((double)depthTotal / sum).ToString("##0.000"));

			MessageBox.Show(builder.ToString(), "model information");
		}

		private void miEditSelectParent_Click(object sender, System.EventArgs e) {
			ChipBase c = rcdata.SelectedChip;
			if (c != null) {
				if (rcdata.SelectedChip.Parent != null) rcdata.SelectedChip = rcdata.SelectedChip.Parent;
			}
			else {
				foreach (ChipBase s in rcdata.SelectedChipList) {
					rcdata.AssignSelectedChips(s);
					rcdata.AssignSelectedChips(s.Parent);
				}
			}

		}

		private void miEditSelectCore_Click(object sender, System.EventArgs e) {
			rcdata.SelectedChip = rcdata.model.root;
		}

		private void miEditSelectChild_Click(object sender, System.EventArgs e) {
			System.Collections.Queue q = new System.Collections.Queue();
			System.Collections.ArrayList a = new System.Collections.ArrayList();

			q.Enqueue(rcdata.SelectedChip);

			if (q.Peek() == null) {
				q.Clear();
				foreach (object o in rcdata.SelectedChipList) q.Enqueue(o);
			}

			while (q.Count > 0) {
				ChipBase c = (ChipBase)q.Dequeue();
				if (c == null) continue;

				foreach (ChipBase cld in c.Children) {
					if (cld != null) {
						a.Add(cld);
						q.Enqueue(cld);
					}
				}
			}
			rcdata.AssignSelectedChips(a.ToArray());

		}

		private void miEditSelectAll_Click(object sender, System.EventArgs e) {
			rcdata.AssignSelectedChips(rcdata.AllChip, true);

		}

		private void labelTip_Click(object sender, System.EventArgs e) {
#if TEST

			device.RenderState.Lighting = !device.RenderState.Lighting;
			labelTip.Text = "Lighting : " + (device.RenderState.Lighting ? "On" : "Off");

#endif
		}

		private void pictTarget_Resize(object sender, System.EventArgs e) {
			if (pictTarget.ClientSize.IsEmpty) {
				pictTarget.Dock = DockStyle.None;
				pictTarget.ClientSize = new Size(4, 4);
			}

			btnEditPanel.Top = 0;
			btnEditPanel.Left = pictTarget.Right - btnEditPanel.Width;
		}
		/*
		 * 動作可能バージョンの推定
		 * 
		 * 1.0.1	Name属性
		 * 1.0.2	_H()
		 *			_BYE()
		 *			_TICKS()
		 * 1.0.3	disp設定
		 * 1.1		_ANALOG()
		 * 1.2		_HAT()
		 * 15B1		Weightチップ
		 *			Cowlチップ
		 *			Wheel.Option属性
		 *			_ZOOM()
		 * 15B2		Color変数化
		 * 15B4		Luaスクリプト
		 *			_ROUND()
		 *			_SPLIT()
		 *			_O?()
		 *			スクリプト内#十六進表記
		 * 15B5		Wheel.Effect属性
		 * 15B6		_E?()
		 *			_R?()
		 * 15B7		Armチップ
		 *			Frame.Option属性系
		 *			_G?()
		 *			_X?()
		 *			_Y?()
		 *			_Z?()
		 *			_Q?()
		 *			_TYPE()
		 *			_OPTION()
		 *			_EFFECT()
		 *			_M()
		 * 15B10	Jet.Option属性
		 * 15B11	_T()
		 *			_MOVE3D()
		 *			_LINE3D()
		 *			_SETCOLOR()
		 * 15B13c	_GET()
		 *			_GETCHILD()
		 * 15B14	_MX()
		 *			_MY()
		 *			_ML()
		 *			_MR()
		 *			_MM()
		 *			_MOBJ()
		 *			_I()
		 *			_IOBJ()
		 *			_MOVE2D()
		 *			_LINE2D()
		 * 15B15	_DIR()
		 *			_ANGLE()
		 *			_POWER()
		 *			_SPRING()
		 *			_DAMPER()
		 *			_BRAKE()
		 *			_COLOR()
		 *			_PARENT()
		 *			_TOP()
		 * 15B17	User1,User2属性
		 * 15B21	_PLAYER*()
		 * 15B24	_FUEL()
		 *			_FUELMAX()
		 *			Weight.Option属性
		 * 15B25	Cowl.Effect属性
		 */

		private void pictTarget_FocusChanged(object sender, EventArgs e) {
			pictTarget_Paint(this, null);
		}

		private void lstSouth_MouseHover(object sender, System.EventArgs e) {
			//	if (tbbCamera.Pushed || tbbCursor.Pushed) return;
			if (selectedButton == tsbCameraMode || selectedButton == tsbSelectMode) return;
			ListBox list = sender as ListBox;
			btnListAdd.Visible = true;
			btnListAdd.Top = list.Bottom - btnListAdd.Height;
			btnListAdd.Left = list.Right - btnListAdd.Width;
			switch (list.Name) {
				case "lstNorth":
					jointPositionBuffer = JointPosition.North;
					break;
				case "lstSouth":
					jointPositionBuffer = JointPosition.South;
					break;
				case "lstEast":
					jointPositionBuffer = JointPosition.East;
					break;
				case "lstWest":
					jointPositionBuffer = JointPosition.West;
					break;
				default:
					jointPositionBuffer = JointPosition.NULL;
					break;
			}

		}

		private void lstEast_MouseLeave(object sender, System.EventArgs e) {
			ListBox list = sender as ListBox;
			if (!list.ClientRectangle.Contains(list.PointToClient(Control.MousePosition)))
				btnListAdd.Visible = false;
		}

		private void btnListAdd_Click(object sender, System.EventArgs e) {
			miListAdd_Click(btnListAdd, null);
		}

		bool preview;
		public bool PreviewMode {
			get { return preview; }
			set {
				if (preview != value) {
					preview = value;
					rcdata.Preview = value;

					Debug.WriteLine(value, "PreviewMode");
					tmr.Interval = value ? 33 : 250;

					Array.ForEach(
						new[]{
							miFileOpen,
							miFileNew,
							miEdit,
							miTool,
						},
						m => m.Enabled = !value);

				}
			}
		}

		bool koukakuMode;
		private void btnEditPanel_Click(object sender, System.EventArgs e) {
			this.SuspendLayout();
			koukakuMode = !koukakuMode;
			toolStripContainer1.TopToolStripPanelVisible = labelTip.Visible = panelCtrl.Visible = !koukakuMode;
			//btnEditPanel.Visible = !koukakuMode;
			btnEditPanel.Text = koukakuMode ? "<<" : ">>";
			PreviewMode = koukakuMode;

			rcdata.model.root.UpdateMatrix();
			rcdata.CalcWeightCenter();
			this.ResumeLayout();
			MainForm_Resize(this, null);

			pictTarget.Focus();
		}

		private void tsbModeChange_Click(object sender, EventArgs e) {
			//	各種モード変更
			foreach (ToolStripItem tsi in toolStrip1.Items)
				if (tsi != tsbAutoCamera) if (tsi is ToolStripButton) ((ToolStripButton)tsi).Checked = false;
			var btn = sender as ToolStripButton;
			btn.Checked = true;
			selectedButton = btn;
			labelTip.Text = "Mode: " + btn.Text;
		}

		private void tsbCut_Click(object sender, EventArgs e) {
			actionCut();
		}

		private void tsbCopy_Click(object sender, EventArgs e) {
			actionCopy();
		}

		private void tsbInsert_Click(object sender, EventArgs e) {
			if (rcdata.SelectedChip is CoreChip) {
				labelTip.Text = "No place to insert.";
				return;
			}
			ChipBase prevParent = rcdata.SelectedChip.Parent;
			JointPosition prevJP = rcdata.SelectedChip.JointPosition;

			ChipBase buffer;

			switch (selectedButton.Text) {
				case "Paste":
					labelTip.Text = "Cannot insert when in paste mode。";
					return;
				case "Perspective":
					labelTip.Text = "Insertion is not possible when in perspective mode。";
					return;
				case "Chip":
					try {
						buffer = new NormalChip(rcdata, prevParent, prevJP);

					}
					catch (Exception err) {
						MessageBox.Show(err.Message, "Additional error");
						return;
					}

					break;
				case "Frame":
					try {
						buffer = new FrameChip(rcdata, prevParent, prevJP);

					}
					catch (Exception err) {
						MessageBox.Show(err.Message, "Additional error");
						return;
					}


					break;
				case "Weight":
					try {
						buffer = new WeightChip(rcdata, prevParent, prevJP);

					}
					catch (Exception err) {
						MessageBox.Show(err.Message, "Additional error");
						return;
					}


					break;
				case "Cowl":
					try {
						buffer = new CowlChip(rcdata, prevParent, prevJP);
					}
					catch (Exception err) {
						MessageBox.Show(err.Message, "Additional error");
						return;
					}

					break;
				case "Rudder":
					try {
						buffer = new RudderChip(rcdata, prevParent, prevJP);

					}
					catch (Exception err) {
						MessageBox.Show(err.Message, "Additional error");
						return;
					}


					break;
				case "Rudder frame":
					try {
						buffer = new RudderFrameChip(rcdata, prevParent, prevJP);

					}
					catch (Exception err) {
						MessageBox.Show(err.Message, "Additional error");
						return;
					}


					break;
				case "Trim":
					try {
						buffer = new TrimChip(rcdata, prevParent, prevJP);

					}
					catch (Exception err) {
						MessageBox.Show(err.Message, "Additional error");
						return;
					}


					break;
				case "Trim frame":
					try {
						buffer = new TrimFrameChip(rcdata, prevParent, prevJP);

					}
					catch (Exception err) {
						MessageBox.Show(err.Message, "Additional error");
						return;
					}


					break;
				case "Wheel":
					try {
						buffer = new WheelChip(rcdata, prevParent, prevJP);

					}
					catch (Exception err) {
						MessageBox.Show(err.Message, "Additional error");
						return;
					}

					break;
				case "Recoilless wheel":
					try {
						buffer = new RLWChip(rcdata, prevParent, prevJP);

					}
					catch (Exception err) {
						MessageBox.Show(err.Message, "Additional error");
						return;
					}


					break;
				case "Jet":
					try {
						buffer = new JetChip(rcdata, prevParent, prevJP);

					}
					catch (Exception err) {
						MessageBox.Show(err.Message, "Additional error");
						return;
					}

					break;
				case "Arm":
					try {
						buffer = new ArmChip(rcdata, prevParent, prevJP);

					}
					catch (Exception err) {
						MessageBox.Show(err.Message, "Additional error");
						return;
					}


					break;
				default:
					labelTip.Text = "Unknown mode. Insertion is not possible.";
					return;
			}

			try {
				rcdata.SelectedChip.Detach();
				rcdata.SelectedChip.Attach(buffer, prevJP);
			}
			catch (Exception err) {
				MessageBox.Show(err.Message, "Insert error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				try {
					buffer.Detach();
				}
				catch { }
				try {
					rcdata.SelectedChip.Attach(prevParent, prevJP);
				}
				catch { }
			}


			LoadChipInfo();
			ProcessViewPoint(rcdata.Cursor.Matrix);
			Modified = true;
			if (treeview != null && !treeview.IsDisposed) treeview.UpdateTree(rcdata.SelectedChip);
			labelTip.Text = selectedButton.Text + "Inserted.";
			pictTarget_Paint(this, null);
		}

		private void tsbZoom_Click(object sender, EventArgs e) {
			CamDepth /= 1.5f;
			UpdateCameraPosition(0, 0, 0, CamNow);
			pictTarget_Paint(this, null);
		}

		private void tsbMooz_Click(object sender, EventArgs e) {
			CamDepth *= 1.5f;
			UpdateCameraPosition(0, 0, 0, CamNow);
			pictTarget_Paint(this, null);
		}

		private void tsbAutoCamera_Click(object sender, EventArgs e) {
			var btn = (ToolStripButton)sender;
			StartScrollCameraPosition(rcdata.Cursor.Matrix);
			labelTip.Text = "Cursor auto tracking: " + (btn.Checked ? "ON" : "OFF");
			drawOption.AutoCamera = btn.Checked;
		}

		private void tsbRemove_Click(object sender, EventArgs e) {
			actionDelete();
		}

		private void MainForm_FormClosed(object sender, FormClosedEventArgs e) {
			if (scriptform != null) scriptform.Close();
		}

		private void ctmPalette_Popup(object sender, EventArgs e) {

		}

		private void miPaletteShowDlg_Click(object sender, EventArgs e)
		{
			switch (dlgColor.ShowDialog())
			{
				case DialogResult.OK:
					var i = dlgColor.Color.ToArgb();
					if (rcdata.SelectedChipCount > 0)
					{
						ChipBase[] selectedChips = rcdata.SelectedChipList;
						foreach (ChipBase c in selectedChips)
						{
							c["Color"] = new ChipAttribute(i);
							ApplyChipInfo();
							LoadChipInfo();
						}
					} else if (rcdata.SelectedChip != null)
					{
						rcdata.SelectedChip["Color"] = new ChipAttribute(i);
						ApplyChipInfo();
						LoadChipInfo();
					}
					return;
				case DialogResult.Cancel:
				default:
					break;
			}
		}

		private void frmMain_KeyUp(object sender, KeyEventArgs e) {

		}

		private void frmMain_KeyDown(object sender, KeyEventArgs e) {

		}

        private void labelAttrValue_Click(object sender, EventArgs e)
        {

        }

        private void miListDelete_Click(object sender, EventArgs e) {
			ListBox list;
			switch (jointPositionBuffer) {
				case JointPosition.North:
					list = lstNorth;
					break;
				case JointPosition.South:
					list = lstSouth;
					break;
				case JointPosition.East:
					list = lstEast;
					break;
				case JointPosition.West:
					list = lstWest;
					break;
				default:
					return;
			}
			if (list.SelectedIndex < 0) return;

			//clipboard = (RcChipBase)list.SelectedItem;
			//rcdata.SelectedChip = clipboard.Parent;
			//clipboard.Detach();
			actionDelete((ChipBase)list.SelectedItem);
			LoadChipInfo();
		}

	}
#if false
	#region 今となっては使わない
	class XModel {
		private Mesh mesh = null;
		private Material[] mat;
		private Texture[] tex;

		private Device dev;

		public XModel(Device device) {
			this.dev = device;
		}

		public bool LoadFile(string Filename) {
			ExtendedMaterial[] matbuff;
			mesh = Mesh.FromFile(Filename, MeshFlags.SystemMemory, dev, out matbuff);
			tex = new Texture[matbuff.Length];
			mat = new Material[matbuff.Length];

			for (int i = 0; i < matbuff.Length; i++) {
				mat[i] = matbuff[i].Material3D;
				mat[i].Ambient = mat[i].Diffuse;

				if (matbuff[i].TextureFilename != null)
					tex[i] = TextureLoader.FromFile(dev, matbuff[i].TextureFilename);

			}

			return true;	//	とりあえず
		}

		public void DrawModel() {
			if (dev == null) return;

			for (int i = 0; i < this.mat.Length; i++) {
				// Set the material and texture for this subset.
				dev.Material = mat[i];
				dev.SetTexture(0, tex[i]);

				// Draw the mesh subset.
				mesh.DrawSubset(i);
			}
		}

		public void DrawModelWithAlpha(int Alpha) {
			if (dev == null) return;

			for (int i = 0; i < this.mat.Length; i++) {
				Material m = mat[i];
				m.Ambient = Color.FromArgb(Alpha, m.Ambient);
				m.Emissive = Color.FromArgb(Alpha, m.Emissive);
				m.Diffuse = Color.FromArgb(Alpha, m.Diffuse);

				dev.Material = m;
				dev.SetTexture(0, tex[i]);

				mesh.DrawSubset(i);
			}
		}
	}
	#endregion
#endif
}
