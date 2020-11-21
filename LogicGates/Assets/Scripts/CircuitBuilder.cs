using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircuitBuilder : MonoBehaviour
{
    public GameObject componentPrefab;
    public List<ComponentSO> components;

    private ComponentSO currComponent;

    private Camera cam;

    private Transform moveComponent;
    private LGComponent currNodeConnector;

    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
        currComponent = components[0];
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) //Left mouse button
        {
            Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition); //Convert mouse pos to world pos

            //Check to see if object is at position
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, 1000);

            if (hit.collider == null) //Nothing was hit
            {
                GameObject newComponent = Instantiate(componentPrefab, transform.position, Quaternion.identity);
                newComponent.transform.position = (Vector2)mousePos;

                if (newComponent.GetComponent<LGComponent>())
                {
                    LGComponent comp = newComponent.GetComponent<LGComponent>();
                    comp.componentData = currComponent;
                }
            }
            else
            {
                if (hit.collider.GetComponentInParent<LGComponent>()) //Hit node
                {
                    currNodeConnector = hit.collider.GetComponentInParent<LGComponent>();
                }
                if (hit.collider.GetComponent<LGComponent>()) //Hit component
                {
                    moveComponent = hit.transform;
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (currNodeConnector != null)
            {
                Debug.Log("1");
                //Check to see if object is at position
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, 1000);

                if (hit.collider != null) //Hit something
                {
                    Debug.Log("2");
                    if (hit.collider.GetComponentInParent<LGComponent>()) //Hit node
                    {
                        Debug.Log("3");
                        LGComponent parentComponent = hit.collider.GetComponentInParent<LGComponent>();
                        if (parentComponent != currNodeConnector)
                        {
                            Debug.Log("4");
                            Debug.Log("Connected nodes");
                            parentComponent.inputConnections.Add(currNodeConnector);
                        }
                    }
                }
            }
            currNodeConnector = null;
            moveComponent = null;
        }

        if (moveComponent != null)
        {
            moveComponent.position = (Vector2)cam.ScreenToWorldPoint(Input.mousePosition);
        }
    }
}
