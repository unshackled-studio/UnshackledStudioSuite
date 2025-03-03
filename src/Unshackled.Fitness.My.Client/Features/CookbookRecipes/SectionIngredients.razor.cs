using Microsoft.AspNetCore.Components;
using Unshackled.Fitness.My.Client.Components;
using Unshackled.Fitness.My.Client.Features.CookbookRecipes.Models;

namespace Unshackled.Fitness.My.Client.Features.CookbookRecipes;

public class SectionIngredientsBase : BaseSectionComponent
{
	[Parameter] public List<RecipeIngredientGroupModel> Groups { get; set; } = [];
	[Parameter] public List<RecipeIngredientModel> Ingredients { get; set; } = [];
	[Parameter] public decimal Scale { get; set; }
	[Parameter] public EventCallback<decimal> ScaleChanged { get; set; }

	protected async Task HandleScaleChanged(decimal value)
	{
		if (value != Scale)
		{
			Scale = value;
			await ScaleChanged.InvokeAsync(value);
		}
	}

	protected string GetScaleFraction()
	{
		return Scale switch
		{
			0.25M => "1/4x",
			0.5M => "1/2x",
			0.75M => "3/4x",
			1M => "1x",
			2M => "2x",
			3M => "3x",
			4M => "4x",
			_ => Scale.ToString("0.##")
		};
	}
}