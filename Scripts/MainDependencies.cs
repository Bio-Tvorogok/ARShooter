using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainDependencies : MonoBehaviour
{
    public static MainDependencies Instance;

    // Todo change this
    public Camera mainCamera;
    // [SerializeField] public UiManager ui;

      // Correctly singleton added
      // Please add the main dependencies in this class
      // And get them from him
      // for example:
      // [SerializeField] public GameObject gameManager;

      // Dont touch this func!!
    void Awake () {
        if (Instance == null) {
          DontDestroyOnLoad (gameObject);
          Instance = this;
        }
        else if (Instance != this) {
          Destroy (gameObject);
        } 
    }
}
