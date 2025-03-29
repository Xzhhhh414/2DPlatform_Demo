namespace UIWidgets
{
	using System.Collections.Generic;
	using UIWidgets.Pool;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.EventSystems;

	/// <summary>
	/// ListViewBase.
	/// You can use it for creating custom ListViews.
	/// </summary>
	public abstract partial class ListViewBase : UIBehaviourInteractable,
			ISelectHandler, IDeselectHandler,
			ISubmitHandler, ICancelHandler,
			IStylable, IUpgradeable
	{
		/// <summary>
		/// Find ListVew items under pointer.
		/// </summary>
		protected class ItemsUnderPointerFinder
		{
			readonly List<ListViewItem> items = new List<ListViewItem>();

			PointerEventData eventData;

			EventSystem eventSystem;

			int frame = -1;

			/// <summary>
			/// Reset this instance.
			/// </summary>
			public void Reset()
			{
				items.Clear();
				eventData = null;
				eventSystem = null;
				frame = -1;
			}

			/// <summary>
			/// Find items under pointer.
			/// </summary>
			/// <returns>Items enumerator.</returns>
			public List<ListViewItem>.Enumerator GetEnumerator()
			{
				Find();
				return items.GetEnumerator();
			}

			IReadOnlyList<ListViewItem> Find()
			{
				if (frame == Time.frameCount)
				{
					return items;
				}

				frame = Time.frameCount;
				items.Clear();

				if (!CompatibilityInput.MousePresent)
				{
					return items;
				}

				using var _ = ListPool<RaycastResult>.Get(out var raycasts);

				if (EventSystem.current != null)
				{
					var es = EventSystem.current;
					if ((eventData == null) || (eventSystem != es))
					{
						eventSystem = es;
						eventData = new PointerEventData(es);
					}

					eventData.position = CompatibilityInput.MousePosition;

					EventSystem.current.RaycastAll(eventData, raycasts);
				}

				foreach (var raycast in raycasts)
				{
					if (!raycast.isValid)
					{
						continue;
					}

					raycast.gameObject.TryGetComponent<ListViewItem>(out var item);
					if ((item != null) && (item.Owner != null))
					{
						items.Add(item);
					}
				}

				return items;
			}
		}
	}
}