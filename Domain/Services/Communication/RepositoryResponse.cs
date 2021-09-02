using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemindONServer.Domain.Services.Communication
{
    public class RepositoryResponse<T>
    {
        public string Message { get; private set; }
        public RepositoryResponseType RepositoryResponseType { get; private set; }
        public T Resource { get; private set; }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="resource">repource from repository</param>
        /// <returns>Response.</returns>
        public RepositoryResponse(T resource)
        {
            Resource = resource;
            Message = string.Empty;
            RepositoryResponseType = RepositoryResponseType.Success;
        }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="repositoryResponseType"> response type</param>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public RepositoryResponse(RepositoryResponseType repositoryResponseType, string message)
        {
            Resource = default;
            Message = message;
            RepositoryResponseType = repositoryResponseType;
        }

    }
}
