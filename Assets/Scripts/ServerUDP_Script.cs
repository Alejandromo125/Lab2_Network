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
    private bool responseSent = false;
    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
    private async void Start()
    {
        udpListener = new UdpClient(port);
        Debug.Log("Server started on port " + port);

        
        UdpReceiveResult result = await udpListener.ReceiveAsync();
        string receivedMessage = Encoding.UTF8.GetString(result.Buffer);
        StartCoroutine(StartWaitingScene(receivedMessage));

             
    }
    private async void Update()
    {
        try
        {
            UdpReceiveResult result = await udpListener.ReceiveAsync();
            string receivedMessage = Encoding.UTF8.GetString(result.Buffer);
            UiManager.instance.UpdateText(receivedMessage);
            if (!responseSent)
            {
                lastMessage = receivedMessage;
                
                responseSent = true;
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

    public async void BroadcastMessage()
    {
        byte[] data = Encoding.UTF8.GetBytes(lastMessage);
        await udpListener.SendAsync(data, data.Length, serverIP, port);
        UiManager.instance.UpdateText(lastMessage);

        responseSent = false;
    }

}
