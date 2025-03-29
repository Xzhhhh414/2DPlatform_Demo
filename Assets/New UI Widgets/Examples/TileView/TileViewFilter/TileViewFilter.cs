namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// TileView filter.
	/// </summary>
	[RequireComponent(typeof(InputFieldAdapter))]
	public class TileViewFilter : MonoBehaviourInitiable
	{
		/// <summary>
		/// TileView.
		/// </summary>
		[SerializeField]
		protected ListViewString TileView;

		readonly ObservableList<string> dataSource = new ObservableList<string>();

		/// <summary>
		/// DataSource.
		/// </summary>
		public ObservableList<string> DataSource => dataSource;

		InputFieldAdapter inputField;

		/// <summary>
		/// Input field.
		/// </summary>
		public InputFieldAdapter InputField
		{
			get
			{
				if (inputField == null)
				{
					TryGetComponent(out inputField);
				}

				return inputField;
			}
		}

		/// <inheritdoc/>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required")]
		protected override void InitOnce()
		{
			base.InitOnce();

			// get items from TileView.DataSource if items list is empty
			if (DataSource.Count == 0)
			{
				// copy items from TileView
				// DataSource.AddRange(TileView.DataSource);

				// or get items from file
				if (TileView.TryGetComponent<ListViewStringDataFile>(out var loader))
				{
					DataSource.AddRange(loader.GetItemsFromFile(loader.File));
				}
			}

			// add callback to update TileView on input change
			InputField.onValueChanged.AddListener(Filter);

			// add callback to update TileView on DataSource change
			DataSource.OnChangeMono.AddListener(DataSourceChanged);

			DataSourceChanged();
		}

		/// <summary>
		/// Process the destroy event.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required")]
		protected void OnDestroy()
		{
			DataSource.OnChangeMono.RemoveListener(DataSourceChanged);
			InputField.onValueChanged.RemoveListener(Filter);
		}

		void DataSourceChanged()
		{
			Filter(InputField.text);
		}

		/// <summary>
		/// Check if item match input.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <param name="input">Input.</param>
		/// <returns>true if item match input; otherwise false.</returns>
		bool IsMatch(string item, string input)
		{
			return UtilitiesCompare.Contains(item, input, false);
		}

		/// <summary>
		/// Filter items by input.
		/// </summary>
		/// <param name="input">Input.</param>
		public void Filter(string input)
		{
			using var _ = TileView.DataSource.BeginUpdate();

			// remove items after previous filter
			TileView.DataSource.Clear();

			// if input is empty add all items
			if (string.IsNullOrEmpty(input))
			{
				TileView.DataSource.AddRange(DataSource);
			}
			else
			{
				foreach (var item in DataSource)
				{
					// add item if match input
					if (IsMatch(item, input))
					{
						TileView.DataSource.Add(item);
					}
				}
			}
		}
	}
}