namespace UIWidgets.Examples
{
	using UIWidgets;

	/// <summary>
	/// TreeViewSample component continent.
	/// </summary>
	public class TreeViewSampleComponentContinent : TreeViewSampleComponent
	{
		/// <inheritdoc/>
		protected override void UpdateView()
		{
			if (Item is TreeViewSampleItemContinent item)
			{
				TextAdapter.text = string.Format("{0} (Countries: {1})", item.Name, item.Countries.ToString());
			}
			else
			{
				Icon.sprite = null;
				TextAdapter.text = string.Empty;
			}
		}
	}
}