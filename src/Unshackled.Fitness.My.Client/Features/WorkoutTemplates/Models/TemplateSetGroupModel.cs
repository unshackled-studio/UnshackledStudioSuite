﻿using Unshackled.Fitness.Core.Interfaces;
using Unshackled.Fitness.My.Client.Models;

namespace Unshackled.Fitness.My.Client.Features.WorkoutTemplates.Models;

public class TemplateSetGroupModel : BaseObject, ISortableGroup
{
	public string TemplateSid { get; set; } = string.Empty;
	public int SortOrder { get; set; }
	public string Title { get; set; } = string.Empty;
}
