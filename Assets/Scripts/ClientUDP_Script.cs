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
    private int clientPort = 9050;
    private string lastMessage;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
    private void Start()
    {
        SceneManager.activeSceneChanged += AddMessageEventHandler;
    }
    public async void LogIn()
    {
        currentServerIP = InputFieldTextIP.text;
        userName = InputFieldTextUserName.text;
        udpClient = new UdpClient(clientPort);

        string username = InputFieldTextUserName.text + " has joined the room";
        byte[] data = Encoding.UTF8.GetBytes(username);
        IPEndPoint recipientEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), serverPort);
        await udpClient.SendAsync(data, data.Length, recipientEndPoint);


        SceneManager.LoadScene("WaitingRoom");
        
        
    }
    private void AddMessageEventHandler(Scene current, Scene next)
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
        byte[] data = Encoding.UTF8.GetBytes(message);
        IPEndPoint recipientEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), serverPort);
        udpClient.SendAsync(data, data.Length, recipientEndPoint);
        UiManager.instance.UpdateText(message);
    }
    private async void Update()
    {
        try
        {
            UdpReceiveResult result = await udpClient.ReceiveAsync();
            string receivedMessage = Encoding.UTF8.GetString(result.Buffer);
            UiManager.instance.UpdateText(receivedMessage);
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
}
