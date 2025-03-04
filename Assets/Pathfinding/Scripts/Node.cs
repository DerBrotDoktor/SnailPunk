using UnityEngine;

public class Node
{
    public Vector3Int Position { get; private set; }
    public float GCost { get; set; }
    public float HCost { get; set; }
    public float FCost => GCost + HCost;  // Gesamtkosten des Knotens
    public Node Parent { get; set; }

    public Node(Vector3Int position)
    {
        Position = position;
        GCost = float.MaxValue;  // Initiale Kosten sind maximal
        HCost = 0;
        Parent = null;
    }

    public override bool Equals(object obj)
    {
        return obj is Node node && Position.Equals(node.Position);
    }

    public override int GetHashCode()
    {
        return Position.GetHashCode();
    }

    public override string ToString()
    {
        return $"Position: {Position}, GCost: {GCost}, HCost: {HCost}, FCost: {FCost}";
    }
}