using Microsoft.AspNetCore.Components;
using Unshackled.Fitness.My.Client.Components;
using Unshackled.Fitness.My.Client.Features.Exercises.Actions;
using Unshackled.Fitness.My.Client.Features.Exercises.Models;
using Unshackled.Fitness.My.Client.Models;

namespace Unshackled.Fitness.My.Client.Features.Exercises;

public class SectionRecordsBase : BaseSectionComponent
{
	[Parameter] public ExerciseModel Exercise { get; set; } = new();

	protected AppSettings AppSettings => State.ActiveMember.Settings;

	protected bool IsLoading { get; set; } = true;
	protected List<RecordListModel> Records { get; set; } = new();
	protected RecordListModel MaxWeight { get; set; } = new();
	protected RecordListModel MaxVolume { get; set; } = new();
	protected RecordListModel MaxReps { get; set; } = new();
	protected RecordListModel MaxSeconds { get; set; } = new();
	protected RecordListModel MaxWeightSeconds { get; set; } = new();

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();

		Records = await Mediator.Send(new ListPersonalRecords.Query(Exercise.Sid));

		MaxWeight = Records.Where(x => x.RecordType == RecordListModel.RecordTypes.Weight).FirstOrDefault() ?? new();
		MaxVolume = Records.Where(x => x.RecordType == RecordListModel.RecordTypes.Volume).FirstOrDefault() ?? new();
		MaxReps = Records.Where(x => x.RecordType == RecordListModel.RecordTypes.Reps).FirstOrDefault() ?? new();
		MaxSeconds = Records.Where(x => x.RecordType == RecordListModel.RecordTypes.Time).FirstOrDefault() ?? new();
		MaxWeightSeconds = Records.Where(x => x.RecordType == RecordListModel.RecordTypes.WeightTime).FirstOrDefault() ?? new();

		IsLoading = false;
	}
}