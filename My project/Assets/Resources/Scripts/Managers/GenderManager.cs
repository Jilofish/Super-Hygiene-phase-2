using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class GenderManager : MonoBehaviour
{

    public void MakeMale()
    {
        GameDataManager.heroGender = "M";
    }

    public void MakeFem()
    {
        GameDataManager.heroGender = "F";
    }
    void Start()
    {

    }

    void Update()
    {
        
    }
}
