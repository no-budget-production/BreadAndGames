using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Towerplacer : InputManager
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
        PlaceTower();
    }

    void BuildmodeON()
    {
        if (!buildmode && Input.GetButtonDown(XBoxButtonY))
        {
            buildmode = true;
            grid.SetActive(true);
            /* activate UI with towers*/
        }
    }


    void InBuildmodeMovement()
    {
        if (buildmode)
        {
            if (Input.GetAxis(XBoxHorizontalLeftStick) == -1)
            {
                Vector3 buildposition = new Vector3(hgfPosx - 1, hgfPosy, hgfPosz);
                highlightedGridField.transform.Translate(buildposition);
            }

            if (Input.GetAxis(XBoxHorizontalLeftStick) == 1)
            {
                Vector3 buildposition = new Vector3(hgfPosx + 1, hgfPosy, hgfPosz);
                highlightedGridField.transform.Translate(buildposition);
            }

            if (Input.GetAxis(XBoxVerticalLeftStick) == -1)
            {
                Vector3 buildposition = new Vector3(hgfPosx, hgfPosy, hgfPosz - 1);
                highlightedGridField.transform.Translate(buildposition);
            }

            if (Input.GetAxis(XBoxVerticalLeftStick) == 1)
            {
                Vector3 buildposition = new Vector3(hgfPosx, hgfPosy, hgfPosz + 1);
                highlightedGridField.transform.Translate(buildposition);
            }
        }
    }


    void ChooseTower()
    {
        if (buildmode /*&& ui.inventory*/)
        {
            //tower = new GameObject(); <- if more than one tower is avaible --> create towers
            /* add components */

            if (Input.GetButtonDown(XBoxPadLeft))
            {
                /* go through inventory ui */
                /* get refence of object */
                /* RotateTower(object); */
                chosen = true;
            }

            if (Input.GetButtonDown(XBoxPadRight))
            {
                /* go through inventory ui */
                /* get refence of object */
                /* RotateTower(object); */
                chosen = true;
            }
        }
    }

    void RotateTower(GameObject chosenTower)
    {
        if (buildmode && chosen)
        { 
            tower.GetComponent<Renderer>().material = outline;

            if (Input.GetAxis(XBoxHorizontalRightStick) >= 0/*Right Stick Steuerung -> right*/)
            {
                tower.transform.Rotate(0, 45, 0);
                rotate = true;

            }

            if (Input.GetAxis(XBoxHorizontalRightStick) <= 0/*Right Stick Steuerung -> left*/)
            {
                tower.transform.Rotate(0, - 45, 0);
                rotate = true;
            }
        }
    }

    void PlaceTower()
    {
        if (Input.GetKeyDown(XBoxButtonX) && buildmode && rotate)
        {
            tower.GetComponent<Renderer>().material = basematerial;
            tower.AddComponent<Rigidbody>();

            rotate = false;
            chosen = false;
            buildmode = false;

        }
    }
}
