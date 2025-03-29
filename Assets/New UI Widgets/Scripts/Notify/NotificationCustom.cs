namespace UIWidgets
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Runtime.CompilerServices;
	using UIWidgets.Attributes;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Base generic class for the custom notifications.
	/// </summary>
	/// <typeparam name="TNotification">Notification type.</typeparam>
	public abstract partial class NotificationCustom<TNotification> : NotificationBase, IStylable, IUpgradeable, IHideable, INotifyCompletion
		where TNotification : NotificationCustom<TNotification>
	{
		[SerializeField]
		Button hideButton;

		/// <summary>
		/// Gets or sets the button that close current notification.
		/// </summary>
		/// <value>The hide button.</value>
		public Button HideButton
		{
			get => hideButton;

			set
			{
				if (hideButton != null)
				{
					hideButton.onClick.RemoveListener(ButtonHideClick);
				}

				hideButton = value;

				if (hideButton != null)
				{
					hideButton.onClick.AddListener(ButtonHideClick);
				}
			}
		}

		[SerializeField]
		[HideInInspector]
		[Obsolete("Replaced with NotifyInfo component.")]
		Text text;

		/// <summary>
		/// Gets or sets the text component.
		/// </summary>
		/// <value>The text.</value>
		[HideInInspector]
		[Obsolete("Replaced with NotifyInfo component.")]
		public Text Text
		{
			get => text;

			set => text = value;
		}

		/// <summary>
		/// Hide delay.
		/// </summary>
		[SerializeField]
		public float HideDelay = 10f;

		static Templates<TNotification> templates;

		/// <summary>
		/// Notifications templates.
		/// </summary>
		public static Templates<TNotification> Templates
		{
			get
			{
				templates ??= new Templates<TNotification>(AddCloseCallback);

				return templates;
			}

			set => templates = value;
		}

		/// <summary>
		/// Function used to run show animation.
		/// </summary>
		public Func<TNotification, IEnumerator> ShowAnimation;

		/// <summary>
		/// Function used to run hide animation.
		/// </summary>
		public Func<TNotification, IEnumerator> HideAnimation;

		Func<TNotification, IEnumerator> oldShowAnimation;

		Func<TNotification, IEnumerator> oldHideAnimation;

		Vector2 oldSize;

		Quaternion oldRotation;

		IEnumerator showCoroutine;

		IEnumerator hideCoroutine;

		/// <summary>
		/// Start slide up animations after hide current notification. Turn it off if its managed with HideAnimation.
		/// </summary>
		[Obsolete("Use the EasyLayout Movement animation.")]
		public bool SlideUpOnHide = true;

		[SerializeField]
		NotifyInfoBase notifyInfo;

		/// <summary>
		/// Gets the notification info.
		/// </summary>
		/// <value>The notification info.</value>
		public NotifyInfoBase NotifyInfo
		{
			get
			{
				if (notifyInfo == null)
				{
					TryGetComponent(out notifyInfo);
				}

				return notifyInfo;
			}
		}

		/// <summary>
		/// Callback on notification close.
		/// </summary>
		[Obsolete("Replaced with OnNotificationHide.")]
		public Action OnReturn;

		/// <summary>
		/// Callback on notification hide.
		/// </summary>
		public Action<TNotification> OnNotificationHide;

		#region buttons

		/// <summary>
		/// Button instance.
		/// </summary>
		public class ButtonInstance : DialogButtonCustom<TNotification, NotificationBase, NotificationButton>
		{
			/// <summary>
			/// Initializes a new instance of the <see cref="ButtonInstance"/> class.
			/// </summary>
			/// <param name="owner">Owner.</param>
			/// <param name="index">Index.</param>
			/// <param name="info">Info.</param>
			/// <param name="template">Template.</param>
			/// <param name="container">Button container.</param>
			public ButtonInstance(TNotification owner, int index, NotificationButton info, Button template, RectTransform container)
				: base(owner, index, info, template, container)
			{
			}
		}

		/// <summary>
		/// Class for the buttons instances.
		/// </summary>
		protected class ButtonsPool : DialogButtonsPoolCustom<TNotification, NotificationBase, ButtonInstance, NotificationButton>
		{
			/// <summary>
			/// Initializes a new instance of the <see cref="ButtonsPool"/> class.
			/// </summary>
			/// <param name="owner">Dialog.</param>
			/// <param name="templates">Templates.</param>
			/// <param name="container">Container.</param>
			/// <param name="active">List for the active buttons.</param>
			/// <param name="cache">List for the cached buttons.</param>
			public ButtonsPool(TNotification owner, IReadOnlyList<Button> templates, RectTransform container, List<ButtonInstance> active, List<List<ButtonInstance>> cache)
				: base(owner, templates, container, active, cache)
			{
			}

			/// <summary>
			/// Create the button.
			/// </summary>
			/// <param name="buttonIndex">Index of the button.</param>
			/// <param name="info">Button info.</param>
			/// <returns>Button.</returns>
			protected override ButtonInstance CreateButtonInstance(int buttonIndex, NotificationButton info)
			{
				return new ButtonInstance(Owner, buttonIndex, info, Templates[info.TemplateIndex], Container);
			}
		}

		ButtonsPool buttonsPool;

		/// <summary>
		/// Buttons pool.
		/// </summary>
		protected ButtonsPool Buttons
		{
			get
			{
				buttonsPool ??= new ButtonsPool(this as TNotification, ButtonsTemplates, ButtonsContainer, ButtonsActive, ButtonsCached);

				return buttonsPool;
			}
		}

		/// <summary>
		/// The buttons in use.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		protected List<ButtonInstance> ButtonsActive = new List<ButtonInstance>();

		/// <summary>
		/// Current buttons.
		/// </summary>
		public IReadOnlyList<ButtonInstance> CurrentButtons => ButtonsActive;

		/// <summary>
		/// The cached buttons.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		protected List<List<ButtonInstance>> ButtonsCached = new List<List<ButtonInstance>>();

		/// <summary>
		/// Buttons container.
		/// </summary>
		[SerializeField]
		protected RectTransform ButtonsContainer;

		[SerializeField]
		List<Button> buttonsTemplates = new List<Button>();

		/// <summary>
		/// Gets or sets the default buttons.
		/// </summary>
		/// <value>The default buttons.</value>
		public IReadOnlyList<Button> ButtonsTemplates
		{
			get
			{
				Upgrade();

				return buttonsTemplates;
			}

			set
			{
				if (IsTemplate && (buttonsTemplates.Count > value.Count))
				{
					throw new ArgumentOutOfRangeException("value", string.Format("Buttons count cannot be decreased. Current is {0}; New is {1}", buttonsTemplates.Count.ToString(), value.Count.ToString()));
				}

				Buttons.Replace(value);

				buttonsTemplates.Clear();
				buttonsTemplates.AddRange(value);

				if (buttonsTemplates.Count > 0)
				{
					Upgrade();
				}
			}
		}

		/// <summary>
		/// Creates the buttons.
		/// </summary>
		/// <param name="buttons">Buttons.</param>
		/// <returns>This instance.</returns>
		[Obsolete("Notifications now use NotificationButton instead of the DialogButton.")]
		public virtual TNotification SetButtons(IList<DialogButton> buttons)
		{
			buttonsPool?.Disable();

			if (buttons == null)
			{
				return this as TNotification;
			}

			for (int index = 0; index < buttons.Count; index++)
			{
				var old_info = buttons[index];
				var info = new NotificationButton(old_info.Label, old_info.Action, old_info.TemplateIndex);
				info.TemplateIndex = GetTemplateIndex(info);
				Buttons.Get(index, info);
			}

			return this as TNotification;
		}

		/// <summary>
		/// Creates the buttons.
		/// </summary>
		/// <param name="buttons">Buttons.</param>
		/// <returns>This instance.</returns>
		public virtual TNotification SetButtons(IList<NotificationButton> buttons = null)
		{
			buttonsPool?.Disable();

			if (buttons == null)
			{
				return this as TNotification;
			}

			for (int index = 0; index < buttons.Count; index++)
			{
				var info = buttons[index];
				info.TemplateIndex = GetTemplateIndex(info);
				Buttons.Get(index, info);
			}

			return this as TNotification;
		}

		/// <summary>
		/// Get button template index.
		/// </summary>
		/// <param name="button">Button.</param>
		/// <returns>Template index,</returns>
		protected int GetTemplateIndex(NotificationButton button)
		{
			var template = button.TemplateIndex;
			if (template < 0)
			{
				Debug.LogWarning(string.Format("Negative button index not supported. Button: {0}. Index: {1}.", button.Label, template.ToString()));
				template = 0;
			}

			if (template >= Buttons.Count)
			{
				Debug.LogWarning(string.Format(
					"Not found button template with index {0} for the button: {1}. Available indices: 0..{2}",
					template.ToString(),
					button.Label,
					(Buttons.Count - 1).ToString()));
				template = 0;
			}

			return template;
		}
		#endregion

		/// <summary>
		/// Gets the name of the template.
		/// </summary>
		/// <value>The name of the template.</value>
		public override string TemplateName
		{
			get
			{
				if (NotificationTemplateName == null)
				{
					Templates.FindTemplates();
				}

				return NotificationTemplateName;
			}

			set
			{
				NotificationTemplateName = value;
			}
		}

		/// <summary>
		/// Finds the templates.
		/// </summary>
		protected static void FindTemplates()
		{
			Templates.FindTemplates();
		}

		/// <summary>
		/// Opened notifications.
		/// </summary>
		protected static HashSet<TNotification> openedNotifications = new HashSet<TNotification>();

		/// <summary>
		/// List of the opened notifications.
		/// </summary>
		protected static List<TNotification> OpenedNotificationsList = new List<TNotification>();

		/// <summary>
		/// Opened notifications.
		/// </summary>
		public static IReadOnlyList<TNotification> OpenedNotifications
		{
			get
			{
				OpenedNotificationsList.Clear();
				OpenedNotificationsList.AddRange(openedNotifications);

				return OpenedNotificationsList;
			}
		}

		/// <summary>
		/// Inactive notifications with the same template.
		/// </summary>
		public List<TNotification> InactiveNotifications => Templates.CachedInstances(TemplateName);

		/// <summary>
		/// All notifications.
		/// </summary>
		public static List<TNotification> AllNotifications
		{
			get
			{
				var notifications = Templates.GetAll();
				notifications.AddRange(OpenedNotifications);

				return notifications;
			}
		}

		/// <summary>
		/// Count of the opened notifications.
		/// </summary>
		public static int Opened => openedNotifications.Count;

		/// <summary>
		/// Event on custom instance opened.
		/// The parameter is opened instances count.
		/// </summary>
		public static event Action<int> OnInstanceOpen;

		/// <summary>
		/// Event on custom instance closed.
		/// The parameter is opened instances count.
		/// </summary>
		public static event Action<int> OnInstanceClose;

		/// <summary>
		/// Get opened notifications.
		/// </summary>
		/// <param name="output">Output list.</param>
		public static void GetOpenedNotifications(List<TNotification> output)
		{
			output.AddRange(openedNotifications);
		}

#if UNITY_EDITOR && UNITY_2019_3_OR_NEWER
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		[DomainReload(nameof(templates), nameof(openedNotifications), nameof(OpenedNotificationsList), nameof(OnInstanceOpen), nameof(OnInstanceClose))]
		static void StaticInit()
		{
			templates = null;
			openedNotifications.Clear();
			OpenedNotificationsList.Clear();
			OnInstanceOpen = null;
			OnInstanceClose = null;
		}
#endif

		/// <summary>
		/// Process the start event.
		/// </summary>
		protected virtual void Start()
		{
			Buttons.Init();
		}

		/// <summary>
		/// Process the enable event.
		/// </summary>
		protected virtual void OnEnable()
		{
		}

		/// <summary>
		/// Process the disable event.
		/// </summary>
		protected virtual void OnDisable()
		{
		}

		/// <summary>
		/// Awake this instance.
		/// </summary>
		protected virtual void Awake()
		{
			if (IsTemplate)
			{
				gameObject.SetActive(false);
			}
		}

		/// <summary>
		/// Remove listeners and template.
		/// </summary>
		protected virtual void OnDestroy()
		{
			buttonsPool?.Disable();

			HideButton = null;

			if (!IsTemplate)
			{
				if (gameObject.activeSelf)
				{
					IsDestroyed = true;
					#pragma warning disable 0618
					SlideUpOnHide = false;
					#pragma warning restore 0618
					Hide();
				}

				templates = null;
				return;
			}

			// if FindTemplates never called than TemplateName == null
			if (TemplateName != null)
			{
				DeleteTemplate(TemplateName);
			}
		}

		/// <summary>
		/// Clears the cached instance of templates.
		/// </summary>
		public static void ClearCache()
		{
			Templates.ClearCache();
		}

		/// <summary>
		/// Clears the cached instance of specified template.
		/// </summary>
		/// <param name="templateName">Template name.</param>
		public static void ClearCache(string templateName)
		{
			Templates.ClearCache(templateName);
		}

		/// <summary>
		/// Gets the template by name.
		/// </summary>
		/// <returns>The template.</returns>
		/// <param name="template">Template name.</param>
		public static TNotification GetTemplate(string template)
		{
			return Templates.Get(template);
		}

		/// <summary>
		/// Deletes the template by name.
		/// </summary>
		/// <param name="template">Template.</param>
		public static void DeleteTemplate(string template)
		{
			Templates.Delete(template);
		}

		/// <summary>
		/// Adds the template.
		/// </summary>
		/// <param name="template">Template name.</param>
		/// <param name="notificationTemplate">Notification template object.</param>
		/// <param name="replace">If set to <c>true</c> replace.</param>
		public static void AddTemplate(string template, TNotification notificationTemplate, bool replace = true)
		{
			Templates.Add(template, notificationTemplate, replace);
		}

		/// <summary>
		/// Return notification instance by the specified template name.
		/// </summary>
		/// <param name="templateName">Template name.</param>
		/// <returns>New notification instance.</returns>
		[Obsolete("Use Clone(templateName) instead.")]
		public static TNotification Template(string templateName)
		{
			return Clone(templateName);
		}

		/// <summary>
		/// Return notification instance using current instance as template.
		/// </summary>
		/// <returns>New notification instance.</returns>
		[Obsolete("Use Clone() instead.")]
		public TNotification Template()
		{
			return Clone();
		}

		/// <summary>
		/// Return notification by the specified template name.
		/// </summary>
		/// <param name="templateName">Template name.</param>
		/// <returns>Returns new notification instance.</returns>
		public static TNotification Clone(string templateName)
		{
			return Templates.Instance(templateName);
		}

		/// <summary>
		/// Return notification instance using current instance as template.
		/// </summary>
		/// <returns>Returns new notification instance.</returns>
		public TNotification Clone()
		{
			var notification = this as TNotification;
			if ((TemplateName != null) && Templates.Exists(TemplateName))
			{
				// do nothing
			}
			else if (!Templates.Exists(gameObject.name))
			{
				Templates.Add(gameObject.name, notification);
			}
			else if (Templates.Get(gameObject.name) != notification)
			{
				Templates.Add(gameObject.name, notification);
			}

			var id = gameObject.GetInstanceID().ToString();
			if (!Templates.Exists(id))
			{
				Templates.Add(id, notification);
			}
			else if (Templates.Get(id) != notification)
			{
				Templates.Add(id, notification);
			}

			return Templates.Instance(id);
		}

		/// <summary>
		/// Adds the close callback.
		/// </summary>
		/// <param name="notification">Notification.</param>
		protected static void AddCloseCallback(TNotification notification)
		{
			if (notification.hideButton == null)
			{
				return;
			}

			notification.hideButton.onClick.AddListener(notification.ButtonHideClick);
		}

		/// <summary>
		/// Show notification.
		/// </summary>
		/// <param name="args">Arguments.</param>
		public virtual void Show(ShowArguments args)
		{
			Show(
				message: args.Message,
				customHideDelay: args.CustomHideDelay,
				container: args.Container,
				showAnimation: args.ShowAnimation,
				hideAnimation: args.HideAnimation,
				slideUpOnHide: args.SlideUpOnHide,
				sequenceType: args.SequenceType,
				sequenceDelay: args.SequenceDelay,
				clearSequence: args.ClearSequence,
				newUnscaledTime: args.UnscaledTime,
				content: args.Content,
				onReturn: null,
				onHide: args.OnHide);
		}

		/// <summary>
		/// Show the notification.
		/// </summary>
		/// <param name="message">Message. Also can be changed with SetMessage().</param>
		/// <param name="customHideDelay">Custom hide delay. Also can be changed with HideDelay field.</param>
		/// <param name="container">Container. Parent object for the current notification. Also can be changed with SetContainer().</param>
		/// <param name="showAnimation">Function used to run show animation. Also can be changed with ShowAnimation field.</param>
		/// <param name="hideAnimation">Function used to run hide animation. Also can be changed with HideAnimation field.</param>
		/// <param name="slideUpOnHide">Start slide up animations after hide current notification. Also can be changed with SlideUpOnHide field.</param>
		/// <param name="sequenceType">Add notification to sequence and display in order according specified sequenceType.</param>
		/// <param name="sequenceDelay">Time between previous notification was hidden and next will be showed. Also can be changed with SequenceDelay field.</param>
		/// <param name="clearSequence">Clear notifications sequence.</param>
		/// <param name="newUnscaledTime">Use unscaled time.</param>
		/// <param name="content">Content. Also can be changed with SetContent().</param>
		/// <param name="onReturn">(Obsolete) Action called when instance return to the cache. Also can be changed with OnReturn field.</param>
		/// <param name="onHide">Action called when instance return to the cache. Also can be changed with OnNotificationHide field.</param>
		public virtual void Show(
			string message = null,
			float? customHideDelay = null,
			RectTransform container = null,
			Func<TNotification, IEnumerator> showAnimation = null,
			Func<TNotification, IEnumerator> hideAnimation = null,
			bool? slideUpOnHide = null,
			NotifySequence sequenceType = NotifySequence.None,
			float sequenceDelay = 0.3f,
			bool clearSequence = false,
			bool? newUnscaledTime = null,
			RectTransform content = null,
			Action onReturn = null,
			Action<TNotification> onHide = null)
		{
			CloseOnButtonClick = true;
			if (IsTemplate)
			{
				Debug.LogWarning("Use the template clone, not the template itself: NotificationTemplate.Clone().Show(...), not NotificationTemplate.Show(...)");
			}

			if (clearSequence)
			{
				ClearSequence();
			}

#pragma warning disable 0618
			OnReturn = onReturn;
#pragma warning restore 0618

			OnNotificationHide = onHide;
			SequenceDelay = sequenceDelay;

			oldShowAnimation = ShowAnimation;
			oldHideAnimation = HideAnimation;

			var rt = transform as RectTransform;
			oldSize = rt.rect.size;
			oldRotation = rt.localRotation;

			SetMessage(message, null);
			SetContent(content);

			SetContainer(container);

			if (newUnscaledTime != null)
			{
				UnscaledTime = (bool)newUnscaledTime;
			}

			if (customHideDelay != null)
			{
				HideDelay = (float)customHideDelay;
			}

			#pragma warning disable 0618
			if (slideUpOnHide != null)
			{
				SlideUpOnHide = (bool)slideUpOnHide;
			}
			#pragma warning restore 0618

			if (showAnimation != null)
			{
				ShowAnimation = showAnimation;
			}

			if (hideAnimation != null)
			{
				HideAnimation = hideAnimation;
			}

			if (sequenceType != NotifySequence.None)
			{
				NotifyManager.Add(this, sequenceType);
			}
			else
			{
				Display();
			}
		}

		/// <summary>
		/// Show notification.
		/// </summary>
		/// <param name="args">Arguments.</param>
		/// <returns>Index of the selected button or -1 if notification was hidden after delay or on hide button click.</returns>
		public virtual NotificationCustom<TNotification> ShowAsync(ShowArguments args)
		{
			return ShowAsync(
				message: args.Message,
				customHideDelay: args.CustomHideDelay,
				container: args.Container,
				showAnimation: args.ShowAnimation,
				hideAnimation: args.HideAnimation,
				slideUpOnHide: args.SlideUpOnHide,
				sequenceType: args.SequenceType,
				sequenceDelay: args.SequenceDelay,
				clearSequence: args.ClearSequence,
				newUnscaledTime: args.UnscaledTime,
				content: args.Content);
		}

		/// <summary>
		/// Show notification.
		/// </summary>
		/// <param name="message">Message. Also can be changed with SetMessage().</param>
		/// <param name="customHideDelay">Custom hide delay. Also can be changed with HideDelay field.</param>
		/// <param name="container">Container. Parent object for the current notification. Also can be changed with SetContainer().</param>
		/// <param name="showAnimation">Function used to run show animation. Also can be changed with ShowAnimation field.</param>
		/// <param name="hideAnimation">Function used to run hide animation. Also can be changed with HideAnimation field.</param>
		/// <param name="slideUpOnHide">Start slide up animations after hide current notification. Also can be changed with SlideUpOnHide field.</param>
		/// <param name="sequenceType">Add notification to sequence and display in order according specified sequenceType.</param>
		/// <param name="sequenceDelay">Time between previous notification was hidden and next will be showed. Also can be changed with SequenceDelay field.</param>
		/// <param name="clearSequence">Clear notifications sequence.</param>
		/// <param name="newUnscaledTime">Use unscaled time.</param>
		/// <param name="content">Content. Also can be changed with SetContent().</param>
		/// <param name="closeOnButtonClick">Close notification on button click.</param>
		/// <returns>Index of the selected button or -1 if notification was hidden after delay or on hide button click.</returns>
		public virtual NotificationCustom<TNotification> ShowAsync(
			string message = null,
			float? customHideDelay = null,
			RectTransform container = null,
			Func<TNotification, IEnumerator> showAnimation = null,
			Func<TNotification, IEnumerator> hideAnimation = null,
			bool? slideUpOnHide = null,
			NotifySequence sequenceType = NotifySequence.None,
			float sequenceDelay = 0.3f,
			bool clearSequence = false,
			bool? newUnscaledTime = null,
			RectTransform content = null,
			bool closeOnButtonClick = true)
		{
			Show(message, customHideDelay, container, showAnimation, hideAnimation, slideUpOnHide, sequenceType, sequenceDelay, clearSequence, newUnscaledTime, content);
			CloseOnButtonClick = closeOnButtonClick;

			return this;
		}

		/// <summary>
		/// Set message text.
		/// </summary>
		/// <param name="message">Message text.</param>
		public virtual void SetMessage(string message)
		{
			NotifyInfo.SetInfo(message, null);
		}

		/// <summary>
		/// Set message text.
		/// </summary>
		/// <param name="message">Message text.</param>
		/// <param name="args">An object array that contains zero or more objects to message format.</param>
		public virtual void SetMessage(string message, params object[] args)
		{
			NotifyInfo.SetInfo(message, args);
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

			NotifyInfo.SetContent(content);
		}

		Action OnHideCallback;

		/// <summary>
		/// Display notification.
		/// </summary>
		/// <param name="onHideCallback">On hide callback.</param>
		public override void Display(Action onHideCallback = null)
		{
			CloseOnButtonClick = true;
			transform.SetAsLastSibling();
			gameObject.SetActive(true);

			OnHideCallback = onHideCallback;

			if (ShowAnimation != null)
			{
				showCoroutine = ShowAnimation(this as TNotification);
				StartCoroutine(showCoroutine);
			}
			else
			{
				showCoroutine = null;
			}

			if (HideDelay > 0.0f)
			{
				hideCoroutine = HideCoroutine();
				StartCoroutine(hideCoroutine);
			}
			else
			{
				hideCoroutine = null;
			}

			InstanceOpened();
		}

		/// <summary>
		/// Display notification.
		/// </summary>
		/// <param name="onHideCallback">On hide callback.</param>
		/// <param name="closeOnButtonClick">Close dialog on button click.</param>
		/// <returns>Index of the selected button or -1 if notification was hidden after delay or on hide button click</returns>
		public virtual NotificationCustom<TNotification> DisplayAsync(Action onHideCallback = null, bool closeOnButtonClick = true)
		{
			Display(onHideCallback);
			CloseOnButtonClick = closeOnButtonClick;

			return this;
		}

		/// <inheritdoc/>
		protected override void InstanceOpened()
		{
			openedNotifications.Add(this as TNotification);
			OnInstanceOpen?.Invoke(openedNotifications.Count);

			base.InstanceOpened();
		}

		/// <inheritdoc/>
		protected override void InstanceClosed()
		{
			openedNotifications.Remove(this as TNotification);
			OnInstanceClose?.Invoke(openedNotifications.Count);

			base.InstanceClosed();
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0401:Possible allocation of reference type enumerator", Justification = "Enumerator is reusable.")]
		IEnumerator HideCoroutine()
		{
			yield return UtilitiesTime.Wait(HideDelay, UnscaledTime);

			if (HideAnimation != null)
			{
				yield return StartCoroutine(HideAnimation(this as TNotification));
			}

			Complete(-1);
		}

		/// <summary>
		/// Hide notification.
		/// </summary>
		public virtual void ButtonHideClick()
		{
			Complete(-1);
		}

		/// <summary>
		/// Hide notification.
		/// </summary>
		public override void Hide()
		{
			Complete(-1, true);
		}

		/// <summary>
		/// Return this instance to cache.
		/// </summary>
		public override void Return()
		{
#pragma warning disable 0618
			if (OnReturn != null)
			{
				OnReturn();
				OnReturn = null;
			}
#pragma warning restore 0618

			if (OnNotificationHide != null)
			{
				OnNotificationHide(this as TNotification);
				OnNotificationHide = null;
			}

			buttonsPool?.Disable();

			Templates.ToCache(this as TNotification);

			ShowAnimation = oldShowAnimation;
			HideAnimation = oldHideAnimation;

			var rt = transform as RectTransform;
			rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, oldSize.x);
			rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, oldSize.y);
			rt.localRotation = oldRotation;

			if (TryGetComponent<LayoutElement>(out var le))
			{
				le.ignoreLayout = false;
			}

			NotifyInfo.RestoreDefaultValues();
			base.Return();
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
		public override bool IsAsync
		{
			get
			{
				return Continuation != null;
			}
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
		public virtual NotificationCustom<TNotification> GetAwaiter()
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

		/// <inheritdoc/>
		public override void Complete(int buttonIndex)
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

			#pragma warning disable 0618
			if (SlideUpOnHide)
			{
				var replacement = GetReplacement(this as TNotification);
				var slide = Utilities.RequireComponent<SlideUp>(replacement);
				slide.UnscaledTime = UnscaledTime;
				slide.Run();
			}
			#pragma warning restore 0618

			OnHideCallback?.Invoke();

			Return();
		}

		#endregion

		/// <summary>
		/// Upgrade fields data to the latest version.
		/// </summary>
		public virtual void Upgrade()
		{
#pragma warning disable 0618
			if (notifyInfo == null)
			{
				notifyInfo = gameObject.AddComponent<NotifyInfoBase>();
				Utilities.RequireComponent(text, ref notifyInfo.MessageAdapter);
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

		#region IStylable implementation

		/// <inheritdoc/>
		public virtual bool SetStyle(Style style)
		{
			style.Notify.Background.ApplyTo(transform.Find("Background"));

			if (NotifyInfo != null)
			{
				NotifyInfo.SetStyle(style);
			}

			if (HideButton != null)
			{
				style.ButtonClose.Background.ApplyTo(HideButton);
				style.ButtonClose.Text.ApplyTo(HideButton.transform.Find("Text"));
			}

			if (buttonsPool != null)
			{
				buttonsPool.SetStyle(style.Dialog.Button);
			}
			else
			{
				for (int template_index = 0; template_index < ButtonsTemplates.Count; template_index++)
				{
					style.Dialog.Button.ApplyTo(ButtonsTemplates[template_index].gameObject);
				}
			}

			return true;
		}

		/// <inheritdoc/>
		public virtual bool GetStyle(Style style)
		{
			style.Notify.Background.GetFrom(transform.Find("Background"));

			if (NotifyInfo != null)
			{
				NotifyInfo.GetStyle(style);
			}

			if (HideButton != null)
			{
				style.ButtonClose.Background.GetFrom(HideButton);
				style.ButtonClose.Text.GetFrom(HideButton.transform.Find("Text"));
			}

			for (int template_index = 0; template_index < ButtonsTemplates.Count; template_index++)
			{
				style.Dialog.Button.GetFrom(ButtonsTemplates[template_index].gameObject);
			}

			return true;
		}

		#endregion
	}
}