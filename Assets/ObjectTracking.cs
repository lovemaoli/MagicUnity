using System;
using System.Data.Common;
// using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace landmarktest
{
public class ObjectTracking : MonoBehaviour
{
    // Start is called before the first frame update
    public UDPReceive2 udpReceive;
    public GameObject seeObj1;
    public GameObject bear;
    public GameObject boat;
    void Awake()
    {

    }


    // Update is called once per frame
    void Update()
    {
        string data = udpReceive.data;
        print(data);
        if(data=="bird"){
            //显示鸟
            print("获取到bird");
            seeObj1.active = true;
        }
        if (data=="bear")
        {  
            bear.active = true;
        }
        if (data=="boat")
        {
            boat.active = true;
        }
    }
}
}