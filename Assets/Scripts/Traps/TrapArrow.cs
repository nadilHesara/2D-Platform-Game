using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapArrow : Trap_Trampoline
{
    [Header("Additional Info")]
    [SerializeField] private float cooldown;
    [SerializeField] private bool rotationRight;
    [SerializeField] private float rotationSpeed = 120;
    private int direction = -1;
    [Space]
    [SerializeField] private float scaleUpSpeed = 10;
    [SerializeField] private Vector3 targetScale;


    private void Start()
    {
        transform.localScale = new Vector3(.3f, .3f, .3f);
    }


    private void Update()
    {
        HandleScaleUp();
        HandleRotation();
    }

    private void HandleScaleUp()
    {
        if (transform.localScale.x < targetScale.x)
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, scaleUpSpeed * Time.deltaTime);
    }

    private void HandleRotation()
    {
        direction = rotationRight ? -1 : 1;

        transform.Rotate(0, 0, (rotationSpeed * direction) * Time.deltaTime);
    }

    private void DestroyMe()
    { 
        GameObject arrowPrefab = ObjectCreator.instance.arrowPrefab;

        ObjectCreator.instance.CreateObject(arrowPrefab, transform,false, cooldown);

        Destroy(gameObject);
    }


    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();

        if (player != null)
        {
            player.Push(transform.up * pushPower, duration);
            anim.SetTrigger("activate");
        }

    }

}
