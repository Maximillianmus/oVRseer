using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class NPC : MonoBehaviour
{
    [Tooltip("The locations the NPC will go to")]
    [SerializeField] Transform[] locations;
    public Transform targetLocation;
    private int numlocations;

    [SerializeField] NavMeshAgent agent;
    [SerializeField] RigidbodyControllerNPC npcController;

    [SerializeField] float minDecisionTime = 0.3f;
    [SerializeField] float maxDecisionTime = 2f;

    [SerializeField] float minWaitTime = 2f;
    [SerializeField] float maxWaitTime = 7f;

    [SerializeField] bool wait = false;


    void Start()
    {
        numlocations = locations.Length;
        targetLocation = locations[Random.Range(0, numlocations - 1)];
        agent.SetDestination(targetLocation.position);

        agent.updatePosition = false;
        agent.updateRotation = false;

        // Handles the AI decision making
        StartCoroutine(MakeDecision(Random.Range(minDecisionTime, maxDecisionTime)));
    }

    private void Update()
    {
        agent.nextPosition = transform.position;
       
        Vector3 newDir = Vector3.RotateTowards(transform.forward, agent.desiredVelocity, Mathf.Deg2Rad * 1, 0.0f);

        npcController.MoveNPC(newDir, wait);
    }

    IEnumerator StopAndWait(float timeToWait)
    {
        wait = true;
        yield return new WaitForSeconds(timeToWait);
        wait = false;
        StartCoroutine(MakeDecision(0));
    }


    IEnumerator MakeDecision(float timeBetweenDecision)
    {
        yield return new WaitForSeconds(timeBetweenDecision);

        float decision = Random.value; // Value between 0 - 1

        // 10% chance of getting a new destination
        // If NPC has reached destination also assign new destination
        if (decision < 0.1 || agent.remainingDistance < 3f)
        {
            targetLocation = locations[Random.Range(0, numlocations - 1)];
            agent.SetDestination(targetLocation.position);
        }

        // 10% chance NPC waits for a bit before calling MakeDecision again
        if(decision > 0.95)
        {
            StartCoroutine(StopAndWait(Random.Range(minWaitTime, maxWaitTime)));
        }
        else
        {
            StartCoroutine(MakeDecision(Random.Range(minDecisionTime, maxDecisionTime)));
        }
    }

}
