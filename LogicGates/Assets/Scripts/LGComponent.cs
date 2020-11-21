using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LGComponent : MonoBehaviour
{
    public ComponentSO componentData;
    public GameObject nodePrefab;
    public GameObject wirePrefab;

    [HideInInspector]
    public int inputs, outputs = 1;
    private ComponentSO.ComponentType componentType;
    
    [HideInInspector]
    public List<GameObject> inputNodes = new List<GameObject>();
    [HideInInspector]
    public List<GameObject> outputNodes = new List<GameObject>();

    //[HideInInspector]
    public bool componentActive = false;
    [HideInInspector]
    public List<Connection> connections = new List<Connection>();

    private SpriteRenderer spr;
    private BoxCollider2D boxCollider;

    // Start is called before the first frame update
    void Start()
    {
        if (GetComponent<SpriteRenderer>()) { spr = GetComponent<SpriteRenderer>(); }
        if (GetComponent<BoxCollider2D>()) { boxCollider = GetComponent<BoxCollider2D>(); }

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

        spr.color = componentData.componentColor;
    }

    void UpdateSpriteRenderer()
    {
        float height = inputs < outputs ? outputs * 0.25f : inputs * 0.25f;
        spr.size = new Vector2(0.5f, height);

        boxCollider.size = new Vector2(0.5f, height);
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

    void UpdateWires()
    {
        foreach (Connection connection in connections)
        {
            if (connection.wire == null)
            {
                LGComponent from = connection.output; //OUTPUT
                LGComponent to = connection.input; //INPUT

                connection.inputNode = connections.Count - 1;
                connection.outputNode = from.outputNodes.Count - 1;

                Vector2 node1 = from.outputNodes[connection.outputNode].transform.position;
                Vector2 node2 = to.inputNodes[connection.inputNode].transform.position;

                GameObject newLR = Instantiate(wirePrefab, transform.position, Quaternion.identity);
                newLR.transform.position = Vector3.zero;

                LineRenderer newLRComp = newLR.GetComponent<LineRenderer>();
                newLRComp.positionCount = 2;
                newLRComp.SetPositions(new Vector3[] { node1, node2 });

                connection.wire = newLRComp;
            }
            else
            {
                LGComponent from = connection.output; //OUTPUT
                LGComponent to = connection.input; //INPUT

                Vector2 node1 = from.outputNodes[connection.outputNode].transform.position;
                Vector2 node2 = to.inputNodes[connection.inputNode].transform.position;

                connection.wire.positionCount = 2;
                connection.wire.SetPositions(new Vector3[] { node1, node2 });
            }
        }
    }

    public void DeleteComponent()
    {
        foreach (Connection conn in connections)
        {
            Destroy(conn.wire.gameObject);
        }

        Destroy(this.gameObject);
    }

    private void Update()
    {
        //See how many connections are active
        int activeConnections = 0;
        foreach (Connection conn in connections)
        {
            if (conn.output.componentActive)
            {
                activeConnections++;
            }
        }

        #region CheckComponentType
        switch (componentType)
        {
            case ComponentSO.ComponentType.AND: //AND GATE 
                if (activeConnections >= inputs)
                {
                    if (connections.Count > 0)
                    {
                        componentActive = true;
                    }
                    else
                    {
                        componentActive = false;
                    }
                }
                else
                {
                    componentActive = false;
                }
                break;
            case ComponentSO.ComponentType.OR: //OR GATE
                if (activeConnections > 0)
                {
                    componentActive = true;
                    break;
                } 
                componentActive = false;
                break;
            case ComponentSO.ComponentType.NAND: //Not AND GATE
                if (activeConnections >= connections.Count)
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
                if (activeConnections == 0 || activeConnections >= connections.Count)
                {
                    componentActive = false;
                }
                else
                {
                    componentActive = true;
                }
                break;
            case ComponentSO.ComponentType.XNOR: //Exclusive NOR GATE
                if (activeConnections == 0 || activeConnections >= connections.Count)
                {
                    componentActive = true;
                }
                else
                {
                    componentActive = false;
                }
                break;
            case ComponentSO.ComponentType.POWER:
                componentActive = true;
                break;
            case ComponentSO.ComponentType.LIGHT:
                if (activeConnections > 0)
                {
                    componentActive = true;
                    spr.color = componentData.componentColor;
                }
                else
                {
                    componentActive = false;
                    spr.color = new Color(0.5f, 0.5f, 0.5f);
                }
                break;
        }
        #endregion

        //Remove null inputs
        List<int> nullInputs = new List<int>();
        foreach(Connection conn in connections)
        {
            if(conn.input == null)
            {
                nullInputs.Add(connections.IndexOf(conn));
            }
        }
        for(int n =0; n < nullInputs.Count;n++)
        {
            connections.RemoveAt(nullInputs[n]);
        }


        //Remove overflowing inputs
        if (connections.Count > inputs)
        {
            int removeRange = connections.Count - inputs;
            connections.RemoveRange(connections.Count - removeRange - 1, removeRange);
        }

        UpdateWires();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        foreach (Connection conn in connections)
        {
            if (conn.input != null)
            {
                Gizmos.DrawLine(transform.position, conn.input.transform.position);
            }
        }
    }
}

public class Connection
{
    public LGComponent input;
    public LGComponent output;
    public LineRenderer wire;

    public int outputNode;
    public int inputNode;

    public Connection(LGComponent input, LGComponent output, LineRenderer wire)
    {
        this.input = input;
        this.output = output;
        this.wire = wire;
    }
}
