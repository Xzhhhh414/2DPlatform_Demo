namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using System.Runtime.CompilerServices;
	using UIWidgets.Attributes;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Popup.
	/// </summary>
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/widgets/dialogs/popup.html")]
	public class Popup : MonoBehaviourInitiable, ITemplatable, IStylable, IUpgradeable, INotifyCompletion
	{
		[SerializeField]
		[HideInInspector]
		[Obsolete("Replaced with Info component.")]
		Text titleText;

		/// <summary>
		/// Gets or sets the text component.
		/// </summary>
		/// <value>The text.</value>
		[Obsolete("Replaced with Info component.")]
		public Text TitleText
		{
			get => titleText;

			set => titleText = value;
		}

		[SerializeField]
		[HideInInspector]
		[Obsolete("Replaced with Info component.")]
		Text contentText;

		/// <summary>
		/// Gets or sets the text component.
		/// </summary>
		/// <value>The text.</value>
		[Obsolete("Replaced with Info component.")]
		public Text ContentText
		{
			get => contentText;

			set => contentText = value;
		}

		[SerializeField]
		[HideInInspector]
		[Obsolete("Replaced with Info component.")]
		Image popupIcon;

		/// <summary>
		/// Gets or sets the icon component.
		/// </summary>
		/// <value>The icon.</value>
		[Obsolete("Replaced with Info component.")]
		public Image Icon
		{
			get => popupIcon;

			set => popupIcon = value;
		}

		[SerializeField]
		DialogInfoBase info;

		/// <summary>
		/// Gets the info component.
		/// </summary>
		/// <value>The info component.</value>
		public DialogInfoBase Info
		{
			get
			{
				if (info == null)
				{
					TryGetComponent(out info);
				}

				return info;
			}
		}

		/// <summary>
		/// Set focus to the last Selectable object in dialog.
		/// </summary>
		[SerializeField]
		[Tooltip("Set focus to the last Selectable object in dialog.")]
		public bool AutoFocus;

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

		/// <summary>
		/// Gets the name of the template.
		/// </summary>
		/// <value>The name of the template.</value>
		public string TemplateName
		{
			get;
			set;
		}

		static Templates<Popup> templates;

		/// <summary>
		/// Popup templates.
		/// </summary>
		public static Templates<Popup> Templates
		{
			get
			{
				templates ??= new Templates<Popup>();

				return templates;
			}

			set => templates = value;
		}

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
					closeButton.onClick.RemoveListener(Close);
				}

				closeButton = value;

				if (IsInited && (closeButton != null))
				{
					closeButton.onClick.AddListener(Close);
				}
			}
		}

		/// <summary>
		/// Callback on popup close.
		/// </summary>
		public Action OnClose;

		/// <summary>
		/// Hide on modal click.
		/// </summary>
		[SerializeField]
		public bool HideOnModalClick = true;

		/// <summary>
		/// Opened popups.
		/// </summary>
		protected static HashSet<Popup> openedPopups = new HashSet<Popup>();

		/// <summary>
		/// List of the opened popups.
		/// </summary>
		protected static List<Popup> OpenedPopupsList = new List<Popup>();

		/// <summary>
		/// Opened popups.
		/// </summary>
		public static IReadOnlyList<Popup> OpenedPopups
		{
			get
			{
				OpenedPopupsList.Clear();
				OpenedPopupsList.AddRange(openedPopups);

				return OpenedPopupsList;
			}
		}

		/// <summary>
		/// Inactive popups with the same template.
		/// </summary>
		public List<Popup> InactivePopups => Templates.CachedInstances(TemplateName);

		/// <summary>
		/// All popup.
		/// </summary>
		public static List<Popup> AllPopups
		{
			get
			{
				var popups = Templates.GetAll();
				popups.AddRange(openedPopups);

				return popups;
			}
		}

		/// <summary>
		/// Count of the opened popups.
		/// </summary>
		public static int Opened => openedPopups.Count;

		/// <summary>
		/// Is instance destroyed?
		/// </summary>
		public bool IsDestroyed
		{
			get;
			protected set;
		}

		/// <summary>
		/// Event on any instance opened.
		/// The parameter is opened instances count.
		/// </summary>
		public static event Action<int> OnBaseInstanceOpen;

		/// <summary>
		/// Event on any instance closed.
		/// The parameter is opened instances count.
		/// </summary>
		public static event Action<int> OnBaseInstanceClose;

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

		/// <inheritdoc/>
		protected override void InitOnce()
		{
			base.InitOnce();

			if (closeButton != null)
			{
				closeButton.onClick.AddListener(Close);
			}
		}

