﻿using System.Text.Json.Serialization;
using Unshackled.Fitness.My.Client.Models;
using Unshackled.Fitness.My.Client.Utils;

namespace Unshackled.Fitness.My.Client.Features.Workouts.Models;

public class FormWorkoutModel : BaseMemberObject
{
	public string Title { get; set; } = string.Empty;
	public DateTime? DateStarted { get; set; }
	public DateTimeOffset? DateStartedUtc { get; set; }
	public DateTime? DateCompleted { get; set; }
	public DateTimeOffset? DateCompletedUtc { get; set; }
	public int RecordSecondsCount { get; set; }
	public int RecordSecondsAtWeightCount { get; set; }
	public int RecordTargetVolumeCount { get; set; }
	public int RecordTargetWeightCount { get; set; }
	public int RecordVolumeCount { get; set; }
	public int RecordWeightCount { get; set; }
	public int Rating { get; set; }
	public string? Notes { get; set; }

	[JsonIgnore]
	public TimeSpan TotalWorkoutTime => Calculator.TotalTime(DateStartedUtc, DateCompletedUtc);

	[JsonIgnore]
	public int AdjustedRecordTargetVolumeCount => RecordTargetVolumeCount - RecordVolumeCount;

	[JsonIgnore]
	public int AdjustedRecordTargetWeightCount => RecordTargetWeightCount - RecordWeightCount;

	public List<FormWorkoutSetGroupModel> Groups { get; set; } = new();
	public List<FormWorkoutSetModel> Sets { get; set; } = new();
	public List<FormWorkoutTaskModel> Tasks { get; set; } = new();

	public int GetFirstUnrecordedIndex()
	{
		int idx = -1;
		var firstUnrecorded = Sets.Where(x => x.DateRecordedUtc == null).FirstOrDefault();
		if (firstUnrecorded != null)
		{
			idx = Sets.IndexOf(firstUnrecorded);
		}
		return idx;
	}
}
