﻿using MediatR;
using Unshackled.Fitness.My.Client.Features.Products.Models;
using Unshackled.Fitness.My.Client.Models;

namespace Unshackled.Fitness.My.Client.Features.Products.Actions;

public class SearchProducts
{
	public class Query : IRequest<SearchResult<ProductListModel>>
	{
		public SearchProductModel Model { get; private set; }

		public Query(SearchProductModel model)
		{
			Model = model;
		}
	}

	public class Handler : BaseProductHandler, IRequestHandler<Query, SearchResult<ProductListModel>>
	{
		public Handler(HttpClient httpClient) : base(httpClient) { }

		public async Task<SearchResult<ProductListModel>> Handle(Query request, CancellationToken cancellationToken)
		{
			return await PostToResultAsync<SearchProductModel, SearchResult<ProductListModel>>($"{baseUrl}search", request.Model) ??
				new SearchResult<ProductListModel>();
		}
	}
}
