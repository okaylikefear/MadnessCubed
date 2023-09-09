using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("NGUI/Interaction/Popup List")]
public class UIPopupListEx : UIWidgetContainer
{
	public UnityEngine.Object ambigiousFont
	{
		get
		{
			if (this.trueTypeFont != null)
			{
				return this.trueTypeFont;
			}
			if (this.bitmapFont != null)
			{
				return this.bitmapFont;
			}
			return this.font;
		}
		set
		{
			if (value is Font)
			{
				this.trueTypeFont = (value as Font);
				this.bitmapFont = null;
				this.font = null;
			}
			else if (value is UIFont)
			{
				this.bitmapFont = (value as UIFont);
				this.trueTypeFont = null;
				this.font = null;
			}
		}
	}

	[Obsolete("Use EventDelegate.Add(popup.onChange, YourCallback) instead, and UIPopupListEx.current.value to determine the state")]
	public UIPopupListEx.LegacyEvent onSelectionChange
	{
		get
		{
			return this.mLegacyEvent;
		}
		set
		{
			this.mLegacyEvent = value;
		}
	}

	public bool isOpen
	{
		get
		{
			return this.mChild != null;
		}
	}

	public string value
	{
		get
		{
			return this.mSelectedItem;
		}
		set
		{
			this.mSelectedItem = value;
			if (this.mSelectedItem == null)
			{
				return;
			}
			if (this.mSelectedItem != null)
			{
				this.TriggerCallbacks();
			}
		}
	}

	[Obsolete("Use 'value' instead")]
	public string selection
	{
		get
		{
			return this.value;
		}
		set
		{
			this.value = value;
		}
	}

	private bool handleEvents
	{
		get
		{
			UIKeyNavigation component = base.GetComponent<UIKeyNavigation>();
			return component == null || !component.enabled;
		}
		set
		{
			UIKeyNavigation component = base.GetComponent<UIKeyNavigation>();
			if (component != null)
			{
				component.enabled = !value;
			}
		}
	}

	private bool isValid
	{
		get
		{
			return this.bitmapFont != null || this.trueTypeFont != null;
		}
	}

	private int activeFontSize
	{
		get
		{
			return (!(this.trueTypeFont != null) && !(this.bitmapFont == null)) ? this.bitmapFont.defaultSize : this.fontSize;
		}
	}

	private float activeFontScale
	{
		get
		{
			return (!(this.trueTypeFont != null) && !(this.bitmapFont == null)) ? ((float)this.fontSize / (float)this.bitmapFont.defaultSize) : 1f;
		}
	}

	protected void TriggerCallbacks()
	{
		if (UIPopupListEx.current != this)
		{
			UIPopupListEx uipopupListEx = UIPopupListEx.current;
			UIPopupListEx.current = this;
			if (this.label != null)
			{
				this.label.text = this.value;
			}
			if (this.mLegacyEvent != null)
			{
				this.mLegacyEvent(this.mSelectedItem);
			}
			if (EventDelegate.IsValid(this.onChange))
			{
				EventDelegate.Execute(this.onChange);
			}
			else if (this.eventReceiver != null && !string.IsNullOrEmpty(this.functionName))
			{
				this.eventReceiver.SendMessage(this.functionName, this.mSelectedItem, SendMessageOptions.DontRequireReceiver);
			}
			UIPopupListEx.current = uipopupListEx;
		}
	}

	private void OnEnable()
	{
		if (EventDelegate.IsValid(this.onChange))
		{
			this.eventReceiver = null;
			this.functionName = null;
		}
		if (this.font != null)
		{
			if (this.font.isDynamic)
			{
				this.trueTypeFont = this.font.dynamicFont;
				this.fontStyle = this.font.dynamicFontStyle;
				this.mUseDynamicFont = true;
			}
			else if (this.bitmapFont == null)
			{
				this.bitmapFont = this.font;
				this.mUseDynamicFont = false;
			}
			this.font = null;
		}
		if (this.textScale != 0f)
		{
			this.fontSize = ((!(this.bitmapFont != null)) ? 16 : Mathf.RoundToInt((float)this.bitmapFont.defaultSize * this.textScale));
			this.textScale = 0f;
		}
		if (this.trueTypeFont == null && this.bitmapFont != null && this.bitmapFont.isDynamic)
		{
			this.trueTypeFont = this.bitmapFont.dynamicFont;
			this.bitmapFont = null;
		}
	}

