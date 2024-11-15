﻿using Unshackled.Fitness.Core.Enums;
using Unshackled.Studio.Core.Client.Models;

namespace Unshackled.Fitness.My.Client.Features.TrainingSessions.Models;

public class ActivityTypeListModel : BaseMemberObject
{
	public string Title { get; set; } = string.Empty;
	public string? Color { get; set; }
	public EventTypes DefaultEventType { get; set; } = EventTypes.Uncategorized;
	public DistanceUnits DefaultDistanceUnits { get; set; }
	public DistanceUnits DefaultElevationUnits { get; set; }
	public SpeedUnits DefaultSpeedUnits { get; set; }
	public CadenceUnits DefaultCadenceUnits { get; set; }
}
