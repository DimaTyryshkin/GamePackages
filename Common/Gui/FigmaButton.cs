using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace GamePackages.Core
{
    [RequireComponent(typeof(Button))]
    public class FigmaButton : MonoBehaviour
    {
        [SerializeField] Transform[] diactivateImmediate;
        [SerializeField] Transform[] activateImmediate;
        [SerializeField] Transform nextFrame;
        [SerializeField] float delay;
        [SerializeField] Animator animatorToPlay;
        [SerializeField] string triggerName;

        private void Start()
        {
            GetComponent<Button>().onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            foreach (var item in diactivateImmediate)
                item.gameObject.SetActive(false);

            foreach (var item in activateImmediate)
                item.gameObject.SetActive(true);

            if (animatorToPlay)
                animatorToPlay.SetTrigger(triggerName);


            if (nextFrame)
                StartCoroutine(ShowNextFrame());
        }

        IEnumerator ShowNextFrame()
        {
            yield return new WaitForSeconds(delay);
            var root = transform.GetComponentInParent<Canvas>().transform;
            for (int i = 0; i < root.childCount; i++)
            {
                Transform child = root.GetChild(i);
                child.gameObject.SetActive(child == nextFrame);
            }

            nextFrame.gameObject.SetActive(true);
        }
    }
}