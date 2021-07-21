using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
public class CharactersMovement : MonoBehaviour
{
    [SerializeField] protected float RangeToFindPath;
    [SerializeField] protected float DistanceToCompletePath;
    [SerializeField] protected float MinDistance;
    [SerializeField] protected float MinStandTime;
    [SerializeField] protected float MaxStandTime;
    [HideInInspector] protected Animator Animator;


    virtual protected Vector3 SelectNextMoveTarget(Vector3 center, float range)
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
    virtual protected Vector3 SelectNextMoveTarget()
    {
        return Vector3.zero;
    }

    protected virtual bool IsDestinationComplete(NavMeshAgent agent)
    {
        if(agent.remainingDistance <= DistanceToCompletePath)
        {
            return true;
        }
        if(agent.isStopped)
        {
            return true;
        }
        return false;
    }


    protected virtual IEnumerator CreateNewPathByTime(NavMeshAgent agent)
    {
        yield return new WaitForSeconds(Random.Range(MinStandTime, MaxStandTime));
        CreateNewPath(agent);
        
    }
    protected virtual IEnumerator CreateNewPathByTime(NavMeshAgent agent, float minTime, float maxTime)
    {
        yield return new WaitForSeconds(Random.Range(minTime, maxTime));
        CreateNewPath(agent);
        
    }
    protected virtual void CreateNewPath(NavMeshAgent agent)
    {
    }
    protected void PlayAnimation(AnimationClip clip)
    {
        Animator.Play(clip.name);
    }
    
 
}
    [  System.Serializable]
   public struct Animations
    {
        public AnimationType Animation;
        public AnimationClip clip;
        public enum AnimationType
        {
            idle,
            walk,
            dance
        }
    }