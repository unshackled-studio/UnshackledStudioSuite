﻿using MudBlazor;
using Unshackled.Fitness.My.Client.Components;
using Unshackled.Fitness.My.Client.Features.WorkoutTemplates.Actions;
using Unshackled.Fitness.My.Client.Features.WorkoutTemplates.Models;

namespace Unshackled.Fitness.My.Client.Features.WorkoutTemplates;

public partial class IndexBase : BaseSearchComponent<SearchTemplateModel, TemplateListItem>
{
	protected override bool DisableControls => IsLoading || IsWorking;
	protected FormTemplateModel FormModel { get; set; } = new();
	protected string TrackNowSid { get; set; } = string.Empty;
	protected string DrawerIcon => Icons.Material.Filled.AddCircle;
	protected bool DrawerOpen { get; set; } = false;
	protected string DrawerTitle => "Add New Template";

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		SearchKey = "SearchWorkoutTemplates";

		Breadcrumbs.Add(new BreadcrumbItem("Templates", null, true));

		SearchModel = await GetLocalSetting(SearchKey) ?? new();

		await DoSearch(SearchModel.Page);
	}

	protected async override Task DoSearch(int page)
	{
		SearchModel.Page = page;

		IsLoading = true;
		await SaveLocalSetting(SearchKey, SearchModel);
		SearchResults = await Mediator.Send(new SearchTemplates.Query(SearchModel));
		IsLoading = false;
	}

	protected void HandleAddClicked()
	{
		FormModel = new();
		DrawerOpen = true;
	}

	protected async Task HandleAddFormSubmitted(FormTemplateModel model)
	{
		IsWorking = true;
		var result = await Mediator.Send(new AddTemplate.Command(model));
		ShowNotification(result);
		if (result.Success)
		{
			DrawerOpen = false;
			NavManager.NavigateTo($"/templates/{result.Payload}");
		}
		IsWorking = false;
	}

	protected void HandleCancelAddClicked()
	{
		DrawerOpen = false;
	}

	protected async Task HandleTrackNowClicked(TemplateListItem item)
	{
		IsWorking = true;
		TrackNowSid = item.Sid;
		var result = await Mediator.Send(new AddWorkout.Command(item.Sid));
		if (result.Success)
		{
			NavManager.NavigateTo($"/workouts/{result.Payload}");
		}
		else
		{
			ShowNotification(result);
		}
		TrackNowSid = string.Empty;
		IsWorking = false;
	}
}
