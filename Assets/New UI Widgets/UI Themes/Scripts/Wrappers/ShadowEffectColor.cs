namespace UIThemes.Wrappers
{
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Theme property for the effectColor of Shadow.
	/// </summary>
	public class ShadowEffectColor : Wrapper<Color, Shadow>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ShadowEffectColor"/> class.
		/// </summary>
		public ShadowEffectColor()
		{
			Name = nameof(Shadow.effectColor);
		}

		/// <inheritdoc/>
		protected override Color Get(Shadow widget)
		{
			return widget.effectColor;
		}

		/// <inheritdoc/>
		protected override void Set(Shadow widget, Color value)
		{
			widget.effectColor = value;
		}
	}
}