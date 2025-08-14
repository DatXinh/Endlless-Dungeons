using UnityEngine;

public class ChaseZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered chase zone");
            foreach (var enemy in FindObjectsOfType<EnemyBase>())
            {
                enemy.StartChasing();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player left chase zone");
            foreach (var enemy in FindObjectsOfType<EnemyBase>())
            {
                enemy.StopChasing();
            }
        }
    }
}
