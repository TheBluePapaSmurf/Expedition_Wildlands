using BDE.Expedition.PlayerControls;
using UnityEngine;

public class RicoAnimatorHelper : MonoBehaviour
{
    private AttackHandler attackHandler;

    void Start()
    {
        attackHandler = FindObjectOfType<AttackHandler>();
        if (attackHandler == null)
        {
            Debug.LogError("AttackHandler component is missing in the scene.");
        }
    }

    // Functie die wordt aangeroepen door het animatie-event
    public void InvokeThrowHelmet()
    {
        if (attackHandler != null)
        {
            attackHandler.ThrowHelmet();
        }
    }
}
