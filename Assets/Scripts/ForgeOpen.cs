using UnityEngine;

public class ForgeOpen : MonoBehaviour
{
    public Animator animator1;
    public Animator animator2;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            animator1.SetBool("IsOpen", true);
            animator2.SetBool("IsOpen", true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            animator1.SetBool("IsOpen", false);
            animator2.SetBool("IsOpen", false);
        }
    }
}
