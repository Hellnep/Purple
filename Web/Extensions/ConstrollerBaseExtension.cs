using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Purple.Web;

internal static class ControllerBaseExtension
{
    /// <summary>
    /// Accept the validation results and sends the result to the sender
    /// </summary>
    /// <param name="controller">The controller that will send the result</param>
    /// <param name="results">Validation results</param>
    public static void ValidationProblems(this ControllerBase controller, ICollection<ValidationResult> results)
    {
        foreach (var result in results)
            foreach (var member in result.MemberNames)
                controller.ModelState.AddModelError(member, result.ErrorMessage ?? string.Empty);

        controller.ValidationProblem();
    }
}