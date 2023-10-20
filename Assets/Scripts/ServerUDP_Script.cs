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

public class ServerUDP_Script : MonoBehaviour
{

    private UdpClient udpListener;
    private string serverIP = "127.0.0.1";
    private int port = 12345;
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

            Debug.Log(receivedMessage);
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
}
