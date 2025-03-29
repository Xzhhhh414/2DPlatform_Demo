namespace UIWidgets.Examples
{
	using System;
	using UnityEngine;
	using UnityEngine.EventSystems;
	using UnityEngine.UI;

	/// <summary>
	/// Confirm example.
	/// </summary>
	public class ConfirmExample : MonoBehaviourInitiable, IAwaitable<bool>
	{
		/// <summary>
		/// Message.
		/// </summary>
		[SerializeField]
		protected TextAdapter Message;

		/// <summary>
		/// Button OK.
		/// </summary>
		[SerializeField]
		protected Button ButtonOk;

		/// <summary>
		/// Button Cancel.
		/// </summary>
		[SerializeField]
		protected Button ButtonCancel;

		event Action<bool> EvOnComplete;

		/// <summary>
		/// Action on complete.
		/// </summary>
		public event Action<bool> OnComplete
		{
			add => EvOnComplete += value;

			remove => EvOnComplete -= value;
		}

		/// <summary>
		/// Get awaiter.
		/// </summary>
		/// <returns>Awaiter.</returns>
		public Awaiter<bool> GetAwaiter() => new Awaiter<bool>(this);

		/// <inheritdoc/>
		protected override void InitOnce()
		{
			base.InitOnce();

			AddListeners();
		}

		/// <summary>
		/// Process the destroy event.
		/// </summary>
		protected virtual void OnDestroy()
		{
			RemoveListeners();
			Cancel();
		}

		void AddListeners()
		{
			ButtonOk.onClick.AddListener(Confirm);
			ButtonCancel.onClick.AddListener(Cancel);
		}

		void RemoveListeners()
		{
			ButtonOk.onClick.RemoveListener(Confirm);
			ButtonCancel.onClick.RemoveListener(Cancel);
		}

		/// <summary>
		/// Confirm.
		/// </summary>
		public void Confirm() => Complete(true);

		/// <summary>
		/// Cancel.
		/// </summary>
		public void Cancel() => Complete(false);

		void Complete(bool result)
		{
			gameObject.SetActive(false);
			EvOnComplete?.Invoke(result);
		}

		/// <summary>
		/// Open.
		/// </summary>
		/// <param name="message">Messages.</param>
		/// <returns>Awaitable.</returns>
		public ConfirmExample Open(string message)
		{
			Message.text = message;
			gameObject.SetActive(true);
			EventSystem.current.SetSelectedGameObject(ButtonOk.gameObject);

			return this;
		}
	}
}