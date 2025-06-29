// Settings.cs
// Copyright Karel Kroeze, 2018-2018

using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using Verse;

namespace Ilysen.PharmacistReprescribed
{
	/// <summary>
	/// Initiates and houses the pharmacist's settings.<br/>
	/// For the building blocks of this comp, see: <c><see cref="MedicalCare"/></c>, <c><see cref="PopulationCare"/></c>
	/// </summary>
	public class PharmacistSettings : WorldComponent
	{
		/// <summary>
		/// Singleton for the sole <c><see cref="MedicalCare"/></c> instance.
		/// </summary>
		public static MedicalCare CareSettings;

		public PharmacistSettings(World world) : base(world)
		{
			SetDefaults();
		}

		/// <summary>
		/// Data-only class that represents the pharmacist's configuration on a given save file.
		/// </summary>
		public class MedicalCare : IExposable
		{
			public PopulationCare this[Population index]
			{
				get
				{
					if (!_populationCare.TryGetValue(index, out PopulationCare populationCare))
					{
						Log.Warning($"Medical Care for {index} not initialized, using Best.");
						populationCare = PopulationCare.Default;
						_populationCare[index] = populationCare;
					}
					return populationCare;
				}
				internal set => _populationCare[index] = value;
			}

			/// <summary>
			/// Contains every <c><see cref="PopulationCare"/></c> instance, associated to its <c><see cref="Population"/></c> type.
			/// </summary>
			private Dictionary<Population, PopulationCare> _populationCare = new();

			/// <summary>
			/// If a dangerous disease's severity is higher than its immunity gain by at least this much, it will be considered life-threatening.
			/// Otherwise, it will be considered major.
			/// </summary>
			private float _diseaseMargin = 0.1f;

			/// <summary>
			/// If a pawn has at least this many infectable injuries, they will be considered major wounds, even if they'd otherwise be minor.
			/// </summary>
			private int _minorWoundsThreshold = 5;

			/// <summary>
			/// Dangerous diseases will only be considered major injuries if their severity is above this threshold.
			/// </summary>
			private float _diseaseThreshold = 0.1f;

			/// <summary>
			/// Doctors will only search for medicine within this radius.
			/// They will search around the patient first, and if they don't find anything, they'll search around themselves instead.
			/// </summary>
			private int _searchRadius = Constants.MaxSearchRadius;

			/// <summary>
			/// Safe wrapper for <c><see cref="_diseaseMargin"/></c>.
			/// </summary>
			public float DiseaseMargin
			{
				protected internal set => _diseaseMargin = value;
				get => _diseaseMargin;
			}

			/// <summary>
			/// Safe wrapper for <c><see cref="_diseaseThreshold"/></c>.
			/// </summary>
			public float DiseaseThreshold
			{
				protected internal set => _diseaseThreshold = value;
				get => _diseaseThreshold;
			}

			/// <summary>
			/// Safe wrapper for <c><see cref="_minorWoundsThreshold"/></c>.
			/// </summary>
			public int MinorWoundsThreshold
			{
				protected internal set => _minorWoundsThreshold = value;
				get => _minorWoundsThreshold;
			}

			/// <summary>
			/// Safe wrapper for <c><see cref="_searchRadius"/></c>.
			/// </summary>
			public int SearchRadius
			{
				protected internal set => _searchRadius = value;
				get => _searchRadius;
			}

			/// <summary>
			/// Calculates the effective configured search radius.
			/// If <c><see cref="SearchRadiusIsUnlimited"/></c> is true, this will return 9999. Otherwise, it will return <c><see cref="SearchRadius"/></c>.
			/// </summary>
			public int EffectiveSearchRadius => !SearchRadiusIsUnlimited ? SearchRadius : 9999;

			/// <summary>
			/// Determine whether or not the search radius mechanic should be disregarded and an unlimited range used instead.
			/// This specifically checks for if the configured <c><see cref="SearchRadius"/></c> is equal to <c><see cref="Constants.MaxSearchRadius"/></c>.
			/// </summary>
			public bool SearchRadiusIsUnlimited => SearchRadius < Constants.MaxSearchRadius;

