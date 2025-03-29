namespace UIThemes.Wrappers
{
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Theme property for the color of Graphic.
	/// </summary>
	public class GraphicColor : Wrapper<Color, Graphic>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="GraphicColor"/> class.
		/// </summary>
		public GraphicColor()
		{
			Name = nameof(Graphic.color);
		}

		/// <inheritdoc/>
		protected override Color Get(Graphic widget)
		{
			return widget.color;
		}

		/// <inheritdoc/>
		protected override void Set(Graphic widget, Color value)
		{
			widget.color = value;
		}

		/// <inheritdoc/>
		protected override bool ShouldAttachValue(Graphic widget)
		{
			var img = widget as Image;
			if (img == null)
			{
				return true;
			}

			return Utilities.ShouldAttachSprite(img.sprite, img.color);
		}
	}
}