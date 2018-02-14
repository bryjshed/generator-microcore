using System;
using System.Collections.Generic;
using System.Linq;

namespace <%= namespace %>
{
    /// <summary>
    /// Thrown when call to Microservice fails
    /// </summary>
    public class ServiceException : Exception
    {
        /// <summary>
        /// User defined error dictionary for maintaining errors
        /// </summary>
        public IDictionary<string, IList<string>> Errors { get; private set; }


        /// <summary>
        /// Initializes new ServiceException
        /// </summary>
        public ServiceException()
        {
        }

        /// <summary>
        /// Initializes new ServiceException
        /// </summary>
        /// <param name="message"></param>
        public ServiceException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes new ServiceException
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public ServiceException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes new ServiceException
        /// </summary>
        /// <param name="message"></param>
        /// <param name="errors"></param>
        public ServiceException(string message, IDictionary<string, IList<string>> errors) : this(message)
        {
            this.Errors = errors;
        }

        /// <summary>
        /// Initializes new ServiceException
        /// </summary>
        /// <param name="message"></param>
        /// <param name="key"></param>
        /// <param name="errorMessage"></param>
        public ServiceException(string message, string key, string errorMessage) : this(message)
        {
            AddError(key, errorMessage);
        }

        /// <summary>
        /// Initializes new ServiceException
        /// </summary>
        /// <param name="key"></param>
        /// <param name="errorMessage"></param>
        public ServiceException(string key, string errorMessage) : this()
        {
            AddError(key, errorMessage);
        }

        /// <summary>
        /// Add a error message to the collection
        /// </summary>
        /// <param name="key"></param>
        /// <param name="errorMessage"></param>
        private void AddError(string key, string errorMessage)
        {
            if (Errors == null)
            {
                Errors = new Dictionary<string, IList<string>>();
            }
            if (Errors.ContainsKey(key))
            {
                Errors[key].Add(errorMessage);
            }
            else
            {
                Errors.Add(key, new List<string>() { errorMessage });
            }
        }
    }
}