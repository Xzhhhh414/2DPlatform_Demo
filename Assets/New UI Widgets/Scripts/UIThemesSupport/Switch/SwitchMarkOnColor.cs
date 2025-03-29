namespace UIWidgets.UIThemesSupport
{
	using UIThemes.Wrappers;
	using UnityEngine;

	/// <summary>
	/// Theme property for the MarkOnColor of Switch.
	/// </summary>
	public class SwitchMarkOnColor : Wrapper<Color, Switch>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SwitchMarkOnColor"/> class.
		/// </summary>
		public SwitchMarkOnColor()
		{
			Name = nameof(Switch.MarkOnColor);
		}

		/// <inheritdoc/>
		protected override Color Get(Switch widget)
		{
			return widget.MarkOnColor;
		}

		/// <inheritdoc/>
		protected override void Set(Switch widget, Color value)
		{
			widget.MarkOnColor = value;
		}
	}
}