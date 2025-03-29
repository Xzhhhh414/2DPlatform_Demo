namespace UIWidgets
{
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.EventSystems;
	using UnityEngine.UI;

	/// <summary>
	/// ScrollRectPage.
	/// </summary>
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/widgets/controls/paginator.html")]
	public class ScrollRectPage : MonoBehaviourConditional, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, ISubmitHandler
	{
		/// <summary>
		/// The page number.
		/// </summary>
		[HideInInspector]
		public int Page;

		RectTransform rectTransform;

		/// <summary>
		/// RectTransform.
		/// </summary>
		public RectTransform RectTransform
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

		/// <summary>
		/// OnPageSelect event.
		/// </summary>
		[SerializeField]
		public ScrollRectPageSelect OnPageSelect = new ScrollRectPageSelect();

		/// <summary>
		/// Sets the page number.
		/// </summary>
		/// <param name="page">Page.</param>
		public virtual void SetPage(int page)
		{
			Page = page;
		}

		/// <summary>
		/// Called by a BaseInputModule when an OnPointerClick event occurs.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnPointerClick(PointerEventData eventData)
		{
			OnPageSelect.Invoke(Page);
		}

		/// <summary>
		/// Called by a BaseInputModule when an OnPointerDown event occurs.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnPointerDown(PointerEventData eventData)
		{
		}

		/// <summary>
		/// Called by a BaseInputModule when an OnPointerUp event occurs.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnPointerUp(PointerEventData eventData)
		{
		}

		/// <summary>
		/// Called by a BaseInputModule when an OnSubmit event occurs.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnSubmit(BaseEventData eventData)
		{
			OnPageSelect.Invoke(Page);
		}

		/// <summary>
		/// Set the specified style.
		/// </summary>
		/// <param name="styleText">Style for the text.</param>
		/// <param name="style">Full style data.</param>
		public virtual void SetStyle(StyleText styleText, Style style)
		{
			// do nothing
		}

		/// <summary>
		/// Set style options from widget properties.
		/// </summary>
		/// <param name="styleText">Style for the text.</param>
		/// <param name="style">Full style data.</param>
		public virtual void GetStyle(StyleText styleText, Style style)
		{
			// do nothing
		}

		/// <summary>
		/// Toggle interactable or GameObject.active.
		/// </summary>
		[SerializeField]
		[Tooltip("Toggle interactable or GameObject.active.")]
		public bool ToggleInteractable;

		/// <summary>
		/// Selectable.
		/// </summary>
		protected Selectable Selectable;

		/// <inheritdoc/>
		protected override void InitOnce()
		{
			base.InitOnce();

			TryGetComponent(out Selectable);
		}

		/// <summary>
		/// Set state.
		/// </summary>
		/// <param name="active">Active.</param>
		public virtual void SetState(bool active)
		{
			Init();

			if (ToggleInteractable && (Selectable != null))
			{
				Selectable.interactable = active;
			}
			else
			{
				gameObject.SetActive(active);
			}
		}
	}
}