namespace UIThemes.Wrappers
{
	using System;
	using UnityEngine;

	/// <summary>
	/// Wrapper validation.
	/// </summary>
	public interface IWrapperValidation
	{
		/// <summary>
		/// Is property/field type is same as type of wrapper value?
		/// </summary>
		Func<Component, bool> IsValidType
		{
			get;
		}
	}
}