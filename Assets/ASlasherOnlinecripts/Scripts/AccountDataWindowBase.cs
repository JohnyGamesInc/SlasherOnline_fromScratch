using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


namespace SlasherOnline
{
    
    public class AccountDataWindowBase : MonoBehaviour
    {

        [SerializeField] private InputField usernameField;
        [SerializeField] private InputField passwordField;

        [SerializeField] protected Button backButton;
        
        
        protected string username;
        protected string password;

        

        private void Awake()
        {
            SubscribeElementsUI();
        }


        protected virtual void SubscribeElementsUI()
        {
            usernameField.onValueChanged.AddListener(name =>
            {
                username = name;
            });
            
            passwordField.onValueChanged.AddListener(pass =>
            {
                password = pass;
            });
        }


        protected void SwitchPhotonScene()
        {
            SceneManager.LoadSceneAsync(1);
        }


        private void OnDestroy()
        {
            usernameField.onValueChanged.RemoveAllListeners();
            passwordField.onValueChanged.RemoveAllListeners();
        }
        
        
    }
}