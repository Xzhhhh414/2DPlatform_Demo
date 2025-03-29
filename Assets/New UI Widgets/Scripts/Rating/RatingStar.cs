namespace UIWidgets
{
	using UnityEngine;
	using UnityEngine.Serialization;
	using UnityEngine.UI;

	/// <summary>
	/// Rating star.
	/// </summary>
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/widgets/input/rating.html")]
	public class RatingStar : MonoBehaviourInitiable
	{
		/// <summary>
		/// Owner.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		public Rating Owner;

		/// <summary>
		/// Rating.
		/// </summary>
		public int Rating
		{
			get;
			set;
		}

		/// <summary>
		/// Interactable.
		/// </summary>
		public virtual bool Interactable
		{
			get => Button.interactable;

			set => Button.interactable = value;
		}

		/// <summary>
		/// RectTransform.
		/// </summary>
		public RectTransform RectTransform
		{
			get;
			protected set;
		}

		/// <summary>
		/// Graphic.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("Graphic")]
		protected Graphic graphic;

		/// <summary>
		/// Graphic.
		/// </summary>
		public Graphic Graphic => graphic;

		/// <summary>
		/// Button.
		/// </summary>
		[SerializeField]
		protected Button Button;

		/// <inheritdoc/>
		protected override void InitOnce()
		{
			base.InitOnce();

			RectTransform = transform as RectTransform;
			Button.onClick.AddListener(ProcessClick);
		}

		/// <summary>
		/// Process the destroy event.
		/// </summary>
		protected virtual void OnDestroy()
		{
			Button.onClick.RemoveListener(ProcessClick);
		}

		/// <summary>
		/// Process click.
		/// </summary>
		protected virtual void ProcessClick()
		{
			Owner.Value = Rating;
		}

		/// <summary>
		/// Update star color.
		/// </summary>
		/// <param name="color">Color.</param>
		public virtual void Coloring(Color color)
		{
			if (Graphic != null)
			{
				Graphic.color = color;
			}
		}

#if UNITY_EDITOR
		/// <summary>
		/// Validate this instance.
		/// </summary>
		protected virtual void OnValidate()
		{
			if (graphic == null)
			{
				graphic = GetComponentInChildren<Graphic>();
			}

			if (Button == null)
			{
				Button = GetComponentInChildren<Button>();
			}
		}
#endif
	}
}