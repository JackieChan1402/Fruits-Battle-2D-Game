using System;
using UnityEngine;
using System.Collections;
using EasyUI.Progress;
using UnityEngine.SceneManagement;

namespace NetworkThread
{
    public class NetworkManager : MonoBehaviour
    {
        private static NetworkManager _instance;

        private bool IsConnecting = false;
        private LoginScenesScript _loginScenesScript;
        
        private void Awake()
        {
            try
            {
                _loginScenesScript = FindObjectOfType<LoginScenesScript>();
                Debug.Log($"FindObjectOfType<LoginScenesScript> ({_loginScenesScript})");
            }
            finally
            {
                if (_instance != null)
                {   
                    Debug.Log("NetworkManager instance already exists!");
                    Destroy(gameObject);
                    
                }
                else
                {
                    Debug.Log("Instantiating NetworkManager!");
                    _instance = this;
                    DontDestroyOnLoad(gameObject);
                    NetworkStaticManager.InitializeGameManager();
                }
            }
            
        }

        void Update()
        {
            if (!NetworkStaticManager.ClientHandle.IsConnected() && !IsConnecting)
            {
                if (SceneManager.GetActiveScene().name != "Login")
                {
                    SceneManager.LoadScene("Login");
                }
                
                _loginScenesScript.ShowLoadingPanel("Connecting...");
                IsConnecting = true;
                StartCoroutine(DiscoveryServer());
            }
            
            else if (NetworkStaticManager.ClientHandle.IsConnected() && IsConnecting)
            {
                IsConnecting = false;
            }
        }

        private IEnumerator DiscoveryServer()
        {
            int time = 1;
            while (!NetworkStaticManager.ClientHandle.IsConnected())
            {
                Debug.Log($"Starting discovery: {time}");
                NetworkStaticManager.ClientHandle.DiscoveryServer();
                yield return new WaitForSeconds(2f);
                time++;
            }
        }

        void OnApplicationQuit()
        {
            Debug.Log("Application is quitting. Sending disconnect to server.");
    
            if (NetworkStaticManager.ClientHandle != null)
            {
                NetworkStaticManager.ClientHandle.SendDisconnect();
            }
            Debug.Log("Game manager quit successfully.");
        }
    }
}
