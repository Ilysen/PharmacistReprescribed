// Resources.cs
// Copyright Karel Kroeze, 2018-2018

using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace Pharmacist
{
	[StaticConstructorOnStartup]
	public static class Resources
	{
		public static Texture2D[] SeverityTextures;
		public static Texture2D[] CareTextures = AccessTools.Field(typeof(MedicalCareUtility), "careTextures").GetValue(null) as Texture2D[];
		public static Texture2D SlightlyDarkBackground = SolidColorMaterials.NewSolidColorTexture(new Color(0f, 0f, 0f, .1f));

		static Resources()
		{
			SeverityTextures = new[]
			{
				ContentFinder<Texture2D>.Get( "UI/Icons/bandage-solid" ),
				ContentFinder<Texture2D>.Get( "UI/Icons/droplet-solid" ),
				ContentFinder<Texture2D>.Get( "UI/Icons/heart-pulse-solid" ),
				ContentFinder<Texture2D>.Get( "UI/Icons/bed-pulse-solid" ),
				ContentFinder<Texture2D>.Get( "UI/Icons/clock-solid" )
            };
		}
	}
}
