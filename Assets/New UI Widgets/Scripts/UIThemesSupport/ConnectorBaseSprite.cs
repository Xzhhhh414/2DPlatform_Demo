namespace UIWidgets.UIThemesSupport
{
	using UIThemes.Wrappers;
	using UnityEngine;

	/// <summary>
	/// Theme property for the sprite of ConnectorBase.
	/// </summary>
	public class ConnectorBaseSprite : Wrapper<Sprite, ConnectorBase>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ConnectorBaseSprite"/> class.
		/// </summary>
		public ConnectorBaseSprite()
		{
			Name = nameof(ConnectorBase.Sprite);
		}

		/// <inheritdoc/>
		protected override Sprite Get(ConnectorBase widget)
		{
			return widget.Sprite;
		}

		/// <inheritdoc/>
		protected override void Set(ConnectorBase widget, Sprite value)
		{
			widget.Sprite = value;
		}

		/// <inheritdoc/>
		protected override bool ShouldAttachValue(ConnectorBase widget)
		{
			return widget.Sprite != null;
		}
	}
}