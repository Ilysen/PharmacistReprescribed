using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using static Ceres.PharmacistReprescribed.Constants;
using static Ceres.PharmacistReprescribed.Resources;

namespace Ceres.PharmacistReprescribed
{
	public class MainTabWindow_Pharmacist : MainTabWindow
	{
		public override Vector2 InitialSize => new Vector2(
			CareSelectorWidth + OptionsWidth + Constants.Margin + (2 * Margin),
			TitleHeight + CareSelectorHeight + (2 * Margin));
		internal static Population[] populations = Enum.GetValues(typeof(Population)).Cast<Population>().ToArray();
		internal static InjurySeverity[] severities = Enum.GetValues(typeof(InjurySeverity)).Cast<InjurySeverity>().ToArray();
		internal static MedicalCareCategory[] medcares = Enum.GetValues(typeof(MedicalCareCategory)).Cast<MedicalCareCategory>().ToArray();

		internal static int CareSelectorWidth => CareSelectorRowLabelWidth + (CareSelectorColumnWidth * severities.Length);
		internal static int CareSelectorHeight
		{
			get
			{
				int activePops = populations.Length;
				if (!ModsConfig.IdeologyActive) // Remove entry for slaves...
					activePops--;
				if (!ModsConfig.AnomalyActive || !Find.Storyteller.difficulty.AnomalyPlaystyleDef.enableAnomalyContent) // ...and/or entities too
					activePops--;
				return RowHeight * (activePops + 1);
			}
		}
		internal static int OptionsWidth => 300;
		internal static int OptionsHeight => CareSelectorHeight;

		public override void DoWindowContents(Rect canvas)
		{
			// todo: remove (debugging)
			if (PharmacistSettings.CareSettings == null)
			{
				PharmacistSettings.SetDefaults();
			}

			Rect titleRect = new Rect(
				canvas.xMin,
				canvas.yMin,
				canvas.width,
				TitleHeight);
			Rect careSelectorRect = new Rect(
				canvas.xMin,
				titleRect.yMax,
				CareSelectorWidth,
				CareSelectorHeight);
			Rect optionsRect = new Rect(
				careSelectorRect.xMax + Constants.Margin,
				titleRect.yMax,
				OptionsWidth,
				OptionsHeight);

			Text.Font = GameFont.Medium;
			Widgets.Label(titleRect, "Ceres.PharmacistReprescribed.Config.Title".Translate());
			Text.Font = GameFont.Small;

			DrawCareSelectors(careSelectorRect);
			DrawOptions(optionsRect);
		}

		private void CreateMedicalCareSelectionFloatMenu(Action<MedicalCareCategory> action)
		{
			List<FloatMenuOption> options = new List<FloatMenuOption>();
			foreach (MedicalCareCategory category in medcares)
			{
				options.Add(new FloatMenuOption($"MedicalCareCategory_{category}".Translate().CapitalizeFirst(),
					() => action(category),
					extraPartWidth: 30,
					extraPartOnGUI: rect =>
					{
						Rect optionIconRect = new Rect(0f, 0f, IconSize, IconSize)
							.CenteredOnXIn(rect)
							.CenteredOnYIn(rect);
						GUI.DrawTexture(optionIconRect, CareTextures[(int)category]);
						return false;
					}));
			}

			Find.WindowStack.Add(new FloatMenu(options));
		}

		private float _optionsHeight = -1f;
		private Vector2 _optionsScrollPosition = Vector2.zero;
		private void DrawOptions(Rect canvas)
		{
			// draw background
			GUI.DrawTexture(canvas, SlightlyDarkBackground);

			Rect viewRect = new Rect(
				canvas.xMin,
				canvas.yMin,
				canvas.width - 16f,
				_optionsHeight);

			Rect row = new Rect(
				viewRect.xMin + Constants.Margin,
				viewRect.yMin + Constants.Margin,
				viewRect.width - (Constants.Margin * 2),
				RowHeight);

			Widgets.BeginScrollView(canvas, ref _optionsScrollPosition, viewRect);

			Widgets.Label(row, "Ceres.PharmacistReprescribed.DiseaseMargin".Translate(PharmacistSettings.CareSettings.DiseaseMargin.ToStringPercent()));
			TooltipHandler.TipRegion(row, "Ceres.PharmacistReprescribed.DiseaseMargin.Tip".Translate());
			row.y += RowHeight;
			PharmacistSettings.CareSettings.DiseaseMargin = Widgets.HorizontalSlider(row, PharmacistSettings.CareSettings.DiseaseMargin, 0f, 1f, roundTo: .01f);
			row.y += RowHeight;

			Widgets.Label(row, "Ceres.PharmacistReprescribed.DiseaseThreshold".Translate(PharmacistSettings.CareSettings.DiseaseThreshold.ToStringPercent()));
			TooltipHandler.TipRegion(row, "Ceres.PharmacistReprescribed.DiseaseThreshold.Tip".Translate());
			row.y += RowHeight;
			PharmacistSettings.CareSettings.DiseaseThreshold = Widgets.HorizontalSlider(row, PharmacistSettings.CareSettings.DiseaseThreshold, 0f, 1f, roundTo: .01f);
			row.y += RowHeight;

			Widgets.Label(row, "Ceres.PharmacistReprescribed.MinorWoundsThreshold".Translate(PharmacistSettings.CareSettings.MinorWoundsThreshold));
			TooltipHandler.TipRegion(row, "Ceres.PharmacistReprescribed.MinorWoundsThreshold.Tip".Translate());
			row.y += RowHeight;
			PharmacistSettings.CareSettings.MinorWoundsThreshold = (int)Widgets.HorizontalSlider(row, PharmacistSettings.CareSettings.MinorWoundsThreshold, 2, 20, roundTo: 1);
			row.y += RowHeight;

			// subtracting 1 from the display here is REAL hacky, but the readout doesn't otherwise accurately represent the way it works under the hood
			// even with a search radius of 2, the game's pathfinding will still only pick up stuff that's adjacent to the pawn, and so on
			// this just accounts for it in a way that hopefully makes sense to the user
			Widgets.Label(row, "Ceres.PharmacistReprescribed.SearchRadius".Translate(!PharmacistSettings.CareSettings.SearchRadiusIsUnlimited ? PharmacistSettings.CareSettings.EffectiveSearchRadius - 1 : "Ceres.PharmacistReprescribed.SearchRadius.Unlimited".Translate()));
			TooltipHandler.TipRegion(row, "Ceres.PharmacistReprescribed.SearchRadius.Tip".Translate());
			row.y += RowHeight;
			PharmacistSettings.CareSettings.SearchRadius = (int)Widgets.HorizontalSlider(row, PharmacistSettings.CareSettings.SearchRadius, 2, MaxSearchRadius, roundTo: 1);

			Widgets.EndScrollView();

			_optionsHeight = row.yMax - canvas.yMin;
		}

