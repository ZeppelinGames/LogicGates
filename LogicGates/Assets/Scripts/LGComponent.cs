using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LGComponent : MonoBehaviour
{
    [Range(1, 10)]
    public int inputs, outputs = 1;
    public GameObject nodePrefab;

    private List<GameObject> inputNodes = new List<GameObject>();
    private List<GameObject> outputNodes = new List<GameObject>();

    [HideInInspector]
    public List<LGComponent> inputConnections = new List<LGComponent>();

    private SpriteRenderer spr;

    public enum gates { AND, OR, NOT, NAND, NOR, XOR, XNOR };

    // Start is called before the first frame update
    void Start()
    {
        spr = GetComponent<SpriteRenderer>();
        UpdateSpriteRenderer();
        UpdateNodes();
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
            float y = currPos.y - sizeYInputs + (sizeYInputs * (n - (inputs / 2) + 1)) + ((sizeYInputs / 2) * (inputs % 2 > 0 ? 0: 1));
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
}
