namespace UIWidgets
{
	using System.Collections.Generic;

	/// <summary>
	/// Event subscriptions.
	/// </summary>
	/// <typeparam name="T">Type of subscription.</typeparam>
	public struct Subscriptions<T> : ISubscription
		where T : ISubscription
	{
		readonly List<T> subscriptions;

		/// <summary>
		/// Initializes a new instance of the <see cref="Subscriptions{T}"/> struct.
		/// </summary>
		/// <param name="subscription">Subscription.</param>
		public Subscriptions(T subscription)
		{
			isOn = true;

			subscriptions = new List<T>()
			{
				subscription,
			};
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Subscriptions{T}"/> struct.
		/// </summary>
		/// <param name="subscription0">Subscription 0.</param>
		/// <param name="subscription1">Subscription 1.</param>
		public Subscriptions(T subscription0, T subscription1)
		{
			isOn = true;

			subscriptions = new List<T>()
			{
				subscription0,
				subscription1,
			};
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Subscriptions{T}"/> struct.
		/// </summary>
		/// <param name="subscription0">Subscription 0.</param>
		/// <param name="subscription1">Subscription 1.</param>
		/// <param name="subscription2">Subscription 2.</param>
		public Subscriptions(T subscription0, T subscription1, T subscription2)
		{
			isOn = true;

			subscriptions = new List<T>()
			{
				subscription0,
				subscription1,
				subscription2,
			};
		}

		/// <summary>
		/// Add subscription.
		/// </summary>
		/// <param name="subscription">Subscription.</param>
		public readonly void Add(T subscription)
		{
			subscriptions.Add(subscription);

			if (IsOn)
			{
				subscription.On();
			}
			else
			{
				subscription.Off();
			}
		}

		bool isOn;

		/// <summary>
		/// Is listeners enabled?
		/// </summary>
		public readonly bool IsOn => isOn;

		/// <summary>
		/// Permanently disable event listeners.
		/// </summary>
		public void Clear()
		{
			Off();

			subscriptions.Clear();
		}

		/// <summary>
		/// Disable event listener.
		/// </summary>
		public void Off()
		{
			if (!isOn)
			{
				return;
			}

			foreach (var s in subscriptions)
			{
				s.Off();
			}

			isOn = false;
		}

		/// <summary>
		/// Enable event listener.
		/// </summary>
		public void On()
		{
			if (isOn)
			{
				return;
			}

			foreach (var s in subscriptions)
			{
				s.On();
			}

			isOn = true;
		}
	}
}