#if UNITY_EDITOR
namespace EasyLayoutNS
{
	using System;
	using System.Collections.Generic;
	using UnityEditor;

	/// <summary>
	/// Property drawer for the EasyLayoutHexSettings.
	/// </summary>
	[CustomPropertyDrawer(typeof(EasyLayoutHexSettings))]
	public class EasyLayoutHexSettingsPropertyDrawer : ConditionalPropertyDrawer
	{
		static readonly  Func<SerializedProperty, bool> isWriteCoordinate = x => (EasyLayoutHexSettings.CoordinatesMode)x.enumValueIndex == EasyLayoutHexSettings.CoordinatesMode.Write;

		static readonly Dictionary<string, Func<SerializedProperty, bool>> isWriteCoordinates = new Dictionary<string, Func<SerializedProperty, bool>>()
		{
			{ "coordinates", isWriteCoordinate },
		};

		static readonly Dictionary<string, Func<SerializedProperty, bool>> isNotFlexible = new Dictionary<string, Func<SerializedProperty, bool>>()
		{
			{ "coordinates", isWriteCoordinate },
			{ "constraint", x => (EasyLayoutHexSettings.HexConstraints)x.enumValueIndex != EasyLayoutHexSettings.HexConstraints.Flexible },
		};

		static readonly Dictionary<string, Func<SerializedProperty, bool>> isItemsPerN = new Dictionary<string, Func<SerializedProperty, bool>>()
		{
			{ "coordinates", isWriteCoordinate },
			{
				"constraint",
				x => {
					var v = (EasyLayoutHexSettings.HexConstraints)x.enumValueIndex;
					return (v == EasyLayoutHexSettings.HexConstraints.CellsPerRow) || (v == EasyLayoutHexSettings.HexConstraints.CellsPerColumn);
				}
			},
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
				new ConditionalFieldInfo("orientation"),
				new ConditionalFieldInfo("shovesOdd"),
				new ConditionalFieldInfo("coordinates"),
				new ConditionalFieldInfo("constraint", 1, isWriteCoordinates),
				new ConditionalFieldInfo("constraintCount", 1, isNotFlexible),
				new ConditionalFieldInfo("decreaseShoved", 1, isItemsPerN),
			};
		}
	}
}
#endif