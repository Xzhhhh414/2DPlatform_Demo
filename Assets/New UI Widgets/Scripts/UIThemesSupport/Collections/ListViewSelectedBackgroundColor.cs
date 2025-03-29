namespace UIWidgets.UIThemesSupport
{
	using UIThemes.Wrappers;
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// Theme property for the SelectedBackgroundColor of ListView.
	/// </summary>
	public class ListViewSelectedBackgroundColor : Wrapper<Color, ListViewBase>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ListViewSelectedBackgroundColor"/> class.
		/// </summary>
		public ListViewSelectedBackgroundColor()
		{
			Name = nameof(ListViewBase.SelectedBackgroundColor);
		}

		/// <inheritdoc/>
		protected override Color Get(ListViewBase listView)
		{
			return listView.SelectedBackgroundColor;
		}

		/// <inheritdoc/>
		protected override void Set(ListViewBase listView, Color value)
		{
			listView.SelectedBackgroundColor = value;
		}

		/// <inheritdoc/>
		protected override bool Active(ListViewBase widget)
		{
			return widget.AllowColoring;
		}
	}
}