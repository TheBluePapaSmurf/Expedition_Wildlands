using BDE.Expedition.PlayerControls;
using UnityEngine;

public class JariAnimatorHelper : MonoBehaviour
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
    public void InvokeThrowBoomerang()
    {
        if (attackHandler != null)
        {
            attackHandler.ThrowBoomerang();
        }
    }
}
