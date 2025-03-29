namespace UIThemes.Wrappers
{
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Theme property for the caret color of InputField.
	/// </summary>
	public class InputFieldCaretColor : Wrapper<Color, InputField>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="InputFieldCaretColor"/> class.
		/// </summary>
		public InputFieldCaretColor()
		{
			Name = nameof(InputField.caretColor);
		}

		/// <inheritdoc/>
		protected override Color Get(InputField widget)
		{
			return widget.caretColor;
		}

		/// <inheritdoc/>
		protected override void Set(InputField widget, Color value)
		{
			widget.caretColor = value;
		}

		/// <inheritdoc/>
		protected override bool Active(InputField widget)
		{
			return widget.customCaretColor;
		}

		/// <inheritdoc/>
		protected override bool ShouldAttachValue(InputField widget)
		{
			return widget.customCaretColor;
		}
	}
}