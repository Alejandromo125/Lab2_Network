using System.Net.Sockets;
using System.Net;
using UnityEngine;
using System.Threading;
using System.Text;
using System;

public class ClientTCP : MonoBehaviour
{
    protected int _port = 61111;
    protected string _socketName = "PlayerName";

    protected Socket _localSocket;

    protected void InitSocket(AddressFamily addressFamily, SocketType socketType, ProtocolType protocolType, Action<int> onConnected = null)
    {
        _localSocket = new Socket(addressFamily, socketType, protocolType);
        if (onConnected != null) onConnected(_port);
    }

    protected void ConnectToSocket(IPEndPoint endPoint, Action onConnected)
    {
        _localSocket.Connect(endPoint);
        if (onConnected != null) onConnected();
    }

    protected void SendData(Socket socket, byte[] data, Action<int> onDataSent)
    {
        socket.Send(data);
        if (onDataSent != null) onDataSent(data.Length);
    }

    protected void ListenData(Socket socket, Action<byte[]> onDataReceived, Action<int> onDataLength)
    {
        byte[] receivedBuffer = new byte[1024];
        int receivedBytes;

        while (true)
        {
            receivedBytes = socket.Receive(receivedBuffer);
            if (receivedBytes > 0)
            {
                byte[] data = new byte[receivedBytes];
                Array.Copy(receivedBuffer, data, receivedBytes);
                if (onDataReceived != null) onDataReceived(data);
                if (onDataLength != null) onDataLength(receivedBytes);
            }
        }
    }

    protected void CloseConnection(Socket socket)
    {
        if (socket != null && socket.Connected)
        {
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
        }
    }
}

public class UtilitiesClient
{
    public static int FreeTcpPort()
    {
        // Creating a temporary socket to find a free port
        using (var tempSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
        {
            tempSocket.Bind(new IPEndPoint(IPAddress.Any, 0));
            var endPoint = (IPEndPoint)tempSocket.LocalEndPoint;
            return endPoint.Port;
        }
    }

    public static bool ValidateIPAddress(string ip, out string cleanedIP)
    {
        if (IPAddress.TryParse(ip, out IPAddress ipAddress))
        {
            cleanedIP = ipAddress.ToString();
            return true;
        }
        else
        {
            cleanedIP = null;
            return false;
        }
    }
}

public class Client_TCP : SocketConnectionTCP
{
    #region Fields
    private Thread _writeServerThread;
    private Thread _readServerThread;
    public string serverIP = "127.0.0.1";
    #endregion

    #region Initializers and Cleanup
    private void Awake()
    {
        // Make sure in localhost client doesn't have the same port as server
        while (_port == 61111)
        {
            _port = UtilitiesClient.FreeTcpPort();
        }

        InitSocket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp, null);
    }

    private void OnDisable()
    {
        DisconnectFromServer();
    }

    #endregion

    #region Core func
    public void InitServerConnection()
    {
        if (_writeServerThread != null)
        {
            _writeServerThread.Abort();
            _writeServerThread = null;
        }

        if (_readServerThread != null)
        {
            _readServerThread.Abort();
            _readServerThread = null;
        }

        if (UtilitiesClient.ValidateIPAddress(serverIP, out string cleanedIp))
        {
            _writeServerThread = new Thread(() => ConnectToServer(cleanedIp));
            _writeServerThread.Start();
        }
        else
        {
            Debug.LogAssertion($"CLIENT TCP: Insert a valid IP Address, {serverIP} is not a valid IP address");
        }
    }

    public void DisconnectFromServer()
    {
        Debug.Log("CLIENT TCP: Init Server Disconnection");
        if (_writeServerThread != null) _writeServerThread.Abort();
        if (_readServerThread != null) _readServerThread.Abort();

        CloseConnection(_localSocket);
    }

    private void CloseConnection(Socket localSocket)
    {
        if (localSocket != null && localSocket.Connected)
        {
            localSocket.Shutdown(SocketShutdown.Both);
            localSocket.Close();
        }
    }

    void ConnectToServer(string ip)
    {
        IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse(ip), 61111);
        ConnectToSocket(serverEndPoint, () =>
        {
            SendData(_localSocket, Encoding.ASCII.GetBytes($"Hello {serverEndPoint}, my user name is: {_socketName}"), null);

            _readServerThread = new Thread(() => ListenData(_localSocket, null, null));
            _readServerThread.Name = "_readServerThread";
            _readServerThread.Start();
        });
    }
    #endregion
}