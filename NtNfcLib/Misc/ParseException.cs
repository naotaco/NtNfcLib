using System;

namespace NdefUtils
{
    public class NdefParseException : Exception
    {

        public NdefParseException()
        {
        }

        public NdefParseException(String message)
            : base(message)
        {
        }
    }
}
