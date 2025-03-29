namespace UIWidgets
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UIWidgets.Attributes;
	using UnityEngine;

	/// <summary>
	/// Base object pool.
	/// Used as shared templates pool.
	/// </summary>
	/// <typeparam name="T">Type of object.</typeparam>
	[Serializable]
	public class BasePool<T>
		where T : Component
	{
		[SerializeField]
		T template;

		/// <summary>
		/// Template.
		/// </summary>
		public T Template => template;

		[SerializeField]
		List<T> cache = new List<T>();

		/// <summary>
		/// Count.
		/// </summary>
		public int Count => cache.Count;

		[DomainReloadExclude]
		static readonly Predicate<T> IsNull = x => x == null;

		/// <summary>
		/// Initializes a new instance of the <see cref="BasePool{T}"/> class.
		/// </summary>
		/// <param name="template">Template.</param>
		public BasePool(T template)
		{
			this.template = template;
		}

		/// <summary>
		/// Get instance.
		/// </summary>
		/// <param name="parent">Parent.</param>
		/// <returns>Instance.</returns>
		public T Get(Transform parent)
		{
			T instance;

			if (cache.Count == 0)
			{
				instance = Compatibility.Instantiate(template, parent);
				Utilities.FixInstantiated(template, instance);
			}
			else
			{
				cache.RemoveAll(IsNull);

				var index = cache.Count - 1;
				instance = cache[index];

				if (!UIThemes.UnityObjectComparer<Transform>.Instance.Equals(parent, instance.transform.parent))
				{
					instance.transform.SetParent(parent, false);
				}

				cache.RemoveAt(index);
			}

			return instance;
		}

		/// <summary>
		/// Return instance to cache.
		/// </summary>
		/// <param name="instance">Instance.</param>
		public void Release(T instance)
		{
			instance.gameObject.SetActive(false);
			cache.Add(instance);
		}

		/// <summary>
		/// Merge.
		/// </summary>
		/// <param name="pool">Pool.</param>
		/// <returns>true if pools are merged, otherwise false.</returns>
		public bool Merge(BasePool<T> pool)
		{
			if (!UIThemes.UnityObjectComparer<T>.Instance.Equals(Template, pool.Template))
			{
				return false;
			}

			cache.AddRange(pool.cache);
			pool.cache.Clear();

			return true;
		}

		/// <summary>
		/// Get enumerator.
		/// </summary>
		/// <returns>Enumerator.</returns>
		public List<T>.Enumerator GetEnumerator() => cache.GetEnumerator();

		/// <summary>
		/// Destroy cache instances.
		/// </summary>
		public void Destroy()
		{
			foreach (var item in cache)
			{
				UnityEngine.Object.Destroy(item.gameObject);
			}
		}
	}
}