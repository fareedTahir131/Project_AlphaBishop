using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using Firebase.Extensions;
using UnityEngine.SceneManagement;

public class FirebaseAuthManager : MonoBehaviour
{
    public static FirebaseAuthManager instance;
    private FirebaseApp firebaseApp;
    private FirebaseAuth auth;
    private FirebaseUser user;
    private FirebaseFirestore db;

    private const string MatchEmailPattern =
        @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
        + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
        + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
        + @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$";

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        InitializeFirebase();
    }

    void InitializeFirebase()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                firebaseApp = FirebaseApp.DefaultInstance;

                InitializeAuth();
                InitializeFirestore();
            }
            else
            {
                Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
            }
        });
    }

    void InitializeFirestore()
    {
        try
        {
            db = FirebaseFirestore.GetInstance(firebaseApp);
            Debug.Log("Firestore initialized successfully.");
            //TestFirestoreConnection();
            //LoadUserData(auth.CurrentUser.UserId);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to initialize Firestore: " + e.Message);
        }
    }

    void InitializeAuth()
    {
        Debug.Log("Setting up Firebase Auth");
        auth = FirebaseAuth.DefaultInstance;
        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);

        //SignOutUser();

        //Signup("fareed","Fareed sur","Fareed username","fareed6@gmail.com","12345678");

        //UpdateUserData("ft","ft surname","ft username");
    }

    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth.CurrentUser != user)
        {
            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null;
            if (!signedIn && user != null)
            {
                Debug.Log("Signed out " + user.UserId);
            }
            user = auth.CurrentUser;
            if (signedIn)
            {
                Debug.Log("Signed in " + user.UserId);
                // Perform actions after successful login/signup, like loading the game scene.
            }
        }
    }

    async void TestFirestoreConnection()
    {
        if (db != null)
        {
            try
            {
                string userId = auth.CurrentUser.UserId;
                DocumentReference docRef = db.Collection("users").Document(userId);
                DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
                if (snapshot.Exists)
                {
                    Debug.Log("Firestore connection is working.");
                    PrintDocumentData(snapshot);
                    //UpdateCurrentUserProfile("ft", "ft surname", "ft username");
                }
                else
                {
                    Debug.Log("Firestore connection is working, but the test document does not exist.");
                }
                //UpdateCurrentUserProfile("ft", "ft surname", "ft username");
                //SaveUserData(userId, "ft", "ft surname", "ft username","fareed@gmail.com");
            }
            catch (System.Exception e)
            {
                Debug.LogError("Firestore connection test failed: " + e.Message);
            }
        }
        else
        {
            Debug.LogError("Firestore was not initialized. Cannot test connection.");
        }
    }

    void PrintDocumentData(DocumentSnapshot snapshot)
    {
        if (snapshot.Exists)
        {
            Dictionary<string, object> data = snapshot.ToDictionary();
            Debug.Log("Document data:");
            foreach (KeyValuePair<string, object> pair in data)
            {
                Debug.Log($"{pair.Key}: {pair.Value}");
            }
        }
        else
        {
            Debug.Log("Document does not exist.");
        }
    }


    // Example function to sign out the user.
    public void SignOutUser()
    {
        auth.SignOut();
        //statusText.text = "Signed out.";
    }

    public void Login(string email, string password)
    {
        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.Log("SignInWithEmailAndPasswordAsync was canceled.");
                //ConsoleManager.instance.ShowMessage("Signin Canceled");
                return;
            }
            if (task.IsFaulted)
            {
                //LoadingPanel.SetActive(false);
                //ConsoleManager.instance.ShowMessage("Email or Password not Valid!");
                Debug.Log("Email or Password not Valid!");
                Debug.Log("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return;
            }

            if (task.IsCompletedSuccessfully)
            {
                FirebaseUser firebaseUser = task.Result.User;
                Debug.Log("Login successful: " + firebaseUser.Email);
                UserID = firebaseUser.UserId;
                Email = firebaseUser.Email;
                //LoadUserData(firebaseUser.UserId);
                //LoadUserData("tiOqVTC8NXVAMpcJLpcasN4AKfl1");
                SceneManager.LoadScene(1);
                //UpdateUserData("", "ft surname", "ft username");
            }
        });
    }


    public void Signup(string name, string surname, string username, string email, string password)
    {
        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompletedSuccessfully)
            {
                FirebaseUser firebaseUser = task.Result.User; // Access the User property
                Debug.Log("Signup successful: " + firebaseUser.Email);
                SceneManager.LoadScene(1);
                //UserID = firebaseUser.UserId;
                //Email = email;
                //Name = name;
                //Username = username;

                //SaveUserData(firebaseUser.UserId, name, surname, username, email);
            }
            else
            {
                Debug.Log("Email or Password not Valid!");
                Debug.LogError("Signup failed: " + task.Exception);
            }
        });
    }


    private void SaveUserData(string userId, string name, string surname, string username, string email)
    {
        DocumentReference docRef = db.Collection("users").Document(userId);
        Dictionary<string, object> userData = new Dictionary<string, object>
        {
            {"name", name},
            {"surname", surname},
            {"username", username},
            {"email", email}
        };

        docRef.SetAsync(userData).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
                Debug.Log("User data saved successfully");
            else
                Debug.LogError("Failed to save user data: " + task.Exception);
        });
    }

    public void LoadUserData(string userId)
    {
        DocumentReference docRef = db.Collection("users").Document(userId);
        docRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && task.Result.Exists)
            {
                Dictionary<string, object> userData = task.Result.ToDictionary();
                Name = userData["name"].ToString();
                Username = userData["username"].ToString();
                Email = userData["email"].ToString();
                Debug.Log("User data loaded successfully "+ Name+", "+ Username+", " + Email);
            }
            else
            {
                Debug.LogError("Failed to load user data");
            }
        });
    }

    public void UpdateCurrentUserProfile(string newName, string newSurname, string newUsername)
    {
        if (auth.CurrentUser == null)
        {
            Debug.LogError("No user is currently logged in.");
            return;
        }

        string userId = auth.CurrentUser.UserId;
        DocumentReference docRef = db.Collection("users").Document(userId);

        Dictionary<string, object> updatedData = new Dictionary<string, object>
        {
            { "name", newName },
            { "surname", newSurname },
            { "username", newUsername }
        };

        docRef.UpdateAsync(updatedData).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("User profile updated successfully.");
                // Update PlayerPrefs or other local data if necessary.
                Name = newName;
                Username = newUsername;
                // You may not want to store surname in PlayerPrefs, based on your application's needs.
            }
            else
            {
                Debug.LogError("Failed to update user profile: " + task.Exception);
            }
        });
    }

    private bool IsValidEmail(string email)
    {
        if (email != null)
            return Regex.IsMatch(email, MatchEmailPattern);
        else
            return false;
    }

    public static string UserID
    {
        get => PlayerPrefs.GetString("UserID", "");
        set => PlayerPrefs.SetString("UserID", value);
    }
    public static string Name
    {
        get => PlayerPrefs.GetString("Name", "");
        set => PlayerPrefs.SetString("Name", value);
    }
    public static string Username
    {
        get => PlayerPrefs.GetString("Username", "");
        set => PlayerPrefs.SetString("Username", value);
    }
    public static string Email
    {
        get => PlayerPrefs.GetString("Email", "");
        set => PlayerPrefs.SetString("Email", value);
    }
}
