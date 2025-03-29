namespace UIThemes
{
	using System;
	using System.Reflection;

	/// <summary>
	/// Property group attribute.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class PropertyGroupAttribute : Attribute
	{
		/// <summary>
		/// Undo name.
		/// </summary>
		public readonly string UndoName;

		/// <summary>
		/// Value type.
		/// </summary>
		public readonly Type ValueType;

		/// <summary>
		/// Constructor.
		/// </summary>
		public readonly ConstructorInfo Constructor;

		/// <summary>
		/// Initializes a new instance of the <see cref="PropertyGroupAttribute"/> class.
		/// </summary>
		/// <param name="fieldView">Type of the field view.</param>
		/// <param name="undoName">Undo name.</param>
		public PropertyGroupAttribute(Type fieldView, string undoName)
		{
			UndoName = undoName;

			ValueType = GetValueType(fieldView);

			if (ValueType != null)
			{
				Constructor = GetConstructor(fieldView);
			}
		}

		Type GetValueType(Type type)
		{
			if (!typeof(FieldViewBase).IsAssignableFrom(type))
			{
				return null;
			}

			while (type != null)
			{
				if (type.IsGenericType)
				{
					var args = type.GenericTypeArguments;
					if (args.Length == 1 && typeof(FieldView<>).MakeGenericType(args) == type)
					{
						return args[0];
					}
				}

				type = type.BaseType;
			}

			return null;
		}

		ConstructorInfo GetConstructor(Type type)
		{
			var args = new[]
			{
				typeof(string),
				typeof(Theme.ValuesWrapper<>).MakeGenericType(ValueType),
			};

			var flags = BindingFlags.Instance | BindingFlags.Public;
			return type.GetConstructor(flags, null, CallingConventions.HasThis, args, null);
		}
	}
}