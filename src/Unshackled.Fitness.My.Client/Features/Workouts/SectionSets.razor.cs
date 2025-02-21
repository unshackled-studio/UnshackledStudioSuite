using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using MudBlazor.Utilities;
using Unshackled.Fitness.Core.Enums;
using Unshackled.Fitness.My.Client.Components;
using Unshackled.Fitness.My.Client.Features.Workouts.Actions;
using Unshackled.Fitness.My.Client.Features.Workouts.Models;
using Unshackled.Fitness.My.Client.Models;

namespace Unshackled.Fitness.My.Client.Features.Workouts;

public class SectionSetsBase : BaseSectionComponent
{
	[Inject] protected IDialogService DialogService { get; set; } = default!;
	[Parameter] public FormWorkoutModel Workout { get; set; } = new();
	[Parameter] public EventCallback OnSetSaved { get; set; }
	[Parameter] public EventCallback<bool> OnHasUnrecordedSetsChanged { get; set; }

	protected int ActiveSetIdx { get; set; }

	protected bool DisableControls => IsWorking
		|| (DisableSectionControls && IsEditMode);

	protected string DrawerIcon => OpenStats
		? Icons.Material.Filled.History
		: OpenNotes ? Icons.Material.Filled.StickyNote2
		: OpenAddSet ? Icons.Material.Filled.AddCircleOutline
		: Icons.Material.Filled.Edit;

	protected bool DrawerOpen => OpenStats || OpenNotes || OpenAddSet || OpenProperties;
	protected AppSettings AppSettings => State.ActiveMember.Settings;

	protected string DrawerTitle => OpenStats
		? "Previous Stats"
		: OpenNotes ? "Set Notes"
		: OpenAddSet ? "Add Set"
		: "Edit Set Properties";

	protected FormWorkoutSetModel DrawerWorkoutSet { get; set; } = new();

	protected FormSetPropertiesModel DrawerProperties { get; set; } = new();

	protected bool HideCompleted { get; set; } = true;

	protected bool IsEditingWorkout => State.ActiveMember.IsActive && IsWorkoutStarted && (!IsWorkoutComplete || IsEditMode);

	protected bool IsWorking { get; set; }

	protected bool IsWorkoutComplete => Workout.DateStartedUtc.HasValue && Workout.DateCompletedUtc.HasValue;

	protected bool IsWorkoutStarted => Workout.DateStartedUtc.HasValue;

	protected bool IsPositiveIncrement { get; set; } = true;

	protected bool OpenAddSet { get; set; } = false;

	protected bool OpenNotes { get; set; } = false;

	protected bool OpenProperties { get; set; } = false;

	protected bool OpenStats { get; set; } = false;

	protected WeightUnits TotalWeightUnit => Workout.Sets.Any() ? Workout.Sets[0].WeightUnit : WeightUnits.kg;

	protected string SplitTrackingCss => new CssBuilder("d-flex flex-wrap")
		.AddClass(AppSettings.DisplaySplitTracking == 0
			? "flex-column flex-sm-row"
			: "flex-column-reverse flex-sm-row-reverse justify-end")
		.Build();

	private bool hasUnrecordedSets = false;

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();

		HideCompleted = !IsWorkoutComplete && AppSettings.HideCompleteSets;

