using UnityEngine;

public class EnemyData : MonoBehaviour
{
    public string enemyID;
    public string enemyName;
    public string enemyDescription;
    public int enemyHP;
    public int enemySpeed;
    public GameObject enemyProjectile;
    public int enemyDamage;
    public RangeType rangeType;
}
public enum RangeType
{ 
    None, // Không có tầm đánh
    Melee, // Gần
    Ranged // Xa
}
