namespace UIWidgets.UIThemesSupport
{
	using UIThemes.Wrappers;
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// Theme property for the DefaultEvenBackgroundColor of ListView.
	/// </summary>
	public class ListViewDefaultEvenBackgroundColor : Wrapper<Color, ListViewBase>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ListViewDefaultEvenBackgroundColor"/> class.
		/// </summary>
		public ListViewDefaultEvenBackgroundColor()
		{
			Name = nameof(ListViewBase.DefaultEvenBackgroundColor);
		}

		/// <inheritdoc/>
		protected override Color Get(ListViewBase listView)
		{
			return listView.DefaultEvenBackgroundColor;
		}

		/// <inheritdoc/>
		protected override void Set(ListViewBase listView, Color value)
		{
			listView.DefaultEvenBackgroundColor = value;
		}

		/// <inheritdoc/>
		protected override bool Active(ListViewBase widget)
		{
			return widget.AllowColoring && widget.ColoringStriped;
		}
	}
}