﻿using Unshackled.Fitness.Core.Enums;
using Unshackled.Fitness.My.Client.Models;

namespace Unshackled.Fitness.My.Client.Features.Programs.Models;

public class ProgramListModel : BaseMemberObject
{
	public string Title { get; set; } = string.Empty;
	public ProgramTypes ProgramType { get; set; }
	public int LengthWeeks { get; set; }
	public int Workouts { get; set; }
	public DateTimeOffset? DateStarted { get; set; }
}
