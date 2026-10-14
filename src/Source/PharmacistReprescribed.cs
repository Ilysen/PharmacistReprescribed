using HarmonyLib;
using System.Reflection;
using UnityEngine;
using Verse;

namespace Ceres.PharmacistReprescribed
{
	public class PharmacistReprescribed : Mod
	{
		public static Settings Settings;

		public PharmacistReprescribed(ModContentPack content) : base(content)
		{
			Settings = GetSettings<Settings>();

#if DEBUG
			Harmony.DEBUG = true;
#endif
			Harmony harmony = new("Ceres.PharmacistReprescribed");
			harmony.PatchAll(Assembly.GetExecutingAssembly());
		}

		public override string SettingsCategory() => Content.Name;

		public override void DoSettingsWindowContents(Rect inRect)
		{
			var ls = new Listing_Standard();
			ls.Begin(inRect);
			ls.CheckboxLabeled(
				Translator.Translate("Fluffy.Ceres.PharmacistReprescribed.Settings.ToggleTab"),
				ref Settings.HideTab,
				Translator.Translate("Fluffy.Ceres.PharmacistReprescribed.Settings.ToggleTab.Tooltip")
			);
			ls.End();
		}
	}
}
