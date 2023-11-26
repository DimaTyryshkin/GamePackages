using UnityEngine.Events;

namespace GamePackages.Core
{
	public class EventSync
	{
		UnityAction onComplete;
		int          counter;
		
		public void Increment()
		{
			counter++;
		}

		public void Decrement()
		{
			counter--;

			if (counter == 0)
				CallEvent();
		}

		public void OnComplete(UnityAction onComplete)
		{
			this.onComplete = onComplete;
			
			if (counter == 0)
				CallEvent();
		}

		void CallEvent()
		{
			if (onComplete != null)
			{
				var copy = onComplete;
				onComplete = null;
				copy.Invoke();
			}
		}
	}
}