#if UNITY_EDITOR && UNITY_2019_3_OR_NEWER
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		[DomainReload(nameof(openedPopups), nameof(OpenedPopupsList), nameof(OnBaseInstanceOpen), nameof(OnBaseInstanceClose), nameof(templates))]
		static void StaticInit()
		{
			openedPopups.Clear();
			OpenedPopupsList.Clear();
			OnBaseInstanceOpen = null;
			OnBaseInstanceClose = null;
			templates = null;
		}
#endif

		/// <summary>
		/// This function is called when the MonoBehaviour will be destroyed.
		/// </summary>
		protected virtual void OnDestroy()
		{
			Position.ParentDestroyed();

			if (!IsTemplate)
			{
				if (gameObject.activeSelf)
				{
					IsDestroyed = true;
					AsyncResult = false;
					Close();
				}

				templates = null;
				return;
			}

			// if FindTemplates never called than TemplateName == null
			if (TemplateName != null)
			{
				Templates.Delete(TemplateName);
			}
		}

		/// <summary>
		/// Return popup instance by the specified template name.
		/// </summary>
		/// <param name="templateName">Template name.</param>
		/// <returns>New popup instance.</returns>
		[Obsolete("Use Clone(templateName) instead.")]
		public static Popup Template(string templateName)
		{
			return Clone(templateName);
		}

		/// <summary>
		/// Return popup instance using current instance as template.
		/// </summary>
		/// <returns>New popup instance.</returns>
		[Obsolete("Use Clone() instead.")]
		public Popup Template()
		{
			return Clone();
		}

		/// <summary>
		/// Return popup instance by the specified template name.
		/// </summary>
		/// <param name="templateName">Template name.</param>
		/// <returns>New popup instance.</returns>
		public static Popup Clone(string templateName)
		{
			return Templates.Instance(templateName);
		}

		/// <summary>
		/// Return popup instance using current instance as template.
		/// </summary>
		/// <returns>New popup instance.</returns>
		public Popup Clone()
		{
			if ((TemplateName != null) && Templates.Exists(TemplateName))
			{
				// do nothing
			}
			else if (!Templates.Exists(gameObject.name))
			{
				Templates.Add(gameObject.name, this);
			}
			else if (Templates.Get(gameObject.name) != this)
			{
				Templates.Add(gameObject.name, this);
			}

			var id = gameObject.GetInstanceID().ToString();
			if (!Templates.Exists(id))
			{
				Templates.Add(id, this);
			}
			else if (Templates.Get(id) != this)
			{
				Templates.Add(id, this);
			}

			return Templates.Instance(id);
		}

		/// <summary>
		/// The modal ID.
		/// </summary>
		protected InstanceID? ModalKey;

		/// <summary>
		/// Hierarchy position.
		/// </summary>
		protected HierarchyPosition Position;

		/// <summary>
		/// Canvas resize subscription.
		/// </summary>
		protected Subscription CanvasResize;

		/// <summary>
		/// Show popup.
		/// </summary>
		/// <param name="title">Title.</param>
		/// <param name="message">Message.</param>
		/// <param name="position">Position.</param>
		/// <param name="icon">Icon.</param>
		/// <param name="modal">If set to <c>true</c> modal.</param>
		/// <param name="modalSprite">Modal sprite.</param>
		/// <param name="modalColor">Modal color.</param>
		/// <param name="canvas">Canvas.</param>
		/// <param name="content">Content. Also can be changed with SetContent().</param>
		/// <param name="onClose">On close callback. Also can be changed with OnClose field.</param>
		public virtual void Show(
			string title = null,
			string message = null,
			Vector3? position = null,
			Sprite icon = null,
			bool modal = false,
			Sprite modalSprite = null,
			Color? modalColor = null,
			Canvas canvas = null,
			RectTransform content = null,
			Action onClose = null)
		{
			AsyncResult = true;

			if (IsTemplate)
			{
				Debug.LogWarning("Use the template clone, not the template itself: PopupTemplate.Clone().Show(...), not PopupTemplate.Show(...)");
			}

			OnClose = onClose;

			SetInfo(title, null, message, null, icon);
			SetContent(content);

			var canvas_rt = SetCanvas(canvas);

			SetModal(modal, modalSprite, modalColor, canvas_rt);

			if (position.HasValue)
			{
				SetPosition(position.Value);
			}

			gameObject.SetActive(true);
			InstanceOpened();
		}

		/// <summary>
		/// Show popup.
		/// </summary>
		/// <param name="title">Title.</param>
		/// <param name="message">Message.</param>
		/// <param name="position">Position.</param>
		/// <param name="icon">Icon.</param>
		/// <param name="modal">If set to <c>true</c> modal.</param>
		/// <param name="modalSprite">Modal sprite.</param>
		/// <param name="modalColor">Modal color.</param>
		/// <param name="canvas">Canvas.</param>
		/// <param name="content">Content. Also can be changed with SetContent().</param>
		/// <returns>This instance.</returns>
		public virtual Popup ShowAsync(
			string title = null,
			string message = null,
			Vector3? position = null,
			Sprite icon = null,
			bool modal = false,
			Sprite modalSprite = null,
			Color? modalColor = null,
			Canvas canvas = null,
			RectTransform content = null)
		{
			Show(title, message, position, icon, modal, modalSprite, modalColor, canvas, content);
			return this;
		}

		/// <summary>
		/// Instance opened.
		/// </summary>
		protected virtual void InstanceOpened()
		{
			if (AutoFocus)
			{
				UtilitiesUI.SelectChild(this, closeButton);
			}

			openedPopups.Add(this);
			OnBaseInstanceOpen?.Invoke(openedPopups.Count);
		}

		/// <summary>
		/// Instance closed.
		/// </summary>
		protected virtual void InstanceClosed()
		{
			openedPopups.Remove(this);
			OnBaseInstanceClose?.Invoke(openedPopups.Count);
		}

		/// <summary>
		/// Set canvas.
		/// </summary>
		/// <param name="canvas">Canvas.</param>
		/// <returns>Canvas RectTransform.</returns>
		public virtual RectTransform SetCanvas(Canvas canvas)
		{
			var parent = (canvas != null) ? canvas.transform as RectTransform : UtilitiesUI.FindTopmostCanvas(gameObject.transform);

			Position.Restore();
			Position = HierarchyPosition.SetParent(transform, parent);

			var resize = Utilities.RequireComponent<ResizeListener>(parent);
			CanvasResize.Clear();
			CanvasResize = new Subscription(resize.OnResize, RefreshPosition);

			return parent;
		}

		/// <summary>
		/// Refresh position.
		/// </summary>
		protected virtual void RefreshPosition() => Position.Refresh();

		/// <summary>
		/// Set modal mode.
		/// Warning: modal block is created at the current root canvas.
		/// </summary>
		/// <param name="modal">If set to <c>true</c> modal.</param>
		/// <param name="modalSprite">Modal sprite.</param>
		/// <param name="modalColor">Modal color.</param>
		/// <param name="parent">Parent.</param>
		public virtual void SetModal(bool modal = false, Sprite modalSprite = null, Color? modalColor = null, RectTransform parent = null)
		{
			ModalHelper.Close(ref ModalKey);

			if (modal)
			{
				ModalKey = ModalHelper.Open(this, modalSprite, modalColor, ProcessModalClick, parent);
			}

			if (Position.Changed)
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
				AsyncResult = false;
				Close();
			}
		}

		/// <summary>
		/// Set position.
		/// </summary>
		/// <param name="position">Position.</param>
		public virtual void SetPosition(Vector3 position)
		{
			transform.localPosition = position;
		}

		/// <summary>
		/// Sets the info.
		/// </summary>
		/// <param name="title">Title.</param>
		/// <param name="message">Message.</param>
		/// <param name="icon">Icon.</param>
		public virtual void SetInfo(string title = null, string message = null, Sprite icon = null)
		{
			Info.SetInfo(title, null, message, null, icon);
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
			Info.SetInfo(title, titleArgs, message, messageArgs, icon);
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

			Info.SetContent(content);
		}

		/// <summary>
		/// Close popup.
		/// </summary>
		public virtual void Close()
		{
			OnClose?.Invoke();

			ModalHelper.Close(ref ModalKey);
			Position.Restore();
			CanvasResize.Clear();

			Complete();
			Return();
		}

		/// <summary>
		/// Return this instance to cache.
		/// </summary>
		protected virtual void Return()
		{
			Templates.ToCache(this);

			ResetParameters();

			InstanceClosed();
		}

		/// <summary>
		/// Resets the parameters.
		/// </summary>
		protected virtual void ResetParameters()
		{
			OnClose = null;

			info.RestoreDefaultValues();
		}

		/// <summary>
		/// Upgrade fields data to the latest version.
		/// </summary>
		public virtual void Upgrade()
		{
#pragma warning disable 0618
			if (info == null)
			{
				info = gameObject.AddComponent<DialogInfoBase>();
				Utilities.RequireComponent(titleText, ref info.TitleAdapter);
				Utilities.RequireComponent(contentText, ref info.MessageAdapter);
				info.Icon = Icon;
			}

			if (info.ContentRoot == null)
			{
				info.ContentRoot = transform.Find("Content") as RectTransform;
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

		#region async

		Action Continuation;

		/// <summary>
		/// Async result.
		/// </summary>
		protected bool AsyncResult;

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
		public virtual Popup GetAwaiter()
		{
			IsCompleted = false;
			return this;
		}

		/// <summary>
		/// Ends the wait for the completion of the asynchronous task.
		/// </summary>
		/// <returns>Result.</returns>
		public virtual bool GetResult()
		{
			return AsyncResult;
		}

		/// <summary>
		/// Sets the action to perform when the this object stops waiting for the asynchronous task to complete.
		/// </summary>
		/// <param name="continuation">The action to perform when the wait operation completes.</param>
		public void OnCompleted(Action continuation)
		{
			Continuation = continuation;
		}

		/// <summary>
		/// Complete asynchronous task.
		/// </summary>
		protected void Complete()
		{
			if (Continuation != null)
			{
				IsCompleted = true;

				var c = Continuation;
				Continuation = null;
				c?.Invoke();
			}
		}

		#endregion

		#region IStylable implementation

		/// <inheritdoc/>
		public bool SetStyle(Style style)
		{
			if (TryGetComponent<Image>(out var bg))
			{
				style.Dialog.Background.ApplyTo(bg);
			}

			style.Dialog.ContentBackground.ApplyTo(transform.Find("Content"));
			style.Dialog.Delimiter.ApplyTo(transform.Find("Delimiter/Delimiter"));
			style.Dialog.Button.ApplyTo(transform.Find("Buttons/Close"));

			if (closeButton != null)
			{
				style.ButtonClose.ApplyTo(closeButton);
			}
			else
			{
				style.ButtonClose.Background.ApplyTo(transform.Find("Header/CloseButton"));
				style.ButtonClose.Text.ApplyTo(transform.Find("Header/CloseButton/Text"));
			}

			if (Info != null)
			{
				Info.SetStyle(style.Dialog);
			}

			return true;
		}

		/// <inheritdoc/>
		public bool GetStyle(Style style)
		{
			if (TryGetComponent<Image>(out var bg))
			{
				style.Dialog.Background.GetFrom(bg);
			}

			style.Dialog.ContentBackground.GetFrom(transform.Find("Content"));
			style.Dialog.Delimiter.GetFrom(transform.Find("Delimiter/Delimiter"));
			style.Dialog.Button.GetFrom(transform.Find("Buttons/Close"));

			if (closeButton != null)
			{
				style.ButtonClose.GetFrom(closeButton);
			}
			else
			{
				style.ButtonClose.Background.GetFrom(transform.Find("Header/CloseButton"));
				style.ButtonClose.Text.GetFrom(transform.Find("Header/CloseButton/Text"));
			}

			if (Info != null)
			{
				Info.GetStyle(style.Dialog);
			}

			return true;
		}
		#endregion
	}
}