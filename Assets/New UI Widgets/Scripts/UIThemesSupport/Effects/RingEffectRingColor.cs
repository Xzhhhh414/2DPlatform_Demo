namespace UIWidgets.UIThemesSupport
{
	using UIThemes.Wrappers;
	using UnityEngine;

	/// <summary>
	/// Theme property for the RingColor of RingEffect.
	/// </summary>
	public class RingEffectRingColor : Wrapper<Color, RingEffect>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="RingEffectRingColor"/> class.
		/// </summary>
		public RingEffectRingColor() => Name = nameof(RingEffect.RingColor);

		/// <inheritdoc/>
		protected override Color Get(RingEffect widget) => widget.RingColor;

		/// <inheritdoc/>
		protected override void Set(RingEffect widget, Color value) => widget.RingColor = value;

		/// <inheritdoc/>
		protected override bool Active(RingEffect widget) => widget.Fill;
	}
}