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
    [SerializeField] Animator animator;

    [SerializeField] float minDecisionTime = 0.3f;
    [SerializeField] float maxDecisionTime = 2f;


    void Start()
    {
        numlocations = locations.Length;
        targetLocation = locations[Random.Range(0, numlocations - 1)];
        agent.SetDestination(targetLocation.position);

        // Handles the AI decision making
        StartCoroutine(MakeDecision(Random.Range(minDecisionTime, maxDecisionTime)));
    }

    void Update()
    {
        animator.SetFloat("Speed", agent.velocity.magnitude);
    }


    IEnumerator MakeDecision(float timeBetweenDecision)
    {
        yield return new WaitForSeconds(timeBetweenDecision);

        float decision = Random.value; // Value between 0 - 1

        // 20% chance of getting a new destination
        // If NPC has reached destination also assign new destination
        if (decision < 0.2 || Vector3.Distance(targetLocation.position, transform.position) < 0.5f)
        {
            targetLocation = locations[Random.Range(0, numlocations - 1)];
            agent.SetDestination(targetLocation.position);
        }

        StartCoroutine(MakeDecision(Random.Range(minDecisionTime, maxDecisionTime)));

    }

}
