namespace UIWidgets.UIThemesSupport
{
	using UIThemes.Wrappers;
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// Theme property for the HighlightedBackgroundColor of ListView.
	/// </summary>
	public class ListViewHighlightedBackgroundColor : Wrapper<Color, ListViewBase>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ListViewHighlightedBackgroundColor"/> class.
		/// </summary>
		public ListViewHighlightedBackgroundColor()
		{
			Name = nameof(ListViewBase.HighlightedBackgroundColor);
		}

		/// <inheritdoc/>
		protected override Color Get(ListViewBase listView)
		{
			return listView.HighlightedBackgroundColor;
		}

		/// <inheritdoc/>
		protected override void Set(ListViewBase listView, Color value)
		{
			listView.HighlightedBackgroundColor = value;
		}

		/// <inheritdoc/>
		protected override bool Active(ListViewBase widget)
		{
			return widget.AllowColoring;
		}
	}
}