namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using UIWidgets.Attributes;
	using UIWidgets.Pool;

	/// <summary>
	/// ListViewEnum typed wrapper.
	/// </summary>
	/// <typeparam name="T">Type of enum.</typeparam>
	public class ListViewEnum<T>
		#if CSHARP_7_3_OR_NEWER
		where T : struct, Enum
		#else
		where T : struct
		#endif
	{
		/// <summary>
		/// ListView.
		/// </summary>
		protected ListViewEnum ListView;

		/// <summary>
		/// Selected value.
		/// </summary>
		public T Selected
		{
			get
			{
				long result = 0;

				using var _ = ListPool<int>.Get(out var indices);
				ListView.GetSelectedIndices(indices);
				foreach (var index in indices)
				{
					result += ListView.DataSource[index].Value;
				}

				return Long2Enum(result);
			}

			set
			{
				ListView.DeselectAll();

				var long_value = Enum2Long(value);
				for (int index = 0; index < ListView.DataSource.Count; index++)
				{
					var item = ListView.DataSource[index];
					if (ListView.MultipleSelect && (item.Value == 0))
					{
						continue;
					}

					if ((long_value & item.Value) > 0)
					{
						long_value ^= item.Value;
						ListView.Select(index);
					}
				}
			}
		}

		readonly Func<long, T> Long2Enum;

		[DomainReloadExclude]
		static readonly Func<long, T> DefaultLong2Enum = value => (T)Enum.ToObject(typeof(T), value);

		readonly Func<T, long> Enum2Long;

		[DomainReloadExclude]
		static readonly Func<T, long> DefaultEnum2Long = value => Convert.ToInt64(value);

		/// <summary>
		/// Initializes a new instance of the <see cref="ListViewEnum{T}"/> class.
		/// </summary>
		/// <param name="listView">ListView.</param>
		/// <param name="showObsolete">Show obsolete values.</param>
		/// <param name="long2enum">Long to enum converter.</param>
		/// <param name="enum2long">Enum to long converter.</param>
		public ListViewEnum(ListViewEnum listView, bool showObsolete = false, Func<long, T> long2enum = null, Func<T, long> enum2long = null)
		{
#if !CSHARP_7_3_OR_NEWER
			var type = typeof(T);
			if (!type.IsEnum)
			{
				throw new ArgumentException(string.Format("{0} is not enum type.", type));
			}
#endif

			Long2Enum = long2enum ?? DefaultLong2Enum;
			Enum2Long = enum2long ?? DefaultEnum2Long;

			ListView = listView;
			ListView.Init();

			ListView.DeselectAll();
			ListView.MultipleSelect = EnumHelper<T>.IsFlags;

			var items = ListView.DataSource;
			items.Clear();
			using var _ = items.BeginUpdate();

			var ints = EnumHelper<T>.ValuesLong;
			var names = EnumHelper<T>.Names;
			var obsolete = EnumHelper<T>.Obsolete;

			for (int i = 0; i < ints.Length; i++)
			{
				if (!showObsolete && obsolete[i])
				{
					continue;
				}

				items.Add(new ListViewEnum.Item(ints[i], names[i], obsolete[i]));
			}
		}
	}
}