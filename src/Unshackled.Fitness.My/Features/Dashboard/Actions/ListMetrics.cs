﻿using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Unshackled.Fitness.Core.Data;
using Unshackled.Fitness.My.Client.Features.Dashboard.Models;
using Unshackled.Fitness.My.Extensions;

namespace Unshackled.Fitness.My.Features.Dashboard.Actions;

public class ListMetrics
{
	public class Query : IRequest<MetricGridModel>
	{
		public long MemberId { get; private set; }
		public DateTimeOffset DisplayDate { get; private set; }

		public Query(long memberId, DateTimeOffset displayDate)
		{
			MemberId = memberId;
			DisplayDate = displayDate;
		}
	}

	public class Handler : BaseHandler, IRequestHandler<Query, MetricGridModel>
	{
		public Handler(BaseDbContext db, IMapper mapper) : base(db, mapper) { }

		public async Task<MetricGridModel> Handle(Query request, CancellationToken cancellationToken)
		{
			var model = new MetricGridModel
			{
				Groups = await db.MetricDefinitionGroups
					.Where(x => x.MemberId == request.MemberId)
					.OrderBy(x => x.SortOrder)
					.Select(x => new MetricDefinitionGroupModel
					{
						DateCreatedUtc = x.DateCreatedUtc,
						DateLastModifiedUtc = x.DateLastModifiedUtc,
						MemberSid = x.MemberId.Encode(),
						Sid = x.Id.Encode(),
						SortOrder = x.SortOrder,
						Title = x.Title
					})
					.ToListAsync(cancellationToken),

				Metrics = await db.MetricDefinitions
					.AsNoTracking()
					.Where(x => x.MemberId == request.MemberId && x.IsArchived == false && x.IsOnDashboard == true)
					.OrderBy(x => x.SortOrder)
					.Select(x => new MetricModel
					{
						DateCreatedUtc = x.DateCreatedUtc,
						DateLastModifiedUtc = x.DateLastModifiedUtc,
						ListGroupSid = x.ListGroupId.Encode(),
						HighlightColor = x.HighlightColor,
						IsArchived = x.IsArchived,
						IsOnDashboard = x.IsOnDashboard,
						MaxValue = x.MaxValue,
						MemberSid = x.MemberId.Encode(),
						MetricType = x.MetricType,
						Sid = x.Id.Encode(),
						SortOrder = x.SortOrder,
						SubTitle = x.SubTitle,
						Title = x.Title
					})
					.ToListAsync(cancellationToken)
			};

			var recordedMetrics = await db.Metrics
				.AsNoTracking()
				.Include(x => x.MetricDefinition)
				.Where(x => x.MemberId == request.MemberId && x.MetricDefinition.IsOnDashboard == true && x.DateRecorded == request.DisplayDate.Date)
				.OrderBy(x => x.MetricDefinition.SortOrder)
				.Select(x => new MetricModel
				{
					DateCreatedUtc = x.MetricDefinition.DateCreatedUtc,
					DateLastModifiedUtc = x.MetricDefinition.DateLastModifiedUtc,
					DateRecorded = x.DateRecorded.Date,
					ListGroupSid = x.MetricDefinition.ListGroupId.Encode(),
					HighlightColor = x.MetricDefinition.HighlightColor,
					IsArchived = x.MetricDefinition.IsArchived,
					IsOnDashboard = x.MetricDefinition.IsOnDashboard,
					MaxValue = x.MetricDefinition.MaxValue,
					MemberSid = x.MemberId.Encode(),
					MetricType = x.MetricDefinition.MetricType,
					RecordedValue = x.RecordedValue,
					Sid = x.MetricDefinition.Id.Encode(),
					SortOrder = x.MetricDefinition.SortOrder,
					SubTitle = x.MetricDefinition.SubTitle,
					Title = x.MetricDefinition.Title
				})
				.ToListAsync(cancellationToken);

			foreach (var metric in recordedMetrics)
			{
				var existing = model.Metrics.Where(x => x.Sid == metric.Sid).SingleOrDefault();
				if (existing != null)
				{
					existing.RecordedValue = metric.RecordedValue;
				}
				else
				{
					model.Metrics.Add(metric);
				}
			}

			model.Metrics = model.Metrics.OrderBy(x => x.SortOrder).ToList();

			return model;
		}
	}
}
