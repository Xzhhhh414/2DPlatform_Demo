namespace UIWidgets.UIThemesSupport
{
	using UIThemes.Wrappers;
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Base class for the theme properties for the colors of SelectableHelper.
	/// </summary>
	public abstract class SelectableHelperColor : Wrapper<Color, SelectableHelper>
	{
		/// <inheritdoc/>
		protected override Color Get(SelectableHelper widget)
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
		protected override void Set(SelectableHelper widget, Color value)
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
		protected override bool Active(SelectableHelper widget)
		{
			return widget.Transition == Selectable.Transition.ColorTint;
		}

		/// <inheritdoc/>
		protected override bool ShouldAttachValue(SelectableHelper widget)
		{
			return widget.Colors != ColorBlock.defaultColorBlock;
		}
	}
}