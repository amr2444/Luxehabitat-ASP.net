using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace RealEstate.Web.Extensions;

public static class TempDataExtensions
{
    public static void PutSuccess(this ITempDataDictionary tempData, string message)
    {
        tempData["SuccessMessage"] = message;
    }

    public static void PutError(this ITempDataDictionary tempData, string message)
    {
        tempData["ErrorMessage"] = message;
    }
}
