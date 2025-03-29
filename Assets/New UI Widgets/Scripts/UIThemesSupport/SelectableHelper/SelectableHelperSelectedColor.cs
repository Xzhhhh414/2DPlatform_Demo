namespace UIWidgets.UIThemesSupport
{
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Theme property for the selected color of SelectableHelper.
	/// </summary>
	public class SelectableHelperSelectedColor : SelectableHelperColor
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SelectableHelperSelectedColor"/> class.
		/// </summary>
		public SelectableHelperSelectedColor()
		{
			Name = nameof(SelectableHelper.Colors.selectedColor);
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
		protected override bool ShouldAttachValue(SelectableHelper widget)
		{
			return widget.Colors.selectedColor != ColorBlock.defaultColorBlock.selectedColor;
		}
	}
}