﻿using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Unshackled.Fitness.Core.Data;
using Unshackled.Fitness.My.Client.Features.Workouts.Models;
using Unshackled.Fitness.My.Client.Models;
using Unshackled.Fitness.My.Extensions;

namespace Unshackled.Fitness.My.Features.Workouts.Actions;

public class UpdateNotes
{
	public class Command : IRequest<CommandResult>
	{
		public long MemberId { get; private set; }
		public FormNotesModel Model { get; private set; }

		public Command(long memberId, FormNotesModel model)
		{
			MemberId = memberId;
			Model = model;
		}
	}

	public class Handler : BaseHandler, IRequestHandler<Command, CommandResult>
	{
		public Handler(BaseDbContext db, IMapper mapper) : base(db, mapper) { }

		public async Task<CommandResult> Handle(Command request, CancellationToken cancellationToken)
		{
			long workoutId = request.Model.Sid.DecodeLong();

			var workout = await db.Workouts
				.Where(x => x.Id == workoutId && x.MemberId == request.MemberId)
				.SingleOrDefaultAsync(cancellationToken);

			if (workout == null)
				return new CommandResult(false, "Invalid workout.");

			workout.Notes = request.Model.Notes;
			// Mark modified to avoid missing string case changes.
			db.Entry(workout).Property(x => x.Notes).IsModified = true;

			await db.SaveChangesAsync(cancellationToken);

			return new CommandResult(true, "Notes updated.");
		}
	}
}