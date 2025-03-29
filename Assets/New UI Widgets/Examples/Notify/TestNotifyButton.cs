namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// Test notification with buttons.
	/// </summary>
	public class TestNotifyButton : MonoBehaviour
	{
		/// <summary>
		/// Notification template.
		/// </summary>
		[SerializeField]
		protected Notify NotificationTemplate;

		/// <summary>
		/// Notification Queue template.
		/// </summary>
		[SerializeField]
		protected Notify NotificationQueueTemplate;

		/// <summary>
		/// Show simple notification.
		/// </summary>
		public void ShowSimple()
		{
			NotificationTemplate.Clone().Show(
				"Simple notification.",
				showAnimation: NotificationBase.FadeIn,
				hideAnimation: NotificationBase.FadeOut);
		}

		/// <summary>
		/// Show notification with auto hide.
		/// </summary>
		public void ShowAutoHide()
		{
			NotificationTemplate.Clone().Show(
				"This notification will closed after 3 seconds.",
				customHideDelay: 3f,
				showAnimation: NotificationBase.FadeIn,
				hideAnimation: NotificationBase.FadeOut);
		}

		/// <summary>
		/// Show notifications one-by-one.
		/// </summary>
		public void ShowQueue()
		{
			NotificationQueueTemplate.Clone().Show(
				"First Notification.",
				customHideDelay: 1f,
				showAnimation: NotificationBase.FadeIn,
				hideAnimation: NotificationBase.FadeOut,
				sequenceType: NotifySequence.Last);

			NotificationQueueTemplate.Clone().Show(
				"Second Notification.",
				customHideDelay: 1f,
				showAnimation: NotificationBase.FadeIn,
				hideAnimation: NotificationBase.FadeOut,
				sequenceType: NotifySequence.Last);

			NotificationQueueTemplate.Clone().Show(
				"Third Notification.",
				customHideDelay: 1f,
				showAnimation: NotificationBase.FadeIn,
				hideAnimation: NotificationBase.FadeOut,
				sequenceType: NotifySequence.Last);
		}

		/// <summary>
		/// Show priority notification.
		/// </summary>
		public void ShowPriority()
		{
			NotificationQueueTemplate.Clone().Show(
				"Very Important Notification.",
				customHideDelay: 1f,
				showAnimation: NotificationBase.FadeIn,
				hideAnimation: NotificationBase.FadeOut,
				sequenceType: NotifySequence.First);
		}

		/// <summary>
		/// Show notification.
		/// </summary>
		public async void ShowNotify()
		{
			var actions = new NotificationButton[]
			{
				"Close",
				"Log",
			};

			var instance = NotificationTemplate.Clone().SetButtons(actions);
			var button_index = await instance.ShowAsync(
				"Notification with buttons. Hide after 5 seconds.",
				customHideDelay: 5f,
				closeOnButtonClick: false,
				showAnimation: NotificationBase.FadeIn,
				hideAnimation: NotificationBase.FadeOut);

			while (button_index == 1)
			{
				Debug.Log("Log click");
				button_index = await instance;
			}

			if (button_index == 0)
			{
				Debug.Log("Close click");
			}
			else
			{
				Debug.Log("Notification closed");
			}

			instance.Hide();
		}

		/// <summary>
		/// Show multiple notifications.
		/// </summary>
		public async void ShowMultiple()
		{
			await NotificationTemplate.Clone().SetButtons().ShowAsync("First Notification. Hide after 5 seconds.", customHideDelay: 5f, hideAnimation: NotificationBase.AnimationCollapseHorizontal);

			await NotificationTemplate.Clone().SetButtons().ShowAsync("Second Notification. Hide after 2 seconds.", customHideDelay: 2f, hideAnimation: NotificationBase.AnimationCollapseHorizontal);

			await NotificationTemplate.Clone().SetButtons().ShowAsync("Third Notification. Hide after 1 seconds.", customHideDelay: 1f, hideAnimation: NotificationBase.AnimationCollapseHorizontal);
		}
	}
}