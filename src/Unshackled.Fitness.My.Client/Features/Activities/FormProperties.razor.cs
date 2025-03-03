using Microsoft.AspNetCore.Components;
using Unshackled.Fitness.My.Client.Components;
using Unshackled.Fitness.My.Client.Features.Activities.Models;

namespace Unshackled.Fitness.My.Client.Features.Activities;

public class FormPropertiesBase : BaseFormComponent<FormActivityModel, FormActivityModel.Validator>
{
	[Parameter] public List<ActivityTypeListModel> ActivityTypes { get; set; } = [];
	[Parameter] public RenderFragment? ChildContent { get; set; }

	protected void HandleActivityTypeChange(string typeSid)
	{
		if (Model.ActivityTypeSid != typeSid)
		{
			Model.ActivityTypeSid = typeSid;

			// Only change units when adding
			if (string.IsNullOrEmpty(Model.Sid))
			{
				var actType = ActivityTypes.Where(x => x.Sid == typeSid).FirstOrDefault();
				if (actType != null) {
					Model.AverageCadenceUnit = actType.DefaultCadenceUnits;
					Model.AverageSpeedUnit = actType.DefaultSpeedUnits;
					Model.EventType = actType.DefaultEventType;
					Model.MaximumAltitudeUnit = actType.DefaultElevationUnits;
					Model.MaximumCadenceUnit = actType.DefaultCadenceUnits;
					Model.MaximumSpeedUnit = actType.DefaultSpeedUnits;
					Model.MinimumAltitudeUnit = actType.DefaultElevationUnits;
					Model.TargetCadenceUnit = actType.DefaultCadenceUnits;
					Model.TargetDistanceUnit = actType.DefaultDistanceUnits;
					Model.TotalAscentUnit = actType.DefaultElevationUnits;
					Model.TotalDescentUnit = actType.DefaultElevationUnits;
					Model.TotalDistanceUnit = actType.DefaultDistanceUnits;
				}
			}
		}
	}
}