namespace UIThemes.Wrappers
{
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Theme property for the selection color of InputField.
	/// </summary>
	public class InputFieldSelectionColor : Wrapper<Color, InputField>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="InputFieldSelectionColor"/> class.
		/// </summary>
		public InputFieldSelectionColor()
		{
			Name = nameof(InputField.selectionColor);
		}

		/// <inheritdoc/>
		protected override Color Get(InputField widget)
		{
			return widget.selectionColor;
		}

		/// <inheritdoc/>
		protected override void Set(InputField widget, Color value)
		{
			widget.selectionColor = value;
		}
	}
}