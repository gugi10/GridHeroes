using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ProjectileAnimation : MonoBehaviour
{
    [SerializeField] ParticleSystem projectilePrefab;
    [SerializeField] ParticleSystem explosionPrefab;
    [SerializeField] Transform root;

    private bool isLerping;
    private float duration = 1f;
    private float elapsedTime = 0f;
    private Vector3 target;
    private Action onHitCallback;
    private ParticleSystem spawnedProjectile;
    private object spawnedProjectiletransform;

    private void Awake()
    {
    }
    private void Update()
    {
        if (!isLerping)
        {
            return;
        }

        if (elapsedTime < duration)
        {
            spawnedProjectile.transform.position = Vector3.Lerp(root.position, target, elapsedTime);
            elapsedTime += Time.deltaTime;
            return;
        }

        if (spawnedProjectile != null)
            spawnedProjectile.Stop();

        elapsedTime = 0;
        Instantiate(explosionPrefab).transform.position = target;
        isLerping = false;
        onHitCallback?.Invoke();
    }

    public void PlayProjectile(Vector3 target, float delay, Action onHitCallback)
    {
        this.target = target;
        this.onHitCallback = onHitCallback;
        
        StartCoroutine(DelayPlay(delay));
    }


    private IEnumerator DelayPlay(float delay)
    {
        yield return new WaitForSeconds(delay);

        spawnedProjectile = Instantiate(projectilePrefab, transform);
        spawnedProjectile.transform.position = root.position;
        spawnedProjectile.transform.LookAt(target);

        isLerping = true;
    }

}
