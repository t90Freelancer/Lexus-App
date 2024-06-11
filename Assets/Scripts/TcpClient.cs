using UnityEngine;
using System;
using System.Net.Sockets;
using System.Text;
using TMPro;

public class TcpClient : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField inf_Ip;
    [SerializeField]
    private TMP_InputField inf_Port;
    [SerializeField]
    private TextMeshProUGUI tmpInform;
    [SerializeField]
    private TextMeshProUGUI tmp_DataReceive;

    [SerializeField]
    private GameObject DataPanel;
 
    private System.Net.Sockets.TcpClient client;
    private NetworkStream stream;
    private bool isConnected = false;

    public void ConnectDart()
    {
        ConnectToServer(inf_Ip.text, int.Parse(inf_Port.text)); // Địa chỉ IP và cổng của máy chủ Dart
    }

    void ConnectToServer(string ipAddress, int port)
    {
        try
        {
            client = new System.Net.Sockets.TcpClient();
            client.BeginConnect(ipAddress, port, OnConnected, null);
        }
        catch (Exception e)
        {
            tmpInform.text = "SocketException: " + e;
            Debug.LogError("SocketException: " + e);
        }
    }

    void OnConnected(IAsyncResult result)
    {
        try
        {
            client.EndConnect(result);
            stream = client.GetStream();
            isConnected = true;

            Debug.Log("Connected to server");
            DataPanel.SetActive(true);
            // Bắt đầu lắng nghe dữ liệu từ máy chủ
            byte[] buffer = new byte[1024];
            stream.BeginRead(buffer, 0, buffer.Length, OnDataReceived, buffer);
        }
        catch (Exception e)
        {
            tmpInform.text = "Connected Faild";
            Debug.LogError("SocketException: " + e);
        }
    }

    void OnDataReceived(IAsyncResult result)
    {
        try
        {
            int bytesRead = stream.EndRead(result);
            if (bytesRead > 0)
            {
                byte[] receivedBytes = (byte[])result.AsyncState;
                string receivedData = Encoding.ASCII.GetString(receivedBytes, 0, bytesRead);
                Debug.Log("Received data: " + receivedData);
                tmp_DataReceive.text = "Receive:"+receivedData;
                // Tiếp tục lắng nghe dữ liệu mới
                byte[] buffer = new byte[1024];
                stream.BeginRead(buffer, 0, buffer.Length, OnDataReceived, buffer);
            }
            else
            {
                // Nếu không còn dữ liệu được nhận, thì đóng kết nối
                Disconnect();
            }
        }
        catch (Exception e)
        {
            Debug.LogError("SocketException: " + e);
        }
    }

    void Disconnect()
    {
        if (isConnected)
        {
            stream.Close();
            client.Close();
            isConnected = false;
            Debug.Log("Disconnected from server");
        }
    }

    void OnDestroy()
    {
        Disconnect();
    }
}
