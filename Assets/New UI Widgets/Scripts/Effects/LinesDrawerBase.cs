namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using UIWidgets.Attributes;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Base class for the lines drawer.
	/// </summary>
	[RequireComponent(typeof(RectTransform))]
	[RequireComponent(typeof(Graphic))]
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/effects/lines-drawer.html")]
	public abstract class LinesDrawerBase : UVEffect
	{
		/// <summary>
		/// IDs of shader properties.
		/// </summary>
		protected static class ShaderIDs
		{
			/// <summary>
			/// Line color ID.
			/// </summary>
			[DomainReloadExclude]
			public static readonly int LineColor = Shader.PropertyToID("_LineColor");

			/// <summary>
			/// Line thickness ID.
			/// </summary>
			[DomainReloadExclude]
			public static readonly int LineThickness = Shader.PropertyToID("_LineThickness");

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
			/// Horizontal lines count ID.
			/// </summary>
			[DomainReloadExclude]
			public static readonly int HorizontalLinesCount = Shader.PropertyToID("_HLinesCount");

			/// <summary>
			/// Horizontal lines ID.
			/// </summary>
			[DomainReloadExclude]
			public static readonly int HorizontalLines = Shader.PropertyToID("_HLines");

			/// <summary>
			/// Vertical lines count ID.
			/// </summary>
			[DomainReloadExclude]
			public static readonly int VerticalLinesCount = Shader.PropertyToID("_VLinesCount");

			/// <summary>
			/// Vertical lines ID.
			/// </summary>
			[DomainReloadExclude]
			public static readonly int VerticalLines = Shader.PropertyToID("_VLines");

			/// <summary>
			/// Transparent ID.
			/// </summary>
			[DomainReloadExclude]
			public static readonly int Transparent = Shader.PropertyToID("_Transparent");
		}

		[SerializeField]
		Color lineColor = Color.black;

		/// <summary>
		/// Line color.
		/// </summary>
		public Color LineColor
		{
			get
			{
				return lineColor;
			}

			set
			{
				if (lineColor != value)
				{
					lineColor = value;
					UpdateMaterial();
				}
			}
		}

		[SerializeField]
		float lineThickness = 1f;

		/// <summary>
		/// Line thickness.
		/// </summary>
		public float LineThickness
		{
			get
			{
				return lineThickness;
			}

			set
			{
				if (lineThickness != value)
				{
					lineThickness = value;
					UpdateMaterial();
				}
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
			get
			{
				return transparentBackground;
			}

			set
			{
				if (transparentBackground != value)
				{
					transparentBackground = value;
					UpdateMaterial();
				}
			}
		}

		/// <summary>
		/// Horizontal lines.
		/// </summary>
		protected List<float> HorizontalLines = new List<float>();

		/// <summary>
		/// Vertical lines.
		/// </summary>
		protected List<float> VerticalLines = new List<float>();

		/// <summary>
		/// Shader horizontal lines.
		/// </summary>
		protected List<float> ShaderHorizontalLines = new List<float>(200);

		/// <summary>
		/// Shader vertical lines.
		/// </summary>
		protected List<float> ShaderVerticalLines = new List<float>(200);

		/// <summary>
		/// Max lines count. Should match with shader setting.
		/// </summary>
		protected int MaxLinesCount = 200;

		/// <inheritdoc/>
		protected override void OnEnable()
		{
			base.OnEnable();

			Mode = UVMode.One;

			UpdateLines();
		}

		/// <summary>
		/// Update lines.
		/// </summary>
		protected virtual void UpdateLines()
		{
			UpdateMaterial();
		}

		/// <summary>
		/// Set material properties.
		/// </summary>
		protected override void SetMaterialProperties()
		{
			if (EffectMaterial != null)
			{
				var size = RectTransform.rect.size;

				UpdateShaderLines(HorizontalLines, ShaderHorizontalLines);
				UpdateShaderLines(VerticalLines, ShaderVerticalLines);

				EffectMaterial.SetColor(ShaderIDs.LineColor, lineColor);
				EffectMaterial.SetFloat(ShaderIDs.LineThickness, lineThickness);
				EffectMaterial.SetFloat(ShaderIDs.ResolutionX, size.x);
				EffectMaterial.SetFloat(ShaderIDs.ResolutionY, size.y);
				EffectMaterial.SetFloat(ShaderIDs.Transparent, transparentBackground ? 1 : 0);

				EffectMaterial.SetInt(ShaderIDs.HorizontalLinesCount, HorizontalLines.Count);
				EffectMaterial.SetFloatArray(ShaderIDs.HorizontalLines, ShaderHorizontalLines);

				EffectMaterial.SetInt(ShaderIDs.VerticalLinesCount, VerticalLines.Count);
				EffectMaterial.SetFloatArray(ShaderIDs.VerticalLines, ShaderVerticalLines);
			}
		}

		/// <summary>
		/// Update shader lines.
		/// </summary>
		/// <param name="lines">Lines.</param>
		/// <param name="shaderLines">Shader lines.</param>
		protected void UpdateShaderLines(List<float> lines, List<float> shaderLines)
		{
			shaderLines.Clear();
			shaderLines.AddRange(lines);

			for (int i = shaderLines.Count; i < MaxLinesCount; i++)
			{
				shaderLines.Add(0);
			}
		}
	}
}