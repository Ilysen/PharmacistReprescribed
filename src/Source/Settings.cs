using Verse;

namespace Ceres.PharmacistReprescribed
{
	public class Settings : ModSettings
	{
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref HideTab, nameof(HideTab), false);
		}

		public bool HideTab = false;
	}
}
