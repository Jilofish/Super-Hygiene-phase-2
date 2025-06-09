using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TrashManager : MonoBehaviour
{
    public GameObject[] trashPrefabs;
    public Transform[] trashSlots;   
    public Button proceedButton;
    public Image praiseImage;
    private int trashCleaned = 0;
    private int totalTrash = 6;
    public GameManager gameManager;

    void Start()
    {
        BootTracer.Log("TrashManager Start()");
        trashCleaned = 0;
    }

    public void SpawnTrash()
    {
        BootTracer.Log("SpawnTrash() started");

        List<GameObject> selectedTrash = new List<GameObject>();
        List<GameObject> pool = new List<GameObject>(trashPrefabs);

        totalTrash = Mathf.Min(trashPrefabs.Length, trashSlots.Length);

        while (selectedTrash.Count < totalTrash && pool.Count > 0)
        {
            GameObject random = pool[Random.Range(0, pool.Count)];
            if (!selectedTrash.Contains(random))
                selectedTrash.Add(random);
        }

        for (int i = 0; i < selectedTrash.Count; i++)
        {
            GameObject prefab = selectedTrash[i];
            TrashData data = prefab.GetComponent<TrashDataHolder>().trashData;

            BootTracer.Log($"Instantiating: {prefab.name} into slot {i}");

            // Instantiate the object
            GameObject obj = Instantiate(prefab, trashSlots[i].position, Quaternion.identity, trashSlots[i]);
            obj.GetComponent<TrashSorter>().enabled = false;
            obj.GetComponent<TrashTapHandler>().enabled = true;
            // Set the tag of the instantiated object here to ensure it's correctly assigned
            obj.tag = data.category.ToString(); // Assign the tag based on trash category
            Debug.Log($"Spawned trash with tag: {obj.tag}");

            // Handle the TrashTapHandler and other components
            TrashTapHandler tapHandler = obj.GetComponent<TrashTapHandler>();
            TrashDataHolder holder = obj.GetComponent<TrashDataHolder>();
            
            if (tapHandler != null && holder != null)
            {
                tapHandler.manager = this;
                tapHandler.dataHolder = holder;
                holder.trashData = data;
                tapHandler.gameManager = gameManager;
            }
            else
            {
                Debug.LogError("Missing component on instantiated object");
            }
        }

        BootTracer.Log("SpawnTrash() completed");
    }

    public void TrashPicked()
    {
        trashCleaned++;
        if (trashCleaned >= totalTrash)
        {
            proceedButton.gameObject.SetActive(true);
            praiseImage.gameObject.SetActive(true);
        }
    }
    public void ClearTrashFromSlots()
    {
        BootTracer.Log("Clearing trash from slots");

        foreach (Transform slot in trashSlots)
        {
            if (slot.childCount > 0)
            {
                foreach (Transform child in slot)
                {
                    Destroy(child.gameObject);
                }
            }
        }
        trashCleaned = 0;
        proceedButton.gameObject.SetActive(false);
    }

}
