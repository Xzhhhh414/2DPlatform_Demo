namespace UIWidgets.Attributes
{
	using System;

	/// <summary>
	/// Mark StaticInit() method and specify static fields that are reseted in that method.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public sealed class DomainReloadAttribute : Attribute
	{
		/// <summary>
		/// The fields.
		/// </summary>
		readonly string[] fields;

		/// <summary>
		/// The fields.
		/// </summary>
		public string[] Fields
		{
			get
			{
				return fields;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DomainReloadAttribute"/> class.
		/// </summary>
		/// <param name="fields">Fields.</param>
		public DomainReloadAttribute(params string[] fields)
		{
			this.fields = fields;
		}
	}
}