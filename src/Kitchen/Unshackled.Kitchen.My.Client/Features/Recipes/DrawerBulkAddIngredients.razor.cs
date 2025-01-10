using Unshackled.Kitchen.Core.Utils;
using Unshackled.Kitchen.My.Client.Features.Recipes.Models;
using Unshackled.Studio.Core.Client.Components;
using Unshackled.Studio.Core.Client.Extensions;

namespace Unshackled.Kitchen.My.Client.Features.Recipes;

public class DrawerBulkAddIngredientsBase : BaseFormComponent<FormBulkAddIngredientModel, FormBulkAddIngredientModel.Validator>
{
	protected List<FormIngredientModel> PreviewIngredients { get; set; } = [];

	protected void HandlePreview(string bulkText)
	{
		PreviewIngredients.Clear();
		if (!string.IsNullOrEmpty(bulkText))
		{
			var reader = new StringReader(bulkText);
			string? line;
			while (null != (line = reader.ReadLine()))
			{
				if (!string.IsNullOrEmpty(line.Trim()))
				{
					var result = IngredientUtils.Parse(line);
					PreviewIngredients.Add(new()
					{
						Amount = result.Amount,
						AmountUnit = result.AmountUnit,
						AmountLabel = result.AmountLabel,
						AmountText = result.AmountText,
						Key = result.Title.NormalizeKey(),
						PrepNote = result.PrepNote,
						SortOrder = PreviewIngredients.Count,
						Title = result.Title
					});
				}
			}
		}
	}
}