using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.VFX;

public class FollowCam : MonoBehaviour
{
    static private FollowCam S; 
    static public GameObject POI; // point of interest

    public enum eView {  none, slingshot, castle, both };

    [Header("Inscribed")]
    public float easing = 0.05f;
    public Vector2 minXY = Vector2.zero;
    public GameObject viewBothGO;

    [Header("Dynamic")]
    private float camZ;
    public eView nextView = eView.slingshot;

    private void Awake()
    {
        S = this;

        camZ = this.transform.position.z;
    }

    private void FixedUpdate()
    {
        Vector3 destination = Vector3.zero;

        if (POI != null)
        {
            Rigidbody poiRigid = POI.GetComponent<Rigidbody>();
            if ((poiRigid != null) && poiRigid.IsSleeping())
            {
                POI = null;
            }
        }

        if (POI != null)
        {
            destination = POI.transform.position;
        }

        destination.x = Mathf.Max(minXY.x, destination.x);
        destination.y = Mathf.Max(minXY.y, destination.y);
        destination = Vector3.Lerp(transform.position, destination, easing);

        destination.z = camZ;
        transform.position = destination;

        Camera.main.orthographicSize = destination.y + 10;
    }

    public void SwitchView(eView newView)
    {
        if (newView == eView.none)
        {
            newView = nextView;
        }

        switch(newView) {
            case eView.slingshot:
                POI = null;
                print("poi is null (slingshot view)");
                nextView = eView.castle;
                break;
            case eView.castle:
                POI = MissionDemolition.GET_CASTLE();
                print("poi is castle: " + POI.transform.position);
                nextView = eView.both;
                break;
            case eView.both:
                POI = viewBothGO;
                print("poi is viewbothgo: " + POI.transform.position);
                nextView = eView.slingshot;
                break;
        }
    }

    public void SwitchView()
    {
        SwitchView(eView.none);
    }

    static public void SWITCH_VIEW(eView newView)
    {
        S.SwitchView(newView);
    }
}
