using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CS_PARALLEL_TCPSOCKET
{
    internal class PARALLEL
    {
        static void Main(string[] args)
        {
            List<SEND_LIST> list = new List<SEND_LIST>();

            // 대충 만든 List 항목
            for (int i = 0; i < 5; i++) 
            {
                SEND_LIST send_list = new SEND_LIST()
                {
                    equipNum = $"{i + 1}",
                    name = $"테스트_{i + 1}",
                    ip = $"123.123.123.00{i}",
                    port = $"111{i}"
                };

                list.Add(send_list);
            }

            Parallel.ForEach(list, new ParallelOptions() { MaxDegreeOfParallelism = 5 }, (vo) =>
            {
                using (SEND_HI send = new SEND_HI())
                {
                    try
                    {
                        send.Connect(vo.ip, vo.port);
                    }
                    catch
                    {
                        Console.WriteLine($"{vo.name}({vo.ip}) :: Connect Error!!!!");
                        return;
                    }

                    try
                    {
                        string res = send.Send();
                        Console.WriteLine($"{vo.name}({vo.ip}) :: {res}");
                    }
                    catch (SocketException ex)
                    {
                        Console.WriteLine($"{vo.name}({vo.ip}) :: {ex}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"{vo.name}({vo.ip}) :: {ex}");
                    }
                }
            });
        }
    }
}
