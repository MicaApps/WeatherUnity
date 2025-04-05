using DG.Tweening;
using DG.Tweening.Plugins.Core.PathCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathMover : MonoBehaviour
{
    public float movetime;
    public EasyPath path;
    
    public bool move = false;
    public bool moveend = false;

    private void Start()
    {
        
    }
    void Update()
    {
        if (move && !moveend)
        {
            transform.DOPath(path.path, movetime);
            moveend = true;
        }
        if (!move) 
        {
            transform.position = path.path[0];
            moveend = false;
        }
    }
}
