using System.Collections;
using UnityEngine;

public class MegaPlayer : MonoBehaviour
{
    [SerializeField] private float height = 0.08f;
    [SerializeField] private float duration = 0.35f;

    private Coroutine bounceCoroutine;

    public void Bounce()
    {
        if (bounceCoroutine != null)
            StopCoroutine(bounceCoroutine);

        bounceCoroutine = StartCoroutine(BounceRoutine());
    }

    private IEnumerator BounceRoutine()
    {
        Vector3 startPosition = transform.localPosition;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;

            float progress = time / duration;
            float y = Mathf.Sin(progress * Mathf.PI * 2f) * height;

            transform.localPosition =
                startPosition + Vector3.up * y;

            yield return null;
        }

        transform.localPosition = startPosition;
        bounceCoroutine = null;
    }
}
