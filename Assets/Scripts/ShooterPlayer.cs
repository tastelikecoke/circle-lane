using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ShooterPlayer : MonoBehaviour
{
    public class Phase
    {
        public float duration = 0f;
        public float remainingTime = 0f;
        public System.Action onEnter;
        public System.Action onUpdate;
        public System.Action onExit;
        public int repeat = 0;

        public void Reset()
        {
            remainingTime = duration;
        }
    }

    public float moveSpeed = 5f; // per second
    public float attackCooldown = 1f;
    public float parryWarmup = 1f;
    public Projectile projectilePrefab;
    public Hitbox parryHitbox;
    public Hitbox damageHitbox;
    public GameObject parryVisuals;

    private float damageTime = 0f;
    private float currentParryWarmup = 0f;
    private float life = 100f;
    private float staminaMax = 100f;
    private float stamina = 100f;
    private bool invulnerable = false;
    public Vector3 direction;

    private List<Phase> phases;
    private Phase prepareAttack;
    private Phase attack;
    private Phase prepareShield;
    private Phase shield;
    private Phase dodge;
    private Phase cooldownDodge;
    private Phase damaged;

    void AddPhase(Phase phase)
    {
        phases.Add(phase);
        phase.Reset();
        phase.onEnter();
    }
    void Awake()
    {
        damageHitbox.onHitEnter += OnHitByDamage;
        phases = new List<Phase>();

        prepareAttack = new Phase();
        prepareAttack.duration = 0.3f;
        prepareAttack.onEnter = () => {
            GetComponent<SpriteRenderer>().color = Color.blue;
        };
        prepareAttack.onExit = () => {
            GetComponent<SpriteRenderer>().color = Color.white;
            attack.repeat = 2;
            AddPhase(attack);
        };
        attack = new Phase();
        attack.duration = 0.14f;
        attack.onEnter = () => {
            Projectile makeProjectile()
            {
                Projectile newBullet = Instantiate(projectilePrefab);
                newBullet.gameObject.SetActive(true);
                newBullet.flag = FightManager.instance.allyFlag;
                newBullet.transform.position = this.transform.position;
                return newBullet;
            };
            Projectile bullet1 = makeProjectile();

        };
        attack.onExit = () => {
        };
        
        prepareShield = new Phase();
        prepareShield.duration = 0.4f;
        prepareShield.onEnter = () => {
            GetComponent<SpriteRenderer>().color = Color.green;
            moveSpeed = 2f;
        };
        prepareShield.onExit = () => {
            moveSpeed = 5f;
            GetComponent<SpriteRenderer>().color = Color.white;
        };
        shield = new Phase();
        shield.duration = 0.3f;
        shield.onEnter = () => {
            invulnerable = true;
            parryVisuals.SetActive(true);
            moveSpeed = 0f;
        };
        shield.onUpdate = () => {
            for(int i=0; i<parryHitbox.collidingObjects.Count; i++)
            {
                Collider2D collider = parryHitbox.collidingObjects[i];
                if(collider == null)
                    continue;
                GameObject blockable = collider.gameObject;
                if(blockable.GetComponent<Projectile>() != null)
                {
                    blockable.GetComponent<Projectile>().Explode();
                }
                /*
                if(firstParryable.GetComponent<Projectile>() != null)
                {
                    firstParryable.GetComponent<Projectile>().direction = Vector3.right;
                    firstParryable.GetComponent<Projectile>().flag = FightManager.instance.allyFlag;
                }*/
            }
            this.transform.position += direction * 15f * Time.deltaTime;
        };
        shield.onExit = () => {
            invulnerable = false;
            parryVisuals.SetActive(false);
            moveSpeed = 5f;
            AddPhase(prepareShield);
        };

        
        dodge = new Phase();
        dodge.duration = 0.3f;
        dodge.onEnter = () => {
            moveSpeed = 7f;
            invulnerable = true;
            parryVisuals.SetActive(true);
        };
        dodge.onUpdate = () => {
        };
        dodge.onExit = () => {
            invulnerable = false;
            parryVisuals.SetActive(false);
            AddPhase(cooldownDodge);
        };
        cooldownDodge = new Phase();
        cooldownDodge.duration = 0.4f;
        cooldownDodge.onEnter = () => {
            GetComponent<SpriteRenderer>().color = Color.green;
            moveSpeed = 2f;
        };
        cooldownDodge.onExit = () => {
            moveSpeed = 5f;
            GetComponent<SpriteRenderer>().color = Color.white;
        };

        
        damaged = new Phase();
        damaged.duration = 0.33f;
        damaged.onEnter = () => {
            invulnerable = true;
            GetComponent<SpriteRenderer>().color = new Color(1,1,1, 1f);
        };
        damaged.onUpdate = () => {
            if(damaged.remainingTime < 0.167f)
            {
                GetComponent<SpriteRenderer>().color = new Color(1,1,1, 0.3f);
            }
        };
        damaged.onExit = () => {
            invulnerable = false;
            GetComponent<SpriteRenderer>().color = new Color(1,1,1, 1f);
        };
    }
    void OnDisable()
    {
        damageHitbox.onHitEnter -= OnHitByDamage;
    }
    void Update()
    {
        if(moveSpeed != 0f)
        {
            float movementX = (Input.GetKey(KeyCode.LeftArrow)? -1f : 0f) + (Input.GetKey(KeyCode.RightArrow)? 1f : 0f);
            float movementY = (Input.GetKey(KeyCode.DownArrow)? -1f : 0f) + (Input.GetKey(KeyCode.UpArrow)? 1f : 0f);
            Vector3 inputDirection = new Vector3(movementX, movementY, 0f).normalized * Mathf.Max(Mathf.Abs(movementX), Mathf.Abs(movementY));
            if(inputDirection != Vector3.zero)
                direction = inputDirection;
            float computedDistance = moveSpeed * Time.deltaTime;
            this.transform.position += inputDirection * computedDistance;
        }

        if(Input.GetKey(KeyCode.A))
        {
            if(!phases.Contains(attack) && !phases.Contains(prepareAttack))
                AddPhase(prepareAttack);
        }
        if(Input.GetKey(KeyCode.S))
        {
            if(!phases.Contains(shield) && !phases.Contains(prepareShield))
                AddPhase(shield);
        }
        if(Input.GetKey(KeyCode.D))
        {
            if(!phases.Contains(dodge) && !phases.Contains(cooldownDodge))
                AddPhase(dodge);
        }
        // if(damageTime > 0f)
        // {
        //     this.GetComponent<SpriteRenderer>().color = Color.red;
        //     damageTime -= Time.deltaTime;
        // }
        // else
        //     this.GetComponent<SpriteRenderer>().color = Color.white;

        
        for(int i = 0; i < phases.Count; i++)
        {
            Phase currentPhase = phases[i];
            if(currentPhase.remainingTime > 0)
            {
                if(currentPhase.onUpdate != null)
                {
                    currentPhase.onUpdate();
                }
                currentPhase.remainingTime -= Time.deltaTime;
            }
            else if(currentPhase.repeat > 0)
            {
                currentPhase.repeat -= 1;
                currentPhase.Reset();
                currentPhase.onEnter();
            }
            else
            {
                if(currentPhase.onExit != null)
                    currentPhase.onExit();
                phases.RemoveAt(i);
                i--;
            }
        }
    }

    public void OnHitByDamage(Collider2D collider)
    {
        if(invulnerable) return;
        if(collider.gameObject.GetComponent<Projectile>() != null)
        {
            if(collider.gameObject.GetComponent<Projectile>().flag == FightManager.instance.enemyFlag)
            {
                life -= 40f;
                collider.gameObject.GetComponent<Projectile>().Explode();
                GameObject explosionPrefab = FightManager.instance.explosionPrefab;
                GameObject explosionInstance = Instantiate(explosionPrefab);
                explosionInstance.transform.position = this.transform.position;
                damageTime = 0.3f;
                damaged.repeat = 2;
                AddPhase(damaged);
            }
        }
        if(collider.gameObject.GetComponent<LaserSegment>() != null)
        {
            if(collider.gameObject.GetComponent<LaserSegment>().flag == FightManager.instance.enemyFlag)
            {
                life -= 40f;
                // collider.gameObject.GetComponent<LaserSegment>().Explode();
                GameObject explosionPrefab = FightManager.instance.explosionPrefab;
                GameObject explosionInstance = Instantiate(explosionPrefab);
                explosionInstance.transform.position = this.transform.position;
                damageTime = 0.3f;
                damaged.repeat = 2;
                AddPhase(damaged);
            }
        }
    }

    public float GetLife()
    {
        return life;
    }
}
