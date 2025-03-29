namespace UIWidgets
{
	using UIWidgets.Attributes;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Border effect.
	/// </summary>
	[RequireComponent(typeof(RectTransform))]
	[RequireComponent(typeof(Graphic))]
	[AddComponentMenu("UI/New UI Widgets/Effects/Border Effect")]
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/effects/border.html")]
	public class BorderEffect : UVEffect
	{
		/// <summary>
		/// IDs of ring shader properties.
		/// </summary>
		protected static class ShaderIDs
		{
			/// <summary>
			/// Line color ID.
			/// </summary>
			[DomainReloadExclude]
			public static readonly int BorderColor = Shader.PropertyToID("_BorderColor");

			/// <summary>
			/// Borders ID.
			/// </summary>
			[DomainReloadExclude]
			public static readonly int Borders = Shader.PropertyToID("_Borders");

			/// <summary>
			/// Resolution X ID.
			/// </summary>
			[DomainReloadExclude]
			public static readonly int ResolutionX = Shader.PropertyToID("_ResolutionX");

			/// <summary>
			/// Resolution Y ID.
			/// </summary>
			[DomainReloadExclude]
			public static readonly int ResolutionY = Shader.PropertyToID("_ResolutionY");

			/// <summary>
			/// Transparent ID.
			/// </summary>
			[DomainReloadExclude]
			public static readonly int Transparent = Shader.PropertyToID("_Transparent");
		}

		[SerializeField]
		Color borderColor = Color.white;

		/// <summary>
		/// Border color.
		/// </summary>
		public Color BorderColor
		{
			get => borderColor;

			set
			{
				borderColor = value;
				UpdateMaterial();
			}
		}

		[SerializeField]
		[Tooltip("Make the background transparent.")]
		bool transparentBackground = false;

		/// <summary>
		/// Make the background transparent.
		/// </summary>
		public bool TransparentBackground
		{
			get => transparentBackground;

			set
			{
				transparentBackground = value;
				UpdateMaterial();
			}
		}

		[SerializeField]
		Vector2 horizontalBorders = new Vector2(1f, 1f);

		/// <summary>
		/// Horizontal borders.
		/// </summary>
		public Vector2 HorizontalBorders
		{
			get => horizontalBorders;

			set
			{
				horizontalBorders = value;
				UpdateMaterial();
			}
		}

		[SerializeField]
		Vector2 verticalBorders = new Vector2(1f, 1f);

		/// <summary>
		/// Thickness.
		/// </summary>
		public Vector2 VerticalBorders
		{
			get => verticalBorders;

			set
			{
				verticalBorders = value;
				UpdateMaterial();
			}
		}

		/// <summary>
		/// Borders.
		/// </summary>
		protected Vector4 Borders => new Vector4(horizontalBorders.x, horizontalBorders.y, verticalBorders.x, verticalBorders.y);

		/// <inheritdoc/>
		protected override void OnEnable()
		{
			base.OnEnable();

			Mode = UVMode.One;
		}

		/// <summary>
		/// Set material properties.
		/// </summary>
		protected override void SetMaterialProperties()
		{
			if (EffectMaterial != null)
			{
				var size = RectTransform.rect.size;

				EffectMaterial.SetColor(ShaderIDs.BorderColor, BorderColor);
				EffectMaterial.SetVector(ShaderIDs.Borders, Borders);
				EffectMaterial.SetFloat(ShaderIDs.ResolutionX, size.x);
				EffectMaterial.SetFloat(ShaderIDs.ResolutionY, size.y);
				EffectMaterial.SetFloat(ShaderIDs.Transparent, transparentBackground ? 1 : 0);
			}
		}
	}
}