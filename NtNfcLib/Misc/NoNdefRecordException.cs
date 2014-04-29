using System;

namespace NtNfcLib
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
