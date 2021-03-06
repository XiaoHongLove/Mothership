﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using MothershipUtility;
using MothershipOS;
using System;

namespace MothershipUI
{
    public class RegisterGUI : MonoBehaviour
    {
        [SerializeField]
        private GameObject content = null;

        [SerializeField]
        private InputField emailField = null;
        [SerializeField]
        private InputField passwordField = null;
        [SerializeField]
        private InputField displayName = null;
        [SerializeField]
        private Button submitButton = null;
        [SerializeField]
        private Text message = null;

        [SerializeField]
        private ProfileGUI profileScreen = null;

        private void Start()
        {
            emailField.characterValidation = InputField.CharacterValidation.EmailAddress;
            emailField.contentType = InputField.ContentType.EmailAddress;

            passwordField.contentType = InputField.ContentType.Password;
            passwordField.characterValidation = InputField.CharacterValidation.Alphanumeric;

            displayName.characterValidation = InputField.CharacterValidation.Alphanumeric;
            displayName.contentType = InputField.ContentType.Alphanumeric;

            submitButton.onClick.AddListener(Submit);
        }

        public void EnableScreen()
        {
            submitButton.interactable = true;
            content.SetActive(true);
        }

        public void DisableScreen()
        {
            emailField.text = "";
            passwordField.text = "";
            displayName.text = "";
            message.text = "";
            content.SetActive(false);
        }

        private void Submit()
        {
            if (passwordField.text.Length >= 6 && displayName.text.Length >= 2)
            {
                submitButton.interactable = false;
                message.text = "Attempting Registration...";
                WWWForm form = CreateForm();
                StartCoroutine(Register(form));
            }
            else
            {
                message.text = "Incorrect input";
            }
        }

        private WWWForm CreateForm()
        {
            WWWForm form = new WWWForm();
            form.AddField("Email", emailField.text);
            form.AddField("Password", HashUtility.GetMD5Hash(passwordField.text));
            form.AddField("DisplayName", displayName.text);
            form.AddField("Hash", HashUtility.GetMD5Hash(emailField.text + AppKey.appKey));

            return form;
        }

        private IEnumerator Register(WWWForm form)
        {
            WWW response = new WWW(WWWFormUtility.registerURL, form);
            yield return response;

            if (response.error == null)
            {
                if (ReadResponse(response.text))
                {
                    DisableScreen();
                    profileScreen.EnableScreen();
                }
            }
            else
            {
                message.text = response.error;
                submitButton.interactable = true;
            }
        }

        private bool ReadResponse(string input)
        {
            // Check if error code was returned, otherwise try to decode form Json
            int errorCode;
            if (Int32.TryParse(input, out errorCode))
            {
                message.text = Enum.GetName(typeof(ResponseEnums.AccountCreationResponse), errorCode);
                submitButton.interactable = true;
                return false;
            }
            else
            {
                User user = JsonUtility.ValidateJsonData<User>(input);
                if (user != default(User))
                {
                    message.text = "Registered. Logging in..";
                    UserDataManager.userData.User = user;
                    return true;
                }
                else
                {
                    Debug.LogError("Failed to deserialize as User: " + input);
                    return false;
                }
            }
        }
    }
}
