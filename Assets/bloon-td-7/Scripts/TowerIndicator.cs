using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
//using static UnityEditor.BaseShaderGUI;

public class TowerIndicator : MonoBehaviour
{
    [SerializeField] private GameObject building;
    [SerializeField] private GameObject rangeIndicator;
    private static readonly Color cantPlaceColor = new(1, 0, 0, 0.5f);
    private static readonly Color canPlaceColor = new(0.5f, 0.5f, 0.5f, 0.5f);
    
    private List<Material> materials = new();

    void Start() {
        Renderer[] renderers = building.GetComponentsInChildren<Renderer>();
        foreach (Renderer i in renderers) {
            materials.AddRange(i.materials.ToList());
        }
        CanPlace();
    }

    public void CantPlace() {
        foreach (Material m in materials) {
            m.color = cantPlaceColor;
        }
    }

    public void CanPlace() {
        foreach (Material m in materials) {
            m.color = canPlaceColor;
        }
    }

    public void SetRangeIndicator(float range) {
        Vector3 newScale = rangeIndicator.transform.localScale;
        newScale.x = range * 2;
        newScale.z = range * 2;
        rangeIndicator.transform.localScale = newScale;
    }
}
