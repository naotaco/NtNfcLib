using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace NtNfcLib
{
    public class NdefRecord
    {

        public byte ndefHeader
        {
            get;
            set;
        }

        public int typeLength
        {
            get;
            set;
        }

        public int payloadLength
        {
            get;
            set;
        }

        public int typeNameFormat
        {
            get;
            set;
        }

        public int idLength
        {
            get;
            set;
        }

        public String type
        {
            get;
            set;
        }

        public String id
        {
            get;
            set;
        }

        public String payload
        {
            get;
            set;
        }

        public String SSID
        {
            get;
            set;
        }

        public String Password
        {
            get;
            set;
        }

        public bool IsIdExist
        {
            get;
            set;
        }

        public List<String> SonyPayload
        {
            get;
            set;
        }

        public List<byte> RawPayload
        {
            get;
            set;
        }

        public NdefRecord()
        {
            ndefHeader = (byte)0;
            typeLength = 0;
            payloadLength = 0;
            idLength = 0;
            typeNameFormat = 0;
            type = "";
            id = "";
            payload = "";
            SSID = "";
            Password = "";
            IsIdExist = false;
            SonyPayload = new List<string>();
            RawPayload = new List<byte>();
        }

        public void dump()
        {
            Debug.WriteLine("NDEF header: " + Convert.ToString(ndefHeader, 2));
            Debug.WriteLine("Type length: " + typeLength);
            Debug.WriteLine("payload length: " + payloadLength);
            Debug.WriteLine("id length: " + idLength);
            Debug.WriteLine("Type name format: " + typeNameFormat);
            Debug.WriteLine("type: " + type);
            Debug.WriteLine("id: " + id);
            Debug.WriteLine("payload: " + payload);
            Debug.WriteLine("SSID: " + SSID);
            Debug.WriteLine("Password: " + Password);
            foreach (String s in SonyPayload)
            {
                Debug.WriteLine("SonyRecord: " + s);
            }
        }


    }
}
