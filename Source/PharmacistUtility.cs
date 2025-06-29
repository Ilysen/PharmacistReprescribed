// Karel Kroeze
// PharmacistUtility.cs
// 2017-02-11

using RimWorld;
using System.Linq;
using Verse;

namespace Pharmacist
{
	/// <summary>
	/// A list of all injurity severity categories that the pharmacist can diagnose.
	/// This is typically calculated for a given health state in <c><see cref="PharmacistUtility.GetTendSeverity(Pawn)"/></c>.
	/// </summary>
	public enum InjurySeverity
	{
		Minor,
		Major,
		LifeThreathening,
		Operation,
		LongTerm
	}

	/// <summary>
	/// A list of all the population groups that a pawn can fall into.
	/// This is determined exclusively in <c><see cref="PharmacistUtility.GetPopulation(Pawn)"/></c>.
	/// </summary>
	/// TODO one day - make these all subclass instances or something; ideally it'd be something more extensible
	public enum Population
	{
		Colonist,
		Prisoner,
		Slave,
		Animal,
		Entity,
		Guest,
	}

	/// <summary>
	/// Contains most of the pharmacist's functionality, including diagnosis, population group calculation, and so on.
	/// </summary>
	public static class PharmacistUtility
	{
		/// <summary>
		/// This is the meat of how the Pharmacist determines what tending is needed.
		/// This method assesses the pawn's health state and determines a severity category based on what it detects.
		/// </summary>
		public static InjurySeverity GetTendSeverity(this Pawn patient)
		{
			if (!HealthAIUtility.ShouldBeTendedNowByPlayer(patient))
				return InjurySeverity.Minor;

			System.Collections.Generic.List<Hediff> hediffs = patient.health.hediffSet.hediffs;
			int ticksToDeathDueToBloodLoss = HealthUtility.TicksUntilDeathDueToBloodLoss(patient);

			// Any of the following is considered life-threatening:
			// * <=6 hours until death from blood loss
			// * Any hediff with a life-threatening stage
			// * Any disease that could be lethal if not treated well
			if (ticksToDeathDueToBloodLoss <= GenDate.TicksPerHour * 6 ||
				 hediffs.Any(h => h.CurStage?.lifeThreatening ?? false) ||
				 hediffs.Any(NearLethalDisease))
				return InjurySeverity.LifeThreathening;

			// One step down, any of the following is considered a major wound:
			// * <=12 hours until death from blood loss
			// * Any disease that has the potential to be deadly, but isn't currently life-threatening
			// * A high number of wounds that can potentially become infected
			if (ticksToDeathDueToBloodLoss <= GenDate.TicksPerHour * 12 ||
				 hediffs.Any(PotentiallyLethalDisease) ||
				 DeathByAThousandCuts(patient))
			{
				return InjurySeverity.Major;
			}

			// If non-major, then check for ongoing conditions; see the relevant method for more documentation on selection criteria
			if (hediffs.Any(TreatableOngoingCondition))
				return InjurySeverity.LongTerm;

			// And if we don't find any of these, then we're presumably only lightly injured
			return InjurySeverity.Minor;
		}

		/// <summary>
		/// Checks for any tendable condition that falls into one of the following categories:<br/>
		/// * Chronic (asthma, carcinoma);<br/>
		/// * Disappears after a specific amount of total tend quality (gut worms, muscles parasites, etc.);<br/>
		/// * Needs regular tending to prevent severity from worsening (blood rot, lung rot, etc.)
		/// </summary>
		private static bool TreatableOngoingCondition(Hediff h)
		{
			if (!h.TendableNow())
				return false;

			// long-term conditions -- asthma, carcinoma
			if (h.def.chronic)
				return true;

			// needs tending to disappear -- gut worms, muscle parasites
			if (h.TryGetComp(out HediffComp_TendDuration td) && td.TProps.disappearsAtTotalTendQuality >= 0)
				return true;

			// needs regular tending to prevent severity from worsening over time -- blood rot, lung rot, fibrous/sensory mechanites
			if (td != null && h.TryGetComp<HediffComp_Disappears>() != null)
			{
				if (td.TProps.severityPerDayTended < 0)
					return true;
			}

			return false;
		}

