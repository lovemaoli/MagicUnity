using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
namespace landmarktest
{
public class CameraController : MonoBehaviour
{
    
    private Vector3 leftPosition = new Vector3(0, 0, -200); // 这是摄像机向左移动的目标位置
    private Vector3 rightPosition = new Vector3(0, 0, 200); // 这是摄像机向右移动的目标位置
    public static bool beforeleftStage = false;
    private bool gobeforeleftStage = false;
    public static bool beforerightStage = false;
    private bool gobeforerightStage = false;
    void Start()
    {
        if(beforeleftStage)
        {
            transform.position = leftPosition;
            print("beforeleftStage");
            beforeleftStage = false;
            gobeforeleftStage = true;
        }
        else if(beforerightStage)
        {
            transform.position = rightPosition;
            print("beforerightStage");
            beforerightStage = false;
            gobeforerightStage = true;
        }
    }
    void Update()
    {
        //输出各个变量的值
        print("beforeleftStage:" + beforeleftStage + " beforerightStage:" + beforerightStage + " gobeforeleftStage:" + gobeforeleftStage + " gobeforerightStage:" + gobeforerightStage);
        //进入
        if(gobeforeleftStage){
            //摄像机逐渐向左加速移动
            transform.position = transform.position * 0.9f;
            //移动一定距离后，加载下一个场景
            if (transform.position.z < 0.5f)
            {
                gobeforeleftStage = false;
                transform.position = new Vector3(0, 0, 0);
            }
        }else if(gobeforerightStage){
            //摄像机逐渐向右加速移动
            transform.position = transform.position * 0.9f;
            //移动一定距离后，加载下一个场景
            if (transform.position.z > -0.5f)
            {
                gobeforerightStage = false;
                transform.position = new Vector3(0, 0, 0);
            }
        }
        //退出
        if (HandTracking.leftStage)
        {
            //摄像机逐渐向右加速移动
            transform.position += new Vector3(0, 0, -4f);
            //移动一定距离后，加载下一个场景
            if (transform.position.z < -200)
            {
                HandTracking.leftStage = false;
                beforeleftStage = true;
                print(SceneManager.sceneCountInBuildSettings);
                if(SceneManager.GetActiveScene().buildIndex == 0){
                    SceneManager.LoadScene(SceneManager.sceneCountInBuildSettings-1);
                }else{
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
                }
            }
        }
        else if (HandTracking.rightStage)
        {
            //摄像机逐渐向左加速移动
            transform.position += new Vector3(0, 0, 4f);
            //移动一定距离后，加载下一个场景
            if (transform.position.z > 200)
            {
                HandTracking.rightStage = false;
                beforerightStage = true;
                print(SceneManager.sceneCountInBuildSettings);
                if(SceneManager.GetActiveScene().buildIndex == SceneManager.sceneCountInBuildSettings-1){
                    SceneManager.LoadScene(0);
                }else{
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                }
            }
        }
    }

}
}