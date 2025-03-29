namespace UIWidgets
{
	using System;
	using UnityEngine;
	using UnityEngine.Events;
	using UnityEngine.UI;

	/// <summary>
	/// InputFieldProxy.
	/// </summary>
	public class InputFieldNull : IInputFieldProxy
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="UIWidgets.InputFieldNull"/> class.
		/// </summary>
		public InputFieldNull()
		{
		}

		#region IInputFieldProxy implementation

		/// <summary>
		/// The current value of the input field.
		/// </summary>
		/// <value>The text.</value>
		public string text
		{
			get => null;

			set
			{
			}
		}

		/// <summary>
		/// Is InputField has rich text support?
		/// </summary>
		public bool richText => false;

		readonly InputField.OnChangeEvent changeEvent = new InputField.OnChangeEvent();

		/// <summary>
		/// The Unity Event to call when edit.
		/// </summary>
		/// <value>The OnValueChange.</value>
		public UnityEvent<string> onValueChanged => changeEvent;

		readonly InputField.SubmitEvent submitEvent = new InputField.SubmitEvent();

		/// <summary>
		/// The Unity Event to call when editing has ended.
		/// </summary>
		/// <value>The OnEndEdit.</value>
		public UnityEvent<string> onEndEdit => submitEvent;

		/// <summary>
		/// Current InputField caret position (also selection tail).
		/// </summary>
		/// <value>The caret position.</value>
		public int caretPosition
		{
			get => 0;

			set
			{
			}
		}

		/// <summary>
		/// Is the InputField eligible for interaction (excludes canvas groups).
		/// </summary>
		/// <value>true</value>
		/// <c>false</c>
		public bool interactable
		{
			get => true;

			set
			{
			}
		}

		/// <summary>
		/// Determines whether InputField instance is null.
		/// </summary>
		/// <returns>true</returns>
		/// <c>false</c>
		public bool IsNull() => true;

		/// <summary>
		/// Gets the game object.
		/// </summary>
		/// <value>The game object.</value>
		public GameObject gameObject => null;

		/// <summary>
		/// Text component.
		/// </summary>
		public Graphic textComponent
		{
			get => null;

			set
			{
			}
		}

		/// <summary>
		/// Placeholder.
		/// </summary>
		public Graphic placeholder
		{
			get => null;

			set
			{
			}
		}

		/// <summary>
		/// If the proxy was canceled and will revert back to the original text.
		/// </summary>
		public bool wasCanceled => false;

		/// <summary>
		/// Determines whether this lineType is LineType.MultiLineNewline.
		/// </summary>
		/// <returns>true</returns>
		/// <c>false</c>
		public bool IsMultiLineNewline() => false;

		/// <summary>
		/// Function to activate the InputField to begin processing Events.
		/// </summary>
		public void Activate()
		{
		}

		/// <summary>
		/// Deactivate InputField.
		/// </summary>
		public void Deactivate()
		{
		}

#if UNITY_4_6 || UNITY_4_7 || UNITY_5_0
		/// <summary>
		/// Move caret to end.
		/// </summary>
		public void MoveToEnd()
		{
		}
#endif

		/// <summary>
		/// Set focus to InputField.
		/// </summary>
		public void Focus()
		{
		}

		/// <summary>
		/// Set focus to InputField.
		/// </summary>
		public void Select()
		{
		}

		/// <summary>
		/// Set text without raising onValueChanged event.
		/// </summary>
		/// <param name="text">Text.</param>
		public void SetTextWithoutNotify(string text)
		{
		}

		#endregion
	}
}