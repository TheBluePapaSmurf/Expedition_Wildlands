using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectionUI : MonoBehaviour
{
    public Button jariButton;
    public Button ricoButton;
    // Voeg hier meer knoppen toe als je meer karakters hebt

    public ParticleSystem characterChangeParticle;

    private void Start()
    {
        // Voeg luisteraars toe aan de knoppen
        jariButton.onClick.AddListener(() => SelectCharacter(CharacterType.Jari));
        ricoButton.onClick.AddListener(() => SelectCharacter(CharacterType.Rico));
        // Voeg hier meer luisteraars toe voor andere knoppen

        // Update de UI om de juiste knoppen interactief te maken
        UpdateUI(CharacterManager.Instance.currentCharacter);
    }

    private void SelectCharacter(CharacterType characterType)
    {
        CharacterManager.Instance.SetCurrentCharacter(characterType);
        PlayCharacterChangeParticle();
        // Zorg ervoor dat de UI het actieve karakter reflecteert
        UpdateUI(characterType);
    }

    private void PlayCharacterChangeParticle()
    {
        if (characterChangeParticle != null)
        {
            characterChangeParticle.Play();
        }
    }

    private void UpdateUI(CharacterType characterType)
    {
        // Update de UI om het actieve karakter te reflecteren
        jariButton.interactable = CharacterManager.Instance.IsCharacterUnlocked(CharacterType.Jari) && characterType != CharacterType.Jari;
        ricoButton.interactable = CharacterManager.Instance.IsCharacterUnlocked(CharacterType.Rico) && characterType != CharacterType.Rico;
        // Voeg hier meer logica toe voor andere karakters
    }
}