		/// <summary>
		/// Checks for any tendable disease that can potentially be fatal.
		/// </summary>
		private static bool PotentiallyLethalDisease(Hediff h)
		{
			if (!h.TendableNow())
				return false;

			if (h.def.lethalSeverity <= 0f)
				return false;

			HediffComp_Immunizable compImmunizable = h.TryGetComp<HediffComp_Immunizable>();
			return compImmunizable != null;
		}

		/// <summary>
		/// Checks for any potentially fatal disease whose severity is greater than its immunity by the configured disease margin.
		/// </summary>
		private static bool NearLethalDisease(Hediff h)
		{
			HediffComp_Immunizable compImmunizable = h.TryGetComp<HediffComp_Immunizable>();
			return PotentiallyLethalDisease(h) &&
				   !compImmunizable.FullyImmune &&
				   h.Severity > PharmacistSettings.CareSettings.DiseaseThreshold &&
				   compImmunizable.Immunity < PharmacistSettings.CareSettings.DiseaseMargin + h.Severity;
		}

		/// <summary>
		/// Checks if a given patient has a number of infectable wounds greater than the configured threshold.
		/// Pawns that are immune to wound infections (ghouls, Perfect Immunity gene, etc) will be skipped.
		/// </summary>
		private static bool DeathByAThousandCuts(Pawn patient)
		{
			if (patient.health.immunity.DiseaseContractChanceFactor(HediffDefOf.WoundInfection) == 0f)
				return false;
			return patient.health.hediffSet.hediffs.Count(hediff => hediff.TryGetComp<HediffComp_Infecter>() != null) >
				   PharmacistSettings.CareSettings.MinorWoundsThreshold;
		}

		/// <summary>
		/// Determines what population group this pawn falls into.
		/// </summary>
		public static Population GetPopulation(this Pawn patient)
		{
			if (patient.IsAnimal && patient.Faction == Faction.OfPlayer)
				return Population.Animal;

			if (patient.IsColonist)
				return Population.Colonist;

			if (patient.IsPrisonerOfColony)
				return Population.Prisoner;

			if (patient.IsSlaveOfColony)
				return Population.Slave;

			if (patient.IsEntity)
				return Population.Entity;

			return Population.Guest;
		}

		/// <summary>
		/// Wrapper for <c><see cref="TendAdvice(Pawn, InjurySeverity)"/></c> that calculates severity automatically instead of taking it as an argument.
		/// </summary>
		public static MedicalCareCategory TendAdvice(Pawn patient)
		{
			InjurySeverity severity = patient.GetTendSeverity();
			return TendAdvice(patient, severity);
		}

		/// <summary>
		/// Determines the appropriate medical care type for the provided pawn and injurity severity. Capped to the patient's maximum allowed medicine.
		/// Unless a specific type of injury is determined in advance (such as with surgery), consider using <c><see cref="TendAdvice(Pawn)"/></c>
		/// instead of this method.
		/// </summary>
		public static MedicalCareCategory TendAdvice(Pawn patient, InjurySeverity severity)
		{
			Population population = patient.GetPopulation();

			MedicalCareCategory pharmacist = PharmacistSettings.CareSettings[population][severity];
			MedicalCareCategory playerSetting = patient?.playerSettings?.medCare ?? MedicalCareCategory.Best;

#if DEBUG
			Log.Message(
				"Pharmacist :: Advice" +
				$"\n\tpatient: {patient?.LabelShort}" +
				$"\n\tpopulation: {population}" +
				$"\n\tseverity: {severity}" +
				$"\n\tplayerSettings: {playerSetting}" +
				$"\n\tpharmacist: {pharmacist}");
#endif

			return pharmacist < playerSetting ? pharmacist : playerSetting;
		}
	}
}
