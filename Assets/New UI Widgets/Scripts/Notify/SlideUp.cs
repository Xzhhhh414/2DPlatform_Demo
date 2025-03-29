namespace UIWidgets
{
	using System.Collections;
	using UnityEngine;

	/// <summary>
	/// Slide up. Helper component for Notify.
	/// </summary>
	[RequireComponent(typeof(RectTransform))]
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/widgets/dialogs/notify.html")]
	public class SlideUp : MonoBehaviour
	{
		/// <summary>
		/// Animate with unscaled time.
		/// </summary>
		public bool UnscaledTime;

		RectTransform rect;

		/// <summary>
		/// Awake this instance.
		/// </summary>
		protected virtual void Awake()
		{
			rect = transform as RectTransform;
		}

		/// <summary>
		/// Start animation.
		/// </summary>
		public void Run()
		{
			StartCoroutine(StartAnimation());
		}

		/// <summary>
		/// Start animation.
		/// </summary>
		/// <returns>Animation coroutine.</returns>
		protected virtual IEnumerator StartAnimation()
		{
			yield return StartCoroutine(AnimationCollapse());

			gameObject.SetActive(false);
		}

		/// <summary>
		/// Handle disable event.
		/// </summary>
		protected virtual void OnDisable()
		{
			NotificationBase.FreeSlide(rect);
		}

		/// <summary>
		/// Collapse animation.
		/// </summary>
		/// <returns>Animation coroutine.</returns>
		protected virtual IEnumerator AnimationCollapse()
		{
			var max_height = rect.rect.height;
			var speed = 200f; // pixels per second

			var duration = max_height / speed;
			var time = 0f;

			do
			{
				var height = Mathf.Lerp(max_height, 0, time / duration);

				rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
				yield return null;

				time += UtilitiesTime.GetDeltaTime(UnscaledTime);
			}
			while (time < duration);

			// return height back for future use
			rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, max_height);
		}

		/// <summary>
		/// Get time.
		/// </summary>
		/// <returns>Time.</returns>
		[System.Obsolete("Use Utilities.GetTime(UnscaledTime).")]
		protected virtual float GetTime()
		{
			return Utilities.GetTime(UnscaledTime);
		}
	}
}