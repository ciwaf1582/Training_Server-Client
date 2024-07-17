using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Socket sock = null;
            try //예외가 발생할 수 있는 코드
            {
                //Socket : 네트워크 연결을 관리하는 기본 도구 
                //주소 패밀리, 소켓 유형, 프로토콜을 지정하여 초기화
                //                IPv4 주소 체계              연결 지향형 소켓   TCP 프로토콜
                sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); //소켓 생성

                /****************서버 측 주소******************/
                IPAddress addr = IPAddress.Parse("127.0.0.1"); //ip
                IPEndPoint iep = new IPEndPoint(addr, 6000); //port
                sock.Bind(iep); //로컬 호스트의 네트워크 장치를 점유

                sock.Listen(5); //연결 요청 동안 자료구조(백로그 큐 크기 설정)
                Socket dosock;
                while (true)
                {
                    dosock = sock.Accept(); //연결 요청을 수락하고 새 소켓을 반환하여 통신을 처리
                    DoItAsync(dosock);
                }
            }
            catch //예외 처리 코드
            {

            }
            finally //예외 발생 여부와 상관없이 항상 실행되는 코드
            {
                sock.Close();
            }
        }
        delegate void DoItDele(Socket dosock); //DoIt메소드와 같은 형식
        /*================DoIt 메소드를 비동기로 실행하기 위한 메소드======================*/
        private static void DoItAsync(Socket dosock) 
        {
            DoItDele dele = DoIt; //델리
            dele.BeginInvoke(dosock, null, null); //BeginInvoke : 비동기 메소드를 호출하기 위한 메소드. 주로 델리게이트와 함께 씀
        }
        private static void DoIt(Socket dosock)
        {
            try
            {
                byte[] packet = new byte[1024];

                // LocalEndPoint : 자신의 끝점, RemoteEndPoint : 상대의 끝점
                IPEndPoint iep = dosock.RemoteEndPoint as IPEndPoint; //끝점을 IP형식으로 변환
                while (true)
                {
                    dosock.Receive(packet) ; //Receive : 소켓이 연결된 상태에서 데이터를 수신
                    MemoryStream ms = new MemoryStream(); //MemoryStream : 데이터의 임시 저장, 변환, 전송 등 사용
                    BinaryReader br = new BinaryReader(ms); //BinaryReader : 데이터를 이진 값으로 읽음
                    string msg = br.ReadString();
                    //불러온 후 클로즈
                    
                    
                    Console.WriteLine("{0}:{1} → {0}", iep.Address, iep.Port, msg);
                    if (msg == "exit") //msg == exit 라면 탈출
                    {
                        break;
                    }
                    dosock.Send(packet); //아니라면 패킷에 출력
                    br.Close();
                    ms.Close();
                }
            }
            catch
            {

            }
            finally
            {
                dosock.Close();
            }
        }
    }
}
