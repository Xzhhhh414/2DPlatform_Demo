namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using System.Runtime.CompilerServices;
	using UIWidgets.Attributes;
	using UIWidgets.l10n;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Base class for the Dialogs.
	/// </summary>
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/widgets/dialogs/dialog.html")]
	public abstract class DialogBase : MonoBehaviourInitiable, ITemplatable, IStylable, IUpgradeable, IHideable, INotifyCompletion, ILocalizationSupport
	{
#pragma warning disable 0649
		[SerializeField]
		[HideInInspector]
		[Obsolete("Replaced with buttonsTemplates")]
		Button defaultButton;
#pragma warning restore 0649

		/// <summary>
		/// Gets or sets the default button.
		/// </summary>
		/// <value>The default button.</value>
		[Obsolete("Replaced with ButtonsTemplates")]
		public Button DefaultButton
		{
			get
			{
				Upgrade();
				return defaultButton;
			}

			set
			{
				Upgrade();

				ButtonsTemplates = new List<Button>(ButtonsTemplates)
				{
					value,
				};
			}
		}

		/// <summary>
		/// Buttons container.
		/// </summary>
		[SerializeField]
		protected RectTransform ButtonsContainer;

		/// <summary>
		/// Buttons templates.
		/// </summary>
		[SerializeField]
		protected List<Button> buttonsTemplates = new List<Button>();

		/// <summary>
		/// Gets or sets the default buttons.
		/// </summary>
		/// <value>The default buttons.</value>
		public abstract IReadOnlyList<Button> ButtonsTemplates
		{
			get;
			set;
		}

		/// <summary>
		/// Content root.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[Obsolete("Replaced with DialogInfo component.")]
		public RectTransform ContentRoot;

		[SerializeField]
		[HideInInspector]
		[Obsolete("Replaced with DialogInfo component.")]
		Text titleText;

		/// <summary>
		/// Gets or sets the text component.
		/// </summary>
		/// <value>The text.</value>
		[Obsolete("Replaced with DialogInfo component.")]
		public Text TitleText
		{
			get => titleText;

			set => titleText = value;
		}

		[SerializeField]
		[HideInInspector]
		[Obsolete("Replaced with DialogInfo component.")]
		Text contentText;

		/// <summary>
		/// Gets or sets the text component.
		/// </summary>
		/// <value>The text.</value>
		[Obsolete("Replaced with DialogInfo component.")]
		public Text ContentText
		{
			get => contentText;

			set => contentText = value;
		}

		[SerializeField]
		[HideInInspector]
		[Obsolete("Replaced with DialogInfo component.")]
		Image dialogIcon;

		/// <summary>
		/// Gets or sets the icon component.
		/// </summary>
		/// <value>The icon.</value>
		[Obsolete("Replaced with DialogInfo component.")]
		public Image Icon
		{
			get => dialogIcon;

			set => dialogIcon = value;
		}

		[SerializeField]
		DialogInfoBase dialogInfo;

		/// <summary>
		/// Gets the dialog info.
		/// </summary>
		/// <value>The dialog info.</value>
		public DialogInfoBase DialogInfo
		{
			get
			{
				if (dialogInfo == null)
				{
					TryGetComponent(out dialogInfo);
				}

				return dialogInfo;
			}
		}

		bool isTemplate = true;

		/// <summary>
		/// Gets a value indicating whether this instance is template.
		/// </summary>
		/// <value><c>true</c> if this instance is template; otherwise, <c>false</c>.</value>
		public bool IsTemplate
		{
			get => isTemplate;

			set => isTemplate = value;
		}

		string dialogTemplateName;

		/// <summary>
		/// Gets the name of the template.
		/// </summary>
		/// <value>The name of the template.</value>
		public string TemplateName
		{
			get
			{
				if (dialogTemplateName == null)
				{
					FindTemplates();
				}

				return dialogTemplateName;
			}

			set => dialogTemplateName = value;
		}

		/// <summary>
		/// Set focus to the last Selectable object in dialog.
		/// </summary>
		[SerializeField]
		[Tooltip("Set focus to the last Selectable object in dialog.")]
		public bool AutoFocus;

		[SerializeField]
		Button closeButton;

		/// <summary>
		/// Close button.
		/// </summary>
		public Button CloseButton
		{
			get => closeButton;

			set
			{
				if (IsInited && (closeButton != null))
				{
					closeButton.onClick.RemoveListener(Cancel);
				}

				closeButton = value;

				if (IsInited && (closeButton != null))
				{
					closeButton.onClick.AddListener(Cancel);
				}
			}
		}

		/// <summary>
		/// The modal ID.
		/// </summary>
		protected InstanceID? ModalKey;

		/// <summary>
		/// Dialog hierarchy position.
		/// </summary>
		protected HierarchyPosition DialogPosition;

		/// <summary>
		/// Canvas resize subscription.
		/// </summary>
		protected Subscription CanvasResize;

		/// <summary>
		/// Callback on dialog close.
		/// </summary>
		public Action OnClose;

		/// <summary>
		/// Callback on dialog cancel.
		/// </summary>
		[Obsolete("Replaced with OnDialogCancel.")]
		public Func<int, bool> OnCancel;

		/// <summary>
		/// Callback on dialog cancel.
		/// </summary>
		public Func<DialogBase, int, bool> OnDialogCancel;

		[SerializeField]
		[Tooltip("If enabled translates buttons labels using Localization.GetTranslation().")]
		bool localizationSupport = true;

		/// <summary>
		/// Localization support.
		/// </summary>
		public bool LocalizationSupport
		{
			get => localizationSupport;

			set => localizationSupport = value;
		}

		/// <summary>
		/// Hide on modal click.
		/// </summary>
		[SerializeField]
		public bool HideOnModalClick = false;

		/// <summary>
		/// Is instance destroyed?
		/// </summary>
		public bool IsDestroyed
		{
			get;
			protected set;
		}

		/// <summary>
		/// Opened base dialogs.
		/// </summary>
		protected static HashSet<DialogBase> openedBaseDialogs = new HashSet<DialogBase>();

		/// <summary>
		/// List of the opened base dialogs.
		/// </summary>
		protected static List<DialogBase> OpenedBaseDialogsList = new List<DialogBase>();

		/// <summary>
		/// Opened base dialogs.
		/// </summary>
		public static IReadOnlyList<DialogBase> OpenedBaseDialogs
		{
			get
			{
				OpenedBaseDialogsList.Clear();
				OpenedBaseDialogsList.AddRange(openedBaseDialogs);

				return OpenedBaseDialogsList;
			}
		}

		/// <summary>
		/// Event on any instance opened.
		/// </summary>
		public static event Action<int> OnBaseInstanceOpen;

		/// <summary>
		/// Event on any instance closed.
		/// </summary>
		public static event Action<int> OnBaseInstanceClose;

		/// <inheritdoc/>
		protected override void InitOnce()
		{
			base.InitOnce();

			if (closeButton != null)
			{
				closeButton.onClick.AddListener(Cancel);
			}
		}

		#if UNITY_EDITOR && UNITY_2019_3_OR_NEWER
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		[DomainReload(nameof(openedBaseDialogs), nameof(OpenedBaseDialogsList), nameof(OnBaseInstanceOpen), nameof(OnBaseInstanceClose))]
		static void StaticInit()
		{
			openedBaseDialogs.Clear();
			OpenedBaseDialogsList.Clear();
			OnBaseInstanceOpen = null;
			OnBaseInstanceClose = null;
		}
		#endif

		bool localeSubscription;

		/// <summary>
		/// Process the enable event.
		/// </summary>
		protected virtual void OnEnable()
		{
			if (localeSubscription)
			{
				return;
			}

			Init();

			localeSubscription = true;
			Localization.OnLocaleChanged += LocaleChanged;
			LocaleChanged();
		}

		/// <summary>
		/// Process the disable event.
		/// </summary>
		protected abstract void OnDisable();

		/// <summary>
		/// Awake is called when the script instance is being loaded.
		/// </summary>
		protected virtual void Awake()
		{
			if (IsTemplate)
			{
				gameObject.SetActive(false);
			}
		}

		/// <summary>
		/// Find templates.
		/// </summary>
		protected abstract void FindTemplates();

		/// <summary>
		/// Locale changed.
		/// </summary>
		protected abstract void LocaleChanged();

		/// <summary>
		/// Process the destroy event.
		/// </summary>
		protected virtual void OnDestroy()
		{
			DialogPosition.ParentDestroyed();

			Localization.OnLocaleChanged -= LocaleChanged;
			localeSubscription = false;
		}

		/// <summary>
		/// Show dialog.
		/// </summary>
		/// <param name="title">Title. Also can be changed with SetInfo().</param>
		/// <param name="message">Message. Also can be changed with SetInfo().</param>
		/// <param name="buttons">Buttons. Also can be changed with SetButtons().</param>
		/// <param name="focusButton">Set focus on button with specified name. Also can be changed with SetButtons() or FocusButton().</param>
		/// <param name="position">Position. Also can be changed with SetPosition().</param>
		/// <param name="icon">Icon. Also can be changed with SetInfo(...).</param>
		/// <param name="modal">If set to <c>true</c> modal. Also can be changed with SetModal().</param>
		/// <param name="modalSprite">Modal sprite. Also can be changed with SetModal().</param>
		/// <param name="modalColor">Modal color. Also can be changed with SetModal().</param>
		/// <param name="canvas">Canvas. Also can be changed with SetCanvas().</param>
		/// <param name="content">Content. Also can be changed with SetContent().</param>
		/// <param name="onClose">On close callback. Also can be changed with OnClose field.</param>
		/// <param name="onCancel">On cancel callback. Also can be changed with OnCancel field.</param>
		public virtual void Show(
			string title = null,
			string message = null,
			IList<DialogButton> buttons = null,
			string focusButton = null,
			Vector3? position = null,
			Sprite icon = null,
			bool modal = false,
			Sprite modalSprite = null,
			Color? modalColor = null,
			Canvas canvas = null,
			RectTransform content = null,
			Action onClose = null,
			Func<int, bool> onCancel = null)
		{
			CloseOnButtonClick = true;

			if (IsTemplate)
			{
				Debug.LogWarning("Use the template clone, not the template itself: DialogTemplate.Clone().Show(...), not DialogTemplate.Show(...)");
			}

			OnClose = onClose;

			#pragma warning disable 0618
			OnCancel = onCancel;
			#pragma warning restore 0618

			SetInfo(title, null, message, null, icon);
			SetContent(content);

			var canvas_rt = SetCanvas(canvas);

			SetModal(modal, modalSprite, modalColor, canvas_rt);

			if (position.HasValue)
			{
				SetPosition(position.Value);
			}

			gameObject.SetActive(true);

			SetButtons(buttons, focusButton);

			if (AutoFocus && string.IsNullOrEmpty(focusButton))
			{
				UtilitiesUI.SelectChild(this, closeButton);
			}

			InstanceOpened();
		}

		/// <summary>
		/// Show dialog.
		/// </summary>
		/// <param name="title">Title. Also can be changed with SetInfo().</param>
		/// <param name="message">Message. Also can be changed with SetInfo().</param>
		/// <param name="buttons">Buttons. Also can be changed with SetButtons().</param>
		/// <param name="focusButton">Set focus on button with specified name. Also can be changed with SetButtons() or FocusButton().</param>
		/// <param name="position">Position. Also can be changed with SetPosition().</param>
		/// <param name="icon">Icon. Also can be changed with SetInfo(...).</param>
		/// <param name="modal">If set to <c>true</c> modal. Also can be changed with SetModal().</param>
		/// <param name="modalSprite">Modal sprite. Also can be changed with SetModal().</param>
		/// <param name="modalColor">Modal color. Also can be changed with SetModal().</param>
		/// <param name="canvas">Canvas. Also can be changed with SetCanvas().</param>
		/// <param name="content">Content. Also can be changed with SetContent().</param>
		/// <param name="closeOnButtonClick">Close dialog on button click.</param>
		/// <returns>Index of the clicked button or -1 in case of Cancel() method.</returns>
		public virtual DialogBase ShowAsync(
			string title = null,
			string message = null,
			IList<DialogButton> buttons = null,
			string focusButton = null,
			Vector3? position = null,
			Sprite icon = null,
			bool modal = false,
			Sprite modalSprite = null,
			Color? modalColor = null,
			Canvas canvas = null,
			RectTransform content = null,
			bool closeOnButtonClick = true)
		{
			Show(title, message, buttons, focusButton, position, icon, modal, modalSprite, modalColor, canvas, content);
			CloseOnButtonClick = closeOnButtonClick;

			return this;
		}

		/// <summary>
		/// Instance opened.
		/// </summary>
		protected virtual void InstanceOpened()
		{
			openedBaseDialogs.Add(this);
			OnBaseInstanceOpen?.Invoke(openedBaseDialogs.Count);
		}

		/// <summary>
		/// Instance closed.
		/// </summary>
		protected virtual void InstanceClosed()
		{
			openedBaseDialogs.Remove(this);
			OnBaseInstanceClose?.Invoke(openedBaseDialogs.Count);
		}

		/// <summary>
		/// Set modal mode.
		/// Warning: modal block is created at the current root canvas.
		/// </summary>
		/// <param name="modal">If set to <c>true</c> modal.</param>
		/// <param name="modalSprite">Modal sprite.</param>
		/// <param name="modalColor">Modal color.</param>
		/// <param name="parentCanvas">Parent canvas.</param>
		public virtual void SetModal(bool modal = false, Sprite modalSprite = null, Color? modalColor = null, RectTransform parentCanvas = null)
		{
			ModalHelper.Close(ref ModalKey);

			if (modal)
			{
				ModalKey = ModalHelper.Open(this, modalSprite, modalColor, ProcessModalClick, parentCanvas);
			}

			if (DialogPosition.Changed)
			{
				transform.SetAsLastSibling();
			}
		}

		/// <summary>
		/// Process modal click.
		/// </summary>
		protected void ProcessModalClick()
		{
			if (HideOnModalClick)
			{
				Hide();
			}
		}

		/// <summary>
		/// Set canvas.
		/// </summary>
		/// <param name="canvas">Canvas.</param>
		/// <returns>Canvas RectTransform.</returns>
		public virtual RectTransform SetCanvas(Canvas canvas)
		{
			var parent = (canvas != null) ? canvas.transform as RectTransform : UtilitiesUI.FindTopmostCanvas(gameObject.transform);

			DialogPosition.Restore();
			DialogPosition = HierarchyPosition.SetParent(transform, parent);

			var resize = Utilities.RequireComponent<ResizeListener>(parent);
			CanvasResize.Clear();
			CanvasResize = new Subscription(resize.OnResize, RefreshPosition);

			return parent;
		}

		/// <summary>
		/// Refresh position.
		/// </summary>
		protected virtual void RefreshPosition() => DialogPosition.Refresh();

		/// <summary>
		/// Set position.
		/// </summary>
		/// <param name="position">Position.</param>
		public virtual void SetPosition(Vector3 position)
		{
			transform.localPosition = position;
		}

		/// <summary>
		/// Sets the info. Pass null to leave default value.
		/// </summary>
		/// <param name="title">Title.</param>
		/// <param name="message">Message.</param>
		/// <param name="icon">Icon.</param>
		public virtual void SetInfo(string title = null, string message = null, Sprite icon = null)
		{
			DialogInfo.SetInfo(title, null, message, null, icon);
		}

		/// <summary>
		/// Sets the info. Pass null to leave default value.
		/// </summary>
		/// <param name="title">Title.</param>
		/// <param name="titleArgs">Title arguments.</param>
		/// <param name="message">Message.</param>
		/// <param name="messageArgs">Message arguments.</param>
		/// <param name="icon">Icon.</param>
		public virtual void SetInfo(string title = null, object[] titleArgs = null, string message = null, object[] messageArgs = null, Sprite icon = null)
		{
			DialogInfo.SetInfo(title, titleArgs, message, messageArgs, icon);
		}

		/// <summary>
		/// Sets the content.
		/// </summary>
		/// <param name="content">Content.</param>
		public virtual void SetContent(RectTransform content)
		{
			if (content == null)
			{
				return;
			}

			DialogInfo.SetContent(content);
		}

		/// <summary>
		/// Cancel dialog.
		/// </summary>
		public virtual void Cancel()
		{
			#pragma warning disable 0618
			if (OnCancel != null)
			{
				if (!OnCancel(-1))
				{
					return;
				}
			}
			#pragma warning restore 0618

			if (OnDialogCancel != null)
			{
				if (!OnDialogCancel(this, -1))
				{
					return;
				}
			}

			Complete(-1);
		}

		/// <summary>
		/// Close dialog.
		/// </summary>
		public virtual void Hide()
		{
			Complete(-1, true);
		}

		/// <summary>
		/// Creates the buttons.
		/// </summary>
		/// <param name="buttons">Buttons.</param>
		/// <param name="focusButton">Focus button.</param>
		public abstract void SetButtons(IList<DialogButton> buttons, string focusButton = null);

		/// <summary>
		/// Set focus to the specified button.
		/// </summary>
		/// <param name="focusButton">Button label.</param>
		/// <returns>true if button found with specified label; otherwise false.</returns>
		public abstract bool FocusButton(string focusButton);

		/// <summary>
		/// Get button template index.
		/// </summary>
		/// <param name="button">Button.</param>
		/// <returns>Template index,</returns>
		protected abstract int GetTemplateIndex(DialogButton button);

		/// <summary>
		/// Return this instance to cache.
		/// </summary>
		protected virtual void Return()
		{
			ResetParameters();
			InstanceClosed();
		}

		/// <summary>
		/// Resets the parameters.
		/// </summary>
		protected virtual void ResetParameters()
		{
			OnClose = null;
			OnDialogCancel = null;
			#pragma warning disable 0618
			OnCancel = null;
			#pragma warning restore 0618

			DialogInfo.RestoreDefaultValues();
		}

		/// <summary>
		/// Default function to close dialog.
		/// </summary>
		/// <returns>true if dialog can be closed; otherwise false.</returns>
		[Obsolete("Replaced with DefaultClose().")]
		public static bool Close()
		{
			return true;
		}

		/// <summary>
		/// Default function to close dialog.
		/// </summary>
		/// <param name="index">Button index.</param>
		/// <returns>true if dialog can be closed; otherwise false.</returns>
		[Obsolete("Replaced with DefaultClose().")]
		public static bool AlwaysClose(int index)
		{
			return true;
		}

		/// <summary>
		/// Default function to close dialog.
		/// </summary>
		/// <param name="dialog">Dialog.</param>
		/// <param name="index">Button index.</param>
		/// <returns>true if dialog can be closed; otherwise false.</returns>
		public static bool DefaultClose(DialogBase dialog, int index)
		{
			return true;
		}

		#region async

		/// <summary>
		/// Action on continuation.
		/// </summary>
		protected Action Continuation;

		/// <summary>
		/// Clicked button index.
		/// </summary>
		protected int ButtonIndex;

		/// <summary>
		/// Is asynchronous?
		/// </summary>
		public bool IsAsync
		{
			get
			{
				return Continuation != null;
			}
		}

		/// <summary>
		/// Close dialog on button click.
		/// </summary>
		public bool CloseOnButtonClick
		{
			get;
			protected set;
		}

		/// <summary>
		/// Gets a value that indicates whether the asynchronous task has completed.
		/// </summary>
		public virtual bool IsCompleted
		{
			get;
			protected set;
		}

		/// <summary>
		/// Gets an awaiter used to await this result.
		/// </summary>
		/// <returns>Awaiter.</returns>
		public virtual DialogBase GetAwaiter()
		{
			IsCompleted = false;
			return this;
		}

		/// <summary>
		/// Ends the wait for the completion of the asynchronous task.
		/// </summary>
		/// <returns>Result.</returns>
		public virtual int GetResult()
		{
			return ButtonIndex;
		}

		/// <summary>
		/// Sets the action to perform when the this object stops waiting for the asynchronous task to complete.
		/// </summary>
		/// <param name="continuation">The action to perform when the wait operation completes.</param>
		public virtual void OnCompleted(Action continuation)
		{
			Continuation = continuation;
		}

		/// <summary>
		/// Complete asynchronous task.
		/// </summary>
		/// <param name="buttonIndex">Button index.</param>
		public virtual void Complete(int buttonIndex)
		{
			Complete(buttonIndex, CloseOnButtonClick);
		}

		/// <summary>
		/// Complete asynchronous task.
		/// </summary>
		/// <param name="buttonIndex">Button index.</param>
		/// <param name="close">Close dialog.</param>
		protected virtual void Complete(int buttonIndex, bool close)
		{
			if (!IsCompleted && (Continuation != null))
			{
				ButtonIndex = buttonIndex;
				IsCompleted = true;

				var c = Continuation;
				Continuation = null;
				c?.Invoke();
			}

			if (!close)
			{
				return;
			}

			OnClose?.Invoke();

			ModalHelper.Close(ref ModalKey);
			DialogPosition.Restore();
			CanvasResize.Clear();

			Return();
		}

		#endregion

		#region IStylable implementation

		/// <inheritdoc/>
		public virtual bool SetStyle(Style style)
		{
			if (TryGetComponent<Image>(out var bg))
			{
				style.Dialog.Background.ApplyTo(bg);
			}

			style.Dialog.ContentBackground.ApplyTo(transform.Find("Content"));
			style.Dialog.Delimiter.ApplyTo(transform.Find("Delimiter/Delimiter"));

			if (closeButton != null)
			{
				style.ButtonClose.ApplyTo(closeButton);
			}
			else
			{
				style.ButtonClose.Background.ApplyTo(transform.Find("Header/CloseButton"));
				style.ButtonClose.Text.ApplyTo(transform.Find("Header/CloseButton/Text"));
			}

			if (DialogInfo != null)
			{
				DialogInfo.SetStyle(style.Dialog);
			}

			return true;
		}

		/// <inheritdoc/>
		public virtual bool GetStyle(Style style)
		{
			if (TryGetComponent<Image>(out var bg))
			{
				style.Dialog.Background.GetFrom(bg);
			}

			style.Dialog.ContentBackground.GetFrom(transform.Find("Content"));
			style.Dialog.Delimiter.GetFrom(transform.Find("Delimiter/Delimiter"));

			if (closeButton != null)
			{
				style.ButtonClose.GetFrom(closeButton);
			}
			else
			{
				style.ButtonClose.Background.GetFrom(transform.Find("Header/CloseButton"));
				style.ButtonClose.Text.GetFrom(transform.Find("Header/CloseButton/Text"));
			}

			if (DialogInfo != null)
			{
				DialogInfo.GetStyle(style.Dialog);
			}

			return true;
		}
		#endregion

		/// <summary>
		/// Upgrade fields data to the latest version.
		/// </summary>
		public virtual void Upgrade()
		{
#pragma warning disable 0618
			if ((buttonsTemplates.Count == 0) && (defaultButton != null))
			{
				buttonsTemplates.Add(defaultButton);
			}

			foreach (var btn in buttonsTemplates)
			{
				if (btn != null)
				{
					var info = Utilities.RequireComponent<DialogButtonComponentBase>(btn);
					info.Upgrade();
					if (info.NameAdapter == null)
					{
						Utilities.RequireComponent(info.GetComponentInChildren<Text>(true), ref info.NameAdapter);
					}
				}
			}

			if (ContentRoot == null)
			{
				ContentRoot = transform.Find("Content") as RectTransform;
			}

			if (dialogInfo == null)
			{
				dialogInfo = Utilities.RequireComponent<DialogInfoBase>(this);
				Utilities.RequireComponent(titleText, ref dialogInfo.TitleAdapter);
				Utilities.RequireComponent(contentText, ref dialogInfo.MessageAdapter);
				dialogInfo.Icon = Icon;
			}

			if (dialogInfo.ContentRoot == null)
			{
				dialogInfo.ContentRoot = ContentRoot;
			}
#pragma warning restore 0618
		}

#if UNITY_EDITOR
		/// <summary>
		/// Update layout when parameters changed.
		/// </summary>
		protected virtual void OnValidate()
		{
			Compatibility.Upgrade(this);
		}
#endif
	}
}