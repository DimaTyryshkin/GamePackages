using GamePackages.Core;
using UnityEngine;

namespace GamePackages.Audio
{
	public class AppSoundsTest : MonoBehaviour
	{
		[SerializeField] AudioSource audioSource;

		[SerializeField] float period;

		[SerializeField] bool test1;

		float timeNextPlay;



		void Update()
		{
			if (test1)
				Test1();
			else
				Test2();
		}

		void Test1()
		{
			if(Time.time> timeNextPlay)
			{
				AudioSource newAudioSource = transform.InstantiateAsChild(audioSource);
				newAudioSource.Play();

				timeNextPlay = Time.time + period;
			}
		}


		AudioSource oldAudioSource;
		void Test2()
		{
			if(Time.time> timeNextPlay)
			{
				AudioSource newAudioSource = transform.InstantiateAsChild(audioSource);
				newAudioSource.Play();

                
				if(oldAudioSource)
					DestroyImmediate(oldAudioSource.gameObject);
                
				oldAudioSource = newAudioSource;

				timeNextPlay = Time.time + period;
			}
		}
	}
}