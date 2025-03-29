namespace UIThemes.Wrappers
{
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Theme property for the font of Text.
	/// </summary>
	public class TextFont : Wrapper<Font, Text>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TextFont"/> class.
		/// </summary>
		public TextFont()
		{
			Name = nameof(Text.font);
		}

		/// <inheritdoc/>
		protected override Font Get(Text widget)
		{
			return widget.font;
		}

		/// <inheritdoc/>
		protected override void Set(Text widget, Font value)
		{
			widget.font = value;
		}

		/// <inheritdoc/>
		protected override bool ShouldAttachValue(Text widget)
		{
			return widget.font != null;
		}
	}
}