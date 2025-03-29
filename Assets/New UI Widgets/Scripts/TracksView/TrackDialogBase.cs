namespace UIWidgets
{
	using System;
	using UnityEngine;

	/// <summary>
	/// Base class for the dialog to add/edit track.
	/// </summary>
	/// <typeparam name="TData">Type of the data.</typeparam>
	/// <typeparam name="TPoint">Type of the points.</typeparam>
	/// <typeparam name="TTrackForm">TrackForm type.</typeparam>
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/widgets/collections/tracksview.html")]
	public class TrackDialogBase<TData, TPoint, TTrackForm> : MonoBehaviour
		where TTrackForm : TrackFormBase<TData, TPoint>
		where TData : class, ITrackData<TPoint>
		where TPoint : IComparable<TPoint>
	{
		/// <summary>
		/// Dialog template.
		/// </summary>
		[SerializeField]
		public Dialog DialogTemplate;

		/// <summary>
		/// Create header text.
		/// </summary>
		[SerializeField]
		public string HeaderCreate = "Create Track";

		/// <summary>
		/// Create button text.
		/// </summary>
		[SerializeField]
		public string ButtonCreate = "Create";

		/// <summary>
		/// Cancel button text.
		/// </summary>
		[SerializeField]
		public string ButtonCancel = "Cancel";

		/// <summary>
		/// Edit header text.
		/// </summary>
		[SerializeField]
		public string HeaderEdit = "Edit Track";

		/// <summary>
		/// Save button text.
		/// </summary>
		[SerializeField]
		public string ButtonEdit = "Save";

		/// <summary>
		/// Create track.
		/// </summary>
		/// <param name="onSuccess">Function to call after track created.</param>
		public void Create(Action<Track<TData, TPoint>> onSuccess)
		{
			var dialog = DialogTemplate.Clone();
			dialog.TryGetComponent<TTrackForm>(out var form);
			form.Create();

			OpenDialog(dialog, form, HeaderCreate, ButtonCreate, onSuccess);
		}

		/// <summary>
		/// Edit track.
		/// </summary>
		/// <param name="track">Track.</param>
		/// <param name="onSuccess">Function to call after track edited.</param>
		public void Edit(Track<TData, TPoint> track, Action<Track<TData, TPoint>> onSuccess)
		{
			var dialog = DialogTemplate.Clone();
			dialog.TryGetComponent<TTrackForm>(out var form);
			form.Edit(track);

			OpenDialog(dialog, form, HeaderEdit, ButtonEdit, onSuccess);
		}

		/// <summary>
		/// Open dialog.
		/// </summary>
		/// <param name="dialog">Dialog instance.</param>
		/// <param name="form">Form.</param>
		/// <param name="title">Title.</param>
		/// <param name="okButton">OK button text.</param>
		/// <param name="onSuccess">Function to call on OK button click.</param>
		protected async void OpenDialog(Dialog dialog, TTrackForm form, string title, string okButton, Action<Track<TData, TPoint>> onSuccess)
		{
			var actions = new DialogButton[]
			{
				new DialogButton(okButton),
				new DialogButton(ButtonCancel),
			};

			var button_index = await dialog.ShowAsync(
				title: title,
				buttons: actions,
				focusButton: okButton,
				modal: true,
				modalColor: new Color(0, 0, 0, 0.8f));

			if (button_index == 0)
			{
				onSuccess(form.Data);
			}
		}
	}
}