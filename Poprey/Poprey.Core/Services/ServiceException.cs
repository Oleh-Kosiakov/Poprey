using System;

namespace Poprey.Core.Services
{
    public class ServiceException :  Exception
    {
        public ServiceResolution KnownErrorCode { get; }

        public ServiceException(ServiceResolution errorCode, string errorMessage = "") : base(errorMessage)
        {
            KnownErrorCode = errorCode;
        }
    }
}