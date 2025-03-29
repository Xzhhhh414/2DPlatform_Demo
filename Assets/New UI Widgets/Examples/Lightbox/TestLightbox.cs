namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.Serialization;

	/// <summary>
	/// Test Lightbox.
	/// </summary>
	public class TestLightbox : MonoBehaviour
	{
		/// <summary>
		/// Lightbox to display image.
		/// </summary>
		[SerializeField]
		public Lightbox Lightbox;

		[SerializeField]
		[FormerlySerializedAs("Image")]
		Sprite image;

		/// <summary>
		/// Image to display.
		/// </summary>
		public Sprite Image
		{
			get
			{
				return image;
			}

			set
			{
				image = value;
			}
		}

		/// <summary>
		/// Show lightbox with image.
		/// </summary>
		public void ShowLightbox()
		{
			Lightbox.Show(Image);
		}
	}
}