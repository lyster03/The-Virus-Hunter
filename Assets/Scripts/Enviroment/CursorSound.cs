using UnityEngine;

public class MouseClickSound : MonoBehaviour
{
    public AudioClip clickSound;

    [HideInInspector]
    public bool isShooting = false;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!isShooting && clickSound != null)
            {
                SoundFXManager.Instance.PlaySoundFXClip(clickSound, transform, 0.6f);
            }
        }
    }
}
