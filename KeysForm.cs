using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using RigidChips;
using System.Text;
using System.Globalization;

namespace rcm {
	/// <summary>
	/// キー設定ダイアログ。使いづらいことで有名(　´_ゝ｀)
	/// </summary>
	public class KeysForm : System.Windows.Forms.Form {
		RigidChips.Environment data;
		ValEntryList vallist;
		KeyEntryList keylist;
		DialogResult result = DialogResult.No;
		private System.Windows.Forms.ListBox lstKeyList;
		private System.Windows.Forms.ListBox lstValList;
		private System.Windows.Forms.Button btnClose;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ComboBox cmbVals;
		private System.Windows.Forms.TextBox txtStep;
		private System.Windows.Forms.Button btnAdd;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label labelValDefault;
		private System.Windows.Forms.Label labelValMin;
		private System.Windows.Forms.Label labelValMax;
		private System.Windows.Forms.Label labelValStep;
		private System.Windows.Forms.Button btnDelete;
		private Label label7;
		/// <summary>
		/// 必要なデザイナ変数です。
		/// </summary>
		private System.ComponentModel.Container components = null;

		public KeysForm(RigidChips.Environment rcdata) {
			//
			// Windows フォーム デザイナ サポートに必要です。
			//
			InitializeComponent();

			data = rcdata;
			vallist = rcdata.vals;
			keylist = rcdata.keys;

			UpdateCmbVals();
			UpdateKeyDescriptions();

			lstValList.Enabled = false;
		}
		private void UpdateKeyDescriptions() {
			UpdateKeyDescriptions(false);
		}

		private void UpdateKeyDescriptions(bool keepIndex) {
			string[] str = {
							   "[↑]","[↓]","[←]","[→]",
                               "[Z]","[X]","[C]","[A]","[S]","[D]",
                               "[V]","[B]","[F]",
							   "[G]","[Q]",
                               "[W]","[E]"
                           };
			int index = lstKeyList.SelectedIndex;
			lstKeyList.SuspendLayout();
			lstKeyList.Items.Clear();
			for (int i = 0; i < str.Length; i++) {
				var key = keylist[i];
				StringBuilder sb = new StringBuilder();
				//bool flag = false;
				//foreach (var kw in key.Works) {
				//    if (flag) sb.Append(", "); else sb.Append("\t: ");
				//    sb.Append(kw.Target.ValName);
				//    flag = true;
				//}
				sb.Append(key.ToString());

				lstKeyList.Items.Add(string.Format("{0} {1}", str[i], sb.ToString()));
			}

			if (keepIndex) lstKeyList.SelectedIndex = index;
			lstKeyList.ResumeLayout();
		}

		private void UpdateCmbVals() {
			cmbVals.SuspendLayout();
			cmbVals.Items.Clear();
			foreach (ValEntry v in vallist.List) {
				cmbVals.Items.Add(v);
			}
			cmbVals.Items.Add("(Val編集...)");
			cmbVals.ResumeLayout();
		}

