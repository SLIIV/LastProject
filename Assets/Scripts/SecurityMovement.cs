using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class SecurityMovement : CharactersMovement
{private bool _isDestinated;

    private VisitorsMovement[] visitors;
    [SerializeField] private Animations[] _animations;

    private NavMeshAgent _agent;
    
    private AnimationClip _currentClip;
    private Dictionary<Animations.AnimationType, AnimationClip> _animationDictionary = new Dictionary<Animations.AnimationType, AnimationClip>();



    protected override Vector3 SelectNextMoveTarget()
    {
        int randomVisitor = Random.Range(0, visitors.Length);
        return visitors[randomVisitor].transform.position;
    }

    private void Start() 
    {
        
        Animator = GetComponent<Animator>();
        visitors = FindObjectsOfType<VisitorsMovement>();
        for(int i = 0; i < _animations.Length; i++)
        {
            _animationDictionary.Add(_animations[i].Animation, _animations[i].clip);
        }
        _agent = GetComponent<NavMeshAgent>();
        _agent.destination = SelectNextMoveTarget();
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
            _agent.isStopped = true;
            _currentClip =  _animationDictionary[Animations.AnimationType.idle];
            PlayAnimation(_currentClip);
            StartCoroutine(CreateNewPathByTime(_agent));
            _isDestinated = true;
        }
    }
    protected override bool IsDestinationComplete(NavMeshAgent agent)
    {
        if(_agent.remainingDistance <= DistanceToCompletePath)
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
        agent.isStopped = false;
        CreateNewPath(agent);
    }
    protected override void CreateNewPath(NavMeshAgent agent)
    {
        Vector3 destination = SelectNextMoveTarget();
        agent.destination = destination;
        _isDestinated = false;
    }
}
