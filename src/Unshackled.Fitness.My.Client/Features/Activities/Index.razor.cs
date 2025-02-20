using MudBlazor;
using Unshackled.Fitness.Core;
using Unshackled.Fitness.Core.Enums;
using Unshackled.Fitness.My.Client.Components;
using Unshackled.Fitness.My.Client.Features.Activities.Actions;
using Unshackled.Fitness.My.Client.Features.Activities.Models;
using Unshackled.Fitness.My.Client.Models;

namespace Unshackled.Fitness.My.Client.Features.Activities;

public class IndexBase : BaseSearchComponent<SearchActivitiesModel, ActivityListModel>
{
	public const string FormId = "formAddActivity";
	protected DateRange DateRangeSearch { get; set; } = new DateRange();
	protected bool DrawerOpen { get; set; }
	protected List<ActivityTypeListModel> ActivityTypes { get; set; } = [];
	protected bool HasActivityTypes => ActivityTypes.Count > 0;
	protected AppSettings AppSettings => State.ActiveMember.Settings;
	protected FormActivityModel FormModel { get; set; } = new();

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		SearchKey = "SearchActivities";

		Breadcrumbs.Add(new BreadcrumbItem("Activities", null, true));

		SearchModel = await GetLocalSetting(SearchKey) ?? new();
		if (SearchModel.DateStart.HasValue && SearchModel.DateEnd.HasValue)
		{
			DateRangeSearch = new DateRange(SearchModel.DateStart.Value.DateTime, SearchModel.DateEnd.Value.DateTime.AddDays(-1));
		}

		ActivityTypes = await Mediator.Send(new ListActivityTypes.Query());

		string? sessionSid = await GetLocalString(Globals.LocalStorageKeys.TrackTrainingSessionSid);
		if (!string.IsNullOrEmpty(sessionSid))
		{
			await RemoveLocalSetting(Globals.LocalStorageKeys.TrackTrainingSessionSid);
			FormModel = await Mediator.Send(new GetSessionForm.Query(sessionSid));
			FormModel.DateEvent = DateTime.Now;
			FormModel.DateEventUtc = FormModel.DateEvent.Value.ToUniversalTime();
			DrawerOpen = true;
		}

		await DoSearch(1);
	}

	protected async override Task DoSearch(int page)
	{
		SearchModel.Page = page;

		IsLoading = true;
		await SaveLocalSetting(SearchKey, SearchModel);
		SearchResults = await Mediator.Send(new SearchActivities.Query(SearchModel));
		IsLoading = false;
	}

	protected async Task HandleAddSubmitted(FormActivityModel model)
	{
		IsWorking = true;
		var result = await Mediator.Send(new AddActivity.Command(model));
		if (result.Success)
		{
			DrawerOpen = false;
			await DoSearch(SearchModel.Page);
		}
		ShowNotification(result);
		IsWorking = false;
	}

	protected void HandleAddActivityClicked()
	{
		FormModel = new();
		FormModel.SetUnits(AppSettings.DefaultUnits == UnitSystems.Metric);
		DrawerOpen = true;
	}

	protected void HandleDateRangeChanged(DateRange dateRange)
	{
		DateRangeSearch = dateRange;
		SearchModel.DateStart = dateRange.Start;
		SearchModel.DateEnd = dateRange.End.HasValue ? dateRange.End.Value.AddDays(1) : dateRange.End;
	}

	protected async override Task HandleResetClicked()
	{
		DateRangeSearch = new DateRange();
		await base.HandleResetClicked();
	}
}