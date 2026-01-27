using System.ComponentModel.DataAnnotations;

namespace PurpleBackendService.Core.Utility
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
        /// <exception cref="ArgumentNullException"></exception>
        public static bool TryValidate<T>(T model, out List<ValidationResult> results)
        {
            if (model is null)
            {
                throw new ArgumentNullException($"Passed {typeof(T)} as a null object");
            }

            var context = new ValidationContext(model,
                serviceProvider: null,
                items: null
            );

            results = new List<ValidationResult>();

            return Validator.TryValidateObject(model,
                context,
                results,
                validateAllProperties: true
            );
        }
    }
}
