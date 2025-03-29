namespace UIWidgets.UIThemesSupport
{
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Theme property for the disabled color of SelectableHelperList.
	/// </summary>
	public class SelectableHelperListDisabledColor : SelectableHelperListColor
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SelectableHelperListDisabledColor"/> class.
		/// </summary>
		public SelectableHelperListDisabledColor()
		{
			Name = nameof(SelectableHelperList.Colors.disabledColor);
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
		protected override bool ShouldAttachValue(SelectableHelperList widget)
		{
			return widget.Colors.disabledColor != ColorBlock.defaultColorBlock.disabledColor;
		}
	}
}