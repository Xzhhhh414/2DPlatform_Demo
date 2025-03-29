namespace UIWidgets
{
	using UnityEngine;
	using UnityEngine.EventSystems;
	using UnityEngine.UI;

	/// <summary>
	/// Toggle tabs using mouse scroll.
	/// </summary>
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/widgets/containers/tabs.html")]
	[RequireComponent(typeof(Graphic))]
	public class TabsScroll : MonoBehaviour, IScrollHandler
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
			/// Toggle to previous tab on scroll up.
			/// </summary>
			UpPrevious = 1,

			/// <summary>
			/// Toggle to next tab on scroll up.
			/// </summary>
			UpNext = 2,
		}

		/// <summary>
		/// Tabs.
		/// </summary>
		[SerializeField]
		protected TabsBase Tabs;

		/// <summary>
		/// Scroll mode.
		/// </summary>
		[SerializeField]
		public ScrollModes ScrollMode = ScrollModes.UpNext;

		/// <summary>
		/// Looped scroll.
		/// </summary>
		[SerializeField]
		public bool Loop = true;

		/// <summary>
		/// Process the scroll event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnScroll(PointerEventData eventData)
		{
			switch (ScrollMode)
			{
				case ScrollModes.UpNext:
					if (eventData.scrollDelta.y > 0)
					{
						Tabs.PreviousTab(Loop);
					}
					else
					{
						Tabs.NextTab(Loop);
					}

					break;
				case ScrollModes.UpPrevious:
					if (eventData.scrollDelta.y > 0)
					{
						Tabs.NextTab(Loop);
					}
					else
					{
						Tabs.PreviousTab(Loop);
					}

					break;
				default:
					break;
			}
		}
	}
}