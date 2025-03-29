namespace UIWidgets.UIThemesSupport
{
	using UIThemes.Wrappers;
	using UnityEngine;

	/// <summary>
	/// Theme property for the BackgroundOnColor of Switch.
	/// </summary>
	public class SwitchBackgroundOnColor : Wrapper<Color, Switch>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SwitchBackgroundOnColor"/> class.
		/// </summary>
		public SwitchBackgroundOnColor()
		{
			Name = nameof(Switch.BackgroundOnColor);
		}

		/// <inheritdoc/>
		protected override Color Get(Switch widget)
		{
			return widget.BackgroundOnColor;
		}

		/// <inheritdoc/>
		protected override void Set(Switch widget, Color value)
		{
			widget.BackgroundOnColor = value;
		}
	}
}