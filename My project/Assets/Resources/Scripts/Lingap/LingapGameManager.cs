using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LingapGameManager : MonoBehaviour
{
    [Header("Instantiated Objects")]
    public GameObject[] AreaGameObjects;
    public GameObject FaucetMinigame1;
    public GameObject FaucetMinigame2;
    public GameObject areaui;
    public Button ProceedButtonMG1;
    public Button ProceedButtonMG2;
    public GameObject ShirtSpawn;

    [Header("Components")]
    public AreaLoader arealoader;
    public LevelUnlockManager levelmanagerunlock;
    public ChairMoveManager chairmanager;
    public CutsceneTransitionManager taskpoints;
    public LingapUIManagement uimanagement;
    public PlayCustsceneAudio playaudio;
    
    public GameObject CreatedAreaUI;
    public GameObject ButtonLookingFor;
    public GameObject AreaGroup3;
    public GameObject AreaGroup4;
    public GameObject AreaGroup5;

    public int ButtonAudioIndex = 20;

    public void LoadGame()
    {
        Debug.Log("Game Reloaded");

        // Make sure uimanagement is always assigned
        if (uimanagement == null)
        {
            uimanagement = FindFirstObjectByType<LingapUIManagement>();
            if (uimanagement == null)
            {
                Debug.LogError("LingapUIManagement not found! UI elements may not load correctly.");
                return;
            }
        }

        // Ensure CanvasTransform is set
        if (uimanagement.CanvasTransform == null)
        {
             Canvas[] canvases = FindObjectsByType<Canvas>(FindObjectsSortMode.None);
            foreach (var canvas in canvases)
            {
                if (canvas.name != "LoaderCanvas") // skip LoaderCanvas
                {
                    uimanagement.CanvasTransform = canvas.transform;
                    break;
                }
            }

            if (uimanagement.CanvasTransform == null)
            {
                Debug.LogError("No valid Canvas found!");
                return;
            }
        }

        //Instantiate areaui correctly under Canvas
        CreatedAreaUI = Instantiate(areaui, uimanagement.CanvasTransform, false);
        CreatedAreaUI.transform.SetSiblingIndex(0);
        if (CreatedAreaUI != null)
        {
            Transform AreaTask = FindChild(CreatedAreaUI.transform, "AreaTaskChecker");
            arealoader.taskUIContainer = CreatedAreaUI.gameObject;
            arealoader.AreaTaskChecker = AreaTask;

            Transform continueButtonGO = FindChild(CreatedAreaUI.transform, "Proceed Button");

            Transform star1GO = FindChild(CreatedAreaUI.transform, "Star-1");
            Transform star2GO = FindChild(CreatedAreaUI.transform, "Star-2");
            Transform star3GO = FindChild(CreatedAreaUI.transform, "Star-3");

            Transform blackOverlayGO = FindChild(CreatedAreaUI.transform, "Black");

            ButtonLookingFor = continueButtonGO.gameObject;
            levelmanagerunlock.continueButton = continueButtonGO.gameObject;
            levelmanagerunlock.blackOverlay = blackOverlayGO.gameObject;
            levelmanagerunlock.stars = new Transform[]
            {
                FindChild(CreatedAreaUI.transform, "Star-1"),
                FindChild(CreatedAreaUI.transform, "Star-2"),
                FindChild(CreatedAreaUI.transform, "Star-3")
            };



            // Add onClick listener
            Button continueButton = continueButtonGO.GetComponent<Button>();
            if (continueButton != null)
            {
                continueButton.onClick.AddListener(() =>
                {
                    levelmanagerunlock.OnContinueButtonClicked();
                    levelmanagerunlock.DisableCurrentAreaGroup();
                    taskpoints.PlayEndingCutscene();
                });
            }
        }



    // // Instantiate level buttons and assign OnClick listeners
    // for (int i = 0; i < 6; i++)
    // {
    //     int index = i;
    //     Transform LevelCutscenes = FindChild(uimanagement.CanvasTransform, "Level Cutscenes");
    //     GameObject LevelCutscenesGO = LevelCutscenes.gameObject;
    //     Button levelButtonInstance = FindChild(LevelCutscenesGO.transform, "Area " + (index + 1) + " Button").gameObject.GetComponent<Button>();
        

    //     // Add the task UI activation
    //     levelButtonInstance.onClick.AddListener(() =>
    //     {
    //         arealoader.taskUIContainer.SetActive(true);
    //     });
    // }

        for (int h=0; h<6; h++)
        {
            arealoader.areaGroups[h] = Instantiate(AreaGameObjects[h]);
            taskpoints.taskEntryPoints[h] = arealoader.areaGroups[h];
        }

        if (chairmanager.chair == null)
        {
            GameObject chairGo=arealoader.areaGroups[1];
            Transform ChairOntableTransform = FindChild(chairGo.transform,"Chair in Table");
            chairmanager.chair= ChairOntableTransform.gameObject;
            Transform TableTransform = FindChild(chairGo.transform,"Table");
            chairmanager.table= TableTransform.gameObject;
            Transform ChairTransform = FindChild(chairGo.transform,"Chair");
            ChairMove ChairmoveComp = ChairTransform.GetComponent<ChairMove>();
            if(ChairmoveComp != null){
                ChairmoveComp.chairMoveManager = GetComponent<ChairMoveManager>();
            }
        }
        
        Button ButtonMG1 = Instantiate(ProceedButtonMG1,uimanagement.CanvasTransform,false);
        GameObject FaucetMini1 = Instantiate(FaucetMinigame1);
        FaucetMinigame1 = FaucetMini1;
        if (FaucetMini1 != null)
        {
            AreaGroup3 = arealoader.areaGroups[2];
            Transform FM1 = FindChild(AreaGroup3.transform,"Faucet");
            GameObject FM1Ref= FM1.gameObject;
            Transform FH1 = FindChild(FaucetMini1.transform,"Faucet Handle");
            GameObject FH1Ref= FH1.gameObject;
            FaucetHandleManager FHM1 = FM1Ref.GetComponent<FaucetHandleManager>();
            FHM1.faucetMinigame = FaucetMini1;
            FHM1.AreaUI = CreatedAreaUI;
            FHM1.proceedButton = ButtonMG1;
            FHM1.FaucetHandle = FH1Ref.GetComponent<FaucetHandleController>();
            FaucetHandleController FHC1 = FHM1.FaucetHandle;
            FHC1.proceedButton = ButtonMG1.gameObject;
            ButtonMG1.onClick.AddListener(() =>
            {
                CreatedAreaUI.SetActive(true);
                AreaGroup3.SetActive(true);
                FaucetMinigame1.SetActive(false);
                ButtonMG1.gameObject.SetActive(false);
            });
            if (FHM1.FaucetHandle != null)
            {
                ButtonMG1.onClick.AddListener(FHM1.FaucetHandle.OnProceedPressed);
            }
            else
            {
                Debug.LogError("FaucetHandleController is NULL! Check if"+ FH1Ref.name +  "has the component attached.");
            }
        }
        
        Button ButtonMG2 = Instantiate(ProceedButtonMG2,uimanagement.CanvasTransform,false);
        GameObject FaucetMini2 = Instantiate(FaucetMinigame2);
        FaucetMinigame2 = FaucetMini2;
        if (FaucetMini2 != null)
        {
            AreaGroup4 = arealoader.areaGroups[3];
            Transform FM2 = FindChild(AreaGroup4.transform,"Faucet");
            GameObject FM2Ref= FM2.gameObject;
            Transform FH2 = FindChild(FaucetMini2.transform,"Faucet Handle");
            GameObject FH2Ref= FH2.gameObject;
            FaucetHandleManager FHM2 = FM2Ref.GetComponent<FaucetHandleManager>();
            FHM2.faucetMinigame = FaucetMini2;
            FHM2.AreaUI = CreatedAreaUI;
            FHM2.proceedButton = ButtonMG2;
            FHM2.FaucetHandle = FH2Ref.GetComponent<FaucetHandleController>();
            FaucetHandleController FHC2= FHM2.FaucetHandle;
            FHC2.proceedButton = ButtonMG2.gameObject;
            ButtonMG2.onClick.AddListener(() =>
            {
                CreatedAreaUI.SetActive(true);
                AreaGroup4.SetActive(true);
                FaucetMinigame2.SetActive(false);
                ButtonMG2.gameObject.SetActive(false);
            });
            if (FHM2.FaucetHandle != null)
            {
                ButtonMG2.onClick.AddListener(FHM2.FaucetHandle.OnProceedPressed);
            }
            else
            {
                Debug.LogError("FaucetHandleController is NULL! Check if"+ FH2Ref.name +  "has the component attached.");
            }
        }

        GameObject ShirtSpawnInt = Instantiate(ShirtSpawn, uimanagement.CanvasTransform,false);
        if (ShirtSpawnInt != null)
        {
            AreaGroup5 = arealoader.areaGroups[4];
            Transform ClothesPile = FindChild(AreaGroup5.transform,"Clothes Pile");
            GameObject ClothesPileRef= ClothesPile.gameObject;
            ShirtFoldManager SFM = ClothesPileRef.GetComponent<ShirtFoldManager>();
            SFM.ShirtSpawn = ShirtSpawnInt;
            Transform ProceedButtonShirts = FindChild(ShirtSpawnInt.transform,"Proceed");
            Button ProceedButtonFolding = ProceedButtonShirts.gameObject.GetComponent<Button>();
            ProceedButtonFolding.onClick.AddListener(() =>
            {
                AreaGroup5.SetActive(true);
                ShirtSpawnInt.SetActive(true);
            });
            ProceedButtonFolding.onClick.AddListener(SFM.FoldingComplete);
            Transform ShirtSpawner = FindChild(ShirtSpawnInt.transform,"Shirt Spawner");
            LingapShirtSpawner Lingap = ShirtSpawner.gameObject.GetComponent<LingapShirtSpawner>();
            Lingap.ResetShirtStack();
        }
    }
    private Transform FindChild( Transform parent, string FindName)
    {
        foreach (Transform child in parent)
        {
            if(child.name == FindName)
                return child;
            Transform deeper = FindChild(child, FindName);
            if(deeper != null)
                return deeper;
        }
        return null;
    }
}
