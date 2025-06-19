using UnityEngine;

public class ClosePopup : MonoBehaviour
{
    public GameObject Canvas;
   
    public void Close() 
    {
        Destroy(Canvas.gameObject);
    }
}
