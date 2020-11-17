using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="ComponentData",menuName ="New Component")]
public class ComponentSO : ScriptableObject
{
    public string componentName;
    [Range(1, 10)]
    public int inputs = 1, outputs = 1;
    public ComponentType componentType;

    public enum ComponentType { AND, OR, NOT, NAND, NOR, XOR, XNOR, SPLITTER, LIGHT };

    private void OnValidate()
    {
        switch (componentType)
        {
            case ComponentType.AND:
                if (outputs > 1) { outputs = 1; }
                break;
            case ComponentType.OR:
                if (outputs > 1) { outputs = 1; }
                break;
            case ComponentType.NOT:
                if (outputs > 1) { outputs = 1; }
                break;
            case ComponentType.NAND:
                if (outputs > 1) { outputs = 1; }
                break;
            case ComponentType.NOR:
                if (outputs > 1) { outputs = 1; }
                break;
            case ComponentType.XOR:
                if (outputs > 1) { outputs = 1; }
                break;
            case ComponentType.XNOR:
                if (outputs > 1) { outputs = 1; }
                break;
            case ComponentType.SPLITTER:
                if (inputs > 1) { inputs = 1; }
                break;
            case ComponentType.LIGHT:
                if (outputs > 0) { outputs = 0; }
                if (inputs > 1) { inputs = 1; }
                break;
        }
    }
}
