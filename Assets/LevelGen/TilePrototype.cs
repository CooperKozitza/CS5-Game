using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEditor;
using System;

namespace Tile
{
    public class TilePrototype : MonoBehaviour
    {
        [Serializable]
        public class Face
        {
            public bool permiable = false;
            public List<GameObject> neighbors = new List<GameObject>();
        }

        void Awake()
        {
            for (int i = 0; i < Left.neighbors.Count; i++)
            {
                if (PrefabUtility.GetCorrespondingObjectFromOriginalSource(Left.neighbors[i]) != null)
                {
                    Left.neighbors[i] = PrefabUtility.GetCorrespondingObjectFromOriginalSource(Left.neighbors[i]);
                }
            }
            for (int i = 0; i < Front.neighbors.Count; i++)
            {
                if (PrefabUtility.GetCorrespondingObjectFromOriginalSource(Front.neighbors[i]) != null)
                {
                    Front.neighbors[i] = PrefabUtility.GetCorrespondingObjectFromOriginalSource(Front.neighbors[i]);
                }
            }
            for (int i = 0; i < Right.neighbors.Count; i++)
            {
                if (PrefabUtility.GetCorrespondingObjectFromOriginalSource(Right.neighbors[i]) != null)
                {
                    Right.neighbors[i] = PrefabUtility.GetCorrespondingObjectFromOriginalSource(Right.neighbors[i]);
                }
            }
            for (int i = 0; i < Back.neighbors.Count; i++)
            {
                if (PrefabUtility.GetCorrespondingObjectFromOriginalSource(Back.neighbors[i]) != null)
                {
                    Back.neighbors[i] = PrefabUtility.GetCorrespondingObjectFromOriginalSource(Back.neighbors[i]);
                }
            }
        }

        void OnEnable()
        {
            for (int i = 0; i < Left.neighbors.Count; i++)
            {
                if (PrefabUtility.GetCorrespondingObjectFromOriginalSource(Left.neighbors[i]) != null)
                {
                    Left.neighbors[i] = PrefabUtility.GetCorrespondingObjectFromOriginalSource(Left.neighbors[i]);
                }
            }
            for (int i = 0; i < Front.neighbors.Count; i++)
            {
                if (PrefabUtility.GetCorrespondingObjectFromOriginalSource(Front.neighbors[i]) != null)
                {
                    Front.neighbors[i] = PrefabUtility.GetCorrespondingObjectFromOriginalSource(Front.neighbors[i]);
                }
            }
            for (int i = 0; i < Right.neighbors.Count; i++)
            {
                if (PrefabUtility.GetCorrespondingObjectFromOriginalSource(Right.neighbors[i]) != null)
                {
                    Right.neighbors[i] = PrefabUtility.GetCorrespondingObjectFromOriginalSource(Right.neighbors[i]);
                }
            }
            for (int i = 0; i < Back.neighbors.Count; i++)
            {
                if (PrefabUtility.GetCorrespondingObjectFromOriginalSource(Back.neighbors[i]) != null)
                {
                    Back.neighbors[i] = PrefabUtility.GetCorrespondingObjectFromOriginalSource(Back.neighbors[i]);
                }
            }
        }

        public Face Left = new Face();
        public Face Front = new Face();
        public Face Right = new Face(); 
        public Face Back = new Face();
    }
}

