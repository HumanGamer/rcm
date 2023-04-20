using System.Diagnostics;

namespace RigidChips {
	/// <summary>
	/// 変数データ
	/// </summary>
	public class ValEntry{
		public ValEntryList List;
		public string ValName;
		public Constant Default;
		public Constant Min;
		public Constant Max;
		public Constant Step;
		public bool Disp;
		public bool OnPreview {
			get;
			set;
		}

		float now;
		public float Now{
			get {
				if (OnPreview) {
					ApplyDefault();
					return now;
				}
				else {
					return Default.Value;
				}
			}
			set { now = value; }
		}

		public int RefCount;

		public ValEntry(){
			Default = Min = Step = 0f;
			Max = 1;// float.PositiveInfinity;
			Disp = true;
			RefCount = 0;
		}

		public ValEntry(string name)
			: this() {
			ValName = name;
		}

		/// <summary>
		/// Val項目の文字列を作成
		/// </summary>
		/// <returns>書式化された Val ブロックの項目</returns>
		public override string ToString() {
			string buff = "";
			buff += ValName + "(";
			if(Default != DefaultDefaultValue)buff += "default=" + Default.ToString() + ",";
			if(Min != DefaultMinValue)buff += "min=" + Min.ToString() + ",";
			if(Max != DefaultMaxValue)buff += "max=" + Max.ToString() + ",";
			if(Step != DefaultStepValue)buff += "step=" + Step.ToString() + ",";
			if(!Disp)buff += "disp=0";
			buff = buff.TrimEnd(',');
			buff += ")";
			return buff;
		}

		public const float DefaultDefaultValue = 0f;
		public const float DefaultMinValue = 0f;
		public const float DefaultMaxValue = float.PositiveInfinity;
		public const float DefaultStepValue = 0f;

		bool ticked = false;
		public bool Ticked {
			get {
				return ticked;
			}
			set {
				ticked = value;
				Debug.WriteLine(string.Format("{0} : {1}", this.ValName, value), "Ticked");
			}
		}
		bool steped;
		bool Steped {
			get { return steped; }
			set {
				steped = value;
				Debug.WriteLine(string.Format("{0} : {1}", this.ValName, value), "Steped");
			}
		}
		public float Tick(float StepValue) {
			Steped = true;
			float newval = now + StepValue;
			if (newval > Max)
				newval = Max;
			else if (newval < Min)
				newval = Min;

			return now = newval;
		}

		public void ApplyDefault() {
			if (Ticked) {
				if (!Steped) {
					Debug.WriteLine(this.ValName, "Default Applied");
					if (now < Default - Step) {
						now += Step;
					}
					else if (now > Default + Step) {
						now -= Step;
					}
					else {
						now = Default;
					}
				}
				Ticked = false;
			}
			Steped = false;
		}
	}
}
