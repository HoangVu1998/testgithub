using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR;


public class RopeScript : MonoBehaviour
{

    public Vector2 destiny;

    int pointCut;
    public float speed = 1;

    public float distance = 2;

    public GameObject nodePrefab;

    public GameObject player;

    public GameObject lastNode;
    private GameObject EntPoints;

    private bool isDrawNewLine;


    public LineRenderer lr;

    int vertexCount = 2;
    public List<GameObject> Nodes = new List<GameObject>();

    public LineRenderer line1;
    public LineRenderer line2;  


    bool done = false;

    // Use this for initialization
    void Start()
    {

        done = false;
        lr = GetComponent<LineRenderer>();

        player = GameObject.FindGameObjectWithTag("Player");

        lastNode = transform.gameObject;


        Nodes.Add(transform.gameObject);

        player.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
    }

    void Update()
    {
        
        if (Vector2.Distance(lastNode.transform.position, player.transform.position) > distance && !done)
        {
            CreateNode();
        }
        else
        {
            done = true;
            lastNode.GetComponent<HingeJoint2D>().connectedBody = player.GetComponent<Rigidbody2D>();
            lastNode.GetComponent<HingeJoint2D>().autoConfigureConnectedAnchor= false;
            Nodes[0].GetComponent<HingeJoint2D>().autoConfigureConnectedAnchor = false;
            player.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        }
        if(!isDrawNewLine) { RenderLine(); }
        DrawLineNew();
    }
    void RenderLine()
    {
        lr.SetVertexCount(vertexCount);
        int i;
        for (i = 0; i < Nodes.Count; i++)
        {

            lr.SetPosition(i, Nodes[i].transform.position);
        }
        lr.SetPosition(i, player.transform.position);
    }
    void CreateNode()
    {

        Vector2 pos2Create = player.transform.position - lastNode.transform.position;
        pos2Create.Normalize();
        pos2Create *= distance;
        pos2Create += (Vector2)lastNode.transform.position;

        GameObject go = (GameObject)Instantiate(nodePrefab, pos2Create, Quaternion.identity);


        go.transform.SetParent(transform);

        lastNode.GetComponent<HingeJoint2D>().connectedBody = go.GetComponent<Rigidbody2D>();

        lastNode = go;

        Nodes.Add(lastNode);

        vertexCount++;
    }
    void DrawLineNew()
    {
      
        if (RopeCutter.isCutter)
        {
            for (int i = 0; i < Nodes.Count; i++)
            {
                if (RopeCutter.destroyedColliderPosition == Nodes[i].transform.position)
                {
                   pointCut = i;    
                 
                }
            }
            isDrawNewLine=true;
            RopeCutter.isCutter = false;
            Destroy(lr);
        }
        if (isDrawNewLine)
        {
          
            line1.SetVertexCount(pointCut);
            int a;
            for (a = 0; a < pointCut; a++)
            {
                line1.SetPosition(a, Nodes[a].transform.position);
                   
            }
        }
        if (isDrawNewLine)
        {
            line2.SetVertexCount(Nodes.Count - pointCut);
            int a;
            int b = 0;
            for (a = pointCut + 2; a < Nodes.Count; a++)
            {
                line2.SetPosition(b, Nodes[a].transform.position);
                Nodes[a].gameObject.GetComponent<Rigidbody2D>().mass = 0.1f;
                b++;
            }
            line2.SetPosition(b, player.transform.position);
            Invoke("destoyNode", 1f);
        }
    }
    void destoyNode()
    {
        for (int a = pointCut + 2; a < Nodes.Count; a++)
        {
            Destroy(Nodes[a]);
        }
    }
}
