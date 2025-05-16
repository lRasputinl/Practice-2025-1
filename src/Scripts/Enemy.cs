using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    Rigidbody2D rb;
    Animator anim;
    SpriteRenderer spR;

    [SerializeField] int health;
    [SerializeField] float stopDistance, distanceToRunOut, speed;

    protected Player player;
    bool isDeath = false;

    bool canAttack = false;

    [SerializeField] GameObject hitEffect;
    Vector3 addRandPosToGo;

    [SerializeField] ParticleSystem footParticle;

    [SerializeField] int minCoinsAdd, maxCoinsAdd;

    [SerializeField] AudioClip heartClip, deathClip;
    [SerializeField] protected AudioClip attackClip;
    public virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spR = GetComponent<SpriteRenderer>();

        player = Player.instance;

        StartCoroutine(nameof(SetRandomPos));
        EnemyOrderInLayerManager.instance.Add(spR);
    }

    private void OnDestroy()
    {
        EnemyOrderInLayerManager.instance.Dell(spR);
    }


    public virtual void Update()
    {
        if (isDeath || !player) return;

        Scale(player.transform.position);
    }

    private void FixedUpdate()
    {
        if (isDeath) return;

        if (player && Vector2.Distance(transform.position, player.transform.position) > stopDistance)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position + addRandPosToGo, speed * Time.fixedDeltaTime);
            anim.SetBool("run", true);

            footParticle.Pause();
            footParticle.Play();

            var emission = footParticle.emission;

            emission.rateOverTime = 10;

            canAttack = false;
        }
        else if(player && Vector2.Distance(transform.position, player.transform.position) < distanceToRunOut)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position + addRandPosToGo, -speed * Time.fixedDeltaTime);
            anim.SetBool("run", true);

            footParticle.Pause();
            footParticle.Play();

            var emission = footParticle.emission;

            emission.rateOverTime = 10;

            canAttack = false;
        }
        else
        {
            anim.SetBool("run", false);

            var emission = footParticle.emission;

            emission.rateOverTime = 0;

            canAttack = true;
        }
    }

     IEnumerator SetRandomPos()
     {
         addRandPosToGo = new Vector3(Random.Range(-stopDistance + 0.1f, stopDistance - 0.1f), Random.Range(-stopDistance + 0.1f, stopDistance - 0.1f));

         yield return new WaitForSeconds(1.5f);

         StartCoroutine(nameof(SetRandomPos));
     }

    void Scale(Vector3 pos)
    {
        if (pos.x >= transform.position.x) transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        else transform.rotation = Quaternion.Euler(0f, 180f, 0f);
    }

    public void Damage(int damage)
    {
        if (isDeath) return;

        health -= damage;

        Instantiate(hitEffect, transform.position, Quaternion.identity);
        SoundManager.instance.PlayerSound(heartClip);

        if (health <= 0) Death();
    }

    protected void Death()
    {
        isDeath = true;

        SoundManager.instance.PlayerSound(deathClip);

        Player.instance.AddMoney(Random.Range(minCoinsAdd, maxCoinsAdd));
        if (PlayerPrefs.GetInt("Position3") == 1) Player.instance.AddHealth(1);

        anim.SetTrigger("death");
    }

    public IEnumerator DestroyObj()
    {
        while (spR.color.a > 0)
        {
            float p = spR.color.a;
            spR.color = new Color(255f, 255f, 255f, p - 0.1f);
            yield return new WaitForSeconds(0.1f);
        }
        Destroy(gameObject);
    }

    public virtual bool CheckIfCanAttack()
    {
        return canAttack && !isDeath;
    }
}
