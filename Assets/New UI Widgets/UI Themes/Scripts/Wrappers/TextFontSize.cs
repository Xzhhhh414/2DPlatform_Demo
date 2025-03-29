namespace UIThemes.Wrappers
{
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Theme property for the fontSize of Text.
	/// </summary>
	public class TextFontSize : Wrapper<FontSizeValue, Text>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TextFontSize"/> class.
		/// </summary>
		public TextFontSize() => Name = nameof(Text.fontSize);

		/// <inheritdoc/>
		protected override FontSizeValue Get(Text widget) => widget.fontSize;

		/// <inheritdoc/>
		protected override void Set(Text widget, FontSizeValue value) => widget.fontSize = Mathf.RoundToInt(value);
	}
}