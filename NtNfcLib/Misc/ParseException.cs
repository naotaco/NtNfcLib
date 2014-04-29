using System;

namespace NtNfcLib
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
