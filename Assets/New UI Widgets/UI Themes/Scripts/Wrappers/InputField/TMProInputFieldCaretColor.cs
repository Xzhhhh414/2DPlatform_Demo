#if UIWIDGETS_TMPRO_SUPPORT
namespace UIThemes.Wrappers
{
	using TMPro;
	using UnityEngine;

	/// <summary>
	/// Theme property for the caret color of TMP_InputField.
	/// </summary>
	public class TMProInputFieldCaretColor : Wrapper<Color, TMP_InputField>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TMProInputFieldCaretColor"/> class.
		/// </summary>
		public TMProInputFieldCaretColor()
		{
			Name = nameof(TMP_InputField.caretColor);
		}

		/// <inheritdoc/>
		protected override Color Get(TMP_InputField widget)
		{
			return widget.caretColor;
		}

		/// <inheritdoc/>
		protected override void Set(TMP_InputField widget, Color value)
		{
			widget.caretColor = value;
		}

		/// <inheritdoc/>
		protected override bool Active(TMP_InputField widget)
		{
			return widget.customCaretColor;
		}

		/// <inheritdoc/>
		protected override bool ShouldAttachValue(TMP_InputField widget)
		{
			return widget.customCaretColor;
		}
	}
}
#endif