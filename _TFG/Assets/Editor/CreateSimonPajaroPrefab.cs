using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Linq;

public static class CreateSimonPajaroPrefab
{
    [MenuItem("Tools/Recrear SimonMiniGameRoot Prefab")]
    static void CreatePrefab()
    {
        const string scenePath  = "Assets/Scenes/SimonSaysPajaro.unity";
        const string prefabPath = "Assets/Resources/Prefabs/SimonMiniGameRoot.prefab";

        // Hijos del Canvas que conservamos
        string[] keepInCanvas    = { "PanelVentana", "Contador", "StartButton", "BackButtonPanel" };
        // Hijos de PanelVentana que conservamos
        string[] keepInPanelVent = { "SimonSaysCircles", "Pajaro", "LetraSecuencia", "EnergyRoundsWon" };

        var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);

        SimonGameManagerPajaro gmComp = null;
        Canvas mainCanvas = null;

        foreach (var root in scene.GetRootGameObjects())
        {
            if (gmComp == null)
                gmComp = root.GetComponentInChildren<SimonGameManagerPajaro>(true);
            if (mainCanvas == null)
                mainCanvas = root.GetComponentInChildren<Canvas>(true);
        }

        if (gmComp == null || mainCanvas == null)
        {
            Debug.LogError("[CreateSimonPajaroPrefab] No se encontró SimonGameManagerPajaro o Canvas.");
            EditorSceneManager.CloseScene(scene, false);
            return;
        }

        var prefabRoot = new GameObject("SimonMiniGameRoot");

        var gmCopy     = Object.Instantiate(gmComp.gameObject);
        var canvasCopy = Object.Instantiate(mainCanvas.gameObject);
        gmCopy.name     = "GameManager";
        canvasCopy.name = "Canvas";

        gmCopy.transform.SetParent(prefabRoot.transform, false);
        canvasCopy.transform.SetParent(prefabRoot.transform, false);

        // Eliminar hijos innecesarios del Canvas
        var ct = canvasCopy.transform;
        for (int i = ct.childCount - 1; i >= 0; i--)
        {
            var child = ct.GetChild(i);
            if (!keepInCanvas.Contains(child.name))
                Object.DestroyImmediate(child.gameObject);
        }

        // Eliminar hijos innecesarios de PanelVentana
        var panelVentana = canvasCopy.transform.Find("PanelVentana");
        if (panelVentana != null)
        {
            for (int i = panelVentana.childCount - 1; i >= 0; i--)
            {
                var child = panelVentana.GetChild(i);
                if (!keepInPanelVent.Contains(child.name))
                    Object.DestroyImmediate(child.gameObject);
            }
        }
        else
        {
            Debug.LogWarning("[CreateSimonPajaroPrefab] No se encontró PanelVentana dentro del Canvas.");
        }

        // Reasignar referencias del GameManager copiado
        var gmNew = gmCopy.GetComponent<SimonGameManagerPajaro>();

        // Círculos: solo los padres (Image con RaycastTarget=true), ordenados por index
        var circlesCopy = canvasCopy.GetComponentsInChildren<CircleButtonPajaro>(true)
            .Where(c => {
                var img = c.GetComponent<UnityEngine.UI.Image>();
                return img != null && img.raycastTarget;
            })
            .OrderBy(c => c.index)
            .ToArray();

        if (circlesCopy.Length == 8)
            gmNew.circles = circlesCopy;
        else
            Debug.LogWarning($"[CreateSimonPajaroPrefab] Se esperaban 8 círculos padre, se encontraron {circlesCopy.Length}");

        // Pajaro
        var pajaroT = panelVentana != null ? panelVentana.Find("Pajaro") : null;
        if (pajaroT != null)
            gmNew.pajaro = pajaroT.GetComponent<UnityEngine.UI.Image>();
        else
            Debug.LogWarning("[CreateSimonPajaroPrefab] No se encontró Pajaro.");

        // Contador TextMeshPro
        var contadorT = canvasCopy.transform.Find("Contador");
        if (contadorT != null)
            gmNew.contadorText = contadorT.GetComponentInChildren<TMPro.TextMeshProUGUI>(true);
        else
            Debug.LogWarning("[CreateSimonPajaroPrefab] No se encontró Contador.");

        // StartButton
        var startBtnT = canvasCopy.transform.Find("StartButton");
        if (startBtnT != null)
            gmNew.startButton = startBtnT.GetComponent<UnityEngine.UI.Button>();
        else
            Debug.LogWarning("[CreateSimonPajaroPrefab] No se encontró StartButton.");

        // BackButton (dentro de BackButtonPanel)
        var backPanelT = canvasCopy.transform.Find("BackButtonPanel");
        if (backPanelT != null)
        {
            var backBtn = backPanelT.GetComponentInChildren<UnityEngine.UI.Button>(true);
            if (backBtn != null)
                gmNew.backButton = backBtn;
        }
        else
            Debug.LogWarning("[CreateSimonPajaroPrefab] No se encontró BackButtonPanel.");

        // LetterDisplayUI — letterImage y energyText
        var letraT = panelVentana != null ? panelVentana.Find("LetraSecuencia") : null;
        if (letraT != null)
        {
            var ldu = letraT.GetComponent<LetterDisplayUI>();
            if (ldu != null)
            {
                ldu.letterImage = letraT.GetComponent<UnityEngine.UI.Image>();
                var energyT = panelVentana.Find("EnergyRoundsWon");
                if (energyT != null)
                    ldu.energyText = energyT.GetComponent<TMPro.TextMeshProUGUI>();
            }
        }
        else
            Debug.LogWarning("[CreateSimonPajaroPrefab] No se encontró LetraSecuencia.");

        PrefabUtility.SaveAsPrefabAsset(prefabRoot, prefabPath);

        Object.DestroyImmediate(prefabRoot);
        EditorSceneManager.CloseScene(scene, false);

        AssetDatabase.Refresh();
        Debug.Log($"[CreateSimonPajaroPrefab] Prefab guardado en {prefabPath}");
    }
}
