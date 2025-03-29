namespace UIWidgets.UIThemesSupport
{
	using UIThemes.Wrappers;
	using UnityEngine;

	/// <summary>
	/// Theme property for the BorderColor of RoundedCorners.
	/// </summary>
	public class RoundedCornersColor : Wrapper<Color, RoundedCorners>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="RoundedCornersColor"/> class.
		/// </summary>
		public RoundedCornersColor() => Name = nameof(RoundedCorners.BorderColor);

		/// <inheritdoc/>
		protected override Color Get(RoundedCorners widget) => widget.BorderColor;

		/// <inheritdoc/>
		protected override void Set(RoundedCorners widget, Color value) => widget.BorderColor = value;
	}
}