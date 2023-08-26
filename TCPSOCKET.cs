using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CS_PARALLER_TCPSOCKET
{
    public enum TCP_CLIENT_STATUS { DISCONNECTED = 0, CONNECTING = 1, CONNECTED = 2 }

    public partial class TCPSOCKET : IDisposable
    {
        public Socket clnt_sock;

        public string ip;
        public int port;

        private bool disposedValue;

        public TCP_CLIENT_STATUS Status { get; set; } = TCP_CLIENT_STATUS.DISCONNECTED;

        public void Connect(string ip, int port)
        {
            this.ip = ip;
            this.port = port;

            try
            {
                Connect();
            }
            catch
            {
                throw;
            }
        }

        public void Connect()
        {
            try
            {
                // IPv4, 양방향, Tcp
                // https://learn.microsoft.com/ko-kr/dotnet/api/system.net.sockets.socketoptionname?view=netframework-4.7.2
                clnt_sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                clnt_sock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                clnt_sock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, false);
            }
            catch (Exception ex)
            {
                Status = TCP_CLIENT_STATUS.DISCONNECTED;
                throw ex;
            }

            try
            {
                // 비동기 Connect
                IAsyncResult result = clnt_sock.BeginConnect(ip, port, null, null);
                if (result.AsyncWaitHandle.WaitOne(5000) == false)
                {
                    throw new SocketException(10060);
                }
                else
                {
                    clnt_sock.EndConnect(result);
                    if (clnt_sock.Connected)
                    {

                    }
                    else
                    {
                        throw new SocketException(10060);
                    }
                }
            }
            catch (Exception ex)
            {
                Status = TCP_CLIENT_STATUS.DISCONNECTED;
                throw ex;
            }

            Status = TCP_CLIENT_STATUS.CONNECTED;
        }

        public void DisConnect()
        {
            Status = TCP_CLIENT_STATUS.DISCONNECTED;

            try
            {
                if (clnt_sock.Connected)
                    clnt_sock.Shutdown(SocketShutdown.Both);

            }
            catch { }

            try
            {
                if (clnt_sock.Connected)
                    clnt_sock.Close();
            }
            catch { }

            try
            {
                clnt_sock.Dispose();
            }
            catch { }
        }

        public int Send(byte[] buff)
        {
            return Send(buff, buff.Length);
        }

        public int Send(byte[] buff, int length)
        {
            return Send(clnt_sock, buff, length);
        }

        public int Send(Socket sock, byte[] buff, int length)
        {
            int nSend = 0;

            try
            {
                do
                {
                    // 비동기 Send, TimeOut 5초
                    IAsyncResult result = sock.BeginSend(buff, nSend, length - nSend, SocketFlags.None, null, null);
                    if (result.AsyncWaitHandle.WaitOne(5000) == false)
                    {
                        throw new SocketException(10060);
                    }

                    int rtv = sock.EndSend(result);
                    if (sock.Connected)
                    {
                        nSend += rtv;
                    }
                    else
                    {
                        throw new SocketException(10053);//?
                    }
                } while (nSend < length);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return nSend;
        }

        public int Recv(byte[] buff)
        {
            return Recv(clnt_sock, buff, buff.Length);
        }

        public int Recv(byte[] buff, int length)
        {
            return Recv(clnt_sock, buff, length);
        }

        public int Recv(Socket sock, byte[] buff, int length)
        {
            // 비동기 통신이라 Buffer 크기를 명확히 할것!
            int nRead = 0;

            try
            {
                do
                {
                    IAsyncResult result = sock.BeginReceive(buff, nRead, length - nRead, SocketFlags.None, null, null);
                    if (result.AsyncWaitHandle.WaitOne(5000) == false)
                    {
                        throw new SocketException(10060);
                    }

                    int rtv = sock.EndReceive(result);
                    if (rtv == 0)
                    {
                        sock.Shutdown(SocketShutdown.Both);
                        throw new SocketException(10053);
                    }

                    if (sock.Connected)
                    {
                        nRead += rtv;
                    }
                    else
                    {
                        throw new SocketException(10053);//?
                    }
                } while (nRead < length);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return nRead;
        }

        public bool Peek(byte[] buff, int length)
        {
            return Peek(clnt_sock, buff, length);
        }

        public bool Peek(Socket sock, byte[] buff, int length)
        {
            int nRead = 0;

            try
            {
                do
                {
                    IAsyncResult result = sock.BeginReceive(buff, nRead, length - nRead, SocketFlags.Peek, null, null);
                    if (result.AsyncWaitHandle.WaitOne(5000) == false)
                    {
                        throw new SocketException(10060);
                    }

                    int rtv = sock.EndReceive(result);
                    if (rtv == 0)
                    {
                        sock.Shutdown(SocketShutdown.Both);
                        throw new SocketException(10053);
                    }

                    if (sock.Connected)
                    {
                        nRead += rtv;
                    }
                    else
                    {
                        throw new SocketException(10053);//?
                    }
                } while (nRead < length);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return true;
        }

        private string GetIP(string hostname)
        {
            string ip = hostname;

            // DNS 조회
            try
            {
                IPHostEntry iphost = Dns.GetHostEntry(IPAddress.Parse(ip));
                foreach (IPAddress ipaddress in iphost.AddressList)
                {
                    if (ipaddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        ip = ipaddress.ToString();
                    }
                }
            }
            catch { }

            return ip;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (clnt_sock != null)
                    {

                        // TODO: 관리형 상태(관리형 개체)를 삭제합니다.
                        clnt_sock.Shutdown(SocketShutdown.Both);
                        clnt_sock.Close();

                        clnt_sock.Dispose();
                    }
                }

                // TODO: 비관리형 리소스(비관리형 개체)를 해제하고 종료자를 재정의합니다.
                // TODO: 큰 필드를 null로 설정합니다.
                disposedValue = true;
            }
        }

        // TODO: 비관리형 리소스를 해제하는 코드가 'Dispose(bool disposing)'에 포함된 경우에만 종료자를 재정의합니다.
        ~TCPSOCKET()
        {
            // 이 코드를 변경하지 마세요. 'Dispose(bool disposing)' 메서드에 정리 코드를 입력합니다.
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // 이 코드를 변경하지 마세요. 'Dispose(bool disposing)' 메서드에 정리 코드를 입력합니다.
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
