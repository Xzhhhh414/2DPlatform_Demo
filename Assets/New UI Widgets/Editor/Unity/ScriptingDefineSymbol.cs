#if UNITY_EDITOR
namespace UIWidgets
{
	using System;

	/// <summary>
	/// Scripting Define Symbol.
	/// </summary>
	public class ScriptingDefineSymbol : ISetting
	{
		/// <summary>
		/// Symbol.
		/// </summary>
		public string Symbol
		{
			get;
			private set;
		}

		/// <summary>
		/// Required package is installed.
		/// </summary>
		public bool Available
		{
			get;
			private set;
		}

		/// <summary>
		/// State.
		/// </summary>
		public readonly ScriptingDefineSymbols.State State;

		bool enabled;

		/// <summary>
		/// Enabled.
		/// </summary>
		public bool Enabled
		{
			get => enabled;

			set
			{
				if (enabled == value)
				{
					return;
				}

				enabled = value;

				if (enabled)
				{
					ScriptingDefineSymbols.Add(Symbol);
				}
				else
				{
					ScriptingDefineSymbols.Remove(Symbol);
				}

				OnChanged?.Invoke(enabled);
			}
		}

		/// <summary>
		/// Status text.
		/// </summary>
		public string Status
		{
			get
			{
				if (!Available)
				{
					return "Not installed";
				}

				return Enabled ? EnabledText : DisabledText;
			}
		}

		/// <summary>
		/// Is support enabled for all BuildTargets?
		/// </summary>
		public bool IsFullSupport => !State.HasMissing;

		/// <summary>
		/// Action on enable/disable.
		/// </summary>
		protected Action<bool> OnChanged;

		/// <summary>
		/// Status text when symbol is enabled.
		/// </summary>
		protected string EnabledText = "Enabled";

		/// <summary>
		/// Status text when symbol is disabled.
		/// </summary>
		protected string DisabledText = "Disabled";

		/// <summary>
		/// Initializes a new instance of the <see cref="ScriptingDefineSymbol"/> class.
		/// </summary>
		/// <param name="symbol">Scripting Define Symbol.</param>
		/// <param name="installed">Required package is installed.</param>
		/// <param name="onChanged">Action on enable/disable.</param>
		/// <param name="enabledText">Status text when symbol is enabled.</param>
		/// <param name="disabledText">Status text when symbol is disabled.</param>
		public ScriptingDefineSymbol(string symbol, bool installed, Action<bool> onChanged = null, string enabledText = "Enabled", string disabledText = "Disabled")
		{
			Symbol = symbol;
			Available = installed;
			OnChanged = onChanged;
			EnabledText = enabledText;
			DisabledText = disabledText;

			State = ScriptingDefineSymbols.GetState(Symbol);
			enabled = Available && State.Has > 0;
		}

		/// <summary>
		/// Enabled symbol for all target groups.
		/// </summary>
		public void EnableForAll()
		{
			enabled = true;
			ScriptingDefineSymbols.Add(Symbol);
			OnChanged?.Invoke(enabled);
		}
	}
}
#endif