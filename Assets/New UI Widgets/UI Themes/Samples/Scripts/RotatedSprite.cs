namespace UIThemes.Samples
{
	using System;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Rotated sprite.
	/// </summary>
	[Serializable]
	public struct RotatedSprite : IEquatable<RotatedSprite>
	{
		/// <summary>
		/// Sprite.
		/// </summary>
		[SerializeField]
		public Sprite Sprite;

		/// <summary>
		/// Rotation on Z axis.
		/// </summary>
		[SerializeField]
		public float RotationZ;

		/// <summary>
		/// Initializes a new instance of the <see cref="RotatedSprite"/> struct.
		/// </summary>
		/// <param name="image">Image.</param>
		public RotatedSprite(Image image)
		{
			if (image == null)
			{
				Sprite = null;
				RotationZ = 0f;
			}
			else
			{
				Sprite = image.sprite;
				RotationZ = image.transform.localRotation.eulerAngles.z;
			}
		}

		/// <summary>
		/// Compare with other rotated sprite.
		/// </summary>
		/// <param name="other">Other rotated sprite.</param>
		/// <returns>true if rotated sprites are equals; otherwise false.</returns>
		public readonly bool Equals(RotatedSprite other)
		{
			if (!Mathf.Approximately(RotationZ, other.RotationZ))
			{
				return false;
			}

			return UnityObjectComparer<Sprite>.Instance.Equals(Sprite, other.Sprite);
		}

		/// <summary>
		/// Set value.
		/// </summary>
		/// <param name="image">Image.</param>
		/// <returns>true if value was changed; otherwise false.</returns>
		public readonly bool Set(Image image)
		{
			if (image == null)
			{
				return false;
			}

			var rotation = image.transform.localRotation.eulerAngles;
			if (UnityObjectComparer<Sprite>.Instance.Equals(image.sprite, Sprite) && Mathf.Approximately(rotation.z, RotationZ))
			{
				return false;
			}

			image.sprite = Sprite;
			rotation.z = RotationZ;
			image.transform.localRotation = Quaternion.Euler(rotation);

			return true;
		}
	}
}