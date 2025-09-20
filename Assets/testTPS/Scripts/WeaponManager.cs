using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [Header("Fire Rate")]
    [SerializeField] float fireRate;
    float fireRateTimer;
    [SerializeField] bool semiAuto;

    [Header("Bullet Properties")]
    [SerializeField] GameObject bullet;
    [SerializeField] Transform barrelPos;
    [SerializeField] float bulletVelocity;
    [SerializeField] int bulletPerShot;
    public float damage = 20;
    AimStateManager aim;

    [SerializeField] AudioClip gunShot;
    [HideInInspector] public AudioSource audioSource;
    [HideInInspector] public WeaponAmmo ammo;

    WeaponBloom bloom;      //bloom

    // Disable shooting when reloading
    ActionStateManager actions;

    // camera recoil 
    WeaponRecoil recoil;

    Light muzzleFlashLight;
    ParticleSystem muzzleFlashParticles;
    float lightIntensity;
    [SerializeField] float lightReturnSpeed = 20;

    public float enemyKickBackForce = 100;

    // Weapon Changing
    public Transform leftHandTarget, lefthandHint;
    WeaponClassManager weaponClass;

    // Start is called before the first frame update
    void Start()
    {
        //recoil = GetComponent<WeaponRecoil>();
        //audioSource = GetComponent<AudioSource>();
        aim = GetComponentInParent<AimStateManager>();
        //ammo = GetComponent<WeaponAmmo>();
        bloom = GetComponent<WeaponBloom>();        //bloom

        actions = GetComponentInParent<ActionStateManager>();
        muzzleFlashLight = GetComponentInChildren<Light>();
        // muzzleFlashLight intensity will start at 0
        lightIntensity = muzzleFlashLight.intensity;
        muzzleFlashLight.intensity = 0;
        muzzleFlashParticles = GetComponentInChildren<ParticleSystem>();
        fireRateTimer = fireRate;
    }

    private void OnEnable()
    {
        if (weaponClass == null)
        {
            weaponClass = GetComponentInParent<WeaponClassManager>();
            ammo = GetComponent<WeaponAmmo>();
            audioSource = GetComponent<AudioSource>();
            recoil = GetComponent<WeaponRecoil>();
            recoil.recoilFollowPos = weaponClass.recoilFollowPos;
        }
        weaponClass.SetCurrentWeapon(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (ShouldFire())
        {
            Fire();
        }
        muzzleFlashLight.intensity = Mathf.Lerp(muzzleFlashLight.intensity, 0, lightReturnSpeed * Time.deltaTime);
        //Debug.Log(ammo.currentAmmo);
    }
        
    bool ShouldFire()
    {
        fireRateTimer += Time.deltaTime;
        if (fireRateTimer < fireRate)
        {
            return false;
        }
        if (ammo.currentAmmo == 0)
        {
            return false;
        }
        if(actions.currentState== actions.Reload) // disable shooting when reloading
        {
            return false;
        }
        if(actions.currentState == actions.Swap)
        {
            return false;
        }
        if (semiAuto && Input.GetKeyDown(KeyCode.Mouse0))
        {
            return true;
        }
        if (!semiAuto && Input.GetKey(KeyCode.Mouse0))
        {
            return true;
        }
        else
        {
            return false;
        }       

    }
    void Fire()
    {
        fireRateTimer = 0;
        ammo.currentAmmo--;

        barrelPos.LookAt(aim.aimPos);
        barrelPos.localEulerAngles = bloom.BloomAngle(barrelPos);   //bloom
        
        //audioSource.PlayOneShot(gunShot);
        recoil.TriggerRecoil();
        TriggerMuzzleFlash();
        for (int i = 0; i < bulletPerShot; i++)
        {
            GameObject currentBullet = Instantiate(bullet, barrelPos.position, barrelPos.rotation);

            Bullet bulletScript = currentBullet.GetComponent<Bullet>();
            bulletScript.weapon = this;

            //saves the direction of the bullet that has been shot in, enables to add force to the direction
            bulletScript.dir = barrelPos.transform.forward;

            Rigidbody rb = currentBullet.GetComponent<Rigidbody>();
            rb.AddForce(barrelPos.forward * bulletVelocity, ForceMode.Impulse);
        }
    }

    void TriggerMuzzleFlash()
    {
        muzzleFlashParticles.Play();
        muzzleFlashLight.intensity = lightIntensity;
    }
}
