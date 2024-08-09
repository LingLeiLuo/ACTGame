using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TimelineItem : MonoBehaviour
{
    //public PlayableDirector PlayableDirector;

    public void PlayTimeline(PlayableDirector playableDirector)
    {
        playableDirector.Play();
    }
    private void Start()
    {
        //gameObject.transform.SetParent(GameObject.Find("WorldSpaceRoot").transform);
    }
    private void Update()
    {
        
    }

}
