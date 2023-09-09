using System;
using UnityEngine;

namespace kube.ui
{
	public class BoxUI : BaseUI
	{
		protected override Vector2 getMousePos()
		{
			Vector2 mousePosition = Event.current.mousePosition;
			mousePosition.x += (float)((Screen.width - this._width) / 2);
			mousePosition.y += (float)((Screen.height - this._height) / 2);
			return GUIUtility.ScreenToGUIPoint(mousePosition);
		}

		public override void draw()
		{
			this._px = (Screen.width - this._width) / 2;
			this._py = (Screen.height - this._height) / 2;
			GUI.BeginGroup(new Rect((float)this._px, (float)this._py, (float)this._width, (float)this._height));
			if (this._draw != null)
			{
				this._draw();
			}
			GUI.EndGroup();
		}

		protected void GUILabel(Rect rect, string title, int size = 22)
		{
			Color color = GUI.color;
			GUIStyle label = GUI.skin.label;
			int fontSize = label.fontSize;
			label.fontSize = size;
			GUI.color = new Color(0f, 0f, 0f, 2f);
			GUI.Label(rect, title);
			GUI.color = color;
			rect.x -= 2f;
			rect.y -= 2f;
			GUI.Label(rect, title);
			label.fontSize = fontSize;
		}

		protected void GUITitle(Rect rect, string title, int size = 22)
		{
			GUISkin skin = Kube.ASS1.sharedUIAssets.skin;
			GUIStyle style = skin.GetStyle("EPBODY");
			style.alignment = TextAnchor.UpperCenter;
			style.fontSize = size;
			GUI.color = new Color(0f, 0f, 0f, 2f);
			GUI.Label(rect, title, style);
			GUI.color = new Color(1f, 1f, 1f, 2f);
			rect.x -= 2f;
			rect.y -= 2f;
			GUI.Label(rect, title, style);
		}

		protected void GUIH1(Rect rect, string title)
		{
			if (title.Length > 10)
			{
				this.GUITitle(rect, title, 22);
				return;
			}
			GUISkin skin = Kube.ASS1.sharedUIAssets.skin;
			GUIStyle style = skin.GetStyle("TXSHADOW");
			style.alignment = TextAnchor.UpperCenter;
			GUIStyle style2 = skin.GetStyle("TXBODY");
			style2.alignment = TextAnchor.UpperCenter;
			GUI.Label(rect, title, style);
			rect.x -= 2f;
			rect.y -= 2f;
			GUI.Label(rect, title, style2);
		}

		protected int _px;

		protected int _py;

		protected int _width = 760;

		protected int _height = 600;
	}
}
