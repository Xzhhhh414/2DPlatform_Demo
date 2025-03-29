namespace UIWidgets.Examples
{
	using System;
	using UnityEngine;
	using UnityEngine.EventSystems;
	using UnityEngine.UI;

	/// <summary>
	/// Test gradient.
	/// </summary>
	[RequireComponent(typeof(Image))]
	public class TestGradient : UIBehaviour, IMaterialModifier
	{
		/// <summary>
		/// Shader.
		/// </summary>
		[SerializeField]
		protected Shader Shader;

		[SerializeField]
		Color left = Color.red;

		/// <summary>
		/// Left color.
		/// </summary>
		public Color Left
		{
			get => left;

			set
			{
				left = value;
				UpdateMaterial();
			}
		}

		[SerializeField]
		Color right = Color.green;

		/// <summary>
		/// Right color.
		/// </summary>
		public Color Right
		{
			get => right;

			set
			{
				right = value;
				UpdateMaterial();
			}
		}

		[NonSerialized]
		Image image;

		/// <summary>
		/// Image.
		/// </summary>
		protected Image Image
		{
			get
			{
				if (image == null)
				{
					TryGetComponent(out image);
				}

				return image;
			}
		}

		/// <summary>
		/// Base material.
		/// </summary>
		[NonSerialized]
		protected Material BaseMaterial;

		/// <summary>
		/// Material.
		/// </summary>
		[NonSerialized]
		protected Material Material;

		/// <summary>
		/// Process the enable event.
		/// </summary>
		protected override void OnEnable()
		{
			base.OnEnable();

			if (Image != null)
			{
				Image.SetMaterialDirty();
			}
		}

		/// <summary>
		/// Process the disable event.
		/// </summary>
		protected override void OnDisable()
		{
			if (Image != null)
			{
				Image.SetMaterialDirty();
			}

			base.OnDisable();
		}

		void SetMaterialProperties()
		{
			if (Material != null)
			{
				Material.SetColor("_ColorLeft", Left);
				Material.SetColor("_ColorRight", Right);
			}
		}

		void UpdateMaterial()
		{
			SetMaterialProperties();

			Image.SetMaterialDirty();
		}

		/// <summary>
		/// Get modified material.
		/// </summary>
		/// <param name="newBaseMaterial">Base material.</param>
		/// <returns>Modified material.</returns>
		public Material GetModifiedMaterial(Material newBaseMaterial)
		{
			if ((BaseMaterial != null) && (newBaseMaterial.GetInstanceID() == BaseMaterial.GetInstanceID()))
			{
				return Material;
			}

			if (Material != null)
			{
#if UNITY_EDITOR
				DestroyImmediate(Material);
#else
				Destroy(Material);
#endif
			}

			BaseMaterial = newBaseMaterial;
			Material = new Material(newBaseMaterial)
			{
				shader = Shader,
			};
			SetMaterialProperties();

			return Material;
		}

#if UNITY_EDITOR

		/// <summary>
		/// Process the validate event.
		/// </summary>
		protected override void OnValidate()
		{
			base.OnValidate();

			UpdateMaterial();
		}
#endif
	}
}