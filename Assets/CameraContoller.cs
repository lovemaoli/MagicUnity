using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class CameraController : MonoBehaviour
{
    public float transitionSpeed = 1f; // 你可以根据需要调整这个值
    private Vector3 leftPosition = new Vector3(-10, 0, -10); // 这是摄像机向左移动的目标位置
    private Vector3 rightPosition = new Vector3(10, 0, -10); // 这是摄像机向右移动的目标位置

    void Update()
    {
        if (HandTracking.leftStage)
        {
            StartCoroutine(MoveCamera(leftPosition, "PreviousScene")); // 你需要将"PreviousScene"替换为你的上一个场景的名字
            StageController.leftStage = false;
        }
        else if (HandTracking.rightStage)
        {
            StartCoroutine(MoveCamera(rightPosition, "NextScene")); // 你需要将"NextScene"替换为你的下一个场景的名字
            StageController.rightStage = false;
        }
    }

    IEnumerator MoveCamera(Vector3 targetPosition, string sceneName)
    {
        while ((transform.position - targetPosition).magnitude > 0.1f)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, transitionSpeed * Time.deltaTime);
            yield return null;
        }

        SceneManager.LoadScene(sceneName);
    }
}
