﻿using System.Text.Json.Serialization;

namespace Unshackled.Fitness.My.Client.Features.Exercises.Models;

public class RecordListModel
{
	public enum RecordTypes
	{
		Volume,
		Weight,
		Time,
		WeightTime,
		Reps
	}

	public string WorkoutSid { get; set; } = string.Empty;
	public string WorkoutSetSid { get; set; } = string.Empty;
	public DateTimeOffset DateWorkoutUtc { get; set; }
	public decimal WeightLb { get; set; }
	public decimal WeightKg { get; set; }
	public decimal VolumeLb { get; set; }
	public decimal VolumeKg { get; set; }
	public int Reps { get; set; }
	public int RepsLeft { get; set; }
	public int RepsRight { get; set; }
	public int Seconds { get; set; }
	public int SecondsLeft { get; set; }
	public int SecondsRight { get; set; }
	public RecordTypes RecordType { get; set; }

	[JsonIgnore]
	public int MaxReps => Math.Max(Reps, Math.Max(RepsLeft, RepsRight));

	[JsonIgnore]
	public int MaxSeconds => Math.Max(Seconds, Math.Max(SecondsLeft, SecondsRight));
}
