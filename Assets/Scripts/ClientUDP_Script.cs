using UnityEngine;
using System.Net.Sockets;
using System.Text;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using System.Net;
using System.Threading;

public class ClientUDP_Script : MonoBehaviour
{
    
    [SerializeField]
    private TMP_InputField InputFieldTextIP;
    [SerializeField]
    private TMP_InputField InputFieldTextUserName;
    [SerializeField]
    private TMP_InputField InputPortClient;

    private UdpClient udpClient;
    private string currentServerIP;
    private string userName;
    private int serverPort = 12345;
    private int clientPort;


    private Thread listenerThread;

    private Message lastMessage;
    private bool lastMessageUpdated = true;


    #region ConnectionCheckerTimers
    private float lastPingTime;
    #endregion


    //Creating an instance to access it through player's scripts
    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
    private void Update()
    {
        if(lastMessageUpdated == false)
        {
            HandleMessageOutput(lastMessage);
            lastMessageUpdated = true;
        }


       
    }
    public async void LogIn()
    {
        currentServerIP = InputFieldTextIP.text;
        userName = InputFieldTextUserName.text;
        clientPort = int.Parse(InputPortClient.text);
        udpClient = new UdpClient(clientPort);

        string connectText = "/connect:" + InputFieldTextUserName.text;
        Message _message = new Message(connectText, null, TypesOfMessage.WAITING_ROOM);
        string jsonData = JsonUtility.ToJson(_message);
        byte[] data = Encoding.UTF8.GetBytes(jsonData);
        IPEndPoint recipientEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), serverPort);
        await udpClient.SendAsync(data, data.Length, recipientEndPoint);

        listenerThread = new Thread(RecieveMessages);
        listenerThread.Start();

        SceneManager.LoadScene("WaitingRoom");
    }
    #region CheckConnectionMessages
    public void SendCheckConnection()
    {
        string message = "ping";
        IPEndPoint recipientEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), serverPort);
        Message _message = new Message(message, null, TypesOfMessage.CHECK_CONNECTION);
        string jsonData = JsonUtility.ToJson(_message);
        byte[] data = Encoding.UTF8.GetBytes(jsonData);
        udpClient.SendAsync(data, data.Length, recipientEndPoint);
    }
    #endregion
    #region WaitingRoomMessages
    public void SendMessageWaitingRoom()
    {
        string message = userName + ":" + UiManager.instance.InputFieldMessage.text;
        IPEndPoint recipientEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), serverPort);
        Message _message = new Message(message, null, TypesOfMessage.WAITING_ROOM);
        string jsonData = JsonUtility.ToJson(_message);
        byte[] data = Encoding.UTF8.GetBytes(jsonData);
        udpClient.SendAsync(data, data.Length, recipientEndPoint);
        UiManager.instance.UpdateText(message);
    }
    #endregion

    #region GameplayRoomMessages
    public void SendMessageGameplay(Message _message)
    {
        _message.message = userName+":" + _message.message;
        string jsonData = JsonUtility.ToJson(_message);
        byte[] data = Encoding.UTF8.GetBytes(jsonData);
        IPEndPoint recipientEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), serverPort);
        udpClient.SendAsync(data, data.Length, recipientEndPoint);
    }
    #endregion

    private async void RecieveMessages()
    {
        while(true) 
        {
            
            try
            {
                UdpReceiveResult result = await udpClient.ReceiveAsync();
                string receivedMessage = Encoding.UTF8.GetString(result.Buffer);
                Message newMessage = JsonUtility.FromJson<Message>(receivedMessage);

                lastMessage = newMessage;
                lastMessageUpdated = false;
            }
            catch (Exception ex)
            {

            }
               
            
        }
       
    }
    private void HandleMessageOutput(Message message)
    {
        try
        {
            switch (message.type)
            {
                case TypesOfMessage.WAITING_ROOM:
                    if (isFromAnotherUser(message.message))
                    {
                        UiManager.instance.UpdateText(message.message);
                    }
                    break;
                case TypesOfMessage.GAMEPLAY_ROOM:
                    if(isFromAnotherUser(message.message))
                    {
                        message.message = ReturnCorrectDummyName(message.message);
                        GameManager.instance.UpdatePlayersData(message);
                    }
                    break;
                case TypesOfMessage.START_GAME:
                    SceneManager.LoadSceneAsync("GameplayRoom");
                    break;
                case TypesOfMessage.CHECK_CONNECTION:
                    lastPingTime = Time.time;
                    break;
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
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

    private string ReturnCorrectDummyName(string message)
    {
        string[] _messageArray = message.Split(':');

        return _messageArray[1];
    }

    private async void HandleCheck()
    {
        while (true)
        {
            if (lastPingTime + 4 < Time.time)
            {
                SceneManager.LoadScene("MainMenuScene");
                Destroy(this);
            }
        }
    }
}
