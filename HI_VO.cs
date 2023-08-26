using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS_PARALLER_TCPSOCKET
{
    public class HI_VO
    {
        private byte[] header = new byte[1];
        public string Header
        {
            get
            {
                return string.Format("{0:X2}", header[0]);
            }
            set
            {
                header[0] = Convert.ToByte(value, 16);
            }
        }

        private byte[] command = new byte[1];
        public string Command
        {
            get
            {
                return string.Format("{0:X2}", command[0]);
            }
            set
            {
                command[0] = Convert.ToByte(value, 16);
            }
        }

        private byte[] data = new byte[0];
        public byte[] Data
        {
            get
            {
                return data;
            }
            set
            {
                Array.Resize(ref data, value.Length);
                Array.Copy(value, data, value.Length);
            }
        }

        private byte[] crc = new byte[1];
        public string Crc
        {
            get
            {
                return string.Format("{0:X2}", crc[0]);
            }
            set
            {
                crc[0] = Convert.ToByte(value, 16);
            }
        }

        private byte[] footer = new byte[1];
        public string Footer
        {
            get
            {
                return string.Format("{0:X2}", footer[0]);
            }
            set
            {
                footer[1] = Convert.ToByte(value, 16);
            }
        }

        public HI_VO()
        {
            Header = "02";

            Command = "00";

            Footer = "03";
        }

        public HI_VO(string cmd)
        {
            Header = "02";

            Command = cmd;

            Footer = "03";
        }

        public byte[] GetRaw()
        {
            byte[] temp = new byte[header.Length + command.Length + data.Length + crc.Length + footer.Length];
            int offset = 0;

            Array.Copy(header, 0, temp, offset, header.Length);
            offset += header.Length;

            Array.Copy(command, 0, temp, offset, command.Length);
            offset += command.Length;

            Array.Copy(data, 0, temp, offset, data.Length);
            offset += data.Length;

            crc = xorCRC(temp);
            Array.Copy(crc, 0, temp, offset, crc.Length);
            offset += crc.Length;

            Array.Copy(footer, 0, temp, offset, footer.Length);

            return temp;
        }

        public byte[] xorCRC(byte[] bytes, bool isIncHeader = false)
        {
            byte[] rtv = new byte[1];
            int headerLength = header.Length;

            foreach (byte b in bytes) 
            {
                if (isIncHeader == false && headerLength > 0)
                {
                    headerLength--;
                    continue;
                }

                rtv[0] ^= b;
            }

            return rtv;
        }
    }
}
