using Firebase.Auth;                    // Firebase Authentication
using Firebase.Extensions;              // Permite usar ContinueWithOnMainThread
using Firebase.Firestore;               // Firebase Firestore (base de datos)
using Google;                           // Google Sign-In para Unity
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;                            // TextMeshPro para UI
using UnityEngine;

/// <summary>
/// Maneja el login con Google usando Firebase Authentication
/// y crea / carga el usuario en Firestore.
/// </summary>
public class GoogleFirebaseLogin : MonoBehaviour
{
    public static GoogleFirebaseLogin Instance;

    // =========================
    // CONFIGURACIÓN GOOGLE
    // =========================

    [Header("Google API")]
    // Web Client ID obtenido desde Firebase Console (NO Android ID)
    [SerializeField]
    private string webClientId =
        "264966857771-k8ksaobqrna2mtl7fm8p4fl6u16g521u.apps.googleusercontent.com";

    // Evita configurar Google Sign-In más de una vez
    private bool isGoogleSignInInitialized = false;

    // =========================
    // FIREBASE
    // =========================

    private FirebaseAuth auth;           // Sistema de autenticación
    private FirebaseUser user;           // Usuario autenticado actual
    private FirebaseFirestore db;        // Referencia a Firestore

    // =========================
    // UI
    // =========================

    public TextMeshProUGUI textUUID;
    public TextMeshProUGUI textNuevoUser;
    public TextMeshProUGUI textAnteriorUser;
    public TextMeshProUGUI textCoins;

    // =========================
    // DATOS DE JUEGO
    // =========================

    public int coinsActuales = 0;

    // =========================
    // UNITY LIFECYCLE
    // =========================

