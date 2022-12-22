using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class Timer : MonoBehaviour
{
	public UnityEvent timesUpEvent;

	[SerializeField]
	private float timerDuration = 180;
	private float timeRemainingInSecond = 180;
	public float ElapsedTime { get { return timerDuration - timeRemainingInSecond; } }

	[SerializeField]
	private Text timerText = null;

	private bool isActive = true;

	private void Start()
	{
		timeRemainingInSecond = timerDuration;
	}

	// Update is called once per frame
	void Update()
	{
		if (isActive && timeRemainingInSecond > 0)
		{
			timeRemainingInSecond -= Time.deltaTime;

			int minutes = (int)(timeRemainingInSecond / 60);
			int seconds = (int)timeRemainingInSecond % 60;

			timerText.text = minutes + " : ";

			if (seconds < 10)
				timerText.text += "0";

			timerText.text += seconds;

			if (timeRemainingInSecond <= 0)
			{
				Debug.Log("Time's Up !");
				timesUpEvent.Invoke();
			}
		}
	}

	public void Stop()
	{
		isActive = false;
	}
}
