namespace UIWidgets
{
	using UIWidgets.Attributes;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Flare effect.
	/// </summary>
	[RequireComponent(typeof(RectTransform))]
	[RequireComponent(typeof(Graphic))]
	[AddComponentMenu("UI/New UI Widgets/Effects/Flare Effect")]
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/effects/flare.html")]
	public class FlareEffect : UVEffect
	{
		/// <summary>
		/// IDs of flare shader properties.
		/// </summary>
		protected static class ShaderIDs
		{
			/// <summary>
			/// Color.
			/// </summary>
			[DomainReloadExclude]
			public static readonly int Color = Shader.PropertyToID("_FlareColor");

			/// <summary>
			/// Size.
			/// </summary>
			[DomainReloadExclude]
			public static readonly int Size = Shader.PropertyToID("_FlareSize");

			/// <summary>
			/// Speed.
			/// </summary>
			[DomainReloadExclude]
			public static readonly int Speed = Shader.PropertyToID("_FlareSpeed");

			/// <summary>
			/// Delay.
			/// </summary>
			[DomainReloadExclude]
			public static readonly int Delay = Shader.PropertyToID("_FlareDelay");

			/// <summary>
			/// Height delay.
			/// </summary>
			[DomainReloadExclude]
			public static readonly int HeightDelay = Shader.PropertyToID("_FlareHeightDelay");
		}

		/// <summary>
		/// Shader.
		/// </summary>
		[SerializeField]
		protected Shader GlobalShader;

		[SerializeField]
		bool globalSpace = false;

		/// <summary>
		/// Global space.
		/// </summary>
		public bool GlobalSpace
		{
			get => globalSpace;

			set
			{
				if (globalSpace != value)
				{
					globalSpace = value;
					ShaderChanged();
				}
			}
		}

		[SerializeField]
		Color color = Color.white;

		/// <summary>
		/// Color.
		/// </summary>
		public Color Color
		{
			get => color;

			set
			{
				if (color != value)
				{
					color = value;
					UpdateMaterial();
				}
			}
		}

		[SerializeField]
		[Range(0f, 1f)]
		float size = 0.5f;

		/// <summary>
		/// Size.
		/// </summary>
		public float Size
		{
			get => size;

			set
			{
				if (size != value)
				{
					size = value;
					UpdateMaterial();
				}
			}
		}

		[SerializeField]
		float speed = 0.15f;

		/// <summary>
		/// Speed.
		/// </summary>
		public float Speed
		{
			get => speed;

			set
			{
				if (speed != value)
				{
					speed = value;
					UpdateMaterial();
				}
			}
		}

		[SerializeField]
		[Range(0f, 1f)]
		float startPosition = 0f;

		/// <summary>
		/// Delay.
		/// </summary>
		public float StartPosition
		{
			get => startPosition;

			set
			{
				if (startPosition != value)
				{
					startPosition = value;
					UpdateMaterial();
				}
			}
		}

		[SerializeField]
		[Range(-1f, 1f)]
		float angle = 0.2f;

		/// <summary>
		/// Angle.
		/// </summary>
		public float Angle
		{
			get => angle;

			set
			{
				if (angle != value)
				{
					angle = Mathf.Clamp(value, -1f, 1f);
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
				EffectMaterial.SetColor(ShaderIDs.Color, color);
				EffectMaterial.SetFloat(ShaderIDs.Size, Size);
				EffectMaterial.SetFloat(ShaderIDs.Speed, Speed);
				EffectMaterial.SetFloat(ShaderIDs.Delay, StartPosition);
				EffectMaterial.SetFloat(ShaderIDs.HeightDelay, Angle);
			}
		}

		/// <inheritdoc/>
		protected override Shader GetShader() => GlobalSpace ? GlobalShader : base.GetShader();
	}
}