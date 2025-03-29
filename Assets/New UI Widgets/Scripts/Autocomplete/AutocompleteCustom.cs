namespace UIWidgets
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Text;
	using UIWidgets.l10n;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.Events;
	using UnityEngine.EventSystems;
	using UnityEngine.Serialization;
	using UnityEngine.UI;

	/// <summary>
	/// AutocompleteFilter.
	/// </summary>
	public enum AutocompleteFilter
	{
		/// <summary>
		/// Value should beginning with Input.
		/// </summary>
		Startswith = 0,

		/// <summary>
		/// Input occurs with value.
		/// </summary>
		Contains = 1,
	}

	/// <summary>
	/// AutocompleteInput.
	/// </summary>
	public enum AutocompleteInput
	{
		/// <summary>
		/// Use current word in input.
		/// </summary>
		Word = 0,

		/// <summary>
		/// Use entire input.
		/// </summary>
		AllInput = 1,
	}

	/// <summary>
	/// AutocompleteResult.
	/// Append - add to end of input.
	/// Result - replace input.
	/// </summary>
	public enum AutocompleteResult
	{
		/// <summary>
		/// Append value to input
		/// </summary>
		Append = 0,

		/// <summary>
		/// Replace input.
		/// </summary>
		Replace = 1,
	}

	/// <summary>
	/// Autocomplete.
	/// Allow quickly find and select from a list of values as user type.
	/// DisplayListView - used to display list of values.
	/// TargetListView - if specified selected value will be added to this list.
	/// DataSource - list of values.
	/// </summary>
	/// <typeparam name="TValue">Type of value.</typeparam>
	/// <typeparam name="TListViewComponent">Type of ListView.DefaultItem.</typeparam>
	/// <typeparam name="TListView">Type of ListView.</typeparam>
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/widgets/input/autocomplete.html")]
	public abstract class AutocompleteCustom<TValue, TListViewComponent, TListView> : MonoBehaviourInitiable, IStylable, IUpgradeable, IUpdatable, ILateUpdatable
		where TListView : ListViewCustom<TListViewComponent, TValue>
		where TListViewComponent : ListViewItem
	{
		/// <summary>
		/// Parsed input.
		/// </summary>
		[Serializable]
		protected struct ParsedInput
		{
			/// <summary>
			/// Raw input.
			/// </summary>
			public string Raw;

			/// <summary>
			/// Input without tags.
			/// </summary>
			public string WithoutTags;

			/// <summary>
			/// Text to search (cleared or word before caret).
			/// </summary>
			public string Value;

			/// <summary>
			/// Caret position.
			/// </summary>
			public int CaretPosition;

			/// <summary>
			/// Caret position with tags.
			/// </summary>
			public int CaretPositionWithTags;

			/// <summary>
			/// Tags length before caret.
			/// </summary>
			public readonly int CaretDelta => CaretPositionWithTags - CaretPosition;

			/// <summary>
			/// Initializes a new instance of the <see cref="ParsedInput"/> struct.
			/// </summary>
			/// <param name="raw">Original input.</param>
			/// <param name="caretPosition">Caret position.</param>
			public ParsedInput(string raw, int caretPosition)
			{
				Raw = raw;
				WithoutTags = raw;
				Value = raw;
				CaretPosition = caretPosition;
				CaretPositionWithTags = caretPosition;
			}

			readonly int TagEnd(string input, int start)
			{
				var tag_end = '>';
				var quote_symbol = '"';

				var end = input.IndexOf(tag_end, start);
				if (end == -1)
				{
					return input.Length;
				}

				while (true)
				{
					var quote = input.IndexOf(quote_symbol, start);
					if ((quote == -1) || (quote > end))
					{
						break;
					}

					var quote_end = input.IndexOf(quote_symbol, quote + 1);
					end = quote_end == -1
						? input.Length
						: input.IndexOf(tag_end, quote_end);
					start = quote_end + 1;
				}

				return end;
			}

			/// <summary>
			/// Remove tags.
			/// </summary>
			/// <param name="sb">Reusable StringBuilder.</param>
			public void RemoveTags(StringBuilder sb)
			{
				var tag_start = "<";
				if (!Raw.Contains(tag_start))
				{
					WithoutTags = Raw;
					return;
				}

				var prev = 0;
				var length = 0;

				while (true)
				{
					var start = Raw.IndexOf(tag_start, prev);
					if (start == -1)
					{
						break;
					}

					var end = TagEnd(Raw, start);
					sb.Append(Raw.Substring(prev, start - prev));
					length += start - prev;
					if (length < CaretPosition)
					{
						CaretPositionWithTags += end - start + 1;
					}

					prev = end + 1;
				}

				if (prev < Raw.Length)
				{
					sb.Append(Raw.Substring(prev));
				}

				WithoutTags = sb.ToString();
				sb.Clear();
			}

			/// <summary>
			/// String representation of the ParsedInput instance.
			/// </summary>
			/// <param name="parsed">Parsed input.</param>
			public static implicit operator string(ParsedInput parsed)
			{
				return parsed.Value;
			}
		}

		/// <summary>
		/// Select event.
		/// </summary>
		[Serializable]
		public class SelectEvent : UnityEvent<TValue>
		{
		}

		/// <summary>
		/// Select event.
		/// </summary>
		[Serializable]
		public class NotFoundEvent : UnityEvent<string>
		{
		}

		/// <summary>
		/// Autocomplete event.
		/// </summary>
		[Serializable]
		public class AutocompleteEvent : UnityEvent<AutocompleteCustom<TValue, TListViewComponent, TListView>>
		{
		}

		/// <summary>
		/// InputField for autocomplete.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("InputField")]
		[HideInInspector]
		[Obsolete("Replaced with InputFieldAdapter")]
		protected InputField inputField;

		/// <summary>
		/// InputField for autocomplete.
		/// </summary>
		[Obsolete("Replaced with InputFieldAdapter")]
		public InputField InputField
		{
			get => inputField;

			set
			{
				inputField = value;
				inputFieldAdapter = null;
				Utilities.RequireComponent(inputField, ref inputFieldAdapter);
				InitInputField();
			}
		}

		/// <summary>
		/// InputField for autocomplete.
		/// </summary>
		[SerializeField]
		protected InputFieldAdapter inputFieldAdapter;

		/// <summary>
		/// InputField for autocomplete.
		/// </summary>
		public InputFieldAdapter InputFieldAdapter
		{
			get => inputFieldAdapter;

			set
			{
				inputFieldAdapter = value;
				InitInputField();
			}
		}

		/// <summary>
		/// Gets the InputFieldProxy.
		/// </summary>
		[Obsolete("Replaced with InputFieldAdapter")]
		public virtual IInputFieldProxy InputFieldProxy => inputFieldAdapter;

		/// <summary>
		/// ListView to display available values.
		/// </summary>
		[SerializeField]
		public TListView TargetListView;

		/// <summary>
		/// Selected value will be added to this ListView.
		/// </summary>
		[SerializeField]
		public TListView DisplayListView;

		/// <summary>
		/// The allow duplicate of TargetListView items.
		/// </summary>
		[SerializeField]
		public bool AllowDuplicate = false;

		/// <summary>
		/// List of the values.
		/// </summary>
		[SerializeField]
		public List<TValue> DataSource = new List<TValue>();

		/// <summary>
		/// The filter.
		/// </summary>
		[SerializeField]
		protected AutocompleteFilter filter = AutocompleteFilter.Contains;

		/// <summary>
		/// Gets or sets the filter.
		/// </summary>
		/// <value>The filter.</value>
		public AutocompleteFilter Filter
		{
			get => filter;

			set
			{
				filter = value;
				CustomFilter = null;
			}
		}

		/// <summary>
		/// Is filter case sensitive?
		/// </summary>
		[SerializeField]
		public bool CaseSensitive;

		/// <summary>
		/// The delimiter chars to find word for autocomplete if InputType == Word.
		/// </summary>
		[SerializeField]
		public char[] DelimiterChars = new char[] { ' ', '\n' };

		/// <summary>
		/// Custom filter.
		/// </summary>
		public Func<string, ObservableList<TValue>> CustomFilter;

		/// <summary>
		/// Use entire input or current word in input.
		/// </summary>
		[SerializeField]
		protected AutocompleteInput InputType = AutocompleteInput.AllInput;

		/// <summary>
		/// Append value to input or replace input.
		/// </summary>
		[SerializeField]
		protected AutocompleteResult Result = AutocompleteResult.Replace;

		/// <summary>
		/// Current query - word in input or whole input for autocomplete.
		/// </summary>
		[HideInInspector]
		protected ParsedInput Query;

		/// <summary>
		/// Current word in input or whole input for autocomplete.
		/// </summary>
		[HideInInspector]
		[Obsolete("Use Query instead.")]
		protected string Input
		{
			get => Query.Value;

			set => Query.Value = value;
		}

		/// <summary>
		/// The previous query string.
		/// </summary>
		protected string PrevQuery;

		/// <summary>
		/// The previous query string.
		/// </summary>
		[Obsolete("Use PrevQuery instead.")]
		protected string PrevInput
		{
			get => PrevQuery;

			set => PrevQuery = value;
		}

		/// <summary>
		/// InputField.caretPosition. Used to keep caretPosition with Up and Down actions.
		/// </summary>
		[Obsolete("Replaced with Query.CaretPosition")]
		protected int CaretPosition;

		/// <summary>
		/// The minimum number of characters a user must type before a search is performed.
		/// </summary>
		[SerializeField]
		public int MinLength = 0;

		/// <summary>
		/// The delay in seconds between when a keystroke occurs and when a search is performed.
		/// </summary>
		[SerializeField]
		public float SearchDelay = 0f;

		/// <summary>
		/// Use unscaled time.
		/// </summary>
		[SerializeField]
		public bool UnscaledTime = true;

		/// <summary>
		/// Reset DisplayListView selected items.
		/// </summary>
		[SerializeField]
		public bool ResetListViewSelection = true;

		/// <summary>
		/// Canvas will be used as parent for DisplayListView.
		/// </summary>
		[SerializeField]
		public RectTransform ParentCanvas;

		/// <summary>
		/// Is options open?
		/// </summary>
		public bool IsOptionsOpen
		{
			get;
			protected set;
		}

		/// <summary>
		/// To keep DisplayListView position if InputField inside scrollable area.
		/// </summary>
		protected Vector2 DisplayListViewAnchoredPosition;

		/// <summary>
		/// Default parent for DisplayListView.
		/// </summary>
		protected RectTransform DisplayListViewParent;

		/// <summary>
		/// Coroutine to performs search.
		/// </summary>
		protected IEnumerator SearchCoroutine;

		/// <summary>
		/// The modal ID.
		/// </summary>
		protected InstanceID? ModalKey;

		/// <summary>
		/// DisplayListView hierarchy position.
		/// </summary>
		protected HierarchyPosition DisplayListViewPosition;

		/// <summary>
		/// Canvas resize subscription.
		/// </summary>
		protected Subscription CanvasResize;

		/// <summary>
		/// Option selected event.
		/// </summary>
		public UnityEvent OnOptionSelected = new UnityEvent();

		/// <summary>
		/// Item selected event.
		/// </summary>
		public SelectEvent OnOptionSelectedItem = new SelectEvent();

		/// <summary>
		/// Item not found event.
		/// </summary>
		public NotFoundEvent OnItemNotFound = new NotFoundEvent();

		/// <summary>
		/// Cancel event.
		/// </summary>
		public UnityEvent OnCancelInput = new UnityEvent();

		/// <summary>
		/// Search completed event.
		/// </summary>
		public UnityEvent OnSearchCompleted = new UnityEvent();

		/// <summary>
		/// Show options event.
		/// </summary>
		public AutocompleteEvent OnShowOptions = new AutocompleteEvent();

		/// <summary>
		/// Hide options event.
		/// </summary>
		public AutocompleteEvent OnHideOptions = new AutocompleteEvent();

		/// <summary>
		/// Allow cancel on deselect event.
		/// </summary>
		public Func<BaseEventData, AutocompleteCustom<TValue, TListViewComponent, TListView>, bool> AllowCancelOnDeselect;

		/// <summary>
		/// Search results.
		/// </summary>
		protected ObservableList<TValue> SearchResults = new ObservableList<TValue>();

		/// <summary>
		/// Used for ParsedInput.RemoveTags().
		/// </summary>
		protected StringBuilder RemoveTagsTemp = new StringBuilder();

		/// <summary>
		/// Determines whether the beginning of value matches the Input.
		/// </summary>
		/// <param name="value">Value.</param>
		/// <returns>true if beginning of value matches the Input; otherwise, false.</returns>
		public abstract bool Startswith(TValue value);

		/// <summary>
		/// Returns a value indicating whether Input occurs within specified value.
		/// </summary>
		/// <param name="value">Value.</param>
		/// <returns>true if the Input occurs within value parameter; otherwise, false.</returns>
		public abstract bool Contains(TValue value);

		/// <summary>
		/// Convert value to string.
		/// </summary>
		/// <returns>The string value.</returns>
		/// <param name="value">Value.</param>
		protected abstract string GetStringValue(TValue value);

		/// <inheritdoc/>
		protected override void InitOnce()
		{
			base.InitOnce();

			InitInputField();

			DisplayListView.gameObject.SetActive(false);
			DisplayListView.KeepHighlight = false;

			DisplayListView.MultipleSelect = false;
			DisplayListView.OnSelect.AddListener(ItemSelected);
		}

		bool localeSubscription;

		/// <summary>
		/// Process the enable event.
		/// </summary>
		protected virtual void OnEnable()
		{
			if (!localeSubscription)
			{
				Init();

				localeSubscription = true;
				Localization.OnLocaleChanged += LocaleChanged;
				LocaleChanged();
			}

			Updater.Add(this);
			Updater.AddLateUpdate(this);
		}

		/// <summary>
		/// Process locale changes.
		/// </summary>
		public virtual void LocaleChanged()
		{
			if (DisplayListView.SelectedIndex >= 0)
			{
				var item = DisplayListView.DataSource[DisplayListView.SelectedIndex];

				if (Result == AutocompleteResult.Replace)
				{
					InputFieldAdapter.text = GetStringValue(item);
				}
			}
		}

		/// <summary>
		/// Init input field.
		/// </summary>
		protected virtual void InitInputField()
		{
			if (!IsInited)
			{
				return;
			}

			InputFieldAdapter.onValueChanged.AddListener(ApplyFilter);
			InputFieldAdapter.onEndEdit.AddListener(CheckCancel);

			var inputListener = Utilities.RequireComponent<InputFieldListener>(InputFieldAdapter.gameObject);
			inputListener.OnMoveEvent.AddListener(SelectResult);
			inputListener.OnSubmitEvent.AddListener(SubmitResult);
			inputListener.onDeselect.AddListener(InputDeselected);
		}

		/// <summary>
		/// Show all options.
		/// </summary>
		public void ShowAllOptions()
		{
			InputFieldAdapter.Focus();
			ApplyFilter(string.Empty, false);
		}

		/// <summary>
		/// Gets the input field text.
		/// </summary>
		/// <returns>The input field text.</returns>
		public virtual string GetInputFieldText()
		{
			return InputFieldAdapter.text;
		}

		/// <summary>
		/// Allow to handle item selection event.
		/// </summary>
		protected bool AllowItemSelectionEvent;

		/// <summary>
		/// Handle input deselected event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		protected virtual void InputDeselected(BaseEventData eventData)
		{
			AllowItemSelectionEvent = (eventData is PointerEventData ev)
				&& (ev.pointerCurrentRaycast.gameObject != null)
				&& ev.pointerCurrentRaycast.gameObject.transform.IsChildOf(DisplayListView.transform);

			if (!AllowItemSelectionEvent && (AllowCancelOnDeselect != null))
			{
				AllowItemSelectionEvent = AllowCancelOnDeselect(eventData, this);
			}

			if (!AllowItemSelectionEvent)
			{
				Cancel();
			}
		}

		/// <summary>
		/// Handle item selected event.
		/// </summary>
		/// <param name="index">Index.</param>
		/// <param name="component">Component.</param>
		protected virtual void ItemSelected(int index, ListViewItem component)
		{
			if (!IsOptionsOpen)
			{
				return;
			}

			if (AllowItemSelectionEvent)
			{
				AllowItemSelectionEvent = false;
				SubmitResult(null);
			}
		}

		/// <summary>
		/// Closes the options.
		/// </summary>
		protected virtual void HideOptions()
		{
			IsOptionsOpen = false;

			ModalHelper.Close(ref ModalKey);
			CanvasResize.Clear();
			DisplayListViewPosition.Restore();
			DisplayListView.gameObject.SetActive(false);

			OnHideOptions.Invoke(this);
		}

		/// <summary>
		/// Check if input was canceled.
		/// </summary>
		/// <param name="input">Input.</param>
		protected virtual void CheckCancel(string input)
		{
			if (inputFieldAdapter.wasCanceled)
			{
				Cancel();
			}
		}

		/// <summary>
		/// Cancel.
		/// </summary>
		protected virtual void Cancel()
		{
			HideOptions();

			OnCancelInput.Invoke();
		}

		/// <summary>
		/// Shows the options.
		/// </summary>
		protected virtual void ShowOptions()
		{
			if (IsOptionsOpen)
			{
				return;
			}

			IsOptionsOpen = true;

			var parent = transform.parent;
			var lv_transform = DisplayListView.transform;
			if (parent != lv_transform.parent)
			{
				lv_transform.SetParent(parent, false);
			}

			OnShowOptions.Invoke(this);

			if (ParentCanvas == null)
			{
				ParentCanvas = UtilitiesUI.FindTopmostCanvas(lv_transform);
			}

			if (!ModalKey.HasValue)
			{
				ModalKey = ModalHelper.Open(this, null, new Color(0, 0, 0, 0f), Cancel, ParentCanvas);
			}

			if (ParentCanvas != null)
			{
				DisplayListViewPosition = HierarchyPosition.SetParent(lv_transform, ParentCanvas);

				var resize = Utilities.RequireComponent<ResizeListener>(ParentCanvas);
				CanvasResize.Clear();
				CanvasResize = new Subscription(resize.OnResize, RefreshPosition);
			}

			DisplayListView.gameObject.SetActive(true);
		}

		/// <summary>
		/// Refresh position.
		/// </summary>
		protected virtual void RefreshPosition() => DisplayListViewPosition.Refresh();

		/// <summary>
		/// Gets the results.
		/// </summary>
		/// <returns>Values matches filter.</returns>
		protected virtual ObservableList<TValue> GetResults()
		{
			if (CustomFilter != null)
			{
				return CustomFilter(Query.Value);
			}
			else
			{
				SearchResults.Clear();
				if (Filter == AutocompleteFilter.Startswith)
				{
					UtilitiesCollections.FindAll(DataSource, Startswith, SearchResults);
				}
				else
				{
					UtilitiesCollections.FindAll(DataSource, Contains, SearchResults);
				}

				return SearchResults;
			}
		}

		/// <summary>
		/// Sets the input.
		/// </summary>
		/// <param name="input">Input string.</param>
		/// <returns>Query string.</returns>
		protected virtual ParsedInput Input2Query(string input)
		{
			var parsed = new ParsedInput(input, InputFieldAdapter.caretPosition);
			if (InputFieldAdapter.richText)
			{
				parsed.RemoveTags(RemoveTagsTemp);
			}

			if (InputType == AutocompleteInput.AllInput)
			{
				parsed.Value = parsed.WithoutTags;
				return parsed;
			}

			int end_position = parsed.CaretPosition;

			if (parsed.WithoutTags.Length >= end_position)
			{
				var text = parsed.WithoutTags.Substring(0, end_position);
				var start_position = text.LastIndexOfAny(DelimiterChars) + 1;

				parsed.Value = text.Substring(start_position).Trim();
			}
			else
			{
				parsed.Value = parsed.WithoutTags;
			}

			return parsed;
		}

		/// <summary>
		/// Applies the filter.
		/// </summary>
		/// <param name="input">Input.</param>
		protected virtual void ApplyFilter(string input)
		{
			if (InputFieldAdapter.wasCanceled)
			{
				return;
			}

			ApplyFilter(input, true);
		}

		/// <summary>
		/// Applies the filter.
		/// </summary>
		/// <param name="input">Input.</param>
		/// <param name="skipIfSame">Check if InputField has focus?</param>
		protected virtual void ApplyFilter(string input, bool skipIfSame)
		{
			if (EventSystem.current.currentSelectedGameObject != InputFieldAdapter.gameObject)
			{
				return;
			}

			Query = Input2Query(input);

			if (skipIfSame && (Query.Value == PrevQuery))
			{
				return;
			}

			PrevQuery = Query.Value;

			SearchCancel();

			if (Query.Value.Length < MinLength)
			{
				DisplayListView.DataSource.Clear();
				HideOptions();
				return;
			}

			DisplayListView.Init();
			DisplayListView.MultipleSelect = false;

			SearchStart();
		}

		/// <summary>
		/// Start search.
		/// </summary>
		protected virtual void SearchStart()
		{
			SearchCoroutine = Search();
			StartCoroutine(SearchCoroutine);
		}

		/// <summary>
		/// Cancel search.
		/// </summary>
		protected virtual void SearchCancel()
		{
			if (SearchCoroutine != null)
			{
				StopCoroutine(SearchCoroutine);
			}
		}

		/// <summary>
		/// Performs search with delay.
		/// </summary>
		/// <returns>Yield instruction.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0401:Possible allocation of reference type enumerator", Justification = "Enumerator is reusable.")]
		protected virtual IEnumerator Search()
		{
			if (SearchDelay > 0)
			{
				yield return UtilitiesTime.Wait(SearchDelay, UnscaledTime);
			}

			using (var _ = DisplayListView.DataSource.BeginUpdate())
			{
				DisplayListView.DataSource.Clear();
				DisplayListView.DataSource.AddRange(GetResults());
			}

			SearchCompleted();
		}

		/// <summary>
		/// Search completed.
		/// </summary>
		protected virtual void SearchCompleted()
		{
			OnSearchCompleted.Invoke();

			if (DisplayListView.DataSource.Count > 0)
			{
				ShowOptions();
				if (ResetListViewSelection)
				{
					DisplayListView.SelectedIndex = -1;
				}
			}
			else
			{
				HideOptions();
			}
		}

		/// <summary>
		/// Process the disable event.
		/// </summary>
		protected virtual void OnDisable()
		{
			Updater.Remove(this);
			Updater.RemoveLateUpdate(this);
		}

		/// <summary>
		/// Update this instance.
		/// </summary>
		public virtual void RunUpdate()
		{
		}

		/// <summary>
		/// Caret position after Enter pressed.
		/// </summary>
		protected int FixCaretPosition = -1;

		/// <summary>
		/// LateUpdate.
		/// </summary>
		public virtual void RunLateUpdate()
		{
			if (FixCaretPosition != -1)
			{
#if UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_3_OR_NEWER
				InputFieldAdapter.caretPosition = FixCaretPosition;
#else
				InputFieldAdapter.MoveToEnd();
#endif
				FixCaretPosition = -1;
			}
		}

		/// <summary>
		/// Selects the result.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		protected virtual void SelectResult(AxisEventData eventData)
		{
			if (!DisplayListView.gameObject.activeInHierarchy)
			{
				return;
			}

			if (DisplayListView.DataSource.Count == 0)
			{
				return;
			}

			switch (eventData.moveDir)
			{
				case MoveDirection.Up:
					if (DisplayListView.SelectedIndex == 0)
					{
						DisplayListView.SelectedIndex = DisplayListView.DataSource.Count - 1;
					}
					else
					{
						DisplayListView.SelectedIndex -= 1;
					}

					DisplayListView.ScrollTo(DisplayListView.SelectedIndex);
					InputFieldAdapter.caretPosition = Query.CaretPosition;
					FixCaretPosition = Query.CaretPosition;
					break;
				case MoveDirection.Down:
					if (DisplayListView.SelectedIndex == (DisplayListView.DataSource.Count - 1))
					{
						DisplayListView.SelectedIndex = 0;
					}
					else
					{
						DisplayListView.SelectedIndex += 1;
					}

					DisplayListView.ScrollTo(DisplayListView.SelectedIndex);
					InputFieldAdapter.caretPosition = Query.CaretPosition;
					FixCaretPosition = Query.CaretPosition;
					break;
				default:
					if (Input2Query(InputFieldAdapter.text).Value != Query.Value)
					{
						ApplyFilter(InputFieldAdapter.text);
					}

					break;
			}
		}

		/// <summary>
		/// Submits the result.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		protected virtual void SubmitResult(BaseEventData eventData)
		{
			SubmitResult(eventData, false);
		}

		/// <summary>
		/// Submits the result.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		/// <param name="isEnter">Is Enter pressed?</param>
		protected virtual void SubmitResult(BaseEventData eventData, bool isEnter)
		{
			eventData?.Use();

			if (DisplayListView.SelectedIndex == -1)
			{
				OnItemNotFound.Invoke(InputFieldAdapter.text);
				HideOptions();
				return;
			}

			if (InputFieldAdapter.IsMultiLineNewline())
			{
				if (!DisplayListView.gameObject.activeInHierarchy)
				{
					return;
				}
				else
				{
					isEnter = false;
				}
			}

			var item = DisplayListView.DataSource[DisplayListView.SelectedIndex];

			if (TargetListView != null)
			{
				TargetListView.Init();
				TargetListView.Set(item, AllowDuplicate);
			}

			var value = GetStringValue(item);
			var parsed = new ParsedInput(value, 0);
			parsed.RemoveTags(RemoveTagsTemp);
			if (Result == AutocompleteResult.Append)
			{
				var end_position = Query.CaretPositionWithTags;
				var start_position = Query.Raw.LastIndexOfAny(DelimiterChars, end_position - 1, end_position) + 1;

				InputFieldAdapter.text = string.Format("{0}{1}{2}", Query.Raw.Substring(0, start_position), value, Query.Raw.Substring(end_position));

				var caret = start_position + parsed.WithoutTags.Length - Query.CaretDelta;
				InputFieldAdapter.caretPosition = caret;
#if UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_3_OR_NEWER
				// InputField.selectionFocusPosition = start_position + value.Length;
#else
				InputFieldAdapter.MoveToEnd();
#endif
				if (isEnter)
				{
					FixCaretPosition = caret;
					InputFieldAdapter.Activate();
				}
			}
			else
			{
				InputFieldAdapter.text = value;
#if UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_3_OR_NEWER
				InputFieldAdapter.caretPosition = parsed.WithoutTags.Length;
#else
				InputFieldAdapter.ActivateInputField();
#endif
				FixCaretPosition = parsed.WithoutTags.Length;
			}

			OnOptionSelected.Invoke();
			OnOptionSelectedItem.Invoke(item);

			HideOptions();
		}

		/// <summary>
		/// Upgrade this instance.
		/// </summary>
		public virtual void Upgrade()
		{
#pragma warning disable 0618
			Utilities.RequireComponent(InputField, ref inputFieldAdapter);
#pragma warning restore 0618
		}

		#if UNITY_EDITOR
		/// <summary>
		/// Validate this instance.
		/// </summary>
		protected virtual void OnValidate()
		{
			Compatibility.Upgrade(this);

			if (ParentCanvas == null)
			{
				var t = (DisplayListView != null) ? DisplayListView.transform : transform;
				ParentCanvas = UtilitiesUI.FindTopmostCanvas(t);
			}
		}

		/// <summary>
		/// Reset this instance.
		/// </summary>
		protected virtual void Reset()
		{
			if (ParentCanvas == null)
			{
				ParentCanvas = UtilitiesUI.FindTopmostCanvas(transform);
			}
		}
		#endif

		/// <summary>
		/// This function is called when the MonoBehaviour will be destroyed.
		/// </summary>
		protected virtual void OnDestroy()
		{
			DisplayListViewPosition.ParentDestroyed();

			Localization.OnLocaleChanged -= LocaleChanged;

			if (!Utilities.IsNull(DisplayListView))
			{
				DisplayListView.OnSelect.RemoveListener(ItemSelected);
			}

			if (!Utilities.IsNull(InputFieldAdapter))
			{
				InputFieldAdapter.onValueChanged.RemoveListener(ApplyFilter);
				InputFieldAdapter.onEndEdit.RemoveListener(CheckCancel);

				if (InputFieldAdapter.TryGetComponent<InputFieldListener>(out var input_listener))
				{
					input_listener.OnMoveEvent.RemoveListener(SelectResult);
					input_listener.OnSubmitEvent.RemoveListener(SubmitResult);
					input_listener.onDeselect.RemoveListener(InputDeselected);
				}
			}
		}

		#region IStylable implementation

		/// <summary>
		/// Set InputField style.
		/// </summary>
		/// <param name="style">Style data.</param>
		protected virtual void SetStyleInput(Style style)
		{
			if (InputFieldAdapter == null)
			{
				return;
			}

			if (InputFieldAdapter.textComponent != null)
			{
				style.Autocomplete.InputField.ApplyTo(InputFieldAdapter.textComponent.gameObject, true);
			}

			if (InputFieldAdapter.placeholder != null)
			{
				style.Autocomplete.Placeholder.ApplyTo(InputFieldAdapter.placeholder.gameObject);
			}
		}

		/// <inheritdoc/>
		public virtual bool SetStyle(Style style)
		{
			if (TryGetComponent<Image>(out var bg))
			{
				style.Autocomplete.Background.ApplyTo(bg);
			}

			SetStyleInput(style);

			if (!Utilities.IsNull(DisplayListView))
			{
				DisplayListView.SetStyle(style);
			}

			if (!Utilities.IsNull(TargetListView))
			{
				TargetListView.SetStyle(style);
			}

			return true;
		}

		/// <summary>
		/// Set style options from InputField.
		/// </summary>
		/// <param name="style">Style data.</param>
		protected virtual void GetStyleInput(Style style)
		{
			if (Utilities.IsNull(InputFieldAdapter))
			{
				return;
			}

			if (!Utilities.IsNull(InputFieldAdapter.textComponent))
			{
				style.Autocomplete.InputField.GetFrom(InputFieldAdapter.textComponent.gameObject, true);
			}

			if (!Utilities.IsNull(InputFieldAdapter.placeholder))
			{
				style.Autocomplete.Placeholder.GetFrom(InputFieldAdapter.placeholder.gameObject);
			}
		}

		/// <inheritdoc/>
		public virtual bool GetStyle(Style style)
		{
			if (TryGetComponent<Image>(out var bg))
			{
				style.Autocomplete.Background.GetFrom(bg);
			}

			GetStyleInput(style);

			if (!Utilities.IsNull(DisplayListView))
			{
				DisplayListView.GetStyle(style);
			}

			if (!Utilities.IsNull(TargetListView))
			{
				TargetListView.GetStyle(style);
			}

			return true;
		}
		#endregion
	}
}