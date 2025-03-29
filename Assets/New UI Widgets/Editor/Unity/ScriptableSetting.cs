#if UNITY_EDITOR
namespace UIWidgets
{
	using System;

	/// <summary>
	/// Project setting stored in ScriptableObject.
	/// </summary>
	public class ScriptableSetting : ISetting
	{
		/// <summary>
		/// Status.
		/// </summary>
		public string Status => Enabled ? EnabledText : DisabledText;

		/// <summary>
		/// Available.
		/// </summary>
		public bool Available => true;

		/// <summary>
		/// Is support enabled for all BuildTargets?
		/// </summary>
		public bool IsFullSupport => true;

		/// <summary>
		/// Is enabled?
		/// </summary>
		public bool Enabled
		{
			get => EnabledGetter();
			set => EnabledSetter(value);
		}

		/// <summary>
		/// Status text when symbol is enabled.
		/// </summary>
		protected string EnabledText = "Enabled";

		/// <summary>
		/// Status text when symbol is disabled.
		/// </summary>
		protected string DisabledText = "Disabled";

		/// <summary>
		/// Enabled getter.
		/// </summary>
		protected Func<bool> EnabledGetter;

		/// <summary>
		/// Enabled setter.
		/// </summary>
		protected Action<bool> EnabledSetter;

		/// <summary>
		/// Initializes a new instance of the <see cref="ScriptableSetting"/> class.
		/// </summary>
		/// <param name="enabledGetter">Enabled getter.</param>
		/// <param name="enabledSetter">Enabled Setter.</param>
		/// <param name="enabledText">Status text if enabled.</param>
		/// <param name="disabledText">Status text if disabled.</param>
		public ScriptableSetting(Func<bool> enabledGetter, Action<bool> enabledSetter, string enabledText = "Enabled", string disabledText = "Disabled")
		{
			EnabledGetter = enabledGetter;
			EnabledSetter = enabledSetter;
			EnabledText = enabledText;
			DisabledText = disabledText;
		}

		/// <summary>
		/// Enabled for all BuildTargets.
		/// </summary>
		public void EnableForAll()
		{
			Enabled = true;
		}
	}
}
#endif