using UnityEngine;
using System.Net.Sockets;
using System.Text;
using UnityEngine.SceneManagement;
using TMPro;

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

    private string waitingRoom = "WaitingRoom";
    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
    public async void LogIn()
    {
        currentServerIP = InputFieldTextIP.text;
        userName = InputFieldTextUserName.text;
        udpClient = new UdpClient();

        string username = InputFieldTextUserName.text;
        byte[] data = Encoding.UTF8.GetBytes(username);
        await udpClient.SendAsync(data, data.Length, currentServerIP, serverPort);

       
        SceneManager.LoadScene(waitingRoom);
    }

    public async void SendMessage()
    {
        string message = "Mensaje de ejemplo enviado por" + userName;
        byte[] data = Encoding.UTF8.GetBytes(message);
        await udpClient.SendAsync(data, data.Length, currentServerIP, serverPort);
    }
    private void Update()
    {
        if (Input.GetKey(KeyCode.KeypadEnter))
        {
            SendMessage();
        }
            
        
    }
    private void OnDisable()
    {
        udpClient.Close();
    }
}
