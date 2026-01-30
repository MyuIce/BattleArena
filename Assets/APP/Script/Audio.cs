using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource buttonSource;

    [SerializeField] private AudioClip titleBGM;
    [SerializeField] private AudioClip stageChoiceBGM;
    [SerializeField] private AudioClip stage1BGM;
    [SerializeField] private AudioClip stage2BGM;

    [SerializeField] private AudioClip buttonSE;
    [SerializeField] private AudioClip bagButtonSE;
    [SerializeField] private AudioClip backTitleButtonSE;
    [SerializeField] private AudioClip ItemSlotSE;
    [SerializeField] private AudioClip ItemUseSE;
    [SerializeField] private AudioClip MenuSE;
    [SerializeField] private AudioClip DeathCanvasSE;

    [SerializeField] private AudioClip AttackSE;
    [SerializeField] private AudioClip GuardSE;
    [SerializeField] private AudioClip EquipmentSetSE;
    [SerializeField] private AudioClip SkeletonDownSE;
    [SerializeField] private AudioClip ZombieDownSE;


    private AudioClip currentBGM;

    void Awake()
    {
        // 破棄+生成
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch (scene.name)
        {
            case "Title":
                PlayBGM(titleBGM);
                break;
            case "StageChoice":
                PlayBGM(stageChoiceBGM);
                break;
            case "Stage1":
                PlayBGM(stage1BGM);
                break;
            case "Stage2":
                PlayBGM(stage2BGM);
                break;
        }
    }

    private void PlayBGM(AudioClip bgm)
    {
        if (bgm == null) return;
        if (currentBGM == bgm) return;

        currentBGM = bgm;
        bgmSource.clip = bgm;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    //UIアタッチ用関数(Instance参照)
    public void OnButtonSE_Global()
    {
        Instance.OnClickbutton();
    }
    public void OnBackTitleSE_Global()
    {
        Instance.OnClickBacktitlebutton();
    }
    public void OnBagSE_Global()
    {
        Instance.OnClickBagbutton();
    }
    public void OnItemSlotSE_Global()
    {
        Instance.OnClickItemSlotButton();
    }
    public void OnItemUseSE_Global()
    {
        Instance.OnClickItemUseButton();
    }
    public void OnMenuSE_Global()
    {
        Instance.OnClickMenuButton();
    }
    public void OnDeathCanvasSE_Global()
    {
        //CharaDamage.csで使用
        Instance.OnDeathCanvas();
    }
    public void OnAttackSE_Global()
    {
        //PlayerAttack.OnTriggerEnter()で使用
        Instance.OnAttack();
    }
    public void OnGuardSE_Global()
    {
        //CharaDamage.Damage()で使用
        Instance.OnGuard();
    }
    public void OnEquipmentSetSE_Global()
    {
        //Soubikanri.UpdateUI()で使用
        Instance.OnEquipmentSet();
    }
    public void OnSkeletonDownSE_Global()
    {
        Instance.OnSkeletonDown();
    }
    public void OnZombieDownSE_Global()
    {
        Instance.OnZombieDown();
    }
    


    //音の設定
    private void OnClickbutton()
    {
        buttonSource.PlayOneShot(buttonSE);
    }
    private void OnClickBacktitlebutton()
    {
        buttonSource.PlayOneShot(backTitleButtonSE);
    }
    private void OnClickBagbutton()
    {
        buttonSource.PlayOneShot(bagButtonSE);
    }
    private void OnClickItemSlotButton()
    {
        buttonSource.PlayOneShot(ItemSlotSE);
    }
    private void OnClickItemUseButton()
    {
        buttonSource.PlayOneShot(ItemUseSE);
    }
    private void OnClickMenuButton()
    {
        buttonSource.PlayOneShot(MenuSE);
    }
    private void OnDeathCanvas()
    {
        buttonSource.PlayOneShot(DeathCanvasSE);
    }
    private void OnAttack()
    {
        buttonSource.PlayOneShot(AttackSE);
    }
    private void OnGuard()
    {
        buttonSource.PlayOneShot(GuardSE);
    }
    private void OnEquipmentSet()
    {
        buttonSource.PlayOneShot(EquipmentSetSE);
    }
    private void OnSkeletonDown()
    {
        buttonSource.PlayOneShot(SkeletonDownSE);
    }
    private void OnZombieDown()
    {
        buttonSource.PlayOneShot(ZombieDownSE);
    }
}