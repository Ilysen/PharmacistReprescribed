using RimWorld;

namespace Ceres.PharmacistReprescribed
{
	public class MainButtonWorker_TogglePharmacistTab : MainButtonWorker_ToggleTab
	{
		public override bool Visible => !PharmacistReprescribed.Settings?.HideTab ?? true;
	}
}
