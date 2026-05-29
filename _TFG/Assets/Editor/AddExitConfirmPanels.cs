using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using TMPro;

/// <summary>
/// Menús de Editor que añaden, de forma persistente y reproducible, los paneles de
/// confirmación de salida que pide el tutor: uno en el prefab del Simon Pájaro
/// (Assets/Resources/Prefabs/SimonMiniGameRoot.prefab) y otro —junto con el botón
/// principal de "Salir de la isla"— en la escena del mapa
/// (Assets/Scenes/ProceduralTerrain.unity).
///
/// La UI se genera por código usando la API de Unity para que los GameObjects,
/// componentes y referencias OnClick queden serializados correctamente. Después se
/// puede maquetar/skinear desde el Inspector como cualquier otro Canvas.
/// </summary>
public static class AddExitConfirmPanels
{
    private const string PrefabPath = "Assets/Resources/Prefabs/SimonMiniGameRoot.prefab";
    private const string ScenePath  = "Assets/Scenes/ProceduralTerrain.unity";
    private const string InfiniteScenePath = "Assets/Scenes/SimonSays.unity";

    private const string PanelName = "ExitConfirmPanel";
    private const string IslandExitGOName = "IslandExitController";
    private const string IslandExitButtonName = "BotonSalirIsla";

    // -------- MENÚS --------

    [MenuItem("Tools/TFG/Crear panel de confirmación en Simon Pájaro (prefab)")]
    public static void AddPanelToSimonPajaroPrefab()
    {
        var prefab = PrefabUtility.LoadPrefabContents(PrefabPath);
        if (prefab == null)
        {
            Debug.LogError($"[AddExitConfirmPanels] No se encontró el prefab en {PrefabPath}");
            return;
        }

        try
        {
            var manager = prefab.GetComponentInChildren<SimonGameManagerPajaro>(true);
            if (manager == null)
            {
                Debug.LogError("[AddExitConfirmPanels] El prefab no contiene SimonGameManagerPajaro.");
                return;
            }

            var canvas = prefab.GetComponentInChildren<Canvas>(true);
            if (canvas == null)
            {
                Debug.LogError("[AddExitConfirmPanels] El prefab no contiene Canvas.");
                return;
            }

            // Si ya hay un panel con ese nombre, no duplicar.
            var existing = canvas.transform.Find(PanelName);
            if (existing != null)
            {
                Debug.LogWarning("[AddExitConfirmPanels] Ya existe " + PanelName + " en el prefab. Reemplazándolo.");
                Object.DestroyImmediate(existing.gameObject);
            }

            var panel = BuildConfirmPanel(
                canvas.transform,
                "¿Salir del minijuego?",
                manager,
                onYesMethod: nameof(SimonGameManagerPajaro.ConfirmExit),
                onNoMethod:  nameof(SimonGameManagerPajaro.CancelExit));

            // Asignar referencia al campo serializado del manager.
            AssignSerializedReference(manager, "exitConfirmPanel", panel);

            // Si existe BackButtonPanel (el botón de salir ya presente en el Canvas),
            // wirear su OnClick al OnBackButtonPressed del manager.
            TryWireExistingBackButton(canvas, manager);

            // Dejarlo desactivado por defecto.
            panel.SetActive(false);

            // Guardar el prefab.
            PrefabUtility.SaveAsPrefabAsset(prefab, PrefabPath);
            AssetDatabase.SaveAssets();
            Debug.Log("[AddExitConfirmPanels] Panel creado en el prefab " + PrefabPath);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(prefab);
        }
    }

