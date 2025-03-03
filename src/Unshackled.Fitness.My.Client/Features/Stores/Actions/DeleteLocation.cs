﻿using MediatR;
using Unshackled.Fitness.Core;
using Unshackled.Fitness.My.Client.Models;

namespace Unshackled.Fitness.My.Client.Features.Stores.Actions;

public class DeleteLocation
{
	public class Command : IRequest<CommandResult>
	{
		public string LocationSid { get; private set; }

		public Command(string locationSid)
		{
			LocationSid = locationSid;
		}
	}

	public class Handler : BaseStoreHandler, IRequestHandler<Command, CommandResult>
	{
		public Handler(HttpClient httpClient) : base(httpClient) { }

		public async Task<CommandResult> Handle(Command request, CancellationToken cancellationToken)
		{
			return await PostToCommandResultAsync($"{baseUrl}delete-location", request.LocationSid)
				?? new CommandResult(false, Globals.UnexpectedError);
		}
	}
}
