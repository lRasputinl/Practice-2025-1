using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoomEnemy : Enemy
{

    [SerializeField] float attackRadius;
    [SerializeField] LayerMask whatIsPlayer;

    [SerializeField] int damage;

    [SerializeField] GameObject boomEffect;

    public override void Update()
    {
        base.Update();

        if (CheckIfCanAttack())
        {
            BoomAttack();
        }
    }

    void BoomAttack()
    {
        SoundManager.instance.PlayerSound(attackClip);

        Collider2D[] detectedObject = Physics2D.OverlapCircleAll(transform.position, attackRadius, whatIsPlayer);

        foreach (Collider2D item in detectedObject)
        {
            item?.GetComponent<Player>()?.Damage(damage);
        }

        Instantiate(boomEffect, transform.position, Quaternion.identity);

        Death();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}
