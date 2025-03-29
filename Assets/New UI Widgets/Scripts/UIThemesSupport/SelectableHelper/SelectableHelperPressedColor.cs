namespace UIWidgets.UIThemesSupport
{
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Theme property for the pressed color of SelectableHelper.
	/// </summary>
	public class SelectableHelperPressedColor : SelectableHelperColor
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SelectableHelperPressedColor"/> class.
		/// </summary>
		public SelectableHelperPressedColor()
		{
			Name = nameof(SelectableHelper.Colors.pressedColor);
		}

		/// <inheritdoc/>
		protected override Color Get(ColorBlock colors)
		{
			return colors.pressedColor;
		}

		/// <inheritdoc/>
		protected override void Set(ref ColorBlock colors, Color value)
		{
			colors.pressedColor = value;
		}

		/// <inheritdoc/>
		protected override bool ShouldAttachValue(SelectableHelper widget)
		{
			return widget.Colors.pressedColor != ColorBlock.defaultColorBlock.pressedColor;
		}
	}
}