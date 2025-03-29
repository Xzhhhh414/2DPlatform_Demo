namespace UIWidgets.UIThemesSupport
{
	using UIThemes.Wrappers;
	using UnityEngine;

	/// <summary>
	/// Theme property for the StartColor of RippleEffect.
	/// </summary>
	public class RippleEffectStartColor : Wrapper<Color, RippleEffect>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="RippleEffectStartColor"/> class.
		/// </summary>
		public RippleEffectStartColor() => Name = nameof(RippleEffect.StartColor);

		/// <inheritdoc/>
		protected override Color Get(RippleEffect widget) => widget.StartColor;

		/// <inheritdoc/>
		protected override void Set(RippleEffect widget, Color value) => widget.StartColor = value;
	}
}