using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviour : MonoBehaviour
{
    Transform player;
    NavMeshAgent agent => GetComponent<NavMeshAgent>();
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = FindFirstObjectByType<PlayerBehaviour>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
