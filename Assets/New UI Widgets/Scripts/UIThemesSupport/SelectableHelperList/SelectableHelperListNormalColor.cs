namespace UIWidgets.UIThemesSupport
{
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Theme property for the normal color of SelectableHelperList.
	/// </summary>
	public class SelectableHelperListNormalColor : SelectableHelperListColor
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SelectableHelperListNormalColor"/> class.
		/// </summary>
		public SelectableHelperListNormalColor()
		{
			Name = nameof(SelectableHelperList.Colors.normalColor);
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
		protected override bool ShouldAttachValue(SelectableHelperList widget)
		{
			return widget.Colors.normalColor != ColorBlock.defaultColorBlock.normalColor;
		}
	}
}