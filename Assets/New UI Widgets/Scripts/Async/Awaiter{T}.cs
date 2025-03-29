namespace UIWidgets
{
	using System;
	using System.Runtime.CompilerServices;

	/// <summary>
	/// Awaiter.
	/// </summary>
	/// <typeparam name="T">Type of result.</typeparam>
	public class Awaiter<T> : INotifyCompletion
	{
		T result;

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
		/// Initializes a new instance of the <see cref="Awaiter{TResult}"/> class.
		/// </summary>
		/// <param name="awaitable">Awaitable instance.</param>
		public Awaiter(IAwaitable<T> awaitable)
		{
			void SetResult(T result)
			{
				awaitable.OnComplete -= SetResult;

				IsCompleted = true;
				this.result = result;
				continuation?.Invoke();
			}

			awaitable.OnComplete += SetResult;
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
		/// <returns>Result.</returns>
		public virtual T GetResult()
		{
			return result;
		}
	}
}