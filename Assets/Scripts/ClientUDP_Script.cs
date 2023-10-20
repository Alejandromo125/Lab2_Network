using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine.Windows;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class ClientUDP_Script : MonoBehaviour
{
    private UdpClient udpClient;
    [SerializeField]
    private TMP_InputField InputFieldText;
    private string currentServerIP;
    private int serverPort = 12345;

    public async void LogIn()
    {
        currentServerIP = InputFieldText.text;
        udpClient = new UdpClient();

        // Send a message to the server
        string userName = "WaitingRoom";
        byte[] data = Encoding.UTF8.GetBytes(userName);
        await udpClient.SendAsync(data, data.Length, currentServerIP, serverPort);

        UdpReceiveResult result = await udpClient.ReceiveAsync();
        string receivedMessage = Encoding.UTF8.GetString(result.Buffer);

        SceneManager.LoadScene(receivedMessage, LoadSceneMode.Additive);
    }
    private void OnDisable()
    {
        udpClient.Close();
    }
}
