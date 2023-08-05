using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerControl : MonoBehaviour
{
    //public variables. I actually warrant the need of this header
    //because of all the tooltips lol
    [Tooltip("Esse variável receberá o FPSController")]
    public FirstPersonController fpsControl;
    [Tooltip("Recebe a câmera que será usada no método mirar")]
    public Camera mainCamera;
    //variables for object transform manipulation
    [Tooltip("walk(Andar)")]
    public Transform trfWalk;
    [Tooltip("toJump(Pular)")]
    public Transform trfJump;
    [Tooltip("turnBack(Retornar)")]
    public Transform trfBack;
    [Tooltip("retreat(Recuo)")]
    public Transform trfRetreat;
    [Tooltip("toAim(Mirar)")]
    public Transform trfAim;

    [Tooltip("Essa variável determina a arma que está em uso")]
    public int intWeaponInUse;
    [Tooltip("Essa variáve (Array) guardará a lista de armas (Arsenal)")]
    public int[] vIntWeaponList;
    [Tooltip("Essa variável guardará o estado de saúde dp player")]
    public float playerHP;
    [Tooltip("Essa variável guarda o estado da arma empunhada. " +
             "Se empunhada ou não.")]
    public bool blHandleState;
    [Tooltip("Essa variável será usada para determinar a arma empunhada")]
    public int intWeaponWielded = -1;

    [Tooltip("Recebe as informações da saúde do player.")]  //i hate the word "saúde" in Portuguese to refer to health...
    //even though it is the correct translation I hate it
    public TMP_Text textHP;

    [Tooltip("Recebe as informações da munição do player.")]
    public TMP_Text textAmmo;
    public TMP_Text textBreath;

    [Tooltip("Cria um lista com as variáveis do Script WeaponInfo")]
    public WeaponInfo[] vWinfAboutWeapon;
    public WeaponInfo winfCurrentWeapon;

    [Tooltip("Essa variável definirá o tempo entre o disparo e o contato")]
    public float fltContactTime;

    public RawImage rawContactPoint;
    public bool isShowMouse;
    public GameObject objPauseGame;

    //private variables
    Vector3[] vectorRetreat = new Vector3[4];  //a list of all weapon recoil positions

    //regen / gameover
    bool isGameOver = false;
    public int regenTime = 0;

    //underwater mechanics
    float breathTime;
    int maxBreath = 1200;
    int drownTime = 0;

    // Start is called before the first frame update
    void Awake()
    {
        breathTime = maxBreath;
        ChangeWeapon(0);    //method for changing weapons.
    }

    // Update is called once per frame
    void Update()
    {
        ControlShot();      //manages shots
        DisplayMouse();     //displays mouse cursor or not

        if(playerHP <= 0){
            playerHP = 0;
            isGameOver = true;
        }

        if (Input.GetKeyDown("KeyCode.Esc") || Input.GetKeyDown("KeyCode.P"))
        {
            objPauseGame.SetActive(!objPauseGame.activeInHierarchy);
        }
    }

    void FixedUpdate()
    {
        AnimationControl(); //manages animations
        IndentControl();    //contrary to what the name suggests, controls weapon recoil
        ControlAim();       //manages weapon aim

        regenTime--;
        if(playerHP <= 100 && regenTime <= 0 && isGameOver == false){
            playerHP += 0.3f;
            regenTime = 60;
        }
        if (playerHP > 100) { playerHP = 100; }

        HoldBreath();
    }

    void LateUpdate()
    {
        ControlAmmunition();
        InfoScreen();   //method to display info in use.
        ControlGameOver();  //game over time!
    }

    void HoldBreath()
    {
        if (this.transform.position.y <= 6.5)
        {
            breathTime--;
            if(breathTime <= 0){
                if(drownTime == 0){
                    playerHP -= 7.5f;
                    drownTime = 35;
                    regenTime = 300;
                }
                drownTime--;
                breathTime = 0;
            }
        }
        else {
            breathTime+=2;
        }
        if(breathTime < maxBreath){
            textBreath.gameObject.SetActive(true);
        }

        if (breathTime >= maxBreath){
            breathTime = maxBreath;
            textBreath.gameObject.SetActive(false);
        }
    }

    void ChangeWeapon(int weapon)
    {
        //Arma em uso
        intWeaponInUse = weapon;
        //Informaçõs sobre a arma atualmente em uso
        winfCurrentWeapon = vWinfAboutWeapon[vIntWeaponList[intWeaponInUse]];
        ControlWeapon();
    }

    //Método que chamaa animação para esconder ou exibir a Arma
    void ControlWeapon()
    {
        HideWeapons();
        DisplayWeapon();

        trfBack.GetComponent<Animation>().Play("raiseWeapon");
        //Arma não empunhada
        blHandleState = false;
    }

    void HideWeapons()  //hides all weapons except the one being held/equipped
    {
        for (int i = 0; i < vWinfAboutWeapon.Length; i++)
        {
            if (i != intWeaponInUse)
            {
                vWinfAboutWeapon[vIntWeaponList[i]].trfWeapon.gameObject.SetActive(false);
            }
        }
        
    }

    void DisplayWeapon()    //displays equipped weapon
    {
        vWinfAboutWeapon[vIntWeaponList[intWeaponInUse]].trfWeapon.gameObject.SetActive(true);
    }

    void ControlAmmunition()    //manages ammo shenanigans
    {
        if (winfCurrentWeapon.currentAmmo < 0)
        {
            winfCurrentWeapon.currentAmmo = 0;
        }

        if (winfCurrentWeapon.maxExtraAmmo < 0)
        {
            winfCurrentWeapon.maxExtraAmmo = 0;
        }

        if (winfCurrentWeapon.currentAmmo > winfCurrentWeapon.maxAmmo)
        {
            winfCurrentWeapon.currentAmmo = winfCurrentWeapon.maxAmmo;
        }

        if (winfCurrentWeapon.extraAmmo > winfCurrentWeapon.maxExtraAmmo)
        {
            winfCurrentWeapon.extraAmmo = winfCurrentWeapon.maxExtraAmmo;
        }


        if (Input.GetButtonDown("Reload") && winfCurrentWeapon.canReload == false &&
            winfCurrentWeapon.currentAmmo < winfCurrentWeapon.maxAmmo &&
            winfCurrentWeapon.extraAmmo > 0)
        {
            ReloadWeapon();
        }

        if (Input.GetButtonDown("Fire1") && winfCurrentWeapon.canReload == false && winfCurrentWeapon.currentAmmo == 0 &&
            winfCurrentWeapon.extraAmmo > 0)
        {
            ReloadWeapon();
        }

        if (winfCurrentWeapon.canReload &&
            trfBack.GetComponent<Animation>().isPlaying == false)
        {
            if (winfCurrentWeapon.maxExtraAmmo >= (winfCurrentWeapon.maxAmmo - winfCurrentWeapon.currentAmmo))
            {
                int ammo = winfCurrentWeapon.maxAmmo - winfCurrentWeapon.currentAmmo;
                winfCurrentWeapon.maxExtraAmmo -= ammo;
                winfCurrentWeapon.currentAmmo += ammo;
            } else
            {
                winfCurrentWeapon.currentAmmo += winfCurrentWeapon.maxExtraAmmo;
                winfCurrentWeapon.maxExtraAmmo = 0;
            }

            winfCurrentWeapon.canReload = false;
        }
    }

    void ReloadWeapon() //plays reload animation
    {
        winfCurrentWeapon.canReload = true;
        trfBack.GetComponent<Animation>().Play("reload");
        GetComponent<AudioSource>().PlayOneShot(winfCurrentWeapon.sfxReload);
    }

    void ControlShot()  //controls shots xd
    {
        if(winfCurrentWeapon.autoShot == true)
        {
            if (Input.GetButton("Fire1") && winfCurrentWeapon.canReload == false && winfCurrentWeapon.currentAmmo > 0)
            {
                ShootTheBullet();   //:)
            }
        }
        else
        {
            if (Input.GetButtonDown("Fire1") && winfCurrentWeapon.canReload == false && winfCurrentWeapon.currentAmmo > 0)
            {
                ShootTheBullet();
            }
        }
    }

    void ShootTheBullet()   //shoots the bullet
    {
        if(winfCurrentWeapon.delayNextShot <= Time.time)
        {
            //[1] stores a random Y aim offset for weapon recoil
            vectorRetreat[1] += new Vector3(winfCurrentWeapon.wpnRotate.x,
                Random.Range(-winfCurrentWeapon.wpnRotate.y, winfCurrentWeapon.wpnRetreat.y), winfCurrentWeapon.wpnRotate.z);
            //[3] stores a random X, Y position for weapon recoil
            vectorRetreat[3] += new Vector3(Random.Range(-winfCurrentWeapon.wpnRotate.x, winfCurrentWeapon.wpnRetreat.x),
                Random.Range(-winfCurrentWeapon.wpnRotate.y, winfCurrentWeapon.wpnRetreat.y), winfCurrentWeapon.wpnRotate.z);

            //Debug.Log("método tiro kkkk: " + winfCurrentWeapon.strName); //debugging weapon name for its expected behavior.

            Instantiate(winfCurrentWeapon.objShot, trfRetreat.position, trfRetreat.rotation);
            winfCurrentWeapon.currentAmmo -= 1;
            winfCurrentWeapon.delayNextShot = Time.time + winfCurrentWeapon.shootTime;
        }
    }

    void AnimationControl() //method for animation management
    {
        //press 1 to equip weapon 1. simple enough!
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            intWeaponWielded = 0;
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            intWeaponWielded = 1;
        }

        if (intWeaponWielded != -1 && intWeaponWielded != intWeaponInUse && blHandleState == false)
        {
            blHandleState = true;
            trfBack.GetComponent<Animation>().Play("dropWeapon");
        }
        if(blHandleState == true && trfBack.GetComponent<Animation>().isPlaying == false)
        {
            ChangeWeapon(intWeaponWielded);
        }
    }

    void IndentControl()    //simulates recoil
    {
        //Lerp method. 
        vectorRetreat[0] = Vector3.Lerp(vectorRetreat[0], Vector3.zero, 0.1f);
        vectorRetreat[1] = Vector3.Lerp(vectorRetreat[1], vectorRetreat[0], 0.1f);
        vectorRetreat[2] = Vector3.Lerp(vectorRetreat[2], Vector3.zero, 0.1f);
        vectorRetreat[3] = Vector3.Lerp(vectorRetreat[3], vectorRetreat[2], 0.1f);

        trfRetreat.localEulerAngles = vectorRetreat[1];
        trfRetreat.localPosition = vectorRetreat[3];
    }

    void ControlAim()   //simulates aim
    {
        if (Input.GetButton("Fire2"))
        {
            trfAim.localPosition = Vector3.Lerp(trfAim.localPosition, winfCurrentWeapon.posCrosshair, 0.25f);
            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, 40f, 0.25f);
        }
        else
        {
            trfAim.localPosition = Vector3.Lerp(trfAim.localPosition, Vector3.zero, 0.25f);
            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, 60f, 0.25f);
        }
    }

    void InfoScreen()
    {
        textAmmo.text = "Ammo: " + winfCurrentWeapon.currentAmmo + "/" + winfCurrentWeapon.extraAmmo;
        textHP.text = "HP: " + playerHP + "%";
        textBreath.text = "Breath: " + Mathf.Round((breathTime/maxBreath)*100) + "%";
        if (fltContactTime < Time.time){
            rawContactPoint.gameObject.SetActive(false);
        }
        else{
            rawContactPoint.gameObject.SetActive(true);
        }
    }

    void DisplayMouse()
    {
        if (Input.GetKey(KeyCode.Tab)){
            isShowMouse = !isShowMouse;
            if (isShowMouse){
                Cursor.lockState = CursorLockMode.None;
            }
            else{
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
        Cursor.visible = isShowMouse;
    }

    public void ControlGameOver()
    {
        if(isGameOver == true){
            SceneManager.LoadScene("EndGame");
        }
    }
}


