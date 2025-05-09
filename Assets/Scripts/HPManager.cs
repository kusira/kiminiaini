using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HPManager : MonoBehaviour
{
    [Header("HP画像")]
    public Sprite heartOnSprite;
    public Sprite heartOffSprite;

    [Header("HPスロット")]
    public Image[] baseImages;
    public Image[] onImages;

    [Header("プレイヤー")]
    public PlayerController playerController;
    public GameOverManager gameOverManager;

    [Header("効果音")]
    public AudioSource damageSE;
    public AudioSource healSE;
    public AudioSource gameOverSE;

    private int currentHP = 5;
    public int MaxHP => Mathf.Min(baseImages.Length, onImages.Length);

    public float animTime = 0.25f;
    private Vector3[] initialPositions;
    private bool isAddForce = true;
    private bool isGameOver = false;
    private float slowTime = 0f;
    private bool isInvincible = false;

    void Start()
    {
        InitUI();
        SetHP(currentHP);
    }

    void Update()
    {
        var smObj = GameObject.Find("SoundManager");
        if (smObj != null)
        {
            var sm = smObj.GetComponent<SoundManager>();
            if (sm != null)
            {
                if (damageSE != null) damageSE.volume = sm.SEvalue;
                if (healSE != null) healSE.volume = sm.SEvalue;
                if (gameOverSE != null) gameOverSE.volume = sm.SEvalue;
            }
        }

        if (!isGameOver && currentHP <= 0 && playerController != null)
        {
            isGameOver = true;
            gameOverManager.TriggerGameOver(isAddForce);
        }

        if (playerController != null)
        {
            var rb = playerController.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                float xSpeed = Mathf.Abs(rb.velocity.x);
                if (xSpeed <= 0.1f)
                {
                    slowTime += Time.deltaTime;
                    if (slowTime >= 0.3f)
                    {
                        DecreaseHP(100);
                        slowTime = 0f;
                    }
                }
                else
                {
                    slowTime = 0f;
                }
            }
        }
    }

    void InitUI()
    {
        initialPositions = new Vector3[MaxHP];

        for (int i = 0; i < MaxHP; i++)
        {
            if (baseImages[i] != null)
                baseImages[i].sprite = heartOffSprite;

            if (onImages[i] != null)
            {
                onImages[i].sprite = heartOnSprite;
                onImages[i].gameObject.SetActive(true);
                onImages[i].color = Color.white;
                initialPositions[i] = onImages[i].rectTransform.localPosition;
            }
        }
    }

    public void SetHP(int hp, bool playSE = true)
    {
        hp = Mathf.Clamp(hp, 0, MaxHP);

        if (hp < currentHP)
        {
            for (int i = hp; i < currentHP; i++)
            {
                if (onImages[i] != null && onImages[i].gameObject.activeSelf)
                {
                    var img = onImages[i];
                    img.transform.DOLocalMoveY(initialPositions[i].y - 40f, animTime).SetEase(Ease.InQuad);
                    img.DOFade(0f, animTime).OnComplete(() =>
                    {
                        img.gameObject.SetActive(false);
                        img.rectTransform.localPosition = initialPositions[i];
                        img.color = Color.white;
                    });
                }
            }
        }
        else if (hp > currentHP)
        {
            for (int i = currentHP; i < hp; i++)
            {
                var img = onImages[i];
                if (img != null)
                {
                    img.sprite = heartOnSprite;
                    img.rectTransform.localPosition = initialPositions[i] + new Vector3(0, 50f, 0);
                    img.color = new Color(1, 1, 1, 0);
                    img.gameObject.SetActive(true);

                    img.transform.DOLocalMoveY(initialPositions[i].y, animTime).SetEase(Ease.OutQuad);
                    img.DOFade(1f, animTime);
                }
            }
        }

        currentHP = hp;
    }

    public void DecreaseHP(int amount = 1)
    {
        if (isGameOver || isInvincible) return; // ★ 無敵中は無視
        int nextHP = Mathf.Max(currentHP - amount, 0);

        if (nextHP > 0)
        {
            damageSE?.Play();
        }
        else
        {
            gameOverSE?.Play();
        }

        SetHP(nextHP, playSE: false);
    }

    public void IncreaseHP(int amount = 1)
    {
        if (isGameOver) return; // ★ すでにゲームオーバーなら何もしない
        healSE?.Play(); // ★ 回復SEをここで再生
        SetHP(currentHP + amount, playSE: false);
    }

    public int GetCurrentHP() => currentHP;

    public void SetAddForceEnabled(bool enabled)
    {
        isAddForce = enabled;
    }

    public void SetInvincible(bool value)
    {
        isInvincible = value;
    }

}
