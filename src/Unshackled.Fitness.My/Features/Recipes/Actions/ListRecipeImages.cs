﻿using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Unshackled.Fitness.Core.Data;
using Unshackled.Fitness.Core.Enums;
using Unshackled.Fitness.My.Client.Models;
using Unshackled.Fitness.My.Extensions;

namespace Unshackled.Fitness.My.Features.Recipes.Actions;

public class ListRecipeImages
{
	public class Query : IRequest<List<ImageModel>>
	{
		public long MemberId { get; private set; }
		public long RecipeId { get; private set; }

		public Query(long memberId, long recipeId)
		{
			MemberId = memberId;
			RecipeId = recipeId;
		}
	}

	public class Handler : BaseHandler, IRequestHandler<Query, List<ImageModel>>
	{
		public Handler(BaseDbContext db, IMapper mapper) : base(db, mapper) { }

		public async Task<List<ImageModel>> Handle(Query request, CancellationToken cancellationToken)
		{
			if (await db.HasRecipePermission(request.RecipeId, request.MemberId, PermissionLevels.Read))
			{
				return await mapper.ProjectTo<ImageModel>(db.RecipeImages
					.AsNoTracking()
					.Where(x => x.RecipeId == request.RecipeId)
					.OrderBy(x => x.SortOrder))
					.ToListAsync(cancellationToken) ?? [];
			}
			return [];
		}
	}
}
