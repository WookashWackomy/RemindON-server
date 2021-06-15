using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemindONServer.Domain.Services.Communication
{
    public abstract class BaseResponse
    {
        public string Message { get; protected set; }
        public RepositoryResponse RepositoryResponse { get; protected set; }

        public BaseResponse(RepositoryResponse repositoryResponse, string message)
        {
            Message = message;
            RepositoryResponse = repositoryResponse;
        }
    }
}