		if (Workout.Sets.Any())
		{
			ActiveSetIdx = Workout.GetFirstUnrecordedIndex();
			if (ActiveSetIdx >= 0)
			{
				// Expand first if workout is not complete.
				Workout.Sets[ActiveSetIdx].IsExpanded = !IsWorkoutComplete;
				await SetInitialValuesForActiveSet();
			}

			hasUnrecordedSets = Workout.Sets.Where(x => x.DateRecordedUtc == null).Any();
		}
	}

	private void DeleteGroupIfEmpty(string sid)
	{
		// more than one group and no more sets in group
		if (Workout.Groups.Count > 1 && !Workout.Sets.Where(x => x.ListGroupSid == sid).Any())
		{
			// Delete group
			var group = Workout.Groups.Where(x => x.Sid == sid).FirstOrDefault();
			if (group != null)
				Workout.Groups.Remove(group);
		}
	}

	private async Task GetNextActiveIdx()
	{
		// get index of next unrecorded set
		var idx = Workout.GetFirstUnrecordedIndex();
		if (idx >= 0)
		{
			ActiveSetIdx = idx;
			Workout.Sets[ActiveSetIdx].IsExpanded = true;
			await SetInitialValuesForActiveSet();
		}
		else // nothing unrecorded was found
		{
			ActiveSetIdx = Workout.Sets.Count;
		}
	}

	protected string? GetRepsIcon(FormWorkoutSetModel set)
	{
		return set.IsRecordReps ? Icons.Material.Filled.EmojiEvents : null;
	}

	protected string? GetSecondsIcon(FormWorkoutSetModel set)
	{
		return set.IsRecordSeconds || set.IsRecordSecondsAtWeight ? Icons.Material.Filled.EmojiEvents : null;
	}

	protected string? GetSecondsTitle(FormWorkoutSetModel set)
	{
		string? title = null;
		if (set.IsRecordSeconds)
			title = "Time PR";

		if (set.IsRecordSecondsAtWeight && set.Weight.HasValue)
		{
			string t = $"Time PR @ {set.Weight.Value.ToString("0.#")}{set.WeightUnit.Label()}";
			if (string.IsNullOrEmpty(title))
				title = t;
			else
				title = $"{title}, {t}";
		}

		return title;
	}

	protected string? GetWeightIcon(FormWorkoutSetModel set)
	{
		return set.IsRecordWeight || set.IsRecordTargetWeight ? Icons.Material.Filled.EmojiEvents : null;
	}

	protected string? GetWeightTitle(FormWorkoutSetModel set)
	{
		if (set.IsRecordWeight)
			return "Weight PR";
		else if (set.IsRecordTargetWeight)
			return $"Weight @ {set.RepsTarget}R";
		else
			return null;
	}

	protected string? GetVolumeIcon(FormWorkoutSetModel set)
	{
		return set.IsRecordVolume || set.IsRecordTargetVolume ? Icons.Material.Filled.EmojiEvents : null;
	}

	protected string? GetVolumeTitle(FormWorkoutSetModel set)
	{
		if (set.IsRecordVolume)
			return "Volume PR";
		else if (set.IsRecordTargetVolume)
			return $"Volume @ {set.RepsTarget}R";
		else
			return null;
	}

	protected async Task HandleAddDuplicateClicked(FormWorkoutSetModel set)
	{
		IsWorking = true;
		var result = await Mediator.Send(new DuplicateSet.Command(set));

		if (result.Success)
		{
			int idx = Workout.Sets.IndexOf(set) + 1;

			if (result.Payload != null)
				Workout.Sets.Insert(idx, result.Payload);

			for (int i = 0; i < Workout.Sets.Count; i++)
			{
				Workout.Sets[i].SortOrder = i;
			}
			await GetNextActiveIdx();
			await CheckIfWorkoutHasUnrecordedSets();
		}
		else
		{
			ShowNotification(result);
		}
		IsWorking = false;
		StateHasChanged();
	}

	protected void HandleAddSetClicked()
	{
		OpenAddSet = true;
	}

	protected async Task HandleAddSets(ExercisePickerResult pickerResult)
	{
		OpenAddSet = false;
		if (pickerResult != null)
		{
			IsWorking = true;
			var group = Workout.Groups.LastOrDefault();
			if (group != null)
			{
				var set = new FormWorkoutSetModel()
				{
					Equipment = pickerResult.Equipment,
					ExerciseSid = pickerResult.ExerciseSid,
					ListGroupSid = group.Sid,
					IsTrackingSplit = pickerResult.IsTrackingSplit,
					Muscles = pickerResult.Muscles,
					RepMode = RepModes.Exact,
					SetMetricType = pickerResult.SetMetricType,
					SetType = pickerResult.SetType,
					SortOrder = Workout.Sets.Count,
					Title = pickerResult.Title,
					WorkoutSid = group.WorkoutSid
				};
				var result = await Mediator.Send(new AddSet.Command(set));
				if (result.Success && result.Payload != null)
				{
					var newSet = result.Payload;
					newSet.IsExpanded = true;
					Workout.Sets.Add(newSet);
					Workout.Sets = Workout.Sets
						.OrderBy(x => x.SortOrder)
						.ToList();

					await GetNextActiveIdx();
					await CheckIfWorkoutHasUnrecordedSets();
				}
				else
				{
					ShowNotification(result);
				}
			}
			IsWorking = false;
		}
	}

	protected async Task HandleDeleteIncompleteClicked()
	{
		bool? confirm = await DialogService.ShowMessageBox(
				"Confirm Delete",
				"Are you sure you want to delete the remaining incomplete sets?",
				yesText: "Delete", cancelText: "Cancel");

		if (confirm.HasValue && confirm.Value)
		{
			IsWorking = true;
			StateHasChanged();

			var result = await Mediator.Send(new DeleteIncompleteSets.Command(Workout.Sid));

			if (result.Success)
			{
				var incomplete = Workout.Sets.Where(x => x.DateRecordedUtc == null).ToList();
				foreach (var set in incomplete)
				{
					Workout.Sets.Remove(set);
					DeleteGroupIfEmpty(set.ListGroupSid);
				}
				if (Workout.Sets.Count > 0)
				{
					// Update sort orders
					for (int i = 0; i < Workout.Sets.Count; i++)
					{
						Workout.Sets[i].SortOrder = i;
					}

					// Get new active index
					await GetNextActiveIdx();
				}
				await CheckIfWorkoutHasUnrecordedSets();
			}
			else
			{
				ShowNotification(result);
			}
			IsWorking = false;
			StateHasChanged();
		}
	}

	protected async Task HandleDeleteSetClicked(FormWorkoutSetModel set)
	{
		bool? confirm = await DialogService.ShowMessageBox(
				"Confirm Delete",
				"Are you sure you want to delete this set?",
				yesText: "Delete", cancelText: "Cancel");

		if (confirm.HasValue && confirm.Value)
		{
			IsWorking = true;
			StateHasChanged();

			var result = await Mediator.Send(new DeleteSet.Command(set.Sid));

			if (result.Success)
			{
				Workout.Sets.Remove(set);
				DeleteGroupIfEmpty(set.ListGroupSid);

				if (Workout.Sets.Count > 0)
				{
					// Update sort orders
					for (int i = 0; i < Workout.Sets.Count; i++)
					{
						Workout.Sets[i].SortOrder = i;
					}

					// Get new active index
					await GetNextActiveIdx();
				}
				await CheckIfWorkoutHasUnrecordedSets();
			}
			else
			{
				ShowNotification(result);
			}
			IsWorking = false;
			StateHasChanged();
		}
	}

	protected void HandleDrawerOpenChange(bool value)
	{
		if (!value)
		{
			OpenStats = false;
			OpenNotes = false;
			OpenAddSet = false;
			OpenProperties = false;
			DrawerWorkoutSet = new();
			DrawerProperties = new();
		}
	}

	protected void HandleEditSetClicked(FormWorkoutSetModel set)
	{
		DrawerWorkoutSet = set;
		DrawerProperties = new()
		{
			IntensityTarget = set.IntensityTarget,
			RepMode = set.RepMode,
			RepsTarget = set.RepsTarget,
			SecondsTarget = set.SecondsTarget,
			SetMetricType = set.SetMetricType,
			SetSid = set.Sid,
			SetType = set.SetType
		};
		OpenProperties = true;
	}

	protected void HandleIncrementClicked(FormWorkoutSetModel set, decimal amount)
	{
		if (set.Weight.HasValue)
			set.Weight += IsPositiveIncrement ? amount : -amount;
		else
			set.Weight = IsPositiveIncrement ? amount : 0;
	}

	protected void HandleOpenExerciseClicked(FormWorkoutSetModel set)
	{
		NavManager.NavigateTo($"/exercises/{set.ExerciseSid}?workout={Workout.Sid}");
	}

	protected void HandleOpenNotesClicked(FormWorkoutSetModel set)
	{
		OpenNotes = true;
		DrawerWorkoutSet = set;
	}

	protected void HandleOpenStatsClicked(FormWorkoutSetModel set)
	{
		OpenStats = true;
		DrawerWorkoutSet = set;
	}

	protected async Task HandleSaveNote(FormSetNoteModel model)
	{
		IsWorking = true;
		var result = await Mediator.Send(new SaveSetNote.Command(model));
		if (result.Success)
		{
			DrawerWorkoutSet.Notes = model.Notes;
		}
		HandleDrawerOpenChange(false);
		IsWorking = false;
		ShowNotification(result);
		StateHasChanged();
	}

	protected async Task HandleSetPropertiesChanged(FormSetPropertiesModel model)
	{
		OpenProperties = false;
		var result = await Mediator.Send(new UpdateSetProperties.Command(model));
		if (result.Success)
		{
			DrawerWorkoutSet.IntensityTarget = model.IntensityTarget;
			DrawerWorkoutSet.RepMode = model.RepMode;
			DrawerWorkoutSet.RepsTarget = model.RepsTarget;
			DrawerWorkoutSet.SecondsTarget = model.SecondsTarget;
			DrawerWorkoutSet.SetMetricType = model.SetMetricType;
			DrawerWorkoutSet.SetType = model.SetType;
		}
		ShowNotification(result);

		DrawerWorkoutSet = new();
		DrawerProperties = new();
	}

	protected async Task HandleSetSaved(EditContext context)
	{
		IsWorking = true;
		var set = (FormWorkoutSetModel)context.Model;
		set.IsSaving = true;
		var setIdx = Workout.Sets.IndexOf(set);

		if (!set.DateRecorded.HasValue)
		{
			set.DateRecorded = DateTime.Now;
			set.DateRecordedUtc = set.DateRecorded.Value.ToUniversalTime();
		}
		var result = await Mediator.Send(new SaveSet.Command(set));
		set.IsSaving = false;

		if (result.Success)
		{
			Workout.Sets[setIdx].IsExpanded = false;

			if (result.Payload != null)
			{
				Workout.Sets[setIdx] = result.Payload;
			}

			if (ActiveSetIdx == setIdx)
			{
				await GetNextActiveIdx();
			}
			else
			{
				// Make sure current active set is visible
				if (ActiveSetIdx >= 0 && ActiveSetIdx < Workout.Sets.Count)
					Workout.Sets[ActiveSetIdx].IsExpanded = true;
			}

			await OnSetSaved.InvokeAsync();
			await CheckIfWorkoutHasUnrecordedSets();
		}
		else
		{
			ShowNotification(result);
		}
		IsWorking = false;
		StateHasChanged();
	}

	protected void HandleToggleExpanded(FormWorkoutSetModel set)
	{
		set.IsExpanded = !set.IsExpanded;
	}

	protected async Task HandleSortChanged(SortableGroupResult<FormWorkoutSetGroupModel, FormWorkoutSetModel> sortResult)
	{
		IsWorking = true;

		UpdateSortsModel model = new()
		{
			DeletedGroups = sortResult.DeletedGroups,
			Groups = sortResult.Groups,
			Sets = sortResult.Items,
			WorkoutSid = Workout.Sid
		};
		var result = await Mediator.Send(new UpdateSetSorts.Command(model));
		if (result.Success)
		{
			Workout.Groups = sortResult.Groups
				.OrderBy(x => x.SortOrder)
				.ToList();
			Workout.Sets = sortResult.Items
				.OrderBy(x => x.SortOrder)
				.ToList();

			await GetNextActiveIdx();
		}

		IsWorking = false;
		StateHasChanged();
		ShowNotification(result);
	}

	protected bool HasPR(FormWorkoutSetModel set)
	{
		return set.IsRecordTargetVolume || set.IsRecordTargetWeight || set.IsRecordVolume || set.IsRecordWeight || set.IsRecordSeconds || set.IsRecordSecondsAtWeight || set.IsRecordReps;
	}

	protected string IsExpandedClass(FormWorkoutSetModel set, string? additionalClasses = null)
	{
		string state = set.IsExpanded ? "d-block" : "d-none";
		if (!string.IsNullOrEmpty(additionalClasses))
			return $"{additionalClasses} {state}";
		else
			return state;
	}

	protected bool IsGroupVisible(FormWorkoutSetGroupModel group)
	{
		return !HideCompleted || Workout.Sets
			.Where(x => x.DateRecordedUtc == null && x.ListGroupSid == group.Sid)
			.Any();
	}

	protected bool IsLastInGroup(FormWorkoutSetModel set)
	{
		int idx = Workout.Sets.IndexOf(set);

		// Last set overall
		if (idx == Workout.Sets.Count - 1) return true;

		// Last set in group
		if (set.ListGroupSid != Workout.Sets[idx + 1].ListGroupSid) return true;

		return false;
	}

	private async Task SetInitialValuesForActiveSet(FormWorkoutSetModel? copiedSet = null)
	{
		if (ActiveSetIdx < 0) return;
		if (ActiveSetIdx >= Workout.Sets.Count) return;

		// Don't change anything if actual values have already been recorded.
		if (Workout.Sets[ActiveSetIdx].DateRecordedUtc.HasValue) return;

		// Copy from set we are duplicating
		if (copiedSet != null)
		{
			Workout.Sets[ActiveSetIdx].Weight = copiedSet.Weight;
			Workout.Sets[ActiveSetIdx].RepsLeft = copiedSet.RepsLeft;
			Workout.Sets[ActiveSetIdx].RepsRight = copiedSet.RepsRight;
			Workout.Sets[ActiveSetIdx].Reps = copiedSet.Reps;
			return;
		}

		// Copy weight/reps from previous set with the same exercise and set type
		var prevSet = Workout.Sets
			.Where(x => x.ExerciseSid == Workout.Sets[ActiveSetIdx].ExerciseSid
				&& x.SetType == Workout.Sets[ActiveSetIdx].SetType && x.DateRecordedUtc.HasValue)
			.LastOrDefault();

		if (prevSet != null)
		{
			Workout.Sets[ActiveSetIdx].Weight = prevSet.Weight;
			Workout.Sets[ActiveSetIdx].RepsLeft = prevSet.RepsLeft;
			Workout.Sets[ActiveSetIdx].RepsRight = prevSet.RepsRight;
			Workout.Sets[ActiveSetIdx].Reps = prevSet.Reps;
		}
		else
		{
			// Default to target reps
			if (Workout.Sets[ActiveSetIdx].RepsTarget > 0)
			{
				if (Workout.Sets[ActiveSetIdx].IsTrackingSplit)
				{
					Workout.Sets[ActiveSetIdx].RepsLeft = Workout.Sets[ActiveSetIdx].RepsTarget;
					Workout.Sets[ActiveSetIdx].RepsRight = Workout.Sets[ActiveSetIdx].RepsTarget;
				}
				else
				{
					Workout.Sets[ActiveSetIdx].Reps = Workout.Sets[ActiveSetIdx].RepsTarget;
				}
			}

			// Default to target time
			if (Workout.Sets[ActiveSetIdx].SecondsTarget > 0)
			{
				if (Workout.Sets[ActiveSetIdx].IsTrackingSplit)
				{
					Workout.Sets[ActiveSetIdx].SecondsLeft = Workout.Sets[ActiveSetIdx].SecondsTarget;
					Workout.Sets[ActiveSetIdx].SecondsRight = Workout.Sets[ActiveSetIdx].SecondsTarget;
				}
				else
				{
					Workout.Sets[ActiveSetIdx].Seconds = Workout.Sets[ActiveSetIdx].SecondsTarget;
				}
			}

			// Get last set from workout history
			SearchSetModel searchModel = new()
			{
				ExcludeWorkoutSid = Workout.Sid,
				ExerciseSid = Workout.Sets[ActiveSetIdx].ExerciseSid,
				RepMode = Workout.Sets[ActiveSetIdx].RepMode,
				RepsTarget = Workout.Sets[ActiveSetIdx].RepsTarget,
				PageSize = 1,
				Page = 1
			};

			var results = await Mediator.Send(new SearchCompletedSets.Query(searchModel));
			if (results.Total > 0)
			{
				var completedSet = results.Data.Where(x => x.SetType == Workout.Sets[ActiveSetIdx].SetType).FirstOrDefault();
				if (completedSet != null)
				{
					Workout.Sets[ActiveSetIdx].Weight = TotalWeightUnit == WeightUnits.kg ? completedSet.WeightKg : completedSet.WeightLb;
				}
			}
		}
	}

	protected bool ShowSet(FormWorkoutSetModel model)
	{
		// Show if 
		// - HideCompleted is off
		// - No Date has been recorded
		// - Model is actively saving
		return !HideCompleted || !model.DateRecordedUtc.HasValue || model.IsSaving;
	}

	private async Task CheckIfWorkoutHasUnrecordedSets()
	{
		var unrecorded = Workout.Sets.Where(x => x.DateRecordedUtc == null).Any();
		if (unrecorded != hasUnrecordedSets)
		{
			hasUnrecordedSets = unrecorded;
			await OnHasUnrecordedSetsChanged.InvokeAsync(hasUnrecordedSets);
		}
	}
}