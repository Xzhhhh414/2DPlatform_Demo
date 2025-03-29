namespace UIWidgets
{
	using System;
	using UIWidgets.Attributes;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Ring effect.
	/// </summary>
	[RequireComponent(typeof(RectTransform))]
	[RequireComponent(typeof(Graphic))]
	[AddComponentMenu("UI/New UI Widgets/Effects/Ring Effect")]
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/effects/ring.html")]
	public class RingEffect : UVEffect
	{
		/// <summary>
		/// IDs of ring shader properties.
		/// </summary>
		protected static class ShaderIDs
		{
			/// <summary>
			/// Ring color ID.
			/// </summary>
			[DomainReloadExclude]
			public static readonly int RingColor = Shader.PropertyToID("_RingColor");

			/// <summary>
			/// Thickness ID.
			/// </summary>
			[DomainReloadExclude]
			public static readonly int Thickness = Shader.PropertyToID("_Thickness");

			/// <summary>
			/// Padding ID.
			/// </summary>
			[DomainReloadExclude]
			public static readonly int Padding = Shader.PropertyToID("_Padding");

			/// <summary>
			/// Resolution ID.
			/// </summary>
			[DomainReloadExclude]
			public static readonly int Resolution = Shader.PropertyToID("_Resolution");

			/// <summary>
			/// Fill ID.
			/// </summary>
			[DomainReloadExclude]
			public static readonly int Fill = Shader.PropertyToID("_Fill");

			/// <summary>
			/// Transparent ID.
			/// </summary>
			[DomainReloadExclude]
			public static readonly int Transparent = Shader.PropertyToID("_Transparent");
		}

		[SerializeField]
		[Tooltip("Fill the ring with specified color.")]
		bool fill = true;

		/// <summary>
		/// Fill.
		/// </summary>
		public bool Fill
		{
			get => fill;

			set
			{
				fill = value;
				UpdateMaterial();
			}
		}

		[SerializeField]
		[EditorConditionBool(nameof(fill))]
		Color ringColor = Color.white;

		/// <summary>
		/// Ring color.
		/// </summary>
		public Color RingColor
		{
			get => ringColor;

			set
			{
				ringColor = value;
				UpdateMaterial();
			}
		}

		[SerializeField]
		[EditorConditionBool(nameof(fill))]
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
		float thickness = 10f;

		/// <summary>
		/// Thickness.
		/// </summary>
		public float Thickness
		{
			get => thickness;

			set
			{
				thickness = value;
				UpdateMaterial();
			}
		}

		[SerializeField]
		float padding = 0f;

		/// <summary>
		/// Padding.
		/// </summary>
		public float Padding
		{
			get => padding;

			set
			{
				padding = value;
				UpdateMaterial();
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
				var resolution = Mathf.Max(size.x, size.y);

				EffectMaterial.SetColor(ShaderIDs.RingColor, ringColor);
				EffectMaterial.SetFloat(ShaderIDs.Thickness, thickness);
				EffectMaterial.SetFloat(ShaderIDs.Padding, padding);
				EffectMaterial.SetFloat(ShaderIDs.Resolution, resolution);
				EffectMaterial.SetFloat(ShaderIDs.Fill, fill ? 1 : 0);
				EffectMaterial.SetFloat(ShaderIDs.Transparent, transparentBackground ? 1 : 0);
			}
		}
	}
}