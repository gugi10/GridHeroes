using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ProjectileAnimation : MonoBehaviour
{
    [SerializeField] ParticleSystem projectileSystem;
    [SerializeField] Transform root;
    private bool isLerping;
    private float duration = 1f;
    private float elapsedTime = 0f;
    private Vector3 target;
    private Action onHitCallback;

    private void Update()
    {
        if (!isLerping)
        {
            return;
        }

        if (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(root.position, target, elapsedTime);
            elapsedTime += Time.deltaTime;
            return;
        }
        onHitCallback?.Invoke();
        elapsedTime = 0;
        isLerping = false;
    }

    public void PlayProjectile(Vector3 target, float delay, Action onHitCallback)
    {
        this.target = target;
        this.onHitCallback = onHitCallback;
        transform.LookAt(target);
        StartCoroutine(DelayPlay(delay));
    }


    private IEnumerator DelayPlay(float delay)
    {
        yield return new WaitForSeconds(delay);
        isLerping = true;
    }

}
