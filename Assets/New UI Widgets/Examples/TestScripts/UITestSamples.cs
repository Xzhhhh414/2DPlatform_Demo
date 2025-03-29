namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.Serialization;

	/// <summary>
	/// Notification and Dialog tests.
	/// </summary>
	public class UITestSamples : MonoBehaviour
	{
		/// <summary>
		/// Question icon.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("questionIcon")]
		protected Sprite QuestionIcon;

		/// <summary>
		/// Attention icon.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("attentionIcon")]
		protected Sprite AttentionIcon;

		/// <summary>
		/// Simple notification template.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("notifySimple")]
		protected Notify NotifySimpleTemplate;

		/// <summary>
		/// Auto-hide notification template.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("notifyAutoHide")]
		protected Notify NotifyAutoHideTemplate;

		/// <summary>
		/// Sample dialog template.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("dialogSample")]
		protected Dialog DialogSampleTemplate;

		/// <summary>
		/// Sign-in dialog template.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("dialogSignIn")]
		protected Dialog DialogSignInTemplate;

		/// <summary>
		/// TreeView dialog template.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("dialogTreeView")]
		protected Dialog DialogTreeViewTemplate;

		/// <summary>
		/// Popup template.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("popupSample")]
		protected Popup PopupTemplate;

		/// <summary>
		/// Modal popup template.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("popupModalSample")]
		protected Popup PopupModalTemplate;

		/// <summary>
		/// Show sticky notification.
		/// </summary>
		public void ShowNotifySticky()
		{
			var notification = NotifySimpleTemplate.Clone();
			notification.Show(
				"Sticky Notification. Click on the × above to close.",
				customHideDelay: 0f,
				hideAnimation: NotificationBase.FadeOut);
		}

		/// <summary>
		/// Show 3 notification, one by one in this order:
		/// - Queue Notification 3
		/// - Queue Notification 2
		/// - Queue Notification 1
		/// </summary>
		public void ShowNotifyStack()
		{
			NotifySimpleTemplate.Clone().Show(
				"Stack Notification 1.",
				customHideDelay: 3f,
				hideAnimation: NotificationBase.FadeOut,
				sequenceType: NotifySequence.First);
			NotifySimpleTemplate.Clone().Show(
				"Stack Notification 2.",
				customHideDelay: 3f,
				hideAnimation: NotificationBase.FadeOut,
				sequenceType: NotifySequence.First);
			NotifySimpleTemplate.Clone().Show(
				"Stack Notification 3.",
				customHideDelay: 3f,
				hideAnimation: NotificationBase.FadeOut,
				sequenceType: NotifySequence.First);
		}

		/// <summary>
		/// Show 3 notification, one by one in this order:
		/// - Queue Notification 1.
		/// - Queue Notification 2.
		/// - Queue Notification 3.
		/// </summary>
		public void ShowNotifyQueue()
		{
			NotifySimpleTemplate.Clone().Show(
				"Queue Notification 1.",
				customHideDelay: 3f,
				hideAnimation: NotificationBase.FadeOut,
				sequenceType: NotifySequence.Last);
			NotifySimpleTemplate.Clone().Show(
				"Queue Notification 2.",
				customHideDelay: 3f,
				hideAnimation: NotificationBase.FadeOut,
				sequenceType: NotifySequence.Last);
			NotifySimpleTemplate.Clone().Show(
				"Queue Notification 3.",
				customHideDelay: 3f,
				hideAnimation: NotificationBase.FadeOut,
				sequenceType: NotifySequence.Last);
		}

		/// <summary>
		/// Show only one notification and hide current notifications from sequence, if exists.
		/// Will be displayed only Queue Notification 3.
		/// </summary>
		public void ShowNotifySequenceClear()
		{
			NotifySimpleTemplate.Clone().Show(
				"Stack Notification 1.",
				customHideDelay: 3f,
				hideAnimation: NotificationBase.FadeOut,
				sequenceType: NotifySequence.First);
			NotifySimpleTemplate.Clone().Show(
				"Stack Notification 2.",
				customHideDelay: 3f,
				hideAnimation: NotificationBase.FadeOut,
				sequenceType: NotifySequence.First);
			NotifySimpleTemplate.Clone().Show(
				"Stack Notification 3.",
				customHideDelay: 3f,
				hideAnimation: NotificationBase.FadeOut,
				sequenceType: NotifySequence.First,
				clearSequence: true);
		}

		/// <summary>
		/// Show notify and close after 3 seconds.
		/// </summary>
		public void ShowNotifyAutohide()
		{
			NotifyAutoHideTemplate.Clone().Show("Achievement unlocked. Hide after 3 seconds.", customHideDelay: 3f, hideAnimation: NotificationBase.FadeOut);
		}

		/// <summary>
		/// Show notify with rotate animation.
		/// </summary>
		public void ShowNotifyAutohideRotate()
		{
			NotifyAutoHideTemplate.Clone().Show(
				"Achievement unlocked. Hide after 4 seconds.",
				customHideDelay: 4f,
				hideAnimation: NotificationBase.AnimationRotateVertical);
		}

		/// <summary>
		/// Show notify with collapse animation.
		/// </summary>
		public void ShowNotifyBlack()
		{
			NotifyAutoHideTemplate.Clone().Show(
				"Another Notification. Hide after 5 seconds or click on the × above to close.",
				customHideDelay: 5f,
				hideAnimation: NotificationBase.AnimationCollapseVertical,
				slideUpOnHide: false);
		}

		/// <summary>
		/// Show simple dialog.
		/// </summary>
		public void ShowDialogSimple()
		{
			UtilitiesUI.FindTopmostCanvas(transform).TryGetComponent<Canvas>(out var canvas);

			var dialog = DialogSampleTemplate.Clone();

			var actions = new DialogButton[]
			{
				new DialogButton("Close"),
			};

			dialog.Show(
				title: "Simple Dialog",
				message: "Simple dialog with only close button.",
				buttons: actions,
				focusButton: "Close",
				canvas: canvas);
		}

		/// <summary>
		/// Show dialog in the same position when it was closed.
		/// </summary>
		public void ShowDialogInPosition()
		{
			var dialog = DialogSampleTemplate.Clone();

			var actions = new DialogButton[]
			{
				new DialogButton("Close"),
			};

			dialog.Show(
				title: "Simple Dialog",
				message: "Simple dialog with only close button.",
				buttons: actions,
				focusButton: "Close",
				position: dialog.transform.localPosition);
		}

		/// <summary>
		/// Show warning.
		/// </summary>
		public void ShowWarning()
		{
			var actions = new DialogButton[]
			{
				new DialogButton("OK"),
			};

			DialogSampleTemplate.Clone().Show(
				title: "Warning window",
				message: "Warning test",
				buttons: actions,
				focusButton: "OK",
				icon: AttentionIcon);
		}

		/// <summary>
		/// Show dialog with Yes/No/Cancel buttons.
		/// </summary>
		public async void ShowDialogYesNoCancel()
		{
			var actions = new DialogButton[]
			{
				new DialogButton("Yes"),
				new DialogButton("No"),
				new DialogButton("Cancel"),
			};

			var button_index = await DialogSampleTemplate.Clone().ShowAsync(
				title: "Dialog Yes No Cancel",
				message: "Question?",
				buttons: actions,
				focusButton: "Yes",
				icon: QuestionIcon);

			if (button_index == 0)
			{
				NotifyAutoHideTemplate.Clone().Show("Action on 'Yes' button click.", customHideDelay: 3f);
			}
			else if (button_index == 1)
			{
				NotifyAutoHideTemplate.Clone().Show("Action on 'No' button click.", customHideDelay: 3f);
			}
		}

		/// <summary>
		/// Show dialog with lots of text.
		/// </summary>
		public async void ShowDialogExtended()
		{
			var actions = new DialogButton[]
			{
				new DialogButton("Show notification"),
				new DialogButton("Open simple dialog"),
				new DialogButton("Close"),
			};

			var button_index = await DialogSampleTemplate.Clone().ShowAsync(
				title: "Another Dialog",
				message: "Same template with another position and long text.\nChange\nheight\nto\nfit\ntext.",
				buttons: actions,
				focusButton: "Show notification",
				position: new Vector3(40, -40, 0));

			if (button_index == 0)
			{
				ShowNotifyAutohide();
			}
			else if (button_index == 1)
			{
				ShowDialogSimple();
			}
		}

		/// <summary>
		/// Show modal dialog.
		/// </summary>
		public void ShowDialogModal()
		{
			var actions = new DialogButton[]
			{
				new DialogButton("Close"),
			};

			DialogSampleTemplate.Clone().Show(
				title: "Modal Dialog",
				message: "Simple Modal Dialog.",
				buttons: actions,
				focusButton: "Close",
				modal: true,
				modalColor: new Color(0, 0, 0, 0.8f));
		}

		/// <summary>
		/// Show sing-in dialog.
		/// </summary>
		public async void ShowDialogSignIn()
		{
			// create dialog from template
			var dialog = DialogSignInTemplate.Clone();

			// helper component with references to input fields
			dialog.TryGetComponent<DialogInputHelper>(out var helper);

			// reset input fields to default
			helper.Refresh();

			var actions = new DialogButton[]
			{
				new DialogButton("Sign in"),
				new DialogButton("Cancel"),
			};

			// open dialog
			var button_index = await dialog.ShowAsync(
				title: "Sign into your Account",
				buttons: actions,
				focusButton: "Sign in",
				modal: true,
				modalColor: new Color(0, 0, 0, 0.8f),
				closeOnButtonClick: false);

			while (button_index == 0 && !helper.Validate())
			{
				button_index = await dialog;
			}

			if (button_index == 0)
			{
				var message = string.Format("Sign in.\nUsername: {0}\nPassword: <hidden>", helper.UsernameAdapter.text);
				NotifyAutoHideTemplate.Clone().Show(message, customHideDelay: 3f);
			}

			dialog.Hide();
		}

		/// <summary>
		/// Show dialog with TreeView.
		/// </summary>
		public void ShowDialogTreeView()
		{
			// create dialog from template
			var dialog = DialogTreeViewTemplate.Clone();

			// helper component with references to input fields
			dialog.TryGetComponent<DialogTreeViewInputHelper>(out var helper);
			helper.Refresh();

			var actions = new DialogButton[]
			{
				// on click close dialog
				new DialogButton("Close"),
			};

			// open dialog
			dialog.Show(
				title: "Dialog with TreeView",
				buttons: actions,
				focusButton: "Close",
				modal: true,
				modalColor: new Color(0, 0, 0, 0.8f));
		}

		/// <summary>
		/// Show simple popup.
		/// </summary>
		public void ShowPopup()
		{
			PopupTemplate.Clone().Show(
				title: "Simple Popup",
				message: "Simple Popup.");
		}

		/// <summary>
		/// Show modal popup.
		/// </summary>
		public void ShowPopupModal()
		{
			PopupModalTemplate.Clone().Show(
				title: "Modal Popup",
				message: "Alert text.",
				modal: true,
				modalColor: new Color(0.0f, 0.0f, 0.0f, 0.8f));
		}
	}
}