namespace UIWidgets
{
	using UIWidgets.Attributes;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Rounded corners.
	/// </summary>
	[RequireComponent(typeof(RectTransform))]
	[RequireComponent(typeof(Graphic))]
	[AddComponentMenu("UI/New UI Widgets/Effects/Rounded Corners")]
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/effects/rounded-corners.html")]
	public class RoundedCorners : UVEffect
	{
		/// <summary>
		/// IDs of rounded corners shader properties.
		/// </summary>
		protected static class ShaderIDs
		{
			/// <summary>
			/// Size ID.
			/// </summary>
			[DomainReloadExclude]
			public static readonly int Size = Shader.PropertyToID("_Size");

			/// <summary>
			/// Border radius ID.
			/// </summary>
			[DomainReloadExclude]
			public static readonly int BorderRadius = Shader.PropertyToID("_BorderRadius");

			/// <summary>
			/// Border width ID.
			/// </summary>
			[DomainReloadExclude]
			public static readonly int BorderWidth = Shader.PropertyToID("_BorderWidth");

			/// <summary>
			/// Border color ID.
			/// </summary>
			[DomainReloadExclude]
			public static readonly int BorderColor = Shader.PropertyToID("_BorderColor");
		}

		[SerializeField]
		float radius = 5f;

		/// <summary>
		/// Border radius.
		/// </summary>
		public float Radius
		{
			get => radius;

			set
			{
				if (radius != value)
				{
					radius = ValidateRadius(value);
					borderWidth = ValidateWidth(borderWidth, radius);

					UpdateMaterial();
				}
			}
		}

		[SerializeField]
		float borderWidth = 0f;

		/// <summary>
		/// Border width.
		/// </summary>
		public float BorderWidth
		{
			get => borderWidth;

			set
			{
				if (borderWidth != value)
				{
					borderWidth = ValidateWidth(value, radius);
					UpdateMaterial();
				}
			}
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
				if (borderColor != value)
				{
					borderColor = value;
					UpdateMaterial();
				}
			}
		}

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
				var radius = ValidateRadius(Radius);
				var width = ValidateWidth(BorderWidth, radius);

				EffectMaterial.SetVector(ShaderIDs.Size, size);
				EffectMaterial.SetFloat(ShaderIDs.BorderRadius, radius);
				EffectMaterial.SetFloat(ShaderIDs.BorderWidth, width);
				EffectMaterial.SetColor(ShaderIDs.BorderColor, BorderColor);
			}
		}

		/// <summary>
		/// Validate radius.
		/// </summary>
		/// <param name="radius">Radius.</param>
		/// <returns>Validated radius.</returns>
		protected virtual float ValidateRadius(float radius) => Mathf.Clamp(radius, 0f, MaxRadius());

		/// <summary>
		/// Validate width.
		/// </summary>
		/// <param name="width">Width.</param>
		/// <param name="radius">Radius.</param>
		/// <returns>Validated width.</returns>
		protected virtual float ValidateWidth(float width, float radius) => Mathf.Clamp(width, 0f, radius);

		/// <summary>
		/// Largest allowed radius.
		/// </summary>
		/// <returns>Radius.</returns>
		public virtual float MaxRadius()
		{
			var size = RectTransform.rect.size;
			return Mathf.Min(size.x, size.y) / 2f;
		}
	}
}