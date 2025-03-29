namespace UIWidgets.UIThemesSupport
{
	using UIThemes.Wrappers;
	using UnityEngine;

	/// <summary>
	/// Theme property for the BackgroundOffColor of Switch.
	/// </summary>
	public class SwitchBackgroundOffColor : Wrapper<Color, Switch>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SwitchBackgroundOffColor"/> class.
		/// </summary>
		public SwitchBackgroundOffColor()
		{
			Name = nameof(Switch.BackgroundOffColor);
		}

		/// <inheritdoc/>
		protected override Color Get(Switch widget)
		{
			return widget.BackgroundOffColor;
		}

		/// <inheritdoc/>
		protected override void Set(Switch widget, Color value)
		{
			widget.BackgroundOffColor = value;
		}
	}
}