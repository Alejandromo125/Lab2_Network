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
    private string serverIP = "127.0.0.1";
    private int port = 12345;
    private string lastMessage = string.Empty;
    private Thread serverThread;
    private List<IPEndPoint> clientEndPoint = new List<IPEndPoint>();
    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
    private async void Start()
    {
        udpListener = new UdpClient(port);
        

        Debug.Log("Server started on port " + port);

        SceneManager.activeSceneChanged += AddMessageEventHandler;

        UdpReceiveResult result = await udpListener.ReceiveAsync();
        clientEndPoint.Add(result.RemoteEndPoint);
        string message = Encoding.UTF8.GetString(result.Buffer);
        StartWaitingScene();

        serverThread = new Thread(HandleRecieveMessages);
        serverThread.Start();
             
    }
    private async void Update()
    {
        try
        {
            if(lastMessage != string.Empty)
            {
                UiManager.instance.UpdateText(lastMessage);
                lastMessage = string.Empty;
            }
                

        }
        catch (Exception ex) 
        {

        }
       
    }

    private void OnDisable()
    {
        udpListener.Close();
    }
    public void  StartWaitingScene()
    {
        HandleSendingMessages("/start_room");
        SceneManager.LoadSceneAsync("WaitingRoom");
    }

    private void AddMessageEventHandler(Scene current, Scene next)
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
        HandleSendingMessages(message);
        UiManager.instance.UpdateText(message);
    }

    private async void HandleRecieveMessages()
    {
        while (true)
        {
            UdpReceiveResult result =  await udpListener.ReceiveAsync();
            string receivedMessage = Encoding.UTF8.GetString(result.Buffer);
            if (!clientEndPoint.Contains(result.RemoteEndPoint))
            {
                Debug.Log(receivedMessage);
                clientEndPoint.Add(result.RemoteEndPoint);                
            }
            HandleSendingMessages(receivedMessage);
            lastMessage = receivedMessage;
        }
        
    }

    private void HandleSendingMessages(string messages)
    {
        byte[] data = Encoding.UTF8.GetBytes(messages);
        foreach(IPEndPoint client in clientEndPoint)
        {
            udpListener.SendAsync(data, data.Length, client);
        }
    }

}
