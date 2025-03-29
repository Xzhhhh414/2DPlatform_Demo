namespace UIWidgets
{
	using System.Collections.Generic;
	using UIWidgets.Pool;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.EventSystems;

	/// <summary>
	/// Base class for the AutoCombobox widget.
	/// </summary>
	/// <typeparam name="TItem">Item type.</typeparam>
	/// <typeparam name="TListView">ListView type.</typeparam>
	/// <typeparam name="TListViewComponent">ListView.DefaultItem type.</typeparam>
	/// <typeparam name="TAutocomplete">Autocomplete type.</typeparam>
	/// <typeparam name="TCombobox">Combobox type.</typeparam>
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/widgets/collections/autocombobox.html")]
	public abstract class AutoCombobox<TItem, TListView, TListViewComponent, TAutocomplete, TCombobox> : MonoBehaviourInitiable, IStylable
		where TListView : ListViewCustom<TListViewComponent, TItem>
		where TListViewComponent : ListViewItem
		where TAutocomplete : AutocompleteCustom<TItem, TListViewComponent, TListView>
		where TCombobox : ComboboxCustom<TListView, TListViewComponent, TItem>
	{
		/// <summary>
		/// Autocomplete.
		/// </summary>
		[SerializeField]
		public TAutocomplete Autocomplete;

		/// <summary>
		/// Combobox.
		/// </summary>
		[SerializeField]
		public TCombobox Combobox;

		/// <summary>
		/// Add items if not found.
		/// </summary>
		[SerializeField]
		[Tooltip("Requires overrided Input2Item method.")]
		public bool AddItems = false;

		/// <summary>
		/// Keep selected items for Autocomplete.DisplayListView.
		/// </summary>
		[SerializeField]
		public bool KeepSelection = false;

		/// <summary>
		/// Require selected item.
		/// </summary>
		[SerializeField]
		[Tooltip("Selects the first item if nothing is selected at the start.")]
		public bool RequireSelectedItem = true;

		/// <summary>
		/// Data source.
		/// </summary>
		public ObservableList<TItem> DataSource
		{
			get => Combobox.ListView.DataSource;
			set => Combobox.ListView.DataSource = value;
		}

		/// <summary>
		/// Index of the last selected item.
		/// </summary>
		public int SelectedIndex
		{
			get => Combobox.ListView.SelectedIndex;
			set => Combobox.ListView.SelectedIndex = value;
		}

		/// <summary>
		/// Indices of the selected items.
		/// </summary>
		public List<int> SelectedIndices
		{
			get => Combobox.ListView.SelectedIndices;
			set => Combobox.ListView.SelectedIndices = value;
		}

		/// <summary>
		/// Selected item.
		/// </summary>
		public TItem SelectedItem => Combobox.ListView.SelectedItem;

		/// <summary>
		/// Selected items.
		/// </summary>
		public List<TItem> SelectedItems => Combobox.ListView.SelectedItems;

		/// <summary>
		/// Autocomplete input listener.
		/// </summary>
		protected SelectListener AutocompleteInputListener;

		/// <inheritdoc/>
		protected override void InitOnce()
		{
			base.InitOnce();

			Autocomplete.Init();
			Autocomplete.OnOptionSelectedItem.AddListener(OnOptionSelected);
			Autocomplete.OnItemNotFound.AddListener(ProcessItemNotFound);
			Autocomplete.OnCancelInput.AddListener(ProcessCancel);
			Autocomplete.OnSearchCompleted.AddListener(UpdateAutocompleteListViewSelected);
			Autocomplete.ResetListViewSelection = !KeepSelection;

			Combobox.Init();
			Combobox.ShowListViewOnSubmit = false;
			Combobox.ListView.OnSelectInternal.AddListener(OnSelect);
			Combobox.OnCurrentClick.AddListener(ComboboxCurrentClick);
			Combobox.ListView.OnDataSourceChanged.AddListener(ListViewDataSourceChanged);
			if (RequireSelectedItem && (Combobox.ListView.DataSource.Count > 0))
			{
				Combobox.ListView.Select(0);
			}

			AutocompleteInputListener = Utilities.RequireComponent<SelectListener>(Autocomplete.InputFieldAdapter);
			AutocompleteInputListener.onDeselect.AddListener(InputFocusLost);

			Autocomplete.DataSource = Combobox.ListView.DataSource.ListReference();

			AutocompleteHide(false);
		}

		/// <summary>
		/// Set Autocomplete.DisplayListView selected items.
		/// </summary>
		protected virtual void UpdateAutocompleteListViewSelected()
		{
			if (!KeepSelection)
			{
				return;
			}

			using var _ = ListPool<TItem>.Get(out var temp);

			Combobox.ListView.GetSelectedItems(temp);
			Autocomplete.DisplayListView.SetSelectedItems(temp, true);
		}

		/// <summary>
		/// Process DataSource changed event.
		/// </summary>
		/// <param name="listView">ListView.</param>
		protected virtual void ListViewDataSourceChanged(ListViewCustom<TListViewComponent, TItem> listView)
		{
			Autocomplete.DataSource = listView.DataSource.ListReference();
		}

		/// <summary>
		/// Process the destroy event.
		/// </summary>
		protected virtual void OnDestroy()
		{
			if (Autocomplete != null)
			{
				Autocomplete.OnOptionSelectedItem.RemoveListener(OnOptionSelected);
				Autocomplete.OnItemNotFound.RemoveListener(ProcessItemNotFound);
				Autocomplete.OnCancelInput.RemoveListener(ProcessCancel);
				Autocomplete.OnSearchCompleted.RemoveListener(UpdateAutocompleteListViewSelected);
			}

			if (Combobox != null)
			{
				Combobox.ListView.OnSelectInternal.RemoveListener(OnSelect);
				Combobox.OnCurrentClick.RemoveListener(ComboboxCurrentClick);
				Combobox.ListView.OnDataSourceChanged.RemoveListener(ListViewDataSourceChanged);
			}

			if (AutocompleteInputListener != null)
			{
				AutocompleteInputListener.onDeselect.RemoveListener(InputFocusLost);
			}
		}

		/// <summary>
		/// Process selected option.
		/// </summary>
		/// <param name="item">Item.</param>
		protected virtual void OnOptionSelected(TItem item)
		{
			var index = Combobox.ListView.DataSource.IndexOf(item);
			Combobox.ListView.Select(index);

			AutocompleteHide(false);
		}

		/// <summary>
		/// Process item not found event.
		/// </summary>
		/// <param name="input">Input.</param>
		protected virtual void ProcessItemNotFound(string input)
		{
			var index = Input2Index(input);

			if (Combobox.ListView.IsValid(index))
			{
				Combobox.ListView.Select(index);
			}

			AutocompleteHide(false);
		}

		/// <summary>
		/// Process cancel event.
		/// </summary>
		protected virtual void ProcessCancel()
		{
			AutocompleteHide(false);
		}

		/// <summary>
		/// Get item index by input.
		/// </summary>
		/// <param name="input">Input.</param>
		/// <returns>Index.</returns>
		protected virtual int Input2Index(string input)
		{
			for (int i = 0; i < Combobox.ListView.DataSource.Count; i++)
			{
				var item = Combobox.ListView.DataSource[i];
				if (UtilitiesCompare.Compare(GetStringValue(item), input) == 0)
				{
					return i;
				}
			}

			if (AddItems)
			{
				var new_item = Input2Item(input);
				if (new_item != null)
				{
					return Combobox.ListView.Add(new_item);
				}
			}

			return -1;
		}

		/// <summary>
		/// Create a new item by specified input.
		/// </summary>
		/// <param name="input">Input.</param>
		/// <returns>New item.</returns>
		protected virtual TItem Input2Item(string input) => default;

		/// <summary>
		/// Process the select event.
		/// </summary>
		/// <param name="index">Index.</param>
		protected virtual void OnSelect(int index)
		{
			Autocomplete.InputFieldAdapter.text = GetStringValue(Combobox.ListView.DataSource[index]);
		}

		/// <summary>
		/// Convert item to string.
		/// </summary>
		/// <returns>The string value.</returns>
		/// <param name="item">Item.</param>
		protected abstract string GetStringValue(TItem item);

		/// <summary>
		/// Has selected item?
		/// </summary>
		/// <returns>true if ListView has selected item; otherwise false.</returns>
		protected bool HasSelected()
		{
			return Combobox.ListView.SelectedIndex >= 0;
		}

		/// <summary>
		/// Process combobox OnCurrentClick.
		/// </summary>
		/// <param name="index">Index.</param>
		/// <param name="item">Item.</param>
		protected virtual void ComboboxCurrentClick(int index, TItem item)
		{
			AutocompleteShow();
		}

		/// <summary>
		/// Show autocomplete.
		/// </summary>
		protected virtual void AutocompleteShow()
		{
			if (HasSelected())
			{
				Autocomplete.InputFieldAdapter.text = GetStringValue(Combobox.ListView.SelectedItem);
			}

			Autocomplete.gameObject.SetActive(true);
			Combobox.HideCurrent();

			if (!EventSystem.current.alreadySelecting)
			{
				EventSystem.current.SetSelectedGameObject(Autocomplete.InputFieldAdapter.gameObject);
			}
		}

		/// <summary>
		/// Process InputField focus lost event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		protected void InputFocusLost(BaseEventData eventData)
		{
			AutocompleteHide(false);
		}

		/// <summary>
		/// Hide autocomplete.
		/// </summary>
		/// <param name="requireSelected">Require selected item.</param>
		protected virtual void AutocompleteHide(bool requireSelected)
		{
			if (!requireSelected || HasSelected())
			{
				Autocomplete.gameObject.SetActive(false);
				Combobox.ShowCurrent();
			}
			else
			{
				AutocompleteShow();
			}
		}

		/// <inheritdoc/>
		public virtual bool SetStyle(Style style)
		{
			Autocomplete.SetStyle(style);
			Combobox.SetStyle(style);

			return true;
		}

		/// <inheritdoc/>
		public virtual bool GetStyle(Style style)
		{
			Autocomplete.GetStyle(style);
			Combobox.GetStyle(style);

			return true;
		}
	}
}