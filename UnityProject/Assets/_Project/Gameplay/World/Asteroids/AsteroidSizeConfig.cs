using UnityEngine;



public enum AsteroidSize {Size1, Size2, Size3}



[CreateAssetMenu(
    fileName = "AsteroidSizeConfig",
    menuName = "Asteroids/Size Config")]
public class AsteroidSizeConfig : ScriptableObject
{    
    public AsteroidSize size;

    [Header("Stats")]
    public float maxHP = 10f;

    [Header("Visuals")]
    public Vector2 scaleRange = new Vector2(1f, 1.5f);

    [Header("Splitting")]
    public int splitCount = 2;
    public float splitImpulse = 4f;
    [Range(0f, 1f)]
    public float velocityInheritance = 1f;

    [Header("Start Spin - Degrees per second")]
    public float spinMin = 10f;
    public float spinMax = 20f;
}

