namespace UIWidgets.UIThemesSupport
{
	using UIThemes.Wrappers;
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Base class for the theme properties for the colors of SelectableHelperList.
	/// </summary>
	public abstract class SelectableHelperListColor : Wrapper<Color, SelectableHelperList>
	{
		/// <inheritdoc/>
		protected override Color Get(SelectableHelperList widget)
		{
			return Get(widget.Colors);
		}

		/// <summary>
		/// Get value.
		/// </summary>
		/// <param name="colors">Colors.</param>
		/// <returns>Value.</returns>
		protected abstract Color Get(ColorBlock colors);

		/// <inheritdoc/>
		protected override void Set(SelectableHelperList widget, Color value)
		{
			var colors = widget.Colors;
			Set(ref colors, value);
			widget.Colors = colors;
		}

		/// <summary>
		/// Set value.
		/// </summary>
		/// <param name="colors">Colors.</param>
		/// <param name="value">Value.</param>
		protected abstract void Set(ref ColorBlock colors, Color value);

		/// <inheritdoc/>
		protected override bool Active(SelectableHelperList widget)
		{
			return widget.Transition == Selectable.Transition.ColorTint;
		}

		/// <inheritdoc/>
		protected override bool ShouldAttachValue(SelectableHelperList widget)
		{
			return widget.Colors != ColorBlock.defaultColorBlock;
		}
	}
}