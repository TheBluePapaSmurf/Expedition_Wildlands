using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewStructureData", menuName = "GridPlacement/StructureData")]
public class StructureData : ScriptableObject
{
    [System.Serializable]
    public class Structure
    {
        public string objectName; // Naam van het object
        public int objectID; // Uniek ID voor het object
        public StructureType structureType; // Enum type (vloer, muur, enz.)
        public GameObject prefab; // De daadwerkelijke prefab
        public GameObject previewPrefab; // De preview-versie van de prefab
    }

    public List<Structure> structures = new List<Structure>(); // Lijst van structuren

    public enum StructureType
    {
        Floor,
        Wall,
        Furniture,
        Decoration,
        Roof
    }

}

