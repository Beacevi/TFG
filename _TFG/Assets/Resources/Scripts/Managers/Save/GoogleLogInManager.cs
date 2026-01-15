using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using System.Threading.Tasks;

public class GoogleFirebaseLogin : MonoBehaviour
{
    private FirebaseAuth auth;
    private FirebaseFirestore db;

    void Awake()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                auth = FirebaseAuth.DefaultInstance;
                db = FirebaseFirestore.DefaultInstance;
                Debug.Log("Firebase listo");
            }
            else
            {
                Debug.LogError("Firebase no disponible: " + task.Result);
            }
        });
    }

    // Asignar a botón UI
    public void OnGoogleButtonPressed()
    {
        SignInWithGoogle();
    }

    private void SignInWithGoogle()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        // Llamar al plugin oficial de Firebase para Google Sign-In
        Firebase.Auth.GoogleSignIn.DefaultInstance.SignInAsync().ContinueWith(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("Google Sign-In fallido: " + task.Exception);
                return;
            }

            // Obtienes el idToken real
            string idToken = task.Result.IdToken;

            // Login en Firebase
            Credential credential = GoogleAuthProvider.GetCredential(idToken, null);
            auth.SignInWithCredentialAsync(credential).ContinueWith(firebaseTask =>
            {
                if (firebaseTask.IsCanceled || firebaseTask.IsFaulted)
                {
                    Debug.LogError("Firebase login fallido: " + firebaseTask.Exception);
                    return;
                }

                FirebaseUser user = firebaseTask.Result;
                Debug.Log("Firebase login correcto. UID: " + user.UserId);
                CheckOrCreateUserInFirestore(user.UserId);
            });
        });
#else
        Debug.LogWarning("Login Google solo funciona en Android real");
#endif
    }

    private void CheckOrCreateUserInFirestore(string uid)
    {
        DocumentReference docRef = db.Collection("users").Document(uid);
        docRef.GetSnapshotAsync().ContinueWith(task =>
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
                LoadUserData(snapshot);
            }
            else
            {
                Debug.Log("Usuario nuevo, creando datos base...");
                CreateBaseUserData(docRef);
            }
        });
    }

    private void LoadUserData(DocumentSnapshot snapshot)
    {
        int level = snapshot.ContainsField("level") ? snapshot.GetValue<int>("level") : 1;
        int coins = snapshot.ContainsField("coins") ? snapshot.GetValue<int>("coins") : 0;
        Debug.Log($"Datos cargados → Level: {level}, Coins: {coins}");
    }

    private void CreateBaseUserData(DocumentReference docRef)
    {
        var newUser = new { level = 1, coins = 0, lastLogin = Timestamp.GetCurrentTimestamp() };
        docRef.SetAsync(newUser).ContinueWith(task =>
        {
            if (task.IsCompletedSuccessfully)
                Debug.Log("Datos base creados con éxito.");
            else
                Debug.LogError("Error creando datos base: " + task.Exception);
        });
    }
}
