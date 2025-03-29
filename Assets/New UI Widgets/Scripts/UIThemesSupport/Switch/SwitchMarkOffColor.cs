namespace UIWidgets.UIThemesSupport
{
	using UIThemes.Wrappers;
	using UnityEngine;

	/// <summary>
	/// Theme property for the MarkOffColor of Switch.
	/// </summary>
	public class SwitchMarkOffColor : Wrapper<Color, Switch>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SwitchMarkOffColor"/> class.
		/// </summary>
		public SwitchMarkOffColor()
		{
			Name = nameof(Switch.MarkOffColor);
		}

		/// <inheritdoc/>
		protected override Color Get(Switch widget)
		{
			return widget.MarkOffColor;
		}

		/// <inheritdoc/>
		protected override void Set(Switch widget, Color value)
		{
			widget.MarkOffColor = value;
		}
	}
}