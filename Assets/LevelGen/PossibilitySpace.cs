using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PossibilitySpace : MonoBehaviour
{
    [Range(0, 3)]
    public int rotation = 0;
    public List<GameObject> DefaultEntropy = new List<GameObject>();
    [System.NonSerialized]
    public List<GameObject> Entropy = new List<GameObject>();

    private void Start()
    {
        Entropy = DefaultEntropy;
    }
}
