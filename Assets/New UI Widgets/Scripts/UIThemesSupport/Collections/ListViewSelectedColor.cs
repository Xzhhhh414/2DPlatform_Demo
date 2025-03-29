namespace UIWidgets.UIThemesSupport
{
	using UIThemes.Wrappers;
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// Theme property for the SelectedColor of ListView.
	/// </summary>
	public class ListViewSelectedColor : Wrapper<Color, ListViewBase>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ListViewSelectedColor"/> class.
		/// </summary>
		public ListViewSelectedColor()
		{
			Name = nameof(ListViewBase.SelectedColor);
		}

		/// <inheritdoc/>
		protected override Color Get(ListViewBase listView)
		{
			return listView.SelectedColor;
		}

		/// <inheritdoc/>
		protected override void Set(ListViewBase listView, Color value)
		{
			listView.SelectedColor = value;
		}

		/// <inheritdoc/>
		protected override bool Active(ListViewBase widget)
		{
			return widget.AllowColoring;
		}
	}
}