using System;

namespace ZPMLabs.Mailcow.Exceptions;

public class MailcowException : Exception
{
    public MailcowException(string message)
        : base(message)
    {
    }

    public MailcowException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
