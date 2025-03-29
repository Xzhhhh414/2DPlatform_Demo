namespace UIThemes
{
	using System;
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// Theme.
	/// </summary>
	public partial class Theme : ScriptableObject
	{
		/// <summary>
		/// Theme values wrapper.
		/// </summary>
		public interface IValuesWrapper
		{
			/// <summary>
			/// Theme.
			/// </summary>
			Theme Theme
			{
				get;
			}

			/// <summary>
			/// Options.
			/// </summary>
			IReadOnlyList<Option> Options
			{
				get;
			}

			/// <summary>
			/// Value type.
			/// </summary>
			Type ValueType
			{
				get;
			}

			/// <summary>
			/// Has value.
			/// </summary>
			/// <param name="variationId">Variation ID.</param>
			/// <param name="optionId">Option ID.</param>
			/// <returns>true if has value; otherwise false.</returns>
			bool HasValue(VariationId variationId, OptionId optionId);

			/// <summary>
			/// Delete variation by ID.
			/// </summary>
			/// <param name="id">ID.</param>
			/// <returns>true if variation was deleted; otherwise false.</returns>
			bool DeleteVariation(VariationId id);

			/// <summary>
			/// Delete option by ID.
			/// </summary>
			/// <param name="id">ID.</param>
			/// <returns>true if option was deleted; otherwise false.</returns>
			bool DeleteOption(OptionId id);

			/// <summary>
			/// Move option.
			/// </summary>
			/// <param name="oldIndex">Old index.</param>
			/// <param name="newIndex">New index.</param>
			/// <returns>true if option was moved; otherwise false.</returns>
			bool MoveOption(int oldIndex, int newIndex);

			/// <summary>
			/// Add option.
			/// </summary>
			/// <param name="name">Option name.</param>
			/// <returns>Option.</returns>
			Option AddOption(string name);

			/// <summary>
			/// Get option by name.
			/// </summary>
			/// <param name="name">Option name.</param>
			/// <returns>Option.</returns>
			Option GetOption(string name);

			/// <summary>
			/// Get option by ID.
			/// </summary>
			/// <param name="optionId">Option ID.</param>
			/// <returns>Option.</returns>
			Option GetOption(OptionId optionId);

			/// <summary>
			/// Has option with the specified name.
			/// </summary>
			/// <param name="name">Option name.</param>
			/// <returns>true if has option with the specified name; otherwise false.</returns>
			bool HasOption(string name);

			/// <summary>
			/// Has option with the specified ID.
			/// </summary>
			/// <param name="optionId">Option ID.</param>
			/// <returns>true if has option with the specified ID; otherwise false.</returns>
			bool HasOption(OptionId optionId);
		}
	}
}