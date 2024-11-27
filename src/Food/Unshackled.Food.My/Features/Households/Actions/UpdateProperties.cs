﻿using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Unshackled.Food.Core;
using Unshackled.Food.Core.Data;
using Unshackled.Food.Core.Data.Entities;
using Unshackled.Food.Core.Enums;
using Unshackled.Food.My.Client.Features.Households.Models;
using Unshackled.Food.My.Extensions;
using Unshackled.Studio.Core.Client.Models;
using Unshackled.Studio.Core.Server.Extensions;

namespace Unshackled.Food.My.Features.Households.Actions;

public class UpdateProperties
{
	public class Command : IRequest<CommandResult<HouseholdModel>>
	{
		public long MemberId { get; private set; }
		public FormHouseholdModel Model { get; private set; }

		public Command(long memberId, FormHouseholdModel model)
		{
			MemberId = memberId;
			Model = model;
		}
	}

	public class Handler : BaseHandler, IRequestHandler<Command, CommandResult<HouseholdModel>>
	{
		public Handler(FoodDbContext db, IMapper mapper) : base(db, mapper) { }

		public async Task<CommandResult<HouseholdModel>> Handle(Command request, CancellationToken cancellationToken)
		{
			long householdId = request.Model.Sid.DecodeLong();

			if (householdId == 0)
				return new CommandResult<HouseholdModel>(false, "Invalid household ID.");

			if (!await db.HasHouseholdPermission(householdId, request.MemberId, PermissionLevels.Admin))
				return new CommandResult<HouseholdModel>(false, FoodGlobals.PermissionError);

			HouseholdEntity? household = await db.Households
				.Where(x => x.Id == householdId)
				.SingleOrDefaultAsync(cancellationToken);

			if (household == null)
				return new CommandResult<HouseholdModel>(false, "Invalid household.");

			// Update household
			household.Title = request.Model.Title.Trim();
			await db.SaveChangesAsync(cancellationToken);

			var g = mapper.Map<HouseholdModel>(household);

			g.PermissionLevel = await db.HouseholdMembers
				.Where(x => x.HouseholdId == householdId && x.MemberId == request.MemberId)
				.Select(x => x.PermissionLevel)
				.SingleAsync(cancellationToken);

			return new CommandResult<HouseholdModel>(true, "Household updated.", g);
		}
	}
}