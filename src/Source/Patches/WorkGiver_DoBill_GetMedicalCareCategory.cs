using HarmonyLib;
using RimWorld;
using Verse;

namespace Ceres.PharmacistReprescribed.HarmonyPatches
{
	[HarmonyPatch(typeof(WorkGiver_DoBill), "GetMedicalCareCategory")]
	public static class WorkGiver_DoBill_GetMedicalCareCategory
	{
		public static bool Prefix(Thing billGiver, ref MedicalCareCategory __result)
		{
			if (billGiver is not Pawn pawn)
			{
				// because this is the fallback vanilla uses...
				__result = MedicalCareCategory.Best;
			}
			else
			{
				// assumption: bills on people === operations
				__result = PharmacistFunctionality.TendAdvice(pawn, InjurySeverity.Operation);
			}

			// always cancel execution of vanilla method.
			return false;
		}
	}
}
