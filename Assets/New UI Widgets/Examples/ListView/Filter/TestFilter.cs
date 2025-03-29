namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// Test filter.
	/// </summary>
	public class TestFilter : MonoBehaviour
	{
		/// <summary>
		/// ListView.
		/// </summary>
		[SerializeField]
		public ListViewIcons ListView;

		/// <summary>
		/// InputField.
		/// </summary>
		[SerializeField]
		public InputFieldAdapter InputField;

		/// <summary>
		/// NewItem InputField.
		/// </summary>
		[SerializeField]
		public InputFieldAdapter NewItemInputField;

		ObservableListFilter<ListViewIconsItemDescription> Filter;

		/// <summary>
		/// Process the start event.
		/// </summary>
		protected void Start()
		{
			Filter = new ObservableListFilter<ListViewIconsItemDescription>(ListView.DataSource, Predicate);
			ListView.DataSource = Filter.Output;
			InputField.onValueChanged.AddListener(InputFieldChanged);
		}

		bool Predicate(ListViewIconsItemDescription item) => UtilitiesCompare.Contains(item.Name, InputField.Value, caseSensitive: false);

		void InputFieldChanged(string ignore) => Filter.Refresh();

		/// <summary>
		/// Process the destroy event.
		/// </summary>
		protected void OnDestroy()
		{
			if (InputField != null)
			{
				InputField.onValueChanged.RemoveListener(InputFieldChanged);
			}
		}

		/// <summary>
		/// Add new item
		/// </summary>
		public void Add()
		{
			var name = string.Format("Item {0}", Filter.Input.Count.ToString());
			Filter.Input.Add(new ListViewIconsItemDescription() { Name = name });
		}

		/// <summary>
		/// Add new item
		/// </summary>
		public void AddFromInput()
		{
			if (string.IsNullOrEmpty(NewItemInputField.Value))
			{
				return;
			}

			Filter.Input.Add(new ListViewIconsItemDescription() { Name = NewItemInputField.Value });
			NewItemInputField.Value = string.Empty;
		}
	}
}