using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ComponentData", menuName = "New Component")]
public class ComponentSO : ScriptableObject
{
    public string componentName;
    [Range(0, 10)]
    public int inputs = 1, outputs = 1;
    public ComponentType componentType;
    public Color componentColor = Color.blue;

    [HideInInspector]
    public bool isInput;

    public enum ComponentType { AND, OR, NOT, NAND, NOR, XOR, XNOR, LIGHT, POWER, SWITCH };

    private void OnValidate()
    {
        if (componentType != ComponentType.LIGHT)
        {
            if (outputs > 1) { outputs = 1; }
        }

        if (componentType != ComponentType.POWER)
        {
            if (inputs < 1) { inputs = 1; }
        }

        switch (componentType)
        {
            case ComponentType.LIGHT:
                if (outputs > 0) { outputs = 0; }
                break;
            case ComponentType.POWER:
                if (inputs > 0) { inputs = 0; }
                break;
            case ComponentType.SWITCH:
                isInput = true;
                if (inputs > 1) { inputs = 1; }
                if(outputs < 1) { outputs = 1; }
                break;
        }
    }
}
