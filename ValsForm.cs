using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using RigidChips;
using System.Globalization;
namespace rcm
{
	/// <summary>
	/// 変数(Vals)設定ダイアログ。
	/// </summary>
	public class ValsForm : System.Windows.Forms.Form
	{
		ValEntryList vallist;
		int idx;
		bool Modified = false;
		DialogResult result = DialogResult.No;
		float prevValue = 0f;
		bool prevPushed = false;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.ListBox lstVals;
		private System.Windows.Forms.TextBox txtDefault;
		private System.Windows.Forms.TextBox txtMin;
		private System.Windows.Forms.TextBox txtMax;
		private System.Windows.Forms.TextBox txtStep;
		private System.Windows.Forms.CheckBox chkDisp;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox txtName;
		private System.Windows.Forms.GroupBox grpPreviewer;
		private System.Windows.Forms.Button btnPrevPush;
		private System.Windows.Forms.TextBox txtPrevStep;
		private System.Windows.Forms.VScrollBar vsbPrevBar;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label labelPrevDigit;
		private System.Windows.Forms.Timer tmrPreview;
		private System.Windows.Forms.ToolTip tpVals;
		private System.Windows.Forms.Button btnUp;
		private System.Windows.Forms.Button btnDown;
		private System.Windows.Forms.Button btnApply;
		private System.Windows.Forms.Button btnCopy;
		private Button btnDelete;
		private System.ComponentModel.IContainer components;

		public ValsForm(ValEntryList list)
		{
			//
			// Windows フォーム デザイナ サポートに必要です。
			//
			InitializeComponent();

			vallist = list;
			idx = -1;
		}

