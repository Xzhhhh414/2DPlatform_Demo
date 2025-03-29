namespace UIWidgets.UIThemesSupport
{
	using UIThemes.Wrappers;
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// Theme property for the DefaultOddBackgroundColor of ListView.
	/// </summary>
	public class ListViewDefaultOddBackgroundColor : Wrapper<Color, ListViewBase>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ListViewDefaultOddBackgroundColor"/> class.
		/// </summary>
		public ListViewDefaultOddBackgroundColor()
		{
			Name = nameof(ListViewBase.DefaultOddBackgroundColor);
		}

		/// <inheritdoc/>
		protected override Color Get(ListViewBase listView)
		{
			return listView.DefaultOddBackgroundColor;
		}

		/// <inheritdoc/>
		protected override void Set(ListViewBase listView, Color value)
		{
			listView.DefaultOddBackgroundColor = value;
		}

		/// <inheritdoc/>
		protected override bool Active(ListViewBase widget)
		{
			return widget.AllowColoring && widget.ColoringStriped;
		}
	}
}