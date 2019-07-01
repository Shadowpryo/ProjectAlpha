using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    public int STRMod;
    public bool isEquipped = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(isEquipped)
        {
            SphereCollider spherCollider = GetComponent<SphereCollider>();
            Destroy(spherCollider);
        }
    }
}
