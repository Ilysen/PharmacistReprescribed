using HarmonyLib;
using System.Reflection;
using Verse;

namespace Pharmacist
{
	public class Pharmacist : Mod
	{
		public Pharmacist(ModContentPack content) : base(content)
		{
#if DEBUG
			Harmony.DEBUG = true;
#endif
			Harmony harmony = new("fluffy.pharmacist");
			harmony.PatchAll(Assembly.GetExecutingAssembly());
		}
	}
}
