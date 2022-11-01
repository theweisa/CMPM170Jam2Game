using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteScript : MonoBehaviour
{
    public PlanetScript planet;
    public Transform spawnPoints;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //transform.position = spawnPoints.transform.Find($"NoteSpawn{planet.GetShrinkCounter()}").position;
    }

    private void spawnNote() {
        Transform spawnPos = spawnPoints.transform.Find($"NoteSpawn{planet.GetShrinkCounter()}");
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
            LTDescr done = planet.ShrinkPlanet();
            done.setOnComplete(spawnNote);
        }
    }
}
