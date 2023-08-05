using UnityEngine;

public class CompassControl : MonoBehaviour
{
    public Camera camMinimap;   //assigned to cameraMinimap (object) and its zoom
    public GameObject objMinimap;   //assigned to minimap associated with canvas...?
    public GameObject objCoord;     //manipulates compass coordinates
    public GameObject objFPSControl;    //assigned to "FPSControler"
    public CompassInfo[] compassSetup;      //references CompassInfo script. comp-ass.

    [Range(0, 3)]
    public int startZoomLv;  //controls initial zoom level. whatever that means.
    public bool isTransparent;  //toggles minimap transparence on/off

    [Range(0, 1)]
    public float renderAlpha;    //renders transparence
    public int currZoom;        //takes current zoom level. whatever...that means, again.
    // Start is called before the first frame update
    void Start()
    {
        currZoom = startZoomLv;
        SetMapState();  //sets map state ig
        AlphaLevel();   //controls alpha level
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha4)){
            SetMapState();
        }
        if (Input.GetKeyDown(KeyCode.Alpha5)){
            isTransparent = !isTransparent;
            AlphaLevel();
        }
    }

    void LateUpdate()
    {
        if (camMinimap.enabled){
            Quaternion quatRotate = new Quaternion();
            Vector3 vectGuide = new Vector3();

            vectGuide.z = objFPSControl.transform.rotation.eulerAngles.y;
            objCoord.transform.rotation = quatRotate;
        }
    }

    void SetMapState(){
        //tests if compassSetup level is within range
        if (currZoom - 1 < compassSetup.Length){
            if(currZoom != 0){
                ViewMinimap(true);
                camMinimap.orthographicSize = compassSetup[currZoom - 1].zoomLv;
            }
            else{ ViewMinimap(false); }
            currZoom++;
        }
        else{
            currZoom = 1;
            ViewMinimap(false);
        }
    }

    void AlphaLevel(){
        if(isTransparent == true)
        {
            objMinimap.GetComponent<CanvasGroup>().alpha = renderAlpha;
        }
        else
        {
            objMinimap.GetComponent<CanvasGroup>().alpha = 1;
            isTransparent = false;
        }
    }

    void ViewMinimap(bool state)
    {
        objMinimap.SetActive(state);    //changes map visibility
        camMinimap.enabled = state;
    }
}
