using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public Player player;

    [Header("Fruit Management")]
    public bool fruitsHaveRandomLook;
    public int fruitsCollected;

    public void Awake()
    {
        if(instance == null) 
            instance = this;
        else
            Destroy(gameObject);
    }


    public void AddFruit() => fruitsCollected++;
    public bool FruitsHaveRandomLook() => fruitsHaveRandomLook;

}
