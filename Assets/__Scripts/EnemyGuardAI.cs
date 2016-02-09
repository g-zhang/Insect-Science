using UnityEngine;
using System.Collections;

public class EnemyGuardAI : EnemyBaseBehavior {
    public bool __Guard______________;


    public override void getVisionVals()
    {
        visionVector = body.transform.forward;
        visionPos = body.transform.position + new Vector3(0f, 1.35f, 0f);   
    }
}
