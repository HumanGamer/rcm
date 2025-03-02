using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Globalization;

namespace RigidChips {
	//----------------------------------------------------------------------------------//
	//	プロパティ群
	//	フォームに持たせ、RcDataはその参照を拾う
	//
	//	逆でもいいような気がしてきた。まぁいいか。こいつら独立してるし。

	public class DrawOptions{
		public Color BackColor = Color.Navy;
		public Color CursorFrontColor = Color.Red;
		public Color CursorBackColor = Color.LightBlue;
		public Color NGuideColor = Color.Blue;
		public Color SGuideColor = Color.Yellow;
		public Color EGuideColor = Color.Red;
		public Color WGuideColor = Color.Green;

		public bool XAxisEnable = true;
		public bool XNegAxisEnable = true;
		public bool YAxisEnable = true;
		public bool YNegAxisEnable = true;
		public bool ZAxisEnable = true;
		public bool ZNegAxisEnable = true;
		public Color XAxisColor = Color.Red;
		public Color YAxisColor = Color.Green;
		public Color ZAxisColor = Color.Blue;
		public Color XNegAxisColor = Color.Cyan;
		public Color YNegAxisColor = Color.Magenta;
		public Color ZNegAxisColor = Color.Yellow;

		public bool ShowCowl = true;
		public int FrameGhostView = 0;
		public bool FrameGhostShow = true;
		public bool BaloonSwelling = true;
		public float BaloonSwellingRatio = 0.5f;

		public bool WeightEnable = true;
		public Color WeightColor = Color.White;
		public bool WeightBallEnable = true;
		public float WeightBallSize = 1.5f;
		public float WeightBallAlpha = 0.5f;
		public bool ShowGuideAlways = false;

		public bool CameraOrtho = false;

		public Color WeightBallColor = Color.Black;

		public bool AutoCamera = true;

		public DrawOptions(){
			try{
				System.IO.StreamReader file = new StreamReader(Application.StartupPath + "\\draw.cfg");

				BackColor = Color.FromArgb(int.Parse(file.ReadLine(),System.Globalization.NumberStyles.Number, CultureInfo.InvariantCulture));
				CursorFrontColor = Color.FromArgb(int.Parse(file.ReadLine(),System.Globalization.NumberStyles.Number, CultureInfo.InvariantCulture));
				CursorBackColor = Color.FromArgb(int.Parse(file.ReadLine(),System.Globalization.NumberStyles.Number, CultureInfo.InvariantCulture));
				NGuideColor = Color.FromArgb(int.Parse(file.ReadLine(),System.Globalization.NumberStyles.Number, CultureInfo.InvariantCulture));
				SGuideColor = Color.FromArgb(int.Parse(file.ReadLine(),System.Globalization.NumberStyles.Number, CultureInfo.InvariantCulture));
				EGuideColor = Color.FromArgb(int.Parse(file.ReadLine(),System.Globalization.NumberStyles.Number, CultureInfo.InvariantCulture));
				WGuideColor = Color.FromArgb(int.Parse(file.ReadLine(),System.Globalization.NumberStyles.Number, CultureInfo.InvariantCulture));
				XAxisColor = Color.FromArgb(int.Parse(file.ReadLine(),System.Globalization.NumberStyles.Number, CultureInfo.InvariantCulture));
				YAxisColor = Color.FromArgb(int.Parse(file.ReadLine(),System.Globalization.NumberStyles.Number, CultureInfo.InvariantCulture));
				ZAxisColor = Color.FromArgb(int.Parse(file.ReadLine(),System.Globalization.NumberStyles.Number, CultureInfo.InvariantCulture));
				XNegAxisColor = Color.FromArgb(int.Parse(file.ReadLine(),System.Globalization.NumberStyles.Number, CultureInfo.InvariantCulture));
				YNegAxisColor = Color.FromArgb(int.Parse(file.ReadLine(),System.Globalization.NumberStyles.Number, CultureInfo.InvariantCulture));
				ZNegAxisColor = Color.FromArgb(int.Parse(file.ReadLine(),System.Globalization.NumberStyles.Number, CultureInfo.InvariantCulture));
				WeightColor = Color.FromArgb(int.Parse(file.ReadLine(),System.Globalization.NumberStyles.Number, CultureInfo.InvariantCulture));
				ShowCowl = bool.Parse(file.ReadLine());
				FrameGhostShow = bool.Parse(file.ReadLine());
				string s = file.ReadLine();
				try{
					FrameGhostView = int.Parse(s, CultureInfo.InvariantCulture);
				}
				catch(FormatException){
					FrameGhostView = bool.Parse(s) ? 0 : 1;
				}
				BaloonSwelling = bool.Parse(file.ReadLine());
				ShowGuideAlways = bool.Parse(file.ReadLine());
				XAxisEnable = bool.Parse(file.ReadLine());
				YAxisEnable = bool.Parse(file.ReadLine());
				ZAxisEnable = bool.Parse(file.ReadLine());
				XNegAxisEnable = bool.Parse(file.ReadLine());
				YNegAxisEnable = bool.Parse(file.ReadLine());
				ZNegAxisEnable = bool.Parse(file.ReadLine());
				WeightEnable = bool.Parse(file.ReadLine());
				WeightBallEnable = bool.Parse(file.ReadLine());

				BaloonSwellingRatio = float.Parse(file.ReadLine(),System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture);
				WeightBallSize = float.Parse(file.ReadLine(),System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture);
				WeightBallAlpha = float.Parse(file.ReadLine(),System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture);
				
				CameraOrtho = bool.Parse(file.ReadLine());

				WeightBallColor = Color.FromArgb(int.Parse(file.ReadLine(),System.Globalization.NumberStyles.Number, CultureInfo.InvariantCulture));

				AutoCamera = bool.Parse(file.ReadLine());

				file.Close();
			}
			catch{}
		}

