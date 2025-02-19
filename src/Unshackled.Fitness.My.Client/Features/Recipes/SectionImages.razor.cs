using Microsoft.AspNetCore.Components;
using MudBlazor;
using Unshackled.Fitness.Core.Configuration;
using Unshackled.Fitness.My.Client.Components;
using Unshackled.Fitness.My.Client.Features.Recipes.Actions;
using Unshackled.Fitness.My.Client.Models;

namespace Unshackled.Fitness.My.Client.Features.Recipes;

public class SectionImagesBase : BaseSectionComponent
{
	protected enum Views
	{
		None,
		AddImage
	}

	[Inject] protected IDialogService DialogService { get; set; } = default!;
	[Parameter] public string RecipeSid { get; set; } = string.Empty;
	[Parameter] public List<ImageModel> Images { get; set; } = [];
	[Parameter] public EventCallback<List<ImageModel>> ImagesChanged { get; set; }

	protected bool IsWorking { get; set; } = false;
	protected bool IsSorting { get; set; } = false; 
	protected bool DisableControls => IsWorking;
	protected bool DrawerOpen => DrawerView != Views.None;
	protected Views DrawerView { get; set; } = Views.None;
	protected string DrawerTitle => DrawerView switch
	{
		Views.AddImage => "Add Image",
		_ => string.Empty
	};

	protected void HandleAddClicked()
	{
		DrawerView = Views.AddImage;
	}

	protected void HandleCancelClicked()
	{
		DrawerView = Views.None;
	}

	protected async Task HandleCancelEditClicked()
	{
		IsEditing = await UpdateIsEditingSection(false);
	}

	protected async Task HandleDeleteClicked(ImageModel image)
	{
		bool? confirm = await DialogService.ShowMessageBox(
				"Confirm Delete",
				"Are you sure you want to delete this image?",
				yesText: "Yes", cancelText: "No");

		if (confirm.HasValue && confirm.Value)
		{
			IsWorking = true;
			var result = await Mediator.Send(new DeleteImage.Command(image.Sid));
			ShowNotification(result);
			if (result.Success)
			{
				Images.Remove(image);
				if (!string.IsNullOrEmpty(result.Payload))
				{
					var newFeatured = Images.Where(x => x.Sid == result.Payload).FirstOrDefault();
					if (newFeatured != null)
						newFeatured.IsFeatured = true;
				}
				await ImagesChanged.InvokeAsync(Images);
			}

			IsWorking = false;
		}
	}

	protected async Task HandleImageAdded(ImageModel? image)
	{
		DrawerView = Views.None;
		if (image != null)
		{
			Images.Add(image);
			await ImagesChanged.InvokeAsync(Images);
		}
	}

	protected async Task HandleSetFeaturedClick(ImageModel image)
	{
		if (image.IsFeatured)
			return;

		IsWorking = true;
		var result = await Mediator.Send(new SetFeaturedImage.Command(image.Sid));
		ShowNotification(result);
		if (result.Success)
		{
			var oldFeatured = Images.Where(x => x.IsFeatured == true).FirstOrDefault();
			if (oldFeatured != null)
				oldFeatured.IsFeatured = false;

			image.IsFeatured = true;

			await ImagesChanged.InvokeAsync(Images);
		}

		IsWorking = false;
	}
}