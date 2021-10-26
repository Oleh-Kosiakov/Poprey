using System;
using Poprey.Core.Services;

namespace Poprey.Core.Rest.Models
{
    public class ApiException : Exception
    {
        public  string ErrorText { get; private set; }

        public  string CsrfToken { get; private set; }

        public int ErrorCode { get; set; }

        public ApiException(int errorCode, string errorText, string csrfToken)
        {
            ErrorCode = errorCode;
            ErrorText = errorText;
            CsrfToken = csrfToken;
        }
    }
}
