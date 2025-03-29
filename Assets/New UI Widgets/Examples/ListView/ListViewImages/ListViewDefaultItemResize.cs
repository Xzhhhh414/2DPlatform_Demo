namespace UIWidgets.Examples
{
	using UnityEngine;

	/// <summary>
	/// Resize ListView.DefaultItem when ListView size changed + scale images of items relative position to center.
	/// </summary>
	[RequireComponent(typeof(ResizeListener))]
	[RequireComponent(typeof(RectTransform))]
	[RequireComponent(typeof(ListViewImages))]
	public class ListViewDefaultItemResize : MonoBehaviourInitiable
	{
		/// <summary>
		/// ListView size.
		/// </summary>
		[SerializeField]
		protected Vector2 ListViewSize;

		/// <summary>
		/// DefaultItem size.
		/// </summary>
		[SerializeField]
		protected Vector2 DefaultItemSize;

		/// <summary>
		/// Paginator.
		/// </summary>
		[SerializeField]
		protected ListViewPaginator Paginator;

		/// <summary>
		/// Scale curve.
		/// </summary>
		[SerializeField]
		public AnimationCurve ScaleCurve = AnimationCurve.EaseInOut(0f, 0.8f, 1f, 1f);

		/// <summary>
		/// ListView.
		/// </summary>
		protected ListViewImages ListView;

		/// <summary>
		/// RectTransform.
		/// </summary>
		protected RectTransform RectTransform;

		/// <summary>
		/// Resize listener.
		/// </summary>
		protected ResizeListener ResizeListener;

		/// <summary>
		/// Padding from left and right side for the DefaultItem.
		/// </summary>
		[SerializeField]
		protected float DefaultItemPadding = 30f;

		/// <inheritdoc/>
		protected override void InitOnce()
		{
			base.InitOnce();

			TryGetComponent(out ListView);
			TryGetComponent(out RectTransform);
			TryGetComponent(out ResizeListener);
			ResizeListener.OnResizeNextFrame.AddListener(ResizeDefaultItem);

			ListView.GetScrollRect().onValueChanged.AddListener(ScaleImages);
		}

		/// <summary>
		/// Process the destroy event.
		/// </summary>
		protected virtual void OnDestroy()
		{
			if (ResizeListener != null)
			{
				ResizeListener.OnResizeNextFrame.RemoveListener(ResizeDefaultItem);
			}

			if (ListView != null)
			{
				var sr = ListView.GetScrollRect();
				if (sr != null)
				{
					sr.onValueChanged.RemoveListener(ScaleImages);
				}
			}
		}

		/// <summary>
		/// Resize DefaultItem.
		/// </summary>
		protected virtual void ResizeDefaultItem()
		{
			var padding = 30f; // from left and right side
			var width = RectTransform.rect.width - (padding * 2);
			var scale = width / DefaultItemSize.x; // scale to preserve aspect ratio
			var size = new Vector2(width, DefaultItemSize.y * scale);

			ListView.ChangeDefaultItemSize(size);
		}

		/// <summary>
		/// Scale images.
		/// </summary>
		/// <param name="unused">Unused.</param>
		protected virtual void ScaleImages(Vector2 unused)
		{
			if (Paginator == null)
			{
				return;
			}

			// works only if index == page
			var current_pos = ListView.GetScrollPosition();
			var item_size = ListView.IsHorizontal() ? ListView.GetDefaultItemWidth() : ListView.GetDefaultItemHeight();
			foreach (var instance in ListView.GetComponentsEnumerator(PoolEnumeratorMode.Active))
			{
				var pos = Paginator.Page2Position(instance.Index);
				var delta = Mathf.Abs(pos - current_pos);
				var t = Mathf.Clamp01(delta / item_size);

				instance.SpriteImage.transform.localScale = Vector3.one * ScaleCurve.Evaluate(1f - t);
			}
		}

#if UNITY_EDITOR
		/// <summary>
		/// Validate this instance.
		/// </summary>
		protected virtual void OnValidate()
		{
			if (Application.isPlaying)
			{
				return;
			}

			if (TryGetComponent<ListViewImages>(out var list_view))
			{
				var size = (list_view.transform as RectTransform).rect.size;
				var valid = !(Mathf.Approximately(size.x, 0f) || Mathf.Approximately(size.y, 0f));
				if (valid)
				{
					ListViewSize = size;
					DefaultItemSize = list_view.GetDefaultItemSize();
				}
			}
		}
#endif
	}
}