	private void OnValidate()
	{
		Font x = this.trueTypeFont;
		UIFont uifont = this.bitmapFont;
		this.bitmapFont = null;
		this.trueTypeFont = null;
		if (x != null && (uifont == null || !this.mUseDynamicFont))
		{
			this.bitmapFont = null;
			this.trueTypeFont = x;
			this.mUseDynamicFont = true;
		}
		else if (uifont != null)
		{
			if (uifont.isDynamic)
			{
				this.trueTypeFont = uifont.dynamicFont;
				this.fontStyle = uifont.dynamicFontStyle;
				this.fontSize = uifont.defaultSize;
				this.mUseDynamicFont = true;
			}
			else
			{
				this.bitmapFont = uifont;
				this.mUseDynamicFont = false;
			}
		}
		else
		{
			this.trueTypeFont = x;
			this.mUseDynamicFont = true;
		}
	}

	private void Start()
	{
		if (this.textLabel != null)
		{
			EventDelegate.Add(this.onChange, new EventDelegate.Callback(this.textLabel.SetCurrentSelection));
			this.textLabel = null;
		}
		if (Application.isPlaying)
		{
			if (string.IsNullOrEmpty(this.mSelectedItem))
			{
				if (this.items.Count > 0)
				{
					this.value = this.items[0];
				}
			}
			else
			{
				string value = this.mSelectedItem;
				this.mSelectedItem = null;
				this.value = value;
			}
		}
	}

	private void OnLocalize()
	{
		if (this.isLocalized)
		{
			this.TriggerCallbacks();
		}
	}

	private void Highlight(UILabel lbl, bool instant)
	{
		if (this.mHighlight != null)
		{
			this.mHighlightedLabel = lbl;
			if (this.mHighlight.GetAtlasSprite() == null)
			{
				return;
			}
			Vector3 highlightPosition = this.GetHighlightPosition();
			if (instant || !this.isAnimated)
			{
				this.mHighlight.cachedTransform.localPosition = highlightPosition;
			}
			else
			{
				TweenPosition.Begin(this.mHighlight.gameObject, 0.1f, highlightPosition).method = UITweener.Method.EaseOut;
				if (!this.mTweening)
				{
					this.mTweening = true;
					base.StartCoroutine(this.UpdateTweenPosition());
				}
			}
		}
	}

	private Vector3 GetHighlightPosition()
	{
		if (this.mHighlightedLabel == null)
		{
			return Vector3.zero;
		}
		UISpriteData atlasSprite = this.mHighlight.GetAtlasSprite();
		if (atlasSprite == null)
		{
			return Vector3.zero;
		}
		float pixelSize = this.atlas.pixelSize;
		float num = (float)atlasSprite.borderLeft * pixelSize;
		float y = (float)atlasSprite.borderTop * pixelSize;
		return this.mHighlightedLabel.cachedTransform.localPosition + new Vector3(-num, y, 1f);
	}

	private IEnumerator UpdateTweenPosition()
	{
		if (this.mHighlight != null && this.mHighlightedLabel != null)
		{
			TweenPosition tp = this.mHighlight.GetComponent<TweenPosition>();
			while (tp != null && tp.enabled)
			{
				tp.to = this.GetHighlightPosition();
				yield return null;
			}
		}
		this.mTweening = false;
		yield break;
	}

	private void OnItemHover(GameObject go, bool isOver)
	{
		if (isOver)
		{
			UILabel component = go.GetComponent<UILabel>();
			this.Highlight(component, false);
		}
	}

	private void Select(UILabel lbl, bool instant)
	{
		this.Highlight(lbl, instant);
		UIEventListener component = lbl.gameObject.GetComponent<UIEventListener>();
		this.value = (component.parameter as string);
		UIPlaySound[] components = base.GetComponents<UIPlaySound>();
		int i = 0;
		int num = components.Length;
		while (i < num)
		{
			UIPlaySound uiplaySound = components[i];
			if (uiplaySound.trigger == UIPlaySound.Trigger.OnClick)
			{
				NGUITools.PlaySound(uiplaySound.audioClip, uiplaySound.volume, 1f);
			}
			i++;
		}
	}

	private void OnItemPress(GameObject go, bool isPressed)
	{
		if (isPressed)
		{
			this.Select(go.GetComponent<UILabel>(), true);
		}
	}

