namespace UIThemes.Wrappers
{
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Theme property for the disabled color of Selectable.
	/// </summary>
	public class SelectableDisabledColor : SelectableColor
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SelectableDisabledColor"/> class.
		/// </summary>
		public SelectableDisabledColor() => Name = nameof(Selectable.colors.disabledColor);

		/// <inheritdoc/>
		protected override Color Get(ColorBlock colors) => colors.disabledColor;

		/// <inheritdoc/>
		protected override void Set(ref ColorBlock colors, Color value) => colors.disabledColor = value;

		/// <inheritdoc/>
		protected override bool ShouldAttachValue(Selectable widget)
		{
#if UNITY_EDITOR
			if (ThemesReferences.Instance.AttachDefaultSelectable)
			{
				return true;
			}
#endif

			return widget.colors.disabledColor != ColorBlock.defaultColorBlock.disabledColor;
		}
	}
}