    void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        InitFirebase();
    }

    /// <summary>
    /// Inicializa Firebase Authentication y Firestore
    /// </summary>
    void InitFirebase()
    {
        auth = FirebaseAuth.DefaultInstance;
        db = FirebaseFirestore.DefaultInstance;
        Debug.Log("Firebase listo");
    }

    // =========================
    // EJEMPLO DE LÓGICA DE JUEGO
    // =========================

    /// <summary>
    /// Incrementa las monedas localmente y actualiza la UI
    /// (aún no guarda en Firestore)
    /// </summary>
    public void aumentarCoins()
    {
        int coinsNuevas = coinsActuales + 1;
        coinsActuales = coinsNuevas;
        textCoins.text = coinsActuales.ToString();
    }

    // =========================
    // LOGIN CON GOOGLE
    // =========================

    /// <summary>
    /// Se llama al pulsar el botón de login con Google
    /// </summary>
    public void OnGoogleButtonPressed()
    {
        // Configura Google Sign-In solo una vez
        if (!isGoogleSignInInitialized)
        {
            GoogleSignIn.Configuration = new GoogleSignInConfiguration
            {
                WebClientId = webClientId,   // ID web de Firebase
                RequestEmail = true,
                RequestIdToken = true        // Necesario para Firebase Auth
            };

            isGoogleSignInInitialized = true;
        }

        // Lanza el selector de cuentas de Google
        GoogleSignIn.DefaultInstance.SignIn()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogWarning("Google sign-in cancelado.");
                    return;
                }

                if (task.IsFaulted)
                {
                    Debug.LogError("Google sign-in error: " + task.Exception);
                    textNuevoUser.text = "Google sign-in error: " + task.Exception;
                    return;
                }

                // Usuario de Google autenticado correctamente
                GoogleSignInUser googleUser = task.Result;

                // Se crea la credencial de Firebase usando el ID Token de Google
                Credential credential =
                    GoogleAuthProvider.GetCredential(googleUser.IdToken, null);

                // Login en Firebase usando la credencial de Google
                auth.SignInWithCredentialAsync(credential)
                    .ContinueWithOnMainThread(authTask =>
                    {
                        if (authTask.IsCanceled)
                        {
                            Debug.LogWarning("Firebase auth cancelado.");
                            textNuevoUser.text = "Firebase auth cancelado.";
                            return;
                        }

                        if (authTask.IsFaulted)
                        {
                            Debug.LogError("Firebase auth fallido: " + authTask.Exception);
                            textNuevoUser.text = "Firebase auth fallido: " + authTask.Exception;
                            return;
                        }

                        // Usuario autenticado correctamente en Firebase
                        user = auth.CurrentUser;
                        Debug.Log($"Firebase login correcto. UID: {user.UserId}");
                        textNuevoUser.text = $"Firebase login correcto. UID: {user.UserId}";

                        textUUID.text = user.UserId;

                        // Comprobamos si el usuario existe en Firestore
                        CheckOrCreateUserInFirestore(user.UserId);
                    });
            });
    }

    // =========================
    // FIRESTORE
    // =========================

    /// <summary>
    /// Comprueba si el usuario existe en Firestore.
    /// Si no existe, crea un documento base.
    /// </summary>
    private void CheckOrCreateUserInFirestore(string uid)
    {
        DocumentReference docRef = db.Collection("users").Document(uid);

        docRef.GetSnapshotAsync()
            .ContinueWithOnMainThread(task =>
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
                    textAnteriorUser.text = "El usuario NO es nuevo";
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

    /// <summary>
    /// Carga los datos del usuario desde Firestore
    /// </summary>
    private void LoadUserData(DocumentSnapshot snapshot)
    {
        int level = snapshot.ContainsField("level")
            ? snapshot.GetValue<int>("level")
            : 1;

        int coins = snapshot.ContainsField("coins")
            ? snapshot.GetValue<int>("coins")
            : 0;

        coinsActuales = coins;
        textCoins.text = coinsActuales.ToString();

        Debug.Log($"Datos cargados → Level: {level}, Coins: {coins}");
    }

    /// <summary>
    /// Crea el documento inicial del usuario en Firestore
    /// </summary>
    private void CreateBaseUserData(DocumentReference docRef)
    {
        var newUser = new
        {
            level = 1,
            coins = coinsActuales,
            lastLogin = Timestamp.GetCurrentTimestamp()
        };

        docRef.SetAsync(newUser)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCompletedSuccessfully)
                    Debug.Log("Datos base creados con éxito.");
                else
                    Debug.LogError("Error creando datos base: " + task.Exception);
            });
    }

    /// <summary>
    /// Guarda los datos actuales del jugador en Firestore.
    /// Debe llamarse solo cuando el usuario ya esté logueado.
    /// </summary>
    public void SaveUserData()
    {
        // Seguridad básica: comprobar que hay usuario logueado
        if (user == null)
        {
            Debug.LogWarning("No se puede guardar: usuario no logueado.");
            return;
        }

        // Referencia al documento del usuario (users/{uid})
        DocumentReference docRef = db.Collection("users").Document(user.UserId);

        // Datos a guardar / actualizar
        var dataToSave = new Dictionary<string, object>
    {
        { "coins", coinsActuales },
        { "lastSave", Timestamp.GetCurrentTimestamp() }
        // { "level", levelActual } ← si lo usas más adelante
    };

        //SetOptions.Merge = true → NO borra el resto del documento
        docRef.SetAsync(dataToSave, SetOptions.MergeAll)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCompletedSuccessfully)
                {
                    Debug.Log("Datos del usuario guardados correctamente.");
                }
                else
                {
                    Debug.LogError("Error guardando datos del usuario: " + task.Exception);
                }
            });
    }


    /// <summary>
    /// Guarda los datos de los KPIs actuales del jugador en Firestore.
    /// Debe llamarse solo cuando el usuario ya esté logueado.
    /// </summary>
    public void SaveUserDataKPIs()
    {
        // Seguridad básica: comprobar que hay usuario logueado
        if (user == null)
        {
            Debug.LogWarning("No se puede guardar: usuario no logueado.");
            textNuevoUser.text = "No se puede guardar: usuario no logueado.";
            return;
        }

        // Referencia al documento del usuario (users/{uid})
        DocumentReference docRef = db.Collection("users").Document(user.UserId);

        // Datos a guardar / actualizar
        var dataToSave = new Dictionary<string, object>
        {
            { "SesionesTotales", KPIsManager.Instance.contadorSesiones },
            { "UltimaSesion", Timestamp.GetCurrentTimestamp() },
            { "TiempoTotalJuego", (double)KPIsManager.Instance.tiempoJuego }
            // { "level", levelActual } ← si lo usas más adelante
        };

        //SetOptions.Merge = true → NO borra el resto del documento
        docRef.SetAsync(dataToSave, SetOptions.MergeAll)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCompletedSuccessfully)
                {
                    Debug.Log("Datos del usuario guardados correctamente.");
                    textNuevoUser.text = "Datos del usuario guardados correctamente.";
                }
                else
                {
                    Debug.LogError("Error guardando datos del usuario: " + task.Exception);
                    textNuevoUser.text = "Error guardando datos del usuario: " + task.Exception;
                }
            });
    }


    // =========================
    // LOGOUT
    // =========================

    /// <summary>
    /// Cierra sesión tanto en Google como en Firebase.
    /// </summary>
    public void SignOut()
    {
        GoogleSignIn.DefaultInstance.SignOut();
        auth.SignOut();
        Debug.Log("Usuario desconectado.");
    }

        void OnApplicationQuit()
    {
        SaveUserDataKPIs();
        SaveUserData();
    }

    void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            SaveUserDataKPIs();
            SaveUserData();
        }
            
    }

}
