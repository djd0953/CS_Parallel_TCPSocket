using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CS_PARALLER_TCPSOCKET
{
    public class SEND_HI : IDisposable
    {
        protected TCPSOCKET client = new TCPSOCKET();

        public byte[] tx_buff = new byte[0];
        public byte[] rx_buff = new byte[0];

        private bool disposedValue;

        public void Connect(string ip, string port)
        {
            client.Connect(ip, int.Parse(port));
        }

        public void DisConnect()
        {
            client.DisConnect();
        }

        public string Send()
        {
            HI_VO vo = new HI_VO();
            string rtv = null;

            //SEND
            try
            {
                byte[] asciiHI = { 0x72, 0x73 };
                vo.Data = asciiHI;
                tx_buff = vo.GetRaw();
                client.Send(tx_buff);
            }
            catch
            {
                throw;
            }

            // RECV
            try
            {
                rx_buff = new byte[6];
                client.Recv(rx_buff);

                if (rx_buff[0] == 0x02 && rx_buff[rx_buff.Length - 1] == 0x03)
                {
                    try
                    {
                        rtv = rx_buff[2].ToString("X2");
                        rtv += rx_buff[3].ToString("X2");

                    }
                    catch
                    {
                        throw new Exception("Protocol Error");
                    }
                }

            }
            catch
            {

            }

            return rtv;
        }


        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (client != null)
                    {
                        client.DisConnect();
                    }

                }

                disposedValue = true;
            }
        }

        ~SEND_HI()
        {
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
