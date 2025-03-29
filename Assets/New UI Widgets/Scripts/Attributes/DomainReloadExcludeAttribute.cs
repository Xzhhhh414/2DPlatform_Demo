namespace UIWidgets.Attributes
{
	using System;

	/// <summary>
	/// Mark static fields that does not need to reset for domain reload support.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Event)]
	public sealed class DomainReloadExcludeAttribute : Attribute
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DomainReloadExcludeAttribute"/> class.
		/// </summary>
		public DomainReloadExcludeAttribute()
		{
		}
	}
}