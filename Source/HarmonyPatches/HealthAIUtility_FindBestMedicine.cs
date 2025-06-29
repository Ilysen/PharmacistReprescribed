// Karel Kroeze
// HealthAIUtility_FindBestMedicine.cs
// 2017-02-11

using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace Pharmacist {
    [HarmonyPatch(typeof(HealthAIUtility), nameof(HealthAIUtility.FindBestMedicine))]
    public class HealthAIUtility_FindBestMedicine {
        public static bool Prefix(Pawn healer, Pawn patient, bool onlyUseInventory, ref Thing __result) {
            if (patient.playerSettings == null ||
                patient.playerSettings.medCare <= MedicalCareCategory.NoMeds ||
                Medicine.GetMedicineCountToFullyHeal(patient) <= 0)
            {
                __result = null;
                return false;
            }

            // get lowest of pawn care settings & pharmacy settings
            MedicalCareCategory pharmacistAdvice = PharmacistUtility.TendAdvice( patient );
            if (pharmacistAdvice <= MedicalCareCategory.NoMeds) {
                __result = null;
                return false;
            }

            // check pockets first
            // note: vanilla actually selects the medicine with the _lowest_ potency,
            // I have to assume that that is not intentional.
            //
            // thanks to KennethSammael for adding this check in the unofficial 1.3 update,
            // this code is adapted from his changes.
            // Update; I adapted it badly. The fault was all mine.
            __result = GetMedsFromInventory(healer.inventory.innerContainer);
            if (__result != null || onlyUseInventory) {
                return false;
            }

            int effectiveSearchRadius = PharmacistSettings.medicalCare.SearchRadius == 76 ? 9999 : PharmacistSettings.medicalCare.SearchRadius;
            // search for best meds
            __result = GenClosest.ClosestThing_Global_Reachable(
                patient.PositionHeld,
                patient.MapHeld,
                patient.MapHeld.listerThings.ThingsInGroup(ThingRequestGroup.Medicine),
                PathEndMode.ClosestTouch,
                TraverseParms.For(healer),
                effectiveSearchRadius,
                IsPrescribedMedicine,
                GetMedicalPotency);

            // nothing in inventory, nothing nearby. try checking animals in the radius
            if (__result == null && healer.IsColonist && healer.Map != null)
            {
                foreach (Pawn pawn in healer.Map.mapPawns.SpawnedColonyAnimals.Where(x => x.Position.DistanceTo(healer.Position) < effectiveSearchRadius))
                {
                    Thing meds2Use = GetMedsFromInventory(pawn.inventory.innerContainer);
                    if (meds2Use != null && !pawn.IsForbidden(healer) && healer.CanReach(pawn, PathEndMode.OnCell, Danger.Some))
                    {
                        __result = meds2Use;
                        break;
                    }
                }
            }

            return false;

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
