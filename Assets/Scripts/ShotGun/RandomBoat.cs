using UnityEngine;
using System.Collections;

public class RandomBoat : MonoBehaviour
{
		public NavMeshAgent myAgent;
		float cooldown;
		Vector3 curDestination;

		Vector3 AiRandomPatrolPoints ()
		{
		curDestination = new Vector3 (Random.Range (transform.position.x - 100f, transform.position.x + 100f), 0, Random.Range (transform.position.z - 100f, transform.position.z + 100f));
				NavMeshHit navPointA;
				NavMesh.SamplePosition (curDestination, out navPointA, 400f, 1);
				return navPointA.position;
		}

		void Start ()
		{
				cooldown = Random.Range (5, 15);
				myAgent.SetDestination (AiRandomPatrolPoints ());
		}
	
		// Update is called once per frame
		void Update ()
		{
				cooldown -= Time.deltaTime;
				if (cooldown <= 0f) {
						myAgent.SetDestination (AiRandomPatrolPoints ());
						cooldown = Random.Range (5, 15);
				}
		}
}
