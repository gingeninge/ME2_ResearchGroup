using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class PokeInteractorDebug : MonoBehaviour
{
    private XRBaseInteractor poke;

    void Start()
    {
        poke = GetComponent<XRBaseInteractor>();
    }

    void Update()
    {
        if (poke.hasHover)
        {
            Debug.Log("Poke Interactor is hovering over something!");
        }
    }
}