using UnityEngine;
using System.Net.Sockets;
using System.Text;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using System.Net;

public class ClientUDP_Script : MonoBehaviour
{
    
    [SerializeField]
    private TMP_InputField InputFieldTextIP;
    [SerializeField]
    private TMP_InputField InputFieldTextUserName;

    private UdpClient udpClient;
    private string currentServerIP;
    private string userName;
    private int serverPort = 12345;

    private string lastMessage;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
    private void Start()
    {
        AddMessageEventHandler();
    }
    public async void LogIn()
    {
        currentServerIP = InputFieldTextIP.text;
        userName = InputFieldTextUserName.text;
        udpClient = new UdpClient();
        udpClient.Connect(currentServerIP, serverPort);
        udpClient.BeginReceive(ReceiveCallback, null);

        //string username = InputFieldTextUserName.text + " has joined the room";
        //byte[] data = Encoding.UTF8.GetBytes(username);
        //await udpClient.SendAsync(data, data.Length, currentServerIP, serverPort);

       
        SceneManager.LoadScene("WaitingRoom");
        
        
    }

    private void ReceiveCallback(IAsyncResult ar)
    {
        IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse(currentServerIP), serverPort);
        byte[] data = udpClient.EndReceive(ar, ref serverEndPoint);
        string message = Encoding.UTF8.GetString(data);

        // Handle the received message (e.g., display it in the chat UI)
        HandleReceivedMessage(message);

        udpClient.BeginReceive(ReceiveCallback, null);
    }

    private void AddMessageEventHandler()
    {
        MessageEventHandler messageEventHandler = null;
        messageEventHandler = FindObjectOfType<MessageEventHandler>();
        if(messageEventHandler != null) 
        {
            messageEventHandler.OnButtonClicked += HandleCallbackEvent;
        }
    }
    public void HandleCallbackEvent()
    {
        string message =userName + ":" + UiManager.instance.InputFieldMessage.text;
        SendMessage(message);
        UiManager.instance.UpdateText(message);
    }
    private void Update()
    {
        try
        {
        }
        catch (Exception ex)
        {

        }


    }
    private void OnDisable()
    {
        udpClient.Close();
    }

    private bool isFromAnotherUser(string infoSent)
    {
        string[] messageParts = infoSent.Split(':');
        if (messageParts[0] == userName)
        {
            Debug.Log("Same user");
            return false;
        }
        else
        {
            return true;
        }
    }
    public void HandleReceivedMessage(string message)
    {
        UiManager.instance.UpdateText(message);
    }
    public void SendMessage(string message)
    {
        byte[] data = Encoding.UTF8.GetBytes(message);
        udpClient.Send(data, data.Length, currentServerIP, serverPort);
    }

}
