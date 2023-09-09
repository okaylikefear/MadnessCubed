using System;
using UnityEngine;

namespace kube.ui
{
	public class KUI
	{
		static KUI()
		{
			KUI.BlackTx = new Texture2D(1, 1);
			KUI.BlackTx.SetPixel(0, 0, new Color(0f, 0f, 0f, 0.6f));
			KUI.BlackTx.Apply();
			KUI.AlphaTx = new Texture2D(1, 1);
			KUI.AlphaTx.SetPixel(0, 0, new Color(0f, 0f, 0f, 0f));
			KUI.AlphaTx.Apply();
		}

		public static int width
		{
			get
			{
				return Cub2UI.activeWidth;
			}
		}

		public static int height
		{
			get
			{
				return Cub2UI.activeHeight;
			}
		}

		public static void DownScale()
		{
			if (Cub2UI.activeWidth <= 0)
			{
				float num = (float)Screen.width;
				float num2 = (float)Screen.height;
				float num3 = num / 1000f;
				float num4 = num2 / 600f;
				float num5 = num / num2;
				int num6 = Mathf.RoundToInt(1000f / num5);
				if (num6 < 600)
				{
					num6 = 600;
				}
				float num7 = (float)num6 / (float)Screen.height;
				Cub2UI.activeWidth = Mathf.RoundToInt((float)Screen.width * num7);
				Cub2UI.activeHeight = Mathf.RoundToInt((float)Screen.height * num7);
			}
			float num8 = (float)Screen.width / (float)Cub2UI.activeWidth;
			KUI._scale = num8;
			Vector3 s = new Vector3(num8, num8, 1f);
			Matrix4x4 matrix = GUI.matrix;
			GUI.matrix = Matrix4x4.TRS(new Vector3(0f, 0f, 0f), Quaternion.identity, s);
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

		public static Texture2D BlackTx;

		public static Texture2D AlphaTx;

		protected static float _scale = 1f;
	}
}
