using System;
using UnityEngine;

[Serializable]
public class SymbolData
{
    [SerializeField] private string symbolName;
    [SerializeField] private Sprite sprite;
    [SerializeField] private int payoutMultiplier = 1;
    [SerializeField] private int weight = 1;

    public string SymbolName => symbolName;
    public Sprite Sprite => sprite;
    public int PayoutMultiplier => payoutMultiplier;
    public int Weight => weight;
}