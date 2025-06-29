// Karel Kroeze
// HealthAIUtility_FindBestMedicine.cs
// 2017-02-11

using HarmonyLib;
using RimWorld;
using System.Linq;
using Verse;
using Verse.AI;

namespace Pharmacist
{
	[HarmonyPatch(typeof(HealthAIUtility))]
	public class HealthAIUtility_Patch
	{
		/// <summary>
		/// Replaces the vanilla logic for medicine selection with our own bespoke stuff.
		/// It's almost certainly possible to use transpilers for a more surgical approach, but I'm not that good and the original mod wasn't either, so :bleh:
		/// Otherwise, this has been written to conform to vanilla functionality as closely as possible
		/// </summary>
		[HarmonyPrefix]
		[HarmonyPatch(nameof(HealthAIUtility.FindBestMedicine))]
		public static bool FindBestMedicine_Override(Pawn healer, Pawn patient, bool onlyUseInventory, ref Thing __result)
		{
			__result = null;
			if (patient.playerSettings == null || patient.playerSettings.medCare <= MedicalCareCategory.NoMeds || Medicine.GetMedicineCountToFullyHeal(patient) <= 0)
				return false;

			MedicalCareCategory pharmacistAdvice = PharmacistUtility.TendAdvice(patient);
			if (pharmacistAdvice <= MedicalCareCategory.NoMeds)
				return false;

			// Check the inventory first...
			__result = GetMedsFromInventory(healer.inventory.innerContainer);
			if (__result != null || onlyUseInventory)
				return false;

			// ...then look around the patient if we can't find anything
			__result = GenClosest.ClosestThing_Global_Reachable(
				patient.PositionHeld,
				patient.MapHeld,
				patient.MapHeld.listerThings.ThingsInGroup(ThingRequestGroup.Medicine),
				PathEndMode.ClosestTouch,
				TraverseParms.For(healer),
				PharmacistSettings.medicalCare.EffectiveSearchRadius,
				IsPrescribedMedicine,
				GetMedicalPotency);

			// If there's nothing near the patient, look around the doctor instead
			// This will only happen if a search radius is set, since otherwise we've naturally already searched the whole map
			if (__result == null && !PharmacistSettings.medicalCare.SearchRadiusIsUnlimited)
			{
				__result = GenClosest.ClosestThing_Global_Reachable(
					healer.Position,
					healer.Map,
					healer.Map.listerThings.ThingsInGroup(ThingRequestGroup.Medicine),
					PathEndMode.ClosestTouch,
					TraverseParms.For(healer),
					PharmacistSettings.medicalCare.EffectiveSearchRadius,
					IsPrescribedMedicine,
					GetMedicalPotency);
			}

			// Still nothing. Check nearby animal inventories if we're allowed
			if (__result == null && healer.IsColonist)
			{
				foreach (Pawn pawn in healer.Map.mapPawns.SpawnedColonyAnimals.Where(IsAllowedAnimal))
				{
					Thing meds2Use = GetMedsFromInventory(pawn.inventory.innerContainer);
					if (meds2Use != null)
					{
						__result = meds2Use;
						break;
					}
				}
			}

			// Finally, skip the original method
			return false;

			bool IsAllowedAnimal(Pawn p)
			{
				// We're technically running the logic to check for inventory count twice here,
				// but the list comparison is probably cheaper than going down the line further and calculating pathfinding for every animal,
				// so it should save perf in the long run hopefully!
				return p.inventory.innerContainer.Count > 0 &&
					!p.IsForbidden(healer) &&
					healer.CanReach(p, PathEndMode.OnCell, Danger.Some) &&
					(p.Position.DistanceTo(healer.Position) <= PharmacistSettings.medicalCare.EffectiveSearchRadius ||
					p.Position.DistanceTo(patient.PositionHeld) <= PharmacistSettings.medicalCare.EffectiveSearchRadius);
			}

			float GetMedicalPotency(Thing t) => t.def.GetStatValueAbstract(StatDefOf.MedicalPotency);

			bool IsPrescribedMedicine(Thing m) => !m.IsForbidden(healer) && m.def.IsMedicine &&
				pharmacistAdvice.AllowsMedicine(m.def) && healer.CanReserve(m, 10, 1);

			Thing GetMedsFromInventory(ThingOwner inventory)
			{
				if (inventory.Count == 0)
					return null;
				var located = inventory.Where(IsPrescribedMedicine);
				if (located.Any())
					return located.MaxBy(GetMedicalPotency);
				return null;
			}
		}
	}
}
