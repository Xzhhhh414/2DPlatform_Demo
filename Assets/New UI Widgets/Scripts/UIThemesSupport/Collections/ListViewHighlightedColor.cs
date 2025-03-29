namespace UIWidgets.UIThemesSupport
{
	using UIThemes.Wrappers;
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// Theme property for HighlightedColor of ListView.
	/// </summary>
	public class ListViewHighlightedColor : Wrapper<Color, ListViewBase>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ListViewHighlightedColor"/> class.
		/// </summary>
		public ListViewHighlightedColor()
		{
			Name = nameof(ListViewBase.HighlightedColor);
		}

		/// <inheritdoc/>
		protected override Color Get(ListViewBase listView)
		{
			return listView.HighlightedColor;
		}

		/// <inheritdoc/>
		protected override void Set(ListViewBase listView, Color value)
		{
			listView.HighlightedColor = value;
		}

		/// <inheritdoc/>
		protected override bool Active(ListViewBase widget)
		{
			return widget.AllowColoring;
		}
	}
}