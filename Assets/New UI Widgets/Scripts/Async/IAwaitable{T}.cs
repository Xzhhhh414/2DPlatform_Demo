namespace UIWidgets
{
	using System;

	/// <summary>
	/// Awaitable interface.
	/// </summary>
	/// <typeparam name="T">Type of awaitable value.</typeparam>
	public interface IAwaitable<T>
	{
		/// <summary>
		/// Action on complete.
		/// </summary>
		event Action<T> OnComplete;

		/// <summary>
		/// Get awaiter.
		/// </summary>
		/// <returns>Awaiter.</returns>
		Awaiter<T> GetAwaiter();
	}
}