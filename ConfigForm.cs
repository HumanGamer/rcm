using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using RigidChips;
using System.Globalization;

namespace rcm
{
	/// <summary>
	/// 設定ダイアログ。
	/// </summary>
	public class ConfigForm : System.Windows.Forms.Form
	{
		DrawOptions optDraw;
		OutputOptions optOutput;
		EditOptions optEdit;

		ChipBase chipSample;
		MainForm mainwindow;

		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.TabPage tabPage3;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.CheckBox chkShowWeight;
		private System.Windows.Forms.RadioButton rbAlpha;
		private System.Windows.Forms.CheckBox chkShowGhost;
		private System.Windows.Forms.TextBox txtSwellRate;
		private System.Windows.Forms.CheckBox chkBaloonSwell;
		private System.Windows.Forms.CheckBox chkShowCowl;
		private System.Windows.Forms.PictureBox clrWeight;
		private System.Windows.Forms.CheckBox chkShowWeightGuide;
		private System.Windows.Forms.TextBox txtWeightSize;
		private System.Windows.Forms.TextBox txtWeightAlpha;
		private System.Windows.Forms.PictureBox clrXp;
		private System.Windows.Forms.PictureBox clrXn;
		private System.Windows.Forms.PictureBox clrYp;
		private System.Windows.Forms.PictureBox clrYn;
		private System.Windows.Forms.PictureBox clrZp;
		private System.Windows.Forms.PictureBox clrZn;
		private System.Windows.Forms.CheckBox chkXAxis;
		private System.Windows.Forms.CheckBox chkYAxis;
		private System.Windows.Forms.CheckBox chkZAxis;
		private System.Windows.Forms.CheckBox chkXNegAxis;
		private System.Windows.Forms.CheckBox chkYNegAxis;
		private System.Windows.Forms.CheckBox chkZNegAxis;
		private System.Windows.Forms.PictureBox clrSouth;
		private System.Windows.Forms.PictureBox clrEast;
		private System.Windows.Forms.PictureBox clrWest;
		private System.Windows.Forms.PictureBox clrNorth;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnApply;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.ColorDialog dlgColor;
		private System.Windows.Forms.RadioButton rbFrame;
		private System.Windows.Forms.PictureBox clrBack;
		private System.Windows.Forms.PictureBox clrCursorF;
		private System.Windows.Forms.PictureBox clrCursorB;
		private System.Windows.Forms.CheckBox chkShowAlways;
		private System.Windows.Forms.GroupBox groupBox5;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.CheckBox chkReturnEndChipBracket;
		private System.Windows.Forms.CheckBox chkOpenBracketWithChipDefinition;
		private System.Windows.Forms.CheckBox chkIndentEnable;
		private System.Windows.Forms.CheckBox chkIndentBySpace;
		private System.Windows.Forms.TextBox txtIndentSpaceNum;
		private System.Windows.Forms.CheckBox chkCommaWithSpace;
		private System.Windows.Forms.CheckBox chkPrintAllAttribute;
		private System.Windows.Forms.TextBox txtOutputSample;
		private System.Windows.Forms.CheckBox chkCameraOrtho;
		private System.Windows.Forms.CheckBox chkAttrCopy;
		private System.Windows.Forms.CheckBox chkUnbisibleUnselectable;
		private System.Windows.Forms.TrackBar trkScrollFrame;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.PictureBox clrWeightGuide;
		private System.Windows.Forms.Label lblScrollValue;
		private System.Windows.Forms.CheckBox chkAttrAutoApply;
		private System.Windows.Forms.RadioButton rbGChip;
		private System.Windows.Forms.ToolTip ttDescription;
		private System.Windows.Forms.Label label13;
		private CheckBox chkInvertRotateY;
		private CheckBox chkInvertRotateX;
		private CheckBox chkInvertWheel;
        private Label label14;
		private CheckBox chkColorCopy;
		private System.ComponentModel.IContainer components;

		public ConfigForm(MainForm mainform)
		{
			//
			// Windows フォーム デザイナ サポートに必要です。
			//
			InitializeComponent();

			//
			// TODO: InitializeComponent 呼び出しの後に、コンストラクタ コードを追加してください。
			//
			mainwindow = mainform;
			optDraw = mainform.rcdata.DrawOption;
			optOutput = mainform.rcdata.OutputOption;
			optEdit = mainform.rcdata.EditOption;

			ChipBase buff;
			ChipAttribute attr = new ChipAttribute();
			attr.Const = 120f;
			chipSample = new NormalChip(mainform.rcdata,null,JointPosition.NULL);
			buff = new NormalChip(mainform.rcdata,chipSample,JointPosition.North);
			buff["Angle"] = attr;
			attr.Const = 4000f;
			buff["User1"] = attr;
			new FrameChip(mainform.rcdata,buff,JointPosition.East);
			attr.Const = 0.2f;
			new FrameChip(mainform.rcdata,buff,JointPosition.West)["Damper"] = attr;
			new TrimChip(mainform.rcdata,chipSample,JointPosition.North).Comment = "これはコメントです。";
			chipSample.Name = "Root";

			//mainform.rcdata.CheckBackTrack();
		}

		public int NowTabPage{
			get{
				return tabControl1.SelectedIndex;
			}
			set{
				tabControl1.SelectedIndex = value;
			}
		}

