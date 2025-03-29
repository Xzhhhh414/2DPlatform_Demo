namespace UIWidgets.UIThemesSupport
{
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Theme property for the normal color of SelectableHelper.
	/// </summary>
	public class SelectableHelperNormalColor : SelectableHelperColor
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SelectableHelperNormalColor"/> class.
		/// </summary>
		public SelectableHelperNormalColor()
		{
			Name = nameof(SelectableHelper.Colors.normalColor);
		}

		/// <inheritdoc/>
		protected override Color Get(ColorBlock colors)
		{
			return colors.normalColor;
		}

		/// <inheritdoc/>
		protected override void Set(ref ColorBlock colors, Color value)
		{
			colors.normalColor = value;
		}

		/// <inheritdoc/>
		protected override bool ShouldAttachValue(SelectableHelper widget)
		{
			return widget.Colors.normalColor != ColorBlock.defaultColorBlock.normalColor;
		}
	}
}