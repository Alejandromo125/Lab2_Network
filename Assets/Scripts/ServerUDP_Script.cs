    using System.Collections;
using System.Collections.Generic;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class ServerUDP_Script : MonoBehaviour
{
    // Start is called before the first frame update

    private int recv;
    byte[] data;
    Socket newsock;
    EndPoint Remote;
    void Start()
    {
        data = new byte[1024];
        IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 9050);

        newsock = new Socket(AddressFamily.InterNetwork,
                        SocketType.Dgram, ProtocolType.Udp);

        newsock.Bind(ipep);
        newsock.Listen(10);
        Debug.Log("Waiting for a client...");

        IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
        Remote = (EndPoint)(sender);

        recv = newsock.ReceiveFrom(data, ref Remote);

        Debug.Log("Message received from: " + Remote.ToString());
        Debug.Log(Encoding.ASCII.GetString(data, 0, recv));

        string welcome = "Welcome to my test server";
        data = Encoding.ASCII.GetBytes(welcome);
        newsock.SendTo(data, data.Length, SocketFlags.None, Remote);
    }

    // Update is called once per frame
    void Update()
    {
        data = new byte[1024];
        recv = newsock.ReceiveFrom(data, ref Remote);

        Console.WriteLine(Encoding.ASCII.GetString(data, 0, recv));
        newsock.SendTo(data, recv, SocketFlags.None, Remote);

    }
}
