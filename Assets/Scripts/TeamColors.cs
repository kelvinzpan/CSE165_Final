using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamColors : MonoBehaviour
{
    enum Teams { Blue, Red };

    public Material blueMaterial;
    public Material blueHoverMaterial;
    public Material blueSelectedMaterial;
    public Material redMaterial;
    public Material redHoverMaterial;
    public Material redSelectedMaterial;

    private Teams team;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetBlueTeam()
    {
        team = Teams.Blue;
        SetBlueMaterial();
    }

    public void SetRedTeam()
    {
        team = Teams.Red;
        SetRedMaterial();
    }

    public bool IsBlueTeam()
    {
        return (team == Teams.Blue);
    }

    public bool IsRedTeam()
    {
        return (team == Teams.Red);
    }

    public void SetBlueMaterial()
    {
        MeshRenderer renderer = this.GetComponent<MeshRenderer>();
        renderer.material = blueMaterial;
    }

    public void SetBlueHoverMaterial()
    {
        MeshRenderer renderer = this.GetComponent<MeshRenderer>();
        renderer.material = blueHoverMaterial;
    }

    public void SetBlueSelectedMaterial()
    {
        MeshRenderer renderer = this.GetComponent<MeshRenderer>();
        renderer.material = blueSelectedMaterial;
    }

    public void SetRedMaterial()
    {
        MeshRenderer renderer = this.GetComponent<MeshRenderer>();
        renderer.material = redMaterial;
    }

    public void SetRedHoverMaterial()
    {
        MeshRenderer renderer = this.GetComponent<MeshRenderer>();
        renderer.material = redHoverMaterial;
    }

    public void SetRedSelectedMaterial()
    {
        MeshRenderer renderer = this.GetComponent<MeshRenderer>();
        renderer.material = redSelectedMaterial;
    }
}
