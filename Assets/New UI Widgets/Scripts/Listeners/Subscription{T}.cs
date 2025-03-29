namespace UIWidgets
{
	using UnityEngine.Events;

	/// <summary>
	/// Event subscription.
	/// </summary>
	/// <typeparam name="T">Type of event argument.</typeparam>
	public struct Subscription<T> : ISubscription
	{
		UnityEvent<T> ev;

		UnityAction<T> callback;

		bool isOn;

		/// <summary>
		/// Is listener enabled?
		/// </summary>
		public readonly bool IsOn => isOn;

		/// <summary>
		/// Initializes a new instance of the <see cref="Subscription{T}"/> struct.
		/// </summary>
		/// <param name="ev">Event.</param>
		/// <param name="callback">Callback.</param>
		public Subscription(UnityEvent<T> ev, UnityAction<T> callback)
		{
			this.ev = ev;
			this.callback = callback;
			isOn = false;

			On();
		}

		/// <summary>
		/// Enable event listener.
		/// </summary>
		public void On()
		{
			if (isOn || (ev == null))
			{
				return;
			}

			ev.AddListener(callback);
			isOn = true;
		}

		/// <summary>
		/// Disable event listener.
		/// </summary>
		public void Off()
		{
			if (!isOn || (ev == null))
			{
				return;
			}

			ev.RemoveListener(callback);
			isOn = true;
		}

		/// <summary>
		/// Permanently disable event listener.
		/// </summary>
		public void Clear()
		{
			Off();
			ev = null;
			callback = null;
		}
	}
}