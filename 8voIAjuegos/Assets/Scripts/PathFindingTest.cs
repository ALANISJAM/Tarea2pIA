using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFindingTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //ClassGrid myTest = new ClassGrid(5, 5);
        //myTest.DepthFirstSearch(0, 0, 4, 4);

        //ClassGrid myTest = new ClassGrid(5, 5);
        //myTest.BreadthFirstSearch(0,0,4,4);

        //ClassGrid myTest = new ClassGrid(5, 5);
        //myTest.BestFirstSearch(0, 0, 4, 4);

        ClassGridBFS myTest = new ClassGridBFS(5, 5);
        myTest.BreadthFirstSearch(0, 0, 2, 2);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
