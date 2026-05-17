using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static UnityEditor.Progress;

[System.Serializable]
public struct ColorItem
{
    public string name;
    public string section;
    public Color color;
    public int price;
}
public class Store : MonoBehaviour
{
    public ColorItem shopSlot1 { get; private set; }
    public ColorItem shopSlot2 { get; private set; }
    public ColorItem shopSlot3 { get; private set; }

    [Header("UI - ShopItems")]
    public Button item1Button;
    public Button item2Button;
    public Button item3Button;

    [Header("UI - Images")]
    public Image item1Image;
    public Image item2Image;
    public Image item3Image;

    [Header("UI - Prices")]
    public TMP_Text item1Price;
    public TMP_Text item2Price;
    public TMP_Text item3Price;

    public Sprite baseTopSprite;
    public Sprite baseMidSprite;
    public Sprite baseBotSprite;
    public Sprite baseCestaSprite;

    [Header("Scripts")]
    private CustomMenu customMenu;
    //TODO: Invocar a CollectionMenu

    private Queue<ColorItem> colorQueue = new Queue<ColorItem>();
    public List<ColorItem> colorPool = new List<ColorItem>();

    public const float REFRESH_TIME = 180f;
    [Header("Timer UI")]
    public TMP_Text timerText;

    private float remainingTime;
    public event System.Action OnShopRefreshed;

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

    void RemoveItem(ColorItem item)
    {
        colorPool.RemoveAll(x => x.name == item.name);
    }

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
    IEnumerator ShopTimer()
    {
        while (true)
        {
            yield return new WaitForSeconds(REFRESH_TIME);
            RefreshShop();
        }
    }

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

    public void ForceRefreshShop()
    {
       if(GameManager.Instance.SpendGems(200))
       {
           RefreshShop();
           remainingTime = REFRESH_TIME; // Reiniciar timer
       }
    }
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
    void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(remainingTime / 60f);
        int seconds = Mathf.FloorToInt(remainingTime % 60f);

        timerText.text = $"{minutes}:{seconds:00}";
    }
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


    public List<ColorItem> GetShopItems()
    {
        return new List<ColorItem>(colorQueue);
    }

    public void SetShopItems(List<ColorItem> items)
    {
        colorQueue = new Queue<ColorItem>(items);
    }

    public Color HexToColor(string hex)
    {
        ColorUtility.TryParseHtmlString("#" + hex, out Color c);
        return c;
    }
}
