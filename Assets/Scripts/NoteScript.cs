using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoteScript : MonoBehaviour
{
    public Canvas CV;

    private GameObject planet;
    private GameObject player;
    private PlanetScript planetScript;
    private Transform spawnPoints;
    private AIEnemy enemyScript;
    public bool spawnNewNote = true; 
    private float rotateSpeed = 200f;
    private float dist;
    private float detectDist = 50f;

    public AudioSource noteDetectSfx;
    public AudioSource notePickupSfx;
    public AudioSource earthShakeSfx;
    public AudioSource bgm;

    // Start is called before the first frame update
    void Start()
    {   
        player = GameObject.Find("Player");
        planet = GameObject.Find("Planet");   
        planetScript = planet.GetComponent<PlanetScript>();
        spawnPoints = planet.transform.Find("NoteSpawnPoints");
        enemyScript = GameObject.Find("Enemy").GetComponent<AIEnemy>();
        TweenUp();
        InitRotation();
    }

    void InitRotation() {
        Vector3 gravityUp = (transform.position-planet.transform.position).normalized;
        transform.rotation = Quaternion.FromToRotation(transform.up, gravityUp)*transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0f, rotateSpeed*Time.deltaTime, 0f);

        dist = Vector3.Distance(transform.position, player.transform.position);
        if (dist < detectDist) {
            if (!noteDetectSfx.isPlaying) noteDetectSfx.Play();
            noteDetectSfx.volume = (detectDist-dist)/detectDist;
        }
        else {
            if (noteDetectSfx.isPlaying) noteDetectSfx.Stop();
        }
        // rotate the note
    }

    void TweenUp() {
        LTDescr up = LeanTween.move(gameObject, transform.position+transform.up*0.55f, 2f).setEase(LeanTweenType.easeInOutCubic);
        up.setOnComplete(TweenDown);
        //return up;
    }

    void TweenDown() {
        LTDescr down = LeanTween.move(gameObject, transform.position-transform.up*0.55f, 2f).setEase(LeanTweenType.easeInOutCubic);
        down.setOnComplete(TweenUp);
        //return down;
    }

    public void spawnNote() {
        int spawnPoint = planetScript.GetShrinkCounter();
        Transform spawnPos = spawnPoints.transform.Find($"NoteSpawn{spawnPoint}");
        if (!spawnPos) {
            Destroy(this.gameObject);
            GameWon();
            return;
        }
        print($"spawn point: {spawnPoint}");
        GameObject newNote = Instantiate(this.gameObject, spawnPos);
        newNote.SetActive(true);
        newNote.transform.position = spawnPos.position;
        Destroy(this.gameObject);
    }
    
    private void OnTriggerEnter(Collider obj) {
        if (obj.tag == "Player") {
            print("collected");
            notePickupSfx.Play();
            earthShakeSfx.Play();
            if (noteDetectSfx.isPlaying) noteDetectSfx.Stop();
            bgm.pitch *= 0.9f;
            RenderSettings.ambientIntensity -= 0.2f;
            RenderSettings.reflectionIntensity -= 0.2f;
            gameObject.SetActive(false);
            LTDescr done = planetScript.ShrinkPlanet();
            enemyScript.IncreaseChaseLevel();
            if (spawnNewNote) {
                done.setOnComplete(spawnNote);
            }
        }
    }


    // code for when you win the game
    private void GameWon() {
        //bgm.pitch = 1f;
        bgm.mute = true;
        RenderSettings.ambientIntensity = 1f;
        RenderSettings.reflectionIntensity = 1f;

        // type code for winning game transition
        GameObject.FindGameObjectWithTag("Enemy").GetComponent<AIEnemy>().GameWin();
    }
}
