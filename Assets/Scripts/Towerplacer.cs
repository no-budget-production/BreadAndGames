using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Towerplacer : MonoBehaviour
{
    ////////Variables////////

    public GameObject grid;
    public GameObject highlightedGridField;
    public GameObject tower;
    public Vector3 buildposition;
    public Material outline;
    public Material basematerial;

    private bool buildmode = false;
    private bool chosen = false;
    private bool rotate = false;

    float hgfPosx;
    float hgfPosy;
    float hgfPosz;


    ////////Functions////////

    

    private void Update()
    {
        BuildmodeON();
        InBuildmodeMovement();
        ChooseTower();
        RotateTower();
        PlaceTower();
    }

    void BuildmodeON()
    {
        if (!buildmode && Input.GetKeyDown(KeyCode.JoystickButton3))
        {
            buildmode = true;
            grid.SetActive(true);
        }
    }


    void InBuildmodeMovement()
    {
        if (buildmode)
        {
            if (Input.GetAxis("Horizontal") == -1)
            {
                Vector3 buildposition = new Vector3(hgfPosx - 1, hgfPosy, hgfPosz);
                highlightedGridField.transform.Translate(buildposition);
            }

            if (Input.GetAxis("Horizontal") == 1)
            {
                Vector3 buildposition = new Vector3(hgfPosx + 1, hgfPosy, hgfPosz);
                highlightedGridField.transform.Translate(buildposition);
            }

            if (Input.GetAxis("Vertical") == -1)
            {
                Vector3 buildposition = new Vector3(hgfPosx, hgfPosy, hgfPosz - 1);
                highlightedGridField.transform.Translate(buildposition);
            }

            if (Input.GetAxis("Vertical") == 1)
            {
                Vector3 buildposition = new Vector3(hgfPosx, hgfPosy, hgfPosz + 1);
                highlightedGridField.transform.Translate(buildposition);
            }

            //if (Input.GetKeyDown(KeyCode.A))
            //{
            //    Vector3 buildposition = new Vector3(hgfPosx - 1, hgfPosy, hgfPosz);
            //    highlightedGridField.transform.Translate(buildposition);
            //}

            //if (Input.GetKeyDown(KeyCode.D))
            //{
            //    Vector3 buildposition = new Vector3(hgfPosx + 1, hgfPosy, hgfPosz);
            //    highlightedGridField.transform.Translate(buildposition);
            //}

            //if (Input.GetKeyDown(KeyCode.W))
            //{
            //    Vector3 buildposition = new Vector3(hgfPosx, hgfPosy, hgfPosz + 1);
            //    highlightedGridField.transform.Translate(buildposition);
            //}

            //if (Input.GetKeyDown(KeyCode.S))
            //{
            //    Vector3 buildposition = new Vector3(hgfPosx, hgfPosy, hgfPosz - 1);
            //    highlightedGridField.transform.Translate(buildposition);
            //}
        }
    }


    void ChooseTower()
    {
        if (buildmode /*&& ui(inventory)*/)
        {
            /*if (D-Pad steuerung)
            {
                chosen = true;
            }*/
        }
    }

    void RotateTower()
    {
        if (buildmode && chosen)
        {
            tower = Instantiate(tower, new Vector3(transform.position.x, (transform.position.y + 2), transform.position.z), transform.rotation);
            tower.GetComponent<Renderer>().material = outline;

            if (Input.GetAxis("") >= 0/*Right Stick Steuerung -> right*/)
            {
                tower.transform.Rotate(0, 45, 0);
                rotate = true;

            }

            if (Input.GetAxis("") <= 0/*Right Stick Steuerung -> left*/)
            {
                tower.transform.Rotate(0, - 45, 0);
                rotate = true;
            }
        }
    }

    void PlaceTower()
    {
        if (Input.GetKeyDown(KeyCode.JoystickButton2) && buildmode && rotate)
        {
            tower.GetComponent<Renderer>().material = basematerial;
            tower.AddComponent<Rigidbody>();

            rotate = false;
            chosen = false;
            buildmode = false;

        }
    }
}
