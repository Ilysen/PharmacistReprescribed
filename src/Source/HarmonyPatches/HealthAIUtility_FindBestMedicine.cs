// Karel Kroeze
// HealthAIUtility_FindBestMedicine.cs
// 2017-02-11

using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;

namespace Pharmacist.HarmonyPatches
{
	[HarmonyPatch(typeof(HealthAIUtility))]
	public class HealthAIUtility_Patch
	{
		/// <summary>
		/// Replaces the vanilla logic for medicine selection with our own bespoke stuff.
		/// It's almost certainly possible to use transpilers for a more surgical approach, but I'm not that good and the original mod took this approach, so :bleh:
		/// Otherwise, this has been written to conform to vanilla functionality as closely as possible
		/// </summary>
		[HarmonyPrefix]
		[HarmonyPatch(nameof(HealthAIUtility.FindBestMedicine))]
		public static bool FindBestMedicine_Override(Pawn healer, Pawn patient, bool onlyUseInventory, ref Thing __result)
		{
			__result = null;
#if DEBUG
			Log.Message($"Checking if all are false: {patient.playerSettings == null}, {patient.playerSettings?.medCare <= MedicalCareCategory.NoMeds}, {Medicine.GetMedicineCountToFullyHeal(patient) <= 0}");
#endif
			// patient manually set to have no medical care? skip entirey
			if (patient.playerSettings != null && patient.playerSettings.medCare <= MedicalCareCategory.NoMeds)
				return false;

			// no meds needed to heal? well then we don't need to find any more, now do we, you silly goose
			if (Medicine.GetMedicineCountToFullyHeal(patient) <= 0)
				return false;

			// okay, getting into the meat of it now
			// For drafted tends against targets with no care settings (like downed raiders), use anything in the inventory and then use no meds
			// This mirrors vanilla functionality
			// For anything else, we get the pharmacist's adivce
			bool forceAllowAllMeds = patient.playerSettings == null && onlyUseInventory;
			MedicalCareCategory pharmacistAdvice = healer.Drafted ? (
				patient?.playerSettings?.medCare ?? (forceAllowAllMeds ? MedicalCareCategory.Best : MedicalCareCategory.NoMeds)
				) : PharmacistUtility.TendAdvice(patient);
#if DEBUG
			Log.Message($"checking pharmacist recommends no meds: {pharmacistAdvice <= MedicalCareCategory.NoMeds}");
#endif
			// pharmacist says no meds, so no meds. put those hands to work, good doctor
			if (pharmacistAdvice <= MedicalCareCategory.NoMeds)
				return false;

#if DEBUG
			Log.Message($"checking inventory has meds (only uses from inventory: {onlyUseInventory})");
#endif
			// otherwise, let's find meds!
			// we'll check the inventory first...
			Thing medsFromInventory = GetMedsFromInventory(healer.inventory.innerContainer);
			if (onlyUseInventory) // if we're only using from the inventory, then terminate here
			{
				__result = medsFromInventory;
				return false;
			}
			// otherwise, add it to a larger list of potential candidates...
			List<Thing> candidateMeds = new();
			if (medsFromInventory != null)
				candidateMeds.Add(medsFromInventory);

#if DEBUG
			Log.Message($"Effective search radius: {PharmacistSettings.CareSettings.EffectiveSearchRadius} (actual: {PharmacistSettings.CareSettings.SearchRadius})");

			Log.Message("Now searching for meds near patient...");
#endif
			// ...then look around the patient for something better, if possible...
			Thing medsFromNearPatient = GenClosest.ClosestThing_Global_Reachable(
				patient.PositionHeld,
				patient.MapHeld,
				patient.MapHeld.listerThings.ThingsInGroup(ThingRequestGroup.Medicine),
				PathEndMode.ClosestTouch,
				TraverseParms.For(healer),
				PharmacistSettings.CareSettings.EffectiveSearchRadius,
				IsPrescribedMedicine,
				GetMedicalPotency);
			if (medsFromNearPatient != null)
			{
#if DEBUG
				Log.Message($"Forbidden for patient or healer: ({medsFromNearPatient.IsForbidden(patient)}, {medsFromNearPatient.IsForbidden(healer)})");
#endif
				candidateMeds.Add(medsFromNearPatient);
			}

			// ...and finally, search around the doctor instead
			// if we have an unlimited search radius then we've already covered the whole map and can skip this step entirely
			if (!PharmacistSettings.CareSettings.SearchRadiusIsUnlimited)
			{
#if DEBUG
				Log.Message("Now searching for meds near doctor...");
#endif
				Thing medsFromNearDoctor = PharmacistSettings.CareSettings.SearchRadiusIsUnlimited ? null : GenClosest.ClosestThing_Global_Reachable(
					healer.Position,
					healer.Map,
					healer.Map.listerThings.ThingsInGroup(ThingRequestGroup.Medicine),
					PathEndMode.ClosestTouch,
					TraverseParms.For(healer),
					PharmacistSettings.CareSettings.EffectiveSearchRadius,
					IsPrescribedMedicine,
					GetMedicalPotency);

				if (medsFromNearDoctor != null)
				{
#if DEBUG
					Log.Message($"Forbidden for patient or healer: ({medsFromNearDoctor.IsForbidden(patient)}, {medsFromNearDoctor.IsForbidden(healer)})");
#endif
					candidateMeds.Add(medsFromNearDoctor);
				}
			}

			// search complete. search through all of our candidate meds, and pick the best possible one
			if (candidateMeds.Count != 0)
				__result = candidateMeds.MaxBy(GetMedicalPotency);

			// uh oh, we found nothing usable. check nearby animal inventories if we're allowed
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

			// and finally, skip the original method. we've recreated its functionality here!
			return false;

			bool IsAllowedAnimal(Pawn p)
			{
				// We're technically running the logic to check for inventory count twice here,
				// but the list comparison is probably cheaper than going down the line further and calculating pathfinding for every animal,
				// so it should save perf in the long run hopefully!
				return p.inventory.innerContainer.Count > 0 &&
					!p.IsForbidden(healer) &&
					healer.CanReach(p, PathEndMode.OnCell, Danger.Some) &&
					(p.Position.DistanceTo(healer.Position) <= PharmacistSettings.CareSettings.EffectiveSearchRadius ||
					p.Position.DistanceTo(patient.PositionHeld) <= PharmacistSettings.CareSettings.EffectiveSearchRadius);
			}

			float GetMedicalPotency(Thing t) => t.def.GetStatValueAbstract(StatDefOf.MedicalPotency);

			bool IsPrescribedMedicine(Thing m)
			{
#if DEBUG
				Log.Message($"{m.def.defName}: notForbidden = {!m.IsForbidden(healer)}, isMeds = {m.def.IsMedicine}, pharmacistAllows = {pharmacistAdvice.AllowsMedicine(m.def)}, healerCanReserve = {healer.CanReserve(m, 10, 1)}");
#endif
				return !m.IsForbidden(healer) && m.def.IsMedicine && pharmacistAdvice.AllowsMedicine(m.def) && healer.CanReserve(m, 10, 1);
			}

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
