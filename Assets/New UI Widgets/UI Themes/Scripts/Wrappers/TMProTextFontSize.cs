#if UIWIDGETS_TMPRO_SUPPORT
namespace UIThemes.Wrappers
{
	/// <summary>
	/// Theme property for the font of TMPro Text.
	/// </summary>
	public class TMProTextFontSize : Wrapper<FontSizeValue, TMPro.TMP_Text>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TMProTextFontSize"/> class.
		/// </summary>
		public TMProTextFontSize() => Name = nameof(TMPro.TMP_Text.fontSize);

		/// <inheritdoc/>
		protected override FontSizeValue Get(TMPro.TMP_Text widget) => widget.fontSize;

		/// <inheritdoc/>
		protected override void Set(TMPro.TMP_Text widget, FontSizeValue value) => widget.fontSize = value;
	}
}
#endif