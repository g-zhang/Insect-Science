using UnityEngine;
using System.Collections;

public class EnemyGuardAI : EnemyBaseBehavior {
    public bool __Guard______________;
    public GameObject Weapon;
    public GameObject Barrel;
    public GameObject Projectile;
    public float WeaponCooldown = .5f;
    public float MuzzleVelocity = 10f;
    public float GunFloatHeight = .05f;
    public float GunFloatSpeed = 2.5f;

    float currCooldown = 0;

    Vector3 initGunPos;
    Quaternion initGunRotation;

    public override void getVisionVals()
    {
        visionVector = body.transform.forward;
        visionPos = body.transform.position + new Vector3(0f, 1.35f, 0f);   
    }

    public override void BaseClassStart()
    {
        initGunRotation = Weapon.transform.localRotation;
        initGunPos = Weapon.transform.localPosition;
    }

    public override void BaseClassUpdate()
    {
        GunAnimation();
    }

    public void GunAnimation()
    {
        //lock the gun at the target
        if (currState == EnemyState.attacking)
        {
            Weapon.transform.LookAt(currTargetPos);
            //Quaternion newRot = Quaternion.LookRotation(Weapon.transform.position - currTargetPos);
            //Weapon.transform.localRotation = Quaternion.Slerp(Weapon.transform.localRotation, newRot, .5f);
        }
        else
        {
            Weapon.transform.localRotation = Quaternion.Slerp(Weapon.transform.localRotation, initGunRotation, .1f);
        }

        //make the gun hover
        Vector3 tempPos = Weapon.transform.localPosition;
        tempPos.y = initGunPos.y + GunFloatHeight * Mathf.Sin(GunFloatSpeed * Time.time);
        Weapon.transform.localPosition = tempPos;
    }

    public void FireProjectile()
    {
        GameObject go = Instantiate(Projectile) as GameObject;
        go.transform.position = Barrel.transform.position;
        go.transform.rotation = Barrel.transform.rotation;
        go.GetComponent<Rigidbody>().velocity = Weapon.transform.forward * MuzzleVelocity;

        currCooldown += WeaponCooldown;
    }

    public override void Attack()
    {
        if (currCooldown <= 0)
        {
            FireProjectile();
        }
        else
        {
            currCooldown -= Time.deltaTime;
        }
    }
}
