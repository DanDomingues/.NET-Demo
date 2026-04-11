using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Northstar.Utility
{
    public static class ControllerUtility
    {
        public static void AddOperationFeedback(
            this ITempDataDictionary tempData,
            string actionName,
            string? objName = "Object",
            string feedbackKey = "success")
        {
            tempData[feedbackKey] = $"{objName} {actionName}";
        }

        public static void AddSuccessFeedback(
            this ITempDataDictionary tempData,
            string actionName,
            string? objName = "Object")
        {
            AddOperationFeedback(tempData, $"{actionName} successfuly", objName: objName, feedbackKey: "success");
        }

        public static void AddSuccessFeedback(
            this Controller controller,
            string actionName,
            string? objName = "Object")
        {
            AddSuccessFeedback(controller.TempData, actionName, objName: objName);
        }

        public static void AddErrorFeedback(
            this Controller controller,
            string actionName,
            string? objName = "Object")
        {
            AddOperationFeedback(controller.TempData, actionName, objName: objName, feedbackKey: "error");
        }

        public static IActionResult RedirectToLogin(this Controller controller)
        {
            return controller.RedirectToAction("Login", "Account", new { area = "Identity" });
        }
    }
}