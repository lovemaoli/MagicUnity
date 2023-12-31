using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class UDPReceive2 : MonoBehaviour
{
    Thread receiveThread;
    UdpClient client = null;
    public int port = 50558;
    public bool startRecieving = true;
    public bool printToConsole = false;
    public string data;

    public void Start()
    {
        // Text text = GameObject.Find("Canvas/Port").GetComponent<Text>();
        // port = int.Parse(text.text);
        receiveThread = new Thread(new ThreadStart(ReceiveData));
        //receiveThread.IsBackground = true;
        receiveThread.Start();
    }

    private void ReceiveData()
    {
        client = new UdpClient(port);
        while (startRecieving)
        {
            try
            {
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any,0);
                byte[] dataByte = client.Receive(ref anyIP);
                data = Encoding.UTF8.GetString(dataByte);

                if(printToConsole){ print(data); }
            }
            catch (Exception err)
            {
                print(err.ToString());
            }
        }
        client.Close();
    }

    public void close(){
        //关闭线程，避免无法重复开启
        client.Close();
        receiveThread.Abort();
    }
}
