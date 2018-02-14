using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace <%= namespace %>
{
    /// <summary>
    /// An envelope that wraps the service resposne.
    /// </summary>
    public class ServiceResponse<E>
    {
        private IDictionary<string, IList<string>> _errors;

        /// <summary>
        /// Represents the ServiceResponse.
        /// </summary>
        public ServiceResponse()
        {
            _errors = new Dictionary<string, IList<string>>();
        }

        /// <summary>
        /// Represents the ServiceResponse.
        /// </summary>
        /// <param name="errors">Error messages.</param>
        public ServiceResponse(Dictionary<string, IList<string>> errors)
        {
            _errors = errors;
        }

        /// <summary>
        /// Gets or Sets if the response was a success.
        /// </summary>
        public ServiceResponse(bool isSuccess, E result)
        {
            this.IsSuccess = isSuccess;
            this.Result = result;

        }

        /// <summary>
        /// Gets or Sets if the response was a success.
        /// </summary>
        public bool IsSuccess { get; set; } = false;

        /// <summary>
        /// Gets or Sets the StatusCode.
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }

        /// <summary>
        /// Error messages if the response was not a success.
        /// </summary>
        public IDictionary<string, IList<string>> Errors
        {
            get
            {
                return _errors;
            }
        }

        /// <summary>
        /// The result from a successful response.
        /// </summary>
        public E Result { get; set; }

        /// <summary>
        /// Add a error message to the collection.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="errorMessage"></param>
        public void AddError(string key, string errorMessage)
        {
            if (Errors == null)
            {
                _errors = new Dictionary<string, IList<string>>();
            }
            if (_errors.ContainsKey(key))
            {
                _errors[key].Add(errorMessage);
            }
            else
            {
                _errors.Add(key, new List<string>() { errorMessage });
            }
        }

        /// <summary>
        /// Add error messages to the collection.
        /// </summary>
        /// <param name="errors"></param>
        public void AddErrors(IDictionary<string, IList<string>> errors)
        {
            errors.ToList().ForEach(x => _errors[x.Key] = x.Value);
        }
    }
}