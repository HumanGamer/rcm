using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace rcm
{
	/// <summary>
	/// 文字列入力用簡易ダイアログ
	/// </summary>
	public class TextInputDialog : System.Windows.Forms.Form
	{
		// 許容文字に関係する文字列
		private string ignorelist = "";
		
		// ignorelistの文字群が無視対象になるフラグ。
		private bool ignoremode = false;

		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button button2;
		/// <summary>
		/// 必要なデザイナ変数です。
		/// </summary>
		private System.ComponentModel.Container components = null;

		private TextInputDialog()
		{
			//
			// Windows フォーム デザイナ サポートに必要です。
			//
			InitializeComponent();

			//
			// TODO: InitializeComponent 呼び出しの後に、コンストラクタ コードを追加してください。
			//
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
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(8, 35);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(256, 20);
            this.textBox1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Enter a string";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(208, 61);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(48, 26);
            this.button1.TabIndex = 2;
            this.button1.Text = "OK";
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button2.Location = new System.Drawing.Point(152, 61);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(48, 26);
            this.button2.TabIndex = 3;
            this.button2.Text = "Cancel";
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // TextInputDialog
            // 
            this.AcceptButton = this.button1;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this.button2;
            this.ClientSize = new System.Drawing.Size(266, 85);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "TextInputDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "String input dialog";
            this.Load += new System.EventHandler(this.dlgTextInput_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		///<summery>
		///初期文字列、説明文、入力最大長を指定して、ダイアログを発生させる。
		///maxlengthが0の場合、入力可能文字数はなし。
		///</summery>
		public static string ShowDialog(string defaulttext,string labelcaption, int maxlength){
			TextInputDialog f = new TextInputDialog();
			if(defaulttext != null)f.textBox1.Text = defaulttext;
			if(maxlength > 0)f.textBox1.MaxLength = maxlength;
			if(labelcaption != null)f.label1.Text = labelcaption;
			
			string ret = (f.ShowDialog() == DialogResult.OK) ? f.textBox1.Text : null;
			f.Dispose();
			return ret;
		}

		///<summery>
		///初期文字列、説明文、入力最大長、入力時に発生するイベントハンドラを指定して
		///ダイアログを発生させる。
		///</summery>
		public static string ShowDialog(string defaulttext,string labelcaption, int maxlength,KeyPressEventHandler keypresshandler){
			TextInputDialog f = new TextInputDialog();
			if(defaulttext != null)f.textBox1.Text = defaulttext;
			if(maxlength > 0)f.textBox1.MaxLength = maxlength;
			if(labelcaption != null)f.label1.Text = labelcaption;
			f.textBox1.KeyPress += keypresshandler;
			
			string ret = (f.ShowDialog() == DialogResult.OK) ? f.textBox1.Text : null;
			f.Dispose();
			return ret;

		}

		///<summery>
		///初期文字列、説明文、入力最大長、許容文字リストを指定して、ダイアログを発生させる。
		///ignoreがtrueの時、文字リストは無視対象となる。
		///</summery>
		public static string ShowDialog(string defaulttext,string labelcaption, int maxlength, string checkcharlist,bool ignore){
			TextInputDialog f = new TextInputDialog();
			if(defaulttext != null)f.textBox1.Text = defaulttext;
			if(maxlength > 0)f.textBox1.MaxLength = maxlength;
			if(labelcaption != null)f.label1.Text = labelcaption;
			f.ignorelist = checkcharlist;
			f.ignoremode = ignore;
			f.textBox1.KeyPress += new KeyPressEventHandler(f.textBox1_KeyPress);
			
			string ret = (f.ShowDialog() == DialogResult.OK) ? f.textBox1.Text : null;
			f.Dispose();
			return ret;
		}

		private void button1_Click(object sender, System.EventArgs e) {
			this.DialogResult = DialogResult.OK;
		}

		private void button2_Click(object sender, System.EventArgs e) {
			this.DialogResult = DialogResult.Cancel;
		}

		private void dlgTextInput_Load(object sender, System.EventArgs e) {
		}

		private void textBox1_KeyPress(object sender, KeyPressEventArgs e) {
			int buff = ignorelist.IndexOf(e.KeyChar);
			if(buff >= 0)
				e.Handled = ignoremode;
			else
				e.Handled = !ignoremode;
		}

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
