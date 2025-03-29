namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// TreeViewSample component country.
	/// </summary>
	public class TreeViewSampleComponentCountry : TreeViewSampleComponent
	{
		/// <inheritdoc/>
		protected override void UpdateView()
		{
			if (Item is TreeViewSampleItemCountry item)
			{
				Icon.sprite = item.Icon;
				TextAdapter.text = item.Name;

				if (SetNativeSize)
				{
					Icon.SetNativeSize();
				}

				Icon.enabled = Icon.sprite != null;
			}
			else
			{
				Icon.sprite = null;
				TextAdapter.text = string.Empty;
			}
		}
	}
}