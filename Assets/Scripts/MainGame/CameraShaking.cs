using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShaking : MonoBehaviour
{
    private void OnEnable()
    {
        GameEvents.OnCameraShaking += ShakeCamera;
    }

    private void OnDisable()
    {
        GameEvents.OnCameraShaking -= ShakeCamera;
    }

    private void ShakeCamera(float duration, float magnitude)
    {
        Shake(duration, magnitude);
    }

    public IEnumerator Shake(float duration, float magnitude)
    {
        Vector3 originalPos = transform.localPosition;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector3(x, y, originalPos.z);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPos;
    }
}
