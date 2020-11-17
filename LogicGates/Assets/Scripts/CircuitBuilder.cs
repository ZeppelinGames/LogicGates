using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircuitBuilder : MonoBehaviour
{
    public GameObject componentPrefab;
    public List<ComponentSO> components;

    private ComponentSO currComponent;

    private Camera cam;

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
            GameObject newComponent = Instantiate(componentPrefab, transform.position, Quaternion.identity);
            newComponent.transform.position = mousePos;

            if (newComponent.GetComponent<LGComponent>())
            {
                LGComponent comp = newComponent.GetComponent<LGComponent>();
                comp.componentData = currComponent;
            }
        }
    }
}
