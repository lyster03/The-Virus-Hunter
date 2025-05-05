using UnityEngine;

[CreateAssetMenu(menuName = "PowerUps/Pet Companion")]
public class PetPowerUp : PowerUp
{
    public GameObject petPrefab;

    public override void Apply(GameObject player)
    {
        if (GameObject.FindWithTag("Pet") != null) return;

        GameObject pet = GameObject.Instantiate(petPrefab);
        pet.tag = "Pet";
        PetShooter shooter = pet.GetComponent<PetShooter>();

        if (shooter != null)
        {
            shooter.player = player.transform;
        }
    }
}
