using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShipData", menuName = "CustomData/ShipData", order = 1)]
public class ShipDataSO : ScriptableObject
{
    [SerializeField]
    public ShipData shipData;
}
