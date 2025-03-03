﻿using Microsoft.AspNetCore.Mvc;
using Unshackled.Fitness.My.Client.Features.CookbookRecipes.Models;
using Unshackled.Fitness.My.Features.CookbookRecipes.Actions;

namespace Unshackled.Fitness.My.Features.CookbookRecipes;

[ApiController]
[ApiRoute("cookbook-recipes")]
public class CookbookRecipesController : BaseController
{
	[HttpPost("copy")]
	[ActiveMemberRequired]
	public async Task<IActionResult> Copy([FromBody] FormCopyRecipeModel model)
	{
		return Ok(await Mediator.Send(new CopyRecipe.Command(Member.Id, model)));
	}

	[HttpPost("delete")]
	[ActiveMemberRequired]
	public async Task<IActionResult> Delete([FromBody] string sid)
	{
		return Ok(await Mediator.Send(new DeleteRecipe.Command(Member.Id, Member.ActiveCookbookId, sid)));
	}

	[HttpGet("get/{sid}")]
	[DecodeId]
	public async Task<IActionResult> GetRecipe(long id)
	{
		return Ok(await Mediator.Send(new GetRecipe.Query(Member.Id, Member.ActiveCookbookId, id)));
	}

	[HttpGet("list-member-households")]
	public async Task<IActionResult> ListMemberHouseholds()
	{
		return Ok(await Mediator.Send(new ListMemberHouseholds.Query(Member.Id)));
	}

	[HttpGet("list-recipe-tags")]
	public async Task<IActionResult> ListRecipeTags()
	{
		return Ok(await Mediator.Send(new ListRecipeTags.Query(Member.Id, Member.ActiveCookbookId)));
	}

	[HttpPost("search")]
	public async Task<IActionResult> Search([FromBody] SearchRecipeModel model)
	{
		return Ok(await Mediator.Send(new SearchRecipes.Query(Member.ActiveCookbookId, Member.Id, model)));
	}
}
