using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PossibilitySpace : MonoBehaviour
{
    [Range(0, 3)]
    public int rotation = 0;
    public List<GameObject> DefaultEntropy = new List<GameObject>();
    public List<GameObject> Entropy = new List<GameObject>();
    private Material material { get; set; }
    public bool previouslyPropogated { get; set; }

    void Awake()
    {
        material = GetComponent<Renderer>().material;
        foreach (GameObject possibility in DefaultEntropy) Entropy.Add(possibility);
    }

    void Update()
    {
        material.color = new Color(material.color.r, material.color.g, material.color.b, (float)Entropy.Count / DefaultEntropy.Count);
    }
}
