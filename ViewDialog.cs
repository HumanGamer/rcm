using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace rcm
{
	/// <summary>
	/// 視点を直接値で設定するためのダイアログ、の予定でした。
	/// </summary>
	public class ViewDialog : System.Windows.Forms.Form
	{
		private System.Windows.Forms.TextBox txtTheta;
		private System.Windows.Forms.TextBox txtPhi;
		private System.Windows.Forms.TextBox txtDepth;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Label label4;
		/// <summary>
		/// 必要なデザイナ変数です。
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ViewDialog()
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
            this.txtTheta = new System.Windows.Forms.TextBox();
            this.txtPhi = new System.Windows.Forms.TextBox();
            this.txtDepth = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txtTheta
            // 
            this.txtTheta.Location = new System.Drawing.Point(111, 17);
            this.txtTheta.Name = "txtTheta";
            this.txtTheta.Size = new System.Drawing.Size(56, 20);
            this.txtTheta.TabIndex = 0;
            this.txtTheta.Text = "textBox1";
            // 
            // txtPhi
            // 
            this.txtPhi.Location = new System.Drawing.Point(111, 43);
            this.txtPhi.Name = "txtPhi";
            this.txtPhi.Size = new System.Drawing.Size(56, 20);
            this.txtPhi.TabIndex = 1;
            this.txtPhi.Text = "textBox2";
            // 
            // txtDepth
            // 
            this.txtDepth.Location = new System.Drawing.Point(111, 93);
            this.txtDepth.Name = "txtDepth";
            this.txtDepth.Size = new System.Drawing.Size(56, 20);
            this.txtDepth.TabIndex = 2;
            this.txtDepth.Text = "textBox3";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(16, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 18);
            this.label1.TabIndex = 3;
            this.label1.Text = "Angle of rotation";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(16, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 18);
            this.label2.TabIndex = 4;
            this.label2.Text = "Vert. Angle";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(16, 95);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(88, 18);
            this.label3.TabIndex = 5;
            this.label3.Text = "Camera Distance";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(40, 130);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(64, 26);
            this.button1.TabIndex = 6;
            this.button1.Text = "OK";
            // 
            // label4
            // 
            this.label4.Enabled = false;
            this.label4.Location = new System.Drawing.Point(48, 69);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(96, 18);
            this.label4.TabIndex = 7;
            this.label4.Text = "↑Both are radians";
            // 
            // ViewDialog
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(194, 151);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtDepth);
            this.Controls.Add(this.txtPhi);
            this.Controls.Add(this.txtTheta);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ViewDialog";
            this.Text = "Viewpoint input";
            this.Load += new System.EventHandler(this.dlgView_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		private void dlgView_Load(object sender, System.EventArgs e) {
		
		}
	}
}
