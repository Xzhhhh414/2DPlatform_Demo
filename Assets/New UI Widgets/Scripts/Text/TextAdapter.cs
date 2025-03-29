namespace UIWidgets
{
	using System;
	using UIWidgets.Attributes;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Text adapter to work with both Unity text and TMPro text.
	/// </summary>
	[AddComponentMenu("UI/New UI Widgets/Adapters/Text Adapter")]
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/third-party-support/tmpro.html")]
	[DataBindSupport]
	public class TextAdapter : MonoBehaviour, ITextProxy
	{
		ITextProxy proxy;

		/// <summary>
		/// Proxy.
		/// </summary>
		protected ITextProxy Proxy
		{
			get
			{
				if (Utilities.IsNull(proxy))
				{
					proxy = GetProxy();
				}

				return proxy;
			}
		}

		/// <summary>
		/// Proxy gameobject.
		/// </summary>
		public GameObject GameObject => Proxy.GameObject;

		/// <summary>
		/// Proxy graphic component.
		/// </summary>
		public Graphic Graphic => Proxy.Graphic;

		/// <summary>
		/// Proxy color.
		/// </summary>
		[DataBindField]
		public Color color
		{
			get => Proxy.color;

			set => Proxy.color = value;
		}

		/// <summary>
		/// Font size.
		/// </summary>
		[DataBindField]
		public float fontSize
		{
			get => Proxy.fontSize;

			set => Proxy.fontSize = value;
		}

		/// <summary>
		/// Font style.
		/// </summary>
		[DataBindField]
		public FontStyle fontStyle
		{
			get => Proxy.fontStyle;

			set => Proxy.fontStyle = value;
		}

		/// <summary>
		/// Text alignment.
		/// </summary>

		[DataBindField]
		public TextAnchor alignment
		{
			get => Proxy.alignment;

			set => Proxy.alignment = value;
		}

		/// <summary>
		/// Bold.
		/// </summary>
		public bool Bold
		{
			get => Proxy.Bold;

			set => Proxy.Bold = value;
		}

		/// <summary>
		/// Italic.
		/// </summary>
		public bool Italic
		{
			get => Proxy.Italic;

			set => Proxy.Italic = value;
		}

		/// <summary>
		/// Proxy value.
		/// </summary>
		[DataBindField]
		public string Value
		{
			get => Proxy.text;

			set => Proxy.text = value;
		}

		/// <summary>
		/// Proxy value.
		/// Alias for Value property.
		/// </summary>
		public string text
		{
			get => Proxy.text;

			set => Proxy.text = value;
		}

		/// <summary>
		/// Proxy value.
		/// Alias for Value property.
		/// </summary>
		[Obsolete("Renamed to text.")]
		public string Text
		{
			get => Proxy.text;

			set => Proxy.text = value;
		}

		/// <summary>
		/// Get text proxy.
		/// </summary>
		/// <returns>Proxy instance.</returns>
		protected virtual ITextProxy GetProxy()
		{
			if (TryGetComponent<Text>(out var text_unity))
			{
				return new TextUnity(text_unity);
			}

			#if UIWIDGETS_TMPRO_SUPPORT
			if (TryGetComponent<TMPro.TextMeshProUGUI>(out var text_tmpro))
			{
				return new TextTMPro(text_tmpro);
			}
			#endif

			Debug.LogWarning("Not found any Text component. If you are using TextMeshPro then you need to enable TextMeshPro support in Edit / Project Settings / New UI Widgets.", this);

			return new TextNull();
		}

		#if UNITY_EDITOR
		/// <summary>
		/// Update layout when parameters changed.
		/// </summary>
		protected virtual void OnValidate()
		{
			GetProxy();
		}
		#endif
	}
}