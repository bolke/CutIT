using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CutIT.GRBL
{
    public class TcpGrblClient:GrblClient
    {
        TcpClient _tcpClient;

        public TcpGrblClient()
        {

        }

        public bool IsConnected
        {
            get { return _tcpClient == null ? false : _tcpClient.Connected; }
        }
        public bool Start(string hostname, int port)
        {
            _tcpClient = new TcpClient();
            _tcpClient.Connect(hostname, port);
            if (_tcpClient.Connected)
            {
                return Start(_tcpClient.GetStream());
            }
            _tcpClient.Close();
            _tcpClient = null;
            return false;
        }

        public override void Stop()
        {
            if(_tcpClient!=null)
                _tcpClient.Close();
            base.Stop();
        }
    }
}
