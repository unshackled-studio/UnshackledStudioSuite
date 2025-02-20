﻿using System.Text.Json.Serialization;
using Unshackled.Fitness.My.Client.Models;
using Unshackled.Fitness.My.Client.Utils;

namespace Unshackled.Fitness.My.Client.Features.Workouts.Models;

public class WorkoutListModel : BaseObject
{
	public string Title { get; set; } = string.Empty;
	public DateTimeOffset? DateStartedUtc { get; set; }
	public DateTimeOffset? DateCompletedUtc { get; set; }
	public string? MusclesTargeted { get; set; }
	public int ExerciseCount { get; set; }
	public int SetCount { get; set; }
	public int RepCount { get; set; }
	public decimal VolumeKg { get; set; }
	public decimal VolumeLb { get; set; }
	public int Rating { get; set; }
	public string? Notes { get; set; }

	[JsonIgnore]
	public TimeSpan TotalTime => Calculator.TotalTime(DateStartedUtc, DateCompletedUtc);
}
