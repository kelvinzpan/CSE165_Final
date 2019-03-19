using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject playerCameraContainer;
    public LineRenderer raycastLine;
    public LineRenderer rectangularSelectionLine;
    public GameObject rectangularSelectionBox;
    public GameObject blueCastle;
    public Vector3 raycastOffset;
    public float raycastLength;

    private Vector3 initialCameraForward;
    private bool isRectangularSelecting = false;
    private bool isBlueCastleSelected = false;
    private Vector3 rectangularSelectingStart;
    private Vector3 rectangularSelectingEnd;
    private HashSet<GameObject> hoveredUnits = new HashSet<GameObject>();
    private HashSet<GameObject> selectedUnits = new HashSet<GameObject>();

    const string LAYER_CASTLE = "Castle";
    const string LAYER_SOLDIER = "Soldier";
    const string LAYER_FLOOR = "Floor";

    // Start is called before the first frame update
    void Start()
    {
        initialCameraForward = playerCameraContainer.transform.forward;
        rectangularSelectionLine.positionCount = 5;
        blueCastle.GetComponent<BlueCastle>().HideSpawnRange();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 raycastStart = playerCameraContainer.transform.position;
        Vector3 raycastDir = (playerCameraContainer.transform.rotation * initialCameraForward).normalized;
        bool usingSelectInput = Input.GetMouseButton(1);
        bool usingCommandInput = Input.GetMouseButton(0);

        raycastLine.SetPosition(0, raycastStart + raycastOffset);
        raycastLine.SetPosition(1, raycastStart + raycastDir * raycastLength);

        if (isBlueCastleSelected) blueCastle.GetComponent<BlueCastle>().ShowSpawnRange();
        else blueCastle.GetComponent<BlueCastle>().HideSpawnRange();

        if (usingSelectInput)
        {
            if (isRectangularSelecting)
            {
                RaycastHit hit;
                Ray ray = new Ray(raycastStart + raycastOffset, raycastDir);
                if (Physics.Raycast(ray, out hit, raycastLength, LayerMask.GetMask(LAYER_FLOOR)))
                {
                    rectangularSelectingEnd = hit.point;
                    clearHoveredUnits();

                    List<Collider> colliders = rectangularSelectionBox.GetComponent<OverlapBox>().GetColliders();
                    foreach (Collider collider in colliders)
                    {
                        GameObject currObj = collider.gameObject;

                        if (currObj.layer == LayerMask.NameToLayer(LAYER_SOLDIER) && currObj.GetComponent<TeamColors>().IsBlueTeam()) // Hover blue soldiers
                        {
                            hoverUnit(currObj);
                        }
                        /*
                        else if (currObj.layer == LayerMask.NameToLayer(LAYER_CASTLE) && currObj.GetComponent<TeamColors>().IsBlueTeam()) // Hover blue castle
                        {
                            clearHoveredUnits();
                            hoverUnit(currObj);
                            blueCastle.GetComponent<BlueCastle>().ShowSpawnRange();
                            break;
                        }
                        */
                    }
                }
            }
            else
            {
                RaycastHit hit;
                Ray ray = new Ray(raycastStart + raycastOffset, raycastDir);
                if (Physics.Raycast(ray, out hit, raycastLength, LayerMask.GetMask(LAYER_FLOOR)))
                {
                    rectangularSelectingStart = hit.point;
                    rectangularSelectingEnd = hit.point;
                    isRectangularSelecting = true;
                }

                if (Physics.Raycast(ray, out hit, raycastLength, LayerMask.GetMask(LAYER_CASTLE, LAYER_SOLDIER, LAYER_FLOOR)))
                {
                    GameObject currObj = hit.transform.gameObject;

                    if (currObj.layer == LayerMask.NameToLayer(LAYER_FLOOR)) // Deselect by clicking floor
                    {
                        clearSelectedUnits();
                    }
                    else if (currObj.layer == LayerMask.NameToLayer(LAYER_SOLDIER) && currObj.GetComponent<TeamColors>().IsBlueTeam()) // Select blue soldiers
                    {
                        dehoverUnit(currObj);
                        selectUnit(currObj);
                        if (isBlueCastleSelected) deselectUnit(blueCastle);
                    }
                    else if (currObj.layer == LayerMask.NameToLayer(LAYER_CASTLE) && currObj.GetComponent<TeamColors>().IsBlueTeam()) // Select blue castle
                    {
                        clearSelectedUnits();
                        dehoverUnit(currObj);
                        selectUnit(currObj);
                    }
                }
            }
        }
        else if (usingCommandInput)
        {
            RaycastHit hit;
            Ray ray = new Ray(raycastStart + raycastOffset, raycastDir);

            if (isBlueCastleSelected && Physics.Raycast(ray, out hit, raycastLength, LayerMask.GetMask(LAYER_FLOOR)))
            {
                float distFromCastle = (blueCastle.transform.position - hit.point).magnitude;
                float spawnDist = blueCastle.GetComponent<BlueCastle>().spawnRange + blueCastle.GetComponent<BlueCastle>().castleDiameter / 2.0f;
                if (distFromCastle <= spawnDist)
                {
                    blueCastle.GetComponent<BlueCastle>().SpawnSoldier(hit.point);
                }
            }
            else if (Physics.Raycast(ray, out hit, raycastLength, LayerMask.GetMask(LAYER_FLOOR)))
            {
                foreach (GameObject unit in selectedUnits)
                {
                    if (unit.layer == LayerMask.NameToLayer(LAYER_SOLDIER) && unit.GetComponent<TeamColors>().IsBlueTeam())
                    {
                        unit.GetComponent<Soldier>().Defend(hit.point);
                    }
                }
            }
        }
        else
        {
            if (isRectangularSelecting) // Complete rectangular selection
            {
                isRectangularSelecting = false;

                List<GameObject> toSelect = new List<GameObject>();
                foreach (GameObject hovered in hoveredUnits)
                {
                    if (!selectedUnits.Contains(hovered)) toSelect.Add(hovered);
                }

                clearHoveredUnits();
                foreach (GameObject hovered in toSelect) selectUnit(hovered);
            }
            else // Single unit hover
            {
                clearHoveredUnits();
                RaycastHit hit;
                Ray ray = new Ray(raycastStart + raycastOffset, raycastDir);
                if (Physics.Raycast(ray, out hit, raycastLength, LayerMask.GetMask(LAYER_CASTLE, LAYER_SOLDIER)))
                {
                    GameObject currObj = hit.transform.gameObject;

                    if (!selectedUnits.Contains(currObj))
                    {
                        hoverUnit(currObj);
                        if (currObj.layer == LayerMask.NameToLayer(LAYER_CASTLE)) blueCastle.GetComponent<BlueCastle>().ShowSpawnRange();
                    }
                }
            }
        }

        if (isRectangularSelecting)
        {
            rectangularSelectionLine.gameObject.SetActive(true);
            float xDiff = rectangularSelectingEnd.x - rectangularSelectingStart.x;
            float zDiff = rectangularSelectingEnd.z - rectangularSelectingStart.z;
            rectangularSelectionLine.SetPosition(0, rectangularSelectingStart + new Vector3(0.0f, 0.5f, 0.0f));
            rectangularSelectionLine.SetPosition(1, rectangularSelectingStart + new Vector3(xDiff, 0.5f, 0.0f));
            rectangularSelectionLine.SetPosition(2, rectangularSelectingEnd + new Vector3(0.0f, 0.5f, 0.0f));
            rectangularSelectionLine.SetPosition(3, rectangularSelectingStart + new Vector3(0.0f, 0.5f, zDiff));
            rectangularSelectionLine.SetPosition(4, rectangularSelectingStart + new Vector3(0.0f, 0.5f, 0.0f));

            rectangularSelectionBox.gameObject.SetActive(true);
            rectangularSelectionBox.transform.position = rectangularSelectingStart + new Vector3(xDiff / 2.0f, 0.0f, zDiff / 2.0f);
            rectangularSelectionBox.transform.localScale = new Vector3(Mathf.Abs(xDiff), rectangularSelectionBox.transform.localScale.y, Mathf.Abs(zDiff));
        }
        else
        {
            rectangularSelectionLine.gameObject.SetActive(false);
            rectangularSelectionBox.gameObject.SetActive(false);
        }
    }

    void hoverUnit(GameObject unit)
    {
        unit.GetComponent<TeamColors>().SetDefaultHoverMaterial();
        hoveredUnits.Add(unit);
    }

    void dehoverUnit(GameObject unit)
    {
        unit.GetComponent<TeamColors>().SetDefaultMaterial();
        hoveredUnits.Remove(unit);
    }

    void clearHoveredUnits()
    {
        foreach (GameObject hovered in hoveredUnits) hovered.GetComponent<TeamColors>().SetDefaultMaterial();
        hoveredUnits.Clear();
    }

    void selectUnit(GameObject unit)
    {
        unit.GetComponent<TeamColors>().SetDefaultSelectedMaterial();
        selectedUnits.Add(unit);
        if (unit.layer == LayerMask.NameToLayer(LAYER_CASTLE) && unit.GetComponent<TeamColors>().IsBlueTeam()) isBlueCastleSelected = true;
    }

    void deselectUnit(GameObject unit)
    {
        unit.GetComponent<TeamColors>().SetDefaultMaterial();
        selectedUnits.Remove(unit);
        if (unit.layer == LayerMask.NameToLayer(LAYER_CASTLE) && unit.GetComponent<TeamColors>().IsBlueTeam()) isBlueCastleSelected = false;
    }

    void clearSelectedUnits()
    {
        foreach (GameObject selected in selectedUnits) selected.GetComponent<TeamColors>().SetDefaultMaterial();
        selectedUnits.Clear();
        isBlueCastleSelected = false;
    }
}
