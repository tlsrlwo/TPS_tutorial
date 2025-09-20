using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class ActionStateManager : MonoBehaviour
{
    [HideInInspector] public ActionBaseState currentState;

    // set up states
    public ReloadState Reload = new ReloadState();
    public DefaultState Default = new DefaultState();
    public SwapState Swap = new SwapState();

    [HideInInspector] public WeaponManager currentWeapon;
    [HideInInspector] public WeaponAmmo ammo;
    //AudioSource audioSource; Ep8

    [HideInInspector] public Animator anim;

    public MultiAimConstraint rHandAim;
    public TwoBoneIKConstraint lHandIK;

    // Start is called before the first frame update
    void Start()
    {
        SwitchState(Default);
        //ammo = currentWeapon.GetComponent<WeaponAmmo>();   //weaponChanging part에서 이 부분 삭제
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        currentState.UpdateState(this);
    }

    public void SwitchState(ActionBaseState state)
    {
        currentState = state;
        currentState.EnterState(this);
    }
    public void WeaponReloaded()
    {
        ammo.Reload();
        SwitchState(Default);
    }

    public void SetWeapon(WeaponManager weapon)
    {
        currentWeapon = weapon;
        //audioSource = weapon.audiosource;
        ammo = weapon.ammo;
    }
}
