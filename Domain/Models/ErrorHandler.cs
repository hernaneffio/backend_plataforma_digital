using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models;

public class ErrorHandler : Exception
{
    [IgnoreDataMember]
    public HttpStatusCode Code { get; }

    [DataMember(Name = "message")]
    public override string Message { get; }

    public object ExceptionData { get; }

    public int InternalResponse { get; set; }

    public int Status { get; set; }


    public ErrorHandler(HttpStatusCode code, string message, object data, int internalResponse = 0, int status = 200)
    {
        Code = code;
        Message = message;
        ExceptionData = data;
        InternalResponse = internalResponse;
        Status = status;
    }
}

