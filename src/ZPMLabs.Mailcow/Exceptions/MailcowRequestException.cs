using System;
using System.Net.Http;

namespace ZPMLabs.Mailcow.Exceptions;

public sealed class MailcowRequestException : MailcowException
{
    public HttpRequestMessage? Request { get; }

    public MailcowRequestException(string message, HttpRequestMessage? request = null, Exception? inner = null)
        : base(message, inner ?? new Exception(message))
    {
        Request = request;
    }
}
