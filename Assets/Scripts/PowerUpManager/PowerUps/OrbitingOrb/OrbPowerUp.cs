using UnityEngine;

[CreateAssetMenu(menuName = "PowerUps/Orbiting Orb")]
public class OrbitingOrbPowerUp : PowerUp
{
    public GameObject orbPrefab;

    public override void Apply(GameObject player)
    {
        GameObject orb = Instantiate(orbPrefab, player.transform.position, Quaternion.identity);
        orb.transform.SetParent(player.transform);
        OrbitingOrb orbitScript = orb.GetComponent<OrbitingOrb>();
        if (orbitScript != null)
        {
            orbitScript.player = player.transform;
        }
    }
}
