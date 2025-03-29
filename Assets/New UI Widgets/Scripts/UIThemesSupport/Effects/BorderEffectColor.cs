namespace UIWidgets.UIThemesSupport
{
	using UIThemes.Wrappers;
	using UnityEngine;

	/// <summary>
	/// Theme property for the BorderColor of BorderEffect.
	/// </summary>
	public class BorderEffectColor : Wrapper<Color, BorderEffect>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="BorderEffectColor"/> class.
		/// </summary>
		public BorderEffectColor() => Name = nameof(BorderEffect.BorderColor);

		/// <inheritdoc/>
		protected override Color Get(BorderEffect widget) => widget.BorderColor;

		/// <inheritdoc/>
		protected override void Set(BorderEffect widget, Color value) => widget.BorderColor = value;
	}
}