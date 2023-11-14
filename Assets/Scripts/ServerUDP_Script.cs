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
using UnityEditor.VersionControl;

public class ServerUDP_Script : MonoBehaviour
{

    private UdpClient udpListener;
    private string serverIP = "127.0.0.1";
    private int port = 12345;
    private string lastMessage = string.Empty;
    private Thread serverThread;
    private List<IPEndPoint> clientEndPoint = new List<IPEndPoint>();

    private string currentScene;

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
        Message message = new Message("/start_room",null,TypesOfMessage.WAITING_ROOM);
        HandleSendingMessages(message);
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
        Message newMessage = new Message(message, null, TypesOfMessage.WAITING_ROOM);
        HandleSendingMessages(newMessage);
    }

    private async void HandleRecieveMessages()
    {
        while (true)
        {
            UdpReceiveResult result =  await udpListener.ReceiveAsync();
            string receivedMessage = Encoding.UTF8.GetString(result.Buffer);
            Message newMessage = JsonUtility.FromJson<Message>(receivedMessage);

            switch (newMessage.type)
            {
                case TypesOfMessage.WAITING_ROOM:
                    if (UiManager.instance != null)
                        UiManager.instance.UpdateText(newMessage.message);
                    break;
                case TypesOfMessage.GAMEPLAY_ROOM:
                    break;
                case TypesOfMessage.START_GAME:
                    break;
            }

            if (!clientEndPoint.Contains(result.RemoteEndPoint))
            {
                Debug.Log(newMessage.message);
                clientEndPoint.Add(result.RemoteEndPoint);                
            }

            HandleSendingMessages(newMessage);
            lastMessage = newMessage.message;
        }
        
    }

    public void HandleSendingMessages(Message message)
    {
        string jsonData = JsonUtility.ToJson(message);
        byte[] data = Encoding.UTF8.GetBytes(jsonData);
        switch(message.type)
        {
            case TypesOfMessage.WAITING_ROOM:
                if(UiManager.instance != null)
                {
                    UiManager.instance.UpdateText(message.message);
                }
                break;
            case TypesOfMessage.GAMEPLAY_ROOM:
                break;
            case TypesOfMessage.START_GAME:
                break;
        }

        
        foreach (IPEndPoint client in clientEndPoint)
        {
            udpListener.SendAsync(data, data.Length, client);
        }
    }

}
