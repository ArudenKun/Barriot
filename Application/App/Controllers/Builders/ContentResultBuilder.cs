using Microsoft.AspNetCore.Mvc;

namespace Barriot.Application.Controllers.Builders
{
    /// <summary>
    ///     Represents a class that builds a new <see cref="ContentResult"/>.
    /// </summary>
    public class ContentResultBuilder
    {
        private int _code;
        private string _payload;
        private const string _contentType = "application/json";

        /// <summary>
        ///     Creates a new instance of <see cref="ContentResultBuilder"/> that will build a new <see cref="ContentResult"/>.
        /// </summary>
        public ContentResultBuilder(int statusCode)
        {
            _payload = string.Empty;
            _code = statusCode; // unauthorized
        }

        /// <summary>
        ///     Sets the status code.
        /// </summary>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        public ContentResultBuilder WithStatusCode(int statusCode)
        {
            _code = statusCode;
            return this;
        }

        /// <summary>
        ///     Adds a payload to send.
        /// </summary>
        /// <param name="payload"></param>
        /// <returns></returns>
        public ContentResultBuilder WithPayload(string payload)
        {
            _payload = payload;
            return this;
        }

        /// <summary>
        ///     Builds a new <see cref="ContentResult"/> from the provided values from <see cref="WithPayload(string)"/> and <see cref="WithStatusCode(int)"/>.
        /// </summary>
        /// <returns>The <see cref="ContentResult"/> that should be returned as <see cref="IActionResult"/>.</returns>
        public ContentResult Build()
        {
            return new ContentResult()
            {
                Content = _payload,
                StatusCode = _code,
                ContentType = _contentType
            };
        }
    }
}
