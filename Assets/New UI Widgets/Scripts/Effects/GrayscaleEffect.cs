namespace UIWidgets
{
	using System;
	using UIWidgets.Attributes;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Grayscale effect.
	/// </summary>
	[RequireComponent(typeof(RectTransform))]
	[RequireComponent(typeof(Graphic))]
	[AddComponentMenu("UI/New UI Widgets/Effects/Grayscale Effect")]
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/effects/grayscale.html")]
	public class GrayscaleEffect : UVEffect
	{
		/// <summary>
		/// Color rate.
		/// </summary>
		[Serializable]
		public struct ColorRate
		{
			/// <summary>
			/// Red.
			/// </summary>
			[Range(0f, 1f)]
			public float Red;

			/// <summary>
			/// Green.
			/// </summary>
			[Range(0f, 1f)]
			public float Green;

			/// <summary>
			/// Blue.
			/// </summary>
			[Range(0f, 1f)]
			public float Blue;

			/// <summary>
			/// Initializes a new instance of the <see cref="ColorRate"/> struct.
			/// </summary>
			/// <param name="red">Red.</param>
			/// <param name="green">Green.</param>
			/// <param name="blue">Blue.</param>
			public ColorRate(float red, float green, float blue)
			{
				Red = red;
				Green = green;
				Blue = blue;
			}

			/// <summary>
			/// Determines whether the specified object is equal to the current object.
			/// </summary>
			/// <param name="obj">The object to compare with the current object.</param>
			/// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
			public readonly override bool Equals(object obj) => (obj is ColorRate colorRate) && Equals(colorRate);

			/// <summary>
			/// Determines whether the specified object is equal to the current object.
			/// </summary>
			/// <param name="other">The object to compare with the current object.</param>
			/// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
			public readonly bool Equals(ColorRate other) => Red == other.Red && Green == other.Green && Blue == other.Blue;

			/// <summary>
			/// Hash function.
			/// </summary>
			/// <returns>A hash code for the current object.</returns>
			public override int GetHashCode() => Red.GetHashCode() ^ Green.GetHashCode() ^ Blue.GetHashCode();

			/// <summary>
			/// Compare specified instances.
			/// </summary>
			/// <param name="left">Left instance.</param>
			/// <param name="right">Right instances.</param>
			/// <returns>true if the instances are equal; otherwise, false.</returns>
			public static bool operator ==(ColorRate left, ColorRate right) => left.Equals(right);

			/// <summary>
			/// Compare specified instances.
			/// </summary>
			/// <param name="left">Left instance.</param>
			/// <param name="right">Right instances.</param>
			/// <returns>true if the instances are now equal; otherwise, false.</returns>
			public static bool operator !=(ColorRate left, ColorRate right) => !left.Equals(right);

			/// <summary>
			/// Convert this instance to Color.
			/// </summary>
			/// <param name="rate">Color rate.</param>
			public static implicit operator Color(ColorRate rate) => new Color(rate.Red, rate.Green, rate.Blue);
		}

		/// <summary>
		/// IDs of grayscale shader properties.
		/// </summary>
		protected static class ShaderIDs
		{
			/// <summary>
			/// Rate ID.
			/// </summary>
			[DomainReloadExclude]
			public static readonly int Rate = Shader.PropertyToID("_Rates");

			/// <summary>
			/// Enabled ID.
			/// </summary>
			[DomainReloadExclude]
			public static readonly int Enabled = Shader.PropertyToID("_Enabled");
		}

		[SerializeField]
		ColorRate rate = new ColorRate(0.2126f, 0.7152f, 0.0722f);

		/// <summary>
		/// Rate.
		/// </summary>
		public ColorRate Rate
		{
			get
			{
				return rate;
			}

			set
			{
				rate = value;
				UpdateMaterial();
			}
		}

		[SerializeField]
		bool grayscaleEnabled = true;

		/// <summary>
		/// Grayscale enabled.
		/// </summary>
		public bool GrayscaleEnabled
		{
			get
			{
				return grayscaleEnabled;
			}

			set
			{
				grayscaleEnabled = value;
				UpdateMaterial();
			}
		}

		/// <summary>
		/// Set material properties.
		/// </summary>
		protected override void SetMaterialProperties()
		{
			if (EffectMaterial != null)
			{
				EffectMaterial.SetColor(ShaderIDs.Rate, rate);
				EffectMaterial.SetFloat(ShaderIDs.Enabled, GrayscaleEnabled ? 1f : 0f);
			}
		}
	}
}