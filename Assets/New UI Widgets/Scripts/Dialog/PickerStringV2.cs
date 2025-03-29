namespace UIWidgets
{
	using UIWidgets.Styles;
	using UnityEngine;

	/// <summary>
	/// PickerStringV2.
	/// </summary>
	public class PickerStringV2 : PickerOptionalOK<string, PickerStringV2>
	{
		/// <summary>
		/// ListView.
		/// </summary>
		[SerializeField]
		public ListViewString ListView;

		/// <inheritdoc/>
		protected override void AddListeners()
		{
			base.AddListeners();
			ListView.OnSelectInternal.AddListener(ListViewCallback);
		}

		/// <inheritdoc/>
		protected override void RemoveListeners()
		{
			base.RemoveListeners();
			ListView.OnSelectInternal.RemoveListener(ListViewCallback);
		}

		/// <summary>
		/// Prepare picker to open.
		/// </summary>
		/// <param name="defaultValue">Default value.</param>
		public override void BeforeOpen(string defaultValue)
		{
			base.BeforeOpen(defaultValue);
			ListView.SelectedIndex = ListView.DataSource.IndexOf(defaultValue);
		}

		void ListViewCallback(int index)
		{
			Value = ListView.DataSource[index];

			if (Mode == PickerMode.CloseOnSelect)
			{
				Selected(Value);
			}
		}

		#region IStylable implementation

		/// <inheritdoc/>
		public override bool SetStyle(Style style)
		{
			base.SetStyle(style);
			ListView.SetStyle(style);

			return true;
		}

		/// <inheritdoc/>
		public override bool GetStyle(Style style)
		{
			base.GetStyle(style);
			ListView.GetStyle(style);

			return true;
		}
		#endregion
	}
}