using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Materials
{
    public static Color Water { get => water; }
    public static Color Sand { get => sand; }

    private static readonly Color water = Color.blue;
    private static readonly Color sand = Color.yellow;    
}