		public ValsForm(ValEntryList list, string valname) : this(list) {
			var v = Array.Find(list.List, x => x.ValName == valname);
			if (v == null) {
				v = list.Add(valname);
			}

			idx = Array.IndexOf(list.List, v);

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
			this.lstVals = new System.Windows.Forms.ListBox();
			this.txtDefault = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.txtMin = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.txtMax = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.txtStep = new System.Windows.Forms.TextBox();
			this.chkDisp = new System.Windows.Forms.CheckBox();
			this.btnCancel = new System.Windows.Forms.Button();
			this.label5 = new System.Windows.Forms.Label();
			this.txtName = new System.Windows.Forms.TextBox();
			this.grpPreviewer = new System.Windows.Forms.GroupBox();
			this.labelPrevDigit = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.vsbPrevBar = new System.Windows.Forms.VScrollBar();
			this.txtPrevStep = new System.Windows.Forms.TextBox();
			this.btnPrevPush = new System.Windows.Forms.Button();
			this.tmrPreview = new System.Windows.Forms.Timer(this.components);
			this.btnUp = new System.Windows.Forms.Button();
			this.btnDown = new System.Windows.Forms.Button();
			this.tpVals = new System.Windows.Forms.ToolTip(this.components);
			this.btnCopy = new System.Windows.Forms.Button();
			this.btnDelete = new System.Windows.Forms.Button();
			this.btnApply = new System.Windows.Forms.Button();
			this.grpPreviewer.SuspendLayout();
			this.SuspendLayout();
			// 
			// lstVals
			// 
			this.lstVals.Font = new System.Drawing.Font("MS Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.lstVals.HorizontalScrollbar = true;
			this.lstVals.IntegralHeight = false;
			this.lstVals.ItemHeight = 12;
			this.lstVals.Location = new System.Drawing.Point(8, 13);
			this.lstVals.Name = "lstVals";
			this.lstVals.Size = new System.Drawing.Size(130, 217);
			this.lstVals.TabIndex = 0;
			this.lstVals.SelectedIndexChanged += new System.EventHandler(this.lstVals_SelectedIndexChanged);
			// 
			// txtDefault
			// 
			this.txtDefault.Location = new System.Drawing.Point(208, 61);
			this.txtDefault.Name = "txtDefault";
			this.txtDefault.Size = new System.Drawing.Size(96, 20);
			this.txtDefault.TabIndex = 2;
			this.txtDefault.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.txtDefault.TextChanged += new System.EventHandler(this.ValParams_Changed);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(144, 61);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(56, 26);
			this.label1.TabIndex = 2;
			this.label1.Text = "Default";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(144, 95);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(56, 26);
			this.label2.TabIndex = 4;
			this.label2.Text = "Min";
			// 
			// txtMin
			// 
			this.txtMin.Location = new System.Drawing.Point(208, 95);
			this.txtMin.Name = "txtMin";
			this.txtMin.Size = new System.Drawing.Size(96, 20);
			this.txtMin.TabIndex = 3;
			this.txtMin.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.txtMin.TextChanged += new System.EventHandler(this.ValParams_Changed);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(144, 130);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(56, 26);
			this.label3.TabIndex = 6;
			this.label3.Text = "Max";
			// 
			// txtMax
			// 
			this.txtMax.Location = new System.Drawing.Point(208, 130);
			this.txtMax.Name = "txtMax";
			this.txtMax.Size = new System.Drawing.Size(96, 20);
			this.txtMax.TabIndex = 4;
			this.txtMax.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.txtMax.TextChanged += new System.EventHandler(this.ValParams_Changed);
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(144, 165);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(80, 26);
			this.label4.TabIndex = 8;
			this.label4.Text = "Step";
			// 
			// txtStep
			// 
			this.txtStep.Location = new System.Drawing.Point(208, 165);
			this.txtStep.Name = "txtStep";
			this.txtStep.Size = new System.Drawing.Size(96, 20);
			this.txtStep.TabIndex = 7;
			this.txtStep.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.txtStep.TextChanged += new System.EventHandler(this.ValParams_Changed);
			// 
			// chkDisp
			// 
			this.chkDisp.Location = new System.Drawing.Point(176, 199);
			this.chkDisp.Name = "chkDisp";
			this.chkDisp.Size = new System.Drawing.Size(112, 35);
			this.chkDisp.TabIndex = 9;
			this.chkDisp.Text = "Display on screen (Disp)";
			this.chkDisp.CheckedChanged += new System.EventHandler(this.ValParams_Changed);
			// 
			// btnCancel
			// 
			this.btnCancel.Location = new System.Drawing.Point(344, 246);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(80, 26);
			this.btnCancel.TabIndex = 13;
			this.btnCancel.Text = "Close";
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(144, 26);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(58, 26);
			this.label5.TabIndex = 15;
			this.label5.Text = "Variable Name";
			// 
			// txtName
			// 
			this.txtName.Location = new System.Drawing.Point(208, 26);
			this.txtName.Name = "txtName";
			this.txtName.Size = new System.Drawing.Size(96, 20);
			this.txtName.TabIndex = 1;
			this.txtName.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.tpVals.SetToolTip(this.txtName, "Leave this blank if you want to remove the variable.");
			this.txtName.TextChanged += new System.EventHandler(this.ValParams_Changed);
			this.txtName.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ValParams_KeyPress);
			// 
			// grpPreviewer
			// 
			this.grpPreviewer.Controls.Add(this.labelPrevDigit);
			this.grpPreviewer.Controls.Add(this.label6);
			this.grpPreviewer.Controls.Add(this.vsbPrevBar);
			this.grpPreviewer.Controls.Add(this.txtPrevStep);
			this.grpPreviewer.Controls.Add(this.btnPrevPush);
			this.grpPreviewer.Location = new System.Drawing.Point(310, 13);
			this.grpPreviewer.Name = "grpPreviewer";
			this.grpPreviewer.Size = new System.Drawing.Size(136, 191);
			this.grpPreviewer.TabIndex = 16;
			this.grpPreviewer.TabStop = false;
			this.grpPreviewer.Text = "Super simple operation preview";
			this.grpPreviewer.EnabledChanged += new System.EventHandler(this.grpPreviewer_EnabledChanged);
			// 
			// labelPrevDigit
			// 
			this.labelPrevDigit.Location = new System.Drawing.Point(32, 165);
			this.labelPrevDigit.Name = "labelPrevDigit";
			this.labelPrevDigit.Size = new System.Drawing.Size(80, 17);
			this.labelPrevDigit.TabIndex = 4;
			this.labelPrevDigit.Text = "##";
			this.labelPrevDigit.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(8, 104);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(48, 17);
			this.label6.TabIndex = 3;
			this.label6.Text = "Step:";
			// 
			// vsbPrevBar
			// 
			this.vsbPrevBar.Location = new System.Drawing.Point(112, 17);
			this.vsbPrevBar.Maximum = 0;
			this.vsbPrevBar.Minimum = -10000;
			this.vsbPrevBar.Name = "vsbPrevBar";
			this.vsbPrevBar.Size = new System.Drawing.Size(16, 165);
			this.vsbPrevBar.TabIndex = 2;
			this.vsbPrevBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.vsbPrevBar_Scroll);
			// 
			// txtPrevStep
			// 
			this.txtPrevStep.Location = new System.Drawing.Point(16, 121);
			this.txtPrevStep.Name = "txtPrevStep";
			this.txtPrevStep.Size = new System.Drawing.Size(56, 20);
			this.txtPrevStep.TabIndex = 1;
			this.txtPrevStep.TabStop = false;
			this.txtPrevStep.Text = "10";
			this.txtPrevStep.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// btnPrevPush
			// 
			this.btnPrevPush.Location = new System.Drawing.Point(16, 61);
			this.btnPrevPush.Name = "btnPrevPush";
			this.btnPrevPush.Size = new System.Drawing.Size(56, 26);
			this.btnPrevPush.TabIndex = 0;
			this.btnPrevPush.TabStop = false;
			this.btnPrevPush.Text = "Button";
			this.btnPrevPush.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnPrevPush_MouseDown);
			this.btnPrevPush.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnPrevPush_MouseUp);
			// 
			// tmrPreview
			// 
			this.tmrPreview.Interval = 32;
			this.tmrPreview.Tick += new System.EventHandler(this.tmrPreview_Tick);
			// 
			// btnUp
			// 
			this.btnUp.Location = new System.Drawing.Point(84, 246);
			this.btnUp.Name = "btnUp";
			this.btnUp.Size = new System.Drawing.Size(24, 26);
			this.btnUp.TabIndex = 17;
			this.btnUp.Text = "↑";
			this.tpVals.SetToolTip(this.btnUp, "up");
			this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
			// 
			// btnDown
			// 
			this.btnDown.Location = new System.Drawing.Point(114, 246);
			this.btnDown.Name = "btnDown";
			this.btnDown.Size = new System.Drawing.Size(24, 26);
			this.btnDown.TabIndex = 18;
			this.btnDown.Text = "↓";
			this.tpVals.SetToolTip(this.btnDown, "down");
			this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
			// 
			// btnCopy
			// 
			this.btnCopy.Location = new System.Drawing.Point(8, 246);
			this.btnCopy.Name = "btnCopy";
			this.btnCopy.Size = new System.Drawing.Size(24, 26);
			this.btnCopy.TabIndex = 20;
			this.btnCopy.Text = "+";
			this.tpVals.SetToolTip(this.btnCopy, "Duplicate value");
			this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
			// 
			// btnDelete
			// 
			this.btnDelete.Location = new System.Drawing.Point(38, 246);
			this.btnDelete.Name = "btnDelete";
			this.btnDelete.Size = new System.Drawing.Size(24, 26);
			this.btnDelete.TabIndex = 21;
			this.btnDelete.Text = "X";
			this.tpVals.SetToolTip(this.btnDelete, "Delete");
			this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
			// 
			// btnApply
			// 
			this.btnApply.Location = new System.Drawing.Point(192, 246);
			this.btnApply.Name = "btnApply";
			this.btnApply.Size = new System.Drawing.Size(72, 26);
			this.btnApply.TabIndex = 19;
			this.btnApply.Text = "Apply";
			this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
			// 
			// ValsForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(458, 285);
			this.Controls.Add(this.btnDelete);
			this.Controls.Add(this.btnCopy);
			this.Controls.Add(this.btnApply);
			this.Controls.Add(this.btnDown);
			this.Controls.Add(this.btnUp);
			this.Controls.Add(this.grpPreviewer);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.txtName);
			this.Controls.Add(this.txtStep);
			this.Controls.Add(this.txtMax);
			this.Controls.Add(this.txtMin);
			this.Controls.Add(this.txtDefault);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.chkDisp);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.lstVals);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ValsForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Val{...} Edit";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ValsForm_FormClosing);
			this.Load += new System.EventHandler(this.frmVals_Load);
			this.grpPreviewer.ResumeLayout(false);
			this.grpPreviewer.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		void RefreshValList(){
			lstVals.BeginUpdate();
			lstVals.Items.Clear();
			foreach(ValEntry v in vallist.List){
				lstVals.Items.Add(v.ToString());
			}
			lstVals.Items.Add("(New)");
			if(0 <= idx && idx < lstVals.Items.Count)
				lstVals.SelectedIndex = idx;
			lstVals.EndUpdate();
		}


		void AdoptValData()
		{
			if (idx == -1)
				return;
			result = DialogResult.Yes;
			Modified = false;
			ValEntry target = vallist[idx];
			if(txtName.Text == ""){
				if(target.RefCount > 0){
					if(MessageBox.Show("This variable is used elsewhere. \nTherefore, removing it may result in unintended behavior of the model.", "RefCount = " + target.RefCount,MessageBoxButtons.OKCancel) == DialogResult.Cancel)
						return;
				}
				vallist.Remove(target.ValName);
				lstVals.SelectedIndex = -1;
				RefreshValList();
				return;
			}
			try{


				target.ValName = txtName.Text;
				//target.Default = RcData.ParseNumber(txtDefault.Text);
				//target.Min = RcData.ParseNumber(txtMin.Text);
				//target.Max = RcData.ParseNumber(txtMax.Text);
				//target.Step = RcData.ParseNumber(txtStep.Text);
				target.Default = new Constant(txtDefault.Text);
				target.Min = new Constant(txtMin.Text);
				target.Max = new Constant(txtMax.Text);
				target.Step = new Constant(txtStep.Text);

				if(target.Min > target.Max){
					MessageBox.Show("Failed to set max to min \nInvalid Min/Max range", "Apply Error",MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
					target.Max = target.Min;
				}
				if (target.Min > target.Default) {
					MessageBox.Show("Default is below the min range \nSet the value within the range of min/max.", "Apply Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					target.Default = target.Min;
				}
				else if (target.Default > target.Max) {
					MessageBox.Show("Default is above the max range \nSet the value within the range of min/max.", "Apply Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					target.Default = target.Max;
				}

			}
			catch(FormatException fe){
				MessageBox.Show("Only numbers are valid.", fe.Message,MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
			}
			catch(OverflowException oe){
				MessageBox.Show("Absolute value too large.", oe.Message,MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
			}
			target.Disp = chkDisp.Checked;
			lstVals.Items[idx] = target.ToString();
		}

		private void btnAdd_Click(object sender, System.EventArgs e) {
		}

		private void frmVals_Load(object sender, System.EventArgs e) {
			RefreshValList();
			lstVals_SelectedIndexChanged(sender,e);
		}

		private void lstVals_SelectedIndexChanged(object sender, System.EventArgs e) {
			if(Modified && idx >= 0){
				AdoptValData();
			}

			int buff;
			if(lstVals.SelectedIndex == -1){
				idx = -1;
				btnApply.Enabled = txtName.Enabled = txtDefault.Enabled = txtMin.Enabled = txtMax.Enabled = txtStep.Enabled = chkDisp.Enabled = grpPreviewer.Enabled = false;
			}
			else if (lstVals.Items[lstVals.SelectedIndex] as string == "(New)")
			{
				//else if (lstVals.SelectedItem as string == "(New)") {
				// 新規追加
				ValEntry NewVal = vallist.Add("val" + (vallist.Count + 1).ToString());
				if (NewVal == null) {
					lstVals.SelectedIndex = -1;
					return;
				}
				//string name = dlgTextInput.ShowDialog("val" + vallist.Count, "新しい変数の名前を入力してください", 0, @" ""!#$%&'()-=^~|\[]{}`@:*;+,<.>/?", true);
				//if (name != null)
				int tmp = lstVals.SelectedIndex;
				RefreshValList();
				lstVals.SelectedIndex = tmp;

				txtName.Focus();
			}
			else {
				btnApply.Enabled = txtName.Enabled = txtDefault.Enabled = txtMin.Enabled = txtMax.Enabled = txtStep.Enabled = chkDisp.Enabled = grpPreviewer.Enabled = true;
				idx = lstVals.SelectedIndex;
				txtName.Text = vallist[idx].ValName;
				txtDefault.Text = vallist[idx].Default.ToString();
				txtMin.Text = vallist[idx].Min.ToString();
				txtMax.Text = vallist[idx].Max.ToString();
				txtStep.Text = vallist[idx].Step.ToString();
				chkDisp.Checked = vallist[idx].Disp;

				prevValue = vallist[idx].Default;
				if (prevValue > vallist[idx].Max)
					prevValue = vallist[idx].Max;
				else if (prevValue < vallist[idx].Min)
					prevValue = vallist[idx].Min;
				if (float.IsInfinity(vallist[idx].Max) || float.IsInfinity(vallist[idx].Min) || float.IsInfinity(vallist[idx].Default) || float.IsInfinity(vallist[idx].Step))
					vsbPrevBar.Enabled = grpPreviewer.Enabled = false;
				else {
					vsbPrevBar.Enabled = grpPreviewer.Enabled = true;
					buff = -(int)((prevValue - vallist[idx].Min) * 10000 / (vallist[idx].Max - vallist[idx].Min));
					if (buff > vsbPrevBar.Maximum) buff = vsbPrevBar.Maximum;
					if (buff < vsbPrevBar.Minimum) buff = vsbPrevBar.Minimum;
					vsbPrevBar.Value = buff;
				}
			}


			
		}

		private void btnUp_Click(object sender, System.EventArgs e) {
			if(lstVals.SelectedIndex <= 0)return;
			AdoptValData();

			vallist.Swap(
				vallist[idx],
				vallist[idx - 1]   );

			RefreshValList();

			lstVals.SelectedIndex = idx-1;
		}

		private void btnDown_Click(object sender, System.EventArgs e) {
			if(lstVals.SelectedIndex == lstVals.Items.Count - 1)return;

			if (idx == -1)
				return;

			AdoptValData();

			vallist.Swap(
				vallist[idx],
				vallist[idx + 1]   );

			RefreshValList();

			lstVals.SelectedIndex = idx+1;

		}

		private void ValParams_Changed(object sender, System.EventArgs e){
			Modified = true;
		}

		private void btnCancel_Click(object sender, System.EventArgs e) {
			//if(Modified && idx >= 0){
			//	AdoptValData();
			//}
			//this.DialogResult = result;
			//this.Dispose();
			this.Close();
		}

		private void btnApply_Click(object sender, System.EventArgs e) {
			if(Modified && idx >= 0){
				AdoptValData();
			}
		}

		private void tmrPreview_Tick(object sender, System.EventArgs e) {
			try{
				if(prevPushed){
					try{
						prevValue += float.Parse(txtPrevStep.Text, CultureInfo.InvariantCulture);
					}
					catch{}
				}
				else if(prevValue > vallist[idx].Default + vallist[idx].Step){
					prevValue -= vallist[idx].Step;
				}
				else if(prevValue < vallist[idx].Default - vallist[idx].Step){
					prevValue += vallist[idx].Step;
				}
				else{
					prevValue = vallist[idx].Default;
				}
				if(prevValue > vallist[idx].Max)
					prevValue = vallist[idx].Max;
				else if(prevValue < vallist[idx].Min)
					prevValue = vallist[idx].Min;
				vsbPrevBar.Value = -(int)((prevValue - vallist[idx].Min) * 10000 / (vallist[idx].Max - vallist[idx].Min));
				labelPrevDigit.Text = prevValue.ToString("##0.000");
			}
			catch(Exception){
				grpPreviewer.Enabled = false;
			}
			
		}

		private void btnPrevPush_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e) {
			prevPushed = true;
		}

		private void btnPrevPush_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e) {
			prevPushed = false;
		}

		private void vsbPrevBar_Scroll(object sender, System.Windows.Forms.ScrollEventArgs e) {
			vsbPrevBar.Value = -(int)((prevValue - vallist[idx].Min) * 10000 / (vallist[idx].Max - vallist[idx].Min));
			prevValue = (vallist[idx].Min - vallist[idx].Max) * vsbPrevBar.Value / 10000f + vallist[idx].Min;
		}

		private void grpPreviewer_EnabledChanged(object sender, System.EventArgs e) {
			tmrPreview.Enabled = ((Control)sender).Enabled;
		}

		private void ValParams_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e) {
			if(e.KeyChar == 13)
				btnApply_Click(sender,EventArgs.Empty);
		}

		private void btnCopy_Click(object sender, System.EventArgs e) {
			if (idx == -1)
				return;
			ValEntry temp = vallist[idx];
			ValEntry temp2;
			string copyflag = "_copy";
			int count = 0;

			copystate:
			try{
				temp2 = vallist.Add(temp.ValName + copyflag);
			}
			catch{
				count++;
				copyflag = "_copy" + count.ToString();
				goto copystate;
			}
			temp2.Default = temp.Default;
			temp2.Disp = temp.Disp;
			temp2.Max = temp.Max;
			temp2.Min = temp.Min;
			temp2.RefCount = 0;
			temp2.Step = temp.Step;

			RefreshValList();
		}

		private void btnDelete_Click(object sender, EventArgs e) {
			if (idx == -1)
				return;
			ValEntry target = vallist[idx];
			if (target.RefCount > 0) {
				if (MessageBox.Show("This variable is used elsewhere. \nTherefore, removing it may result in unintended behavior of the model.", "RefCount = " + target.RefCount, MessageBoxButtons.OKCancel) == DialogResult.Cancel)
					return;
			}
			vallist.Remove(target.ValName);
			idx = -1;
			lstVals.SelectedIndex = -1;
			RefreshValList();
			return;
		}

		private void ValsForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (Modified && idx >= 0)
			{
				AdoptValData();
			}
			this.DialogResult = result;
			//this.Dispose();
		}
	}
}
