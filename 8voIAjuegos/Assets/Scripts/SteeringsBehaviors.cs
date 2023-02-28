using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringsBehaviors : MonoBehaviour
{

    //Vector2 currentPosition = Vector2.zero;
    //Vector2 currentVelocity = Vector2.zero;

    //Vector2 TargetPosition = Vector2.zero;

    public Rigidbody myRigidbody = null;
    public float fMaxSpeed = 1.0f;
    public float fMaxForce = 0.5f;
    // Cuantos fixeddeltatime en el futuro se usara para las funciones pursuit y evade
    public float fPredictionSteps = 10.0f;

    public float fArriveRadius = 3.0f;
    public bool bUseArrive = true;

    public enum SteeringBehavior { Seek, Flee, Pursue, Evade, Arrive, Wander}
    public SteeringBehavior currentBehavior = SteeringBehavior.Seek;

    GameObject PursuitTarget = null;
    Rigidbody PursuitTargetRB = null;

    Vector3 v3TargetPosition = Vector3.zero;
    Vector3 v3SteeringForceAux = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody>();
        if (myRigidbody == null)
        {
            Debug.LogError("No Rigidbody component found for this agent´s");
            return;
        }

    }

    private void OnValidate()
    {
        if (currentBehavior == SteeringBehavior.Pursue || currentBehavior == SteeringBehavior.Evade)
        {
            SetPursueTarget();  
        }
    }

    private float ArriveFunction(Vector3 in_v3DesiredDirection)
    {
       
        float fDistance = in_v3DesiredDirection.magnitude;
        float fDesiredMagnitude = fMaxSpeed;

        if (fDistance < fArriveRadius)
        {
            fDesiredMagnitude = Mathf.InverseLerp(0.0f, fArriveRadius, fDistance);

            //print("deaccelerating, inverse lerp is: " + fDesiredMagnitude);
        }
        return fDesiredMagnitude;
        
    }
    public Vector3 Seek(Vector3 in_v3TargetPosition)
    {
        // Dirección deseada es punta ("a dónde quiero llegar") - cola (dónde estoy ahorita)
        Vector3 v3DesiredDirection = in_v3TargetPosition - transform.position;
        float fDesiredMagnitude = fMaxSpeed;
        if (bUseArrive)
        {
            fDesiredMagnitude = ArriveFunction(v3DesiredDirection);
        }

        Vector3 v3DesiredVelocity = v3DesiredDirection.normalized * fDesiredMagnitude;

        Vector3 v3SteeringForce = v3DesiredVelocity - myRigidbody.velocity;
        // Igual aquí, haces este normalized*maxSpeed para que la magnitud de la
        // fuerza nunca sea mayor que la maxSpeed.
        v3SteeringForce = Vector3.ClampMagnitude(v3SteeringForce, fMaxForce);
        return v3SteeringForce;
    }

    public Vector3 Flee(Vector3 in_v3TargetPosition)
    {
        // Dirección deseada es punta ("a dónde quiero llegar") - cola (dónde estoy ahorita)
        Vector3 v3DesiredDirection = -1.0f *(in_v3TargetPosition - transform.position);
        Vector3 v3DesiredVelocity = v3DesiredDirection.normalized * fMaxSpeed;

        Vector3 v3SteeringForce = v3DesiredVelocity - myRigidbody.velocity;
        // Igual aquí, haces este normalized*maxSpeed para que la magnitud de la
        // fuerza nunca sea mayor que la maxSpeed.
        v3SteeringForce = Vector3.ClampMagnitude(v3SteeringForce, fMaxForce);
        return v3SteeringForce;
    }

    void SetPursueTarget()
    {
        Debug.Log("entre a setPursueTarget");

        PursuitTarget = GameObject.Find("PursuitTarget");
        if (PursuitTarget == null)
        {
            //entonces no encontro dicho objeto, es un error
            Debug.LogError("no PursuitTarget gameobject found in scene");
            return;
        }
        PursuitTargetRB = PursuitTarget.GetComponent<Rigidbody>();
        if (PursuitTargetRB == null)
        {
            Debug.LogError("no rigidbody present on GameObject PursuitTarget but it should");
            return ;
        }
    }

    Vector3 Pursuit(Rigidbody in_target)
    {
        //Es importante que hagamos una copia de la posicion del objetivo para no
        //modificarla directamente
        Vector3 v3TargetPosition = in_target.transform.position;
        //Añadimos a dicha posicion el movimiento a
        //fPredictionSteps - veces el deltaTime. Es decir, n-cuadros en el futuro
        v3TargetPosition += in_target.velocity * Time.fixedDeltaTime * fPredictionSteps;

        return Seek(v3TargetPosition);
    }

     Vector3 Evade(Rigidbody in_target)
     {
        Vector3 v3TargetPosition = in_target.transform.position;

        v3TargetPosition += in_target.velocity * Time.fixedDeltaTime * fPredictionSteps;

        return Flee(v3TargetPosition);
     }


    Vector3 Arrive(Vector3 in_v3TargetPosition)
    {
        Vector3 v3Diff = in_v3TargetPosition - transform.position;
        float fDistance = v3Diff.magnitude;
        float fDesiredMagnitude = fMaxSpeed;
        if (fDistance < fArriveRadius)
        {
            fDesiredMagnitude = Mathf.InverseLerp(0.0f, fArriveRadius, fDistance);

            print("deaccelerating, inverse lerp is: " + fDesiredMagnitude);
        }

        Vector3 v3DesiredVelocity = v3Diff.normalized * fDesiredMagnitude;

        Vector3 v3SteeringForce = v3DesiredVelocity - myRigidbody.velocity;
        // Igual aquí, haces este normalized*maxSpeed para que la magnitud de la
        // fuerza nunca sea mayor que la maxSpeed.
        v3SteeringForce = Vector3.ClampMagnitude(v3SteeringForce, fMaxForce);
        return v3SteeringForce;
    }

 
    // Update is called once per frame
    void Update()
    {
    
    }
    private void FixedUpdate()
    {
        Vector3 TargetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        TargetPosition.z = 0.0f;
        //Direccion deseada es punta ("a donde quiero llegar") - cola (donde estoy ahorita)
        Vector3 v3SteeringForce = Vector3.zero;

        switch (currentBehavior)
        {
            case SteeringBehavior.Seek:
                v3SteeringForce = Seek(TargetPosition);
                break;
            case SteeringBehavior.Flee:
                v3SteeringForce = Flee(TargetPosition);
                break;
            case SteeringBehavior.Pursue:
                v3SteeringForce = Pursuit(PursuitTargetRB);
                break;

            case SteeringBehavior.Evade:
                v3SteeringForce = Evade(PursuitTargetRB);
                break;
            
            case SteeringBehavior.Arrive:
                v3SteeringForce = Arrive(TargetPosition);

                v3TargetPosition = TargetPosition;
                break;

            
        }
        //currentVelocity += v3SteeringForce * Time.deltaTime;
        v3SteeringForceAux= v3SteeringForce;

        //Idealmente, usariamos el ForceMode de Force, para tomar en cuenta la masa del objeto
        //Aqui ya no usamos el deltaTome porque viene integrado en como funciona AddForce
        myRigidbody.AddForce(v3SteeringForce, ForceMode.Force);
        //Hacemos un clamp para que no exceda la velocidad maxima que puede tener el agente
        myRigidbody.velocity = Vector3.ClampMagnitude(myRigidbody.velocity, fMaxSpeed);

    }

    private void OnDrawGizmos()
    {
        if (currentBehavior == SteeringBehavior.Pursue ||
            currentBehavior == SteeringBehavior.Evade)
        {
            Gizmos.color = Color.yellow;
            Vector3 nextPosition = PursuitTargetRB.transform.position +
                PursuitTargetRB.velocity * Time.fixedDeltaTime * fPredictionSteps;

            Gizmos.DrawSphere(nextPosition, 0.25f);
        }

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + myRigidbody.velocity);

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + v3SteeringForceAux);

        if (currentBehavior == SteeringBehavior.Arrive)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(v3TargetPosition, fArriveRadius);
        }
    }
}