		private void DrawCareSelectors(Rect canvas)
		{
			// draw background
			GUI.DrawTexture(canvas, SlightlyDarkBackground);

			// draw column headers
			Vector2 pos = new Vector2(canvas.xMin + CareSelectorRowLabelWidth, canvas.yMin);
			foreach (InjurySeverity severity in severities)
			{
				Rect cell = new Rect(pos.x, pos.y, CareSelectorColumnWidth, RowHeight);
				Rect headerIconRect = new Rect(0, 0, IconSize, IconSize)
					.CenteredOnXIn(cell)
					.CenteredOnYIn(cell);

				TooltipHandler.TipRegion(cell,
					$"Ceres.PharmacistReprescribed.Severity.{severity}".Translate() + "\n\n" +
					$"Ceres.PharmacistReprescribed.Severity.{severity}.Tip".Translate());
				GUI.DrawTexture(headerIconRect, SeverityTextures[(int)severity]);

				Widgets.DrawHighlightIfMouseover(cell);
				if (Widgets.ButtonInvisible(cell))
				{
					CreateMedicalCareSelectionFloatMenu(category =>
					{
						foreach (Population population in populations)
						{
							PharmacistSettings.CareSettings[population][severity] = category;
						}
					});
				}

				pos.x += CareSelectorColumnWidth;
			}

			pos.x = canvas.xMin;
			pos.y += RowHeight;

			foreach (Population population in populations)
			{
				if (population == Population.Entities && !ModsConfig.AnomalyActive)
					continue;
				if (population == Population.Slaves && !ModsConfig.IdeologyActive)
					continue;
				Rect populationLabelRect = new Rect(pos.x, pos.y, CareSelectorRowLabelWidth, RowHeight);
				Text.Anchor = TextAnchor.MiddleLeft;
				// lmao this line sucks so bad
				// "ceres why don't you fix it" why don't you pay me!!!!
				Widgets.Label(populationLabelRect, population == Population.Guest ? "Ceres.PharmacistReprescribed.Population.Guest".Translate() : $"MedGroup{population}".Translate());
				TooltipHandler.TipRegion(populationLabelRect, population == Population.Guest ? "Ceres.PharmacistReprescribed.Population.Guest.Desc".Translate() : $"MedGroup{population}Desc".Translate());
				Text.Anchor = TextAnchor.UpperLeft;

				Widgets.DrawHighlightIfMouseover(populationLabelRect);
				if (Widgets.ButtonInvisible(populationLabelRect))
				{
					CreateMedicalCareSelectionFloatMenu(category =>
					{
						foreach (InjurySeverity severity in severities)
						{
							PharmacistSettings.CareSettings[population][severity] = category;
						}
					});
				}

				pos.x += CareSelectorRowLabelWidth;

				foreach (InjurySeverity severity in severities)
				{
					Rect cell = new Rect(
						pos.x,
						pos.y,
						CareSelectorColumnWidth,
						RowHeight);
					Rect iconRect = new Rect(0, 0, IconSize, IconSize)
						.CenteredOnXIn(cell)
						.CenteredOnYIn(cell);

					Widgets.DrawHighlightIfMouseover(cell);
					GUI.DrawTexture(iconRect, CareTextures[(int)PharmacistSettings.CareSettings[population][severity]]);

					if (Widgets.ButtonInvisible(cell))
					{
						CreateMedicalCareSelectionFloatMenu(category => PharmacistSettings.CareSettings[population][severity] = category);
					}

					pos.x += CareSelectorColumnWidth;
				}

				pos.x = canvas.xMin;
				pos.y += RowHeight;
			}
		}
	}
}