    [MenuItem("Tools/TFG/Crear panel de confirmación en Simon infinito (escena)")]
    public static void AddPanelToSimonInfiniteScene()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            Debug.Log("[AddExitConfirmPanels] Operación cancelada por el usuario.");
            return;
        }
        var scene = EditorSceneManager.OpenScene(InfiniteScenePath, OpenSceneMode.Single);

        SimonGameManager manager = null;
        Canvas canvas = null;
        foreach (var root in scene.GetRootGameObjects())
        {
            if (manager == null) manager = root.GetComponentInChildren<SimonGameManager>(true);
            if (canvas == null) canvas = root.GetComponentInChildren<Canvas>(true);
            if (manager != null && canvas != null) break;
        }

        if (manager == null)
        {
            Debug.LogError("[AddExitConfirmPanels] No hay SimonGameManager en " + InfiniteScenePath);
            return;
        }
        if (canvas == null)
        {
            Debug.LogError("[AddExitConfirmPanels] No hay Canvas en " + InfiniteScenePath);
            return;
        }

        // Reemplazar panel anterior si existía.
        var existing = canvas.transform.Find(PanelName);
        if (existing != null) Object.DestroyImmediate(existing.gameObject);

        var panel = BuildConfirmPanel(
            canvas.transform,
            "¿Salir del minijuego?",
            manager,
            onYesMethod: nameof(SimonGameManager.ConfirmExit),
            onNoMethod:  nameof(SimonGameManager.CancelExit));

        AssignSerializedReference(manager, "exitConfirmPanel", panel);
        panel.SetActive(false);

        // Wirear el backButton del manager (campo público asignado en el Inspector)
        // a OnBackButtonPressed, para que abra el panel en vez de salir directamente.
        // Antes hay que LIMPIAR cualquier listener previo (típicamente un Cambiar_A_Escena
        // wireado a mano que provocaba la salida inmediata además del panel).
        if (manager.backButton != null)
        {
            int removed = ClearAllPersistentListeners(manager.backButton.onClick);
            if (removed > 0)
            {
                Debug.Log($"[AddExitConfirmPanels] Limpiados {removed} listener(s) previos del backButton.");
            }
            WirePersistent(manager.backButton.onClick, manager, nameof(SimonGameManager.OnBackButtonPressed));
            Debug.Log("[AddExitConfirmPanels] backButton enlazado a OnBackButtonPressed.");
        }
        else
        {
            Debug.LogWarning("[AddExitConfirmPanels] El SimonGameManager no tiene backButton asignado; wirealo manualmente al OnBackButtonPressed.");
        }

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("[AddExitConfirmPanels] Panel creado en la escena " + InfiniteScenePath);
    }

    [MenuItem("Tools/TFG/Crear panel y botón de salida en escena del mapa")]
    public static void AddExitToIslandScene()
    {
        // Pedir a Unity que guarde la escena abierta si está sucia, y abrir la del mapa.
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            Debug.Log("[AddExitConfirmPanels] Operación cancelada por el usuario.");
            return;
        }
        var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);

        // Buscar un Canvas en la escena.
        Canvas canvas = null;
        foreach (var root in scene.GetRootGameObjects())
        {
            canvas = root.GetComponentInChildren<Canvas>(true);
            if (canvas != null) break;
        }
        if (canvas == null)
        {
            Debug.LogError("[AddExitConfirmPanels] No hay ningún Canvas en la escena " + ScenePath);
            return;
        }

        // GameObject que llevará el IslandExitButton (controlador lógico).
        IslandExitButton islandExit = Object.FindFirstObjectByType<IslandExitButton>();
        if (islandExit == null)
        {
            var holder = new GameObject(IslandExitGOName);
            islandExit = holder.AddComponent<IslandExitButton>();
        }

        // Botón principal en la esquina superior derecha del Canvas.
        var existingBtn = canvas.transform.Find(IslandExitButtonName);
        if (existingBtn != null) Object.DestroyImmediate(existingBtn.gameObject);

        var mainBtn = BuildCornerButton(
            canvas.transform,
            "Salir",
            islandExit,
            onClickMethod: nameof(IslandExitButton.OnExitButtonPressed));
        mainBtn.name = IslandExitButtonName;

        // Asignar la referencia al botón principal para que el IslandExitButton pueda
        // ocultarlo/mostrarlo según SimonGameManagerPajaro.IsActive.
        AssignSerializedReference(islandExit, "mainExitButton", mainBtn.gameObject);

        // Panel de confirmación.
        var existingPanel = canvas.transform.Find(PanelName);
        if (existingPanel != null) Object.DestroyImmediate(existingPanel.gameObject);

        var panel = BuildConfirmPanel(
            canvas.transform,
            "¿Salir de la isla?",
            islandExit,
            onYesMethod: nameof(IslandExitButton.ConfirmExit),
            onNoMethod:  nameof(IslandExitButton.CancelExit));

        AssignSerializedReference(islandExit, "exitConfirmPanel", panel);
        panel.SetActive(false);

        // Guardar la escena.
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("[AddExitConfirmPanels] Botón y panel creados en la escena " + ScenePath);
    }

    // -------- CONSTRUCCIÓN DE UI --------

    private static GameObject BuildConfirmPanel(Transform parent, string title, Object target,
                                                string onYesMethod, string onNoMethod)
    {
        // Raíz del panel: ocupa todo el Canvas y bloquea clics por debajo.
        var panel = NewUIObject(PanelName, parent);
        AddImage(panel, new Color(0f, 0f, 0f, 0.6f));
        Stretch(panel.GetComponent<RectTransform>());
        panel.transform.SetAsLastSibling();

        // Caja central.
        var box = NewUIObject("Box", panel.transform);
        AddImage(box, new Color(0.96f, 0.96f, 0.96f, 1f));
        var boxRT = box.GetComponent<RectTransform>();
        boxRT.anchorMin = boxRT.anchorMax = new Vector2(0.5f, 0.5f);
        boxRT.pivot = new Vector2(0.5f, 0.5f);
        boxRT.sizeDelta = new Vector2(720f, 360f);
        boxRT.anchoredPosition = Vector2.zero;

        // Texto del título.
        var titleGO = NewUIObject("Title", box.transform);
        var titleRT = titleGO.GetComponent<RectTransform>();
        titleRT.anchorMin = new Vector2(0f, 0.45f);
        titleRT.anchorMax = new Vector2(1f, 1f);
        titleRT.offsetMin = new Vector2(30f, 10f);
        titleRT.offsetMax = new Vector2(-30f, -20f);
        var tmp = titleGO.AddComponent<TextMeshProUGUI>();
        tmp.text = title;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontSize = 44f;
        tmp.color = Color.black;
        tmp.enableWordWrapping = true;
        tmp.raycastTarget = false;

        // Botones Sí / No.
        BuildBoxButton(box.transform, "Sí", new Vector2(-150f, -95f),
                       new Color(0.30f, 0.75f, 0.30f), target, onYesMethod);
        BuildBoxButton(box.transform, "No", new Vector2( 150f, -95f),
                       new Color(0.85f, 0.30f, 0.30f), target, onNoMethod);

        return panel;
    }

    private static Button BuildBoxButton(Transform parent, string label, Vector2 anchoredPos,
                                         Color color, Object target, string methodName)
    {
        var go = NewUIObject(label, parent);
        AddImage(go, color);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(240f, 90f);
        rt.anchoredPosition = anchoredPos;

        var btn = go.AddComponent<Button>();
        btn.targetGraphic = go.GetComponent<Image>();
        WirePersistent(btn.onClick, target, methodName);

        AddCenteredLabel(go.transform, label, 38f, Color.white);
        return btn;
    }

    private static Button BuildCornerButton(Transform parent, string label, Object target, string onClickMethod)
    {
        var go = NewUIObject("BotonSalirIsla", parent);
        AddImage(go, new Color(0.18f, 0.18f, 0.18f, 0.9f));
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(1f, 1f);
        rt.pivot = new Vector2(1f, 1f);
        rt.sizeDelta = new Vector2(200f, 80f);
        rt.anchoredPosition = new Vector2(-30f, -30f);

        var btn = go.AddComponent<Button>();
        btn.targetGraphic = go.GetComponent<Image>();
        WirePersistent(btn.onClick, target, onClickMethod);

        AddCenteredLabel(go.transform, label, 34f, Color.white);
        return btn;
    }

    private static void AddCenteredLabel(Transform parent, string text, float fontSize, Color color)
    {
        var go = NewUIObject("Label", parent);
        Stretch(go.GetComponent<RectTransform>());
        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontSize = fontSize;
        tmp.color = color;
        tmp.raycastTarget = false;
    }

    // -------- UTILIDADES --------

    private static GameObject NewUIObject(string name, Transform parent)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        return go;
    }

    private static Image AddImage(GameObject go, Color color)
    {
        var img = go.AddComponent<Image>();
        img.color = color;
        img.raycastTarget = true;
        return img;
    }

    private static void Stretch(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    /// <summary>
    /// Asigna un GameObject a un campo serializado privado por nombre, usando
    /// SerializedObject para que la asignación sobreviva al guardado del prefab/escena.
    /// </summary>
    private static void AssignSerializedReference(Object owner, string propertyName, Object value)
    {
        var so = new SerializedObject(owner);
        var prop = so.FindProperty(propertyName);
        if (prop == null)
        {
            Debug.LogError($"[AddExitConfirmPanels] No se encontró el campo '{propertyName}' en {owner}.");
            return;
        }
        prop.objectReferenceValue = value;
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    /// <summary>
    /// Añade un listener persistente (visible en el Inspector del Button) al UnityEvent.
    /// Idempotente: si ya existe un listener para el mismo target+método, no lo duplica.
    /// </summary>
    private static void WirePersistent(UnityEvent ev, Object target, string methodName)
    {
        // Evitar duplicar si ya estaba enlazado a este target+método.
        for (int i = ev.GetPersistentEventCount() - 1; i >= 0; i--)
        {
            if (ev.GetPersistentTarget(i) == target && ev.GetPersistentMethodName(i) == methodName)
            {
                UnityEventTools.RemovePersistentListener(ev, i);
            }
        }

        var del = System.Delegate.CreateDelegate(
            typeof(UnityAction), target, methodName, ignoreCase: false, throwOnBindFailure: false) as UnityAction;
        if (del == null)
        {
            Debug.LogError($"[AddExitConfirmPanels] No se pudo crear delegado para {target.GetType().Name}.{methodName}().");
            return;
        }
        UnityEventTools.AddPersistentListener(ev, del);
    }

    /// <summary>
    /// Localiza el BackButtonPanel del Canvas del Simon Pájaro y enlaza su OnClick
    /// con SimonGameManagerPajaro.OnBackButtonPressed. Si no existe, deja un aviso.
    /// </summary>
    private static void TryWireExistingBackButton(Canvas canvas, SimonGameManagerPajaro manager)
    {
        var backPanel = canvas.transform.Find("BackButtonPanel");
        if (backPanel == null)
        {
            Debug.LogWarning("[AddExitConfirmPanels] No se encontró 'BackButtonPanel' en el Canvas del prefab; wirea el botón manualmente al OnBackButtonPressed.");
            return;
        }

        var btn = backPanel.GetComponent<Button>();
        if (btn == null) btn = backPanel.GetComponentInChildren<Button>(true);
        if (btn == null)
        {
            Debug.LogWarning("[AddExitConfirmPanels] 'BackButtonPanel' existe pero no contiene Button; wirea manualmente.");
            return;
        }

        int removed = ClearAllPersistentListeners(btn.onClick);
        if (removed > 0)
        {
            Debug.Log($"[AddExitConfirmPanels] Limpiados {removed} listener(s) previos del BackButtonPanel.");
        }
        WirePersistent(btn.onClick, manager, nameof(SimonGameManagerPajaro.OnBackButtonPressed));
        Debug.Log("[AddExitConfirmPanels] BackButtonPanel enlazado a OnBackButtonPressed.");
    }

    /// <summary>
    /// Elimina TODOS los listeners persistentes del UnityEvent (los configurados en
    /// el Inspector). Devuelve cuántos se eliminaron. Útil para asegurarse de que
    /// un botón solo dispara la acción que queremos.
    /// </summary>
    private static int ClearAllPersistentListeners(UnityEvent ev)
    {
        int count = ev.GetPersistentEventCount();
        for (int i = count - 1; i >= 0; i--)
        {
            UnityEventTools.RemovePersistentListener(ev, i);
        }
        return count;
    }
}
