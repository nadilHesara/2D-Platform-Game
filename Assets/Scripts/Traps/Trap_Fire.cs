using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap_Fire : MonoBehaviour
{
    [SerializeField] private Trap_FireButton fireButton;
    [SerializeField] private float offDuration;


    private Animator anim;
    private CapsuleCollider2D fireCollider;
    private bool isActive;


    private void Awake()
    {
        anim = GetComponent<Animator>();    
        fireCollider = GetComponent<CapsuleCollider2D>();
    }

    private void Start()
    {
        if(fireButton == null)
            Debug.LogWarning("You don't have a fire button on " + gameObject.name + "!!!");

        SetFire(true);

    }

    public void SwitchOffFire() 
    {
        if (isActive == false)
            return;

        StartCoroutine(FireCoroutine());
    }
    private IEnumerator FireCoroutine() 
    {
        SetFire(false); 

        yield return new WaitForSeconds(offDuration);

        SetFire(true);
    }
    private void SetFire(bool active)
    {
        anim.SetBool("active", active);
        fireCollider.enabled = active;
        isActive = active;
    }
}
