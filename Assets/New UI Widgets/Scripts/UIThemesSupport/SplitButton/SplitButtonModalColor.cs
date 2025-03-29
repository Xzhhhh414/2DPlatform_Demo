namespace UIWidgets.UIThemesSupport
{
	using UIThemes.Wrappers;
	using UnityEngine;

	/// <summary>
	/// Theme property for the ModalColor of SplitButton.
	/// </summary>
	public class SplitButtonModalColor : Wrapper<Color, SplitButton>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SplitButtonModalColor"/> class.
		/// </summary>
		public SplitButtonModalColor()
		{
			Name = nameof(SplitButton.ModalColor);
		}

		/// <inheritdoc/>
		protected override Color Get(SplitButton widget)
		{
			return widget.ModalColor;
		}

		/// <inheritdoc/>
		protected override void Set(SplitButton widget, Color value)
		{
			widget.ModalColor = value;
		}
	}
}