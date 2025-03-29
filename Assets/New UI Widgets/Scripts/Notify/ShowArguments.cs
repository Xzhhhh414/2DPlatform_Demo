namespace UIWidgets
{
	using System;
	using System.Collections;
	using System.Runtime.CompilerServices;
	using UIWidgets.Styles;
	using UnityEngine;

	/// <summary>
	/// Base generic class for the custom notifications.
	/// </summary>
	public abstract partial class NotificationCustom<TNotification> : NotificationBase, IStylable, IUpgradeable, IHideable, INotifyCompletion
		where TNotification : NotificationCustom<TNotification>
	{
		/// <summary>
		/// Arguments for the TNotification.Show() method.
		/// </summary>
		public class ShowArguments
		{
			/// <summary>
			/// Message.
			/// </summary>
			public string Message
			{
				get;
				set;
			}

			= null;

			/// <summary>
			/// Custom hide delay.
			/// </summary>
			public float? CustomHideDelay
			{
				get;
				set;
			}

			= null;

			/// <summary>
			/// Container. Parent object for the notification.
			/// </summary>
			public RectTransform Container
			{
				get;
				set;
			}

			= null;

			/// <summary>
			/// Function used to run show animation.
			/// </summary>
			public Func<TNotification, IEnumerator> ShowAnimation
			{
				get;
				set;
			}

			= null;

			/// <summary>
			/// Function used to run hide animation.
			/// </summary>
			public Func<TNotification, IEnumerator> HideAnimation
			{
				get;
				set;
			}

			= null;

			/// <summary>
			/// Start slide up animations after hide current notification.
			/// </summary>
			public bool? SlideUpOnHide
			{
				get;
				set;
			}

			= null;

			/// <summary>
			/// Add notification to sequence and display in order according specified sequenceType.
			/// </summary>
			public NotifySequence SequenceType
			{
				get;
				set;
			}

			= NotifySequence.None;

			/// <summary>
			/// Time between previous notification was hidden and next will be showed.
			/// </summary>
			public float SequenceDelay
			{
				get;
				set;
			}

			= 0.3f;

			/// <summary>
			/// Clear notifications sequence.
			/// </summary>
			public bool ClearSequence
			{
				get;
				set;
			}

			= false;

			/// <summary>
			/// Use unscaled time.
			/// </summary>
			public bool? UnscaledTime
			{
				get;
				set;
			}

			= null;

			/// <summary>
			/// Notification content.
			/// </summary>
			public RectTransform Content
			{
				get;
				set;
			}

			= null;

			/// <summary>
			/// Action called when notification instance return to the cache.
			/// Used with Show() method only.
			/// </summary>
			public Action<TNotification> OnHide
			{
				get;
				set;
			}

			= null;

			/// <summary>
			/// Close notification on button click.
			/// Used with ShowAsync() method only.
			/// </summary>
			public bool CloseOnButtonClick
			{
				get;
				set;
			}

			= true;
		}
	}
}