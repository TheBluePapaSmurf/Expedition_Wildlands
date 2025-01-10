using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image normalImage;
    public float fadeDuration = 0.5f;
    public Vector3 hoverOffset = new Vector3(0, 10, 0); // Adjust as needed

    private Coroutine fadeCoroutine;
    private Coroutine moveCoroutine;
    private Vector3 originalPosition;

    void Start()
    {
        normalImage.canvasRenderer.SetAlpha(1f);

        // Store the original position
        originalPosition = GetComponent<RectTransform>().anchoredPosition;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        StartFade(normalImage, 0f);
        StartMove(originalPosition + hoverOffset);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StartFade(normalImage, 1f);
        StartMove(originalPosition);
    }

    private void StartFade(Image image, float targetAlpha)
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
        fadeCoroutine = StartCoroutine(FadeImage(image, targetAlpha));
    }

    private IEnumerator FadeImage(Image image, float targetAlpha)
    {
        float startAlpha = image.canvasRenderer.GetAlpha();
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            image.canvasRenderer.SetAlpha(Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration));
            yield return null;
        }

        image.canvasRenderer.SetAlpha(targetAlpha);
    }

    private void StartMove(Vector3 targetPosition)
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }
        moveCoroutine = StartCoroutine(MoveButton(targetPosition));
    }

    private IEnumerator MoveButton(Vector3 targetPosition)
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        Vector3 startPosition = rectTransform.anchoredPosition;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            rectTransform.anchoredPosition = Vector3.Lerp(startPosition, targetPosition, elapsedTime / fadeDuration);
            yield return null;
        }

        rectTransform.anchoredPosition = targetPosition;
    }
}
