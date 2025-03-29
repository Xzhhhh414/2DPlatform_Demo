namespace UIWidgets
{
	using System;
	using UnityEngine;
	using UnityEngine.Serialization;
	using UnityEngine.UI;

	/// <summary>
	/// UV effect.
	/// </summary>
	[RequireComponent(typeof(RectTransform))]
	[RequireComponent(typeof(Graphic))]
	[DisallowMultipleComponent]
	public abstract class UVEffect : BaseMeshEffect, IMaterialModifier, IMeshModifier
	{
		/// <summary>
		/// UV channel.
		/// </summary>
		public enum UVChannel
		{
			/// <summary>
			/// Use UV0.zw.
			/// </summary>
			UV0zw = 0,

			/// <summary>
			/// Use UV1.xy.
			/// </summary>
			UV1xy = 1,
		}

		/// <summary>
		/// UV mode.
		/// </summary>
		public enum UVMode
		{
			/// <summary>
			/// UV0.y limited by width.
			/// x in [0, 1]; y in [0, 1 / (width / height)]
			/// </summary>
			X = 0,

			/// <summary>
			/// UV0.x limited by height.
			/// x in [0, 1 / (height / width)]; y in [0, 1]
			/// </summary>
			Y = 1,

			/// <summary>
			/// UV0.x is not limited by size.
			/// x in [0, 1]; y in [0, 1]
			/// </summary>
			One = 2,

			/// <summary>
			/// If width more that height then works as X mode; otherwise Y mode.
			/// </summary>
			Max = 3,
		}

		/// <summary>
		/// Mesh sizes.
		/// </summary>
		protected readonly struct MeshSize
		{
			/// <summary>
			/// Minimal x and y values.
			/// </summary>
			public readonly Vector2 Min;

			/// <summary>
			/// Maximal x and y values.
			/// </summary>
			public readonly Vector2 Max;

			/// <summary>
			/// Width.
			/// </summary>
			public readonly float Width;

			/// <summary>
			/// Height.
			/// </summary>
			public readonly float Height;

			/// <summary>
			/// Aspect ratio.
			/// </summary>
			public readonly Vector2 AspectRatio;

			/// <summary>
			/// Initializes a new instance of the <see cref="MeshSize"/> struct.
			/// </summary>
			/// <param name="vh">Vertex helper.</param>
			/// <param name="rt">RectTransform.</param>
			/// <param name="mode">UV mode.</param>
			/// <param name="imageType">Image type.</param>
			public MeshSize(VertexHelper vh, RectTransform rt, UVMode mode, Image.Type imageType)
			{
				var size = rt.rect.size;

				if (imageType == Image.Type.Filled)
				{
					var pivot = rt.pivot;
					Min = new Vector2(-size.x * pivot.x, -size.y * pivot.y);
					Max = new Vector2(size.x * (1f - pivot.x), size.y * (1f - pivot.y));
				}
				else
				{
					// get min and max position to calculate uv1
					var vertex = default(UIVertex);
					vh.PopulateUIVertex(ref vertex, 0);
					Min = new Vector2(vertex.position.x, vertex.position.y);
					Max = Min;

					for (int i = 1; i < vh.currentVertCount; i++)
					{
						vh.PopulateUIVertex(ref vertex, i);

						Min.x = Math.Min(Min.x, vertex.position.x);
						Min.y = Math.Min(Min.y, vertex.position.y);

						Max.x = Math.Max(Max.x, vertex.position.x);
						Max.y = Math.Max(Max.y, vertex.position.y);
					}
				}

				// size
				Width = Max.x - Min.x;
				Height = Max.y - Min.y;

				AspectRatio = CalculateAspectRatio(size, mode);
			}

			static Vector2 CalculateAspectRatio(Vector2 size, UVMode mode)
			{
				// calculate aspect ratio
				return mode switch
				{
					UVMode.X => new Vector2(1f, size.x / size.y),
					UVMode.Y => new Vector2(size.y / size.x, 1f),
					UVMode.One => Vector2.one,
					UVMode.Max => (size.x >= size.y) ? new Vector2(1f, size.x / size.y) : new Vector2(size.y / size.x, 1f),
					_ => throw new NotSupportedException(string.Format("Unknown UVMode: {0}", EnumHelper<UVMode>.ToString(mode))),
				};
			}

			/// <summary>
			/// Returns a string that represents the current object.
			/// </summary>
			/// <returns>A string that represents the current object.</returns>
			[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0101:Array allocation for params parameter", Justification = "Required.")]
			[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0601:Value type to reference type conversion causing boxing allocation", Justification = "Required.")]
			public override string ToString()
			{
				return string.Format("MeshSize(Min = {0}; Max = {1}; Width = {2}; Height = {3}; AspectRatio = {4})", Min, Max, Width, Height, AspectRatio);
			}
		}

		/// <summary>
		/// Shader.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("RippleShader")]
		protected Shader EffectShader;

		/// <summary>
		/// Base material.
		/// </summary>
		[NonSerialized]
		protected Material BaseMaterial;

		/// <summary>
		/// Effect material.
		/// </summary>
		[NonSerialized]
		protected Material EffectMaterial;

		[NonSerialized]
		RectTransform rectTransform;

		/// <summary>
		/// RectTransform component.
		/// </summary>
		protected RectTransform RectTransform
		{
			get
			{
				if (rectTransform == null)
				{
					rectTransform = transform as RectTransform;
				}

				return rectTransform;
			}
		}

		UVChannel channel = UVChannel.UV0zw;

		/// <summary>
		/// UV channel
		/// </summary>
		protected UVChannel Channel
		{
			get => channel;

			set
			{
				if (channel != value)
				{
					channel = value;
					graphic.SetVerticesDirty();
				}
			}
		}

		UVMode mode = UVMode.X;

		/// <summary>
		/// UV1 mode.
		/// </summary>
		public UVMode Mode
		{
			get => mode;

			set
			{
				if (mode != value)
				{
					mode = value;
					graphic.SetVerticesDirty();
				}
			}
		}

		/// <summary>
		/// Process the enable event.
		/// </summary>
		protected override void OnEnable()
		{
			base.OnEnable();

			if (graphic != null)
			{
				if ((graphic.canvas != null) && !IsSet(graphic.canvas.additionalShaderChannels, AdditionalCanvasShaderChannels.TexCoord1))
				{
					graphic.canvas.additionalShaderChannels |= AdditionalCanvasShaderChannels.TexCoord1;
				}

				graphic.SetMaterialDirty();
				graphic.SetVerticesDirty();
			}
		}

		/// <summary>
		/// Is channels has specified channel?
		/// </summary>
		/// <param name="channels">Channels.</param>
		/// <param name="channel">Channel.</param>
		/// <returns>true if specified channel is set; otherwise false.</returns>
		protected bool IsSet(AdditionalCanvasShaderChannels channels, AdditionalCanvasShaderChannels channel)
		{
			return (channels & channel) == channel;
		}

		/// <summary>
		/// Process the disable event.
		/// </summary>
		protected override void OnDisable()
		{
			if (graphic != null)
			{
				graphic.SetMaterialDirty();
			}

			base.OnDisable();
		}

		/// <summary>
		/// Process the animation event.
		/// </summary>
		protected override void OnDidApplyAnimationProperties()
		{
			if (graphic != null)
			{
				graphic.SetMaterialDirty();
			}

			base.OnDidApplyAnimationProperties();
		}

#if UNITY_EDITOR

		/// <summary>
		/// Process the validate event.
		/// </summary>
		protected override void OnValidate()
		{
			base.OnValidate();

			if (graphic != null)
			{
				ShaderChanged();
			}
		}

#endif

		/// <summary>
		/// Process the dimensions change event.
		/// </summary>
		protected override void OnRectTransformDimensionsChange()
		{
			base.OnRectTransformDimensionsChange();

			UpdateMaterial();
		}

		/// <summary>
		/// Get image type.
		/// </summary>
		/// <returns>Image type.</returns>
		protected virtual Image.Type ImageType()
		{
			var img = graphic as Image;
			if ((img == null) || (img.sprite == null))
			{
				return Image.Type.Simple;
			}

			return img.type;
		}

		/// <summary>
		/// Add uv1 to the mesh.
		/// </summary>
		/// <param name="vh">Vertex helper.</param>
		public override void ModifyMesh(VertexHelper vh)
		{
			var size = RectTransform.rect.size;
			if ((size.x < 0f) || (size.y < 0f))
			{
				return;
			}

			var vertex = default(UIVertex);
			var mesh_size = new MeshSize(vh, RectTransform, Mode, ImageType());

			switch (Channel)
			{
				case UVChannel.UV0zw:
					for (int i = 0; i < vh.currentVertCount; i++)
					{
						vh.PopulateUIVertex(ref vertex, i);

						vertex.uv0 = new Vector4(
							vertex.uv0.x,
							vertex.uv0.y,
							(vertex.position.x - mesh_size.Min.x) / mesh_size.Width / mesh_size.AspectRatio.x,
							(vertex.position.y - mesh_size.Min.y) / mesh_size.Height / mesh_size.AspectRatio.y);

						vh.SetUIVertex(vertex, i);
					}

					break;
				case UVChannel.UV1xy:
					for (int i = 0; i < vh.currentVertCount; i++)
					{
						vh.PopulateUIVertex(ref vertex, i);

						vertex.uv1 = new Vector2(
							(vertex.position.x - mesh_size.Min.x) / mesh_size.Width / mesh_size.AspectRatio.x,
							(vertex.position.y - mesh_size.Min.y) / mesh_size.Height / mesh_size.AspectRatio.y);

						vh.SetUIVertex(vertex, i);
					}

					break;
				default:
					throw new NotSupportedException(string.Format("Unknown Channel: {0}", EnumHelper<UVChannel>.ToString(Channel)));
			}
		}

		/// <summary>
		/// Init material.
		/// </summary>
		protected virtual void InitMaterial()
		{
			SetMaterialProperties();
		}

		/// <summary>
		/// Set material properties.
		/// </summary>
		protected abstract void SetMaterialProperties();

		/// <summary>
		/// Update material.
		/// </summary>
		protected virtual void UpdateMaterial()
		{
			SetMaterialProperties();

			if (EffectMaterial != null)
			{
				graphic.SetMaterialDirty();
			}
		}

		/// <summary>
		/// Get modified material.
		/// </summary>
		/// <param name="newBaseMaterial">Base material.</param>
		/// <returns>Modified material.</returns>
		public virtual Material GetModifiedMaterial(Material newBaseMaterial)
		{
			if ((BaseMaterial != null) && (newBaseMaterial.GetInstanceID() == BaseMaterial.GetInstanceID()))
			{
				return EffectMaterial;
			}

			if (EffectMaterial != null)
			{
#if UNITY_EDITOR
				DestroyImmediate(EffectMaterial);
#else
				Destroy(EffectMaterial);
#endif
			}

			BaseMaterial = newBaseMaterial;
			EffectMaterial = new Material(newBaseMaterial)
			{
				shader = GetShader(),
			};
			InitMaterial();

			return EffectMaterial;
		}

		/// <summary>
		/// Get shader.
		/// </summary>
		/// <returns>Shader.</returns>
		protected virtual Shader GetShader() => EffectShader;

		/// <summary>
		/// Process shader changes.
		/// </summary>
		protected virtual void ShaderChanged()
		{
			if (EffectMaterial == null)
			{
				return;
			}

			EffectMaterial = new Material(EffectMaterial)
			{
				shader = GetShader(),
			};

			UpdateMaterial();
		}
	}
}