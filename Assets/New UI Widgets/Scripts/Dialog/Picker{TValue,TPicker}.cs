namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using UIWidgets.Attributes;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Base custom class for Pickers.
	/// </summary>
	/// <typeparam name="TValue">Type of value.</typeparam>
	/// <typeparam name="TPicker">Type of picker.</typeparam>
	public abstract class Picker<TValue, TPicker> : Picker
		where TPicker : Picker<TValue, TPicker>
	{
		/// <summary>
		/// Picker result.
		/// </summary>
		public readonly struct Result
		{
			/// <summary>
			/// Selected value or default value if nothing selected.
			/// </summary>
			public readonly TValue Value;

			/// <summary>
			/// True if value was selected; otherwise false.
			/// </summary>
			public readonly bool Success;

			/// <summary>
			/// Initializes a new instance of the <see cref="Result"/> struct.
			/// </summary>
			/// <param name="value">Value.</param>
			/// <param name="success">True if value was selected; otherwise false.</param>
			public Result(TValue value, bool success = true)
			{
				Value = value;
				Success = success;
			}

			/// <summary>
			/// Convert this instance to value.
			/// </summary>
			/// <param name="result">Picker result.</param>
			public static implicit operator TValue(Result result) => result.Value;

			/// <summary>
			/// Returns a string that represents the selected value.
			/// </summary>
			/// <returns>A string that represents the selected value.</returns>
			public override string ToString() => Value.ToString();
		}

		static Templates<TPicker> templates;

		/// <summary>
		/// Picker templates.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes", Justification = "Reviewed")]
		public static Templates<TPicker> Templates
		{
			get
			{
				templates ??= new Templates<TPicker>();

				return templates;
			}

			set => templates = value;
		}

		/// <summary>
		/// Opened pickers.
		/// </summary>
		protected static HashSet<TPicker> openedPickers = new HashSet<TPicker>();

		/// <summary>
		/// List of the opened pickers.
		/// </summary>
		protected static List<TPicker> OpenedPickersList = new List<TPicker>();

		/// <summary>
		/// Opened pickers.
		/// </summary>
		public static IReadOnlyList<TPicker> OpenedPickers
		{
			get
			{
				OpenedPickersList.Clear();
				OpenedPickersList.AddRange(openedPickers);

				return OpenedPickersList;
			}
		}

		/// <summary>
		/// Inactive pickers with the same template.
		/// </summary>
		public List<TPicker> InactivePickers => Templates.CachedInstances(TemplateName);

		/// <summary>
		/// All pickers.
		/// </summary>
		public static List<TPicker> AllPickers
		{
			get
			{
				var pickers = Templates.GetAll();
				pickers.AddRange(openedPickers);

				return pickers;
			}
		}

		/// <summary>
		/// Count of the opened pickers.
		/// </summary>
		public static int Opened => openedPickers.Count;

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
		/// Get opened pickers.
		/// </summary>
		/// <param name="output">Output list.</param>
		public static void GetOpenedPickers(List<TPicker> output) => output.AddRange(openedPickers);

#if UNITY_EDITOR && UNITY_2019_3_OR_NEWER
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		[DomainReload(nameof(openedPickers), nameof(OpenedPickersList), nameof(OnInstanceOpen), nameof(OnInstanceClose), nameof(templates))]
		static void StaticInit()
		{
			openedPickers.Clear();
			OpenedPickersList.Clear();
			OnInstanceOpen = null;
			OnInstanceClose = null;
			templates = null;
		}
#endif

		/// <summary>
		/// Value.
		/// </summary>
		protected TValue Value;

		/// <inheritdoc/>
		protected override void OnDestroy()
		{
			base.OnDestroy();

			if (!IsTemplate)
			{
				if (gameObject.activeSelf)
				{
					IsDestroyed = true;
					Cancel();
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
		/// Return picker instance using current instance as template.
		/// </summary>
		/// <returns>New picker instance.</returns>
		[Obsolete("Use Clone() instead.")]
		public TPicker Template()
		{
			return Clone();
		}

		/// <summary>
		/// Return picker instance using current instance as template.
		/// </summary>
		/// <returns>New picker instance.</returns>
		public virtual TPicker Clone()
		{
			var picker = this as TPicker;
			if ((TemplateName != null) && Templates.Exists(TemplateName))
			{
				// do nothing
			}
			else if (!Templates.Exists(gameObject.name))
			{
				Templates.Add(gameObject.name, picker);
			}
			else if (Templates.Get(gameObject.name) != this)
			{
				Templates.Add(gameObject.name, picker);
			}

			var id = gameObject.GetInstanceID().ToString();
			if (!Templates.Exists(id))
			{
				Templates.Add(id, picker);
			}
			else if (Templates.Get(id) != this)
			{
				Templates.Add(id, picker);
			}

			return Templates.Instance(id);
		}

		/// <summary>
		/// Callback with selected value.
		/// </summary>
		protected Action<TValue> OnSelect;

		/// <summary>
		/// Show picker.
		/// </summary>
		/// <param name="defaultValue">Default value.</param>
		/// <param name="onSelect">Callback with selected value.</param>
		/// <param name="onCancel">Callback when picker closed without any value selected.</param>
		/// <param name="modalSprite">Modal sprite.</param>
		/// <param name="modalColor">Modal color.</param>
		/// <param name="canvas">Canvas.</param>
		public virtual void Show(
			TValue defaultValue = default,
			Action<TValue> onSelect = null,
			Action onCancel = null,
			Sprite modalSprite = null,
			Color? modalColor = null,
			Canvas canvas = null)
		{
			AsyncResult = new Result(defaultValue, false);

			if (modalColor == null)
			{
				modalColor = new Color(0, 0, 0, 0.8f);
			}

			if (IsTemplate)
			{
				Debug.LogWarning("Use the template clone, not the template itself: PickerTemplate.Clone().Show(...), not PickerTemplate.Show(...)");
			}

			OnSelect = onSelect;
			OnCancel = onCancel;

			var canvas_rt = SetCanvas(canvas);
			SetModal(modalSprite, modalColor, canvas_rt);

			gameObject.SetActive(true);

			BeforeOpen(defaultValue);
			InstanceOpened();
		}

		/// <summary>
		/// Show picker.
		/// </summary>
		/// <param name="defaultValue">Default value.</param>
		/// <param name="modalSprite">Modal sprite.</param>
		/// <param name="modalColor">Modal color.</param>
		/// <param name="canvas">Canvas.</param>
		/// <returns>Selected value and confirmation.</returns>
		public virtual Picker<TValue, TPicker> ShowAsync(
			TValue defaultValue = default,
			Sprite modalSprite = null,
			Color? modalColor = null,
			Canvas canvas = null)
		{
			Show(defaultValue, modalSprite: modalSprite, modalColor: modalColor, canvas: canvas);
			return this;
		}

		/// <inheritdoc/>
		protected override void InstanceOpened()
		{
			if (AutoFocus)
			{
				UtilitiesUI.SelectChild(this, null);
			}

			openedPickers.Add(this as TPicker);
			OnInstanceOpen?.Invoke(openedPickers.Count);

			base.InstanceOpened();
		}

		/// <inheritdoc/>
		protected override void InstanceClosed()
		{
			openedPickers.Remove(this as TPicker);
			OnInstanceClose?.Invoke(openedPickers.Count);

			base.InstanceClosed();
		}

		/// <summary>
		/// Close picker with specified value.
		/// </summary>
		/// <param name="value">Value.</param>
		public virtual void Selected(TValue value)
		{
			OnSelect?.Invoke(value);

			AsyncResult = new Result(value);
			Complete();
			Close();
		}

		/// <summary>
		/// Prepare picker to open.
		/// </summary>
		/// <param name="defaultValue">Default value.</param>
		public virtual void BeforeOpen(TValue defaultValue) => Value = defaultValue;

		/// <inheritdoc/>
		protected override void Return()
		{
			Templates.ToCache(this as TPicker);

			InstanceClosed();
		}

		#region async

		/// <summary>
		/// Async result.
		/// </summary>
		protected Result AsyncResult;

		/// <summary>
		/// Gets an awaiter used to await this result.
		/// </summary>
		/// <returns>Awaiter.</returns>
		public virtual Picker<TValue, TPicker> GetAwaiter()
		{
			IsCompleted = false;
			return this;
		}

		/// <summary>
		/// Ends the wait for the completion of the asynchronous task.
		/// </summary>
		/// <returns>Result.</returns>
		public virtual Result GetResult() => AsyncResult;

		#endregion

		#region IStylable implementation

		/// <inheritdoc/>
		public override bool SetStyle(Style style)
		{
			if (TryGetComponent<Image>(out var bg))
			{
				style.Dialog.Background.ApplyTo(bg);
			}

			style.Dialog.Title.ApplyTo(transform.Find("Header/Title"));

			style.Dialog.ContentBackground.ApplyTo(transform.Find("Content"));

			style.Dialog.Delimiter.ApplyTo(transform.Find("Delimiter/Delimiter"));

			if (CloseButton != null)
			{
				style.ButtonClose.ApplyTo(CloseButton);
			}
			else
			{
				style.ButtonClose.Background.ApplyTo(transform.Find("Header/CloseButton"));
				style.ButtonClose.Text.ApplyTo(transform.Find("Header/CloseButton/Text"));
			}

			return true;
		}

		/// <inheritdoc/>
		public override bool GetStyle(Style style)
		{
			if (TryGetComponent<Image>(out var bg))
			{
				style.Dialog.Background.GetFrom(bg);
			}

			style.Dialog.Title.GetFrom(transform.Find("Header/Title"));

			style.Dialog.ContentBackground.GetFrom(transform.Find("Content"));

			style.Dialog.Delimiter.GetFrom(transform.Find("Delimiter/Delimiter"));

			if (CloseButton != null)
			{
				style.ButtonClose.GetFrom(CloseButton);
			}
			else
			{
				style.ButtonClose.Background.GetFrom(transform.Find("Header/CloseButton"));
				style.ButtonClose.Text.GetFrom(transform.Find("Header/CloseButton/Text"));
			}

			return true;
		}

		#endregion
	}
}