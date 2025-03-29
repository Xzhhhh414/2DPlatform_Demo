namespace UIWidgets
{
	using System;

	/// <summary>
	/// Awaitable interface.
	/// </summary>
	public interface IAwaitable
	{
		/// <summary>
		/// Action on complete.
		/// </summary>
		event Action OnComplete;

		/// <summary>
		/// Get awaiter.
		/// </summary>
		/// <returns>Awaiter.</returns>
		Awaiter GetAwaiter();
	}
}