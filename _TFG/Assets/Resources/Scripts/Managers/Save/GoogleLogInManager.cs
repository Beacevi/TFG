using System.Collections;
using Firebase.Extensions;
using Google;
using System.Threading.Tasks;
using UnityEngine;
using Firebase.Auth;
using Firebase.Firestore;
using TMPro;
using System;

public class GoogleFirebaseLogin : MonoBehaviour
{
    [Header("Google API")]
    [SerializeField] private string webClientId = "264966857771-k8ksaobqrna2mtl7fm8p4fl6u16g521u.apps.googleusercontent.com"; // Web Client ID de Firebase
    private bool isGoogleSignInInitialized = false;

    [Header("Firebase")]
    private FirebaseAuth auth;
    private FirebaseUser user;
    private FirebaseFirestore db;

    public TextMeshProUGUI textUUID;
    public TextMeshProUGUI textNuevoUser;
    public TextMeshProUGUI textAnteriorUser;
    public TextMeshProUGUI textCoins;

    public int coins = 0;

    void Start()
    {
        InitFirebase();
    }

    void InitFirebase()
    {
        auth = FirebaseAuth.DefaultInstance;
        db = FirebaseFirestore.DefaultInstance;
        Debug.Log("Firebase listo");
    }

    public void aumentarCoins()
    {
        int coinsNuevas = coins + 1;

        textCoins.text = "" + coinsNuevas;

        coins = coinsNuevas;

    }

    public void OnGoogleButtonPressed()
    {
        if (!isGoogleSignInInitialized)
        {
            GoogleSignIn.Configuration = new GoogleSignInConfiguration
            {
                WebClientId = webClientId,
                RequestEmail = true,
                RequestIdToken = true
            };
            isGoogleSignInInitialized = true;
        }

        GoogleSignIn.DefaultInstance.SignIn().ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogWarning("Google sign-in cancelado.");
                return;
            }

            if (task.IsFaulted)
            {
                Debug.LogError("Google sign-in error: " + task.Exception);
                return;
            }

            GoogleSignInUser googleUser = task.Result;

            Credential credential = GoogleAuthProvider.GetCredential(googleUser.IdToken, null);

            auth.SignInWithCredentialAsync(credential).ContinueWithOnMainThread(authTask =>
            {
                if (authTask.IsCanceled)
                {
                    Debug.LogWarning("Firebase auth cancelado.");
                    return;
                }

                if (authTask.IsFaulted)
                {
                    Debug.LogError("Firebase auth fallido: " + authTask.Exception);
                    return;
                }

                user = auth.CurrentUser;
                Debug.Log($"Firebase login correcto. UID: {user.UserId}");

                textUUID.text = user.UserId.ToString();
                // Ahora manejamos Firestore
                CheckOrCreateUserInFirestore(user.UserId);
            });
        });
    }

    private void CheckOrCreateUserInFirestore(string uid)
    {
        DocumentReference docRef = db.Collection("users").Document(uid);
        docRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.LogError("Error accediendo a Firestore: " + task.Exception);
                return;
            }

            DocumentSnapshot snapshot = task.Result;
            if (snapshot.Exists)
            {
                Debug.Log("Usuario existe, cargando datos...");
                
                textAnteriorUser.text = "El usuario NOOOO es nuevo";
                LoadUserData(snapshot);
            }
            else
            {
                Debug.Log("Usuario nuevo, creando datos base...");
                textNuevoUser.text = "El usuario es nuevo";
                CreateBaseUserData(docRef);
            }
        });
    }

    private void LoadUserData(DocumentSnapshot snapshot)
    {
        int level = snapshot.ContainsField("level") ? snapshot.GetValue<int>("level") : 1;
        int coins = snapshot.ContainsField("coins") ? snapshot.GetValue<int>("coins") : 0;
        Debug.Log($"Datos cargados → Level: {level}, Coins: {coins}");
        // Aquí puedes actualizar UI o variables de juego según lo necesites
    }

    private void CreateBaseUserData(DocumentReference docRef)
    {
        var newUser = new { level = 1, coins = coins, lastLogin = Timestamp.GetCurrentTimestamp() };
        docRef.SetAsync(newUser).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompletedSuccessfully)
                Debug.Log("Datos base creados con éxito.");
            else
                Debug.LogError("Error creando datos base: " + task.Exception);
        });
    }

    public void SignOut()
    {
        GoogleSignIn.DefaultInstance.SignOut();
        auth.SignOut();
        Debug.Log("Usuario desconectado.");
    }
}
