namespace UIThemes.Wrappers
{
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Theme property for the texture of RawImage.
	/// </summary>
	public class RawImageTexture : Wrapper<Texture, RawImage>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="RawImageTexture"/> class.
		/// </summary>
		public RawImageTexture()
		{
			Name = nameof(RawImage.texture);
		}

		/// <inheritdoc/>
		protected override Texture Get(RawImage widget)
		{
			return widget.texture;
		}

		/// <inheritdoc/>
		protected override void Set(RawImage widget, Texture value)
		{
			widget.texture = value;
		}

		/// <inheritdoc/>
		protected override bool ShouldAttachValue(RawImage widget)
		{
			return widget.texture != null;
		}
	}
}