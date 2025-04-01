using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InputUIManager : MonoBehaviour
{
    public static InputUIManager instance;
    [Header("Panel")]
    //public GameObject SignupPanel;
    //public GameObject SigninPanel;
    //public GameObject ColorSelectionPanel;
    //public  GameObject LoadingPanel;

    [Header("Forgot Password Input Area")]
    public TMP_InputField f_emailInput;

    [Header("Login Input Area")]
    public TMP_InputField s_emailInput;
    public TMP_InputField s_passwordInput;

    [Header("SignUp Input Area")]
    //public Animator animator;
    public TMP_InputField nameInput;
    public TMP_InputField SurnameInput;
    public TMP_InputField UsernameInput;
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public TMP_InputField ConfirmPasswordInput;

    private const string MatchEmailPattern =
        @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
        + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
        + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
        + @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$";


    private const string MatchDOBpattern = "^[0-9]{1,2}\\-[0-9]{1,2}\\-[0-9]{4}$";


    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }
    public void Next()
    {
        if (nameInput.text == "")
        {
            Debug.Log("Name is Empty!");
            return;
        }
        if (emailInput.text == "")
        {
            Debug.Log("Email is Empty!");
            return;
        }
        if (!ValidateEmail(emailInput.text))
        {
            Debug.Log("Email not Valid!");
            return;
        }
    }

    public void ForgotPassword()
    {
        if (f_emailInput.text == "")
        {
            Debug.Log("Email is Empty!");
            return;
        }
        if (!ValidateEmail(f_emailInput.text))
        {
            Debug.Log("Email not Valid!");
            return;
        }
        //LoadingPanel.SetActive(true);
        //FirebaseAuthManager.instance.SendPasswordResetEmail(f_emailInput.text);
    }
    public void SignInUser()
    {

        if (s_emailInput.text == "")
        {
            Debug.Log("Email is Empty!");
            return;
        }
        if (!ValidateEmail(s_emailInput.text))
        {
            Debug.Log("Email not Valid!");
            return;
        }
        if (s_passwordInput.text == "")
        {
            Debug.Log("Password is Empty!");
            return;
        }

        if (s_passwordInput.text.Length < 8)
        {
            Debug.Log("Password is not 8 characters!");
            return;
        }

        //LoadingPanel.SetActive(true);
        FirebaseAuthManager.instance.Login(s_emailInput.text, s_passwordInput.text);
        //FirebaseManagerTesting.instance.LoginUser(s_emailInput.text, s_passwordInput.text);
        //AuthManager.instance.LoginUser(s_emailInput.text.ToLower(), s_passwordInput.text);
    }
    //public void PanelSwitchOnSignup(bool SignupPanelValue, bool SigninPanelValue)
    //{
    //    SigninPanel.SetActive(SigninPanelValue);
    //    SignupPanel.SetActive(SignupPanelValue);
    //}

    private bool ValidateEmail(string email)
    {
        if (email != null)
            return Regex.IsMatch(email, MatchEmailPattern);
        else
            return false;
    }
    public void PasswordVisibilityManager(bool ShowPassword)
    {
        // this function is for login
        if (ShowPassword)
        {
            s_passwordInput.contentType = TMP_InputField.ContentType.Alphanumeric;
        }
        else
        {
            s_passwordInput.contentType = TMP_InputField.ContentType.Password;
        }
        s_passwordInput.ForceLabelUpdate();
    }
    public void SignupPasswordVisibilityManager(bool ShowPassword)
    {
        if (ShowPassword)
        {
            passwordInput.contentType = TMP_InputField.ContentType.Alphanumeric;
            ConfirmPasswordInput.contentType = TMP_InputField.ContentType.Alphanumeric;
        }
        else
        {
            passwordInput.contentType = TMP_InputField.ContentType.Password;
            ConfirmPasswordInput.contentType = TMP_InputField.ContentType.Password;
        }
        passwordInput.ForceLabelUpdate();
        ConfirmPasswordInput.ForceLabelUpdate();
    }
    //private bool ValidateDOB(string DOB)
    //{
    //    if (DOBInput!= null)
    //        return Regex.IsMatch(DOB, MatchDOBpattern);
    //    else
    //        return false;
    //}


    public void SignUpPanelValidation()
    {

        if (nameInput.text == "")
        {
            Debug.Log("Name is Empty!");
            return;
        }
        if (SurnameInput.text == "")
        {
            Debug.Log("UserName is Empty!");
            return;
        }
        if (UsernameInput.text == "")
        {
            Debug.Log("UserName is Empty!");
            return;
        }
        if (emailInput.text == "")
        {
            Debug.Log("Email is Empty!");
            return;
        }
        if (passwordInput.text == "")
        {
            Debug.Log("Password is Empty!");
            return;
        }
        if (ConfirmPasswordInput.text == "")
        {
            Debug.Log("Confirm Password Password is Empty!");
            return;
        }
        if (ConfirmPasswordInput.text != passwordInput.text)
        {
            Debug.Log("Password Doesn't match");
            return;
        }
        if (passwordInput.text.Length < 8)
        {
            Debug.Log("Password is not 8 characters!");
            return;
        }

        if (!ValidateEmail(emailInput.text))
        {
            Debug.Log("Email not Valid!");
            return;
        }

        //if (!ValidateDOB(DOBInput.text))
        //{
        //    Debug.Log("Date of Birth not Valid! use(-) between Dates");
        //    return;
        //}
        //LoadingPanel.SetActive(true);
        FirebaseAuthManager.instance.Signup(nameInput.text,SurnameInput.text,UsernameInput.text,emailInput.text, passwordInput.text);
        //CreateUser();
        //SignupPanel.SetActive(false);
        //SigninPanel.SetActive(false);
        //ColorSelectionPanel.SetActive(true);
    }

}
