/**
 * @file Store.cs
 * @brief Gestiona el catálogo de elementos comprables, desbloqueos y operaciones de tienda.
 * @author Hortensia Studio - Beatriz Ceballos Vidal
 * @date 2026
 * @details Archivo perteneciente al proyecto Cloud Wander. La documentación se ha preparado con comentarios XML compatibles con Doxygen para describir clases, campos y métodos relevantes.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Estructura de datos que describe un elemento de color o personalización dentro de la tienda.
/// </summary>
[System.Serializable]
public struct ColorItem
{
    /// <summary>
    /// Campo de tipo string utilizado para almacenar o configurar name.
    /// </summary>
    public string name;
    /// <summary>
    /// Campo de tipo string utilizado para almacenar o configurar section.
    /// </summary>
    public string section;
    /// <summary>
    /// Color utilizado para representar color.
    /// </summary>
    public Color color;
    /// <summary>
    /// Campo de tipo int utilizado para almacenar o configurar price.
    /// </summary>
    public int price;
}
/// <summary>
/// Gestiona el catálogo de elementos comprables, desbloqueos y operaciones de tienda.
/// </summary>
public class Store : MonoBehaviour
{
    /// <summary>
    /// Propiedad que expone o modifica shop slot 1.
    /// </summary>
    public ColorItem shopSlot1 { get; private set; }
    /// <summary>
    /// Propiedad que expone o modifica shop slot 2.
    /// </summary>
    public ColorItem shopSlot2 { get; private set; }
    /// <summary>
    /// Propiedad que expone o modifica shop slot 3.
    /// </summary>
    public ColorItem shopSlot3 { get; private set; }

    /// <summary>
    /// Botón de interfaz asociado a item 1 button.
    /// </summary>
    [Header("UI - ShopItems")]
    public Button item1Button;
    /// <summary>
    /// Botón de interfaz asociado a item 2 button.
    /// </summary>
    public Button item2Button;
    /// <summary>
    /// Botón de interfaz asociado a item 3 button.
    /// </summary>
    public Button item3Button;

    /// <summary>
    /// Campo de tipo Image utilizado para almacenar o configurar item 1 image.
    /// </summary>
    [Header("UI - Images")]
    public Image item1Image;
    /// <summary>
    /// Campo de tipo Image utilizado para almacenar o configurar item 2 image.
    /// </summary>
    public Image item2Image;
    /// <summary>
    /// Campo de tipo Image utilizado para almacenar o configurar item 3 image.
    /// </summary>
    public Image item3Image;

    /// <summary>
    /// Campo de tipo TMP_Text utilizado para almacenar o configurar item 1 price.
    /// </summary>
    [Header("UI - Prices")]
    public TMP_Text item1Price;
    /// <summary>
    /// Campo de tipo TMP_Text utilizado para almacenar o configurar item 2 price.
    /// </summary>
    public TMP_Text item2Price;
    /// <summary>
    /// Campo de tipo TMP_Text utilizado para almacenar o configurar item 3 price.
    /// </summary>
    public TMP_Text item3Price;

    /// <summary>
    /// Referencia visual o sprite asociado a base top sprite.
    /// </summary>
    public Sprite baseTopSprite;
    /// <summary>
    /// Referencia visual o sprite asociado a base mid sprite.
    /// </summary>
    public Sprite baseMidSprite;
    /// <summary>
    /// Referencia visual o sprite asociado a base bot sprite.
    /// </summary>
    public Sprite baseBotSprite;
    /// <summary>
    /// Referencia visual o sprite asociado a base cesta sprite.
    /// </summary>
    public Sprite baseCestaSprite;

    /// <summary>
    /// Campo de tipo CustomMenu utilizado para almacenar o configurar custom menu.
    /// </summary>
    [Header("Scripts")]
    private CustomMenu customMenu;
    //TODO: Invocar a CollectionMenu

    /// <summary>
    /// Color utilizado para representar color queue.
    /// </summary>
    private Queue<ColorItem> colorQueue = new Queue<ColorItem>();
    /// <summary>
    /// Colección de color pool utilizada por este componente.
    /// </summary>
    public List<ColorItem> colorPool = new List<ColorItem>();

    /// <summary>
    /// Tiempo o duración asociado a refresh time.
    /// </summary>
    public const float REFRESH_TIME = 180f;
    /// <summary>
    /// Referencia de interfaz utilizada para mostrar o actualizar timer text.
    /// </summary>
    [Header("Timer UI")]
    public TMP_Text timerText;

    /// <summary>
    /// Tiempo o duración asociado a remaining time.
    /// </summary>
    private float remainingTime;
    /// <summary>
    /// Campo de tipo System.Action utilizado para almacenar o configurar on shop refreshed.
    /// </summary>
    public event System.Action OnShopRefreshed;

    /// <summary>
    /// Inicializa el componente cuando la escena ya está cargada y lista para comenzar.
    /// </summary>
    void Start()
    {
        customMenu = GetComponent<CustomMenu>();
        // collectionMenu = GetComponent<CollectionMenu>();

        BuildQueue();
        RefreshShop();
        StartCoroutine(ShopTimer());

        remainingTime = REFRESH_TIME;
        StartCoroutine(UpdateTimer());
    }
    /// <summary>
    /// Ejecuta la lógica asociada a on item selected dentro de Store.
    /// </summary>
    /// <param name="button">Parámetro button empleado por el método.</param>
    public void OnItemSelected(Button button)
    {
        ColorItem item;

        // Saber que slot corresponde al boton pulsado
        if (button == item1Button)
        {
            item = shopSlot1;
        }
        else if (button == item2Button)
        {
            item = shopSlot2;
        }
        else if (button == item3Button)
        {
            item = shopSlot3;
        }
        else
        {
            Debug.LogWarning("Boton no reconocido");
            return;
        }

        // TODO: DIFERENCIAR DE STICKER O COLOR, SI ES STICKER EN VEZ DE ANADIR COLOR AL CUSTOMMENU, ANADIR EL STICKER EN LA SECCION CORRESPONDIENTE(COLLECTION, para que sea mas facil hacer una funcion de anadir sticker en el prpio menu de colleccion collectionMenu.AddSticker()).

     
        // Restar monedas
        if (GameManager.Instance.SpendMoney(item.price))
        {
            Debug.LogWarning("Entra");
            customMenu.AddColor((Color32)item.color);
            Debug.LogWarning("Sale");

            RemoveItem(item);

            button.interactable = false;

            Debug.Log($"Comprado: {item.name}");
        }
    }

    /// <summary>
    /// Elimina item del estado gestionado por el componente.
    /// </summary>
    /// <param name="item">Parámetro item empleado por el método.</param>
    void RemoveItem(ColorItem item)
    {
        colorPool.RemoveAll(x => x.name == item.name);
    }

    /// <summary>
    /// Elimina item from queue del estado gestionado por el componente.
    /// </summary>
    /// <param name="item">Parámetro item empleado por el método.</param>
    void RemoveItemFromQueue(ColorItem item)
    {
        Queue<ColorItem> newQueue = new Queue<ColorItem>();

        while (colorQueue.Count > 0)
        {
            ColorItem current = colorQueue.Dequeue();

            if (current.name != item.name)
            {
                newQueue.Enqueue(current);
            }
        }

        colorQueue = newQueue;
    }
    /// <summary>
    /// Ejecuta la lógica asociada a shop timer dentro de Store.
    /// </summary>
    /// <returns>Corrutina que permite ejecutar la operación de forma diferida en Unity.</returns>
    IEnumerator ShopTimer()
    {
        while (true)
        {
            yield return new WaitForSeconds(REFRESH_TIME);
            RefreshShop();
        }
    }

    /// <summary>
    /// Ejecuta la lógica asociada a refresh shop dentro de Store.
    /// </summary>
    void RefreshShop()
    {
        List<ColorItem> result = GetThreeRandom();

        shopSlot1 = result[0];
        shopSlot2 = result[1];
        shopSlot3 = result[2];

        item1Button.interactable = true;
        item2Button.interactable = true;
        item3Button.interactable = true;

        UpdateUI();
    }

    /// <summary>
    /// Ejecuta la lógica asociada a force refresh shop dentro de Store.
    /// </summary>
    public void ForceRefreshShop()
    {
       if(GameManager.Instance.SpendGems(200))
       {
           RefreshShop();
           remainingTime = REFRESH_TIME; // Reiniciar timer
       }
    }
    /// <summary>
    /// Actualiza timer para reflejar el estado actual del sistema.
    /// </summary>
    /// <returns>Corrutina que permite ejecutar la operación de forma diferida en Unity.</returns>
    IEnumerator UpdateTimer()
    {
        while (true)
        {
            UpdateTimerUI();

            yield return new WaitForSeconds(1f);

            remainingTime--;

            if (remainingTime <= 0)
            {
                remainingTime = REFRESH_TIME;
            }
        }
    }
    /// <summary>
    /// Actualiza timer ui para reflejar el estado actual del sistema.
    /// </summary>
    void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(remainingTime / 60f);
        int seconds = Mathf.FloorToInt(remainingTime % 60f);

        timerText.text = $"{minutes}:{seconds:00}";
    }
    /// <summary>
    /// Actualiza ui para reflejar el estado actual del sistema.
    /// </summary>
    void UpdateUI()
    {
        // TODO: Lo de abajo seria si el item es un color, si hay stickers, habria que diferenciarlo y poner la imagen del sticker en vez de la base.
        item1Image.sprite = GetBaseSprite(shopSlot1.section);
        item2Image.sprite = GetBaseSprite(shopSlot2.section);
        item3Image.sprite = GetBaseSprite(shopSlot3.section);

        item1Image.color = shopSlot1.color;
        item2Image.color = shopSlot2.color;
        item3Image.color = shopSlot3.color;

        item1Price.text = shopSlot1.price.ToString();
        item2Price.text = shopSlot2.price.ToString();
        item3Price.text = shopSlot3.price.ToString();
    }
    /// <summary>
    /// Obtiene base sprite a partir del estado actual del sistema.
    /// </summary>
    /// <param name="section">Parámetro section empleado por el método.</param>
    /// <returns>Instancia o valor de tipo Sprite resultante de la operación.</returns>
    Sprite GetBaseSprite(string section)
    {
        switch (section)
        {
            case "Top": return baseTopSprite;
            case "Mid": return baseMidSprite;
            case "Bot": return baseBotSprite;
            case "Cesta": return baseCestaSprite;
            default: return baseTopSprite;
        }
    }

    /// <summary>
    /// Obtiene three random a partir del estado actual del sistema.
    /// </summary>
    /// <returns>Instancia o valor de tipo List<ColorItem> resultante de la operación.</returns>
    List<ColorItem> GetThreeRandom()
    {
        // TODO: Unir las dos queue en una y sacar de ahi los 3 items random. 

        List<ColorItem> pool = new List<ColorItem>(colorPool);
        List<ColorItem> result = new List<ColorItem>();

        for (int i = 0; i < 3; i++)
        {
            int idx = Random.Range(0, pool.Count);
            result.Add(pool[idx]);
            pool.RemoveAt(idx);
        }

        return result;
    }

    /// <summary>
    /// Ejecuta la lógica asociada a build queue dentro de Store.
    /// </summary>
    void BuildQueue()
    {
        colorPool.Clear();

        colorPool.Add(new ColorItem { name = "Top color 1", section = "Top", color = HexToColor("885053"), price = 150 });
        colorPool.Add(new ColorItem { name = "Top color 2", section = "Top", color = HexToColor("628395"), price = 300 });
        colorPool.Add(new ColorItem { name = "Top color 3", section = "Top", color = HexToColor("638475"), price = 900 });
        colorPool.Add(new ColorItem { name = "Top color 4", section = "Top", color = HexToColor("DB5461"), price = 1200 });

        colorPool.Add(new ColorItem { name = "Mid color 1", section = "Mid", color = HexToColor("FE5F55"), price = 150 });
        colorPool.Add(new ColorItem { name = "Mid color 2", section = "Mid", color = HexToColor("DFD5A5"), price = 300 });
        colorPool.Add(new ColorItem { name = "Mid color 3", section = "Mid", color = HexToColor("90E39A"), price = 900 });
        colorPool.Add(new ColorItem { name = "Mid color 4", section = "Mid", color = HexToColor("FFD9CE"), price = 1200 });

        colorPool.Add(new ColorItem { name = "Bot color 1", section = "Bot", color = HexToColor("777DA7"), price = 150 });
        colorPool.Add(new ColorItem { name = "Bot color 2", section = "Bot", color = HexToColor("96897B"), price = 300 });
        colorPool.Add(new ColorItem { name = "Bot color 3", section = "Bot", color = HexToColor("DDF093"), price = 900 });
        colorPool.Add(new ColorItem { name = "Bot color 4", section = "Bot", color = HexToColor("593C8F"), price = 1200 });

        colorPool.Add(new ColorItem { name = "Cesta color 1", section = "Cesta", color = HexToColor("94C9A9"), price = 150 });
        colorPool.Add(new ColorItem { name = "Cesta color 2", section = "Cesta", color = HexToColor("DBAD6A"), price = 300 });
        colorPool.Add(new ColorItem { name = "Cesta color 3", section = "Cesta", color = HexToColor("F6D0B1"), price = 900 });
        colorPool.Add(new ColorItem { name = "Cesta color 4", section = "Cesta", color = HexToColor("8EF9F3"), price = 1200 });

        colorQueue = new Queue<ColorItem>(colorPool);
    }

    //TODO: List para stickers


    /// <summary>
    /// Obtiene shop items a partir del estado actual del sistema.
    /// </summary>
    /// <returns>Instancia o valor de tipo List<ColorItem> resultante de la operación.</returns>
    public List<ColorItem> GetShopItems()
    {
        return new List<ColorItem>(colorQueue);
    }

    /// <summary>
    /// Establece o actualiza shop items dentro del sistema.
    /// </summary>
    /// <param name="items">Parámetro items empleado por el método.</param>
    public void SetShopItems(List<ColorItem> items)
    {
        colorQueue = new Queue<ColorItem>(items);
    }

    /// <summary>
    /// Ejecuta la lógica asociada a hex to color dentro de Store.
    /// </summary>
    /// <param name="hex">Parámetro hex empleado por el método.</param>
    /// <returns>Instancia o valor de tipo Color resultante de la operación.</returns>
    public Color HexToColor(string hex)
    {
        ColorUtility.TryParseHtmlString("#" + hex, out Color c);
        return c;
    }
}
