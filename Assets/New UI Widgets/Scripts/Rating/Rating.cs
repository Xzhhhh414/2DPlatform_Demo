namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using UIThemes;
	using UIWidgets.Attributes;
	using UnityEngine;
	using UnityEngine.Events;

	/// <summary>
	/// Rating.
	/// </summary>
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/widgets/input/rating.html")]
	[DataBindSupport]
	public class Rating : UIBehaviourInteractable, ITargetOwner
	{
		/// <summary>
		/// Rating event.
		/// </summary>
		[Serializable]
		public class RatingEvent : UnityEvent<int>
		{
		}

		/// <summary>
		/// Color lerp mode.
		/// </summary>
		public enum ColorLerpMode
		{
			/// <summary>
			/// RGB lerp.
			/// </summary>
			RGB = 0,

			/// <summary>
			/// HSV lerp.
			/// Prevent dirty colors unlike RGB.
			/// </summary>
			HSV = 1,
		}

		[SerializeField]
		int value = 1;

		/// <summary>
		/// Value.
		/// </summary>
		[DataBindField]
		public int Value
		{
			get => value;

			set
			{
				var v = Mathf.Clamp(value, 0, valueMax);
				if (v != this.value)
				{
					this.value = v;
					ValueChanged();
				}
			}
		}

		[SerializeField]
		int valueMax = 5;

		/// <summary>
		/// Maximum value.
		/// </summary>
		[DataBindField]
		public int ValueMax
		{
			get => valueMax;

			set
			{
				valueMax = value > 2 ? value : 2;
				Value = this.value;
			}
		}

		/// <summary>
		/// Empty star.
		/// </summary>
		[SerializeField]
		protected RatingStar StarEmpty;

		/// <summary>
		/// Full star.
		/// </summary>
		[SerializeField]
		protected RatingStar StarFull;

		[Header("Colors")]
		[SerializeField]
		Color colorMin = Color.red;

		/// <summary>
		/// Color when Value = 1.
		/// </summary>
		[DataBindField]
		public Color ColorMin
		{
			get => colorMin;

			set
			{
				if (colorMin != value)
				{
					colorMin = value;
					Coloring();
				}
			}
		}

		[SerializeField]
		Color colorMax = Color.green;

		/// <summary>
		/// Color when Value = ValueMax.
		/// </summary>
		[DataBindField]
		public Color ColorMax
		{
			get => colorMax;

			set
			{
				if (colorMax != value)
				{
					colorMax = value;
					Coloring();
				}
			}
		}

		[SerializeField]
		ColorLerpMode lerpMode = ColorLerpMode.HSV;

		/// <summary>
		/// Color lerp mode.
		/// </summary>
		[DataBindField]
		public ColorLerpMode LerpMode
		{
			get => lerpMode;

			set
			{
				if (lerpMode != value)
				{
					lerpMode = value;
					Coloring();
				}
			}
		}

		Func<int, Color> value2Color;

		/// <summary>
		/// Convert value to color.
		/// </summary>
		[DataBindField]
		public Func<int, Color> Value2Color
		{
			get
			{
				value2Color ??= DefaultValue2Color;

				return value2Color;
			}

			set
			{
				if (value2Color == value)
				{
					return;
				}

				value2Color = value;

				if (value2Color != null)
				{
					Coloring();
				}
			}
		}

		/// <summary>
		/// Value changed event.
		/// </summary>
		[SerializeField]
		[DataBindEvent(nameof(Value))]
		public RatingEvent OnChange = new RatingEvent();

		/// <summary>
		/// Default converter for value to color.
		/// </summary>
		/// <param name="value">Value.</param>
		/// <returns>Color.</returns>
		public Color DefaultValue2Color(int value)
		{
			var t = (float)Mathf.Max(value - 1, 0) / (ValueMax - 1);
			return LerpMode == ColorLerpMode.RGB
				? Color.Lerp(ColorMin, ColorMax, t)
				: ColorHSV.Lerp(new ColorHSV(ColorMin), new ColorHSV(ColorMax), t);
		}

		[NonSerialized]
		ListComponentPool<RatingStar> starsPoolEmpty;

		/// <summary>
		/// Pool for empty stars.
		/// </summary>
		protected ListComponentPool<RatingStar> StarsPoolEmpty
		{
			get
			{
				if ((starsPoolEmpty == null) || (starsPoolEmpty.Template == null))
				{
					starsPoolEmpty = new ListComponentPool<RatingStar>(StarEmpty, StarsPoolEmptyInstances, StarsPoolEmptyCache, transform as RectTransform);
				}

				return starsPoolEmpty;
			}
		}

		/// <summary>
		/// Cache for StarsPoolEmpty.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		protected List<RatingStar> StarsPoolEmptyCache = new List<RatingStar>();

		/// <summary>
		/// Instances for StarsPoolEmpty.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		protected List<RatingStar> StarsPoolEmptyInstances = new List<RatingStar>();

		[NonSerialized]
		ListComponentPool<RatingStar> starsPoolFull;

		/// <summary>
		/// Pool for full stars.
		/// </summary>
		protected ListComponentPool<RatingStar> StarsPoolFull
		{
			get
			{
				if ((starsPoolFull == null) || (starsPoolFull.Template == null))
				{
					starsPoolFull = new ListComponentPool<RatingStar>(StarFull, StarsPoolFullCache, StarsPoolFullActive, transform as RectTransform);
				}

				return starsPoolFull;
			}
		}

		/// <summary>
		/// Cache for StarsPoolFull.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		protected List<RatingStar> StarsPoolFullCache = new List<RatingStar>();

		/// <summary>
		/// Instances for StarsPoolFull.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		protected List<RatingStar> StarsPoolFullActive = new List<RatingStar>();

		/// <inheritdoc/>
		protected override void InitOnce()
		{
			base.InitOnce();

			SetTargetOwner();

			foreach (var star in StarsPoolEmpty.GetEnumerator(PoolEnumeratorMode.All))
			{
				OnStarCreate(star);
			}

			StarsPoolEmpty.OnCreate = OnStarCreate;
			StarsPoolEmpty.OnDestroy = OnStarDestroy;

			foreach (var star in StarsPoolFull.GetEnumerator(PoolEnumeratorMode.All))
			{
				OnStarCreate(star);
			}

			StarsPoolFull.OnCreate = OnStarCreate;
			StarsPoolFull.OnDestroy = OnStarDestroy;

			UpdateStars();
			Coloring();
			InteractableChanged();
		}

		/// <inheritdoc/>
		protected override void OnInteractableChange(bool interactableState)
		{
			foreach (var s in StarsPoolEmpty)
			{
				s.Interactable = interactableState;
			}

			foreach (var s in StarsPoolFull)
			{
				s.Interactable = interactableState;
			}
		}

		/// <summary>
		/// Set theme target owner.
		/// </summary>
		public virtual void SetTargetOwner()
		{
			UIThemes.Utilities.SetTargetOwner(typeof(Color), StarEmpty.Graphic, nameof(StarEmpty.Graphic.color), this);
			UIThemes.Utilities.SetTargetOwner(typeof(Color), StarFull.Graphic, nameof(StarFull.Graphic.color), this);
		}

		/// <summary>
		/// Process created star.
		/// </summary>
		/// <param name="star">Star.</param>
		protected virtual void OnStarCreate(RatingStar star)
		{
			star.Init();
			star.Owner = this;
			UIThemes.Utilities.SetTargetOwner(typeof(Color), star.Graphic, nameof(star.Graphic.color), this);
		}

		/// <summary>
		/// Process destroyed star.
		/// </summary>
		/// <param name="star">Star.</param>
		protected virtual void OnStarDestroy(RatingStar star)
		{
			star.Owner = null;
			UIThemes.Utilities.SetTargetOwner(typeof(Color), star.Graphic, nameof(star.Graphic.color), null);
		}

		/// <summary>
		/// Process the destroy event.
		/// </summary>
		protected override void OnDestroy()
		{
			StarsPoolEmpty.Clear();
			StarsPoolEmpty.OnCreate = null;
			StarsPoolEmpty.OnDestroy = null;
			starsPoolEmpty = null;

			StarsPoolFull.Clear();
			StarsPoolFull.OnCreate = null;
			StarsPoolFull.OnDestroy = null;
			starsPoolFull = null;

			value2Color = null;

			base.OnDestroy();
		}

		/// <summary>
		/// Process value changes.
		/// </summary>
		protected virtual void ValueChanged()
		{
			UpdateStars();
			Coloring();
			OnChange.Invoke(Value);

			UtilitiesUI.Select(StarsPoolFull[Value - 1].gameObject);
		}

		/// <summary>
		/// Update stars instances.
		/// </summary>
		protected virtual void UpdateStars()
		{
			StarsPoolFull.Require(Value);
			for (var i = 0; i < StarsPoolFull.Count; i++)
			{
				var s = StarsPoolFull[i];
				s.Rating = i + 1;
				s.RectTransform.SetAsLastSibling();
			}

			StarsPoolEmpty.Require(ValueMax - Value);
			for (var i = 0; i < StarsPoolEmpty.Count; i++)
			{
				var s = StarsPoolEmpty[i];
				s.Rating = Value + i + 1;
				s.RectTransform.SetAsLastSibling();
			}
		}

		/// <summary>
		/// Update stars color.
		/// </summary>
		protected virtual void Coloring()
		{
			var color = Value2Color(Value);
			for (int i = 0; i < StarsPoolFull.Count; i++)
			{
				StarsPoolFull[i].Coloring(color);
			}
		}

#if UNITY_EDITOR

		/// <summary>
		/// Validate this instance.
		/// </summary>
		protected override void OnValidate()
		{
			if (valueMax < 2)
			{
				valueMax = 2;
			}

			value = Mathf.Clamp(value, 0, valueMax);
		}
#endif
	}
}