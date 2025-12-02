using System;
using System.Net.Http;

namespace ZPMLabs.Mailcow.Exceptions;

public sealed class MailcowResponseException : MailcowException
{
    public HttpRequestMessage? Request { get; }
    public HttpResponseMessage? Response { get; }

    public MailcowResponseException(
        string message,
        HttpRequestMessage? request = null,
        HttpResponseMessage? response = null,
        Exception? inner = null)
        : base(message, inner ?? new Exception(message))
    {
        Request = request;
        Response = response;
    }
}
