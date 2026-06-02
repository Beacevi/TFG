using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

/// <summary>
/// Menú de Editor que asigna automáticamente los 6 AudioClips descargados a sus
/// campos correspondientes:
///   - 4 SFX de pájaros (Sfx Dive / Rummage / Found / Collect) en el componente
///     BirdFollower del prefab Player.
///   - 2 efectos de mapa (sonidoClickMapa, sonidoPasos) en el componente Sounds
///     del GameManager dentro de la escena UI.unity.
///
/// Los .mp3 deben estar previamente en Assets/Resources/Sounds/ con estos nombres:
///   SfxDive.mp3, SfxRummage.mp3, SfxFound.mp3, SfxCollect.mp3,
///   SonidoClickMapa.mp3, SonidoPasos.mp3
/// </summary>
public static class AssignSoundClips
{
    private const string PlayerPrefabPath = "Assets/Resources/Prefabs/Player.prefab";
    private const string UIScenePath      = "Assets/Scenes/UI.unity";

    private const string SoundsFolder = "Assets/Resources/Sounds/";

    [MenuItem("Tools/TFG/Asignar SFX al Player prefab y a Sounds (UI scene)")]
    public static void Run()
    {
        // Cargar todos los clips. Si alguno falta, abortar con error claro.
        var dive    = LoadClip("SfxDive.mp3");
        var rummage = LoadClip("SfxRummage.mp3");
        var found   = LoadClip("SfxFound.mp3");
        var collect = LoadClip("SfxCollect.mp3");
        var click   = LoadClip("SonidoClickMapa.mp3");
        var pasos   = LoadClip("SonidoPasos.mp3");

        if (dive == null || rummage == null || found == null || collect == null
            || click == null || pasos == null)
        {
            Debug.LogError("[AssignSoundClips] Faltan uno o más .mp3 en " + SoundsFolder + ". Aborto.");
            return;
        }

        // --- 1) Asignar los 4 SFX al BirdFollower del prefab Player ---
        AssignBirdFollower(dive, rummage, found, collect);

        // --- 2) Asignar los 2 efectos a Sounds en la escena UI ---
        AssignSoundsInUIScene(click, pasos);

        AssetDatabase.SaveAssets();
        Debug.Log("[AssignSoundClips] Listo: 6 AudioClips asignados.");
    }

    private static AudioClip LoadClip(string filename)
    {
        string path = SoundsFolder + filename;
        var clip = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
        if (clip == null) Debug.LogError("[AssignSoundClips] No se encontró " + path);
        return clip;
    }

    private static void AssignBirdFollower(AudioClip dive, AudioClip rummage, AudioClip found, AudioClip collect)
    {
        var prefab = PrefabUtility.LoadPrefabContents(PlayerPrefabPath);
        if (prefab == null)
        {
            Debug.LogError("[AssignSoundClips] No se pudo cargar " + PlayerPrefabPath);
            return;
        }

        try
        {
            var bf = prefab.GetComponentInChildren<BirdFollower>(true);
            if (bf == null)
            {
                Debug.LogError("[AssignSoundClips] El prefab Player no contiene BirdFollower.");
                return;
            }

            var so = new SerializedObject(bf);
            AssignSerialized(so, "sfxDive",    dive);
            AssignSerialized(so, "sfxRummage", rummage);
            AssignSerialized(so, "sfxFound",   found);
            AssignSerialized(so, "sfxCollect", collect);
            so.ApplyModifiedPropertiesWithoutUndo();

            PrefabUtility.SaveAsPrefabAsset(prefab, PlayerPrefabPath);
            Debug.Log("[AssignSoundClips] BirdFollower del prefab Player actualizado con los 4 SFX.");
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(prefab);
        }
    }

    private static void AssignSoundsInUIScene(AudioClip click, AudioClip pasos)
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            Debug.LogWarning("[AssignSoundClips] Cancelado por el usuario al guardar la escena actual.");
            return;
        }

        var scene = EditorSceneManager.OpenScene(UIScenePath, OpenSceneMode.Single);

        Sounds sounds = null;
        foreach (var root in scene.GetRootGameObjects())
        {
            sounds = root.GetComponentInChildren<Sounds>(true);
            if (sounds != null) break;
        }

        if (sounds == null)
        {
            Debug.LogError("[AssignSoundClips] No hay componente Sounds en " + UIScenePath);
            return;
        }

        var so = new SerializedObject(sounds);
        AssignSerialized(so, "sonidoClickMapa", click);
        AssignSerialized(so, "sonidoPasos",     pasos);
        so.ApplyModifiedPropertiesWithoutUndo();

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("[AssignSoundClips] Sounds (en UI.unity) actualizado con SonidoClickMapa y SonidoPasos.");
    }

    private static void AssignSerialized(SerializedObject so, string propertyName, Object value)
    {
        var prop = so.FindProperty(propertyName);
        if (prop == null)
        {
            Debug.LogError("[AssignSoundClips] No se encontró el campo serializado '" + propertyName + "' en " + so.targetObject);
            return;
        }
        prop.objectReferenceValue = value;
    }
}
