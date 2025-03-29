namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using UIWidgets.AsyncExtensions;
	using UIWidgets.Pool;

	/// <summary>
	/// Data loader: process and cache requests.
	/// </summary>
	/// <typeparam name="TRequest">Type of request.</typeparam>
	/// <typeparam name="TData">Type of data.</typeparam>
	public class DataLoader<TRequest, TData>
	{
		readonly Dictionary<TRequest, TData> cache = new Dictionary<TRequest, TData>();

		readonly Dictionary<TRequest, Task<Tuple<bool, TData>>> tasks = new Dictionary<TRequest, Task<Tuple<bool, TData>>>();

		readonly Func<TRequest, Task<Tuple<bool, TData>>> loader;

		/// <summary>
		/// Initializes a new instance of the <see cref="DataLoader{TRequest, TData}"/> class.
		/// </summary>
		/// <param name="loader">Loader.</param>
		public DataLoader(Func<TRequest, Task<TData>> loader)
		{
			this.loader = async request => new Tuple<bool, TData>(true, await loader(request));
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DataLoader{TRequest, TData}"/> class.
		/// </summary>
		/// <param name="loader">Loader.</param>
		public DataLoader(Func<TRequest, Task<Tuple<bool, TData>>> loader)
		{
			this.loader = loader;
		}

		/// <summary>
		/// Is request was processed and received valid result.
		/// </summary>
		/// <param name="request">Request.</param>
		/// <returns>true if request was processed and received valid result; otherwise false.</returns>
		public bool IsLoaded(TRequest request) => cache.ContainsKey(request);

		/// <summary>
		/// Try get value.
		/// </summary>
		/// <param name="request">Request.</param>
		/// <param name="result">Result.</param>
		/// <returns>true if request was processed and received valid result; otherwise false.</returns>
		public bool TryGetValue(TRequest request, out TData result) => cache.TryGetValue(request, out result);

		/// <summary>
		/// Remove all requests from the cache that satisfy a condition.
		/// </summary>
		/// <param name="condition">Condition.</param>
		/// <returns>Number of removed requests.</returns>
		public int Remove(Func<TRequest, TData, bool> condition)
		{
			using var _ = ListPool<TRequest>.Get(out var requests);

			var removed = 0;
			requests.AddRange(cache.Keys);
			foreach (var request in requests)
			{
				if (!condition(request, cache[request]))
				{
					continue;
				}

				if (cache.Remove(request))
				{
					removed += 1;
				}
			}

			return removed;
		}

		/// <summary>
		/// Remove all specified requests from cache.
		/// </summary>
		/// <param name="requests">Requests.</param>
		/// <returns>Number of removed requests.</returns>
		public int Remove(IReadOnlyList<TRequest> requests)
		{
			var removed = 0;
			for (var i = 0; i < requests.Count; i++)
			{
				if (cache.Remove(requests[i]))
				{
					removed += 1;
				}
			}

			return removed;
		}

		/// <summary>
		/// Remove all specified request from the cache.
		/// </summary>
		/// <param name="request">Request.</param>
		/// <returns>true if item was removed; otherwise false.</returns>
		public bool Remove(TRequest request) => cache.Remove(request);

		/// <summary>
		/// Get data for the specified request.
		/// </summary>
		/// <param name="request">Request.</param>
		/// <param name="callback">Callback.</param>
		public async void Get(TRequest request, Action<TRequest, TData> callback)
		{
			if (cache.TryGetValue(request, out var data))
			{
				callback.Invoke(request, data);
				return;
			}

			// prevent multiple requests
			if (tasks.ContainsKey(request))
			{
				var (_, await_data) = await tasks[request];
				callback.Invoke(request, await_data);
				return;
			}

			// create task
			var task = loader(request);
			tasks[request] = task;
			var (success, task_data) = await task;

			tasks.Remove(request);
			if (success)
			{
				cache[request] = task_data;
			}

			callback.Invoke(request, task_data);
		}

		/// <summary>
		/// Get data for the specified request.
		/// </summary>
		/// <param name="request">Request.</param>
		/// <returns>Data.</returns>
		public async Task<Tuple<bool, TData>> GetAsync(TRequest request)
		{
			if (cache.TryGetValue(request, out var data))
			{
				return new Tuple<bool, TData>(true, data);
			}

			// prevent multiple requests
			if (tasks.ContainsKey(request))
			{
				return await tasks[request];
			}

			// create task
			var task = loader(request);
			tasks[request] = task;
			var (success, task_data) = await task;

			tasks.Remove(request);

			if (success)
			{
				cache[request] = task_data;
			}

			return new Tuple<bool, TData>(success, task_data);
		}
	}
}