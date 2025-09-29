using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "ScaryGame")]
public class EnemyTemplate : ScriptableObject
{
    public new string name;
    public float speed;
    public float hp;
    public int damage;
    public int PointsReward;
    public bool SlowedByLight;

    public Sprite sprite;



}
