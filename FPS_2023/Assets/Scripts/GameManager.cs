using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject prefabZombie;
    public GameObject pauseMenu;

    public float firstGen = 0;
    public float waitGen = 0;
    public float waitback;
    public int genZombie;
    public int returnedInt;

    public int maxZombie;
    public int currZombie = 0;
    public TMP_Text textReturn;

    // Start is called before the first frame update
    void Start()
    {
        returnedInt = 0;
        CalcZombieNumber();
        genZombie = currZombie;
    }

    // Update is called once per frame
    void Update()
    {
        if (pauseMenu.activeInHierarchy)
        {
            Time.timeScale = 0;
        }
        else { Time.timeScale = 1;   }

        if (currZombie <= maxZombie && genZombie < maxZombie
            && firstGen <= Time.time && waitback <= Time.time)
        {
            SpawnZombie();
            firstGen = Time.time + waitGen;
        }

        if (genZombie == maxZombie && currZombie == 0){
            waitback = Time.time + 0.3f;
            genZombie = 0;
            currZombie = 0;
            returnedInt++;
        }

        textReturn.text = "Zombies: " + currZombie.ToString();
        ItEnded();
    }

    void CalcZombieNumber()
    {
        maxZombie = Random.Range(1, returnedInt * 2);
    }

    public void KillZombie()
    {
        currZombie--;
    }

    void SpawnZombie()
    {
        currZombie++;
        genZombie++;
        float posX = Random.Range(80, 420);
        float posY = 0f;
        float posZ = Random.Range(80, 420);

        Vector3 createZombie = new Vector3(posX, posY, posZ);
        createZombie.y = Terrain.activeTerrain.SampleHeight(createZombie) + Terrain.activeTerrain.GetPosition().y;

        Instantiate(prefabZombie, new Vector3(posX, createZombie.y + 0.15f, posZ), Quaternion.identity);
    }

    public void ItEnded()
    {
        if(returnedInt > 4)
        {
            SceneManager.LoadScene("YouWin");
        }
    }
}
