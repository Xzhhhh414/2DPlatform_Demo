namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using UIWidgets.Attributes;
	using UnityEngine;
	using UnityEngine.EventSystems;
	using UnityEngine.Serialization;
	using UnityEngine.UI;

	/// <summary>
	/// Ripple effect.
	/// </summary>
	[RequireComponent(typeof(RectTransform))]
	[RequireComponent(typeof(Graphic))]
	[AddComponentMenu("UI/New UI Widgets/Effects/Ripple Effect")]
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/effects/ripple.html")]
	public class RippleEffect : UVEffect, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IMaterialModifier, IMeshModifier
	{
		/// <summary>
		/// Max ripples count.
		/// </summary>
		protected const int MaxRipples = 10;

		/// <summary>
		/// Float values per ripple.
		/// </summary>
		protected const int FloatPerRipple = 3;

		/// <summary>
		/// IDs of ripple shader properties.
		/// </summary>
		protected static class ShaderIDs
		{
			/// <summary>
			/// Start color ID.
			/// </summary>
			[DomainReloadExclude]
			public static readonly int StartColor = Shader.PropertyToID("_RippleStartColor");

			/// <summary>
			/// End color ID.
			/// </summary>
			[DomainReloadExclude]
			public static readonly int EndColor = Shader.PropertyToID("_RippleEndColor");

			/// <summary>
			/// Speed ID.
			/// </summary>
			[DomainReloadExclude]
			public static readonly int Speed = Shader.PropertyToID("_RippleSpeed");

			/// <summary>
			/// Max size ID.
			/// </summary>
			[DomainReloadExclude]
			public static readonly int MaxSize = Shader.PropertyToID("_RippleMaxSize");

			/// <summary>
			/// Count ID.
			/// </summary>
			[DomainReloadExclude]
			public static readonly int Count = Shader.PropertyToID("_RippleCount");

			/// <summary>
			/// Ripple ID.
			/// </summary>
			[DomainReloadExclude]
			public static readonly int Ripple = Shader.PropertyToID("_Ripple");
		}

		[SerializeField]
		[FormerlySerializedAs("color")]
		Color startColor = Color.white;

		/// <summary>
		/// Start color of the ripple.
		/// </summary>
		public Color StartColor
		{
			get
			{
				return startColor;
			}

			set
			{
				startColor = value;
				UpdateMaterial();
			}
		}

		[SerializeField]
		Color endColor = Color.white;

		/// <summary>
		/// End color of the ripple.
		/// </summary>
		public Color EndColor
		{
			get
			{
				return endColor;
			}

			set
			{
				endColor = value;
				UpdateMaterial();
			}
		}

		[SerializeField]
		float speed = 0.5f;

		/// <summary>
		/// Ripple speed.
		/// </summary>
		public float Speed
		{
			get
			{
				return speed;
			}

			set
			{
				speed = value;
				UpdateMaterial();
			}
		}

		[SerializeField]
		[Range(0f, 1f)]
		float maxSize = 1f;

		/// <summary>
		/// Ripple size.
		/// </summary>
		public float MaxSize
		{
			get
			{
				return maxSize;
			}

			set
			{
				maxSize = value;
				UpdateMaterial();
			}
		}

		/// <summary>
		/// Ripples data.
		/// </summary>
		[NonSerialized]
		protected List<float> RipplesData;

		/// <summary>
		/// Remove oldest and dead ripples.
		/// </summary>
		protected void CleanRipples()
		{
			// remove oldest ripple
			if (RipplesData.Count > ((MaxRipples - 1) * FloatPerRipple))
			{
				RipplesData.RemoveRange(0, FloatPerRipple);
			}

			// remove dead ripples
			var died = UtilitiesTime.GetTime(false) - (MaxSize / Speed);
			var start = RipplesData.Count - FloatPerRipple;
			for (var i = start; i >= 0; i -= FloatPerRipple)
			{
				if (RipplesData[i + 2] < died)
				{
					RipplesData.RemoveRange(i, FloatPerRipple);
				}
			}
		}

		/// <summary>
		/// Add ripple.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		protected virtual void AddRipple(PointerEventData eventData)
		{
			CleanRipples();

			var size = RectTransform.rect.size;
			var aspect_ratio = size.x / size.y;

			var pivot = RectTransform.pivot;

			RectTransformUtility.ScreenPointToLocalPointInRectangle(RectTransform, eventData.position, eventData.pressEventCamera, out var current_point);
			current_point.x += size.x * pivot.x;
			current_point.y -= size.y * (1f - pivot.y);

			var center_x = current_point.x / size.x;
			var center_y = (1f + (current_point.y / size.y)) / aspect_ratio;

			RipplesData.Add(center_x);
			RipplesData.Add(center_y);
			RipplesData.Add(UtilitiesTime.GetTime(false));

			if (EffectMaterial != null)
			{
				EffectMaterial.SetInt(ShaderIDs.Count, RipplesData.Count / FloatPerRipple);
				EffectMaterial.SetFloatArray(ShaderIDs.Ripple, RipplesData);
				graphic.SetMaterialDirty();
			}
		}

		/// <summary>
		/// Process pointer click event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnPointerClick(PointerEventData eventData)
		{
			AddRipple(eventData);
		}

		/// <summary>
		/// Process pointer down event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnPointerDown(PointerEventData eventData)
		{
		}

		/// <summary>
		/// Process pointer up event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnPointerUp(PointerEventData eventData)
		{
		}

		/// <summary>
		/// Init material.
		/// </summary>
		protected override void InitMaterial()
		{
			var n = MaxRipples * FloatPerRipple;
			RipplesData ??= new List<float>(n);

			for (int i = RipplesData.Count; i < n; i++)
			{
				RipplesData.Add(-2);
			}

			base.InitMaterial();
		}

		/// <summary>
		/// Set material properties.
		/// </summary>
		protected override void SetMaterialProperties()
		{
			if (EffectMaterial != null)
			{
				EffectMaterial.SetColor(ShaderIDs.StartColor, startColor);
				EffectMaterial.SetColor(ShaderIDs.EndColor, endColor);
				EffectMaterial.SetFloat(ShaderIDs.Speed, speed);
				EffectMaterial.SetFloat(ShaderIDs.MaxSize, maxSize);
				EffectMaterial.SetInt(ShaderIDs.Count, RipplesData.Count / FloatPerRipple);
				EffectMaterial.SetFloatArray(ShaderIDs.Ripple, RipplesData);
			}
		}
	}
}