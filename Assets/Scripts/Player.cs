using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class Player : MonoBehaviour
{
    public GameObject playerCameraContainer;
    private GameObject rightHand;
    private GameObject leftHand;

    public Canvas leftControllerUI;

    public Vector3 initialPos;
    public Quaternion initialRot;
    public Vector3 sliderInitialPos;
    private bool isflightMode;
    private bool isDraggingSlider;

    public LineRenderer raycastLine;
    public LineRenderer meteorRaycastLine;
    private Ray leftHandRay;
    private bool isCastingMeteor;
    public LineRenderer rectangularSelectionLine;
    public GameObject rectangularSelectionBox;
    public GameObject blueCastle;
    public Vector3 raycastOffset;
    public float raycastLength;

    public float soldierFormationSpacing;

    private Vector3 initialCameraForward;
    private bool isRectangularSelecting = false;
    private bool isBlueCastleSelected = false;
    private bool isUnitUISelected = false;
    private Vector3 rectangularSelectingStart;
    private Vector3 rectangularSelectingEnd;
    private HashSet<GameObject> hoveredUnits = new HashSet<GameObject>();
    private HashSet<GameObject> selectedUnits = new HashSet<GameObject>();

    const string LAYER_CASTLE = "Castle";
    const string LAYER_SOLDIER = "Soldier";
    const string LAYER_RESOURCE = "Resource";
    const string LAYER_FLOOR = "Floor";
    const string LAYER_UNIT_UI = "UI";

    // Start is called before the first frame update
    void Start()
    {
        initialCameraForward = playerCameraContainer.transform.forward;

        rightHand = playerCameraContainer.transform.GetChild(0).GetChild(2).gameObject;
        leftHand = playerCameraContainer.transform.GetChild(0).GetChild(1).gameObject;
       
        rectangularSelectionLine.positionCount = 5;
        blueCastle.GetComponent<BlueCastle>().HideSpawnRange();
    }

    // Update is called once per frame
    void Update()
    {
        anchorUIToLeftController();
        Vector3 raycastStart = rightHand.transform.position;
        //Vector3 raycastDir = (playerCameraContainer.transform.rotation * initialCameraForward).normalized;
        Vector3 raycastDir = rightHand.transform.forward.normalized;
        // just need to show if its true (pressed or held down)
        bool usingSelectInput = SteamVR_Input.GetBooleanAction("SelectInput").GetState(SteamVR_Input_Sources.RightHand);
        // only want to know if pressed
        bool usingCommandInput = SteamVR_Input.GetBooleanAction("CommandInput").GetStateDown(SteamVR_Input_Sources.RightHand);

        bool selectHeldDown = (usingSelectInput && !SteamVR_Input.GetBooleanAction("SelectInput").GetStateUp(SteamVR_Input_Sources.RightHand)
                                                && !SteamVR_Input.GetBooleanAction("SelectInput").GetStateDown(SteamVR_Input_Sources.RightHand));
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

                        if (currObj && currObj.layer == LayerMask.NameToLayer(LAYER_SOLDIER) && currObj.GetComponent<TeamColors>().IsBlueTeam()) // Hover blue soldiers
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
            else if(selectHeldDown) 
            {
                RaycastHit hit;
                Ray ray = new Ray(raycastStart + raycastOffset, raycastDir);
                if (!isDraggingSlider) 
                {
                    if (Physics.Raycast(ray, out hit, raycastLength, LayerMask.GetMask(LAYER_UNIT_UI))) 
                    {
                        sliderInitialPos = rightHand.transform.localPosition;
                        isDraggingSlider = true;
                    }
                }
                else 
                {
                    Vector3 deltaPos = calculateCurrFramePositionDelta(rightHand);
                    blueCastle.GetComponent<BlueCastle>().changeSliderValue(-deltaPos.x / 50.0f);
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
                if (Physics.Raycast(ray, out hit, raycastLength, LayerMask.GetMask(LAYER_UNIT_UI))) 
                {
                    //if (currObj.layer == LayerMask.NameToLayer(LAYER_UNIT_UI) && currObj.GetComponent<TeamColors>().IsBlueTeam()) // Select blue castle
                    //{
                    //    clearSelectedUnits();
                    //    dehoverUnit(currObj);
                    //    selectUnit(currObj);
                    //}
                }

                if (Physics.Raycast(ray, out hit, raycastLength, LayerMask.GetMask(LAYER_CASTLE, LAYER_SOLDIER, LAYER_FLOOR, LAYER_UNIT_UI)))
                {
                    GameObject currObj = hit.transform.gameObject;

                    if (currObj.layer == LayerMask.NameToLayer(LAYER_FLOOR)) // Deselect by clicking floor
                    {
                        clearSelectedUnits();
                    }
                    else if (currObj.layer == LayerMask.NameToLayer(LAYER_SOLDIER) && currObj.GetComponent<TeamColors>().IsBlueTeam()) // Select blue soldiers
                    {
                        currObj.GetComponents<AudioSource>()[0].Play();
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
            // release all boolean
            if (SteamVR_Input.GetBooleanAction("CommandInput").GetStateUp(SteamVR_Input_Sources.RightHand)) {
                isRectangularSelecting = false;
                isBlueCastleSelected = false;
                isUnitUISelected = false;
            }

        }
        else if (usingCommandInput)
        {
            RaycastHit hit;
            Ray ray = new Ray(raycastStart + raycastOffset, raycastDir);

            if(!isBlueCastleSelected && Physics.Raycast(ray, out hit, raycastLength, LayerMask.GetMask(LAYER_CASTLE))) {
                if(hit.transform.gameObject.GetComponent<TeamColors>().IsBlueTeam())
                    blueCastle.GetComponent<BlueCastle>().toggleBaseMenu();
            }

            if (Physics.Raycast(ray, out hit, raycastLength, LayerMask.GetMask(LAYER_UNIT_UI))) {
                blueCastle.GetComponent<BlueCastle>().toggleCurrentUnit();
            }

            if (isBlueCastleSelected && Physics.Raycast(ray, out hit, raycastLength, LayerMask.GetMask(LAYER_FLOOR)))
            {
                blueCastle.GetComponent<BlueCastle>().SpawnSoldierInRange(hit.point);
            }
            else if(Physics.Raycast(ray, out hit, raycastLength, LayerMask.GetMask(LAYER_CASTLE)))
            {
                if (hit.transform.gameObject.GetComponent<TeamColors>().IsRedTeam())
                {
                    AttackWithSelectedUnits(hit.transform.gameObject);
                }
            }
            else if (Physics.Raycast(ray, out hit, raycastLength, LayerMask.GetMask(LAYER_RESOURCE))) {
                GatherWithSelectedUnits(hit.transform.gameObject);
            }
            else if (Physics.Raycast(ray, out hit, raycastLength, LayerMask.GetMask(LAYER_SOLDIER))) {
                if (hit.transform.gameObject.GetComponent<TeamColors>().IsRedTeam()) {
                    AttackWithSelectedUnits(hit.transform.gameObject);
                }
            }
            else if (Physics.Raycast(ray, out hit, raycastLength, LayerMask.GetMask(LAYER_FLOOR)))
            {
                DefendWithSelectedUnits(hit.point);
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
                    hovered.GetComponents<AudioSource>()[0].Play();
                }

                clearHoveredUnits();
                foreach (GameObject hovered in toSelect) selectUnit(hovered);
            }
            else // Single unit hover
            {
                clearHoveredUnits();
                RaycastHit hit;
                Ray ray = new Ray(raycastStart + raycastOffset, raycastDir);
                if (Physics.Raycast(ray, out hit, raycastLength, LayerMask.GetMask(LAYER_CASTLE, LAYER_SOLDIER, LAYER_RESOURCE)))
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

        listenAndCastMeteor();
        listenAndExecuteFlying();
    }

    /*---------Unit Selection-----------*/

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

    public void selectUnit(GameObject unit)
    {
        unit.GetComponent<TeamColors>().SetDefaultSelectedMaterial();
        selectedUnits.Add(unit);
        if (unit.layer == LayerMask.NameToLayer(LAYER_CASTLE) && unit.GetComponent<TeamColors>().IsBlueTeam()) isBlueCastleSelected = true;
        if (unit.layer == LayerMask.NameToLayer(LAYER_UNIT_UI) && unit.GetComponent<TeamColors>().IsBlueTeam()) isUnitUISelected = true;
    }

    void deselectUnit(GameObject unit)
    {
        unit.GetComponent<TeamColors>().SetDefaultMaterial();
        selectedUnits.Remove(unit);
        if (unit.layer == LayerMask.NameToLayer(LAYER_CASTLE) && unit.GetComponent<TeamColors>().IsBlueTeam()) isBlueCastleSelected = false;
        if (unit.layer == LayerMask.NameToLayer(LAYER_UNIT_UI) && unit.GetComponent<TeamColors>().IsBlueTeam()) isUnitUISelected = false;
    }

    void clearSelectedUnits()
    {
        foreach (GameObject selected in selectedUnits) selected.GetComponent<TeamColors>().SetDefaultMaterial();
        selectedUnits.Clear();
        isBlueCastleSelected = false;
        isUnitUISelected = false;
    }

    /*---------Unit Commands-----------*/

    public void AttackWithSelectedUnits(GameObject target)
    {
        foreach (GameObject unit in selectedUnits)
        {
            if (unit && unit.layer == LayerMask.NameToLayer(LAYER_SOLDIER) && unit.GetComponent<TeamColors>().IsBlueTeam())
            {
                unit.GetComponent<Soldier>().Attack(target);
            }
        }
    }

    public void DefendWithSelectedUnits(Vector3 location)
    {
        List<Soldier> selectedSoldiers = new List<Soldier>();
        foreach (GameObject unit in selectedUnits)
        {
            if (unit && unit.layer == LayerMask.NameToLayer(LAYER_SOLDIER) && unit.GetComponent<TeamColors>().IsBlueTeam())
            {
                selectedSoldiers.Add(unit.GetComponent<Soldier>());
            }
        }
        
        // Square grid formation centered at location
        int numSoldiers = selectedSoldiers.Count;

        if (numSoldiers > 1)
        {
            int numRows = Mathf.FloorToInt(Mathf.Sqrt(numSoldiers));
            int numCols = Mathf.CeilToInt((float) numSoldiers / numRows);
            float soldierWidth = selectedSoldiers[0].gameObject.transform.localScale.x;
            float soldierLength = selectedSoldiers[0].gameObject.transform.localScale.z;
            float formationWidth = (numCols - 1) * (soldierWidth + soldierFormationSpacing);
            float formationLength = (numRows - 1) * (soldierLength + soldierFormationSpacing);
            Vector3 formationStart = new Vector3(location.x - formationWidth / 2.0f,
                                                 location.y,
                                                 location.z + formationLength / 2.0f);

            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < numCols; j++)
                {
                    int soldierIndex = i * numCols + j;
                    if (soldierIndex < numSoldiers)
                    {
                        Vector3 defenseLocation = new Vector3(formationStart.x + j * soldierFormationSpacing + soldierWidth,
                                                              formationStart.y,
                                                              formationStart.z - i * soldierFormationSpacing - soldierLength);
                        // selectedSoldiers[soldierIndex].Defend(defenseLocation);
                        selectedSoldiers[soldierIndex].Flee(defenseLocation); // Soldiers defend after fleeing to location
                    }
                }
            }
        }
        else if (numSoldiers == 1)
        {
            selectedSoldiers[0].Defend(location);
        }
    }

    public void GatherWithSelectedUnits(GameObject target)
    {
        foreach (GameObject unit in selectedUnits)
        {
            if (unit && unit.layer == LayerMask.NameToLayer(LAYER_SOLDIER) && unit.GetComponent<TeamColors>().IsBlueTeam())
            {
                unit.GetComponent<Soldier>().Gather(target, blueCastle);
            }
        }
    }

    /*---------Movement-----------*/

    private Vector3 calculateCurrFramePositionDelta(GameObject hand) {
        return initialPos - hand.transform.localPosition;
    }

    private Quaternion calculateCurrFrameRotationDelta(GameObject hand) {
        return initialRot * Quaternion.Inverse(hand.transform.localRotation);
    }

    private void flyToPositionWithRotation(Vector3 deltaPos, Quaternion deltaRot) {
        
        playerCameraContainer.transform.position += playerCameraContainer.transform.right * -(deltaPos.x / 2.0f) 
                                                    + playerCameraContainer.transform.up * -(deltaPos.y / 2.0f) 
                                                    + playerCameraContainer.transform.forward * -(deltaPos.z / 2.0f);
        deltaRot.x *= 0.0f;
        deltaRot.z *= 0.0f;
        deltaRot.y *= 0.1f;
        playerCameraContainer.transform.rotation = Quaternion.Inverse(Quaternion.Slerp(Quaternion.identity, deltaRot, 0.1f)) * playerCameraContainer.transform.rotation;
    }

    void listenForFlightModeTriggerAndSavePosition() {
        // Check if trigger value is 1.0 (pulled completely) and was not last active (first time triggered)
        //Debug.Log(SteamVR_Input.GetSingleAction("FlyingTrigger").GetAxis(SteamVR_Input_Sources.LeftHand));
        //Debug.Log(SteamVR_Input.GetSingleAction("FlyingTrigger").GetChanged(SteamVR_Input_Sources.LeftHand));
        if (SteamVR_Input.GetSingleAction("FlyingTrigger").GetAxis(SteamVR_Input_Sources.LeftHand) >= 1.0f
            && SteamVR_Input.GetSingleAction("FlyingTrigger").GetChanged(SteamVR_Input_Sources.LeftHand)) {
            
            initialPos = leftHand.transform.localPosition;
            initialRot = leftHand.transform.localRotation;
            isflightMode = true;
        } else if(isflightMode && SteamVR_Input.GetSingleAction("FlyingTrigger").GetAxis(SteamVR_Input_Sources.LeftHand) < 1.0f){
            isflightMode = false;    
        }

    }

    void listenAndExecuteFlying() {
        listenForFlightModeTriggerAndSavePosition();
        if(isflightMode) {
            Vector3 deltaPos = calculateCurrFramePositionDelta(leftHand);
            Quaternion deltaRot = calculateCurrFrameRotationDelta(leftHand);
            flyToPositionWithRotation(deltaPos, deltaRot);
        }
    }

    private void renderBezierCurve(Vector3 start, Vector3 end, Vector3 control) {
        LineRenderer rayRenderer = meteorRaycastLine.GetComponent<LineRenderer>();
        rayRenderer.positionCount = 50;
        Vector3[] positions = new Vector3[50];
        positions[0] = leftHand.transform.position;
        //first point should stay where the controller is, start i at 1

        //Instead of time since we want to do it 1 frame, just do it 50 times and have 50 positions in the ray renderer
        for (int i = 2; i < 50 + 1; i++) {
            //since t in lert should be between 0 and 1, must change i so its between 0 to 1
            float t = i / 50.0f;

            Vector3 q0 = Vector3.Lerp(start, control, t);
            Vector3 q1 = Vector3.Lerp(control, end, t);
            Vector3 q2 = Vector3.Lerp(q0, q1, t);

            positions[i - 1] = q2;
        }

        rayRenderer.SetPositions(positions);
    }

    void listenAndCastMeteor() {
        Vector3 rayEndPos = meteorRaycastLine.GetComponent<LineRenderer>().GetPosition(1);
        if (SteamVR_Input.GetBooleanAction("CastMeteor").GetStateDown(SteamVR_Input_Sources.LeftHand)) {
            Debug.Log("pressed down");
            isCastingMeteor = true;
            meteorRaycastLine.GetComponent<LineRenderer>().enabled = true;
        }
        if (isCastingMeteor) {
            Vector3 endPos = meteorRaycastLine.GetComponent<LineRenderer>().GetPosition(1);
            //half way along the ray?
            Vector3 controlPos = leftHandRay.GetPoint(100.0f / 2);
            endPos.y = 0.0f;
            controlPos.y += 25.0f;
            renderBezierCurve(leftHandRay.origin, endPos, controlPos);
            // Set the flag, so the moment the trigger isn't held we can teleport
            rayEndPos = endPos;
        }
        if (SteamVR_Input.GetBooleanAction("CastMeteor").GetStateUp(SteamVR_Input_Sources.LeftHand)) {
            Debug.Log("released");
            blueCastle.GetComponent<BlueCastle>().UseMeteor(rayEndPos, blueCastle.GetComponent<BlueCastle>().getForce());
            isCastingMeteor = false;
            meteorRaycastLine.GetComponent<LineRenderer>().enabled = false;
        }
    }

    /*----------VR UI-------------*/
    void anchorUIToLeftController() {
        leftControllerUI.transform.position = new Vector3(leftHand.transform.position.x, 
                                                          leftHand.transform.position.y + 1.0f, 
                                                          leftHand.transform.position.z);
        leftControllerUI.transform.rotation = leftHand.transform.rotation;

        //meteorRaycastLine.setPosition(0, leftHand.transform.position);
        leftHandRay = new Ray(leftHand.transform.position, leftHand.transform.forward);
        meteorRaycastLine.GetComponent<LineRenderer>().positionCount = 2;
        meteorRaycastLine.GetComponent<LineRenderer>().SetPosition(0, leftHand.transform.position);
        meteorRaycastLine.GetComponent<LineRenderer>().SetPosition(1, leftHandRay.GetPoint(100.0f));
        meteorRaycastLine.GetComponent<LineRenderer>().enabled = isCastingMeteor;
    }
}
