using UnityEngine;

[RequireComponent(typeof(ReceiveDamage))]
public class PoolableEnemy : MonoBehaviour
{
    ReceiveDamage receiveDamage;

    void Awake()
    {
        receiveDamage = GetComponent<ReceiveDamage>();
    }

    void OnEnable()
    {
        if (receiveDamage != null)
        {
            receiveDamage.hitPoint = receiveDamage.maxHitPoint;
            receiveDamage.isInvulnerable = false;
        }
    }

    // Called via SendMessage from ReceiveDamage (when defeated)
    public void Defeated()
    {
        gameObject.SetActive(false);
    }
}
