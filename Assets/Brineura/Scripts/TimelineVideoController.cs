using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Video;

[Serializable] public struct VideoStruct
{
    public VideoClip video;
    public float startTime;
    public float endTime;
}


public class TimelineVideoController : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    private PlayableDirector playableDirector;

    public List<VideoStruct> videoList = new List<VideoStruct>();

    private bool isPlaying = false;
    private bool waitingToSeekFrame = false;
    private bool seekReady = false;

    private int frameRate;
    private int currentIndex;

    private void Start()
    {
        playableDirector = GetComponentInParent<PlayableDirector>();

        videoPlayer.seekCompleted += SeekCompleted;
    }

    private void SeekCompleted(VideoPlayer source)
    {
        seekReady = true;
    }

    private void Update()
    {
        if (!isPlaying)
        {
            PlayCheck();
        }
        if (isPlaying)
        {
            StopCheck();
        }
    }

    private void StopCheck()
    {
        double currentTime = playableDirector.time;
        if (currentTime < videoList[currentIndex].startTime || currentTime > videoList[currentIndex].endTime)
        {
            StopClip();
        }
    }

    private void StopClip()
    {
        Debug.Log("Stop Clip");
        isPlaying = false;
    }

    private void PlayCheck()
    {
        double currentTime = playableDirector.time;
        for (int i = 0; i < videoList.Count; i++)
        {
            if (currentTime >= videoList[i].startTime && currentTime <= videoList[i].endTime)
            {
                PlayClip(i, currentTime);
            }
        }
    }

    private void PlayClip(int index, double time)
    {
        Debug.Log("Play Clip");
        videoPlayer.clip = videoList[index].video;
        videoPlayer.Play();
        isPlaying = true;
        currentIndex = index;
        ChangeTime(time);
        frameRate = Mathf.RoundToInt((float)videoPlayer.frameRate);
    }

    public void ChangeTime(double time)
    {
        if (!isPlaying) return;
        if (waitingToSeekFrame) return;
        StartCoroutine(SetTimeCoroutine(time));
    }

    public void PauseVideo()
    {
        if (!isPlaying) return;
        videoPlayer.Pause();
    }

    public void PlayVideo()
    {
        if (!isPlaying) return;
        videoPlayer.Play();
    }

    public IEnumerator SetTimeCoroutine(double time)
    {
        waitingToSeekFrame = true;
        seekReady = false;
        videoPlayer.time = time - videoList[currentIndex].startTime;
        while (waitingToSeekFrame)
        {
            if (seekReady)
            {
                waitingToSeekFrame = false;
                StopAllCoroutines();
            }
            yield return null;
        }
        yield return null;
    }

}