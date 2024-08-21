using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Opera
{
    public sealed class GoToScene : MonoBehaviour
    {
        public string scene;

        public void LoadScene() => SceneManager.LoadScene(scene);
    }
}
