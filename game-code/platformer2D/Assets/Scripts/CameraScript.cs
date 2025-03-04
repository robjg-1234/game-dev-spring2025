using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [SerializeField] GameObject roomCamera;
    [SerializeField] GameObject tutorial;
    public void toggleCamera()
    {
        if (!roomCamera.activeSelf)
        {
            roomCamera.SetActive(true);
        }
        else
        {
            if (tutorial != null)
            {
                Destroy(tutorial);
            }
            roomCamera.SetActive(false);
        }
    }
}