		/// <summary>
		/// 使用されているリソースに後処理を実行します。
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows フォーム デザイナで生成されたコード 
		/// <summary>
		/// デザイナ サポートに必要なメソッドです。このメソッドの内容を
		/// コード エディタで変更しないでください。
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.chkShowAlways = new System.Windows.Forms.CheckBox();
			this.label10 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.clrNorth = new System.Windows.Forms.PictureBox();
			this.clrWest = new System.Windows.Forms.PictureBox();
			this.clrEast = new System.Windows.Forms.PictureBox();
			this.clrSouth = new System.Windows.Forms.PictureBox();
			this.clrCursorB = new System.Windows.Forms.PictureBox();
			this.clrCursorF = new System.Windows.Forms.PictureBox();
			this.clrBack = new System.Windows.Forms.PictureBox();
			this.chkCameraOrtho = new System.Windows.Forms.CheckBox();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.chkZNegAxis = new System.Windows.Forms.CheckBox();
			this.chkYNegAxis = new System.Windows.Forms.CheckBox();
			this.chkXNegAxis = new System.Windows.Forms.CheckBox();
			this.chkZAxis = new System.Windows.Forms.CheckBox();
			this.chkYAxis = new System.Windows.Forms.CheckBox();
			this.chkXAxis = new System.Windows.Forms.CheckBox();
			this.clrZn = new System.Windows.Forms.PictureBox();
			this.clrZp = new System.Windows.Forms.PictureBox();
			this.clrYn = new System.Windows.Forms.PictureBox();
			this.clrYp = new System.Windows.Forms.PictureBox();
			this.clrXn = new System.Windows.Forms.PictureBox();
			this.clrXp = new System.Windows.Forms.PictureBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.clrWeightGuide = new System.Windows.Forms.PictureBox();
			this.txtWeightAlpha = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.txtWeightSize = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.chkShowWeightGuide = new System.Windows.Forms.CheckBox();
			this.clrWeight = new System.Windows.Forms.PictureBox();
			this.chkShowWeight = new System.Windows.Forms.CheckBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.rbGChip = new System.Windows.Forms.RadioButton();
			this.rbFrame = new System.Windows.Forms.RadioButton();
			this.rbAlpha = new System.Windows.Forms.RadioButton();
			this.chkShowGhost = new System.Windows.Forms.CheckBox();
			this.txtSwellRate = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.chkBaloonSwell = new System.Windows.Forms.CheckBox();
			this.chkShowCowl = new System.Windows.Forms.CheckBox();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.label13 = new System.Windows.Forms.Label();
			this.chkPrintAllAttribute = new System.Windows.Forms.CheckBox();
			this.chkCommaWithSpace = new System.Windows.Forms.CheckBox();
			this.label11 = new System.Windows.Forms.Label();
			this.txtIndentSpaceNum = new System.Windows.Forms.TextBox();
			this.chkIndentBySpace = new System.Windows.Forms.CheckBox();
			this.chkIndentEnable = new System.Windows.Forms.CheckBox();
			this.chkOpenBracketWithChipDefinition = new System.Windows.Forms.CheckBox();
			this.chkReturnEndChipBracket = new System.Windows.Forms.CheckBox();
			this.groupBox5 = new System.Windows.Forms.GroupBox();
			this.txtOutputSample = new System.Windows.Forms.TextBox();
			this.tabPage3 = new System.Windows.Forms.TabPage();
			this.label14 = new System.Windows.Forms.Label();
			this.chkInvertRotateY = new System.Windows.Forms.CheckBox();
			this.chkInvertRotateX = new System.Windows.Forms.CheckBox();
			this.chkInvertWheel = new System.Windows.Forms.CheckBox();
			this.chkAttrAutoApply = new System.Windows.Forms.CheckBox();
			this.lblScrollValue = new System.Windows.Forms.Label();
			this.label12 = new System.Windows.Forms.Label();
			this.trkScrollFrame = new System.Windows.Forms.TrackBar();
			this.chkUnbisibleUnselectable = new System.Windows.Forms.CheckBox();
			this.chkAttrCopy = new System.Windows.Forms.CheckBox();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnApply = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.dlgColor = new System.Windows.Forms.ColorDialog();
			this.ttDescription = new System.Windows.Forms.ToolTip(this.components);
			this.chkColorCopy = new System.Windows.Forms.CheckBox();
			this.tabControl1.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.groupBox4.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.clrNorth)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.clrWest)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.clrEast)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.clrSouth)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.clrCursorB)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.clrCursorF)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.clrBack)).BeginInit();
			this.groupBox3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.clrZn)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.clrZp)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.clrYn)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.clrYp)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.clrXn)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.clrXp)).BeginInit();
			this.groupBox2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.clrWeightGuide)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.clrWeight)).BeginInit();
			this.groupBox1.SuspendLayout();
			this.tabPage2.SuspendLayout();
			this.groupBox5.SuspendLayout();
			this.tabPage3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.trkScrollFrame)).BeginInit();
			this.SuspendLayout();
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Controls.Add(this.tabPage2);
			this.tabControl1.Controls.Add(this.tabPage3);
			this.tabControl1.Dock = System.Windows.Forms.DockStyle.Top;
			this.tabControl1.Location = new System.Drawing.Point(0, 0);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(553, 310);
			this.tabControl1.TabIndex = 0;
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add(this.groupBox4);
			this.tabPage1.Controls.Add(this.groupBox3);
			this.tabPage1.Controls.Add(this.groupBox2);
			this.tabPage1.Controls.Add(this.groupBox1);
			this.tabPage1.Location = new System.Drawing.Point(4, 22);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Size = new System.Drawing.Size(545, 284);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "Display";
			// 
			// groupBox4
			// 
			this.groupBox4.Controls.Add(this.chkShowAlways);
			this.groupBox4.Controls.Add(this.label10);
			this.groupBox4.Controls.Add(this.label9);
			this.groupBox4.Controls.Add(this.label8);
			this.groupBox4.Controls.Add(this.label7);
			this.groupBox4.Controls.Add(this.label6);
			this.groupBox4.Controls.Add(this.label5);
			this.groupBox4.Controls.Add(this.label4);
			this.groupBox4.Controls.Add(this.clrNorth);
			this.groupBox4.Controls.Add(this.clrWest);
			this.groupBox4.Controls.Add(this.clrEast);
			this.groupBox4.Controls.Add(this.clrSouth);
			this.groupBox4.Controls.Add(this.clrCursorB);
			this.groupBox4.Controls.Add(this.clrCursorF);
			this.groupBox4.Controls.Add(this.clrBack);
			this.groupBox4.Controls.Add(this.chkCameraOrtho);
			this.groupBox4.Location = new System.Drawing.Point(303, 130);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size(232, 130);
			this.groupBox4.TabIndex = 11;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "Other";
			// 
			// chkShowAlways
			// 
			this.chkShowAlways.Location = new System.Drawing.Point(11, 104);
			this.chkShowAlways.Name = "chkShowAlways";
			this.chkShowAlways.Size = new System.Drawing.Size(107, 17);
			this.chkShowAlways.TabIndex = 34;
			this.chkShowAlways.Text = "Always display";
			this.ttDescription.SetToolTip(this.chkShowAlways, "Displays guides even while changing the viewpoint.");
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(184, 26);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(16, 17);
			this.label10.TabIndex = 33;
			this.label10.Text = "B";
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(106, 17);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(40, 23);
			this.label9.TabIndex = 32;
			this.label9.Text = "cursor table";
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(168, 61);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(32, 17);
			this.label8.TabIndex = 31;
			this.label8.Text = "BG";
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(56, 78);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(16, 17);
			this.label7.TabIndex = 30;
			this.label7.Text = "S";
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(80, 43);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(16, 18);
			this.label6.TabIndex = 29;
			this.label6.Text = "E";
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(8, 69);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(16, 18);
			this.label5.TabIndex = 28;
			this.label5.Text = "W";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(56, 17);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(16, 18);
			this.label4.TabIndex = 27;
			this.label4.Text = "N";
			// 
			// clrNorth
			// 
			this.clrNorth.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.clrNorth.Location = new System.Drawing.Point(32, 17);
			this.clrNorth.Name = "clrNorth";
			this.clrNorth.Size = new System.Drawing.Size(24, 26);
			this.clrNorth.TabIndex = 25;
			this.clrNorth.TabStop = false;
			this.ttDescription.SetToolTip(this.clrNorth, "Set Color");
			this.clrNorth.Click += new System.EventHandler(this.ColorBox_Click);
			// 
			// clrWest
			// 
			this.clrWest.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.clrWest.Location = new System.Drawing.Point(8, 43);
			this.clrWest.Name = "clrWest";
			this.clrWest.Size = new System.Drawing.Size(24, 26);
			this.clrWest.TabIndex = 23;
			this.clrWest.TabStop = false;
			this.ttDescription.SetToolTip(this.clrWest, "Set Color");
			this.clrWest.Click += new System.EventHandler(this.ColorBox_Click);
			// 
			// clrEast
			// 
			this.clrEast.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.clrEast.Location = new System.Drawing.Point(56, 43);
			this.clrEast.Name = "clrEast";
			this.clrEast.Size = new System.Drawing.Size(24, 26);
			this.clrEast.TabIndex = 21;
			this.clrEast.TabStop = false;
			this.ttDescription.SetToolTip(this.clrEast, "Set Color");
			this.clrEast.Click += new System.EventHandler(this.ColorBox_Click);
			// 
			// clrSouth
			// 
			this.clrSouth.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.clrSouth.Location = new System.Drawing.Point(32, 69);
			this.clrSouth.Name = "clrSouth";
			this.clrSouth.Size = new System.Drawing.Size(24, 26);
			this.clrSouth.TabIndex = 19;
			this.clrSouth.TabStop = false;
			this.ttDescription.SetToolTip(this.clrSouth, "Set Color");
			this.clrSouth.Click += new System.EventHandler(this.ColorBox_Click);
			// 
			// clrCursorB
			// 
			this.clrCursorB.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.clrCursorB.Location = new System.Drawing.Point(200, 17);
			this.clrCursorB.Name = "clrCursorB";
			this.clrCursorB.Size = new System.Drawing.Size(24, 26);
			this.clrCursorB.TabIndex = 17;
			this.clrCursorB.TabStop = false;
			this.ttDescription.SetToolTip(this.clrCursorB, "Set Color");
			this.clrCursorB.Click += new System.EventHandler(this.ColorBox_Click);
			// 
			// clrCursorF
			// 
			this.clrCursorF.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.clrCursorF.Location = new System.Drawing.Point(152, 17);
			this.clrCursorF.Name = "clrCursorF";
			this.clrCursorF.Size = new System.Drawing.Size(24, 26);
			this.clrCursorF.TabIndex = 15;
			this.clrCursorF.TabStop = false;
			this.ttDescription.SetToolTip(this.clrCursorF, "Set color");
			this.clrCursorF.Click += new System.EventHandler(this.ColorBox_Click);
			// 
			// clrBack
			// 
			this.clrBack.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.clrBack.Location = new System.Drawing.Point(200, 52);
			this.clrBack.Name = "clrBack";
			this.clrBack.Size = new System.Drawing.Size(24, 26);
			this.clrBack.TabIndex = 13;
			this.clrBack.TabStop = false;
			this.ttDescription.SetToolTip(this.clrBack, "Set color");
			this.clrBack.Click += new System.EventHandler(this.ColorBox_Click);
			// 
			// chkCameraOrtho
			// 
			this.chkCameraOrtho.Location = new System.Drawing.Point(124, 104);
			this.chkCameraOrtho.Name = "chkCameraOrtho";
			this.chkCameraOrtho.Size = new System.Drawing.Size(102, 17);
			this.chkCameraOrtho.TabIndex = 0;
			this.chkCameraOrtho.Text = "Ortho Cam";
			this.ttDescription.SetToolTip(this.chkCameraOrtho, "Use the orthographic camera");
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.chkZNegAxis);
			this.groupBox3.Controls.Add(this.chkYNegAxis);
			this.groupBox3.Controls.Add(this.chkXNegAxis);
			this.groupBox3.Controls.Add(this.chkZAxis);
			this.groupBox3.Controls.Add(this.chkYAxis);
			this.groupBox3.Controls.Add(this.chkXAxis);
			this.groupBox3.Controls.Add(this.clrZn);
			this.groupBox3.Controls.Add(this.clrZp);
			this.groupBox3.Controls.Add(this.clrYn);
			this.groupBox3.Controls.Add(this.clrYp);
			this.groupBox3.Controls.Add(this.clrXn);
			this.groupBox3.Controls.Add(this.clrXp);
			this.groupBox3.Location = new System.Drawing.Point(303, 9);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(232, 121);
			this.groupBox3.TabIndex = 10;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Grid lines";
			// 
			// chkZNegAxis
			// 
			this.chkZNegAxis.Location = new System.Drawing.Point(120, 87);
			this.chkZNegAxis.Name = "chkZNegAxis";
			this.chkZNegAxis.Size = new System.Drawing.Size(64, 26);
			this.chkZNegAxis.TabIndex = 27;
			this.chkZNegAxis.Text = "Z Minus";
			this.ttDescription.SetToolTip(this.chkZNegAxis, "Displays a line in the negative direction (back) of the Z coordinate.");
			// 
			// chkYNegAxis
			// 
			this.chkYNegAxis.Location = new System.Drawing.Point(120, 52);
			this.chkYNegAxis.Name = "chkYNegAxis";
			this.chkYNegAxis.Size = new System.Drawing.Size(64, 26);
			this.chkYNegAxis.TabIndex = 26;
			this.chkYNegAxis.Text = "Y Minus";
			this.ttDescription.SetToolTip(this.chkYNegAxis, "Displays a line in the negative direction (up) of the Y coordinate.");
			// 
			// chkXNegAxis
			// 
			this.chkXNegAxis.Location = new System.Drawing.Point(120, 17);
			this.chkXNegAxis.Name = "chkXNegAxis";
			this.chkXNegAxis.Size = new System.Drawing.Size(64, 26);
			this.chkXNegAxis.TabIndex = 25;
			this.chkXNegAxis.Text = "X Minus";
			this.ttDescription.SetToolTip(this.chkXNegAxis, "Displays a line in the negative direction (right) of the X coordinate.");
			// 
			// chkZAxis
			// 
			this.chkZAxis.Location = new System.Drawing.Point(8, 87);
			this.chkZAxis.Name = "chkZAxis";
			this.chkZAxis.Size = new System.Drawing.Size(64, 26);
			this.chkZAxis.TabIndex = 24;
			this.chkZAxis.Text = "Z Plus";
			this.ttDescription.SetToolTip(this.chkZAxis, "Displays a line in the positive direction (back) of the Z coordinate.");
			// 
			// chkYAxis
			// 
			this.chkYAxis.Location = new System.Drawing.Point(8, 52);
			this.chkYAxis.Name = "chkYAxis";
			this.chkYAxis.Size = new System.Drawing.Size(64, 26);
			this.chkYAxis.TabIndex = 23;
			this.chkYAxis.Text = "Y Plus";
			this.ttDescription.SetToolTip(this.chkYAxis, "Displays a line in the positive direction (up) of the Y coordinate.");
			// 
			// chkXAxis
			// 
			this.chkXAxis.Location = new System.Drawing.Point(8, 17);
			this.chkXAxis.Name = "chkXAxis";
			this.chkXAxis.Size = new System.Drawing.Size(64, 26);
			this.chkXAxis.TabIndex = 22;
			this.chkXAxis.Text = "X Plus";
			this.ttDescription.SetToolTip(this.chkXAxis, "Displays a line in the positive direction (right) of the X coordinate.");
			this.chkXAxis.CheckedChanged += new System.EventHandler(this.chkXAxis_CheckedChanged);
			// 
			// clrZn
			// 
			this.clrZn.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.clrZn.Location = new System.Drawing.Point(200, 87);
			this.clrZn.Name = "clrZn";
			this.clrZn.Size = new System.Drawing.Size(24, 26);
			this.clrZn.TabIndex = 21;
			this.clrZn.TabStop = false;
			this.ttDescription.SetToolTip(this.clrZn, "Set Color");
			this.clrZn.Click += new System.EventHandler(this.ColorBox_Click);
			// 
			// clrZp
			// 
			this.clrZp.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.clrZp.Location = new System.Drawing.Point(88, 87);
			this.clrZp.Name = "clrZp";
			this.clrZp.Size = new System.Drawing.Size(24, 26);
			this.clrZp.TabIndex = 19;
			this.clrZp.TabStop = false;
			this.ttDescription.SetToolTip(this.clrZp, "Set Color");
			this.clrZp.Click += new System.EventHandler(this.ColorBox_Click);
			// 
			// clrYn
			// 
			this.clrYn.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.clrYn.Location = new System.Drawing.Point(200, 52);
			this.clrYn.Name = "clrYn";
			this.clrYn.Size = new System.Drawing.Size(24, 26);
			this.clrYn.TabIndex = 17;
			this.clrYn.TabStop = false;
			this.ttDescription.SetToolTip(this.clrYn, "Set Color");
			this.clrYn.Click += new System.EventHandler(this.ColorBox_Click);
			// 
			// clrYp
			// 
			this.clrYp.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.clrYp.Location = new System.Drawing.Point(88, 52);
			this.clrYp.Name = "clrYp";
			this.clrYp.Size = new System.Drawing.Size(24, 26);
			this.clrYp.TabIndex = 15;
			this.clrYp.TabStop = false;
			this.ttDescription.SetToolTip(this.clrYp, "Set Color");
			this.clrYp.Click += new System.EventHandler(this.ColorBox_Click);
			// 
			// clrXn
			// 
			this.clrXn.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.clrXn.Location = new System.Drawing.Point(200, 17);
			this.clrXn.Name = "clrXn";
			this.clrXn.Size = new System.Drawing.Size(24, 26);
			this.clrXn.TabIndex = 13;
			this.clrXn.TabStop = false;
			this.ttDescription.SetToolTip(this.clrXn, "Set Color");
			this.clrXn.Click += new System.EventHandler(this.ColorBox_Click);
			// 
			// clrXp
			// 
			this.clrXp.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.clrXp.Location = new System.Drawing.Point(88, 17);
			this.clrXp.Name = "clrXp";
			this.clrXp.Size = new System.Drawing.Size(24, 26);
			this.clrXp.TabIndex = 11;
			this.clrXp.TabStop = false;
			this.ttDescription.SetToolTip(this.clrXp, "Set Color");
			this.clrXp.Click += new System.EventHandler(this.ColorBox_Click);
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.clrWeightGuide);
			this.groupBox2.Controls.Add(this.txtWeightAlpha);
			this.groupBox2.Controls.Add(this.label3);
			this.groupBox2.Controls.Add(this.txtWeightSize);
			this.groupBox2.Controls.Add(this.label2);
			this.groupBox2.Controls.Add(this.chkShowWeightGuide);
			this.groupBox2.Controls.Add(this.clrWeight);
			this.groupBox2.Controls.Add(this.chkShowWeight);
			this.groupBox2.Location = new System.Drawing.Point(8, 130);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(289, 130);
			this.groupBox2.TabIndex = 9;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Center of mass";
			// 
			// clrWeightGuide
			// 
			this.clrWeightGuide.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.clrWeightGuide.Location = new System.Drawing.Point(242, 89);
			this.clrWeightGuide.Name = "clrWeightGuide";
			this.clrWeightGuide.Size = new System.Drawing.Size(24, 26);
			this.clrWeightGuide.TabIndex = 15;
			this.clrWeightGuide.TabStop = false;
			this.ttDescription.SetToolTip(this.clrWeightGuide, "Set Color of model");
			this.clrWeightGuide.Click += new System.EventHandler(this.ColorBox_Click);
			// 
			// txtWeightAlpha
			// 
			this.txtWeightAlpha.Location = new System.Drawing.Point(152, 95);
			this.txtWeightAlpha.Name = "txtWeightAlpha";
			this.txtWeightAlpha.Size = new System.Drawing.Size(40, 20);
			this.txtWeightAlpha.TabIndex = 14;
			this.txtWeightAlpha.Text = "textBox3";
			this.ttDescription.SetToolTip(this.txtWeightAlpha, "The opacity of the centroid guide. The smaller the value, the more transparent.");
			this.txtWeightAlpha.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtValueInput_KeyPress);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(127, 78);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(96, 17);
			this.label3.TabIndex = 13;
			this.label3.Text = "Opacity (0.0 - 1.0)";
			// 
			// txtWeightSize
			// 
			this.txtWeightSize.Location = new System.Drawing.Point(41, 95);
			this.txtWeightSize.Name = "txtWeightSize";
			this.txtWeightSize.Size = new System.Drawing.Size(40, 20);
			this.txtWeightSize.TabIndex = 12;
			this.txtWeightSize.Text = "textBox2";
			this.ttDescription.SetToolTip(this.txtWeightSize, "The size of the center of gravity guide.");
			this.txtWeightSize.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtValueInput_KeyPress);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(16, 78);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(96, 17);
			this.label2.TabIndex = 11;
			this.label2.Text = "Size (Default 1.5)";
			// 
			// chkShowWeightGuide
			// 
			this.chkShowWeightGuide.Location = new System.Drawing.Point(8, 43);
			this.chkShowWeightGuide.Name = "chkShowWeightGuide";
			this.chkShowWeightGuide.Size = new System.Drawing.Size(184, 26);
			this.chkShowWeightGuide.TabIndex = 10;
			this.chkShowWeightGuide.Text = "Show Center Of Mass model";
			this.ttDescription.SetToolTip(this.chkShowWeightGuide, "Displays a diamond-shaped guide at the center of gravity.");
			this.chkShowWeightGuide.CheckedChanged += new System.EventHandler(this.chkShowWeightGuide_CheckedChanged);
			// 
			// clrWeight
			// 
			this.clrWeight.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.clrWeight.Location = new System.Drawing.Point(192, 17);
			this.clrWeight.Name = "clrWeight";
			this.clrWeight.Size = new System.Drawing.Size(24, 26);
			this.clrWeight.TabIndex = 9;
			this.clrWeight.TabStop = false;
			this.ttDescription.SetToolTip(this.clrWeight, "Set Color of lines");
			this.clrWeight.Click += new System.EventHandler(this.ColorBox_Click);
			// 
			// chkShowWeight
			// 
			this.chkShowWeight.Location = new System.Drawing.Point(8, 17);
			this.chkShowWeight.Name = "chkShowWeight";
			this.chkShowWeight.Size = new System.Drawing.Size(178, 26);
			this.chkShowWeight.TabIndex = 7;
			this.chkShowWeight.Text = "Display Center Of Mass lines";
			this.ttDescription.SetToolTip(this.chkShowWeight, "Displays a line representing the centroid.");
			this.chkShowWeight.CheckedChanged += new System.EventHandler(this.chkShowWeight_CheckedChanged);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.rbGChip);
			this.groupBox1.Controls.Add(this.rbFrame);
			this.groupBox1.Controls.Add(this.rbAlpha);
			this.groupBox1.Controls.Add(this.chkShowGhost);
			this.groupBox1.Controls.Add(this.txtSwellRate);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this.chkBaloonSwell);
			this.groupBox1.Controls.Add(this.chkShowCowl);
			this.groupBox1.Location = new System.Drawing.Point(8, 9);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(289, 121);
			this.groupBox1.TabIndex = 8;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Chips";
			// 
			// rbGChip
			// 
			this.rbGChip.Location = new System.Drawing.Point(168, 87);
			this.rbGChip.Name = "rbGChip";
			this.rbGChip.Size = new System.Drawing.Size(64, 17);
			this.rbGChip.TabIndex = 14;
			this.rbGChip.Text = "？？G.x";
			this.ttDescription.SetToolTip(this.rbGChip, "Use separate files named ChipG.x and RudderG.x.");
			// 
			// rbFrame
			// 
			this.rbFrame.Location = new System.Drawing.Point(96, 87);
			this.rbFrame.Name = "rbFrame";
			this.rbFrame.Size = new System.Drawing.Size(64, 17);
			this.rbFrame.TabIndex = 13;
			this.rbFrame.Text = "Frame";
			this.ttDescription.SetToolTip(this.rbFrame, "It has the same display as the non-ghosted frame.");
			// 
			// rbAlpha
			// 
			this.rbAlpha.Location = new System.Drawing.Point(16, 87);
			this.rbAlpha.Name = "rbAlpha";
			this.rbAlpha.Size = new System.Drawing.Size(88, 17);
			this.rbAlpha.TabIndex = 12;
			this.rbAlpha.Text = "Translucent";
			this.ttDescription.SetToolTip(this.rbAlpha, "Translucent display of Chip for Frame and Rudder for RudderF.");
			this.rbAlpha.CheckedChanged += new System.EventHandler(this.rbAlpha_CheckedChanged);
			// 
			// chkShowGhost
			// 
			this.chkShowGhost.Location = new System.Drawing.Point(8, 69);
			this.chkShowGhost.Name = "chkShowGhost";
			this.chkShowGhost.Size = new System.Drawing.Size(128, 18);
			this.chkShowGhost.TabIndex = 11;
			this.chkShowGhost.Text = "Show Ghost Chips";
			this.ttDescription.SetToolTip(this.chkShowGhost, "Visualize ghost chips.");
			// 
			// txtSwellRate
			// 
			this.txtSwellRate.Location = new System.Drawing.Point(243, 16);
			this.txtSwellRate.Name = "txtSwellRate";
			this.txtSwellRate.Size = new System.Drawing.Size(40, 20);
			this.txtSwellRate.TabIndex = 10;
			this.txtSwellRate.Text = "textBox1";
			this.txtSwellRate.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtValueInput_KeyPress);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(152, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(91, 42);
			this.label1.TabIndex = 9;
			this.label1.Text = "Expansion rate (standard: 0.5)";
			// 
			// chkBaloonSwell
			// 
			this.chkBaloonSwell.Location = new System.Drawing.Point(8, 43);
			this.chkBaloonSwell.Name = "chkBaloonSwell";
			this.chkBaloonSwell.Size = new System.Drawing.Size(128, 18);
			this.chkBaloonSwell.TabIndex = 8;
			this.chkBaloonSwell.Text = "Inflate Balloon";
			this.ttDescription.SetToolTip(this.chkBaloonSwell, "Make the size change according to the Jet balloon\'s Power value.");
			this.chkBaloonSwell.CheckedChanged += new System.EventHandler(this.chkBaloonSwell_CheckedChanged);
			// 
			// chkShowCowl
			// 
			this.chkShowCowl.Location = new System.Drawing.Point(8, 17);
			this.chkShowCowl.Name = "chkShowCowl";
			this.chkShowCowl.Size = new System.Drawing.Size(104, 18);
			this.chkShowCowl.TabIndex = 7;
			this.chkShowCowl.Text = "Show Cowl";
			this.ttDescription.SetToolTip(this.chkShowCowl, "Remove the check to hide the cowl.");
			// 
			// tabPage2
			// 
			this.tabPage2.Controls.Add(this.label13);
			this.tabPage2.Controls.Add(this.chkPrintAllAttribute);
			this.tabPage2.Controls.Add(this.chkCommaWithSpace);
			this.tabPage2.Controls.Add(this.label11);
			this.tabPage2.Controls.Add(this.txtIndentSpaceNum);
			this.tabPage2.Controls.Add(this.chkIndentBySpace);
			this.tabPage2.Controls.Add(this.chkIndentEnable);
			this.tabPage2.Controls.Add(this.chkOpenBracketWithChipDefinition);
			this.tabPage2.Controls.Add(this.chkReturnEndChipBracket);
			this.tabPage2.Controls.Add(this.groupBox5);
			this.tabPage2.Location = new System.Drawing.Point(4, 22);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Size = new System.Drawing.Size(545, 284);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "RCD Output";
			// 
			// label13
			// 
			this.label13.Location = new System.Drawing.Point(24, 234);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(176, 17);
			this.label13.TabIndex = 9;
			this.label13.Text = "Hit \"Apply\" to save";
			// 
			// chkPrintAllAttribute
			// 
			this.chkPrintAllAttribute.Location = new System.Drawing.Point(16, 182);
			this.chkPrintAllAttribute.Name = "chkPrintAllAttribute";
			this.chkPrintAllAttribute.Size = new System.Drawing.Size(176, 17);
			this.chkPrintAllAttribute.TabIndex = 8;
			this.chkPrintAllAttribute.Text = "Export all attributes";
			this.chkPrintAllAttribute.CheckedChanged += new System.EventHandler(this.updateOutputPreview);
			// 
			// chkCommaWithSpace
			// 
			this.chkCommaWithSpace.Location = new System.Drawing.Point(16, 156);
			this.chkCommaWithSpace.Name = "chkCommaWithSpace";
			this.chkCommaWithSpace.Size = new System.Drawing.Size(176, 17);
			this.chkCommaWithSpace.TabIndex = 7;
			this.chkCommaWithSpace.Text = "Add spaces to commas";
			this.chkCommaWithSpace.CheckedChanged += new System.EventHandler(this.updateOutputPreview);
			// 
			// label11
			// 
			this.label11.Location = new System.Drawing.Point(112, 121);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(48, 18);
			this.label11.TabIndex = 6;
			this.label11.Text = "Quantity";
			// 
			// txtIndentSpaceNum
			// 
			this.txtIndentSpaceNum.Location = new System.Drawing.Point(160, 121);
			this.txtIndentSpaceNum.Name = "txtIndentSpaceNum";
			this.txtIndentSpaceNum.Size = new System.Drawing.Size(48, 20);
			this.txtIndentSpaceNum.TabIndex = 5;
			this.txtIndentSpaceNum.Text = "1";
			this.txtIndentSpaceNum.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.txtIndentSpaceNum.TextChanged += new System.EventHandler(this.txtIndentSpaceNum_TextChanged);
			this.txtIndentSpaceNum.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtIndentSpaceNum_KeyPress);
			// 
			// chkIndentBySpace
			// 
			this.chkIndentBySpace.Location = new System.Drawing.Point(32, 95);
			this.chkIndentBySpace.Name = "chkIndentBySpace";
			this.chkIndentBySpace.Size = new System.Drawing.Size(176, 18);
			this.chkIndentBySpace.TabIndex = 4;
			this.chkIndentBySpace.Text = "Spaces instead of Tabs";
			this.chkIndentBySpace.CheckedChanged += new System.EventHandler(this.updateOutputPreview);
			// 
			// chkIndentEnable
			// 
			this.chkIndentEnable.Location = new System.Drawing.Point(16, 69);
			this.chkIndentEnable.Name = "chkIndentEnable";
			this.chkIndentEnable.Size = new System.Drawing.Size(176, 18);
			this.chkIndentEnable.TabIndex = 3;
			this.chkIndentEnable.Text = "To indent";
			this.chkIndentEnable.CheckedChanged += new System.EventHandler(this.updateOutputPreview);
			// 
			// chkOpenBracketWithChipDefinition
			// 
			this.chkOpenBracketWithChipDefinition.Location = new System.Drawing.Point(16, 43);
			this.chkOpenBracketWithChipDefinition.Name = "chkOpenBracketWithChipDefinition";
			this.chkOpenBracketWithChipDefinition.Size = new System.Drawing.Size(251, 18);
			this.chkOpenBracketWithChipDefinition.TabIndex = 2;
			this.chkOpenBracketWithChipDefinition.Text = "put \'{\' on the same line as the chip definition";
			this.chkOpenBracketWithChipDefinition.CheckedChanged += new System.EventHandler(this.updateOutputPreview);
			// 
			// chkReturnEndChipBracket
			// 
			this.chkReturnEndChipBracket.Location = new System.Drawing.Point(16, 17);
			this.chkReturnEndChipBracket.Name = "chkReturnEndChipBracket";
			this.chkReturnEndChipBracket.Size = new System.Drawing.Size(239, 18);
			this.chkReturnEndChipBracket.TabIndex = 1;
			this.chkReturnEndChipBracket.Text = "Insert new line at end of the chip";
			this.chkReturnEndChipBracket.CheckedChanged += new System.EventHandler(this.updateOutputPreview);
			// 
			// groupBox5
			// 
			this.groupBox5.Controls.Add(this.txtOutputSample);
			this.groupBox5.Location = new System.Drawing.Point(273, 3);
			this.groupBox5.Name = "groupBox5";
			this.groupBox5.Size = new System.Drawing.Size(264, 251);
			this.groupBox5.TabIndex = 0;
			this.groupBox5.TabStop = false;
			this.groupBox5.Text = "Preview";
			this.groupBox5.Paint += new System.Windows.Forms.PaintEventHandler(this.groupBox5_Paint);
			// 
			// txtOutputSample
			// 
			this.txtOutputSample.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtOutputSample.Font = new System.Drawing.Font("MS Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.txtOutputSample.Location = new System.Drawing.Point(3, 16);
			this.txtOutputSample.Multiline = true;
			this.txtOutputSample.Name = "txtOutputSample";
			this.txtOutputSample.ReadOnly = true;
			this.txtOutputSample.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
			this.txtOutputSample.Size = new System.Drawing.Size(258, 232);
			this.txtOutputSample.TabIndex = 0;
			this.txtOutputSample.WordWrap = false;
			// 
			// tabPage3
			// 
			this.tabPage3.Controls.Add(this.chkColorCopy);
			this.tabPage3.Controls.Add(this.label14);
			this.tabPage3.Controls.Add(this.chkInvertRotateY);
			this.tabPage3.Controls.Add(this.chkInvertRotateX);
			this.tabPage3.Controls.Add(this.chkInvertWheel);
			this.tabPage3.Controls.Add(this.chkAttrAutoApply);
			this.tabPage3.Controls.Add(this.lblScrollValue);
			this.tabPage3.Controls.Add(this.label12);
			this.tabPage3.Controls.Add(this.trkScrollFrame);
			this.tabPage3.Controls.Add(this.chkUnbisibleUnselectable);
			this.tabPage3.Controls.Add(this.chkAttrCopy);
			this.tabPage3.Location = new System.Drawing.Point(4, 22);
			this.tabPage3.Name = "tabPage3";
			this.tabPage3.Size = new System.Drawing.Size(545, 284);
			this.tabPage3.TabIndex = 2;
			this.tabPage3.Text = "Edit";
			// 
			// label14
			// 
			this.label14.Location = new System.Drawing.Point(13, 195);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(280, 18);
			this.label14.TabIndex = 9;
			this.label14.Text = "Camera settings";
			// 
			// chkInvertRotateY
			// 
			this.chkInvertRotateY.AutoSize = true;
			this.chkInvertRotateY.Location = new System.Drawing.Point(151, 240);
			this.chkInvertRotateY.Name = "chkInvertRotateY";
			this.chkInvertRotateY.Size = new System.Drawing.Size(91, 17);
			this.chkInvertRotateY.TabIndex = 8;
			this.chkInvertRotateY.Text = "Invert Vertical";
			this.chkInvertRotateY.UseVisualStyleBackColor = true;
			// 
			// chkInvertRotateX
			// 
			this.chkInvertRotateX.AutoSize = true;
			this.chkInvertRotateX.Location = new System.Drawing.Point(16, 240);
			this.chkInvertRotateX.Name = "chkInvertRotateX";
			this.chkInvertRotateX.Size = new System.Drawing.Size(103, 17);
			this.chkInvertRotateX.TabIndex = 7;
			this.chkInvertRotateX.Text = "Invert Horizontal";
			this.chkInvertRotateX.UseVisualStyleBackColor = true;
			// 
			// chkInvertWheel
			// 
			this.chkInvertWheel.AutoSize = true;
			this.chkInvertWheel.Location = new System.Drawing.Point(16, 216);
			this.chkInvertWheel.Name = "chkInvertWheel";
			this.chkInvertWheel.Size = new System.Drawing.Size(81, 17);
			this.chkInvertWheel.TabIndex = 6;
			this.chkInvertWheel.Text = "Invert zoom";
			this.chkInvertWheel.UseVisualStyleBackColor = true;
			// 
			// chkAttrAutoApply
			// 
			this.chkAttrAutoApply.Location = new System.Drawing.Point(16, 87);
			this.chkAttrAutoApply.Name = "chkAttrAutoApply";
			this.chkAttrAutoApply.Size = new System.Drawing.Size(288, 17);
			this.chkAttrAutoApply.TabIndex = 5;
			this.chkAttrAutoApply.Text = "Automatically apply chip attribute changes";
			// 
			// lblScrollValue
			// 
			this.lblScrollValue.Location = new System.Drawing.Point(344, 156);
			this.lblScrollValue.Name = "lblScrollValue";
			this.lblScrollValue.Size = new System.Drawing.Size(48, 17);
			this.lblScrollValue.TabIndex = 4;
			this.lblScrollValue.Text = "0";
			// 
			// label12
			// 
			this.label12.Location = new System.Drawing.Point(24, 121);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(280, 18);
			this.label12.TabIndex = 3;
			this.label12.Text = "Camera Auto-Tracking slowness";
			this.label12.Click += new System.EventHandler(this.label12_Click);
			// 
			// trkScrollFrame
			// 
			this.trkScrollFrame.LargeChange = 10;
			this.trkScrollFrame.Location = new System.Drawing.Point(48, 147);
			this.trkScrollFrame.Maximum = 100;
			this.trkScrollFrame.Name = "trkScrollFrame";
			this.trkScrollFrame.Size = new System.Drawing.Size(280, 45);
			this.trkScrollFrame.TabIndex = 2;
			this.trkScrollFrame.TickFrequency = 5;
			this.trkScrollFrame.ValueChanged += new System.EventHandler(this.trkScrollFrame_ValueChanged);
			// 
			// chkUnbisibleUnselectable
			// 
			this.chkUnbisibleUnselectable.Location = new System.Drawing.Point(16, 52);
			this.chkUnbisibleUnselectable.Name = "chkUnbisibleUnselectable";
			this.chkUnbisibleUnselectable.Size = new System.Drawing.Size(312, 17);
			this.chkUnbisibleUnselectable.TabIndex = 1;
			this.chkUnbisibleUnselectable.Text = "Make hidden cowls and frames unselectable";
			// 
			// chkAttrCopy
			// 
			this.chkAttrCopy.Location = new System.Drawing.Point(16, 17);
			this.chkAttrCopy.Name = "chkAttrCopy";
			this.chkAttrCopy.Size = new System.Drawing.Size(440, 18);
			this.chkAttrCopy.TabIndex = 0;
			this.chkAttrCopy.Text = "When adding same chip as derived chip. Copy atributes other than angle of parent";
			this.chkAttrCopy.CheckedChanged += new System.EventHandler(this.chkAttrCopy_CheckedChanged);
			// 
			// btnOK
			// 
			this.btnOK.Location = new System.Drawing.Point(380, 316);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(80, 26);
			this.btnOK.TabIndex = 1;
			this.btnOK.Text = "OK";
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// btnApply
			// 
			this.btnApply.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnApply.Location = new System.Drawing.Point(294, 316);
			this.btnApply.Name = "btnApply";
			this.btnApply.Size = new System.Drawing.Size(80, 26);
			this.btnApply.TabIndex = 2;
			this.btnApply.Text = "Apply";
			this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(466, 316);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(80, 26);
			this.btnCancel.TabIndex = 3;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// chkColorCopy
			// 
			this.chkColorCopy.Location = new System.Drawing.Point(290, 51);
			this.chkColorCopy.Name = "chkColorCopy";
			this.chkColorCopy.Size = new System.Drawing.Size(211, 18);
			this.chkColorCopy.TabIndex = 10;
			this.chkColorCopy.Text = "When adding chip, use parent color";
			// 
			// ConfigForm
			// 
			this.AcceptButton = this.btnOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(553, 354);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnApply);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.tabControl1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ConfigForm";
			this.ShowInTaskbar = false;
			this.Text = "Settings";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.frmConfig_Closing);
			this.Load += new System.EventHandler(this.frmConfig_Load);
			this.tabControl1.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.groupBox4.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.clrNorth)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.clrWest)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.clrEast)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.clrSouth)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.clrCursorB)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.clrCursorF)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.clrBack)).EndInit();
			this.groupBox3.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.clrZn)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.clrZp)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.clrYn)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.clrYp)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.clrXn)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.clrXp)).EndInit();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.clrWeightGuide)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.clrWeight)).EndInit();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.tabPage2.ResumeLayout(false);
			this.tabPage2.PerformLayout();
			this.groupBox5.ResumeLayout(false);
			this.groupBox5.PerformLayout();
			this.tabPage3.ResumeLayout(false);
			this.tabPage3.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.trkScrollFrame)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		private void frmConfig_Load(object sender, System.EventArgs e) {
			chkBaloonSwell.Checked = optDraw.BaloonSwelling;
			chkShowCowl.Checked = optDraw.ShowCowl;
			chkShowGhost.Checked = optDraw.FrameGhostShow;
			chkShowWeight.Checked = optDraw.WeightEnable;
			chkShowWeightGuide.Checked = optDraw.WeightBallEnable;
			chkXAxis.Checked = optDraw.XAxisEnable;
			chkYAxis.Checked = optDraw.YAxisEnable;
			chkZAxis.Checked = optDraw.ZAxisEnable;
			chkXNegAxis.Checked = optDraw.XNegAxisEnable;
			chkYNegAxis.Checked = optDraw.YNegAxisEnable;
			chkZNegAxis.Checked = optDraw.ZNegAxisEnable;
			switch(optDraw.FrameGhostView){
				case 0:
					rbAlpha.Checked = true;
					break;
				case 1:
					rbFrame.Checked = true;
					break;
				case 2:
					rbGChip.Checked = true;
					break;
			}
			chkShowAlways.Checked  = optDraw.ShowGuideAlways;

			txtSwellRate.Text = optDraw.BaloonSwellingRatio.ToString();
			txtWeightAlpha.Text = optDraw.WeightBallAlpha.ToString();
			txtWeightSize.Text = optDraw.WeightBallSize.ToString();

			clrEast.BackColor = optDraw.EGuideColor;
			clrNorth.BackColor = optDraw.NGuideColor;
			clrSouth.BackColor = optDraw.SGuideColor;
			clrWeight.BackColor = optDraw.WeightColor;
			clrWest.BackColor = optDraw.WGuideColor;
			clrXn.BackColor = optDraw.XNegAxisColor;
			clrXp.BackColor = optDraw.XAxisColor;
			clrYn.BackColor = optDraw.YNegAxisColor;
			clrYp.BackColor = optDraw.YAxisColor;
			clrZn.BackColor = optDraw.ZNegAxisColor;
			clrZp.BackColor = optDraw.ZAxisColor;
			clrCursorB.BackColor = optDraw.CursorBackColor;
			clrCursorF.BackColor = optDraw.CursorFrontColor;
			clrBack.BackColor = optDraw.BackColor;

			chkCameraOrtho.Checked = optDraw.CameraOrtho;

			clrWeightGuide.BackColor = optDraw.WeightBallColor;

			chkCommaWithSpace.Checked = optOutput.CommaWithSpace;
			chkIndentBySpace.Checked = optOutput.IndentBySpace;
			chkIndentEnable.Checked = optOutput.IndentEnable;
			chkOpenBracketWithChipDefinition.Checked = optOutput.OpenBracketWithChipDefinition;
			chkPrintAllAttribute.Checked = optOutput.PrintAllAttributes;
			chkReturnEndChipBracket.Checked = optOutput.ReturnEndChipBracket;
			txtIndentSpaceNum.Text = optOutput.IndentNum.ToString();

			chkAttrCopy.Checked = optEdit.ConvertParentAttributes;
			chkUnbisibleUnselectable.Checked = optEdit.UnvisibleNotSelected;
			trkScrollFrame.Value = optEdit.ScrollFrameNum;
			chkAttrAutoApply.Checked = optEdit.AttributeAutoApply;
			chkInvertWheel.Checked = optEdit.InvertWheel;
			chkInvertRotateX.Checked = optEdit.InvertRotateX;
			chkInvertRotateY.Checked = optEdit.InvertRotateY;
			chkColorCopy.Checked = optEdit.KeepParentColor;

		}

		private void Apply(){
			buildDrawOption(null);

			buildOutputOption(null);

			buildEditOption(null);

			mainwindow.SetListBackColor();
		}

		private void buildEditOption(EditOptions target) {
			if (target == null) target = optEdit;
			target.ConvertParentAttributes = chkAttrCopy.Checked;
			target.UnvisibleNotSelected = chkUnbisibleUnselectable.Checked;
			target.ScrollFrameNum = trkScrollFrame.Value;
			target.AttributeAutoApply = chkAttrAutoApply.Checked;
			target.InvertWheel = chkInvertWheel.Checked;
			target.InvertRotateX = chkInvertRotateX.Checked;
			target.InvertRotateY = chkInvertRotateY.Checked;
			target.KeepParentColor = chkColorCopy.Checked;
		}

		private void buildDrawOption(DrawOptions target) {
			if (target == null) target = optDraw;
			target.BaloonSwelling = chkBaloonSwell.Checked;
			target.ShowCowl = chkShowCowl.Checked;
			target.FrameGhostShow = chkShowGhost.Checked;
			target.WeightEnable = chkShowWeight.Checked;
			target.WeightBallEnable = chkShowWeightGuide.Checked;
			target.XAxisEnable = chkXAxis.Checked;
			target.YAxisEnable = chkYAxis.Checked;
			target.ZAxisEnable = chkZAxis.Checked;
			target.XNegAxisEnable = chkXNegAxis.Checked;
			target.YNegAxisEnable = chkYNegAxis.Checked;
			target.ZNegAxisEnable = chkZNegAxis.Checked;
			if (rbAlpha.Checked)
				target.FrameGhostView = 0;
			else if (rbFrame.Checked)
				target.FrameGhostView = 1;
			else
				target.FrameGhostView = 2;
			target.ShowGuideAlways = chkShowAlways.Checked;

            try
            {
                target.BaloonSwellingRatio = float.Parse(txtSwellRate.Text, CultureInfo.InvariantCulture);
            } catch
			{
                target.BaloonSwellingRatio = 0.5f;
				txtSwellRate.Text = "0.5";
			}
            try
            {
                target.WeightBallAlpha = float.Parse(txtWeightAlpha.Text, CultureInfo.InvariantCulture);
            } catch
			{
                target.WeightBallAlpha = 0.5f;
				txtWeightAlpha.Text = "0.5";
			}
            try
            {
                target.WeightBallSize = float.Parse(txtWeightSize.Text, CultureInfo.InvariantCulture);
            } catch
			{
                target.WeightBallSize = 1.5f;
				txtWeightSize.Text = "1.5";

			}

			target.EGuideColor = clrEast.BackColor;
			target.NGuideColor = clrNorth.BackColor;
			target.SGuideColor = clrSouth.BackColor;
			target.WeightColor = clrWeight.BackColor;
			target.WGuideColor = clrWest.BackColor;
			target.XNegAxisColor = clrXn.BackColor;
			target.XAxisColor = clrXp.BackColor;
			target.YNegAxisColor = clrYn.BackColor;
			target.YAxisColor = clrYp.BackColor;
			target.ZNegAxisColor = clrZn.BackColor;
			target.ZAxisColor = clrZp.BackColor;
			target.CursorBackColor = clrCursorB.BackColor;
			target.CursorFrontColor = clrCursorF.BackColor;
			target.BackColor = clrBack.BackColor;
			target.CameraOrtho = chkCameraOrtho.Checked;
			target.WeightBallColor = clrWeightGuide.BackColor;
		}

		private void buildOutputOption(OutputOptions target) {
			if (target == null) target = optOutput;
			target.CommaWithSpace = chkCommaWithSpace.Checked;
			target.IndentBySpace = chkIndentBySpace.Checked;
			target.IndentEnable = chkIndentEnable.Checked;
			target.OpenBracketWithChipDefinition = chkOpenBracketWithChipDefinition.Checked;
			target.PrintAllAttributes = chkPrintAllAttribute.Checked;
			target.ReturnEndChipBracket = chkReturnEndChipBracket.Checked;
			if (!uint.TryParse(txtIndentSpaceNum.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out target.IndentNum)) {
				target.IndentNum = 2;
				txtIndentSpaceNum.Text = "2";
			}


		}
		private void ColorBox_Click(object sender, System.EventArgs e) {
			PictureBox box = (PictureBox)sender;

			dlgColor.Color = box.BackColor;
			if(dlgColor.ShowDialog() == DialogResult.OK)
				box.BackColor = dlgColor.Color;

		}

		private void txtValueInput_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e) {
			if(!char.IsNumber(e.KeyChar) && !(e.KeyChar == '.') && !char.IsControl(e.KeyChar))
				e.Handled = true;
		}

		private void btnOK_Click(object sender, System.EventArgs e) {
			Apply();
			this.Close();
		}

		private void btnApply_Click(object sender, System.EventArgs e) {
			Apply();
		}

		private void frmConfig_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
			this.Visible = false;
			e.Cancel = true;
		}

		private void chkIndentBySpace_CheckedChanged(object sender, System.EventArgs e) {
		}

		private void groupBox5_Paint(object sender, System.Windows.Forms.PaintEventArgs e) {
			updateOutputPreview(sender, e);
		}

		private void btnCancel_Click(object sender, System.EventArgs e) {
			this.Hide();
		}

		private void txtIndentSpaceNum_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e) {
			e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
		}

		private void trkScrollFrame_ValueChanged(object sender, System.EventArgs e) {
			lblScrollValue.Text = trkScrollFrame.Value.ToString();
		}

		private void updateOutputPreview(object sender, EventArgs e) {
			OutputOptions opt = new OutputOptions(true);
			buildOutputOption(opt);

			txtOutputSample.Text = chipSample.ToString(0, opt).Replace("\n", "\r\n");
		}

		private void txtIndentSpaceNum_TextChanged(object sender, EventArgs e) {
			int dummy;
			if (int.TryParse(txtIndentSpaceNum.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out dummy)) {
				updateOutputPreview(sender, e);
			}
		}

        private void rbAlpha_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void chkAttrCopy_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void chkBaloonSwell_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void chkXAxis_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void chkShowWeightGuide_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void chkShowWeight_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
