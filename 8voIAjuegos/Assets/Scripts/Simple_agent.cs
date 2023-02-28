using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Simple_agent : SteeringsBehaviors
{
    float fCurrentTime = 0.0f;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        fCurrentTime += Time.deltaTime;
        float xPos = Mathf.Sin(fCurrentTime);
        float yPos = Mathf.Cos(fCurrentTime);
        Vector3 targetPosition = new Vector3(xPos, yPos, 0.0f);

        Vector3 v3SteeringForce = Seek(targetPosition);

        myRigidbody.AddForce(v3SteeringForce, ForceMode.Force);

        myRigidbody.velocity = Vector3.ClampMagnitude(myRigidbody.velocity, fMaxSpeed);


    }
}

