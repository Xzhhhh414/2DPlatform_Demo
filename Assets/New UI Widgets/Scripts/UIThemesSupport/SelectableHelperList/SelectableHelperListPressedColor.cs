namespace UIWidgets.UIThemesSupport
{
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Theme property for the pressed color of SelectableHelperList.
	/// </summary>
	public class SelectableHelperListPressedColor : SelectableHelperListColor
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SelectableHelperListPressedColor"/> class.
		/// </summary>
		public SelectableHelperListPressedColor()
		{
			Name = nameof(SelectableHelperList.Colors.pressedColor);
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
		protected override bool ShouldAttachValue(SelectableHelperList widget)
		{
			return widget.Colors.pressedColor != ColorBlock.defaultColorBlock.pressedColor;
		}
	}
}