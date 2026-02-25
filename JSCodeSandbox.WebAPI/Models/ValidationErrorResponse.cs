
namespace JSCodeSandbox.WebAPI.Models
{
    /// <summary>
    /// Represents a validation error response returned when a request fails validation.
    /// </summary>
    public class ValidationErrorResponse
    {
        /// <summary>
        /// The category of the validation error.
        /// Possible values are <c>InvalidRequest</c> (general validation failure such as missing or empty fields,
        /// or a non-existent environment) and <c>CodeSyntaxError</c> (the submitted code has a syntax error
        /// detected before execution).
        /// </summary>
        /// <example>InvalidRequest</example>
        public string ErrorType { get; set; } = string.Empty;

        /// <summary>
        /// A human-readable message describing the validation error.
        /// </summary>
        /// <example>Environment name cannot be empty.</example>
        public string Error { get; set; } = string.Empty;
    }
}
