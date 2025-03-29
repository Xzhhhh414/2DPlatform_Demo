namespace UIWidgets.UIThemesSupport
{
	using UIThemes.Wrappers;
	using UnityEngine;

	/// <summary>
	/// Theme property for the ColorMin of Rating.
	/// </summary>
	public class RatingColorMin : Wrapper<Color, Rating>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="RatingColorMin"/> class.
		/// </summary>
		public RatingColorMin()
		{
			Name = nameof(Rating.ColorMin);
		}

		/// <inheritdoc/>
		protected override Color Get(Rating widget)
		{
			return widget.ColorMin;
		}

		/// <inheritdoc/>
		protected override void Set(Rating widget, Color value)
		{
			widget.ColorMin = value;
		}
	}
}