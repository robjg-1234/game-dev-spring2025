using UnityEngine;

public class ButtonScript : MonoBehaviour
{
    [SerializeField] int myVal;
    private void OnMouseDown()
    {
        SafeScript.instance.AddToCode(myVal);
    }
}
