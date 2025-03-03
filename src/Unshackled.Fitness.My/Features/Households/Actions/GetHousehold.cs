﻿using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Unshackled.Fitness.Core.Data;
using Unshackled.Fitness.Core.Enums;
using Unshackled.Fitness.My.Client.Features.Households.Models;
using Unshackled.Fitness.My.Extensions;

namespace Unshackled.Fitness.My.Features.Households.Actions;

public class GetHousehold
{
	public class Query : IRequest<HouseholdModel>
	{
		public long MemberId { get; private set; }
		public long HouseholdId { get; private set; }

		public Query(long memberId, long householdId)
		{
			MemberId = memberId;
			HouseholdId = householdId;
		}
	}

	public class Handler : BaseHandler, IRequestHandler<Query, HouseholdModel>
	{
		public Handler(BaseDbContext db, IMapper mapper) : base(db, mapper) { }

		public async Task<HouseholdModel> Handle(Query request, CancellationToken cancellationToken)
		{
			if (await db.HasHouseholdPermission(request.HouseholdId, request.MemberId, PermissionLevels.Read))
			{
				var household = await mapper.ProjectTo<HouseholdModel>(db.Households
				.AsNoTracking()
				.Where(x => x.Id == request.HouseholdId))
				.SingleOrDefaultAsync(cancellationToken) ?? new();

				household.PermissionLevel = await db.HouseholdMembers
					.Where(x => x.HouseholdId == request.HouseholdId && x.MemberId == request.MemberId)
					.Select(x => x.PermissionLevel)
					.SingleAsync(cancellationToken);

				return household;
			}
			return new();
		}
	}
}
