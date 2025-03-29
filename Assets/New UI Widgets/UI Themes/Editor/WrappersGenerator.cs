#if UNITY_EDITOR
namespace UIThemes.Editor
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using UIThemes.Wrappers;
	using UnityEditor;
	using UnityEngine;

	/// <summary>
	/// Wrappers scripts generator.
	/// </summary>
	public class WrappersGenerator : IFormattable
	{
		readonly string folder;

		readonly string wrappersNamespace;

		readonly IReadOnlyDictionary<Type, IReadOnlyCollection<string>> wrappers;

		readonly List<string> classes = new List<string>();

		Type currentComponent;

		string currentField;

		Type currentFieldType;

		/// <summary>
		/// Initializes a new instance of the <see cref="WrappersGenerator"/> class.
		/// </summary>
		/// <param name="folder">Folder.</param>
		/// <param name="wrappersNamespace">Wrappers namespace.</param>
		/// <param name="wrappers">Required wrappers data.</param>
		public WrappersGenerator(string folder, string wrappersNamespace, IReadOnlyDictionary<Type, IReadOnlyCollection<string>> wrappers)
		{
			this.folder = folder;
			this.wrappersNamespace = wrappersNamespace;
			this.wrappers = wrappers;
		}

		/// <summary>
		/// Run wrappers scripts generator.
		/// </summary>
		public void Run()
		{
			if (!Directory.Exists(folder))
			{
				Debug.LogErrorFormat("Folder \"{0}\" is not exist.", folder);
				return;
			}

			var wrapper_template = ReferencesGUIDs.WrapperTemplate;
			if (string.IsNullOrEmpty(ReferencesGUIDs.WrapperTemplate))
			{
				Debug.LogError("Wrapper script template is not found.");
				return;
			}

			var registry_template = ReferencesGUIDs.RegistryTemplate;
			if (string.IsNullOrEmpty(ReferencesGUIDs.RegistryTemplate))
			{
				Debug.LogError("Wrappers registry script template is not found.");
				return;
			}

			LoadExistingWrappers();

			CreateWrappers(wrapper_template);

			if (classes.Count > 0)
			{
				CreateRegistry(registry_template);
				AssetDatabase.Refresh();
			}
		}

		void AddClass(string classname, Type fieldType)
		{
			var line = string.Format("\t\t\tUIThemes.PropertyWrappers<{0}>.Add(new {1}());", UtilitiesEditor.GetFriendlyTypeName(fieldType), classname);
			classes.Add(line);
		}

		void LoadExistingWrappers()
		{
			foreach (var file in Directory.GetFiles(folder, "*.cs"))
			{
				var (classname, field_type) = File2Wrapper(file);
				if (!string.IsNullOrEmpty(classname))
				{
					AddClass(classname, field_type);
				}
			}
		}

		(string Classname, Type FieldType) File2Wrapper(string file)
		{
			var script = AssetDatabase.LoadAssetAtPath<MonoScript>(file);
			if ((script != null) && IsWrapperType(script.GetClass(), out var fieldType))
			{
				return (script.GetClass().Name, fieldType);
			}

			var classname = Path.GetFileNameWithoutExtension(file);
			var typename = string.Format("{0}.{1}", wrappersNamespace, classname);
			var type = UtilitiesEditor.GetType(typename);
			if (IsWrapperType(type, out fieldType))
			{
				return (classname, fieldType);
			}

			return (null, null);
		}

		bool IsWrapperType(Type wrapper, out Type fieldType)
		{
			fieldType = null;
			if (wrapper == null)
			{
				return false;
			}

			foreach (var type in wrapper.GetInterfaces())
			{
				if (type.IsConstructedGenericType && type.GetGenericTypeDefinition() == typeof(IWrapper<>))
				{
					fieldType = type.GetGenericArguments()[0];
					return true;
				}
			}

			return false;
		}

		void CreateRegistry(string template)
		{
			var code = string.Format(template, this);
			var path = Path.Combine(folder, "PropertyWrappersRegistry.cs");
			File.WriteAllText(path, code);
		}

		void CreateWrappers(string template)
		{
			foreach (var wrapper in wrappers)
			{
				currentComponent = wrapper.Key;

				foreach (var field in wrapper.Value)
				{
					currentField = field;
					currentFieldType = GetFieldType(currentComponent, currentField);
					if (currentFieldType == null)
					{
						continue;
					}

					var classname = string.Format("{0}{1}", currentComponent.Name, currentField);
					var code = string.Format(template, this);
					var path = Path.Combine(folder, string.Format("{0}.cs", classname));
					File.WriteAllText(path, code);

					AddClass(classname, currentFieldType);
				}
			}
		}

		Type GetFieldType(Type type, string memberName)
		{
			var field_info = type.GetField(memberName);
			if (field_info != null)
			{
				return field_info.FieldType;
			}

			var property_info = type.GetProperty(memberName);
			if (property_info != null)
			{
				return property_info.PropertyType;
			}

			return null;
		}

		/// <summary>
		/// Formats the value of the current instance using the specified format.
		/// </summary>
		/// <param name="format">The format to use.</param>
		/// <param name="formatProvider">The provider to use to format the value.</param>
		/// <returns>The value of the current instance in the specified format.</returns>
		public string ToString(string format, IFormatProvider formatProvider)
		{
			return format switch
			{
				"Namespace" => wrappersNamespace,
				"WrapperClass" => string.Format("{0}{1}", currentComponent.Name, currentField),
				"Field" => currentField,
				"FieldType" => UtilitiesEditor.GetFriendlyTypeName(currentFieldType),
				"ComponentType" => UtilitiesEditor.GetFriendlyTypeName(currentComponent),
				"Wrappers" => string.Join("\r\n", classes.ToArray()),
				_ => throw new ArgumentOutOfRangeException("Unsupported format: " + format),
			};
		}
	}
}
#endif