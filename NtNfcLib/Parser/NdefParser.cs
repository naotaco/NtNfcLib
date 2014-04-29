using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Networking.Proximity;

namespace NtNfcLib
{
    public class NdefParser
    {

        private Byte[] raw;
        private List<NdefRecord> records;

        public NdefParser(Byte[] input)
        {
            raw = input;
            records = new List<NdefRecord>();

        }

        public NdefParser(ProximityMessage message)
        {
            var rawMsg = message.Data.ToArray();
            raw = rawMsg;
            records = new List<NdefRecord>();

        }

        public List<NdefRecord> Parse()
        {
            try
            {
                this._parse();
            }
            catch
            {
                throw new NdefParseException("Failed to parse");
            }

            return records;
        }

        private void _parse()
        {
            int recordPointer = 0;

            while (recordPointer < raw.Length)
            {
                bool isLastMessage = false;

                var record = new NdefRecord();
                record.ndefHeader = raw[recordPointer];
                recordPointer++;
                Debug.WriteLine("NDEF header: " + Convert.ToString((int)record.ndefHeader, 2).PadLeft(8));

                record.typeLength = raw[recordPointer];
                recordPointer++;

                // to check whether short record or not
                if ((0x10 & record.ndefHeader) == 0x10)
                {
                    // short length
                    record.payloadLength = raw[recordPointer];
                    recordPointer += 1;
                }
                else
                {
                    // not sony nfc format
                    record.payloadLength = raw[recordPointer + 3] << 24 | raw[recordPointer + 2] << 16 | raw[recordPointer + 1] << 8 | raw[recordPointer];
                    recordPointer += 4;
                }

                // parse TNF
                record.typeNameFormat = (record.ndefHeader & 0x07);

                // to check last message
                if ((0x40 & record.ndefHeader) == 0x40)
                {
                    isLastMessage = true;
                }

                // to check id length (0 or 1)
                if ((0x08 & record.ndefHeader) == 0x08)
                {
                    record.IsIdExist = true;
                }
                else
                {
                    record.IsIdExist = false;
                }


                // get id length
                if (record.IsIdExist)
                {
                    record.idLength = raw[recordPointer];
                    recordPointer++;
                }

                // get type
                record.type = Encoding.UTF8.GetString(raw, recordPointer, record.typeLength);
                recordPointer += record.typeLength;

                StringBuilder sb = new StringBuilder();

                // get id (if exist)
                if (record.IsIdExist)
                {
                    record.id = Encoding.UTF8.GetString(raw, recordPointer, record.idLength);
                    recordPointer += record.idLength;
                }

                // get payload
                Byte[] payload = new Byte[record.payloadLength];
                Array.Copy(raw, recordPointer, payload, 0, record.payloadLength);
                recordPointer += record.payloadLength;

                record.RawPayload = new List<byte>(payload);

                // store payload as text
                record.payload = ForceConvertToString(payload);


                // try to parse the payload as sony's NDEF payload
                try
                {
                    var parsedPayload = this.ParseSonyNdefPayload(payload);
                    record.SSID = parsedPayload.SSID;
                    record.Password = parsedPayload.Password;
                    record.SonyPayload = parsedPayload.SonyPayload;
                }
                catch
                {
                    // if failed, store as just normal NDEF record.
                }

                records.Add(record);
                record.dump();

                if (isLastMessage)
                {
                    Debug.WriteLine("Found the last record.");
                    break;
                }

            }
        }

        private NdefRecord ParseSonyNdefPayload(byte[] payload)
        {
            var ret = new NdefRecord();
            StringBuilder sb = new StringBuilder();
            
            int pointer = 0;
            int contentCount = 0;

            while (pointer < payload.Length)
            {

                Debug.WriteLine("-----Record[" + contentCount++ + "]-----");

                // find id?
                int id = payload[pointer] << 8 | payload[pointer + 1];
                pointer += 2;
                Debug.WriteLine("id: " + id.ToString("x4"));

                // find size?
                int size = payload[pointer] << 8 | payload[pointer + 1];
                pointer += 2;
                Debug.WriteLine("size: " + size.ToString("x4"));

                // get value
                String valueText = Encoding.UTF8.GetString(payload, pointer, size);

                if (valueText.Length < 2)
                {
                    string s = "0x" + payload[pointer].ToString("x");
                    Debug.WriteLine("value: " + s);
                    ret.SonyPayload.Add(s);
                }
                else
                {
                    Debug.WriteLine("value: " + valueText);
                    ret.SonyPayload.Add(valueText);
                }

                if (id == 0x1000)
                {
                    ret.SSID = valueText;
                }
                else if (id == 0x1001)
                {
                    ret.Password = valueText;
                }

                pointer += size;

                Debug.WriteLine("-----Record End----");
            }
            return ret;
        }

        private int CountValidSonyNdefRecord(List<NdefRecord> list)
        {
            int ret = 0;

            foreach (NdefRecord record in list){
                if (record.SSID.Length > 0 && record.Password.Length > 0)
                {
                    ret++;
                }
            }

            return ret;
        }

        private String ForceConvertToString(Byte[] data)
        {
            StringBuilder sb = new StringBuilder();

            bool isLastByteAscii = false;

            foreach (Byte b in data)
            {
                // if ascii
                if ((int)b >= 0x20 && (int)b <= 0x7e)
                {
                    sb.Append((char)b);
                    isLastByteAscii = true;
                }
                else
                {
                    if (isLastByteAscii)
                    {
                        // if not ascii, add newline
                        sb.Append(System.Environment.NewLine);
                    }

                    isLastByteAscii = false;
                }
                
            }


            return sb.ToString();
        }
    }
}
