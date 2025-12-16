using System.ComponentModel.DataAnnotations;

namespace PurpleBackendService.Domain.Utility
{
    public static class Validate
    {
        /// <summary>
        /// Checking the properties of the entered data for validation
        /// </summary>
        /// <typeparam name="T">Type of data model</typeparam>
        /// <param name="model">Entered data model</param>
        /// <param name="results">Generated list of validation errors</param>
        /// <returns>Returns a list of data verification results</returns>
        public static bool TryValidate<T>(T model, out List<ValidationResult> results)
        {
            var context = new ValidationContext(model,
                serviceProvider: null,
                items: null);

            results = new List<ValidationResult>();

            return Validator.TryValidateObject(model,
                context,
                results,
                validateAllProperties: true
            );
        }
    }
}