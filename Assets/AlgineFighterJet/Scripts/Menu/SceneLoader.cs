using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

namespace Algine.Aircraft
{

    public class SceneLoader : MonoBehaviour
    {
        public Image loadingImage;
        public TextMeshProUGUI loadingText;
        public GameObject LoadingParent;

        // Start is called before the first frame update
        private void Start()
        {
            LoadingParent.SetActive(false);
        }

        public void LoadScene1()
        {
            StartCoroutine(LoadAsynchronously(1));
        }
        public void LoadScene2()
        {
            StartCoroutine(LoadAsynchronously(2));
        }

        IEnumerator LoadAsynchronously(int sceneIndex)
        {
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneIndex);
            LoadingParent.SetActive(true);
            while (!asyncOperation.isDone)
            {
                float progress = Mathf.Clamp01(asyncOperation.progress / .9f);
                loadingImage.fillAmount = progress;
                loadingText.text =  (progress * 100).ToString("f2")+"%";
                yield return null;
            }
        }
    }

}
