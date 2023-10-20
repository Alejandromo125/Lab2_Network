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
    // Start is called before the first frame update

    private UdpClient udpListener;
    private string serverIP = "127.0.0.1";
    private int port = 12345;
    [SerializeField] SceneLoader sceneLoader;
    private async void Start()
    {
        udpListener = new UdpClient(port);
        Debug.Log("Server started on port " + port);
    }
    private async void Update()
    {
        UdpReceiveResult result = await udpListener.ReceiveAsync();
        string receivedMessage = Encoding.UTF8.GetString(result.Buffer);
        if(SceneManager.LoadSceneAsync(receivedMessage,LoadSceneMode.Additive).isDone)
        {
            byte[] data = Encoding.UTF8.GetBytes(receivedMessage);
            await udpListener.SendAsync(data, data.Length, serverIP, port);
        }

        
    }
    private void OnDisable()
    {
        udpListener.Close();
    }
}
