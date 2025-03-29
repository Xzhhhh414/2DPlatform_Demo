namespace UIWidgets.UIThemesSupport
{
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Theme property for the selected color of SelectableHelperList.
	/// </summary>
	public class SelectableHelperListSelectedColor : SelectableHelperListColor
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SelectableHelperListSelectedColor"/> class.
		/// </summary>
		public SelectableHelperListSelectedColor()
		{
			Name = nameof(SelectableHelperList.Colors.selectedColor);
		}

		/// <inheritdoc/>
		protected override Color Get(ColorBlock colors)
		{
			return colors.selectedColor;
		}

		/// <inheritdoc/>
		protected override void Set(ref ColorBlock colors, Color value)
		{
			colors.selectedColor = value;
		}

		/// <inheritdoc/>
		protected override bool ShouldAttachValue(SelectableHelperList widget)
		{
			return widget.Colors.selectedColor != ColorBlock.defaultColorBlock.selectedColor;
		}
	}
}