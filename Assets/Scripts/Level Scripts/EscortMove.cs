using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EscortMove : MonoBehaviour
{
    public bool startEscortEvent;

    public float energyMax;
    [Range(0f, 100f)]
    public float energy;

    public int firstStopWaypoint;
    public float firstStopWaitTime;
    public GameObject FirstBlockade;

    public int secondStopBlockWaypoint;
    public float secondStopWaittime;
    public GameObject SecondBlockade;

    public int thirdStopBlockWaypoint;
    public float thirdStopWaittime;
    public GameObject thirdBlockade;

    public Transform[] points;


    private int destPoint = 0;
    private NavMeshAgent agent;
    private float AgentLeechSpeed;
    private float AgentSpeedStep;

    private bool FirstPhaseFinished;
    private bool SecondPhaseFinished;
    private bool ThirdPhaseFinished;

    private Coroutine PhaseTimer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        startEscortEvent = false;
        FirstPhaseFinished = false;
        SecondPhaseFinished = false;
        ThirdPhaseFinished = false;

        AgentSpeedStep = agent.speed / energyMax;         // 
    }


    void GotoNextPoint()
    {
        // Returns if no points have been set up
        if (points.Length == 0)
            return;

        // Set the agent to go to the currently selected destination.
        agent.destination = points[destPoint].position;

        // Choose the next point in the array as the destination,
        // cycling to the start if necessary.

        //destPoint = (destPoint + 1) % points.Length;
        destPoint++;

    }


    void Update()
    {
        AgentLeechSpeed = energy * AgentSpeedStep;
        agent.speed = AgentLeechSpeed;

        if (startEscortEvent)
        {
            if (!FirstPhaseFinished)
            {
                // Start of the first phase
                if (destPoint < firstStopWaypoint)
                {
                    if (!agent.pathPending && agent.remainingDistance < 0.5f)
                        GotoNextPoint();
                }
                else if (destPoint == firstStopWaypoint)
                {
                    agent.isStopped = true;
                    StartCoroutine(TimerFirstPhase(firstStopWaitTime));
                }
            }


            // Start of the second phase
            if (FirstPhaseFinished && !SecondPhaseFinished)
            {
                if (destPoint < secondStopBlockWaypoint)
                {
                    if (!agent.pathPending && agent.remainingDistance < 0.5f)
                        GotoNextPoint();
                }
                else if (destPoint == secondStopBlockWaypoint)
                {
                    agent.isStopped = true;
                    StartCoroutine(TimerSecondPhase(secondStopWaittime));
                }
            }

            // Start of the third phase
            if (SecondPhaseFinished && !ThirdPhaseFinished)
            {
                if (destPoint < thirdStopBlockWaypoint)
                {
                    if (!agent.pathPending && agent.remainingDistance < 0.5f)
                        GotoNextPoint();
                }
                else if (destPoint == thirdStopBlockWaypoint)
                {
                    agent.isStopped = true;
                    StartCoroutine(TimerThirdPhase(thirdStopWaittime));
                }
            }

            // Start of the final phase
            if (ThirdPhaseFinished)
            {
                if (destPoint < points.Length)
                {
                    if (!agent.pathPending && agent.remainingDistance < 0.5f)
                        GotoNextPoint();
                }
                else if (destPoint == points.Length)
                {
                    agent.isStopped = true;
                }
            }
        }


    }
    private IEnumerator TimerFirstPhase(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        FirstBlockade.SetActive(false);
        agent.isStopped = false;
        FirstPhaseFinished = true;
        // End of the first phase
    }
    private IEnumerator TimerSecondPhase(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        SecondBlockade.SetActive(false);
        agent.isStopped = false;
        SecondPhaseFinished = true;
        // End of the second phase
    }
    private IEnumerator TimerThirdPhase(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        thirdBlockade.SetActive(false);
        agent.isStopped = false;
        ThirdPhaseFinished = true;
        // End of the third phase
    }

}
