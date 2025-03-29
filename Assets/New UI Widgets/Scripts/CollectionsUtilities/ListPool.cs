namespace UIWidgets.Pool
{
	using System.Collections.Generic;

	/// <summary>
	/// List pool.
	/// </summary>
	public static class ListPool
	{
		/// <summary>
		/// Release list.
		/// </summary>
		/// <typeparam name="T">Item type.</typeparam>
		/// <param name="list">List.</param>
		public static void Release<T>(List<T> list) => ListPool<T>.Release(list);
	}
}