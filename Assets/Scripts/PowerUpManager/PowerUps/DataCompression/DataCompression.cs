using UnityEngine;

[CreateAssetMenu(menuName = "PowerUps/Data Compression")]
public class DataCompression : PowerUp
{
    [Range(0.5f, 0.95f)]
    public float scaleMultiplier = 0.75f;

    public override void Apply(GameObject target)
    {
        if (target.transform.localScale.x > 0.6f)
        {
            target.transform.localScale *= scaleMultiplier;
        }

    }
}
