namespace UIWidgets
{
	using System;
	using UIWidgets.Attributes;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Rounded corners, each corner has own radius.
	/// </summary>
	[RequireComponent(typeof(RectTransform))]
	[RequireComponent(typeof(Graphic))]
	[AddComponentMenu("UI/New UI Widgets/Effects/Rounded Corners X4")]
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/effects/rounded-corners.html")]
	public class RoundedCornersX4 : UVEffect
	{
		/// <summary>
		/// Width normal.
		/// </summary>
		[DomainReloadExclude]
		protected static readonly Vector2 WidthNormal = new Vector2(Mathf.Sqrt(0.5f), -Mathf.Sqrt(0.5f));

		/// <summary>
		/// Height normal.
		/// </summary>
		[DomainReloadExclude]
		protected static readonly Vector2 HeightNormal = new Vector2(Mathf.Sqrt(0.5f), Mathf.Sqrt(0.5f));

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

			/// <summary>
			/// Half size (xy) + origin (zw).
			/// </summary>
			[DomainReloadExclude]
			public static readonly int HalfSizeAndOrigin = Shader.PropertyToID("_HalfSizeAndOrigin");

			/// <summary>
			/// Internal half size (xy) + origin (zw).
			/// Internal is minus border width.
			/// </summary>
			[DomainReloadExclude]
			public static readonly int InternalHalfSizeAndOrigin = Shader.PropertyToID("_InternalHalfSizeAndOrigin");
		}

		/// <summary>
		/// Border radius.
		/// </summary>
		[Serializable]
		public struct BorderRadius : IEquatable<BorderRadius>
		{
			/// <summary>
			/// Radius of the top left corner.
			/// </summary>
			[SerializeField]
			public float TopLeft;

			/// <summary>
			/// Radius of the top right corner.
			/// </summary>
			[SerializeField]
			public float TopRight;

			/// <summary>
			/// Radius of the bottom left corner.
			/// </summary>
			[SerializeField]
			public float BottomLeft;

			/// <summary>
			/// Radius of the bottom right corner.
			/// </summary>
			[SerializeField]
			public float BottomRight;

			/// <summary>
			/// Largest radius.
			/// </summary>
			public readonly float Max => Mathf.Max(TopLeft, TopRight, BottomLeft, BottomRight);

			/// <summary>
			/// Initializes a new instance of the <see cref="BorderRadius"/> struct.
			/// </summary>
			/// <param name="topLeft">Radius of the top left corner.</param>
			/// <param name="topRight">Radius of the top right corner.</param>
			/// <param name="bottomLeft">Radius of the bottom left corner.</param>
			/// <param name="bottomRight">Radius of the bottom right corner.</param>
			public BorderRadius(float topLeft, float topRight, float bottomLeft, float bottomRight)
			{
				TopLeft = topLeft;
				TopRight = topRight;
				BottomLeft = bottomLeft;
				BottomRight = bottomRight;
			}

			/// <summary>
			/// Initializes a new instance of the <see cref="BorderRadius"/> struct.
			/// </summary>
			/// <param name="radius">Radius.</param>
			public BorderRadius(Vector4 radius)
			{
				TopLeft = radius.x;
				TopRight = radius.y;
				BottomLeft = radius.w;
				BottomRight = radius.z;
			}

			/// <summary>
			/// BorderRadius can be implicitly converted to the Vector4.
			/// </summary>
			/// <param name="radius">Radius.</param>
			public static implicit operator Vector4(BorderRadius radius)
			{
				return new Vector4(radius.TopLeft, radius.TopRight, radius.BottomRight, radius.BottomLeft);
			}

			/// <summary>
			/// Vector4 can be implicitly converted to the BorderRadius.
			/// </summary>
			/// <param name="radius">Radius.</param>
			public static implicit operator BorderRadius(Vector4 radius) => new BorderRadius(radius);

			/// <summary>
			/// Determines whether the specified object is equal to the current object.
			/// </summary>
			/// <param name="obj">The object to compare with the current object.</param>
			/// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
			public readonly override bool Equals(object obj) => (obj is BorderRadius radius) && Equals(radius);

			/// <summary>
			/// Determines whether the specified object is equal to the current object.
			/// </summary>
			/// <param name="other">The object to compare with the current object.</param>
			/// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
			public readonly bool Equals(BorderRadius other)
			{
				return TopLeft == other.TopLeft && TopRight == other.TopRight && BottomLeft == other.BottomLeft && BottomRight == other.BottomRight;
			}

			/// <summary>
			/// Hash function.
			/// </summary>
			/// <returns>A hash code for the current object.</returns>
			public override int GetHashCode()
			{
				return TopLeft.GetHashCode() ^ TopRight.GetHashCode() ^ BottomLeft.GetHashCode() ^ BottomRight.GetHashCode();
			}