	private void OnItemClick(GameObject go)
	{
		this.Close();
	}

	private void OnKey(KeyCode key)
	{
		if (base.enabled && NGUITools.GetActive(base.gameObject) && this.handleEvents)
		{
			int num = this.mLabelList.IndexOf(this.mHighlightedLabel);
			if (num == -1)
			{
				num = 0;
			}
			if (key == KeyCode.UpArrow)
			{
				if (num > 0)
				{
					this.Select(this.mLabelList[num - 1], false);
				}
			}
			else if (key == KeyCode.DownArrow)
			{
				if (num + 1 < this.mLabelList.Count)
				{
					this.Select(this.mLabelList[num + 1], false);
				}
			}
			else if (key == KeyCode.Escape)
			{
				this.OnSelect(false);
			}
		}
	}

	private void OnSelect(bool isSelected)
	{
		this._selected = isSelected;
	}

	private void Update()
	{
		if (this.mChild != null)
		{
			if (this._selected && UICamera.selectedObject == base.gameObject)
			{
				return;
			}
			if (UICamera.selectedObject != null)
			{
				Transform transform = UICamera.selectedObject.transform;
				while (transform)
				{
					if (transform.gameObject == this.popup)
					{
						return;
					}
					transform = transform.parent;
				}
				this.Close();
			}
		}
	}

	public void Close()
	{
		if (this.mChild != null)
		{
			this.mLabelList.Clear();
			this.handleEvents = false;
			if (this.isAnimated)
			{
				UIWidget[] componentsInChildren = this.mChild.GetComponentsInChildren<UIWidget>();
				int i = 0;
				int num = componentsInChildren.Length;
				while (i < num)
				{
					UIWidget uiwidget = componentsInChildren[i];
					Color color = uiwidget.color;
					color.a = 0f;
					TweenColor.Begin(uiwidget.gameObject, 0.15f, color).method = UITweener.Method.EaseOut;
					i++;
				}
				Collider[] componentsInChildren2 = this.mChild.GetComponentsInChildren<Collider>();
				int j = 0;
				int num2 = componentsInChildren2.Length;
				while (j < num2)
				{
					componentsInChildren2[j].enabled = false;
					j++;
				}
			}
			this.mBackground = null;
			this.mHighlight = null;
			UnityEngine.Object.Destroy(this.popup);
			this.popup = null;
			this.mChild = null;
		}
	}

	private void AnimateColor(UIWidget widget)
	{
		Color color = widget.color;
		widget.color = new Color(color.r, color.g, color.b, 0f);
		TweenColor.Begin(widget.gameObject, 0.15f, color).method = UITweener.Method.EaseOut;
	}

	private void AnimatePosition(UIWidget widget, bool placeAbove, float bottom)
	{
		Vector3 localPosition = widget.cachedTransform.localPosition;
		Vector3 localPosition2 = (!placeAbove) ? new Vector3(localPosition.x, 0f, localPosition.z) : new Vector3(localPosition.x, bottom, localPosition.z);
		widget.cachedTransform.localPosition = localPosition2;
		GameObject gameObject = widget.gameObject;
		TweenPosition.Begin(gameObject, 0.15f, localPosition).method = UITweener.Method.EaseOut;
	}

	private void AnimateScale(UIWidget widget, bool placeAbove, float bottom)
	{
		GameObject gameObject = widget.gameObject;
		Transform cachedTransform = widget.cachedTransform;
		float num = (float)this.activeFontSize * this.activeFontScale + this.mBgBorder * 2f;
		cachedTransform.localScale = new Vector3(1f, num / (float)widget.height, 1f);
		TweenScale.Begin(gameObject, 0.15f, Vector3.one).method = UITweener.Method.EaseOut;
		if (placeAbove)
		{
			Vector3 localPosition = cachedTransform.localPosition;
			cachedTransform.localPosition = new Vector3(localPosition.x, localPosition.y - (float)widget.height + num, localPosition.z);
			TweenPosition.Begin(gameObject, 0.15f, localPosition).method = UITweener.Method.EaseOut;
		}
	}

	private void Animate(UIWidget widget, bool placeAbove, float bottom)
	{
		this.AnimateColor(widget);
		this.AnimatePosition(widget, placeAbove, bottom);
	}

