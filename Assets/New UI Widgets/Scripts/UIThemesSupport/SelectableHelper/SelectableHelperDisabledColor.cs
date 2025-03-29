namespace UIWidgets.UIThemesSupport
{
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Theme property for the disabled color of SelectableHelper.
	/// </summary>
	public class SelectableHelperDisabledColor : SelectableHelperColor
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SelectableHelperDisabledColor"/> class.
		/// </summary>
		public SelectableHelperDisabledColor()
		{
			Name = nameof(SelectableHelper.Colors.disabledColor);
		}

		/// <inheritdoc/>
		protected override Color Get(ColorBlock colors)
		{
			return colors.disabledColor;
		}

		/// <inheritdoc/>
		protected override void Set(ref ColorBlock colors, Color value)
		{
			colors.disabledColor = value;
		}

		/// <inheritdoc/>
		protected override bool ShouldAttachValue(SelectableHelper widget)
		{
			return widget.Colors.disabledColor != ColorBlock.defaultColorBlock.disabledColor;
		}
	}
}