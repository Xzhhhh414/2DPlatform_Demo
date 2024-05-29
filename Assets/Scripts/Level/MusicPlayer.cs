using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    //public AudioSource introSource;
    public AudioSource loopSource;

    // Start is called before the first frame update
    void Start()
    {
        //introSource.Play();
        //loopSource.PlayScheduled(AudioSettings.dspTime + introSource.clip.length);

        loopSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