		~DrawOptions(){
			StreamWriter file = new StreamWriter(Application.StartupPath + "\\draw.cfg");

			file.WriteLine(BackColor.ToArgb().ToString());
			file.WriteLine(CursorFrontColor.ToArgb().ToString());
			file.WriteLine(CursorBackColor.ToArgb().ToString());
			file.WriteLine(NGuideColor.ToArgb().ToString());
			file.WriteLine(SGuideColor.ToArgb().ToString());
			file.WriteLine(EGuideColor.ToArgb().ToString());
			file.WriteLine(WGuideColor.ToArgb().ToString());
			file.WriteLine(XAxisColor.ToArgb().ToString());
			file.WriteLine(YAxisColor.ToArgb().ToString());
			file.WriteLine(ZAxisColor.ToArgb().ToString());
			file.WriteLine(XNegAxisColor.ToArgb().ToString());
			file.WriteLine(YNegAxisColor.ToArgb().ToString());
			file.WriteLine(ZNegAxisColor.ToArgb().ToString());
			file.WriteLine(WeightColor.ToArgb().ToString());
			file.WriteLine(ShowCowl.ToString());
			file.WriteLine(FrameGhostShow.ToString());
			file.WriteLine(FrameGhostView.ToString());
			file.WriteLine(BaloonSwelling.ToString());
			file.WriteLine(ShowGuideAlways.ToString());
			file.WriteLine(XAxisEnable.ToString());
			file.WriteLine(YAxisEnable.ToString());
			file.WriteLine(ZAxisEnable.ToString());
			file.WriteLine(XNegAxisEnable.ToString());
			file.WriteLine(YNegAxisEnable.ToString());
			file.WriteLine(ZNegAxisEnable.ToString());
			file.WriteLine(WeightEnable.ToString());
			file.WriteLine(WeightBallEnable.ToString());

			file.WriteLine(BaloonSwellingRatio.ToString());
			file.WriteLine(WeightBallSize.ToString());
			file.WriteLine(WeightBallAlpha.ToString());

			file.WriteLine(CameraOrtho.ToString());

			file.WriteLine(WeightBallColor.ToArgb().ToString());

			file.WriteLine(AutoCamera.ToString());

			file.Close();
		}


	}
}
