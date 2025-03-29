namespace UIWidgets
{
	using UIWidgets.Attributes;
	using UnityEngine;
	using UnityEngine.Events;
	using UnityEngine.UI;

	/// <summary>
	/// InputField adapter to work with both Unity text and TMPro text.
	/// </summary>
	[AddComponentMenu("UI/New UI Widgets/Adapters/Input Field Adapter")]
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/third-party-support/tmpro.html")]
	[DataBindSupport]
	public class InputFieldAdapter : MonoBehaviour, IInputFieldProxy
	{
		IInputFieldProxy proxy;

		/// <summary>
		/// Proxy.
		/// </summary>
		protected IInputFieldProxy Proxy
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
		public GameObject GameObject => Proxy.gameObject;

		/// <summary>
		/// Is InputField has rich text support?
		/// </summary>
		public bool richText => Proxy.richText;

		/// <summary>
		/// Proxy value.
		/// </summary>
		[DataBindField]
		public string Value
		{
			get => Proxy.text;

			set => Proxy.text = value ?? string.Empty;
		}

		/// <summary>
		/// Proxy value.
		/// Alias for Value.
		/// </summary>
		public string text
		{
			get => Proxy.text;

			set => Proxy.text = value;
		}

		/// <summary>
		/// The Unity Event to call when edit.
		/// </summary>
		/// <value>The OnValueChange.</value>
		[DataBindEvent(nameof(Value))]
		public UnityEvent<string> onValueChanged => Proxy.onValueChanged;

		/// <summary>
		/// The Unity Event to call when editing has ended.
		/// </summary>
		/// <value>The OnEndEdit.</value>
		[DataBindEvent(nameof(Value))]
		public UnityEvent<string> onEndEdit => Proxy.onEndEdit;

		/// <summary>
		/// Current InputField caret position (also selection tail).
		/// </summary>
		/// <value>The caret position.</value>
		[DataBindField]
		public int caretPosition
		{
			get => Proxy.caretPosition;

			set => Proxy.caretPosition = value;
		}

		/// <summary>
		/// Is the InputField eligible for interaction (excludes canvas groups).
		/// </summary>
		/// <value><c>true</c> if interactable; otherwise, <c>false</c>.</value>
		[DataBindField]
		public bool interactable
		{
			get => Proxy.interactable;

			set => Proxy.interactable = value;
		}

		/// <summary>
		/// Text component.
		/// </summary>
		public Graphic textComponent
		{
			get => Proxy.textComponent;

			set => Proxy.textComponent = value;
		}

		/// <summary>
		/// Placeholder.
		/// </summary>
		public Graphic placeholder
		{
			get => Proxy.placeholder;

			set => Proxy.placeholder = value;
		}

		/// <summary>
		/// If the proxy was canceled and will revert back to the original text.
		/// </summary>
		public bool wasCanceled => Proxy.wasCanceled;

		/// <summary>
		/// Get text proxy.
		/// </summary>
		/// <returns>Proxy instance.</returns>
		protected virtual IInputFieldProxy GetProxy()
		{
			if (TryGetComponent<InputField>(out var input_unity))
			{
				return new InputFieldProxy(input_unity);
			}

#if UIWIDGETS_TMPRO_SUPPORT && (UNITY_5_2 || UNITY_5_3 || UNITY_5_3_OR_NEWER)
			if (TryGetComponent<TMPro.TMP_InputField>(out var input_tmpro))
			{
				return new InputFieldTMProProxy(input_tmpro);
			}
#endif

			Debug.LogWarning("Not found any InputField component. If you are using TextMeshPro then you need to enable TextMeshPro support in Edit / Project Settings / New UI Widgets.", this);

			return new InputFieldNull();
		}

		/// <summary>
		/// Determines whether InputField instance is null.
		/// </summary>
		/// <returns><c>true</c> if InputField instance is null; otherwise, <c>false</c>.</returns>
		public bool IsNull() => Proxy.IsNull();

		/// <summary>
		/// Determines whether this lineType is LineType.MultiLineNewline.
		/// </summary>
		/// <returns><c>true</c> if lineType is LineType.MultiLineNewline; otherwise, <c>false</c>.</returns>
		public bool IsMultiLineNewline() => Proxy.IsMultiLineNewline();

		/// <summary>
		/// Function to activate the InputField to begin processing Events.
		/// </summary>
		public void Activate() => Proxy.Activate();

		/// <summary>
		/// Deactivate InputField.
		/// </summary>
		public void Deactivate() => Proxy.Deactivate();

#if UNITY_4_6 || UNITY_4_7 || UNITY_5_0
		/// <summary>
		/// Move caret to end.
		/// </summary>
		public void MoveToEnd() => Proxy.MoveToEnd();
#endif

		/// <summary>
		/// Set focus to InputField.
		/// </summary>
		public void Focus() => Proxy.Focus();

		/// <summary>
		/// Set focus to InputField.
		/// </summary>
		public void Select() => Proxy.Select();

		/// <summary>
		/// Set text without raising onValueChanged event.
		/// </summary>
		/// <param name="text">Text.</param>
		public void SetTextWithoutNotify(string text) => Proxy.SetTextWithoutNotify(text);

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