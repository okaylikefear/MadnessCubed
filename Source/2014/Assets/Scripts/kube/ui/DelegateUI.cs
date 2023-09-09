using System;

namespace kube.ui
{
	public class DelegateUI : BaseUI
	{
		public DelegateUI(DrawCall func)
		{
			if (func != null)
			{
				this._draw = func;
			}
		}

		public override void show()
		{
			if (this.onOpen != null)
			{
				this.onOpen();
			}
		}

		public override void hide()
		{
			if (this.onClose != null)
			{
				this.onClose();
			}
		}

		public DrawCall drawCall
		{
			get
			{
				return this._draw;
			}
		}

		public DrawCall onClose;

		public DrawCall onOpen;
	}
}