			/// <summary>
			/// Compare specified values.
			/// </summary>
			/// <param name="a">First value.</param>
			/// <param name="b">Second value.</param>
			/// <returns>true if the values are equal; otherwise, false.</returns>
			public static bool operator ==(BorderRadius a, BorderRadius b) => a.Equals(b);

			/// <summary>
			/// Compare specified values.
			/// </summary>
			/// <param name="a">First value.</param>
			/// <param name="b">Second value.</param>
			/// <returns>true if the value not equal; otherwise, false.</returns>
			public static bool operator !=(BorderRadius a, BorderRadius b) => !a.Equals(b);

			/// <summary>
			/// Returns a string that represents the current object.
			/// </summary>
			/// <returns>A string that represents the current object.</returns>
			[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0101:Array allocation for params parameter", Justification = "Required.")]
			public override string ToString()
			{
				return string.Format("BorderRadius({0}, {1}, {2}, {3})", TopLeft.ToString(), TopRight.ToString(), BottomLeft.ToString(), BottomRight.ToString());
			}
		}

		[SerializeField]
		BorderRadius radius = new BorderRadius(5f, 5f, 5f, 5f);

		/// <summary>
		/// Border radius.
		/// </summary>
		public BorderRadius Radius
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

				var corners_half_size = HalfSizeRotated(size, radius);
				var origin = Origin(size, corners_half_size, radius);

				var b_size = new Vector2(size.x - width, size.y - width);
				var b_corners_half_size = HalfSizeRotated(b_size, radius);
				var b_origin = Origin(b_size, b_corners_half_size, radius);

				EffectMaterial.SetVector(ShaderIDs.Size, size);
				EffectMaterial.SetVector(ShaderIDs.HalfSizeAndOrigin, new Vector4(corners_half_size.x, corners_half_size.y, origin.x, origin.y));
				EffectMaterial.SetVector(ShaderIDs.InternalHalfSizeAndOrigin, new Vector4(b_corners_half_size.x, b_corners_half_size.y, b_origin.x, b_origin.y));
				EffectMaterial.SetVector(ShaderIDs.BorderRadius, radius);
				EffectMaterial.SetFloat(ShaderIDs.BorderWidth, width);
				EffectMaterial.SetColor(ShaderIDs.BorderColor, BorderColor);
			}
		}

		/// <summary>
		/// Validate radius.
		/// </summary>
		/// <param name="radius">Radius.</param>
		/// <returns>Validated radius.</returns>
		protected virtual BorderRadius ValidateRadius(BorderRadius radius)
		{
			var max = MaxRadius();

			radius.TopLeft = Mathf.Clamp(radius.TopLeft, 0f, max);
			radius.TopRight = Mathf.Clamp(radius.TopRight, 0f, max);
			radius.BottomLeft = Mathf.Clamp(radius.BottomLeft, 0f, max);
			radius.BottomRight = Mathf.Clamp(radius.BottomRight, 0f, max);

			return radius;
		}

		/// <summary>
		/// Validate width.
		/// </summary>
		/// <param name="width">Width.</param>
		/// <param name="radius">Radius.</param>
		/// <returns>Validated width.</returns>
		protected virtual float ValidateWidth(float width, BorderRadius radius) => Mathf.Clamp(width, 0f, radius.Max);

		/// <summary>
		/// Largest allowed radius.
		/// </summary>
		/// <returns>Radius.</returns>
		public virtual float MaxRadius()
		{
			var size = RectTransform.rect.size;
			return Mathf.Min(size.x, size.y) / 2f;
		}

		/// <summary>
		/// Get half size of rectangle rotated on angle 45.
		/// </summary>
		/// <param name="size">Size.</param>
		/// <param name="radius">Radius.</param>
		/// <returns>Rotated size.</returns>
		protected Vector2 HalfSizeRotated(Vector2 size, BorderRadius radius)
		{
			var left_right = new Vector2(size.x, -size.y + radius.TopLeft + radius.BottomRight);
			var width_half = Vector2.Dot(left_right, WidthNormal) * .5f;

			var bottom_top = new Vector2(size.x, size.y - radius.BottomLeft - radius.TopRight);
			var height_half = Vector2.Dot(bottom_top, HeightNormal) * .5f;

			return new Vector2(width_half, height_half);
		}

		/// <summary>
		/// Get origin.
		/// </summary>
		/// <param name="size">Size.</param>
		/// <param name="halfSizeRotated">Rotated half-size.</param>
		/// <param name="radius">Radius.</param>
		/// <returns>Origin.</returns>
		protected Vector2 Origin(Vector2 size, Vector2 halfSizeRotated, BorderRadius radius)
		{
			var left2top = new Vector2(size.x - radius.TopLeft - radius.TopRight, 0);
			var point = new Vector2(radius.TopLeft - (size.x / 2), size.y / 2);

			return point
				+ (HeightNormal * Vector2.Dot(left2top, HeightNormal))
				+ (WidthNormal * halfSizeRotated.x)
				+ (HeightNormal * -halfSizeRotated.y);
		}
	}
}