using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReceiveDamage : MonoBehaviour
{
    public int maxHitPoint = 5;
    public int hitPoint = 0;
    
    public bool isInvulnerable;
    public float invulnerabiltyTime;
    private float timeSinceLastHit = 0.0f;

    private void Start()
    {
        hitPoint = maxHitPoint;
        isInvulnerable = false;
    }

    // AJOUT : Cette fonction automatique d'Unity s'exécute quand le WaveSpawner fait un SetActive(true)
    private void OnEnable()
    {
        hitPoint = maxHitPoint; // Redonne toute sa vie au zombie !
        isInvulnerable = false;
        timeSinceLastHit = 0.0f;
    }

    private void Update()
    {
        if (isInvulnerable)
        {
            timeSinceLastHit += Time.deltaTime;
            if (timeSinceLastHit > invulnerabiltyTime)
            {
                timeSinceLastHit = 0.0f;
                isInvulnerable = false;
            }
        }
    }

    public void GetDamage(int damage)
{
    if (isInvulnerable || hitPoint <= 0) return;

    isInvulnerable = true;
    hitPoint -= damage;

    if (hitPoint > 0)
    {
        gameObject.SendMessage("TakeDamage", SendMessageOptions.DontRequireReceiver);
    }
    else
    {
        gameObject.SendMessage("Defeated", SendMessageOptions.DontRequireReceiver);

        // SÉCURITÉ : on vérifie que l'objet est bien actif avant de lancer la coroutine
        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(DisableAfterDelay(2.0f));
        }
        else
        {
            // L'objet a déjà été désactivé ailleurs, pas besoin de coroutine
            gameObject.SetActive(false);
        }
    }
}

    private IEnumerator DisableAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false); // Désactive l'objet pour que le WaveSpawner comprenne qu'il est mort !
    }
}