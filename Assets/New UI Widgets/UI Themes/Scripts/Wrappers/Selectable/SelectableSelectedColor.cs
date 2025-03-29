namespace UIThemes.Wrappers
{
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Theme property for the selected color of Selectable.
	/// </summary>
	public class SelectableSelectedColor : SelectableColor
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SelectableSelectedColor"/> class.
		/// </summary>
		public SelectableSelectedColor() => Name = nameof(Selectable.colors.selectedColor);

		/// <inheritdoc/>
		protected override Color Get(ColorBlock colors) => colors.selectedColor;

		/// <inheritdoc/>
		protected override void Set(ref ColorBlock colors, Color value) => colors.selectedColor = value;

		/// <inheritdoc/>
		protected override bool ShouldAttachValue(Selectable widget)
		{
#if UNITY_EDITOR
			if (ThemesReferences.Instance.AttachDefaultSelectable)
			{
				return true;
			}
#endif

			return widget.colors.selectedColor != ColorBlock.defaultColorBlock.selectedColor;
		}
	}
}