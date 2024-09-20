using System;
using System.Reflection;
using UnityEngine;

namespace kube.ui
{
	public class BaseUI
	{
		public BaseUI()
		{
			MethodInfo method = base.GetType().GetMethod("OnGUI", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (method != null)
			{
				this._draw = (DrawCall)Delegate.CreateDelegate(typeof(DrawCall), this, method, false);
			}
		}

		public virtual void show()
		{
		}

		public virtual void hide()
		{
		}

		public virtual void draw()
		{
			if (this._draw != null)
			{
				this._draw();
			}
		}

		protected virtual Vector2 getMousePos()
		{
			Vector2 mousePosition = Event.current.mousePosition;
			return GUIUtility.ScreenToGUIPoint(mousePosition);
		}

		public bool popup;

		protected DrawCall _draw;

		public bool canClose = true;
	}
}
