using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSetting : MonoBehaviour
{
    public Camera _topCamera;
    public Camera _bottomCamera;
    public Camera _leftCamera;
    public Camera _rightCamera;
    public int _width; //屏幕宽度
    public int _high; //屏幕高度
    public delegate void CameraSetDelegate(Camera TargetCamera);
    public event CameraSetDelegate _cameraSetTopCamera;
    public event CameraSetDelegate _cameraSetBottomCamera;
    public event CameraSetDelegate _cameraSetLeftCamera;
    public event CameraSetDelegate _cameraSetRightCamera;
    Rect _cameraRect = new Rect();
    private void Awake()
    {
            _cameraSetBottomCamera += SettingCameraBottom;
        _cameraSetLeftCamera += SettingCameraLeft;
        _cameraSetRightCamera += SettingCameraRight;
        _cameraSetTopCamera += SettingCameraTop;
    }
    Rect OutRect(float width, float high, string cameraName)
    {
        _cameraRect.width = (float)Math.Round(((float)1 / 3 * high / width), 2);
        _cameraRect.height = (float)Math.Round((float)1 / 3, 2);
        switch (cameraName)
        {
            case "TopCamera":
                _cameraRect.x = (float)Math.Round(0.5 - _cameraRect.width / 2, 2);
                _cameraRect.y = (float)Math.Round((float)2 / (float)3, 2);
                Debug.Log("Top" + _cameraRect);
                break;
            case "BottomCamera":
                _cameraRect.x = (float)Math.Round(0.5 - _cameraRect.width / 2, 2);
                _cameraRect.y = 0;
                Debug.Log("Bottom" + _cameraRect);
                break;
            case "LeftCamera":
                _cameraRect.x = (float)Math.Round(0.5 + _cameraRect.width / 2, 2);
                _cameraRect.y = (float)Math.Round((float)1 / 3, 2);
                Debug.Log("Left" + _cameraRect);
                break;
            case "RightCamera":
                _cameraRect.x = (float)Math.Round(0.5 - 3 * _cameraRect.width / 2, 2);
                _cameraRect.y = (float)Math.Round((float)1 / 3, 2);
                Debug.Log("Right" + _cameraRect);
                break;
        }

        return _cameraRect;
    }
    private void SettingCameraBottom(Camera TargetCamera)
    {
        TargetCamera.rect = OutRect(_width, _high, "BottomCamera");
    }
    private void SettingCameraTop(Camera TargetCamera)
    {
        TargetCamera.rect = OutRect(_width, _high, "TopCamera");
    }
    private void SettingCameraLeft(Camera TargetCamera)
    {
        TargetCamera.rect = OutRect(_width, _high, "LeftCamera");
    }
    private void SettingCameraRight(Camera TargetCamera)
    {
        TargetCamera.rect = OutRect(_width, _high, "RightCamera");
    }
    private void Start()
    {

        try
        {
            _cameraSetBottomCamera(_bottomCamera);
            _cameraSetLeftCamera(_leftCamera);
            _cameraSetRightCamera(_rightCamera);
            _cameraSetTopCamera(_topCamera);
        }
        catch (Exception e)
        {

            Debug.Log(e.Message);
        }
    }
}