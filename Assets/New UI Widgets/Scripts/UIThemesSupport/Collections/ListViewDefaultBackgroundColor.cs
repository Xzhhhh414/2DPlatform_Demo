namespace UIWidgets.UIThemesSupport
{
	using UIThemes.Wrappers;
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// Theme property for the DefaultBackgroundColor of ListView.
	/// </summary>
	public class ListViewDefaultBackgroundColor : Wrapper<Color, ListViewBase>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ListViewDefaultBackgroundColor"/> class.
		/// </summary>
		public ListViewDefaultBackgroundColor()
		{
			Name = nameof(ListViewBase.DefaultBackgroundColor);
		}

		/// <inheritdoc/>
		protected override Color Get(ListViewBase listView)
		{
			return listView.DefaultBackgroundColor;
		}

		/// <inheritdoc/>
		protected override void Set(ListViewBase listView, Color value)
		{
			listView.DefaultBackgroundColor = value;
		}

		/// <inheritdoc/>
		protected override bool Active(ListViewBase widget)
		{
			return widget.AllowColoring && !widget.ColoringStriped;
		}
	}
}