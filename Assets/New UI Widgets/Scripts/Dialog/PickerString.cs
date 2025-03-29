namespace UIWidgets
{
	using UIWidgets.Styles;
	using UnityEngine;

	/// <summary>
	/// PickerString.
	/// </summary>
	[System.Obsolete("Replaced with PickerStringV2.")]
	public class PickerString : PickerOptionalOK<string, PickerString>
	{
		/// <summary>
		/// ListView.
		/// </summary>
		[SerializeField]
		public ListView ListView;

		/// <inheritdoc/>
		protected override void AddListeners()
		{
			base.AddListeners();
			ListView.OnSelectString.AddListener(ListViewCallback);
		}

		/// <inheritdoc/>
		protected override void RemoveListeners()
		{
			base.RemoveListeners();
			ListView.OnSelectString.RemoveListener(ListViewCallback);
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

		void ListViewCallback(int index, string value)
		{
			Value = value;

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