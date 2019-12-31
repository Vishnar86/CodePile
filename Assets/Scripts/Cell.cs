﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public Vector2 GridCord;
    public int GridIndex;
    public int[] Neighbors;
    public float MoveCost = 5;
    public float Cost;
    public int Previous;
    public bool IsWalkable = true;
}
