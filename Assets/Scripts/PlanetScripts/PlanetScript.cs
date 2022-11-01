using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetScript : MonoBehaviour
{
    private float shrink_factor = 0.05f;
    private float min_size = 0.5f;
    private float transition_timer = 1f;

    private Vector3 baseScale;
    private int shrink_counter = 0;
    // Start is called before the first frame update
    void Start()
    {
        baseScale = gameObject.transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public LTDescr ShrinkPlanet()
    {
        if (1f-(shrink_factor*(shrink_counter+1)) < min_size) return null;
        shrink_counter++;
        Vector3 newScale = baseScale * (1f-(shrink_factor*shrink_counter));
        return LeanTween.scale(gameObject, newScale, transition_timer);
        //return LeanTween.descr(id);
    }

    public int GetShrinkCounter() {
        return shrink_counter;
    }
}
