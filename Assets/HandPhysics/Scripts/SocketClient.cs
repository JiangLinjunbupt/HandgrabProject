//using UnityEngine;
//using System.Collections;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Net.Sockets;
//using System.Text;
//using Newtonsoft.Json;
//using System.Threading;
//using Assets.Script;

//public class SocketClient : MonoBehaviour
//{

//    public string serverIP = "127.0.0.1";
//    public Int32 port = 10200;

//    private HandinfController handinfCtrller;
//    private HandController handCtrller;

//    private static SocketClient Instance;
//    private Socket client;
//    private string SPLIT = "<EOF>";
//    private string SPLIT1 = "<EOF1>";
//     Size of receive buffer.
//    private const int BufferSize = 4096;
//     Receive buffer.
//    private byte[] buffer = new byte[BufferSize];
//     Received data string.

//    public static SocketClient GetInstance()
//    {
//        if (Instance == null)
//        {
//            Instance = new SocketClient();
//        }
//        return Instance;
//    }

//    private SocketClient()
//    {

//    }

//    void Start()
//    {
//        set hand controller
//        handinfCtrller = GetComponent<HandinfController>();
//         start client
//        StartClient(serverIP, port);
//        Debug.Log(System.Environment.Version);
//    }



//    public void StartClient(string addr, int port)
//    {
//        try
//        {
//            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse(addr), port);
//             Create a TCP/IP  socket.
//            client = new Socket(AddressFamily.InterNetwork,
//                SocketType.Stream, ProtocolType.Tcp);
//            client.Connect(remoteEP);
//            if (client.Connected)
//            {
//                string logInfo = string.Format("connected to {0}", remoteEP);
//                Debug.Log(logInfo);
//            }

//            new Thread(ReceiveFunc).Start();
//        }
//        catch (Exception)
//        {
//            Debug.Log("initialization fail");
//        }
//    }

//    private void ReceiveFunc()
//    {
//        string data = null;
//        string data1 = null;
//        while (client != null && client.Connected)
//        {
//            string logInfo = string.Format("connected to receive");
//            Debug.Log(logInfo);
//            try
//            {
//                data = null;
//                 An incoming connection needs to be processed.
//                while (true)
//                {
//                    buffer = new byte[4096];//1024*8
//                    int bytesRec = client.Receive(buffer);

//                    data += Encoding.ASCII.GetString(buffer, 0, bytesRec);
//                    string logInfo1 = string.Format("receive data");
//                    Debug.Log(logInfo1);
//                    if (data.IndexOf(SPLIT1) > -1)
//                    {
//                        break;
//                    }
//                }

//                string logInfo2 = string.Format("next");
//                Debug.Log(logInfo2);
//                data = data.Substring(0, data.IndexOf(SPLIT1));
//                 Show the data on the console.
//                data1 = data.Substring(0, data.IndexOf(SPLIT));
//                string logInfokk = string.Format("Data1 received : {0}", data1);
//                Debug.Log(logInfokk);

//                data = data.Substring(data.IndexOf(SPLIT)+5);
//                string logInfokkk = string.Format("Data received : {0}", data);
//                Debug.Log(logInfokkk);


//                var frame1 = JsonConvert.DeserializeObject<SkeletonJson>(data1);

//                var frame = JsonConvert.DeserializeObject<HandInf>(data);

//                if (!handinfCtrller.Mutex)
//                {
//                    handinfCtrller.update_data(frame);
//                    handCtrller.update_data(frame);
//                    string logInfo22 = string.Format("Handinf");
//                    Debug.Log(logInfo22);
//                }

//                if (!handCtrller.Mutex)
//                {
//                    string logInfo222 = string.Format("HandContr");
//                    Debug.Log(logInfo222);
//                    handCtrller.update_data(frame1);
//                    string logInfo222 = string.Format("HandContr");
//                    Debug.Log(logInfo222);
//                }


//            }
//            catch (Exception ex)
//            {
//                Debug.Log(ex.ToString());
//                break;
//            }

//        }


//    }
//}




using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using System.Threading;
using Assets.Script;

public class SocketClient : MonoBehaviour
{

    public string serverIP = "127.0.0.1";
    public Int32 port = 10200;

    private HandinfController handinfCtrller;
    private HandController handCtrller;

    private static SocketClient Instance;
    private Socket client;
    private string SPLIT = "<EOF>";
    private string SPLIT1 = "<EOF1>";
    //Size of receive buffer.
    private const int BufferSize = 4096;
    //Receive buffer.
    private byte[] buffer = new byte[BufferSize];
    // Received data string.

    public static SocketClient GetInstance()
    {
        if (Instance == null)
        {
            Instance = new SocketClient();
        }
        return Instance;
    }

    private SocketClient()
    {

    }

    void Start()
    {
        //set hand controller
        handCtrller = GetComponent<HandController>();
        handinfCtrller=GetComponent<HandinfController>();
        //handinfCtrller = GameObject.Find("RiggedPepperCutHandsinf").GetComponent<HandinfController>();
        //start client
        StartClient(serverIP, port);
        Debug.Log(System.Environment.Version);
    }



    public void StartClient(string addr, int port)
    {
        try
        {
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse(addr), port);
            //Create a TCP / IP  socket.
            client = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);
            client.Connect(remoteEP);
            if (client.Connected)
            {
                string logInfo = string.Format("connected to {0}", remoteEP);
                Debug.Log(logInfo);
            }

            new Thread(ReceiveFunc).Start();
        }
        catch (Exception)
        {
            Debug.Log("initialization fail");
        }
    }

    private void ReceiveFunc()
    {
        string data = null;
        while (client != null && client.Connected)
        {
            try
            {
                data = null;
                //An incoming connection needs to be processed.
                while (true)
                {
                    buffer = new byte[1024 * 8];
                    int bytesRec = client.Receive(buffer);
                    data += Encoding.ASCII.GetString(buffer, 0, bytesRec);
                    if (data.IndexOf(SPLIT1) > -1)
                    {
                        break;
                    }
                }
                //Show the data on the console.
                data = data.Substring(0, data.IndexOf(SPLIT1));
                var data1 = data.Substring(data.IndexOf(SPLIT) + 5);
                data = data.Substring(0, data.IndexOf(SPLIT));
                
                
                //string logInfo = string.Format("Data received : {0}", data);
                //Debug.Log(logInfo);
                //FrameData frame = JsonConvert.DeserializeObject<FrameData>(data);

                var frame = JsonConvert.DeserializeObject<SkeletonJson>(data);
                var frame1 = JsonConvert.DeserializeObject<HandInf>(data1);

                if (!handCtrller.Mutex)
                {
                    handCtrller.update_data(frame);
                    string logInfo222 = string.Format("HandContr");
                    Debug.Log(logInfo222);
                }

                if (!handinfCtrller.Mutex)
                {
                    handinfCtrller.update_data(frame1);
                    string logInfo223 = string.Format("Handinf");
                    Debug.Log(logInfo223);
                }
            }
            catch (Exception ex)
            {
                Debug.Log(ex.ToString());
            }
        }
    }
}