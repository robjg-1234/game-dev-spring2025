using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [SerializeField] GameObject roomCamera;
    public void toggleCamera()
    {
        if (!roomCamera.activeSelf)
        {
            roomCamera.SetActive(true);
        }
        else
        {
            roomCamera.SetActive(false);
        }
    }
}
