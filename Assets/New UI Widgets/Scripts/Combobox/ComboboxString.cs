namespace UIWidgets
{
	/// <summary>
	/// Combobox.
	/// </summary>
	public class ComboboxString : ComboboxCustom<ListViewString, ListViewStringItemComponent, string>
	{
		/// <inheritdoc/>
		protected override void InitCustomWidgets()
		{
			if ((ListView != null) && ListView.TryGetComponent<ListViewStringDataFile>(out var data))
			{
				data.Init();
			}

			base.InitCustomWidgets();
		}
	}
}