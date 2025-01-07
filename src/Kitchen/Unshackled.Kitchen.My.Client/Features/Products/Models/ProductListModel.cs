﻿using Unshackled.Kitchen.Core.Models;
using Unshackled.Studio.Core.Client.Models;

namespace Unshackled.Kitchen.My.Client.Features.Products.Models;

public class ProductListModel : BaseHouseholdObject
{
	public string? Brand { get; set; }
	public string Title { get; set; } = string.Empty;
	public string? Description { get; set; }
	public string? Category { get; set; }
	public bool IsPinned { get; set; }
	public List<ImageModel> Images { get; set; } = [];
}
