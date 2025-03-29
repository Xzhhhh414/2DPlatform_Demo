#if UNITY_EDITOR
namespace EasyLayoutNS
{
	using System;
	using System.Collections.Generic;
	using UnityEditor;

	/// <summary>
	/// Property drawer for the EasyLayoutEllipseSettings.
	/// </summary>
	[CustomPropertyDrawer(typeof(EasyLayoutEllipseSettings))]
	public class EasyLayoutEllipseSettingsPropertyDrawer : ConditionalPropertyDrawer
	{
		static readonly Dictionary<string, Func<SerializedProperty, bool>> isNotWidthAuto = new Dictionary<string, Func<SerializedProperty, bool>>()
		{
			{ "widthAuto", x => !x.boolValue },
		};

		static readonly Dictionary<string, Func<SerializedProperty, bool>> isNotHeightAuto = new Dictionary<string, Func<SerializedProperty, bool>>()
		{
			{ "heightAuto", x => !x.boolValue },
		};

		static readonly Dictionary<string, Func<SerializedProperty, bool>> isAngleAuto = new Dictionary<string, Func<SerializedProperty, bool>>()
		{
			{ "angleStepAuto", x => x.boolValue },
		};

		static readonly Dictionary<string, Func<SerializedProperty, bool>> isNotAngleAuto = new Dictionary<string, Func<SerializedProperty, bool>>()
		{
			{ "angleStepAuto", x => !x.boolValue },
		};

		static readonly Dictionary<string, Func<SerializedProperty, bool>> isRotate = new Dictionary<string, Func<SerializedProperty, bool>>()
		{
			{ "elementsRotate", x => x.boolValue },
		};

		/// <summary>
		/// Init this instance.
		/// </summary>
		protected override void Init()
		{
			if (Fields != null)
			{
				return;
			}

			Fields = new List<ConditionalFieldInfo>()
			{
				new ConditionalFieldInfo("widthAuto"),
				new ConditionalFieldInfo("width", 1, isNotWidthAuto),
				new ConditionalFieldInfo("heightAuto"),
				new ConditionalFieldInfo("height", 1, isNotHeightAuto),
				new ConditionalFieldInfo("angleStart"),
				new ConditionalFieldInfo("angleStepAuto"),
				new ConditionalFieldInfo("angleStep", 1, isNotAngleAuto),
				new ConditionalFieldInfo("fill", 1, isAngleAuto),
				new ConditionalFieldInfo("arcLength", 1),
				new ConditionalFieldInfo("align"),
				new ConditionalFieldInfo("elementsRotate"),
				new ConditionalFieldInfo("elementsRotationStart", 1, isRotate),
			};
		}
	}
}
#endif