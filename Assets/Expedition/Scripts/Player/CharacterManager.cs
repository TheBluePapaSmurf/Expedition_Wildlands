using UnityEngine;
using System.Collections.Generic;
using Cinemachine;
using BDE.Expedition.PlayerControls;

public class CharacterManager : MonoBehaviour
{
    public static CharacterManager Instance { get; private set; }

    public CharacterType currentCharacter; // Huidig actief karakter
    public List<Animator> characterAnimators; // Lijst met animators voor alle karakters
    public List<GameObject> characterGameObjects; // Lijst met GameObjects voor alle karakters
    public List<SkinnedMeshRenderer> characterMeshRenderers; // Lijst met SkinnedMeshRenderers voor alle karakters
    public List<PlayerRagdoll> characterRagdolls; // Lijst met PlayerRagdoll componenten voor alle karakters

    public List<bool> characterUnlocked; // Lijst met unlocked status voor alle karakters

    public CinemachineFreeLook freeLookCamera; // Referentie naar de Cinemachine FreeLook camera

    // Movement eigenschappen voor elk karakter
    [System.Serializable]
    public class CharacterMovementProperties
    {
        public float originalSpeed;
        public float jumpSpeed;
        public float maxJumpHoldTime;
        public float gravity;
        public float rotationSpeed;
    }

    public List<CharacterMovementProperties> characterMovementProperties;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject); // Zorgt ervoor dat de manager niet wordt vernietigd bij het laden van een nieuwe scene
        }
    }

    private void Start()
    {
        ActivateCharacter(currentCharacter); // Activeer het standaard karakter bij het starten van de game
        UpdateCameraTargets(); // Update de camera targets bij de start
    }

    public Animator GetCurrentAnimator()
    {
        switch (currentCharacter)
        {
            case CharacterType.Jari:
                return characterAnimators[0];
            case CharacterType.Rico:
                return characterAnimators[1];
            // Voeg meer cases toe voor andere karakters
            default:
                return null;
        }
    }

    public SkinnedMeshRenderer GetCurrentMeshRenderer()
    {
        switch (currentCharacter)
        {
            case CharacterType.Jari:
                return characterMeshRenderers[0];
            case CharacterType.Rico:
                return characterMeshRenderers[1];
            // Voeg meer cases toe voor andere karakters
            default:
                return null;
        }
    }

    public PlayerRagdoll GetCurrentRagdoll()
    {
        switch (currentCharacter)
        {
            case CharacterType.Jari:
                return characterRagdolls[0];
            case CharacterType.Rico:
                return characterRagdolls[1];
            // Voeg meer cases toe voor andere karakters
            default:
                return null;
        }
    }

    public CharacterMovementProperties GetCurrentMovementProperties()
    {
        switch (currentCharacter)
        {
            case CharacterType.Jari:
                return characterMovementProperties[0];
            case CharacterType.Rico:
                return characterMovementProperties[1];
            // Voeg meer cases toe voor andere karakters
            default:
                return null;
        }
    }

    public void SetCurrentCharacter(CharacterType character)
    {
        if (IsCharacterUnlocked(character))
        {
            currentCharacter = character;
            ActivateCharacter(character);
            UpdatePlayerComponents();
            UpdateCameraTargets(); // Update de camera targets bij het wisselen van karakter
            UpdateThrowAbility();
        }
        else
        {
            Debug.Log("Character is not unlocked yet.");
        }
    }

    private void ActivateCharacter(CharacterType character)
    {
        for (int i = 0; i < characterGameObjects.Count; i++)
        {
            characterGameObjects[i].SetActive((CharacterType)i == character);
        }
    }

    private void UpdatePlayerComponents()
    {
        var playerMovement = FindObjectOfType<PlayerMovement>();
        if (playerMovement != null)
        {
            playerMovement.SetCurrentAnimator();
            playerMovement.UpdateMovementProperties();
        }

        var playerHealth = FindObjectOfType<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.SetCurrentMeshRenderer();
        }

        var playerRagdoll = FindObjectOfType<PlayerRagdoll>();
        if (playerRagdoll != null)
        {
            PlayerRagdoll.Instance = GetCurrentRagdoll();
            PlayerRagdoll.Instance.characterTransform = GetCurrentCharacterTransform(); // Voeg dit toe
        }
    }

    private void UpdateCameraTargets()
    {
        if (freeLookCamera != null)
        {
            Transform currentCharacterTransform = GetCurrentCharacterTransform();
            freeLookCamera.Follow = currentCharacterTransform;
            freeLookCamera.LookAt = currentCharacterTransform;
        }
    }

    private Transform GetCurrentCharacterTransform()
    {
        switch (currentCharacter)
        {
            case CharacterType.Jari:
                return characterGameObjects[0].transform;
            case CharacterType.Rico:
                return characterGameObjects[1].transform;
            // Voeg meer cases toe voor andere karakters
            default:
                return null;
        }
    }

    public bool IsCharacterUnlocked(CharacterType character)
    {
        return characterUnlocked[(int)character];
    }

    public void UnlockCharacter(CharacterType character)
    {
        characterUnlocked[(int)character] = true;
    }

    private void UpdateThrowAbility()
    {
        var attackHandler = FindObjectOfType<AttackHandler>();
        if (attackHandler != null)
        {
            attackHandler.canThrowWeapon = currentCharacter == CharacterType.Jari;
        }
    }
}