		/// <summary>
		/// 使用されているリソースに後処理を実行します。
		/// </summary>
		protected override void Dispose( bool disposing ) {
			if( disposing ) {
				if(components != null) {
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
		private void InitializeComponent() {
            this.lstKeyList = new System.Windows.Forms.ListBox();
            this.lstValList = new System.Windows.Forms.ListBox();
            this.cmbVals = new System.Windows.Forms.ComboBox();
            this.txtStep = new System.Windows.Forms.TextBox();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.labelValStep = new System.Windows.Forms.Label();
            this.labelValMax = new System.Windows.Forms.Label();
            this.labelValMin = new System.Windows.Forms.Label();
            this.labelValDefault = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnDelete = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lstKeyList
            // 
            this.lstKeyList.Font = new System.Drawing.Font("MS Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lstKeyList.IntegralHeight = false;
            this.lstKeyList.ItemHeight = 12;
            this.lstKeyList.Location = new System.Drawing.Point(8, 9);
            this.lstKeyList.Name = "lstKeyList";
            this.lstKeyList.Size = new System.Drawing.Size(120, 225);
            this.lstKeyList.TabIndex = 0;
            this.lstKeyList.SelectedIndexChanged += new System.EventHandler(this.lstKeyList_SelectedIndexChanged);
            // 
            // lstValList
            // 
            this.lstValList.Location = new System.Drawing.Point(144, 26);
            this.lstValList.Name = "lstValList";
            this.lstValList.Size = new System.Drawing.Size(136, 43);
            this.lstValList.TabIndex = 1;
            this.lstValList.SelectedIndexChanged += new System.EventHandler(this.lstValList_SelectedIndexChanged);
            this.lstValList.EnabledChanged += new System.EventHandler(this.lstValList_EnabledChanged);
            // 
            // cmbVals
            // 
            this.cmbVals.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbVals.Location = new System.Drawing.Point(178, 113);
            this.cmbVals.Name = "cmbVals";
            this.cmbVals.Size = new System.Drawing.Size(102, 21);
            this.cmbVals.TabIndex = 2;
            this.cmbVals.SelectedIndexChanged += new System.EventHandler(this.cmbVals_SelectedIndexChanged);
            // 
            // txtStep
            // 
            this.txtStep.Location = new System.Drawing.Point(178, 141);
            this.txtStep.Name = "txtStep";
            this.txtStep.Size = new System.Drawing.Size(102, 20);
            this.txtStep.TabIndex = 3;
            this.txtStep.TextChanged += new System.EventHandler(this.txtStep_TextChanged);
            this.txtStep.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtStep_KeyDown);
            // 
            // btnAdd
            // 
            this.btnAdd.Enabled = false;
            this.btnAdd.Location = new System.Drawing.Point(178, 173);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(48, 26);
            this.btnAdd.TabIndex = 4;
            this.btnAdd.Text = "Apply";
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(356, 208);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(60, 26);
            this.btnClose.TabIndex = 6;
            this.btnClose.Text = "Close";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(144, 144);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(28, 17);
            this.label1.TabIndex = 6;
            this.label1.Text = "Step";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(144, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(112, 17);
            this.label3.TabIndex = 8;
            this.label3.Text = "Registered Variables";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.labelValStep);
            this.groupBox1.Controls.Add(this.labelValMax);
            this.groupBox1.Controls.Add(this.labelValMin);
            this.groupBox1.Controls.Add(this.labelValDefault);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(288, 9);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(128, 190);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Variable Details";
            // 
            // labelValStep
            // 
            this.labelValStep.Location = new System.Drawing.Point(48, 165);
            this.labelValStep.Name = "labelValStep";
            this.labelValStep.Size = new System.Drawing.Size(72, 17);
            this.labelValStep.TabIndex = 7;
            this.labelValStep.Text = "(No info)";
            this.labelValStep.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // labelValMax
            // 
            this.labelValMax.Location = new System.Drawing.Point(48, 121);
            this.labelValMax.Name = "labelValMax";
            this.labelValMax.Size = new System.Drawing.Size(72, 18);
            this.labelValMax.TabIndex = 6;
            this.labelValMax.Text = "(No info)";
            this.labelValMax.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // labelValMin
            // 
            this.labelValMin.Location = new System.Drawing.Point(48, 78);
            this.labelValMin.Name = "labelValMin";
            this.labelValMin.Size = new System.Drawing.Size(72, 17);
            this.labelValMin.TabIndex = 5;
            this.labelValMin.Text = "(No info)";
            this.labelValMin.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // labelValDefault
            // 
            this.labelValDefault.Location = new System.Drawing.Point(48, 35);
            this.labelValDefault.Name = "labelValDefault";
            this.labelValDefault.Size = new System.Drawing.Size(72, 17);
            this.labelValDefault.TabIndex = 4;
            this.labelValDefault.Text = "(No info)";
            this.labelValDefault.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(8, 147);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(88, 18);
            this.label6.TabIndex = 3;
            this.label6.Text = "Step";
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(8, 104);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(88, 17);
            this.label5.TabIndex = 2;
            this.label5.Text = "Max";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(8, 61);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(88, 17);
            this.label4.TabIndex = 1;
            this.label4.Text = "Min";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(8, 17);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(88, 18);
            this.label2.TabIndex = 0;
            this.label2.Text = "Default";
            // 
            // btnDelete
            // 
            this.btnDelete.Enabled = false;
            this.btnDelete.Location = new System.Drawing.Point(232, 173);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(48, 26);
            this.btnDelete.TabIndex = 5;
            this.btnDelete.Text = "Delete";
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(144, 113);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(28, 17);
            this.label7.TabIndex = 6;
            this.label7.Text = "Val";
            // 
            // KeysForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(424, 244);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.txtStep);
            this.Controls.Add(this.cmbVals);
            this.Controls.Add(this.lstValList);
            this.Controls.Add(this.lstKeyList);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "KeysForm";
            this.ShowInTaskbar = false;
            this.Text = "Key{...) Edit";
            this.Load += new System.EventHandler(this.frmKeys_Load);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		private void frmKeys_Load(object sender, System.EventArgs e) {
		}

		private void btnClose_Click(object sender, System.EventArgs e) {
			this.DialogResult = result;
			this.Dispose();
		}

		private void lstKeyList_SelectedIndexChanged(object sender, System.EventArgs e) {
			cmbVals.SelectedIndex = -1;
			if(lstKeyList.SelectedIndex < 0){
				lstValList.Enabled = false;
				return;
			}
			lstValList.Enabled = true;
			lstValList.Items.Clear();
			foreach(KeyEntry.KeyEntryWork w in keylist[lstKeyList.SelectedIndex].Works){
				if(w.Target.RefCount < 0){
					keylist[lstKeyList.SelectedIndex].DeleteWork(w.Target);
				}
				else
					lstValList.Items.Add(w.ToString());
			}
			lstValList.Items.Add("(New)");
			lstValList_SelectedIndexChanged(sender,e);
			txtStep.Text = "0";
		}

		private void btnAdd_Click(object sender, System.EventArgs e) {
			if(cmbVals.SelectedIndex < 0)return;
			if (cmbVals.SelectedItem.ToString() == "(ValEdit...)") return;
			try{
				keylist[lstKeyList.SelectedIndex].AssignWork((ValEntry)cmbVals.SelectedItem,float.Parse(txtStep.Text, CultureInfo.InvariantCulture));
			}
			catch(FormatException fe){
				if(txtStep.Text != "")
					MessageBox.Show(fe.Message);
				return;
			}
			UpdateKeyDescriptions(true);
			result = DialogResult.Yes;
		}

		private void lstValList_SelectedIndexChanged(object sender, System.EventArgs e) {
			if(lstValList.SelectedIndex < 0 || lstValList.SelectedItem as string == "(New)"){
				btnDelete.Enabled = false;
				return;
			}
			btnDelete.Enabled = true;
			foreach(object i in cmbVals.Items){
				if(((ValEntry)i).ValName == lstValList.SelectedItem.ToString().Split('(')[0]){
					cmbVals.SelectedItem = i;
					break;
				}
			}

			txtStep.Text = lstValList.SelectedItem.ToString().Split(')','=')[1];
		}

		private void lstValList_EnabledChanged(object sender, System.EventArgs e) {
			lstValList.BackColor = lstValList.Enabled ? Color.FromKnownColor(KnownColor.Window) : Color.FromKnownColor(KnownColor.InactiveCaption);
			txtStep.Enabled = cmbVals.Enabled = lstValList.Enabled;
		}

		private void cmbVals_SelectedIndexChanged(object sender, System.EventArgs e) {
			if(cmbVals.SelectedIndex < 0){
				labelValDefault.Text = labelValMin.Text = labelValMax.Text = labelValStep.Text = "(No information)";
				btnAdd.Enabled = false;
			}
			else if(cmbVals.SelectedItem as string == "(ValEdit...)"){
				ValsForm valform = new ValsForm( data.vals);
				bool val = (valform.ShowDialog() == DialogResult.Yes);
				if (val) {
					result = DialogResult.Yes;
				}
				UpdateCmbVals();
				cmbVals.SelectedIndex = -1;
			}
			else{
				labelValDefault.Text = vallist[cmbVals.SelectedIndex].Default.ToString();
				labelValMin.Text = vallist[cmbVals.SelectedIndex].Min.ToString();
				labelValMax.Text = vallist[cmbVals.SelectedIndex].Max.ToString();
				labelValStep.Text = vallist[cmbVals.SelectedIndex].Step.ToString();
				btnAdd.Enabled = true;
			}
		}

		private void btnDelete_Click(object sender, System.EventArgs e) {
			if(lstValList.SelectedIndex < 0)return;
			keylist[lstKeyList.SelectedIndex].DeleteWork(keylist[lstKeyList.SelectedIndex].Works[lstValList.SelectedIndex].Target);
			UpdateKeyDescriptions(true);
			lstKeyList_SelectedIndexChanged(sender,e);
			result = DialogResult.Yes;
		}

		private void txtStep_TextChanged(object sender, EventArgs e) {

		}

		private void txtStep_KeyDown(object sender, KeyEventArgs e) {
			if (e.KeyCode == Keys.Enter) {
				btnAdd_Click(sender, e);
				lstValList.Focus();
				e.Handled = true;
			}
		}
	}
}
