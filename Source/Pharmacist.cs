using HarmonyLib;
using System.Reflection;
using Verse;

namespace Ilysen.PharmacistReprescribed
{
	public class Pharmacist : Mod
	{
		public Pharmacist(ModContentPack content) : base(content)
		{
#if DEBUG
			Harmony.DEBUG = true;
#endif
			Harmony harmony = new("Ilysen.PharmacistReprescribed");
			harmony.PatchAll(Assembly.GetExecutingAssembly());
		}
	}
}
