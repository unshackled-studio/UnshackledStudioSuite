﻿using Microsoft.AspNetCore.Mvc;
using Unshackled.Fitness.My.Client.Features.Stores.Models;
using Unshackled.Fitness.My.Features.Stores.Actions;

namespace Unshackled.Fitness.My.Features.Stores;

[ApiController]
[ApiRoute("stores")]
public class StoresController : BaseController
{
	[HttpPost("add")]
	[ActiveMemberRequired]
	public async Task<IActionResult> Add([FromBody] FormStoreModel model)
	{
		return Ok(await Mediator.Send(new AddStore.Command(Member.Id, Member.ActiveHouseholdId, model)));
	}

	[HttpPost("add-location")]
	[ActiveMemberRequired]
	public async Task<IActionResult> AddLocation([FromBody] FormStoreLocationModel model)
	{
		return Ok(await Mediator.Send(new AddLocation.Command(Member.Id, Member.ActiveHouseholdId, model)));
	}

	[HttpPost("bulk-add-locations")]
	[ActiveMemberRequired]
	public async Task<IActionResult> BulkAddLocations([FromBody] FormBulkAddLocationModel model)
	{
		return Ok(await Mediator.Send(new BulkAddLocations.Command(Member.Id, Member.ActiveHouseholdId, model)));
	}

	[HttpPost("change-product-location")]
	[ActiveMemberRequired]
	public async Task<IActionResult> ChangeProductLocation([FromBody] ChangeLocationModel model)
	{
		return Ok(await Mediator.Send(new ChangeProductLocation.Command(Member.Id, model)));
	}

	[HttpPost("delete")]
	[ActiveMemberRequired]
	public async Task<IActionResult> Delete([FromBody] string sid)
	{
		return Ok(await Mediator.Send(new DeleteStore.Command(Member.Id, sid)));
	}

	[HttpPost("delete/{sid}/product")]
	[DecodeId]
	[ActiveMemberRequired]
	public async Task<IActionResult> DeleteProductLocation(long id, [FromBody] string sid)
	{
		return Ok(await Mediator.Send(new DeleteProductLocation.Command(Member.Id, id, sid)));
	}

	[HttpPost("delete-location")]
	[ActiveMemberRequired]
	public async Task<IActionResult> DeleteLocation([FromBody] string sid)
	{
		return Ok(await Mediator.Send(new DeleteLocation.Command(Member.Id, sid)));
	}

	[HttpGet("get/{sid}")]
	[DecodeId]
	public async Task<IActionResult> GetStore(long id)
	{
		return Ok(await Mediator.Send(new GetStore.Query(Member.Id, id)));
	}

	[HttpGet("get-location/{sid}")]
	[DecodeId]
	public async Task<IActionResult> GetStoreLocation(long id)
	{
		return Ok(await Mediator.Send(new GetStoreLocation.Query(Member.Id, id)));
	}

	[HttpGet("list/{sid}/aisles")]
	[DecodeId]
	public async Task<IActionResult> ListLocations(long id)
	{
		return Ok(await Mediator.Send(new ListStoreLocations.Query(Member.Id, id)));
	}

	[HttpPost("search")]
	public async Task<IActionResult> Search([FromBody] SearchStoreModel model)
	{
		return Ok(await Mediator.Send(new SearchStores.Query(Member.ActiveHouseholdId, Member.Id, model)));
	}

	[HttpPost("update")]
	[ActiveMemberRequired]
	public async Task<IActionResult> Update([FromBody] FormStoreModel model)
	{
		return Ok(await Mediator.Send(new UpdateStoreProperties.Command(Member.Id, model)));
	}

	[HttpPost("update/{sid}/products")]
	[DecodeId]
	[ActiveMemberRequired]
	public async Task<IActionResult> UpdateProductLocations(long id, [FromBody] List<FormProductLocationModel> products)
	{
		return Ok(await Mediator.Send(new UpdateProductLocations.Command(Member.Id, id, products)));
	}

	[HttpPost("update-location")]
	[ActiveMemberRequired]
	public async Task<IActionResult> UpdateLocation([FromBody] FormStoreLocationModel model)
	{
		return Ok(await Mediator.Send(new UpdateLocation.Command(Member.Id, model)));
	}

	[HttpPost("update-location-sorts")]
	[ActiveMemberRequired]
	public async Task<IActionResult> UpdateLocationSorts([FromBody] UpdateSortsModel model)
	{
		return Ok(await Mediator.Send(new UpdateLocationSorts.Command(Member.Id, model)));
	}
}
