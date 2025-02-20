﻿using Unshackled.Fitness.Core.Enums;
using Unshackled.Fitness.My.Client.Models;

namespace Unshackled.Fitness.My.Client.Features.Activities.Models;

public class SearchActivitiesModel : SearchModel
{
	public string? ActivityTypeSid { get; set; }
	public EventTypes EventType { get; set; }
	public DateTimeOffset? DateStart { get; set; }
	public DateTimeOffset? DateEnd { get; set; }
	public string? Title { get; set; }
}