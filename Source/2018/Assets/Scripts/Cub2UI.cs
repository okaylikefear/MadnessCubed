using System;
using kube;
using UnityEngine;

public class Cub2UI : MonoBehaviour
{
	public static T FindObject<T>() where T : UnityEngine.Object
	{
		return Cub2UI.instance.transform.GetComponentInChildren<T>(true);
	}

	public static MessageBox MessageBox(string text, EventDelegate.Callback cb = null)
	{
		MessageBox messageBox = Cub2UI.FindDialog<MessageBox>("dialog_message");
		Transform parent = messageBox.transform.parent;
		GameObject gameObject = parent.gameObject.AddChild(messageBox.gameObject);
		messageBox = gameObject.GetComponent<MessageBox>();
		messageBox.label.text = text;
		messageBox.handler = new EventDelegate(cb);
		messageBox.autoDestroy = true;
		gameObject.SetActive(true);
		return messageBox;
	}

	public static MessageBox YesNo(string text, EventDelegate.Callback cb = null)
	{
		MessageBox messageBox = Cub2UI.FindDialog<MessageBox>("dialog_yesno");
		Transform parent = messageBox.transform.parent;
		GameObject gameObject = parent.gameObject.AddChild(messageBox.gameObject);
		messageBox = gameObject.GetComponent<MessageBox>();
		messageBox.label.text = text;
		messageBox.handler = new EventDelegate(cb);
		messageBox.autoDestroy = true;
		gameObject.SetActive(true);
		return messageBox;
	}

	private void Awake()
	{
		Cub2UI.instance = this;
		if (Cub2UI._currentMenu)
		{
			Kube.OH.isMenu = false;
		}
		Cub2UI._currentMenu = null;
	}

	private void Start()
	{
		this.mRoot = base.GetComponent<UIRoot>();
		this.Update();
	}

	public static void CloseMenu(GameObject menuGo)
	{
		if (Cub2UI._currentMenu == menuGo)
		{
			Cub2UI.currentMenu = null;
		}
	}

	public static GameObject currentMenu
	{
		get
		{
			return Cub2UI._currentMenu;
		}
		set
		{
			if (!Kube.OH)
			{
				return;
			}
			Kube.OH.isMenu = (value != null);
			if (Cub2UI._currentMenu == value)
			{
				return;
			}
			if (Cub2UI._currentMenu)
			{
				Cub2UI._currentMenu.SetActive(false);
			}
			Cub2UI._currentMenu = value;
			if (Cub2UI._currentMenu && !Cub2UI._currentMenu.activeSelf)
			{
				Cub2UI._currentMenu.SetActive(true);
			}
		}
	}

	public static void FindAndOpenDialog(string name)
	{
		ModalChild modalChild = Cub2Menu.Find<ModalChild>(name);
		if (modalChild)
		{
			modalChild.gameObject.SetActive(true);
		}
	}

	public static T FindAndOpenDialog<T>(string name) where T : Component
	{
		ModalChild modalChild = Cub2Menu.Find<ModalChild>(name);
		if (modalChild)
		{
			modalChild.gameObject.SetActive(true);
		}
		return modalChild.GetComponent<T>();
	}

	public static T MessageBox<T>(string name) where T : Component
	{
		ModalChild modalChild = Cub2Menu.Find<ModalChild>(name);
		if (modalChild)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(modalChild.gameObject);
		}
		return modalChild.GetComponent<T>();
	}

	public static T FindAndOpenMenu<T>(string name) where T : Component
	{
		Transform transform = Cub2UI.instance.transform.Find(name);
		if (!transform)
		{
			return (T)((object)null);
		}
		transform.gameObject.SetActive(true);
		return transform.GetComponent<T>();
	}

	private void Update()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		float num = (float)Screen.width;
		float num2 = (float)Screen.height;
		float a = num / 1000f;
		float b = num2 / 600f;
		float num3 = Mathf.Min(a, b);
		float num4 = num / num2;
		int num5 = Mathf.RoundToInt(1000f / num4);
		if (num5 < 600)
		{
			num5 = 600;
		}
		float num6 = (float)num5 / (float)Screen.height;
		Cub2UI.activeWidth = Mathf.RoundToInt((float)Screen.width * num6);
		Cub2UI.activeHeight = Mathf.RoundToInt((float)Screen.height * num6);
		this.mRoot.manualHeight = num5;
	}

	public static T FindDialog<T>(string name) where T : Component
	{
		ModalChild modalChild = Cub2Menu.Find<ModalChild>(name);
		return modalChild.transform.GetComponent<T>();
	}

	public static T FindMenu<T>(string name) where T : Component
	{
		Transform transform = Cub2UI.instance.transform.Find(name);
		if (!transform)
		{
			return (T)((object)null);
		}
		return transform.GetComponent<T>();
	}

	private UIRoot mRoot;

	public static Cub2UI instance;

	private static GameObject _currentMenu;

	public static int activeWidth;

	public static int activeHeight;
}
