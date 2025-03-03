﻿using Unshackled.Fitness.Core.Enums;

namespace Unshackled.Fitness.My.Client.Models;

public class ParsedIngredientResult
{
	public string OriginalText { get; set; } = string.Empty;
	public decimal Amount { get; set; }
	public string AmountText { get; set; } = string.Empty;
	public MeasurementUnits AmountUnit { get; set; } = MeasurementUnits.Item;
	public string AmountLabel { get; set; } = string.Empty;
	public string Title { get; set; } = string.Empty;
	public string PrepNote { get; set; } = string.Empty;
}
