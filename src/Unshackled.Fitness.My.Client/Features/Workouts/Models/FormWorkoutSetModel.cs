﻿using System.Text.Json.Serialization;
using FluentValidation;
using Unshackled.Fitness.Core;
using Unshackled.Fitness.Core.Enums;
using Unshackled.Fitness.Core.Interfaces;
using Unshackled.Fitness.My.Client.Models;
using Unshackled.Fitness.My.Client.Utils;

namespace Unshackled.Fitness.My.Client.Features.Workouts.Models;

public class FormWorkoutSetModel : BaseObject, IGroupedSortable, ICloneable
{
	public string WorkoutSid { get; set; } = string.Empty;
	public string ExerciseSid { get; set; } = string.Empty;
	public string ListGroupSid { get; set; } = string.Empty;
	public string Title { get; set; } = string.Empty;
	public bool IsTrackingSplit { get; set; }
	public WorkoutSetTypes SetType { get; set; }
	public SetMetricTypes SetMetricType { get; set; }
	public List<MuscleTypes> Muscles { get; set; } = new();
	public List<EquipmentTypes> Equipment { get; set; } = new();
	public string? ExerciseNotes { get; set; }
	public int SortOrder { get; set; }
	public RepModes RepMode { get; set; }
	public int RepsTarget { get; set; }
	public int IntensityTarget { get; set; }
	public int? Reps { get; set; }
	public int? RepsLeft { get; set; }
	public int? RepsRight { get; set; }
	public int? Seconds { get; set; }
	public int? SecondsLeft { get; set; }
	public int? SecondsRight { get; set; }
	public int? SecondsTarget { get; set; }
	public decimal? Weight { get; set; }
	public WeightUnits WeightUnit { get; set; } = WeightUnits.lb;
	public DateTime? DateRecorded { get; set; }
	public DateTimeOffset? DateRecordedUtc { get; set; }
	public bool IsBestSetByReps { get; set; }
	public bool IsBestSetBySeconds { get; set; }
	public bool IsBestSetByVolume { get; set; }
	public bool IsBestSetByWeight { get; set; }
	public bool IsRecordTargetVolume { get; set; }
	public bool IsRecordTargetWeight { get; set; }
	public bool IsRecordReps { get; set; }
	public bool IsRecordSeconds { get; set; }
	public bool IsRecordSecondsAtWeight { get; set; }
	public bool IsRecordVolume { get; set; }
	public bool IsRecordWeight { get; set; }
	public string? Notes { get; set; }

	[JsonIgnore]
	public bool IsExpanded { get; set; } = false;

	[JsonIgnore]
	public bool IsSaving { get; set; } = false;

	[JsonIgnore]
	public bool HasTarget => RepsTarget > 0;

	[JsonIgnore]
	public TimeSpan? TargetTimeSeconds
	{
		get => SecondsTarget != null && SecondsTarget > 0 ? new(0, 0, SecondsTarget.Value) : null;
		set
		{
			SecondsTarget = value.HasValue ? (int)Math.Round(value.Value.TotalSeconds, 0, MidpointRounding.AwayFromZero) : 0;
		}
	}

	[JsonIgnore]
	public decimal Volume => IsTrackingSplit
		? Calculator.Volume(Weight, RepsLeft, RepsRight)
		: Calculator.Volume(Weight, Reps);

	[JsonIgnore]
	public Validator ModelValidator { get; set; } = new();

	[JsonIgnore]
	public bool RepsRequired => SetMetricType == SetMetricTypes.WeightReps || SetMetricType == SetMetricTypes.RepsOnly;

	[JsonIgnore]
	public bool WeightRequired => SetMetricType == SetMetricTypes.WeightReps || SetMetricType == SetMetricTypes.WeightTime;

	public object Clone()
	{
		return new FormWorkoutSetModel
		{
			DateCreatedUtc = DateCreatedUtc,
			DateLastModifiedUtc = DateLastModifiedUtc,
			DateRecorded = DateRecorded,
			DateRecordedUtc = DateRecordedUtc,
			Equipment = Equipment,
			ExerciseNotes = ExerciseNotes,
			SetMetricType = SetMetricType,
			ExerciseSid = ExerciseSid,
			ListGroupSid = ListGroupSid,
			IntensityTarget = IntensityTarget,
			IsExpanded = IsExpanded,
			IsSaving = IsSaving,
			IsTrackingSplit = IsTrackingSplit,
			Muscles = Muscles,
			RepMode = RepMode,
			Reps = Reps,
			RepsLeft = RepsLeft,
			RepsRight = RepsRight,
			RepsTarget = RepsTarget,
			Seconds = Seconds,
			SecondsLeft = SecondsLeft,
			SecondsRight = SecondsRight,
			SecondsTarget = SecondsTarget,
			SetType = SetType,
			Sid = Sid,
			SortOrder = SortOrder,
			Title = Title,
			Weight = Weight,
			WeightUnit = WeightUnit,
			WorkoutSid = WorkoutSid
		};
	}

	public class Validator : BaseValidator<FormWorkoutSetModel>
	{
		public Validator()
		{
			RuleFor(x => x.Weight)
				.NotNull()
				.When(x => x.WeightRequired == true)
				.WithMessage("Required")
				.GreaterThanOrEqualTo(0)
				.When(x => x.WeightRequired == true)
				.WithMessage("Weight must be a positive number.")
				.LessThanOrEqualTo(Globals.MaxSetWeight)
				.When(x => x.WeightRequired == true)
				.WithMessage($"Weight must be less than {Math.Ceiling(Globals.MaxSetWeight)}.");

			RuleFor(x => x.Reps)
				.NotNull()
				.When(x => x.IsTrackingSplit == false && x.RepsRequired == true)
				.WithMessage("Required")
				.GreaterThanOrEqualTo(1)
				.When(x => x.IsTrackingSplit == false && x.RepsRequired == true)
				.WithMessage("Reps must be greater than zero.")
				.LessThanOrEqualTo(Globals.MaxSetReps)
				.When(x => x.IsTrackingSplit == false && x.RepsRequired == true)
				.WithMessage($"Reps must be less than or equal to {Globals.MaxSetReps}.");

			RuleFor(x => x.RepsLeft)
				.NotNull()
				.When(x => x.IsTrackingSplit == true && x.RepsRequired == true)
				.WithMessage("Required")
				.GreaterThanOrEqualTo(1)
				.When(x => x.IsTrackingSplit == true && x.RepsRequired == true && x.RepsRight == 0)
				.WithMessage("Left reps must be greater than zero.")
				.LessThanOrEqualTo(Globals.MaxSetReps)
				.When(x => x.IsTrackingSplit == true && x.RepsRequired == true && x.RepsRight == 0)
				.WithMessage($"Left reps must be less than or equal to {Globals.MaxSetReps}.");

			RuleFor(x => x.RepsRight)
				.NotNull()
				.When(x => x.IsTrackingSplit == true && x.RepsRequired == true)
				.WithMessage("Required.")
				.GreaterThanOrEqualTo(1)
				.When(x => x.IsTrackingSplit == true && x.RepsRequired == true && x.RepsLeft == 0)
				.WithMessage("Right reps must be greater than zero.")
				.LessThanOrEqualTo(Globals.MaxSetReps)
				.When(x => x.IsTrackingSplit == true && x.RepsRequired == true && x.RepsLeft == 0)
				.WithMessage($"Right reps must be less than or equal to {Globals.MaxSetReps}.");
		}
	}
}
