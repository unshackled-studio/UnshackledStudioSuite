﻿using System.Text.Json.Serialization;
using Unshackled.Fitness.Core.Interfaces;
using Unshackled.Fitness.My.Client.Models;

namespace Unshackled.Fitness.My.Client.Features.Recipes.Models;

public class RecipeIngredientGroupModel : BaseHouseholdObject, ISortableGroup
{
	public string RecipeSid { get; set; } = string.Empty;
	public int SortOrder { get; set; }
	public string Title { get; set; } = string.Empty;

	[JsonIgnore]
	public bool? IsSelectAll { get; set; } = false;
}
