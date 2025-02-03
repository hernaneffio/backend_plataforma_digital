using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models;

public class MessageResult<T>
{
    public int Code { get; set; }

    public string Message { get; set; }

    public T Data { get; set; }

    public int Status { get; set; }

    public MessageResult(int code, string message, T data, int status)
    {
        Code = code;
        Message = message;
        Data = data;
        Status = status;
    }



    public static MessageResult<T> Of(string message, T data, int? status = 200, int? code = 1) => new(code.Value, message, data, status.Value);

    public static MessageResult<T> Success(T data, string message, int? statusCode = 200)
    {
        return new MessageResult<T>(1, message, data, statusCode.Value);
    }

    public static MessageResult<T> Failure(string message, int statusCode = 400)
    {
        return new MessageResult<T>(0, message, default(T), statusCode);
    }

}