			public void ExposeData()
			{
				Scribe_Collections.Look(ref _populationCare, nameof(PopulationCare), LookMode.Value, LookMode.Deep);
				Scribe_Values.Look(ref _diseaseMargin, nameof(DiseaseMargin), 0.1f);
				Scribe_Values.Look(ref _diseaseThreshold, nameof(DiseaseThreshold), 0.1f);
				Scribe_Values.Look(ref _minorWoundsThreshold, nameof(MinorWoundsThreshold), 5);
				Scribe_Values.Look(ref _searchRadius, nameof(SearchRadius), Constants.MaxSearchRadius);
			}
		}

		/// <summary>
		/// Data-only class representing the pharmacist's configured care settings for a given population type.
		/// The <c><see cref="MedicalCare"/></c> singleton tracks one of these for every category.
		/// </summary>
		public class PopulationCare : IExposable
		{
			/// <summary>
			/// The prescribed treatments for this population group. Severity is associated to its configured care level.
			/// </summary>
			private Dictionary<InjurySeverity, MedicalCareCategory> _populationCare;

			public PopulationCare(MedicalCareCategory minor, MedicalCareCategory major,
				MedicalCareCategory lifethreatening, MedicalCareCategory operation, MedicalCareCategory longTerm)
			{
				_populationCare = new Dictionary<InjurySeverity, MedicalCareCategory> {
					{ InjurySeverity.Minor, minor },
					{ InjurySeverity.Major, major },
					{ InjurySeverity.LifeThreathening, lifethreatening },
					{ InjurySeverity.Operation, operation },
					{ InjurySeverity.LongTerm, longTerm }
				};
			}

			public PopulationCare()
			{
				_populationCare = new Dictionary<InjurySeverity, MedicalCareCategory>();
			}

			public static PopulationCare Default => new PopulationCare(
				MedicalCareCategory.Best,
				MedicalCareCategory.Best,
				MedicalCareCategory.Best,
				MedicalCareCategory.Best,
				MedicalCareCategory.Best);

			public MedicalCareCategory this[InjurySeverity index]
			{
				get => _populationCare[index];
				set => _populationCare[index] = value;
			}

			public void ExposeData()
			{
				Scribe_Collections.Look(ref _populationCare, nameof(_populationCare), LookMode.Value, LookMode.Value);
			}
		}

		/// <summary>
		/// Initializes the <c><see cref="MedicalCare"/></c> singleton and sets up all the population groups with sensible default values.
		/// </summary>
		public static void SetDefaults()
		{
			CareSettings = new MedicalCare();
			CareSettings[Population.Colonist] = new PopulationCare(
				MedicalCareCategory.HerbalOrWorse,
				MedicalCareCategory.NormalOrWorse,
				MedicalCareCategory.Best,
				MedicalCareCategory.Best,
				MedicalCareCategory.Best);

			CareSettings[Population.Guest] = new PopulationCare(
				MedicalCareCategory.NoMeds,
				MedicalCareCategory.HerbalOrWorse,
				MedicalCareCategory.NormalOrWorse,
				MedicalCareCategory.NormalOrWorse,
				MedicalCareCategory.NoMeds);

			CareSettings[Population.Prisoner] = new PopulationCare(
				MedicalCareCategory.NoMeds,
				MedicalCareCategory.HerbalOrWorse,
				MedicalCareCategory.NormalOrWorse,
				MedicalCareCategory.NormalOrWorse,
				MedicalCareCategory.HerbalOrWorse);

			CareSettings[Population.Animal] = new PopulationCare(
				MedicalCareCategory.NoMeds,
				MedicalCareCategory.HerbalOrWorse,
				MedicalCareCategory.NormalOrWorse,
				MedicalCareCategory.NormalOrWorse,
				MedicalCareCategory.HerbalOrWorse);

			CareSettings[Population.Slave] = new PopulationCare(
				MedicalCareCategory.NoMeds,
				MedicalCareCategory.HerbalOrWorse,
				MedicalCareCategory.NormalOrWorse,
				MedicalCareCategory.NormalOrWorse,
				MedicalCareCategory.HerbalOrWorse);

			CareSettings[Population.Entity] = new PopulationCare(
				MedicalCareCategory.NoMeds,
				MedicalCareCategory.NoMeds,
				MedicalCareCategory.HerbalOrWorse,
				MedicalCareCategory.HerbalOrWorse,
				MedicalCareCategory.NoCare); // Entities can't get diseases, so we can just skip this
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Deep.Look(ref CareSettings, nameof(CareSettings));
		}
	}
}
