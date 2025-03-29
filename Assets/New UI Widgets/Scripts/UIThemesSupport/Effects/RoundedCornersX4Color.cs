namespace UIWidgets.UIThemesSupport
{
	using UIThemes.Wrappers;
	using UnityEngine;

	/// <summary>
	/// Theme property for the BorderColorX4 of RoundedCorners.
	/// </summary>
	public class RoundedCornersX4Color : Wrapper<Color, RoundedCornersX4>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="RoundedCornersX4Color"/> class.
		/// </summary>
		public RoundedCornersX4Color() => Name = nameof(RoundedCornersX4.BorderColor);

		/// <inheritdoc/>
		protected override Color Get(RoundedCornersX4 widget) => widget.BorderColor;

		/// <inheritdoc/>
		protected override void Set(RoundedCornersX4 widget, Color value) => widget.BorderColor = value;
	}
}