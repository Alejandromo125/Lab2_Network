using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine.Windows;

public class ClientUDP_Script : MonoBehaviour
{
    // Start is called before the first frame update
    byte[] data;
    string input, stringData;
    Socket server;
    EndPoint Remote;
    int recv;
    void Start()
    {
        data = new byte[1024];
        
        IPEndPoint ipep = new IPEndPoint(
                        IPAddress.Parse("192.168.104.32"), 9050);

        server = new Socket(AddressFamily.InterNetwork,
                       SocketType.Dgram, ProtocolType.Udp);


        string welcome = "Hello, are you there?";
        data = Encoding.ASCII.GetBytes(welcome);
        server.SendTo(data, data.Length, SocketFlags.None, ipep);

        IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
        Remote = (EndPoint)sender;

        data = new byte[1024];
        recv = server.ReceiveFrom(data, ref Remote);

        Debug.Log("Message received from: " +  Remote.ToString());
        Debug.Log(Encoding.ASCII.GetString(data, 0, recv));
    }

    // Update is called once per frame
    void Update()
    {
        if(UnityEngine.Input.GetKeyDown(KeyCode.Escape)) 
        {
            CloseClient();
        }

        if (UnityEngine.Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            SendMessage("hola");
        }
        
    }

    void CloseClient()
    {
        Debug.Log("Stopping client");
        server.Close();
    }

    void SendMessage(string message)
    {
        server.SendTo(Encoding.ASCII.GetBytes(message), Remote);
        data = new byte[1024];
        recv = server.ReceiveFrom(data, ref Remote);
        stringData = Encoding.ASCII.GetString(data, 0, recv);
        Debug.Log(stringData);
    }
}
