using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class VisitorsMovement : CharactersMovement
{
    [SerializeField] private string _effectAreaName;
    [SerializeField] private Animations[] _animations;
    private bool _isDestinated;

    private NavMeshAgent _agent;

    private Dictionary<Animations.AnimationType, AnimationClip> _animationDictionary = new Dictionary<Animations.AnimationType, AnimationClip>();

    private AnimationClip _currentClip;


    protected override Vector3 SelectNextMoveTarget(Vector3 center, float range)
    {
        Vector3 randomPoint = center + Random.insideUnitSphere * range;
        NavMeshHit hit;
        if(NavMesh.SamplePosition(randomPoint, out hit, range / 2, NavMesh.AllAreas))
        {
            return hit.position;
            
        } 
        else 
        {
            return SelectNextMoveTarget(center, range);
        }
        
    }

    private void Awake() 
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.destination = SelectNextMoveTarget(_agent.transform.position, RangeToFindPath);
        Animator = GetComponent<Animator>();
        for(int i = 0; i < _animations.Length; i++)
        {
            _animationDictionary.Add(_animations[i].Animation, _animations[i].clip);
        }
        
    }
    private void Update()
    {
        if(!_agent.isStopped)
        {
            if(_currentClip != _animationDictionary[Animations.AnimationType.walk])
            {
                _currentClip = _animationDictionary[Animations.AnimationType.walk];
                PlayAnimation(_currentClip);
                
            }
        }
       
        if(IsDestinationComplete(_agent) && !_isDestinated)
        {
            
            if(NavMesh.SamplePosition(_agent.transform.position, out NavMeshHit hit, 0.1f, NavMesh.AllAreas))
            {
                if(IndexFromMask(hit.mask) == NavMesh.GetAreaFromName("Dance"))
                {
                    _currentClip = _animationDictionary[Animations.AnimationType.dance];
                    PlayAnimation(_currentClip);
                    _agent.isStopped = true;
                    StartCoroutine(CreateNewPathByTime(_agent, MinStandTime * 3, MaxStandTime * 2));
                }
                else 
                {
                    _agent.isStopped = true;
                    _currentClip = _animationDictionary[Animations.AnimationType.idle];
                    PlayAnimation(_currentClip);
                    StartCoroutine(CreateNewPathByTime(_agent));
                }
            
            }
            _isDestinated = true;
            
        }
    }
    protected override bool IsDestinationComplete(NavMeshAgent agent)
    {
        if(agent.remainingDistance <= DistanceToCompletePath)
        {
            return true;
        }
        else if(agent.isStopped)
        {
            return true;
        }
        return false;
    }
    protected override IEnumerator CreateNewPathByTime(NavMeshAgent agent)
    {
        yield return new WaitForSeconds(Random.Range(MinStandTime, MaxStandTime));
        _agent.isStopped = false;
        CreateNewPath(agent);
    }
    protected override IEnumerator CreateNewPathByTime(NavMeshAgent agent, float minTime, float maxTime)
    {
        yield return new WaitForSeconds(Random.Range(MinStandTime, MaxStandTime));
        _agent.isStopped = false;
        CreateNewPath(agent);
    }
    protected override void CreateNewPath(NavMeshAgent agent)
    {
        Vector3 destination = SelectNextMoveTarget(agent.transform.position, RangeToFindPath);
        if(Vector3.Distance(destination, agent.transform.position) >= MinDistance)
        {
            agent.destination = destination;
            _isDestinated = false;
            
        }
        else CreateNewPath(agent);
    }

    private int IndexFromMask(int mask)
    {
        for (int i = 0; i < 32; ++i)
        {
            if ((1 << i) == mask)
                return i;
        }
        return -1;
    }


}


