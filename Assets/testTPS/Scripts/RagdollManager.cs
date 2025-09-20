using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollManager : MonoBehaviour
{
    Rigidbody[] rbs;

    // Start is called before the first frame update
    void Start()
    {
        rbs = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in rbs) rb.isKinematic = true;
        // kinematic 이 true 면 gravity 에 영향을 받지 않음
    }

    public void TriggerRagdoll()
    {
        foreach (Rigidbody rb in rbs) rb.isKinematic = false;

    }
}
