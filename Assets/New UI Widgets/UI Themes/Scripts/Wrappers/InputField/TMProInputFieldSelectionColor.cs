#if UIWIDGETS_TMPRO_SUPPORT
namespace UIThemes.Wrappers
{
	using TMPro;
	using UnityEngine;

	/// <summary>
	/// Theme property for the selection color of TMP_InputField.
	/// </summary>
	public class TMProInputFieldSelectionColor : Wrapper<Color, TMP_InputField>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TMProInputFieldSelectionColor"/> class.
		/// </summary>
		public TMProInputFieldSelectionColor()
		{
			Name = nameof(TMP_InputField.selectionColor);
		}

		/// <inheritdoc/>
		protected override Color Get(TMP_InputField widget)
		{
			return widget.selectionColor;
		}

		/// <inheritdoc/>
		protected override void Set(TMP_InputField widget, Color value)
		{
			widget.selectionColor = value;
		}
	}
}
#endif