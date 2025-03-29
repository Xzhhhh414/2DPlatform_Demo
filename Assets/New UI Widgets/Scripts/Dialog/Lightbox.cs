namespace UIWidgets
{
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Lightbox.
	/// Display modal image.
	/// </summary>
	[RequireComponent(typeof(Image))]
	[RequireComponent(typeof(RectTransform))]
	[DisallowMultipleComponent]
	[AddComponentMenu("UI/New UI Widgets/Lightbox")]
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/components/lightbox.html")]
	public class Lightbox : MonoBehaviour
	{
		/// <summary>
		/// The modal ID.
		/// </summary>
		protected InstanceID? ModalKey;

		/// <summary>
		/// Lightbox hierarchy position.
		/// </summary>
		protected HierarchyPosition Position;

		/// <summary>
		/// Canvas resize subscription.
		/// </summary>
		protected Subscription CanvasResize;

		/// <summary>
		/// Hide on modal click.
		/// </summary>
		[SerializeField]
		public bool HideOnModalClick = true;

		/// <summary>
		/// Display specified image.
		/// </summary>
		/// <param name="sprite">Image to display.</param>
		/// <param name="modalSprite">Modal background sprite.</param>
		/// <param name="modalColor">Modal background color.</param>
		/// <param name="canvas">Canvas.</param>
		public virtual void Show(
			Sprite sprite,
			Sprite modalSprite = null,
			Color? modalColor = null,
			Canvas canvas = null)
		{
			TryGetComponent<Image>(out var image);
			image.sprite = sprite;
			image.preserveAspect = true;

			if (modalColor == null)
			{
				modalColor = new Color(0, 0, 0, 0.8f);
			}

			var parent = (canvas != null) ? canvas.transform as RectTransform : UtilitiesUI.FindTopmostCanvas(gameObject.transform);
			ModalKey = ModalHelper.Open(this, modalSprite, modalColor, ProcessModalClick, parent);
			Position = HierarchyPosition.SetParent(transform, parent);

			var resize = Utilities.RequireComponent<ResizeListener>(parent);
			CanvasResize.Clear();
			CanvasResize = new Subscription(resize.OnResize, RefreshPosition);

			gameObject.SetActive(true);
		}

		/// <summary>
		/// Refresh position.
		/// </summary>
		protected virtual void RefreshPosition() => Position.Refresh();

		/// <summary>
		/// Process modal click.
		/// </summary>
		protected void ProcessModalClick()
		{
			if (HideOnModalClick)
			{
				Close();
			}
		}

		/// <summary>
		/// Close lightbox.
		/// </summary>
		public virtual void Close()
		{
			gameObject.SetActive(false);

			ModalHelper.Close(ref ModalKey);
			Position.Restore();
			CanvasResize.Clear();
		}

		/// <summary>
		/// Process the destroy event.
		/// </summary>
		protected virtual void OnDestroy()
		{
			CanvasResize.Clear();
			Position.ParentDestroyed();
		}
	}
}