﻿using MediatR;
using Unshackled.Fitness.Core;
using Unshackled.Fitness.My.Client.Features.Products.Models;
using Unshackled.Fitness.My.Client.Models;

namespace Unshackled.Fitness.My.Client.Features.Products.Actions;

public class UpdateProduct
{
	public class Command : IRequest<CommandResult<ProductModel>>
	{
		public FormProductModel Model { get; private set; }

		public Command(FormProductModel model)
		{
			Model = model;
		}
	}

	public class Handler : BaseProductHandler, IRequestHandler<Command, CommandResult<ProductModel>>
	{
		public Handler(HttpClient httpClient) : base(httpClient) { }

		public async Task<CommandResult<ProductModel>> Handle(Command request, CancellationToken cancellationToken)
		{
			return await PostToCommandResultAsync<FormProductModel, ProductModel>($"{baseUrl}update", request.Model)
				?? new CommandResult<ProductModel>(false, Globals.UnexpectedError);
		}
	}
}