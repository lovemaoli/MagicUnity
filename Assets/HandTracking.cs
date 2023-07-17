using System;
using System.Data.Common;
// using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace landmarktest
{
public class HandTracking : MonoBehaviour
{
    // Start is called before the first frame update
    public UDPReceive udpReceive;
    public List<GameObject> handPoints;
    

    [SerializeField]
    private float thumbModelLength = 0.03f;
    private float scale;
    public Transform scaleTransform;
    private DepthCalibrator depthCalibrator = new DepthCalibrator(-0.0719f, 0.439f);
    private TransformLink[] transformLinkers;
    public string LinkType = "None";
    int flagR = 0;
    int flagL = 0;
    float Rx0 = 0;
    float Ry0 = 0;
    float Rz0 = 0;
    float RzD = 0;
    float Lx0 = 0;
    float Ly0 = 0;
    float Lz0 = 0;
    float LzD = 0;
    private float previousLeftPosition;
    private float previousRightPosition;
    public static bool leftStage = false;
    public static bool rightStage = false;
    //切换场景的相关变量
    private float leftTimeThreshold = 0.5f;
    private float rightTimeThreshold = 0.5f;
    private float elapsedLeftTime = 0f;
    private float elapsedRightTime = 0f;
    private int leftHandSwitch = 0;
    private int rightHandSwitch = 0;
    //放大缩小的相关变量
    private float scaleThreshold = 1f;
    private float elapsedScaleTime = 0f;
    private int scaleSwitch = 0;
    private float previousScale;

    void Awake()
    {
        transformLinkers = this.transform.GetComponentsInChildren<TransformLink>();
    }


    // Update is called once per frame
    void Update()
    {
        //识别按空格键切换下一个场景
        if(Input.GetKeyDown(KeyCode.Space)){
            udpReceive.close();
            if(SceneManager.GetActiveScene().buildIndex == SceneManager.sceneCountInBuildSettings-1){
                SceneManager.LoadScene(0);
            }else{
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
            return;
        }
        string data = udpReceive.data;
        // print(data);
        data = data.Remove(0, 1);
        data = data.Remove(data.Length-1, 1);
        string data1 = data;
        // print(data);
        string[] pointsLeft = null;
        string[] pointsRight = null;
        if(data.Contains("Left") == true && data.Contains("Right") == true)
        {
            data =data.Remove(data.LastIndexOf("Right")-3);
            data =data.Remove(0,data.LastIndexOf("Left")+6);
            pointsLeft = data.Split(',');
            data1 =data1.Remove(0,data1.LastIndexOf("Right")+7);
            pointsRight = data1.Split(',');
            
            //print(data1);
        }
        else if(data.Contains("Left") == true && data.Contains("Right") == false)
        {
            data =data.Remove(0,data.LastIndexOf("Left")+6);
            pointsLeft = data.Split(',');           
            print("OnlyL"+pointsLeft[0]+" flag:"+leftHandSwitch);
            //把pointsLeft[0]转换为float的currentLeftPosition
            float currentLeftPosition = float.Parse(pointsLeft[0]);
            //添加计时，如果在一定时间内data1[0]2s内增加超过200，就跳转到下一个场景
            if(leftHandSwitch==1 && previousLeftPosition-currentLeftPosition>150)leftHandSwitch=2;
            else if(leftHandSwitch==0 && Math.Abs(currentLeftPosition-previousLeftPosition)>50){
                elapsedLeftTime = 0f;
            }else if(leftHandSwitch==2 && Math.Abs(currentLeftPosition-previousLeftPosition)>50){
                leftHandSwitch=0;
                elapsedLeftTime = 0f;
            }else{
                if(leftHandSwitch!=1 && leftHandSwitch<=3){
                    elapsedLeftTime += Time.deltaTime;
                    if(elapsedLeftTime > leftTimeThreshold){
                        leftHandSwitch++;
                        elapsedLeftTime = 0f;
                    }
                }
            }
            if(leftHandSwitch==3){
                //切换上一个场景
                udpReceive.close();
                if(SceneManager.GetActiveScene().buildIndex == 0){
                    SceneManager.LoadScene(SceneManager.sceneCountInBuildSettings-1);
                }else{
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
                }
                // leftStage = true;
                return;
            }
            previousLeftPosition = currentLeftPosition;
        }
        else if(data.Contains("Left") == false && data.Contains("Right") == true)
        {
            data1 =data1.Remove(0,data1.LastIndexOf("Right")+7);
            pointsRight = data1.Split(',');           
            print("OnlyR"+pointsRight[0]+" flag1:"+rightHandSwitch+" "+ pointsRight[3] +" flag2:"+scaleSwitch);
            //判断切换场景
            //把pointsRight[0]转换为float的currentRightPosition
            float currentRightPosition = float.Parse(pointsRight[0]);
            //添加计时，如果在一定时间内data1[0]2s内增加超过200，就跳转到下一个场景
            if(rightHandSwitch==1 && currentRightPosition-previousRightPosition>200)rightHandSwitch=2;
            else if(rightHandSwitch==0 && Math.Abs(currentRightPosition-previousRightPosition)>50){
                elapsedRightTime = 0f;
            }else if(rightHandSwitch==2 && Math.Abs(currentRightPosition-previousRightPosition)>50){
                rightHandSwitch=0;
                elapsedRightTime = 0f;
            }else{
                if(rightHandSwitch!=1 && rightHandSwitch<=3){
                    elapsedRightTime += Time.deltaTime;
                    if(elapsedRightTime > rightTimeThreshold){
                        rightHandSwitch++;
                        elapsedRightTime = 0f;
                    }
                }
                
            }
            if(rightHandSwitch==3){
                //切换下一个场景
                udpReceive.close();
                if(SceneManager.GetActiveScene().buildIndex == SceneManager.sceneCountInBuildSettings-1){
                    SceneManager.LoadScene(0);
                }else{
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                }
                // rightStage = true;
                return;
            }
            previousRightPosition = currentRightPosition;
            //判断放大缩小
            float currentScale = float.Parse(pointsRight[3]);
            if(scaleSwitch==1 && Math.Abs(currentScale-previousScale)>7f){
                if(currentScale-previousScale>0){
                    //放大
                    scaleSwitch=4;//检测一段时间后再放大
                }else{
                    //缩小
                    scaleSwitch=2;//检测一段时间后再缩小
                }
            }
            else if(scaleSwitch==0 && Math.Abs(currentScale-previousScale)>5){
                elapsedScaleTime = 0f;
            }else if( (scaleSwitch==2||scaleSwitch==4) && Math.Abs(currentScale-previousScale)>5){
                scaleSwitch=0;
                elapsedScaleTime = 0f;
            }else{
                if(scaleSwitch!=1){
                    elapsedScaleTime += Time.deltaTime;
                    if(elapsedScaleTime > scaleThreshold){
                        scaleSwitch++;
                        elapsedScaleTime = 0f;
                    }
                }
            }
            if(scaleSwitch==2){
                //放大
                //获取原来的scale
                float oldScale = scaleTransform.localScale.x;
                scaleTransform.localScale = new Vector3(oldScale+0.1f, oldScale+0.1f, oldScale+0.1f);
                scaleSwitch=0;
            }else if(scaleSwitch==4){
                //缩小
                //获取原来的scale
                float oldScale = scaleTransform.localScale.x;
                scaleTransform.localScale = new Vector3(oldScale-0.1f, oldScale-0.1f, oldScale-0.1f);
                scaleSwitch=0;
            }
            previousScale = currentScale;
        }

        //updateLandmarkPosition 更新手部关键点位置
        if(LinkType == "Left" && pointsLeft != null)
        {
            for (int i = 1; i<handPoints.Count; i++)
            {

            float x = float.Parse(pointsLeft[i*4])-float.Parse(pointsLeft[0]);
            float y = float.Parse(pointsLeft[i*4+1])-float.Parse(pointsLeft[1]);
            float z = float.Parse(pointsLeft[i*4+2])-float.Parse(pointsLeft[2]);
            // print("LX"+x);

            LzD = float.Parse(pointsLeft[i*4+3]);

            if (x == 0 && y == 0 && z == 0)
                return;

            handPoints[i].transform.localPosition = (new Vector3(x,y,z)) * 1;         

            }
        }
        
        if(LinkType == "Right" && pointsRight != null)
        {
            for (int i = 1; i<handPoints.Count; i++)
            {

            float x = float.Parse(pointsRight[i*4])-float.Parse(pointsRight[0]);
            float y = float.Parse(pointsRight[i*4+1])-float.Parse(pointsRight[1]);
            float z = float.Parse(pointsRight[i*4+2])-float.Parse(pointsRight[2]);
            // print("RX"+x);

            RzD = float.Parse(pointsRight[i*4+3]);

            if (x == 0 && y == 0 && z == 0)
                return;

            handPoints[i].transform.localPosition = (new Vector3(x,y,z)) * 1;         

            }
        }

        
        if(LinkType == "Right" && pointsRight != null)
        {
            float RHx=0.557769716f;
            float RHy=0.728625596f;
            float RHz=0.126790136f;
            float depth = depthCalibrator.GetDepthFromThumbLength(scale);
            if (flagR == 0)
            {
                Rx0 = float.Parse(pointsRight[0]);
                Ry0 = float.Parse(pointsRight[1]);
                Rz0 =RzD;
                flagR=1;
            }
            this.transform.localPosition = new Vector3((float.Parse(pointsRight[1])-Ry0)/1000+RHx,(-float.Parse(pointsRight[0])+Rx0)/1000+RHy, RHz+(RzD-Rz0)/200);
            //print(RHz+(zD-z0)/100);
        
        

        //updateLandmarkScale 更新手部关键点大小
        var pointA = new Vector3(float.Parse(pointsRight[0]), float.Parse(pointsRight[1]), float.Parse(pointsRight[2]));
        var pointB = new Vector3(float.Parse(pointsRight[4]), float.Parse(pointsRight[5]), float.Parse(pointsRight[6]));
        var thumbDetectedLength = Vector3.Distance(pointA, pointB);
        if (thumbDetectedLength == 0)
            return;
        scale = thumbModelLength / thumbDetectedLength;
        this.transform.localScale = new Vector3(scale, scale, scale);
        }

        if(LinkType == "Left" && pointsLeft != null)
        {
            float LHx=0.460089773f;
            float LHy=0.420398116f;
            float LHz=0.129199326f;
            float depth = depthCalibrator.GetDepthFromThumbLength(scale);
            if (flagL == 0)
            {
                Lx0 = float.Parse(pointsLeft[0]);
                Ly0 = float.Parse(pointsLeft[1]);
                Lz0 =LzD;
                flagL=1;
            }
            this.transform.localPosition = new Vector3((float.Parse(pointsLeft[1])-Ly0)/1000+LHx,(-float.Parse(pointsLeft[0])+Lx0)/1000+LHy, LHz+(LzD-Lz0)/200);
            //print(RHz+(zD-z0)/100);
        
        

        //updateLandmarkScale
        var pointA = new Vector3(float.Parse(pointsLeft[0]), float.Parse(pointsLeft[1]), float.Parse(pointsLeft[2]));
        var pointB = new Vector3(float.Parse(pointsLeft[4]), float.Parse(pointsLeft[5]), float.Parse(pointsLeft[6]));
        var thumbDetectedLength = Vector3.Distance(pointA, pointB);
        if (thumbDetectedLength == 0)
            return;
        scale = thumbModelLength / thumbDetectedLength;
        this.transform.localScale = new Vector3(scale, scale, scale);
        }


        updateWristRotation();
        foreach (var linker in transformLinkers)
        {
            linker.UpdateTransform();
        }

    }

    private void updateWristRotation()
    {
        var wristTransform = handPoints[0].transform;
        var indexFinger = handPoints[5].transform.position;
        var middleFinger = handPoints[9].transform.position;

        var vectorToMiddle = middleFinger - wristTransform.position;
        var vectorToIndex = indexFinger - wristTransform.position;

        Vector3.OrthoNormalize(ref vectorToMiddle, ref vectorToIndex);

        Vector3 normalVector = Vector3.Cross(vectorToIndex, vectorToMiddle);

        wristTransform.rotation = Quaternion.LookRotation(normalVector, vectorToIndex);

    }
}
}