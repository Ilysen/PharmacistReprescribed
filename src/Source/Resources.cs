// Resources.cs
// Copyright Karel Kroeze, 2018-2018

using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace Ilysen.PharmacistReprescribed
{
	[StaticConstructorOnStartup]
	public static class Resources
	{
		public static Texture2D[] SeverityTextures;
		public static Texture2D TabIcon;
		public static Texture2D[] CareTextures = AccessTools.Field(typeof(MedicalCareUtility), "careTextures").GetValue(null) as Texture2D[];
		public static Texture2D SlightlyDarkBackground = SolidColorMaterials.NewSolidColorTexture(new Color(0f, 0f, 0f, .1f));

		static Resources()
		{
			SeverityTextures = new[]
			{
				ContentFinder<Texture2D>.Get( "UI/Icons/finger" ),
				ContentFinder<Texture2D>.Get( "UI/Icons/blood" ),
				ContentFinder<Texture2D>.Get( "UI/Icons/heart" ),
				ContentFinder<Texture2D>.Get( "UI/Icons/scalpel" ),
				ContentFinder<Texture2D>.Get( "UI/Icons/clock" ) //<a href="https://www.flaticon.com/free-icons/clock" title="clock icons">Clock icons created by Ilham Fitrotul Hayat - Flaticon</a>
            };
			TabIcon = ContentFinder<Texture2D>.Get("UI/Icons/hospital");
		}
	}
}
