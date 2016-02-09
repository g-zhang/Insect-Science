using UnityEngine;
using System.Collections;

public class EnemyFrogAI : EnemyBaseBehavior {
    public bool __Frog_______________;
    public GameObject tongueAxis;
    public GameObject tongue;
    public float AttackRange = 5f;
    public float AttackSpeed = 1f;
    public float AttackCooldown = 1f;

    Vector3 initTonguePos;
    Vector3 initTongueScale;
    Quaternion initTongueRot;
    float AttackAnimationTime;
    float currAnimationTime = 0f;
    float attackAngle = -10f;
    float currCooldown = 0f;

    public override void BaseClassStart()
    {
        initTonguePos = tongue.transform.localPosition;
        initTongueScale = tongue.transform.localScale;
        initTongueRot = tongueAxis.transform.localRotation;

        AttackAnimationTime = (AttackRange / AttackSpeed) * 2f;
        currAnimationTime = AttackAnimationTime;
    }

    public override void BaseClassUpdate()
    {
        attackAnim();
    }

    public void attackAnim()
    {
        if(currAnimationTime < AttackAnimationTime)
        {
            tongue.transform.localScale = new Vector3(tongue.transform.localScale.x,
                                                     tongue.transform.localScale.y,
                                                     Mathf.PingPong(currAnimationTime * AttackSpeed, AttackRange));
            tongue.transform.localPosition = new Vector3(tongue.transform.localPosition.x, tongue.transform.localPosition.y, (tongue.transform.lossyScale.z / 2));
            //Quaternion rot = Quaternion.Euler(attackAngle, 0f, 0f);
            //tongueAxis.transform.localRotation = Quaternion.Slerp(tongueAxis.transform.localRotation, rot, .05f);
            if(currTarget != AttackTarget.none)
            {
                tongueAxis.transform.LookAt(currTargetPos);
            } else
            {
                tongueAxis.transform.localRotation = Quaternion.Slerp(tongueAxis.transform.localRotation, initTongueRot, .05f);
            }
            

            currAnimationTime += Time.deltaTime;
        } else
        {
            tongue.transform.localScale = initTongueScale;
            tongue.transform.localPosition = initTonguePos;
            tongueAxis.transform.localRotation = initTongueRot;
        }
    }


    public override void Attack()
    {
        if(currCooldown <= 0)
        {
            currAnimationTime = 0;
            currCooldown = AttackCooldown;
        } else
        {
            currCooldown -= Time.deltaTime;
        }
    }
}
