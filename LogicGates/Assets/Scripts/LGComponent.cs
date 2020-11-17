using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LGComponent : MonoBehaviour
{
    public ComponentSO componentData;
    public GameObject nodePrefab;

    private int inputs, outputs = 1;
    private ComponentSO.ComponentType componentType;
    private List<GameObject> inputNodes = new List<GameObject>();
    private List<GameObject> outputNodes = new List<GameObject>();

    [HideInInspector]
    public bool componentActive = false;
    [HideInInspector]
    public List<LGComponent> inputConnections = new List<LGComponent>();

    private SpriteRenderer spr;

    // Start is called before the first frame update
    void Start()
    {
        spr = GetComponent<SpriteRenderer>();

        GetComponentData();
        UpdateSpriteRenderer();
        UpdateNodes();
    }

    void GetComponentData()
    {
        this.name = componentData.name;

        inputs = componentData.inputs;
        outputs = componentData.outputs;
        componentType = componentData.componentType;
    }

    void UpdateSpriteRenderer()
    {
        float height = inputs < outputs ? outputs * 0.5f : inputs * 0.5f;
        spr.size = new Vector2(0.5f, height);
    }

    void UpdateNodes()
    {
        //INPUT NODES
        if (inputNodes.Count < inputs) //Check to see if we need to instantiate more node GOs
        {
            int spawnNodeCount = inputs - inputNodes.Count;
            for (int i = 0; i < spawnNodeCount; i++)
            {
                GameObject newNode = Instantiate(nodePrefab, transform.position, Quaternion.identity);
                newNode.transform.SetParent(this.transform);
                inputNodes.Add(newNode);
            }
        }
        else //There are enough nodes already. Turn them on
        {
            int activeNodes = 0;
            int lastActiveNodeIndex = 0;
            for (int n = 0; n < inputNodes.Count; n++)
            {
                if (inputNodes[n].activeInHierarchy)
                {
                    activeNodes++;
                    lastActiveNodeIndex = n;
                }
            }

            if (activeNodes > inputs) //More nodes are active than needed. Turn some off
            {
                for (int i = inputNodes.Count - 1; i > activeNodes - inputs; i--)
                {
                    inputNodes[i].SetActive(false);
                }
            }
            else //Not enough nodes are on. Turn some on
            {
                for (int i = lastActiveNodeIndex; i < inputs - activeNodes; i++)
                {
                    inputNodes[i].SetActive(true);
                }
            }
        }

        //OUTPUT NODES
        if (outputNodes.Count < outputs) //Check to see if we need to instantiate more node GOs
        {
            int spawnNodeCount = outputs - outputNodes.Count;
            for (int i = 0; i < spawnNodeCount; i++)
            {
                GameObject newNode = Instantiate(nodePrefab, transform.position, Quaternion.identity);
                newNode.transform.SetParent(this.transform);
                outputNodes.Add(newNode);
            }
        }
        else //There are enough nodes already. Turn them on
        {
            int activeNodes = 0;
            int lastActiveNodeIndex = 0;
            for (int n = 0; n < outputNodes.Count; n++)
            {
                if (outputNodes[n].activeInHierarchy)
                {
                    activeNodes++;
                    lastActiveNodeIndex = n;
                }
            }

            if (activeNodes > outputs) //More nodes are active than needed. Turn some off
            {
                for (int i = outputNodes.Count - 1; i > activeNodes - outputs; i--)
                {
                    outputNodes[i].SetActive(false);
                }
            }
            else //Not enough nodes are on. Turn some on
            {
                for (int i = lastActiveNodeIndex; i < outputs - activeNodes; i++)
                {
                    outputNodes[i].SetActive(true);
                }
            }
        }

        //SET NODE POSITIONS
        Vector2 sprSize = spr.size;
        Vector2 halfSprSize = sprSize / 2;

        float sizeYInputs = sprSize.y / inputs;
        float sizeYOutputs = sprSize.y / outputs;

        Vector2 currPos = transform.position;

        //Inputs Positions
        for (int n = 0; n < inputs; n++)
        {
            float x = currPos.x - halfSprSize.x;
            float y = currPos.y - sizeYInputs + (sizeYInputs * (n - (inputs / 2) + 1)) + ((sizeYInputs / 2) * (inputs % 2 > 0 ? 0 : 1));
            inputNodes[n].transform.position = new Vector2(x, y);
        }

        //Outputs Positions
        for (int n = 0; n < outputs; n++)
        {
            float x = currPos.x + halfSprSize.x;
            float y = currPos.y - sizeYOutputs + (sizeYOutputs * (n - (outputs / 2) + 1)) + ((sizeYOutputs / 2) * (outputs % 2 > 0 ? 0 : 1));
            outputNodes[n].transform.position = new Vector2(x, y);
        }
    }

    private void Update()
    {
        //See how many connections are active
        int activeConnections = 0;
        foreach (LGComponent input in inputConnections)
        {
            if (input.componentActive)
            {
                activeConnections++;
            }
        }

        switch (componentType)
        {
            case ComponentSO.ComponentType.AND: //AND GATE 
                if (activeConnections >= inputConnections.Count)
                {
                    componentActive = true;
                }
                else
                {
                    componentActive = false;
                }
                break;
            case ComponentSO.ComponentType.OR: //OR GATE
                foreach (LGComponent input in inputConnections)
                {
                    if (input.componentActive)
                    {
                        componentActive = true;
                        break;
                    }
                }
                componentActive = false;
                break;
            case ComponentSO.ComponentType.NAND: //Not AND GATE
                if (activeConnections >= inputConnections.Count)
                {
                    componentActive = false;
                }
                else
                {
                    componentActive = true;
                }
                break;
            case ComponentSO.ComponentType.NOR: //Not OR GATE
                if (activeConnections == 0)
                {
                    componentActive = true;
                }
                else
                {
                    componentActive = false;
                }
                break;
            case ComponentSO.ComponentType.XOR: //Exclusive OR GATE
                if (activeConnections == 0 || activeConnections >= inputConnections.Count)
                {
                    componentActive = false;
                }
                else
                {
                    componentActive = true;
                }
                break;
            case ComponentSO.ComponentType.XNOR: //Exclusive NOR GATE
                if (activeConnections == 0 || activeConnections >= inputConnections.Count)
                {
                    componentActive = true;
                }
                else
                {
                    componentActive = false;
                }
                break;
            case ComponentSO.ComponentType.SPLITTER: //Splits current
                if (activeConnections > 0)
                {
                    componentActive = true;
                } else
                {
                    componentActive = false;
                }
                break;
        }
    }
}
