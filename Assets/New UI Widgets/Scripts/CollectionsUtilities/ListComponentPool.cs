namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using UIWidgets.Extensions;
	using UnityEngine;

	/// <summary>
	/// List component pool.
	/// </summary>
	/// <typeparam name="TItemView">Component type.</typeparam>
	[Serializable]
	public class ListComponentPool<TItemView>
		where TItemView : Component
	{
		[SerializeField]
		List<TItemView> instances;

		[SerializeField]
		List<TItemView> cache;

		[SerializeField]
		RectTransform parent;

		[SerializeField]
		bool movableToCache;

		[SerializeField]
		TItemView template;

		/// <summary>
		/// Template.
		/// </summary>
		public TItemView Template
		{
			get => template;

			set
			{
				if (template == value)
				{
					return;
				}

				template = value;

				Require(0);

				foreach (var c in cache)
				{
					UnityEngine.Object.Destroy(c);
				}

				cache.Clear();

				if (!Utilities.IsNull(template))
				{
					template.gameObject.SetActive(false);
				}
			}
		}

		/// <summary>
		/// Default item.
		/// </summary>
		[Obsolete("Replaced with Template.")]
		public TItemView DefaultItem
		{
			get => template;

			set => Template = value;
		}

		/// <summary>
		/// Action on create star.
		/// </summary>
		public Action<TItemView> OnCreate;

		/// <summary>
		/// Action on destroy star.
		/// </summary>
		public Action<TItemView> OnDestroy;

		/// <summary>
		/// Action on enable star.
		/// </summary>
		public Action<TItemView> OnEnable;

		/// <summary>
		/// Action on disable star.
		/// </summary>
		public Action<TItemView> OnDisable;

		/// <summary>
		/// Initializes a new instance of the <see cref="ListComponentPool{TItemView}"/> class.
		/// </summary>
		/// <param name="template">Template.</param>
		/// <param name="instances">List of the active items.</param>
		/// <param name="cache">List of the cached items.</param>
		/// <param name="parent">Parent.</param>
		public ListComponentPool(TItemView template, List<TItemView> instances, List<TItemView> cache, RectTransform parent)
		{
			movableToCache = typeof(IMovableToCache).IsAssignableFrom(typeof(TItemView));

			this.template = template;
			if (!Utilities.IsNull(this.template))
			{
				this.template.gameObject.SetActive(false);
			}

			this.instances = instances;
			this.cache = cache;
			this.parent = parent;
		}

		/// <summary>
		/// Returns an enumerator that iterates through the <see cref="ListComponentPool{TItemView}" />.
		/// </summary>
		/// <returns>A <see cref="PoolEnumerator{TItemView}" /> for the <see cref="ListComponentPool{TItemView}" />.</returns>
		public PoolEnumerator<TItemView> GetEnumerator()
		{
			return new PoolEnumerator<TItemView>(PoolEnumeratorMode.Active, template, instances, cache);
		}

		/// <summary>
		/// Returns an enumerator that iterates through the <see cref="ListComponentPool{TItemView}" />.
		/// </summary>
		/// <param name="mode">Mode.</param>
		/// <returns>A <see cref="PoolEnumerator{TItemView}" /> for the <see cref="ListComponentPool{TItemView}" />.</returns>
		public PoolEnumerator<TItemView> GetEnumerator(PoolEnumeratorMode mode)
		{
			return new PoolEnumerator<TItemView>(mode, template, instances, cache);
		}

		/// <summary>
		/// Get component instance by the specified index.
		/// </summary>
		/// <param name="index">Item.</param>
		/// <returns>Component instance.</returns>
		public TItemView this[int index] => instances[index];

		/// <summary>
		/// Count of the active components.
		/// </summary>
		public int Count => instances.Count;

		/// <summary>
		/// Set the count of the active instances.
		/// </summary>
		/// <param name="instancesCount">Required instances count.</param>
		public void Require(int instancesCount)
		{
			if (instances.Count > instancesCount)
			{
				for (var i = instances.Count - 1; i >= instancesCount; i--)
				{
					Disable(instances[i]);
					instances.RemoveAt(i);
				}
			}

			while (instances.Count < instancesCount)
			{
				GetInstance();
			}
		}

		/// <summary>
		/// Disable components by indices.
		/// </summary>
		/// <param name="indices">Indices of the components to disable.</param>
		public void Disable(List<int> indices)
		{
			indices.Sort();

			for (int i = indices.Count - 1; i >= 0; i--)
			{
				var index = indices[i];
				Disable(instances[index]);
				instances.RemoveAt(index);
			}
		}

		/// <summary>
		/// Disable instance.
		/// </summary>
		/// <param name="instance">Instance.</param>
		protected void Disable(TItemView instance)
		{
			OnDisable?.Invoke(instance);

			if (movableToCache)
			{
				(instance as IMovableToCache).MovedToCache();
			}

			instance.gameObject.SetActive(false);
			cache.Add(instance);
		}

		/// <summary>
		/// Get instance.
		/// </summary>
		/// <returns>Instance.</returns>
		public TItemView GetInstance()
		{
			TItemView instance;

			if (cache.Count > 0)
			{
				instance = cache.Pop();
				instance.gameObject.SetActive(true);

				OnEnable?.Invoke(instance);
			}
			else
			{
				instance = Compatibility.Instantiate(Template);
				Utilities.FixInstantiated(Template, instance);
				instance.transform.SetParent(parent, false);
				instance.gameObject.SetActive(true);

				OnCreate?.Invoke(instance);
			}

			instances.Add(instance);

			return instance;
		}

		/// <summary>
		/// Apply function for each component.
		/// </summary>
		/// <param name="action">Action.</param>
		public void ForEach(Action<TItemView> action)
		{
			foreach (var a in instances)
			{
				action(a);
			}
		}

		/// <summary>
		/// Apply function for each component and cached components.
		/// </summary>
		/// <param name="action">Action.</param>
		public void ForEachAll(Action<TItemView> action)
		{
			foreach (var a in instances)
			{
				action(a);
			}

			foreach (var a in cache)
			{
				action(a);
			}
		}

		/// <summary>
		/// Apply function for each cached component.
		/// </summary>
		/// <param name="action">Action.</param>
		public void ForEachCache(Action<TItemView> action)
		{
			foreach (var a in cache)
			{
				action(a);
			}
		}

		/// <summary>
		/// Clear instances.
		/// </summary>
		public void Clear()
		{
			Require(0);

			foreach (var item in cache)
			{
				OnDestroy?.Invoke(item);
			}

			cache.Clear();
		}
	}
}