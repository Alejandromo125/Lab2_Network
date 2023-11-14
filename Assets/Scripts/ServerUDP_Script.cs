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
    private bool lastMessageUpdated = true;
    private Message lastMessage;
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
        UdpReceiveResult result = await udpListener.ReceiveAsync();
        clientEndPoint.Add(result.RemoteEndPoint);
        string message = Encoding.UTF8.GetString(result.Buffer);
        StartWaitingScene();

        serverThread = new Thread(HandleRecieveMessages);
        serverThread.Start();
             
    }
    private void Update()
    {
       if(lastMessageUpdated == false)
       {
            HandleMessageOutput(lastMessage);
            lastMessageUpdated = true;
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

            lastMessage = newMessage;
            lastMessageUpdated = false;


            if (!clientEndPoint.Contains(result.RemoteEndPoint))
            {
                Debug.Log(newMessage.message);
                clientEndPoint.Add(result.RemoteEndPoint);                
            }

            HandleSendingMessages(newMessage);

        }
        
    }

    private void HandleMessageOutput(Message message)
    {
        try
        {
            switch (message.type)
            {
                case TypesOfMessage.WAITING_ROOM:
                    UiManager.instance.UpdateText(message.message);
                    break;
                case TypesOfMessage.GAMEPLAY_ROOM:
                    GameManager.instance.UpdatePlayersData(message);
                    break;
                case TypesOfMessage.START_GAME:
                    SceneManager.LoadSceneAsync("GameplayRoom");
                    break;
            }
        }
        catch(Exception e) 
        {
            Debug.Log(e);
        }
    }

    public void HandleSendingMessages(Message message)
    {
        string jsonData = JsonUtility.ToJson(message);
        byte[] data = Encoding.UTF8.GetBytes(jsonData);
        switch (message.type)
        {
            case TypesOfMessage.WAITING_ROOM:
                if (UiManager.instance != null)
                {
                    UiManager.instance.UpdateText(message.message);
                    Debug.Log("SEND MESSAGE");

                }
                break;
            case TypesOfMessage.GAMEPLAY_ROOM:

                break;
            case TypesOfMessage.START_GAME:
                Debug.Log("SEND MESSAGE");
                SceneManager.LoadSceneAsync("GameplayRoom");

                break;
        }
        foreach (IPEndPoint client in clientEndPoint)
        {
            udpListener.SendAsync(data, data.Length, client);
        }
    }

}
