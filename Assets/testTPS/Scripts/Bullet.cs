using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float timeToDestory;
    [HideInInspector] public WeaponManager weapon;
    [HideInInspector] public Vector3 dir;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(this.gameObject, timeToDestory);    
    }
    

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponentInParent<EnemyHealth>())
        {
            EnemyHealth enemyHealth = collision.gameObject.GetComponentInParent<EnemyHealth>();
            enemyHealth.TakeDamage(weapon.damage); 

            if(enemyHealth.health <= 0 && enemyHealth.isDead == false) // added 'isDead = false' for disable force after enemy death
            {
                Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
                rb.AddForce(dir * weapon.enemyKickBackForce, ForceMode.Impulse); 
                enemyHealth.isDead = true;
            }
        }
        Destroy(this.gameObject);
    }
}
