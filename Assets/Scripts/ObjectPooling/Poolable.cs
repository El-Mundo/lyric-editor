using Lyrics.Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poolable : PoolableBehaviour
{
    int i = 0;
    public override bool IsAvailable()
    {
        i++;
        return i==1;
    }

    public override void Recycle()
    {
        i = 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
