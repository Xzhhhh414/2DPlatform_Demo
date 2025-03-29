namespace UIWidgets
{
	using System;

	/// <summary>
	/// Awaiter.
	/// </summary>
	public class Awaiter
	{
		readonly IAwaitable awaitable;

		Action continuation;

		/// <summary>
		/// Is completed.
		/// </summary>
		public bool IsCompleted
		{
			get;
			private set;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Awaiter"/> class.
		/// </summary>
		/// <param name="awaitable">Awaitable instance.</param>
		public Awaiter(IAwaitable awaitable)
		{
			this.awaitable = awaitable;

			this.awaitable.OnComplete += () =>
			{
				IsCompleted = true;
				continuation?.Invoke();
			};
		}

		/// <summary>
		/// Add action after complete.
		/// </summary>
		/// <param name="continuation">Action on continuation.</param>
		public virtual void OnCompleted(Action continuation)
		{
			if (IsCompleted)
			{
				continuation();
				return;
			}

			this.continuation = continuation;
		}

		/// <summary>
		/// Get result.
		/// </summary>
		public virtual void GetResult()
		{
		}
	}
}