using UnityEngine;

public class LingapUIManagement : MonoBehaviour
{
    public Transform CanvasTransform;

    private void Awake(){
        GameObject canvasGo = GameObject.Find("Canvas");
        if(canvasGo != null)
        {
            CanvasTransform = canvasGo.transform;
        }
    }
}
