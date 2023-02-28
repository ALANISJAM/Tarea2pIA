using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WanderP : MonoBehaviour
{
    UnityEngine.AI.NavMeshAgent agent;
    // Start is called before the first frame update
    void Start()
    {

        agent = this.GetComponent<UnityEngine.AI.NavMeshAgent>();
    }
    void Seek(Vector3 location)
    {
        agent.SetDestination(location);
    }
    Vector3 wanderTarget = Vector3.zero;
    void Wander()
    {

        float wanderRadius = 10;
        float wanderDistance = 20;
        float wanderJitter = 1;

        wanderTarget += new Vector3(Random.Range(-1.0f, 1.0f) * wanderJitter, 0, Random.Range(-1.0f, 1.0f) * wanderJitter);
        wanderTarget.Normalize();
        wanderTarget *= wanderRadius;

        Vector3 targetLocal = wanderTarget + new Vector3(0, 0, wanderDistance);
        Vector3 targetWorld = this.gameObject.transform.InverseTransformVector(targetLocal);
        Seek(targetWorld);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
