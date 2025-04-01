using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using System.Threading.Tasks;
using System.Collections.Generic;

public class FirestoreAuthInitializationCheck : MonoBehaviour
{
    private FirebaseApp firebaseApp;
    private FirebaseFirestore firestore;
    private FirebaseAuth auth;
    private FirebaseUser user;

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

                InitializeFirestore();
                InitializeAuth();
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
            firestore = FirebaseFirestore.GetInstance(firebaseApp);
            Debug.Log("Firestore initialized successfully.");
            TestFirestoreConnection();
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
        if (firestore != null)
        {
            try
            {
                DocumentReference docRef = firestore.Collection("users").Document("tiOqVTC8NXVAMpcJLpcasN4AKfl1");
                DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
                if (snapshot.Exists)
                {
                    Debug.Log("Firestore connection is working.");
                    PrintDocumentData(snapshot);
                }
                else
                {
                    Debug.Log("Firestore connection is working, but the test document does not exist.");
                }
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
}