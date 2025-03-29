namespace UIWidgets
{
	using UnityEngine;

	/// <summary>
	/// Auto-resize DefaultItem of the ListView.
	/// </summary>
	[RequireComponent(typeof(ListViewBase))]
	[RequireComponent(typeof(RectTransform))]
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/components/collections/listview-defaultitem-autoresize.html")]
	public class ListViewDefaultItemAutoResize : MonoBehaviourInitiable
	{
		/// <summary>
		/// Size difference.
		/// </summary>
		[SerializeField]
		public Vector2 SizeDifference = new Vector2(float.MinValue, float.MinValue);

		/// <summary>
		/// ListView.
		/// </summary>
		protected ListViewBase ListView;

		/// <summary>
		/// Viewport size.
		/// </summary>
		protected Vector2 ViewportSize => ListView.GetScrollRect().viewport.rect.size;

		/// <summary>
		/// Resize listener.
		/// </summary>
		protected ResizeListener ResizeListener;

		/// <summary>
		/// Axis.
		/// </summary>
		protected int Axis => ListView.IsHorizontal() ? 0 : 1;

		/// <inheritdoc/>
		protected override void InitOnce()
		{
			base.InitOnce();

			TryGetComponent(out ListView);

			var viewport = ListView.GetScrollRect().viewport;
			ResizeListener = Utilities.RequireComponent<ResizeListener>(viewport);
			ResizeListener.OnResizeNextFrame.AddListener(ResizeItem);
		}

		/// <summary>
		/// Process the destroy event.
		/// </summary>
		protected virtual void OnDestroy()
		{
			if (ResizeListener != null)
			{
				ResizeListener.OnResizeNextFrame.RemoveListener(ResizeItem);
			}
		}

		/// <summary>
		/// Resize DefaultItem.
		/// </summary>
		protected virtual void ResizeItem()
		{
			ListView.ChangeDefaultItemSize(ViewportSize - SizeDifference);
		}

#if UNITY_EDITOR
		/// <summary>
		/// Validate this instance.
		/// </summary>
		protected virtual void OnValidate()
		{
			if ((SizeDifference.x == float.MinValue) || (SizeDifference.y == float.MinValue))
			{
				TryGetComponent(out ListView);

				SizeDifference = ViewportSize - ListView.GetDefaultItemSize();
			}
		}

		/// <summary>
		/// Reset this instance.
		/// </summary>
		protected virtual void Reset() => OnValidate();
#endif
	}
}