	private void OnClick()
	{
		if (base.enabled && NGUITools.GetActive(base.gameObject) && this.mChild == null && this.atlas != null && this.isValid && this.items.Count > 0)
		{
			this.mLabelList.Clear();
			this.popup = UnityEngine.Object.Instantiate<GameObject>(this.popupPrefab);
			this.popup.transform.parent = base.transform.parent;
			this.popup.SetActive(true);
			this._selected = true;
			UIPanel uipanel = UIPanel.Find(base.transform);
			if (uipanel == null)
			{
				return;
			}
			this.mPanel = this.popup.GetComponentInChildren<UIPanel>();
			if (this.mPanel == null)
			{
				return;
			}
			this.handleEvents = true;
			Transform transform = base.transform;
			Bounds bounds = NGUIMath.CalculateRelativeWidgetBounds(transform.parent, transform);
			this.mChild = this.mPanel.gameObject;
			KGUITools.removeAllChildren(this.mChild, true);
			Transform transform2 = this.popup.transform;
			transform2.parent = transform.parent;
			transform2.localPosition = bounds.min;
			transform2.localRotation = Quaternion.identity;
			transform2.localScale = Vector3.one;
			this.mPanel.depth = uipanel.depth + 1;
			this.mBackground = this.popup.GetComponent<UISprite>();
			this.mBackground.depth = NGUITools.CalculateNextDepth(uipanel.gameObject);
			UIScrollBar componentInChildren = this.popup.GetComponentInChildren<UIScrollBar>();
			if (componentInChildren.backgroundWidget)
			{
				componentInChildren.backgroundWidget.depth = NGUITools.CalculateNextDepth(uipanel.gameObject);
			}
			if (componentInChildren.foregroundWidget)
			{
				componentInChildren.foregroundWidget.depth = NGUITools.CalculateNextDepth(uipanel.gameObject);
			}
			Vector4 zero = Vector4.zero;
			this.mHighlight = this.mChild.AddSprite(this.atlas, this.highlightSprite, int.MaxValue);
			this.mHighlight.pivot = UIWidget.Pivot.TopLeft;
			this.mHighlight.color = this.highlightColor;
			UISpriteData atlasSprite = this.mHighlight.GetAtlasSprite();
			if (atlasSprite == null)
			{
				return;
			}
			float num = (float)atlasSprite.borderTop;
			float num2 = (float)this.activeFontSize;
			float activeFontScale = this.activeFontScale;
			float num3 = num2 * activeFontScale;
			float num4 = 0f;
			float num5 = -this.padding.y;
			int num6 = (!(this.bitmapFont != null)) ? this.fontSize : this.bitmapFont.defaultSize;
			List<UILabel> list = new List<UILabel>();
			int i = 0;
			int count = this.items.Count;
			while (i < count)
			{
				string text = this.items[i];
				UILabel uilabel = this.mChild.AddWidget(int.MaxValue);
				uilabel.name = i.ToString();
				uilabel.pivot = UIWidget.Pivot.TopLeft;
				uilabel.bitmapFont = this.bitmapFont;
				uilabel.trueTypeFont = this.trueTypeFont;
				uilabel.fontSize = num6;
				uilabel.fontStyle = this.fontStyle;
				uilabel.text = ((!this.isLocalized) ? text : Localization.Get(text));
				uilabel.color = this.textColor;
				uilabel.cachedTransform.localPosition = new Vector3(zero.x + this.padding.x, num5, -1f);
				uilabel.overflowMethod = UILabel.Overflow.ResizeFreely;
				uilabel.MakePixelPerfect();
				if (activeFontScale != 1f)
				{
					uilabel.cachedTransform.localScale = Vector3.one * activeFontScale;
				}
				list.Add(uilabel);
				num5 -= num3;
				num5 -= this.padding.y;
				num4 = Mathf.Max(num4, uilabel.printedSize.x);
				UIEventListener uieventListener = UIEventListener.Get(uilabel.gameObject);
				uieventListener.onHover = new UIEventListener.BoolDelegate(this.OnItemHover);
				uieventListener.onPress = new UIEventListener.BoolDelegate(this.OnItemPress);
				uieventListener.onClick = new UIEventListener.VoidDelegate(this.OnItemClick);
				uieventListener.parameter = text;
				if (this.mSelectedItem == text || (i == 0 && string.IsNullOrEmpty(this.mSelectedItem)))
				{
					this.Highlight(uilabel, true);
				}
				this.mLabelList.Add(uilabel);
				i++;
			}
			num4 = Mathf.Max(num4, bounds.size.x * activeFontScale - (zero.x + this.padding.x) * 2f);
			float num7 = num4 / activeFontScale;
			Vector3 vector = new Vector3(num7 * 0.5f, -num2 * 0.5f, 0f);
			Vector3 vector2 = new Vector3(num7, (num3 + this.padding.y) / activeFontScale, 1f);
			int j = 0;
			int count2 = list.Count;
			while (j < count2)
			{
				UILabel uilabel2 = list[j];
				NGUITools.AddWidgetCollider(uilabel2.gameObject);
				BoxCollider component = uilabel2.GetComponent<BoxCollider>();
				if (component != null)
				{
					vector.z = component.center.z;
					component.center = vector;
					component.size = vector2;
				}
				else
				{
					BoxCollider2D component2 = uilabel2.GetComponent<BoxCollider2D>();
					component2.offset = vector;
					component2.size = vector2;
				}
				j++;
			}
			num4 += (zero.x + this.padding.x) * 2f;
			num5 -= zero.y;
			float num8 = 2f * this.atlas.pixelSize;
			float f = num4 - (zero.x + this.padding.x) * 2f + (float)atlasSprite.borderLeft * num8;
			float f2 = num3 + num * num8;
			this.mHighlight.width = Mathf.RoundToInt(f);
			this.mHighlight.height = Mathf.RoundToInt(f2);
			bool flag = this.position == UIPopupListEx.Position.Above;
			if (this.position == UIPopupListEx.Position.Auto)
			{
				UICamera uicamera = UICamera.FindCameraForLayer(base.gameObject.layer);
				if (uicamera != null)
				{
					flag = (uicamera.cachedCamera.WorldToViewportPoint(transform.position).y < 0.5f);
				}
			}
			this.popup.GetComponentInChildren<UIScrollView>().ResetPosition();
			if (this.isAnimated)
			{
				float bottom = num5 + num3;
				this.Animate(this.mHighlight, flag, bottom);
				int k = 0;
				int count3 = list.Count;
				while (k < count3)
				{
					this.Animate(list[k], flag, bottom);
					k++;
				}
				this.AnimateColor(this.mBackground);
				this.AnimateScale(this.mBackground, flag, bottom);
			}
			if (flag)
			{
				transform2.localPosition = new Vector3(bounds.min.x, bounds.max.y - num5 - zero.y, bounds.min.z);
			}
		}
		else
		{
			this.OnSelect(false);
		}
	}

