using UnityEngine;

public class Finishpoint_Damage_Trap : MonoBehaviour
{
    [SerializeField] private GameObject pickupVfx;
    private Animator anim => GetComponent<Animator>();
    public void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.tag == "Player")
        {
            Player player = collision.gameObject.GetComponent<Player>();

            if (player != null)
            {
                player.Damage();
                anim.SetTrigger("activate");
                player.KnockBack(transform.position.x);
                Destroy(gameObject, 0.5f);
                GameObject newFx = Instantiate(pickupVfx, transform.position, Quaternion.identity);
            }

        }
    }
}
