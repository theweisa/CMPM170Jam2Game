using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteScript : MonoBehaviour
{
    private GameObject planet;
    private PlanetScript planetScript;
    private Transform spawnPoints;
    private AIEnemy enemyScript;
    public bool spawnNewNote = true; 

    // Start is called before the first frame update
    void Start()
    {
        planet = GameObject.Find("Planet");
        planetScript = planet.GetComponent<PlanetScript>();
        spawnPoints = planet.transform.Find("NoteSpawnPoints");
        enemyScript = GameObject.Find("Enemy").GetComponent<AIEnemy>();
        InitRotation();
    }

    void InitRotation() {
        Vector3 gravityUp = (transform.position-planet.transform.position).normalized;
        transform.rotation = Quaternion.FromToRotation(transform.up, gravityUp)*transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void spawnNote() {
        Transform spawnPos = spawnPoints.transform.Find($"NoteSpawn{planetScript.GetShrinkCounter()}");
        if (!spawnPos) {
            Destroy(this.gameObject);
            return;
        }
        GameObject newNote = Instantiate(gameObject, spawnPos);
        newNote.SetActive(true);
        newNote.transform.position = spawnPos.position;
        Destroy(this.gameObject);
    }
    
    private void OnTriggerEnter(Collider obj) {
        if (obj.tag == "Player") {
            print("collected");
            gameObject.SetActive(false);
            LTDescr done = planetScript.ShrinkPlanet();
            enemyScript.IncreaseChaseLevel();
            if (spawnNewNote) {
                done.setOnComplete(spawnNote);
            }
        }
    }
}
