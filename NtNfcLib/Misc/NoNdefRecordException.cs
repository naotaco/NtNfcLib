using System;

namespace Naotaco.Nfc
{
    public class NoNdefRecordException : Exception
    {

        public NoNdefRecordException()
        {
        }

        public NoNdefRecordException(String message)
            : base(message)
        {
        }


    }
}
