namespace UIWidgets
{
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// ScrollRect content resize.
	/// </summary>
	[RequireComponent(typeof(ScrollRect))]
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/components/scrollrect/scrollrect-content-resize.html")]
	public class ScrollRectContentResize : MonoBehaviour
	{
		ResizeListener resizeListener;

		ScrollRect scrollRect;

		/// <summary>
		/// ScrollRect.
		/// </summary>
		protected ScrollRect ScrollRect
		{
			get
			{
				if (scrollRect == null)
				{
					TryGetComponent(out scrollRect);
				}

				return scrollRect;
			}
		}

		/// <summary>
		/// Process start event.
		/// </summary>
		protected virtual void Start()
		{
			if (resizeListener == null)
			{
				resizeListener = Utilities.RequireComponent<ResizeListener>(this);
			}

			resizeListener.OnResizeNextFrame.AddListener(Resize);

			Resize();
		}

		/// <summary>
		/// Process destroy event.
		/// </summary>
		protected virtual void OnDestroy()
		{
			if (resizeListener != null)
			{
				resizeListener.OnResizeNextFrame.RemoveListener(Resize);
			}
		}

		void Resize()
		{
			var size = (ScrollRect.transform as RectTransform).rect.size;

			var content = ScrollRect.content;
			for (int i = 0; i < content.childCount; i++)
			{
				var rt = content.GetChild(i) as RectTransform;
				var current_size = rt.rect.size;
				if (ScrollRect.horizontal && !Mathf.Approximately(current_size.x, size.x))
				{
					rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
				}

				if (ScrollRect.vertical && !Mathf.Approximately(current_size.y, size.y))
				{
					rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
				}
			}
		}
	}
}