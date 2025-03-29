namespace UIWidgets.UIThemesSupport
{
	using UIThemes.Wrappers;
	using UnityEngine;

	/// <summary>
	/// Theme property for the LineColor of LinesDrawerBase.
	/// </summary>
	public class LinesDrawerBaseLineColor : Wrapper<Color, LinesDrawerBase>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="LinesDrawerBaseLineColor"/> class.
		/// </summary>
		public LinesDrawerBaseLineColor() => Name = nameof(LinesDrawerBase.LineColor);

		/// <inheritdoc/>
		protected override Color Get(LinesDrawerBase widget) => widget.LineColor;

		/// <inheritdoc/>
		protected override void Set(LinesDrawerBase widget, Color value) => widget.LineColor = value;
	}
}