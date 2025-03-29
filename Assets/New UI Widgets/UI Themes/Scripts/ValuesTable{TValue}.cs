namespace UIThemes
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using UIThemes.Pool;
	using UnityEngine;
	using UnityEngine.Serialization;

	/// <summary>
	/// Values table.
	/// Contains options and values.
	/// </summary>
	/// <typeparam name="TValue">Type of value.</typeparam>
	[Serializable]
	public class ValuesTable<TValue>
	{
		/// <summary>
		/// Value wrapper.
		/// </summary>
		[Serializable]
		public class ValueWrapper
		{
			[SerializeField]
			VariationId variationId;

			/// <summary>
			/// Variation ID.
			/// </summary>
			public VariationId VariationId
			{
				get => variationId;

				private set => variationId = value;
			}

			[SerializeField]
			OptionId optionId;

			/// <summary>
			/// Option ID.
			/// </summary>
			public OptionId OptionId
			{
				get => optionId;

				private set => optionId = value;
			}

			[SerializeField]
			[FormerlySerializedAs("Value")]
			TValue data;

			/// <summary>
			/// Value.
			/// </summary>
			public TValue Value
			{
				get
				{
					if (LoadRequired)
					{
#if UITHEMES_ADDRESSABLE_SUPPORT && UNITY_EDITOR
						var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
						var obj = UnityEditor.AssetDatabase.LoadAssetAtPath(path, typeof(TValue));
						if (obj is TValue t)
						{
							data = t;
						}
#elif UITHEMES_ADDRESSABLE_SUPPORT
						var op = UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<TValue>(Address);
						data = op.WaitForCompletion();
#endif
					}

					return data;
				}

				set => data = value;
			}

			/// <summary>
			/// Is required asset load?
			/// </summary>
			public bool LoadRequired => ValuesTable<TValue>.IsUnityObject && (data == null) && !string.IsNullOrEmpty(Address);

			[SerializeField]
			string address = string.Empty;

			/// <summary>
			/// Address.
			/// </summary>
			public string Address => address;

			[SerializeField]
			string guid = string.Empty;

			/// <summary>
			/// GUID.
			/// </summary>
			public string GUID => guid;

			/// <summary>
			/// Key.
			/// </summary>
			public ValueKey Key => new ValueKey(VariationId, OptionId);

			/// <summary>
			/// Initializes a new instance of the <see cref="ValueWrapper"/> class.
			/// </summary>
			/// <param name="variationId">Variation ID.</param>
			/// <param name="optionId">Option ID.</param>
			/// <param name="value">Value.</param>
			public ValueWrapper(VariationId variationId, OptionId optionId, TValue value)
			{
				VariationId = variationId;
				OptionId = optionId;
				Value = value;
			}

			/// <summary>
			/// Set address.
			/// </summary>
			/// <param name="asset">Asset addressable info.</param>
			/// <param name="resetValue">Should reset the value?</param>
			public void SetAddress(AddressableAsset asset, bool resetValue)
			{
				if (asset.Valid)
				{
					address = asset.Address;
					guid = asset.GUID;

					if (resetValue)
					{
						data = default;
					}
				}
				else
				{
					address = string.Empty;
					guid = string.Empty;
				}
			}

			/// <summary>
			/// Reset address.
			/// </summary>
			public void ResetAddress()
			{
				address = string.Empty;
				guid = string.Empty;
			}
		}

		/// <summary>
		/// Is TValue inherited from UnityEngine.Object?
		/// </summary>
		public static bool IsUnityObject;

		static ValuesTable()
		{
			IsUnityObject = typeof(UnityEngine.Object).IsAssignableFrom(typeof(TValue));
		}

		/// <summary>
		/// Options.
		/// </summary>
		[SerializeField]
		protected List<Option> options = new List<Option>();

		/// <summary>
		/// Options.
		/// </summary>
		public IReadOnlyList<Option> Options => options;

		/// <summary>
		/// Values.
		/// </summary>
		[SerializeField]
		protected List<ValueWrapper> values = new List<ValueWrapper>();

		/// <summary>
		/// Values.
		/// </summary>
		public IReadOnlyList<ValueWrapper> Values => values;

		/// <summary>
		/// Cache.
		/// </summary>
		protected Dictionary<ValueKey, TValue> Cache = new Dictionary<ValueKey, TValue>();

		IEqualityComparer<TValue> comparer;

		/// <summary>
		/// Values comparer.
		/// </summary>
		public IEqualityComparer<TValue> Comparer
		{
			get => comparer != null ? comparer : EqualityComparer<TValue>.Default;

			set => comparer = value;
		}

		/// <summary>
		/// Has values comparer.
		/// </summary>
		public bool HasComparer => comparer != null;

		/// <summary>
		/// Fill cache.
		/// </summary>
		protected void FillCache()
		{
			if (Cache.Count > 0)
			{
				return;
			}

			foreach (var v in values)
			{
				Cache[v.Key] = v.Value;
			}
		}

		/// <summary>
		/// Get index of the specified option ID.
		/// </summary>
		/// <param name="optionId">Option ID.</param>
		/// <returns>Index.</returns>
		protected int OptionId2Index(OptionId optionId)
		{
			for (var i = 0; i < options.Count; i++)
			{
				if (options[i].Id == optionId)
				{
					return i;
				}
			}

			return -1;
		}

		/// <summary>
		/// Move option.
		/// </summary>
		/// <param name="oldIndex">Old index.</param>
		/// <param name="newIndex">New index.</param>
		/// <returns>true if option was moved; otherwise false.</returns>
		public bool MoveOption(int oldIndex, int newIndex)
		{
			var old_valid = oldIndex >= 0 && oldIndex < options.Count;
			var new_valid = newIndex >= 0 && newIndex <= options.Count;
			if (!old_valid || !new_valid || (oldIndex == newIndex))
			{
				return false;
			}

			var current = options[oldIndex];
			options.RemoveAt(oldIndex);
			if (oldIndex < newIndex)
			{
				newIndex -= 1;
			}

			options.Insert(newIndex, current);

			return true;
		}

		/// <summary>
		/// Get value.
		/// </summary>
		/// <param name="variationId">Variation ID.</param>
		/// <param name="optionId">Option ID.</param>
		/// <param name="defaultValue">Default value.</param>
		/// <returns>Value.</returns>
		/// <exception cref="ArgumentException">Thrown if variation not found.</exception>
		public TValue Get(VariationId variationId, OptionId optionId, TValue defaultValue = default)
		{
			if (!HasOption(optionId))
			{
				throw new ArgumentException("Not found option with id: " + optionId.Id, nameof(optionId));
			}

			FillCache();

			if (Cache.TryGetValue(new ValueKey(variationId, optionId), out var value))
			{
				return value;
			}

			return defaultValue;
		}

		/// <summary>
		/// Set value.
		/// </summary>
		/// <param name="variationId">Variation ID.</param>
		/// <param name="optionId">Option ID.</param>
		/// <param name="value">Value.</param>
		/// <returns>true if value was changed; otherwise false.</returns>
		/// <exception cref="ArgumentException">Thrown if variation not found.</exception>
		public bool Set(VariationId variationId, OptionId optionId, TValue value)
		{
			if (!HasOption(optionId))
			{
				throw new ArgumentException("Not found option with id: " + optionId.Id, nameof(optionId));
			}

			FillCache();

			foreach (var v in values)
			{
				if ((v.VariationId == variationId) && (v.OptionId == optionId))
				{
					if (Comparer.Equals(v.Value, value))
					{
						return false;
					}

					v.Value = value;
					v.ResetAddress();
					Cache[v.Key] = v.Value;
					return true;
				}
			}

			var vw = new ValueWrapper(variationId, optionId, value);
			values.Add(vw);
			Cache[vw.Key] = vw.Value;
			return true;
		}

		/// <summary>
		/// Has value.
		/// </summary>
		/// <param name="variationId">Variation ID.</param>
		/// <param name="optionId">Option ID.</param>
		/// <returns>true if has value; otherwise false.</returns>
		/// <exception cref="ArgumentException">Thrown if variation not found.</exception>
		public bool HasValue(VariationId variationId, OptionId optionId)
		{
			if (!HasOption(optionId))
			{
				throw new ArgumentException("Not found option with id: " + optionId.Id, nameof(optionId));
			}

			FillCache();

			return Cache.ContainsKey(new ValueKey(variationId, optionId));
		}

		/// <summary>
		/// Try get value.
		/// </summary>
		/// <param name="variationId">Variation ID.</param>
		/// <param name="optionId">Option ID.</param>
		/// <param name="value">Value.</param>
		/// <returns>true if value exists; otherwise false.</returns>
		public bool TryGetValue(VariationId variationId, OptionId optionId, out TValue value)
		{
			FillCache();

			return Cache.TryGetValue(new ValueKey(variationId, optionId), out value);
		}

		/// <summary>
		/// Delete variation by ID.
		/// </summary>
		/// <param name="id">ID.</param>
		/// <returns>true if variation was deleted; otherwise false.</returns>
		public bool DeleteVariation(VariationId id)
		{
			var total = 0;
			for (var i = values.Count - 1; i >= 0; i--)
			{
				var v = values[i];
				if (v.VariationId == id)
				{
					values.RemoveAt(i);
					Cache.Remove(v.Key);
					total += 1;
				}
			}

			return total > 0;
		}

		/// <summary>
		/// Copy variation data.
		/// </summary>
		/// <param name="sourceId">Source ID.</param>
		/// <param name="destinationId">Destination ID.</param>
		public void Copy(VariationId sourceId, VariationId destinationId)
		{
			foreach (var option in options)
			{
				var value = Get(sourceId, option.Id);
				Set(destinationId, option.Id, value);
			}
		}

		/// <summary>
		/// Get next option ID.
		/// </summary>
		/// <returns>Option ID.</returns>
		protected OptionId NextOptionId()
		{
			var id = 0;
			foreach (var s in options)
			{
				id = Mathf.Max(id, s.Id.Id);
			}

			return new OptionId(id + 1);
		}

		/// <summary>
		/// Add option.
		/// </summary>
		/// <param name="name">Option name.</param>
		/// <returns>Option.</returns>
		public Option AddOption(string name)
		{
			var options = new Option(NextOptionId(), name);
			this.options.Add(options);

			return options;
		}

		/// <summary>
		/// Delete option by ID.
		/// </summary>
		/// <param name="id">ID.</param>
		/// <returns>true if option was deleted; otherwise false.</returns>
		public bool DeleteOption(OptionId id)
		{
			var option = GetOption(id);
			if (option == null)
			{
				return false;
			}

			var total = options.Remove(option) ? 1 : 0;
			for (var i = values.Count - 1; i >= 0; i--)
			{
				var v = values[i];
				if (v.OptionId == id)
				{
					values.RemoveAt(i);
					Cache.Remove(v.Key);
					total += 1;
				}
			}

			return total > 0;
		}

		/// <summary>
		/// Get option by name.
		/// </summary>
		/// <param name="name">Option name.</param>
		/// <returns>Option.</returns>
		public Option GetOption(string name)
		{
			foreach (var s in options)
			{
				if (s.Name == name)
				{
					return s;
				}
			}

			return null;
		}

		/// <summary>
		/// Get option by ID.
		/// </summary>
		/// <param name="optionId">Option ID.</param>
		/// <returns>Option.</returns>
		public Option GetOption(OptionId optionId)
		{
			foreach (var s in options)
			{
				if (s.Id == optionId)
				{
					return s;
				}
			}

			return null;
		}

		/// <summary>
		/// Has option with the specified name.
		/// </summary>
		/// <param name="name">Option name.</param>
		/// <returns>true if has option with the specified name; otherwise false.</returns>
		public bool HasOption(string name) => GetOption(name) != null;

		/// <summary>
		/// Has option with the specified ID.
		/// </summary>
		/// <param name="optionId">Option ID.</param>
		/// <returns>true if has option with the specified ID; otherwise false.</returns>
		public bool HasOption(OptionId optionId) => GetOption(optionId) != null;

		/// <summary>
		/// Clear cache.
		/// </summary>
		public void ClearCache() => Cache.Clear();

		/// <summary>
		/// Find option with the specified value.
		/// </summary>
		/// <param name="variationId">Variation ID.</param>
		/// <param name="value">Value.</param>
		/// <returns>Option ID.</returns>
		public OptionId FindOption(VariationId variationId, TValue value)
		{
			foreach (var option in options)
			{
				if (!TryGetValue(variationId, option.Id, out var optionValue))
				{
					continue;
				}

				if (Comparer.Equals(optionValue, value))
				{
					return option.Id;
				}
			}

			return OptionId.None;
		}

		/// <summary>
		/// Find option or create option with the specified value.
		/// </summary>
		/// <param name="variationId">Variation ID.</param>
		/// <param name="value">Value.</param>
		/// <param name="optionName">Option name.</param>
		/// <param name="created">Option was created.</param>
		/// <returns>Option ID.</returns>
		public OptionId RequireOption(VariationId variationId, TValue value, string optionName, out bool created)
		{
			var option = FindOption(variationId, value);
			if (option != OptionId.None)
			{
				created = false;
				return option;
			}

			var i = 2;
			var name = optionName;
			while (HasOption(name))
			{
				name = optionName + " " + i;
				i++;
			}

			var new_option = AddOption(name);
			Set(variationId, new_option.Id, value);
			created = true;

			return new_option.Id;
		}

#if UITHEMES_ADDRESSABLE_SUPPORT
		/// <summary>
		/// Set addresses for the addressable assets.
		/// </summary>
		/// <param name="object2address">Get address for the specified object.</param>
		/// <param name="resetValues">Reset values.</param>
		public void EnsureAddressable(Func<UnityEngine.Object, AddressableAsset> object2address, bool resetValues)
		{
			if (!IsUnityObject)
			{
				return;
			}

			foreach (var v in values)
			{
				v.SetAddress(object2address(v.Value as UnityEngine.Object), resetValues);
			}
		}

		/// <summary>
		/// Preload addressable assets.
		/// </summary>
		/// <param name="variationId">Load assets only for the specified variationId.</param>
		/// <returns>Task.</returns>
		public Task PreloadAddressable(VariationId? variationId = null)
		{
			#if UNITY_EDITOR
			foreach (var v in values)
			{
				var valid_variation = variationId.HasValue && (variationId.Value == v.VariationId);
				var load = valid_variation && v.LoadRequired;
				if (!load)
				{
					continue;
				}

				var ignore = v.Value;
			}
			return null;
			#else
			if (!IsUnityObject)
			{
				return null;
			}

			var tasks = new List<Task>();

			foreach (var v in values)
			{
				var valid_variation = variationId.HasValue && (variationId.Value == v.VariationId);
				var load = valid_variation && v.LoadRequired;
				if (!load)
				{
					continue;
				}

				var op = UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<TValue>(v.Address);
				op.Task.GetAwaiter().OnCompleted(() => v.Value = op.Task.Result);
				tasks.Add(op.Task);
			}

			return Task.WhenAll(tasks);
			#endif
		}
#endif
	}
}