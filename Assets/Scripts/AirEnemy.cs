﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("GameScripts/AirEnemy")]

public class AirEnemy : Enemy
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RotateTo();
        MoveTo();
        Fly();
    }

    public void Fly() {
        float flyspeed = 0;
        if (this.transform.position.y < 2.0f) {
            flyspeed = 1.0f;
        }
        this.transform.Translate(new Vector3(0, flyspeed * Time.deltaTime, 0));
    }
}
