#if UNITY_EDITOR
namespace UIWidgets
{
	/// <summary>
	/// Project setting option.
	/// </summary>
	public interface ISetting
	{
		/// <summary>
		/// Status.
		/// </summary>
		string Status { get; }

		/// <summary>
		/// Available.
		/// </summary>
		bool Available { get; }

		/// <summary>
		/// Is support enabled for all BuildTargets?
		/// </summary>
		bool IsFullSupport { get; }

		/// <summary>
		/// Is enabled?
		/// </summary>
		bool Enabled { get; set; }

		/// <summary>
		/// Enabled for all BuildTargets.
		/// </summary>
		void EnableForAll();
	}
}
#endif