	private const float animSpeed = 0.15f;

	public static UIPopupListEx current;

	public UILabel label;

	public GameObject popup;

	public GameObject popupPrefab;

	public UIAtlas atlas;

	public UIFont bitmapFont;

	public Font trueTypeFont;

	public int fontSize = 16;

	public FontStyle fontStyle;

	public string backgroundSprite;

	public string highlightSprite;

	public UIPopupListEx.Position position;

	public List<string> items = new List<string>();

	public Vector2 padding = new Vector3(4f, 4f);

	public Color textColor = Color.white;

	public Color backgroundColor = Color.white;

	public Color highlightColor = new Color(0.882352948f, 0.784313738f, 0.5882353f, 1f);

	public bool isAnimated = true;

	public bool isLocalized;

	public List<EventDelegate> onChange = new List<EventDelegate>();

	[HideInInspector]
	[SerializeField]
	private string mSelectedItem;

	private UIPanel mPanel;

	private GameObject mChild;

	private UISprite mBackground;

	private UISprite mHighlight;

	private UILabel mHighlightedLabel;

	private List<UILabel> mLabelList = new List<UILabel>();

	private float mBgBorder;

	[HideInInspector]
	[SerializeField]
	private GameObject eventReceiver;

	[HideInInspector]
	[SerializeField]
	private string functionName = "OnSelectionChange";

	[HideInInspector]
	[SerializeField]
	private float textScale;

	[HideInInspector]
	[SerializeField]
	private UIFont font;

	[SerializeField]
	[HideInInspector]
	private UILabel textLabel;

	private UIPopupListEx.LegacyEvent mLegacyEvent;

	private bool mUseDynamicFont;

	private bool mTweening;

	protected bool _selected;

	public enum Position
	{
		Auto,
		Above,
		Below
	}

	public delegate void LegacyEvent(string val);
}
