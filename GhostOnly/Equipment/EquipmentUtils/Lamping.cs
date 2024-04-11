using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lamping : PoolAble
{
    public bool IsPlayer { get; private set; } = false;
    public bool IsSkill { get; private set; } = false;

    private float timer = 0;

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer > 1)
        {
            ReleaseObject();
            timer = 0;
        }
    }

    public void Initialize(float rotZ, bool isPlayer, bool isSkill)
    {
        transform.rotation = Quaternion.Euler(0, 0, rotZ);

        IsPlayer = isPlayer;

        IsSkill = isSkill;
    }
}
