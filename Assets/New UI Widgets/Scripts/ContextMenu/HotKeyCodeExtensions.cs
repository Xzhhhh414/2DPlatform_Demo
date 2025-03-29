namespace UIWidgets.Menu
{
	/// <summary>
	/// HotKeyCode extensions.
	/// </summary>
	public static class HotKeyCodeExtensions
	{
		/// <summary>
		/// Convert HotKeyCode to human-readable string.
		/// </summary>
		/// <param name="code">Code.</param>
		/// <returns>Human-readable string.</returns>
		public static string ToHumanString(this HotKeyCode code)
		{
			return code switch
			{
				HotKeyCode.Quote => "'",
				HotKeyCode.Comma => ",",
				HotKeyCode.Minus => "-",
				HotKeyCode.Period => ".",
				HotKeyCode.Slash => "/",
				HotKeyCode.Semicolon => ";",
				HotKeyCode.Equals => "=",
				HotKeyCode.Backslash => "\\",
				HotKeyCode.LeftBracket => "[",
				HotKeyCode.RightBracket => "]",
				HotKeyCode.Alpha0 => "0",
				HotKeyCode.Alpha1 => "1",
				HotKeyCode.Alpha2 => "2",
				HotKeyCode.Alpha3 => "3",
				HotKeyCode.Alpha4 => "4",
				HotKeyCode.Alpha5 => "5",
				HotKeyCode.Alpha6 => "6",
				HotKeyCode.Alpha7 => "7",
				HotKeyCode.Alpha8 => "8",
				HotKeyCode.Alpha9 => "9",
				HotKeyCode.Multiply => "*",
				HotKeyCode.Plus => "Num+",
				HotKeyCode.UpArrow => "↑",
				HotKeyCode.DownArrow => "↓",
				HotKeyCode.RightArrow => "→",
				HotKeyCode.LeftArrow => "←",
				_ => EnumHelper<HotKeyCode>.ToString(code),
			};
		}
	}
}