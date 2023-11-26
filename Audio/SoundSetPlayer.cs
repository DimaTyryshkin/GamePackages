using GamePackages.Core.Validation;
using UnityEngine;
using UnityEngine.Assertions;

namespace GamePackages.Audio
{
	public class SoundSetPlayerHandler
	{
		AppSounds appSound;
		SoundSetPlayer player;
		SoundsSet skidSound;

		public void SetFollowTarget(Transform target)
		{
			if(player)            
				player.SetFollowTarget(target);
		}
		
		public void SetWorldPosition(Vector3 position)
		{
			if(player)            
				player.SetWorldPosition(position);
		}
		
		public void SetVolume(float volume)
		{
			if(player)            
				player.SetLocalVolume(volume);
		}
		
		// void SetEnable(bool isEnable)
		// {
		// 	if isEnable && !player
		// 	player = AppSound.GetPlayer(skidSound)
		// 	player.PlayIn();
		//
		// 	if !isEnable && player
		// 	player.PlayOut()
		// 	player = null
		// }
	}

	public class SoundSetPlayer : MonoBehaviour
	{
		[SerializeField, IsntNull] AudioSource audioSource;

		AppSounds appSounds;
		SoundsSet soundsSet;
		AudioClipWrapper audioClipWrapper;
		Transform followTarget;
		float deathTime;
		float localVolume;
		float typeVolume;
		float audioClipWrapperVolume;

		public float LastClipLenght { get; private set; }
		public bool AutoReplay { get; set; }
		public bool IsDontDestroyOnLoad => soundsSet.dontDestroyOnload;
		public string LastClipName => audioClipWrapper.AudioClip.name;
		public bool IsPlaying => gameObject.activeSelf;

		void Update()
		{
			if (followTarget)
				transform.position = followTarget.position;

			if (Time.realtimeSinceStartup > deathTime)
			{
				if (AutoReplay)
				{
					audioClipWrapper = soundsSet.NextClip();
					Play();
				}
				else
				{
					Stop();
				}
			}
		}

		public void SetSoundSet(AppSounds appSounds, SoundsSet soundsSet)
		{
			Assert.IsNotNull(soundsSet.AudioMixerGroup);
			Assert.IsNotNull(soundsSet);
			Assert.IsNotNull(appSounds);
            
			this.soundsSet = soundsSet;
			this.appSounds = appSounds;

#if UNITY_EDITOR
			gameObject.name = "SoundSetPlayer-" + soundsSet.name;
#endif
			
			followTarget = null;
			audioClipWrapper = null;
			
			LastClipLenght = 0;
			audioClipWrapperVolume = 1;
			AutoReplay = false;
			audioSource.spatialBlend = 0;
			localVolume = 1;
			typeVolume = 1;
			audioSource.loop = false;
			audioClipWrapper = soundsSet.NextClip(); 
		}

		public void SetFollowTarget(Transform target)
		{
			Assert.IsNotNull(target);
            
			audioSource.spatialBlend = 1;
			followTarget = target;
		}

		public void SetWorldPosition(Vector3 position)
		{
			audioSource.spatialBlend = 1;
			transform.position = position;
		}

		public void SetLocalVolume(float localVolume)
		{
			this.localVolume = localVolume;
			audioSource.volume = AppSounds.Volume * typeVolume * audioClipWrapperVolume * localVolume;
		}
		
		internal void SetTypeVolume(float typeVolume)
		{
			this.typeVolume = typeVolume;
			audioSource.volume = AppSounds.Volume * typeVolume * audioClipWrapperVolume * localVolume;
		}
		
		public void SetLoop()
		{
			audioSource.loop = true;
			deathTime = float.MaxValue;
		}

		public void Play()
		{
			if (!audioClipWrapper)
			{
				LastClipLenght = 0;
				
				if(!audioSource.loop)
					deathTime = 0;
				return;
			}
			
			if (appSounds.soundGroups.TryGetValue(soundsSet.soundGroup, out SoundGroup group))
			{
				if (Time.realtimeSinceStartup < group.timeNextPlay)
				{
					appSounds.ReturnToPool(this);
					return;
				}

				group.SetNewPlayer(this);
			}

			AudioClip clip = audioClipWrapper.AudioClip;
			LastClipLenght = clip.length;
			audioClipWrapperVolume = audioClipWrapper.Volume;
			
			if(!audioSource.loop)
				deathTime = Time.realtimeSinceStartup + LastClipLenght + 0.1f;


			audioSource.clip = clip;
			//audioSource.volume = appSounds. * audioClipWrapper.Volume * volume;
			SetLocalVolume(localVolume);
			audioSource.pitch = audioClipWrapper.PitchRandom;
			audioSource.outputAudioMixerGroup = soundsSet.AudioMixerGroup;
			audioSource.time = audioClipWrapper.startTime;
			audioSource.Play(); 
		}

		public void Stop()
		{
			if (!IsPlaying)
				return;

			if(audioClipWrapper)
			{
				if (appSounds.soundGroups.TryGetValue(soundsSet.soundGroup, out SoundGroup group))
					group.OnPlayerStop(this);
			}

			appSounds.ReturnToPool(this);
		}
	}
}