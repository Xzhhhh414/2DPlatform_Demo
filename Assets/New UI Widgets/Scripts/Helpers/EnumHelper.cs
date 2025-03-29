namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using UIWidgets.Attributes;

	/// <summary>
	/// Enum helper.
	/// </summary>
	/// <typeparam name="T">Type of the enum.</typeparam>
	public static class EnumHelper<T>
#if CSHARP_7_3_OR_NEWER
		where T : struct, Enum
#else
		where T : struct
#endif
	{
		[DomainReloadExclude]
		private static readonly Type EnumType = typeof(T);

		[DomainReloadExclude]
		private static readonly object Sync = new object();

		/// <summary>
		/// Is enum has [Flags] attribute.
		/// </summary>
		[DomainReloadExclude]
		public static readonly bool IsFlags = GetIsFlags();

		[DomainReloadExclude]
		private static long[] valuesLong;

		/// <summary>
		/// Values converted to long.
		/// </summary>
		public static long[] ValuesLong
		{
			get
			{
				if (valuesLong == null)
				{
					valuesLong = new long[Values.Length];
					for (int i = 0; i < values.Length; i++)
					{
						valuesLong[i] = Convert.ToInt64(values[i]);
					}
				}

				return valuesLong;
			}
		}

		[DomainReloadExclude]
		private static T[] values;

		/// <summary>
		/// Values.
		/// </summary>
		public static T[] Values
		{
			get
			{
				values ??= GetValues();

				return values;
			}
		}

		[DomainReloadExclude]
		private static string[] names;

		/// <summary>
		/// Names.
		/// </summary>
		public static string[] Names
		{
			get
			{
				names ??= GetNames();

				return names;
			}
		}

		[DomainReloadExclude]
		private static Dictionary<T, string> value2Name;

		private static Dictionary<T, string> Value2Name
		{
			get
			{
				value2Name ??= GetValue2Name();

				return value2Name;
			}
		}

		[DomainReloadExclude]
		private static bool[] obsolete;

		/// <summary>
		/// Obsolete values.
		/// </summary>
		public static bool[] Obsolete
		{
			get
			{
				obsolete ??= GetObsolete();

				return obsolete;
			}
		}

		private static T[] GetValues()
		{
			lock (Sync)
			{
				return (T[])Enum.GetValues(EnumType);
			}
		}

		private static bool[] GetObsolete()
		{
			var names = Names;

			lock (Sync)
			{
				var result = new bool[names.Length];
				for (int i = 0; i < names.Length; i++)
				{
					var fi = EnumType.GetField(names[i]);
					var attributes = (ObsoleteAttribute[])fi.GetCustomAttributes(typeof(ObsoleteAttribute), false);
					result[i] = (attributes != null) && (attributes.Length > 0);
				}

				return result;
			}
		}

		private static string[] GetNames()
		{
			lock (Sync)
			{
				return Enum.GetNames(EnumType);
			}
		}

		private static Dictionary<T, string> GetValue2Name()
		{
			lock (Sync)
			{
				var result = value2Name;
				if (result != null)
				{
					return result;
				}

				result = new Dictionary<T, string>(Names.Length, EqualityComparer<T>.Default);
				for (int i = 0; i < Values.Length; i++)
				{
					if (!result.ContainsKey(values[i]))
					{
						result.Add(values[i], names[i]);
					}
				}

				return result;
			}
		}

		private static bool GetIsFlags()
		{
			return EnumType.IsEnum && EnumType.IsDefined(typeof(FlagsAttribute), false);
		}

		/// <summary>
		/// Check is value contains flag.
		/// </summary>
		/// <param name="value">Value.</param>
		/// <param name="flag">Flag.</param>
		/// <returns>true if value contains flag; otherwise false.</returns>
		public static bool HasFlag(T value, T flag)
		{
			var value_long = Convert.ToInt64(value);
			var flag_long = Convert.ToInt64(flag);
			return (value_long & flag_long) == flag_long;
		}

		/// <summary>
		/// Convert enum value to the string.
		/// </summary>
		/// <param name="value">Value.</param>
		/// <returns>String representation of the value.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0102:Non-overridden virtual method call on value type", Justification = "Temporaty.")]
		public static string ToString(T value)
		{
			if (Value2Name.TryGetValue(value, out var name))
			{
				return name;
			}

			// optional: flags version
			// optional: int conversion
			return value.ToString();
		}
	}
}