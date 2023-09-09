using System;
using UnityEngine;

namespace kube.ui
{
	public class KUI
	{
		static KUI()
		{
			KUI.BlackTx.SetPixel(0, 0, new Color(0f, 0f, 0f, 0.6f));
			KUI.BlackTx.Apply();
			KUI.AlphaTx = new Texture2D(1, 1);
			KUI.AlphaTx.SetPixel(0, 0, new Color(0f, 0f, 0f, 0f));
			KUI.AlphaTx.Apply();
		}

		public static bool LRButton(Rect rect, string text, out int dir)
		{
			dir = KUI._LRButton(rect, text);
			return dir != 0;
		}

		public static bool LRButton(Rect rect, Texture tx, out int dir)
		{
			dir = KUI._LRButton(rect, tx);
			return dir != 0;
		}

		private static int _LRButton(Rect rect, object obj)
		{
			Rect position = rect;
			GUISkin skin = GUI.skin;
			GUI.DrawTexture(rect, GUI.skin.button.normal.background);
			if (obj is string)
			{
				string text = obj as string;
				GUI.skin = Kube.ASS1.smallBlackCenterSkin;
				GUI.Label(rect, text);
			}
			else
			{
				Texture texture = obj as Texture;
				GUI.DrawTexture(new Rect(rect.x + (rect.width - (float)texture.width) * 0.5f, rect.y + (rect.height - (float)texture.height) * 0.5f, (float)texture.width, (float)texture.height), texture);
			}
			int result = 0;
			GUI.skin = Kube.ASS1.emptySkin;
			position.width = rect.width / 2f;
			if (GUI.Button(position, KUI.AlphaTx))
			{
				result = -1;
			}
			position.width = rect.width / 2f;
			position.x += position.width;
			if (GUI.Button(position, KUI.AlphaTx))
			{
				result = 1;
			}
			GUI.skin = skin;
			return result;
		}

		public static Texture2D BlackTx = new Texture2D(1, 1);

		public static Texture2D AlphaTx;
	}
}
