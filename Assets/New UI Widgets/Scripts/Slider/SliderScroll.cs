namespace UIWidgets
{
	using UIWidgets.Attributes;
	using UnityEngine;
	using UnityEngine.EventSystems;
	using UnityEngine.UI;

	/// <summary>
	/// Changes Slider value on the mouse scroll.
	/// </summary>
	[RequireComponent(typeof(Slider))]
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/components/slider-scroll.html")]
	[DataBindSupport]
	public class SliderScroll : MonoBehaviourConditional,
		IScrollHandler
	{
		/// <summary>
		/// Scroll modes.
		/// </summary>
		public enum ScrollModes
		{
			/// <summary>
			/// Ignore.
			/// </summary>
			Ignore = 0,

			/// <summary>
			/// Increase on scroll up.
			/// </summary>
			UpIncrease = 1,

			/// <summary>
			/// Increase on scroll down.
			/// </summary>
			UpDecrease = 2,
		}

		/// <summary>
		/// Scroll mode.
		/// </summary>
		[SerializeField]
		[DataBindField]
		public ScrollModes ScrollMode = ScrollModes.UpIncrease;

		/// <summary>
		/// Scroll step.
		/// </summary>
		[SerializeField]
		[DataBindField]
		public float Step = 0.1f;

		Slider slider;

		/// <summary>
		/// Slider.
		/// </summary>
		public Slider Slider
		{
			get
			{
				if (slider == null)
				{
					TryGetComponent(out slider);
				}

				return slider;
			}
		}

		/// <summary>
		/// Process the scroll event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnScroll(PointerEventData eventData)
		{
			if ((Slider == null) || !Slider.IsInteractable())
			{
				return;
			}

			switch (ScrollMode)
			{
				case ScrollModes.UpIncrease:
					Slider.value += Step * Mathf.Sign(eventData.scrollDelta.y);
					break;
				case ScrollModes.UpDecrease:
					Slider.value -= Step * Mathf.Sign(eventData.scrollDelta.y);
					break;
				default:
					break;
			}
		}
	}
}