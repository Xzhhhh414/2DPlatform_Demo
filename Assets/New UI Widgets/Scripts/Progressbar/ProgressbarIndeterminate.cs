namespace UIWidgets
{
	using System.Collections;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// ProgressbarIndeterminate.
	/// </summary>
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/widgets/misc/progressbar-indeterminate.html")]
	public class ProgressbarIndeterminate : MonoBehaviour, IStylable
	{
		/// <summary>
		/// Progress bar animation direction.
		/// </summary>
		[SerializeField]
		public ProgressbarDirection Direction = ProgressbarDirection.Horizontal;

		/// <summary>
		/// The indeterminate bar.
		/// Use texture type "texture" and set wrap mode = repeat;
		/// </summary>
		[SerializeField]
		public RawImage Bar;

		/// <summary>
		/// Border.
		/// </summary>
		[SerializeField]
		public Image Border;

		/// <summary>
		/// Mask.
		/// </summary>
		[SerializeField]
		public Image Mask;

		/// <summary>
		/// The speed.
		/// For Indeterminate speed of changing uvRect coordinates.
		/// </summary>
		[SerializeField]
		public float Speed = 0.1f;

		/// <summary>
		/// The unscaled time.
		/// </summary>
		[SerializeField]
		public bool UnscaledTime = false;

		[SerializeField]
		bool correctUVRect = true;

		/// <summary>
		/// Correct UV rect of the Bar.
		/// </summary>
		public bool CorrectUVRect
		{
			get
			{
				return correctUVRect;
			}

			set
			{
				correctUVRect = value;
				if (correctUVRect)
				{
					UpdateUVRect();
				}
			}
		}

		/// <summary>
		/// Process the start event.
		/// </summary>
		protected virtual void Start()
		{
			UpdateUVRect();
		}

		/// <summary>
		/// Update UV rect.
		/// </summary>
		protected virtual void UpdateUVRect()
		{
			if ((Bar == null) || (Bar.texture == null))
			{
				return;
			}

			var t_width = Bar.texture.width;
			var t_height = Bar.texture.height;

			Bar.TryGetComponent<RectTransform>(out var rt);
			var rt_size = rt.rect.size;
			var rect = Bar.uvRect;
			rect.width = rt_size.x / t_width;
			rect.height = rt_size.y / t_height;
			Bar.uvRect = rect;
		}

		/// <summary>
		/// Gets a value indicating whether this instance is animation run.
		/// </summary>
		/// <value><c>true</c> if this instance is animation run; otherwise, <c>false</c>.</value>
		public bool IsAnimationRunning
		{
			get;
			protected set;
		}

		IEnumerator currentAnimation;

		/// <summary>
		/// Animate the progress bar to specified targetValue.
		/// </summary>
		public void Animate()
		{
			if (currentAnimation != null)
			{
				StopCoroutine(currentAnimation);
			}

			currentAnimation = IndeterminateAnimation();

			IsAnimationRunning = true;
			StartCoroutine(currentAnimation);
		}

		/// <summary>
		/// Stop animation.
		/// </summary>
		public void Stop()
		{
			if (IsAnimationRunning)
			{
				StopCoroutine(currentAnimation);
				IsAnimationRunning = false;
			}
		}

		IEnumerator IndeterminateAnimation()
		{
			while (true)
			{
				var r = Bar.uvRect;
				var position = UtilitiesTime.GetDeltaTime(UnscaledTime) * -Speed;
				if (Direction == ProgressbarDirection.Horizontal)
				{
					r.x = (r.x + position) % 1;
				}
				else
				{
					r.y = (r.y + position) % 1;
				}

				Bar.uvRect = r;
				yield return null;
			}
		}

		#region IStylable implementation

		/// <inheritdoc/>
		public virtual bool SetStyle(Style style)
		{
			style.ProgressbarIndeterminate.Texture.ApplyTo(Bar);

			style.ProgressbarIndeterminate.Mask.ApplyTo(Mask);
			style.ProgressbarIndeterminate.Border.ApplyTo(Border);

			return true;
		}

		/// <inheritdoc/>
		public virtual bool GetStyle(Style style)
		{
			style.ProgressbarIndeterminate.Texture.GetFrom(Bar);

			style.ProgressbarIndeterminate.Mask.GetFrom(Mask);
			style.ProgressbarIndeterminate.Border.GetFrom(Border);

			return true;
		}
		#endregion

#if UNITY_EDITOR
		/// <summary>
		/// Process the validate event.
		/// </summary>
		protected virtual void OnValidate()
		{
			if (correctUVRect)
			{
				UpdateUVRect();
			}
		}
#endif
	}
}