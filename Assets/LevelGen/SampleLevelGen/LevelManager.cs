using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public int levelX, levelY;
    public List<GameObject[,]> Mansion { get; set; }

    void Start()
    {
        Mansion = new List<GameObject[,]>();
    }
}
