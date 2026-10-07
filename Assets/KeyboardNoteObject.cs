using UnityEngine;
using System.Collections;

public class KeyboardNoteObject : MonoBehaviour
{
    [SerializeField] private float downAmount = 0.1f;
    [SerializeField] private float movementTime = 0.05f;

    [Header("Materiales")]
    [SerializeField] private Material normalMaterial;
    [SerializeField] private Material pressedMaterial;

    private Renderer objectRenderer;

    private Vector3 originalPosition;
    private Vector3 downPosition;

    private Coroutine movementCoroutine;

    private void Awake()
    {
        objectRenderer = GetComponent<Renderer>();

        originalPosition = transform.localPosition;
        downPosition = originalPosition + Vector3.down * downAmount;
    }

    public void Press()
    {
        objectRenderer.material = pressedMaterial;
        MoveTo(downPosition);
    }

    public void Release()
    {
        objectRenderer.material = normalMaterial;
        MoveTo(originalPosition);
    }

    private void MoveTo(Vector3 targetPosition)
    {
        if (movementCoroutine != null)
            StopCoroutine(movementCoroutine);

        movementCoroutine = StartCoroutine(Move(targetPosition));
    }

    private IEnumerator Move(Vector3 targetPosition)
    {
        Vector3 startPosition = transform.localPosition;

        float timer = 0f;

        while (timer < movementTime)
        {
            timer += Time.deltaTime;

            float t = timer / movementTime;

            transform.localPosition = Vector3.Lerp(
                startPosition,
                targetPosition,
                t
            );

            yield return null;
        }

        transform.localPosition = targetPosition;
        movementCoroutine = null;
    }
}
