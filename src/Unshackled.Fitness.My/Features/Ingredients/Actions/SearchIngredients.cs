﻿using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Unshackled.Fitness.Core.Data;
using Unshackled.Fitness.Core.Enums;
using Unshackled.Fitness.My.Client.Features.Ingredients.Models;
using Unshackled.Fitness.My.Client.Models;
using Unshackled.Fitness.My.Extensions;

namespace Unshackled.Fitness.My.Features.Ingredients.Actions;

public class SearchIngredients
{
	public class Query : IRequest<SearchResult<IngredientListModel>>
	{
		public long MemberId { get; private set; }
		public long HouseholdId { get; private set; }
		public SearchIngredientModel Model { get; private set; }

		public Query(long memberId, long householdId, SearchIngredientModel model)
		{
			HouseholdId = householdId;
			MemberId = memberId;
			Model = model;
		}
	}

	public class Handler : BaseHandler, IRequestHandler<Query, SearchResult<IngredientListModel>>
	{
		public Handler(BaseDbContext db, IMapper map) : base(db, map) { }

		public async Task<SearchResult<IngredientListModel>> Handle(Query request, CancellationToken cancellationToken)
		{
			if (!await db.HasHouseholdPermission(request.HouseholdId, request.MemberId, PermissionLevels.Read))
				return new();

			var result = new SearchResult<IngredientListModel>(request.Model.PageSize);

			var query = db.RecipeIngredients
				.AsNoTracking()
				.Where(x => x.HouseholdId == request.HouseholdId);

			if (!string.IsNullOrEmpty(request.Model.Title))
				query = query.Where(x => x.Title != null && EF.Functions.Like(x.Title, $"%{request.Model.Title}%"));

			var groupedQuery = query
				.GroupBy(x => new { x.Key, x.Title })
				.Select(x => new IngredientListModel
				{
					IngredientKey = x.Key.Key,
					Title = x.Key.Title,
					RecipeCount = x.Count(),
					SubstitutionsCount = db.ProductSubstitutions.Where(y => y.HouseholdId == request.HouseholdId && y.IngredientKey == x.Key.Key).Count()
				});

			result.Total = await groupedQuery.CountAsync(cancellationToken);

			result.Data = await groupedQuery
				.OrderBy(x => x.Title)
				.Skip(request.Model.Skip)
				.Take(request.Model.PageSize)
				.ToListAsync(cancellationToken);

			return result;
		}
	}
}
