using UnityEngine;
using System.Net.Sockets;
using System.Text;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using System.Net;
using System.Threading;
using UnityEditor.VersionControl;

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


    //Creating an instance to access it through player's scripts
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
    #region WaitingRoomMessages
    private void AddMessageEventHandler(Scene current, Scene next)
    {
        MessageEventHandler messageEventHandler = null;
        messageEventHandler = FindObjectOfType<MessageEventHandler>();
        if(messageEventHandler != null) 
        {
            messageEventHandler.OnButtonClicked += SendMessageWaitingRoom;
        }
    }
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
        string message = userName + ":" + "gameplay_action";
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
                switch(newMessage.type)
                {
                    case TypesOfMessage.WAITING_ROOM: {
                            if (isFromAnotherUser(newMessage.message))
                            {
                                UiManager.instance.UpdateText(newMessage.message);
                            }

                            break; }
                    case TypesOfMessage.GAMEPLAY_ROOM: { break; }
                    case TypesOfMessage.START_GAME: { break; }
                       
                }
               
            }
            catch (Exception ex)
            {

            }
               
            
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
