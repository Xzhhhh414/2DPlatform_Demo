namespace UIWidgets.UIThemesSupport
{
	using UIThemes.Wrappers;
	using UnityEngine;

	/// <summary>
	/// Theme property for the ColorMax of Rating.
	/// </summary>
	public class RatingColorMax : Wrapper<Color, Rating>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="RatingColorMax"/> class.
		/// </summary>
		public RatingColorMax()
		{
			Name = nameof(Rating.ColorMax);
		}

		/// <inheritdoc/>
		protected override Color Get(Rating widget)
		{
			return widget.ColorMax;
		}

		/// <inheritdoc/>
		protected override void Set(Rating widget, Color value)
		{
			widget.ColorMax = value;
		}
	}
}