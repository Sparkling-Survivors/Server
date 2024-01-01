using System.Net.Sockets;
using System.Text;

namespace ServerCore;

public class Session
{
    private Socket _socket;
    private int _disconnected = 0;

    public void Start(Socket socket)
    {
        _socket = socket;
        SocketAsyncEventArgs recvArgs = new SocketAsyncEventArgs();
        recvArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnRecvCompleted);

        recvArgs.SetBuffer(new byte[1024], 0, 1024);

        RegisterRecv(recvArgs);
    }

    public void Send(byte[] sendBuffer)
    {
        SocketAsyncEventArgs sendArgs = new SocketAsyncEventArgs();
        sendArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnSendCompleted);
        sendArgs.SetBuffer(sendBuffer, 0, sendBuffer.Length);

        RegisterSend(sendArgs);
    }

    public void Disconnect()
    {
        if (Interlocked.Exchange(ref _disconnected, 1) == 1)
            return;
        _socket.Shutdown(SocketShutdown.Both);
        _socket.Close();
    }

    #region 네트워크 통신

    void RegisterSend(SocketAsyncEventArgs args)
    {
        bool pending = _socket.SendAsync(args);
        if (pending == false)
        {
            OnSendCompleted(null, args);
        }
    }

    void OnSendCompleted(object sender, SocketAsyncEventArgs args)
    {
        if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
        {
            try
            {
            }
            catch (Exception e)
            {
                Console.WriteLine($"OnSendCompleted Failed {e}");
            }
        }
        else
        {
            Disconnect();
        }
    }

    void RegisterRecv(SocketAsyncEventArgs args)
    {
        bool pending = _socket.ReceiveAsync(args);
        if (pending == false)
        {
            OnRecvCompleted(null, args);
        }
    }

    void OnRecvCompleted(Object sender, SocketAsyncEventArgs args)
    {
        if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
        {
            //TODO
            try
            {
                string recvData = Encoding.UTF8.GetString(args.Buffer, args.Offset, args.BytesTransferred);
                Console.WriteLine($"[From Client] {recvData}");
                RegisterRecv(args);
            }
            catch (Exception e)
            {
                Console.WriteLine($"OnRecvCompleted Failed {e}");
            }
        }
        else
        {
            //TODO
            Disconnect();
        }
    }

    #endregion
}