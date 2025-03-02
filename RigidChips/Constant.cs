using System.Globalization;

namespace RigidChips {
	/*
	public class RcHistory{
		const int HistoryListSize = 16;
		RcData rcdata;
		RcEditAction undo;
		RcEditAction redo;

		public RcHistory(RcData data){
			rcdata = data;
		}

		public void Clear(){
			redo.Previous = undo.Next = null;

			RcEditAction act = redo;
			while(act != null){
				act = redo.Next;
				act.Previous = null;
			}

			RcEditAction act = undo;
			while(act != null){
				act = undo.Previous;
				act.Next = null;
			}

			undo = redo = null;
		}

		public void RegistAction(RcEditAction action){
			redo = null;
			undo.Next = action;
			undo = action;
		}
		public void Undo(){
			if(!CanUndo)
				throw new ApplicationException("アンドゥできません。");

			undo.Undo(rcdata);
			redo = undo;
			undo = undo.Previous;
			
		}
		public void Redo(){
			if(!CanRedo)
				throw new ApplicationException("リドゥできません。");

			redo.Redo(rcdata);
			undo = redo;
			redo = redo.Next;
		}
		public bool CanUndo{
			get{return undo != null;}
		}
		public bool CanRedo{
			get{return redo != null;}
		}
	}

	public abstract class RcEditAction{
		protected RcEditAction prev;
		protected RcEditAction next;

		public RcEditAction(){
			prev = next = null;
		}

		/// <summary>
		/// 実行された処理を戻す作業を行う。
		/// </summary>
		public abstract void Undo(RcData data);
		/// <summary>
		/// 同じ動作を再度行う
		/// </summary>
		public abstract void Redo(RcData data);

		public RcEditAction Previous{
			set{prev = value;}
			get{ return prev;}
		}
		
		public RcEditAction Next{
			set{next = value;}
			get{ return next;}
		}
	}

	public class RcAddAction : RcEditAction{
		public RcAddAction(RcChipBase chip,RcChipBase to) : base(){

		}

		public override void Undo(RcData data) {
			
		}

		public override void Redo(RcData data) {

		}


	}
	public class RcDeleteAction : RcEditAction{
	}
	public class RcModifyAction : RcEditAction{
	}
	*/
	//----------------------------------------------------------------------------------//

	public struct Constant {
		float val;
		bool hex;
		public float Value { get { return val; } set { val = value; } }
		public bool IsHexagonal { get { return hex; } set { hex = value; } }

		public Constant(string expression) {
			string s = expression.Trim();

			if (string.IsNullOrEmpty(expression))
			{
				val = 0;
				hex = false;
				return;
			}
			val = Environment.ParseNumber(s);
			hex = s[0] == '#';
		}
		public Constant(float val, bool ishex) {
			this.val = val;
			this.hex = ishex;
		}

		public static implicit operator float(Constant c) {
			return c.val;
		}
		public static implicit operator Constant(float f) {
			return new Constant(f, false);
		}

		public override string ToString() {
			if (hex) {
				return "#" + ((int)val).ToString("X6", CultureInfo.InvariantCulture);
			}
			else {
				return val.ToString("#########0.########", CultureInfo.InvariantCulture);
			}
		}
	}
}
