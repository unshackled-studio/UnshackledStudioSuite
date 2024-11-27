﻿using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Unshackled.Food.Core.Data;
using Unshackled.Food.Core.Enums;
using Unshackled.Food.My.Client.Features.Cookbooks.Models;
using Unshackled.Food.My.Extensions;

namespace Unshackled.Food.My.Features.Cookbooks.Actions;

public class GetCookbook
{
	public class Query : IRequest<CookbookModel>
	{
		public long MemberId { get; private set; }
		public long CookbookId { get; private set; }

		public Query(long memberId, long cookbookId)
		{
			MemberId = memberId;
			CookbookId = cookbookId;
		}
	}

	public class Handler : BaseHandler, IRequestHandler<Query, CookbookModel>
	{
		public Handler(FoodDbContext db, IMapper mapper) : base(db, mapper) { }

		public async Task<CookbookModel> Handle(Query request, CancellationToken cancellationToken)
		{
			if (await db.HasCookbookPermission(request.CookbookId, request.MemberId, PermissionLevels.Read))
			{
				var cookbook = await mapper.ProjectTo<CookbookModel>(db.Cookbooks
				.AsNoTracking()
				.Where(x => x.Id == request.CookbookId))
				.SingleOrDefaultAsync(cancellationToken) ?? new();

				cookbook.PermissionLevel = await db.CookbookMembers
					.Where(x => x.CookbookId == request.CookbookId && x.MemberId == request.MemberId)
					.Select(x => x.PermissionLevel)
					.SingleAsync(cancellationToken);

				return cookbook;
			}
			return new();
		}
	}
}
