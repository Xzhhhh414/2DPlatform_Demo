namespace UIWidgets.UIThemesSupport
{
	using UIThemes.Wrappers;
	using UnityEngine;

	/// <summary>
	/// Theme property for the EndColor of RippleEffect.
	/// </summary>
	public class RippleEffectEndColor : Wrapper<Color, RippleEffect>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="RippleEffectEndColor"/> class.
		/// </summary>
		public RippleEffectEndColor() => Name = nameof(RippleEffect.EndColor);

		/// <inheritdoc/>
		protected override Color Get(RippleEffect widget) => widget.EndColor;

		/// <inheritdoc/>
		protected override void Set(RippleEffect widget, Color value) => widget.EndColor = value;
	}
}