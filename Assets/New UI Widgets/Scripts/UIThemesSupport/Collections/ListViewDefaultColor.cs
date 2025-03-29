namespace UIWidgets.UIThemesSupport
{
	using UIThemes.Wrappers;
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// Theme property for the DefaultColor of ListView.
	/// </summary>
	public class ListViewDefaultColor : Wrapper<Color, ListViewBase>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ListViewDefaultColor"/> class.
		/// </summary>
		public ListViewDefaultColor()
		{
			Name = nameof(ListViewBase.DefaultColor);
		}

		/// <inheritdoc/>
		protected override Color Get(ListViewBase listView)
		{
			return listView.DefaultColor;
		}

		/// <inheritdoc/>
		protected override void Set(ListViewBase listView, Color value)
		{
			listView.DefaultColor = value;
		}

		/// <inheritdoc/>
		protected override bool Active(ListViewBase widget)
		{
			return widget.AllowColoring;
		}
	}
}