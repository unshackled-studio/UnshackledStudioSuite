using Microsoft.AspNetCore.Components;
using MudBlazor;
using Unshackled.Fitness.Core.Configuration;
using Unshackled.Fitness.Core.Enums;
using Unshackled.Fitness.My.Client.Components;
using Unshackled.Fitness.My.Client.Features.TrainingSessions.Actions;
using Unshackled.Fitness.My.Client.Features.TrainingSessions.Models;
using Unshackled.Fitness.My.Client.Models;

namespace Unshackled.Fitness.My.Client.Features.TrainingSessions;

public class SectionPropertiesBase : BaseSectionComponent
{
	[Inject] protected IDialogService DialogService { get; set; } = default!;
	[Parameter] public TrainingSessionModel Session { get; set; } = new();
	[Parameter] public EventCallback<TrainingSessionModel> SessionChanged { get; set; }
	[Parameter] public List<ActivityTypeListModel> ActivityTypes { get; set; } = [];

	protected const string FormId = "formTrainingSession";
	protected bool IsUpdating { get; set; } = false;
	protected bool IsDuplicating { get; set; } = false;
	protected FormSessionModel Model { get; set; } = new();
	protected AppSettings AppSettings => State.ActiveMember.Settings;

	public bool DisableControls => IsUpdating;
	public int StatElevation => IsEditMode ? 0 : 1;

	protected async Task HandleDeleteClicked()
	{
		bool? confirm = await DialogService.ShowMessageBox(
				"Warning",
				"Are you sure you want to delete this training session?",
				yesText: "Delete", cancelText: "Cancel");

		if (confirm.HasValue && confirm.Value)
		{
			await UpdateIsEditingSection(true);

			var result = await Mediator.Send(new DeleteSession.Command(Session.Sid));
			ShowNotification(result);
			if (result.Success)
			{
				NavManager.NavigateTo("/training-sessions");
			}
		}
	}

	protected async Task HandleDuplicateClicked()
	{
		FillModel();
		Model.Sid = string.Empty;
		IsDuplicating = await UpdateIsEditingSection(true);
	}

	protected async Task HandleEditClicked()
	{
		FillModel();		
		IsEditing = await UpdateIsEditingSection(true);
	}

	protected async Task HandleCancelClicked()
	{
		IsDuplicating = false;
		IsEditing = await UpdateIsEditingSection(false);
	}

	protected async Task HandleDuplicateFormSubmitted(FormSessionModel model)
	{
		var result = await Mediator.Send(new DuplicateSession.Command(Session.Sid, model));
		ShowNotification(result);
		if (result.Success)
		{
			NavManager.NavigateTo($"/training-sessions/{result.Payload}");
		}
		IsDuplicating = await UpdateIsEditingSection(false);
	}

	protected async Task HandleEditFormSubmitted(FormSessionModel model)
	{
		IsUpdating = true;
		var result = await Mediator.Send(new UpdateProperties.Command(model));
		ShowNotification(result);
		if (result.Success)
		{
			Session.ActivityTypeSid = model.ActivityTypeSid;
			Session.EventType = model.EventType;
			Session.TargetCadence = model.TargetCadence;
			Session.TargetCadenceUnit = model.TargetCadenceUnit;
			Session.TargetCalories = model.TargetCalories;
			Session.TargetDistance = model.TargetDistance;
			Session.TargetDistanceUnit = model.TargetDistanceUnit;
			Session.TargetHeartRateBpm = model.TargetHeartRateBpm;
			Session.TargetPace = model.TargetPace;
			Session.TargetPower = model.TargetPower;
			Session.TargetTimeSeconds = model.TargetTimeSeconds;
			Session.Title = model.Title;

			if (SessionChanged.HasDelegate)
				await SessionChanged.InvokeAsync(Session);
		}
		IsUpdating = false;
		IsEditing = await UpdateIsEditingSection(false);
		StateHasChanged();
	}

	private void FillModel()
	{
		bool isMetric = ((Member)State.ActiveMember).Settings.DefaultUnits == UnitSystems.Metric;
		Model = new();
		Model.SetUnits(isMetric);

		Model.ActivityTypeSid = Session.ActivityTypeSid;
		Model.EventType = Session.EventType;
		Model.Notes = Session.Notes;
		Model.Sid = Session.Sid;
		Model.TargetCadence = Session.TargetCadence;
		Model.TargetCadenceUnit = Session.TargetCadenceUnit;
		Model.TargetCalories = Session.TargetCalories;
		Model.TargetDistance = Session.TargetDistance;
		Model.TargetDistanceUnit = Session.TargetDistanceUnit;
		Model.TargetHeartRateBpm = Session.TargetHeartRateBpm;
		Model.TargetPace = Session.TargetPace;
		Model.TargetPower = Session.TargetPower;
		Model.TargetTimeSeconds = Session.TargetTimeSeconds;
		Model.Title = Session.Title;
	}
}