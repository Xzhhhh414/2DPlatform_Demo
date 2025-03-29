namespace UIWidgets.AsyncExtensions
{
	using System.Runtime.CompilerServices;
	using System.Threading.Tasks;
	using UnityEngine;

	/// <summary>
	/// Async extensions.
	/// </summary>
	public static class AsyncExtensions
	{
		#if !UNITY_2023_2_OR_NEWER
		/// <summary>
		/// Allows to await Unity AsyncOperation.
		/// </summary>
		/// <param name="asyncOp">AsyncOperation.</param>
		/// <returns>Task awaiter.</returns>
		public static TaskAwaiter GetAwaiter(this AsyncOperation asyncOp)
		{
			var tcs = new TaskCompletionSource<object>();
			asyncOp.completed += obj => { tcs.SetResult(null); };
			return ((Task)tcs.Task).GetAwaiter();
		}
		#endif
	}
}