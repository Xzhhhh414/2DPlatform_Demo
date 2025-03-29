namespace UIThemes.Samples
{
	using System;
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// Sample of extended ThemeTarget.
	/// </summary>
	public class ThemeTargetExtended : ThemeTargetCustom<ThemeExtended>
	{
		/// <summary>
		/// Rotated sprites.
		/// </summary>
		[SerializeField]
		[ThemeProperty(nameof(ThemeExtended.RotatedSprites))]
		protected List<Target> rotatedSprites = new List<Target>();

		/// <summary>
		/// Rotated sprites.
		/// </summary>
		public IReadOnlyList<Target> RotatedSprites => rotatedSprites;

		/// <inheritdoc/>
		public override void SetPropertyOwner<TComponent>(Type propertyType, TComponent component, string property, Component owner)
		{
			if (propertyType == typeof(RotatedSprite))
			{
				SetPropertyOwner(RotatedSprites, component, property, owner);
			}
			else
			{
				base.SetPropertyOwner(propertyType, component, property, owner);
			}
		}

		/// <inheritdoc/>
		protected override void ThemeChanged(VariationId variationId)
		{
			base.ThemeChanged(variationId);

			SetValue(variationId, Theme.RotatedSprites, rotatedSprites);
		}

		#if UNITY_EDITOR
		/// <inheritdoc/>
		protected override void FindTargets(List<Component> components, ExclusionList exclusion)
		{
			base.FindTargets(components, exclusion);

			if (IsActiveProperty(nameof(Theme.RotatedSprites)))
			{
				FindTargets<RotatedSprite>(components, rotatedSprites, exclusion);
			}
		}
		#endif
	}
}