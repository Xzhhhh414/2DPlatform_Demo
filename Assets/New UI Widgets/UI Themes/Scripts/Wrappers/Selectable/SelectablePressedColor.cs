namespace UIThemes.Wrappers
{
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Theme property for the pressed color of Selectable.
	/// </summary>
	public class SelectablePressedColor : SelectableColor
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SelectablePressedColor"/> class.
		/// </summary>
		public SelectablePressedColor() => Name = nameof(Selectable.colors.pressedColor);

		/// <inheritdoc/>
		protected override Color Get(ColorBlock colors) => colors.pressedColor;

		/// <inheritdoc/>
		protected override void Set(ref ColorBlock colors, Color value) => colors.pressedColor = value;

		/// <inheritdoc/>
		protected override bool ShouldAttachValue(Selectable widget)
		{
#if UNITY_EDITOR
			if (ThemesReferences.Instance.AttachDefaultSelectable)
			{
				return true;
			}
#endif

			return widget.colors.pressedColor != ColorBlock.defaultColorBlock.pressedColor;
		}
	}
}