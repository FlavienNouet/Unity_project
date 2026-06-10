using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.AI;
 
public class MonsterController : MonoBehaviour
{
    public GameObject player;
 
    public MeleeWeapon meleeWeapon;
 
    //Agent de Navigation
    NavMeshAgent navMeshAgent;
 
    //Composants
    Animator animator;
 
    //Actions possibles
    const string STAND_STATE = "Stand";
    const string TAKE_DAMAGE_STATE = "Damage";
    public const string DEFEATED_STATE = "Defeated";
    public const string WALK_STATE = "Walk";
    public const string ATTACK_STATE = "Attack";
 
    //Mémorise l'action actuelle
    public string currentAction;
 
    private void Awake()
    {
        //Au départ, la créature attend en restant debout
        currentAction = STAND_STATE;
 
        //Référence vers l'Animator
        animator = GetComponent<Animator>();
 
        //Référence NavMeshAgent
        navMeshAgent = GetComponent<NavMeshAgent>();
 
        //Référence de Player
        player = FindObjectOfType<PlayerFPS>().gameObject;
    }

    // AJOUT CONCRET : Se déclenche automatiquement quand le WaveSpawner réactive le zombie via le Pool
    private void OnEnable()
    {
        // On remet le monstre dans son état initial pour la nouvelle vague
        currentAction = STAND_STATE;
        
        // On s'assure que le NavMeshAgent est bien actif et prêt à se déplacer
        if (navMeshAgent != null)
        {
            navMeshAgent.enabled = true;
        }
    }
 
    private void Update()
    {
        //si la créature est défaite
        //Elle ne peut rien faire d'autres
        if (currentAction == DEFEATED_STATE)
        {
            navMeshAgent.ResetPath();
            return;
        }
 
        //Si la créature reçoit des dommages:
        //Elle ne peut rien faire d'autres.
        if (currentAction == TAKE_DAMAGE_STATE)
        {
            navMeshAgent.ResetPath();
            TakingDamage();
            return;
        }
 
        if (player != null)
        {
            //Est-ce que l'IA se déplace vers le joueur ?
            if (MovingToTarget())
            {
                //En train de marcher
                return;
            }
            else
            {
                if (currentAction != ATTACK_STATE && currentAction != TAKE_DAMAGE_STATE)
                {
                    Attack();
                    return;
                }
                if (currentAction == ATTACK_STATE)
                {
                    // Appelle la méthode corrigée ci-dessous
                    Attacking();
                    return;
                }
 
                //Defaut
                Stand();
                return;
            }
        }
    }
 
    //La créature attend
    private void Stand()
    {
        ResetAnimation();
        currentAction = STAND_STATE;
        animator.SetBool("Stand", true);
    }
 
    public void TakeDamage()
    {
        ResetAnimation();
        currentAction = TAKE_DAMAGE_STATE;
        animator.SetBool("Damage", true);
    }
 
    public void Defeated()
{
    ResetAnimation();
    currentAction = DEFEATED_STATE;
    animator.SetBool(DEFEATED_STATE, true);
    
    if (navMeshAgent != null)
    {
        navMeshAgent.enabled = false;
    }
    // ⛔ Ne jamais mettre SetActive(false) ici !
}
 
    //Permet de surveiller l'animation lorsque l'on prend un dégât
    private void TakingDamage()
    {
        if (this.animator.GetCurrentAnimatorStateInfo(0).IsName(TAKE_DAMAGE_STATE))
        {
            float normalizedTime = this.animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
 
            //Fin de l'animation
            if (normalizedTime > 1)
            {
                Stand();
            }
        }
    }
 
    private void Attacking()
    {
        if (this.animator.GetCurrentAnimatorStateInfo(0).IsName(ATTACK_STATE))
        {
            // MODIFICATION CONCRÈTE : On retire le "% 1" pour que le temps dépasse réellement 1 à la fin
            float normalizedTime = this.animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
 
            //Fin de l'animation (normalizedTime > 1 signifie que l'animation est terminée à 100%)
            if (normalizedTime > 1f)
            {
                meleeWeapon.StopAttack();
                Stand();
                return;
            }
 
            meleeWeapon.StartAttack();
        }
    }
 
    private bool MovingToTarget()
    {
        // Si le NavMeshAgent est désactivé (pendant la mort), on empêche le calcul
        if (!navMeshAgent.enabled) return false;

        //Assigne la destination : le joueur
        navMeshAgent.SetDestination(player.transform.position);
 
        //Si navMeshAgent n'est pas prêt
        if (navMeshAgent.remainingDistance == 0)
            return true;
 
        if (navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance)
        {
            if (currentAction != WALK_STATE)
                Walk();
        }
        else
        {
            //Si arrivé à bonne distance, regarde vers le joueur
            RotateToTarget(player.transform);
            return false;
        }
 
        return true;
    }
 
    //Walk = Marcher
    private void Walk()
    {
        ResetAnimation();
        currentAction = WALK_STATE;
        animator.SetBool(WALK_STATE, true);
    }
 
    private void Attack()
    {
        ResetAnimation();
        currentAction = ATTACK_STATE;
        animator.SetBool(ATTACK_STATE, true);
    }
 
    //Permet de tout le temps regarder en direction de la cible
    private void RotateToTarget(Transform target)
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 3f);
    }
 
    //Réinitialise les paramètres de l'animator
    private void ResetAnimation()
    {
        animator.SetBool(STAND_STATE, false);
        animator.SetBool(TAKE_DAMAGE_STATE, false);
        animator.SetBool(DEFEATED_STATE, false);
        animator.SetBool(WALK_STATE, false);
        animator.SetBool(ATTACK_STATE, false);
    }
}