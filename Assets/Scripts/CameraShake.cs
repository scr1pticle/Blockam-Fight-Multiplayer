using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CameraShake : MonoBehaviour
{
    public static  CameraShake inst;
    
    public float shakeDuration = 0.5f; // duration of the camera shake
    public float shakeMagnitude = 0.1f; // magnitude of the camera shake
    public AnimationCurve shakeCurve; // curve for the camera shake effect
    public bool isShaking = false; // flag to determine if the camera is currently shaking

    private Vector3 originalPosition; // the original position of the camera before shaking

    private void Awake()
    {
        if (inst == null)
        {
            inst = this;
        }
    }

    // Method to start the camera shake effect
    public void Shake()
    {
        if (!isShaking)
        {
            originalPosition = transform.localPosition;
            StartCoroutine(ShakeCoroutine());
        }
    }

    // Coroutine that performs the camera shake effect
    private IEnumerator ShakeCoroutine()
    {
        isShaking = true;

        float elapsedTime = 0f;
        while (elapsedTime < shakeDuration)
        {
            float magnitude = shakeMagnitude * shakeCurve.Evaluate(elapsedTime / shakeDuration);

            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = originalPosition + new Vector3(x, y, 0f);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = originalPosition;

        isShaking = false;
    }
}
