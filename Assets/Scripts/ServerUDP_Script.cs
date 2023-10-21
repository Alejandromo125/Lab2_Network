using System.Collections;
using System.Collections.Generic;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using TMPro;

public class ServerUDP_Script : MonoBehaviour
{

    private UdpClient udpListener;
    private string serverIP = "192.168.56.1";
    private int port = 12345;
    private string lastMessage = string.Empty;

    private IPEndPoint _clientEndPoint;
    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
    private async void Start()
    {
        udpListener = new UdpClient(port);
        Debug.Log("Server started on port " + port);
        AddMessageEventHandler();
        udpListener.BeginReceive(ReceiveCallback, null);
    }

    private void ReceiveCallback(IAsyncResult ar)
    {
        IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, port);
        _clientEndPoint = clientEndPoint;
        byte[] data =  udpListener.EndReceive(ar, ref clientEndPoint);
        string message = Encoding.UTF8.GetString(data);

        // Handle the received message here (e.g., broadcast to clients)
        UiManager.instance.UpdateText(message);

        udpListener.BeginReceive(ReceiveCallback, null);
    }

    private async void Update()
    {
        try
        {
            //udpListener.BeginReceive(ReceiveCallback, null);
        }
        catch (Exception ex) 
        {

        }
       
    }

    private void OnDisable()
    {
        udpListener.Close();
    }
    private IEnumerator StartWaitingScene(string userConnected)
    {
        Debug.Log("Client connected " + userConnected);
        yield return(5);
    }

    public async void BroadcastMessage(string message)
    {
        byte[] data = Encoding.UTF8.GetBytes(message);
        udpListener.Send(data, data.Length, _clientEndPoint);
        UiManager.instance.UpdateText(message);

    }

    private void AddMessageEventHandler()
    {
        MessageEventHandler messageEventHandler = null;
        messageEventHandler = FindObjectOfType<MessageEventHandler>();
        if (messageEventHandler != null)
        {
            messageEventHandler.OnButtonClicked += HandleCallbackEvent;
        }
    }
    public void HandleCallbackEvent()
    {
        string message = "Server" + ":" + UiManager.instance.InputFieldMessage.text;
        BroadcastMessage(message);
    }

}
