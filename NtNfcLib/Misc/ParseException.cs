using System;

namespace Naotaco.Nfc
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
