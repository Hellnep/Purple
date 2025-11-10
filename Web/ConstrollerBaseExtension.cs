using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Purple.Web;

internal static class ControllerBaseExtension
{
    public static IActionResult ValidationProblems(this ControllerBase controller, ICollection<ValidationResult> results)
    {
        foreach (var result in results)
            foreach (var member in result.MemberNames)
                controller.ModelState.AddModelError(member, result.ErrorMessage ?? string.Empty);

        return controller.ValidationProblem();
    }
}