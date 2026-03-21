using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Demo.Utility
{
    public static class ControllerUtility
    {
        public static void AddOperationFeedback(
            this ITempDataDictionary tempData,
            string actionName,
            string? objName = "Object")
        {
            tempData["success"] = $"{objName} {actionName} successfully";
        }

        public static void AddOperationFeedback(
            this Controller controller,
            string actionName,
            string? objName = "Object")
        {
            AddOperationFeedback(controller.TempData, actionName, objName: objName);
        }
    }
}