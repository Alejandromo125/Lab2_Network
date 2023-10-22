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
    private IPEndPoint clientEndPoint;
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
        clientEndPoint = result.RemoteEndPoint;
        string message = Encoding.UTF8.GetString(result.Buffer);
        StartCoroutine(StartWaitingScene(message));

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
    private IEnumerator StartWaitingScene(string userConnected)
    {
        Debug.Log("Client connected" + userConnected);
        yield return(5);
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
        byte[] data = Encoding.UTF8.GetBytes(message);
        udpListener.SendAsync(data, data.Length, clientEndPoint);
        UiManager.instance.UpdateText(message);
    }

    private void HandleRecieveMessages()
    {
        while (true)
        {
            byte[] receivedData = udpListener.Receive(ref clientEndPoint);
            string receivedMessage = Encoding.UTF8.GetString(receivedData);
            lastMessage = receivedMessage;
        }
        
    }

}
