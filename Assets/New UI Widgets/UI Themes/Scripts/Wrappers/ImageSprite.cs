namespace UIThemes.Wrappers
{
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Theme property for the sprite of Image.
	/// </summary>
	public class ImageSprite : Wrapper<Sprite, Image>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ImageSprite"/> class.
		/// </summary>
		public ImageSprite()
		{
			Name = nameof(Image.sprite);
		}

		/// <inheritdoc/>
		protected override Sprite Get(Image widget)
		{
			return widget.sprite;
		}

		/// <inheritdoc/>
		protected override void Set(Image widget, Sprite value)
		{
			widget.sprite = value;
		}

		/// <inheritdoc/>
		protected override bool ShouldAttachValue(Image widget)
		{
			return !Utilities.IsExcludedSprite(widget.sprite);
		}
	}
}