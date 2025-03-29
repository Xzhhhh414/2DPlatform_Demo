namespace UIThemes.Pool
{
	using System;
	using System.Collections.Generic;

	/// <summary>
	/// List pool.
	/// </summary>
	/// <typeparam name="T">Item type.</typeparam>
	public static class ListPool<T>
	{
		/// <summary>
		/// Pooled list wrapper.
		/// </summary>
		public readonly ref struct PooledList
		{
			readonly List<T> list;

			/// <summary>
			/// Initializes a new instance of the <see cref="PooledList"/> struct.
			/// </summary>
			/// <param name="instance">Instance.</param>
			public PooledList(List<T> instance) => list = instance;

			/// <summary>
			/// Dispose (release list).
			/// </summary>
			public void Dispose() => Release(list);
		}

		[DomainReloadExclude]
		static readonly Stack<List<T>> Cache = new Stack<List<T>>();

		/// <summary>
		/// Get list.
		/// </summary>
		/// <returns>List.</returns>
		public static List<T> Get() => Cache.Count > 0 ? Cache.Pop() : new List<T>();

		/// <summary>
		/// Get list.
		/// </summary>
		/// <param name="list">List.</param>
		/// <returns>Pooled list wrapper.</returns>
		/// <example>using var _ = ListPool&lt;int&gt;.Get(out var temp);</example>
		public static PooledList Get(out List<T> list) => new PooledList(list = Get());

		/// <summary>
		/// Release.
		/// </summary>
		/// <param name="list">List.</param>
		public static void Release(List<T> list)
		{
			list.Clear();
			Cache.Push(list);
		}
	}
}