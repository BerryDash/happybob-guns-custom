using System.Linq;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GamePlayer : MonoBehaviour
{
    public static GamePlayer instance;
    private readonly float spawnRate = 1f;
    private float nextSpawnTime;
    internal BigInteger score;
    private BigInteger attempts;
    private BigInteger highscore;
    private BigInteger totalNormalBerries;
    private BigInteger totalPoisonBerries;
    private BigInteger totalSlowBerries;
    private BigInteger totalUltraBerries;
    private BigInteger totalSpeedyBerries;
    private BigInteger totalRandomBerries;
    private BigInteger totalAntiBerries;
    private BigInteger totalAttempts;
    private float boostLeft;
    private float slownessLeft;
    private float speedyLeft;
    private float antiLeft;
    private float screenWidth;
    internal bool isGrounded;
    internal bool isGrounded2;
    public TMP_Text scoreText;
    public TMP_Text highScoreText;
    public TMP_Text boostText;
    public GameObject bird;
    public GameObject bird2;
    public GameObject pausePanel;
    public GameObject statsPanel;
    public Rigidbody2D rb;
    public Rigidbody2D rb2;
    public AudioSource backgroundMusic;
    public GameObject berryParent;

    public GameObject mobileButtons;
    public Button pauseButton;
    public Button restartButton;
    public Button jumpButton;
    public Button rightButton;
    public Button leftButton;
    public Button shootButton;

    void Start()
    {
        UnityEngine.InputSystem.EnhancedTouch.EnhancedTouchSupport.Enable();
        instance = this;
        highscore = BazookaManager.Instance.GetGameStoreHighScore();
        totalNormalBerries = BazookaManager.Instance.GetGameStoreTotalNormalBerries();
        totalPoisonBerries = BazookaManager.Instance.GetGameStoreTotalPoisonBerries();
        totalSlowBerries = BazookaManager.Instance.GetGameStoreTotalSlowBerries();
        totalUltraBerries = BazookaManager.Instance.GetGameStoreTotalUltraBerries();
        totalSpeedyBerries = BazookaManager.Instance.GetGameStoreTotalSpeedyBerries();
        totalRandomBerries = BazookaManager.Instance.GetGameStoreTotalRandomBerries();
        totalAntiBerries = BazookaManager.Instance.GetGameStoreTotalAntiBerries();
        totalAttempts = BazookaManager.Instance.GetGameStoreTotalAttepts();

        backgroundMusic.volume = BazookaManager.Instance.GetSettingMusicVolume();
        screenWidth = Camera.main.orthographicSize * 2f * Camera.main.aspect;
        if (Application.isMobilePlatform) mobileButtons.SetActive(true);
        UpdateStats(0, 1);

        if (Application.isMobilePlatform) shootButton.onClick.AddListener(() => GunHandler.Instance.ShootGun());
    }

    void MoveBird()
    {
        float screenWidth = Camera.main.orthographicSize * 2f * Camera.main.aspect;
        float baseSpeed = 0.18f * (screenWidth / 20.19257f);
        bool doMoveRight = false;
        bool doMoveLeft = false;
        bool doMoveRight2 = false;
        bool doMoveLeft2 = false;
        bool doJump = false;
        bool doJump2 = false;
        bool doRestart = false;
        bool doBack = false;
        float movespeed = baseSpeed;
        if (boostLeft > 0f || speedyLeft > 0f)
        {
            movespeed = baseSpeed * 1.39f;
        }
        else if (slownessLeft > 0f)
        {
            movespeed = baseSpeed * 0.56f;
        }
        CheckIfGrounded();
        bool controllerLeft = Gamepad.current != null && (Gamepad.current.leftStick.left.isPressed || Gamepad.current.dpad.left.isPressed || Gamepad.current.rightStick.left.isPressed);
        bool controllerRight = Gamepad.current != null && (Gamepad.current.leftStick.right.isPressed || Gamepad.current.dpad.right.isPressed || Gamepad.current.rightStick.right.isPressed);
        bool controllerJump = Gamepad.current != null && (Gamepad.current.leftStick.up.isPressed || Gamepad.current.leftStick.down.isPressed || Gamepad.current.dpad.up.isPressed || Gamepad.current.dpad.down.isPressed || Gamepad.current.rightStick.up.isPressed || Gamepad.current.rightStick.down.isPressed);
        if (!Application.isMobilePlatform)
        {
            if (controllerLeft || Keyboard.current.aKey.isPressed)
            {
                doMoveLeft = true;
            }
            if (controllerRight || Keyboard.current.dKey.isPressed)
            {
                doMoveRight = true;
            }
            if (Keyboard.current.leftArrowKey.isPressed)
            {
                doMoveLeft2 = true;
            }
            if (Keyboard.current.rightArrowKey.isPressed)
            {
                doMoveRight2 = true;
            }
            if (Keyboard.current.wKey.isPressed)
            {
                doJump = true;
            }
            if (Keyboard.current.upArrowKey.isPressed)
            {
                doJump2 = true;
            }
            if (Keyboard.current.rKey.isPressed)
            {
                doRestart = true;
            }
        }
        else
        {
            var touches = UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches;
            for (int i = 0; i < touches.Count; i++)
            {
                var pos = touches[i].screenPosition;
                UnityEngine.Vector3 clickPosition = Camera.main.ScreenToWorldPoint(new UnityEngine.Vector3(pos.x, pos.y, 0f));
                clickPosition.z = 0f;
                if (leftButton.GetComponent<HoldableButton>().isPressed) doMoveLeft = true;
                if (rightButton.GetComponent<HoldableButton>().isPressed) doMoveRight = true;
                if (jumpButton.GetComponent<HoldableButton>().isPressed) doJump = true;
                if (restartButton.GetComponent<HoldableButton>().isPressed) doRestart = true;
                if (pauseButton.GetComponent<HoldableButton>().isPressed) doBack = true;
            }
        }
        if (doMoveLeft && !doMoveRight)
        {
            bird.transform.position += new UnityEngine.Vector3(-movespeed, 0f, 0f);
            ClampPosition(bird);
        }
        if (doMoveRight && !doMoveLeft)
        {
            bird.transform.position += new UnityEngine.Vector3(movespeed, 0f, 0f);
            ClampPosition(bird);
        }
        if (doMoveLeft2 && !doMoveRight2)
        {
            bird2.transform.position += new UnityEngine.Vector3(-movespeed, 0f, 0f);
            ClampPosition(bird2);
        }
        if (doMoveRight2 && !doMoveLeft2)
        {
            bird2.transform.position += new UnityEngine.Vector3(movespeed, 0f, 0f);
            ClampPosition(bird2);
        }
        if (doJump && isGrounded)
        {
            AudioSource.PlayClipAtPoint(Resources.Load<AudioClip>("Sounds/Jump"), Camera.main.transform.position, 0.75f * BazookaManager.Instance.GetSettingSFXVolume());
            if (boostLeft > 0f || speedyLeft > 0f)
            {
                rb.linearVelocity = UnityEngine.Vector2.up * 12f;
            }
            else if (slownessLeft > 0f)
            {
                rb.linearVelocity = UnityEngine.Vector2.up * 6f;
            }
            else
            {
                rb.linearVelocity = UnityEngine.Vector2.up * 9f;
            }
        }
        if (doJump2 && isGrounded2)
        {
            AudioSource.PlayClipAtPoint(Resources.Load<AudioClip>("Sounds/Jump"), Camera.main.transform.position, 0.75f * BazookaManager.Instance.GetSettingSFXVolume());
            if (boostLeft > 0f || speedyLeft > 0f)
            {
                rb2.linearVelocity = UnityEngine.Vector2.up * 12f;
            }
            else if (slownessLeft > 0f)
            {
                rb2.linearVelocity = UnityEngine.Vector2.up * 6f;
            }
            else
            {
                rb2.linearVelocity = UnityEngine.Vector2.up * 9f;
            }
        }
        if (doBack)
        {
            TogglePause();
        }
        if (doRestart)
        {
            if (score != 0) Respawn();
        }
        if (antiLeft > 0f)
        {
            string[] berryTags = { "NormalBerry", "PoisonBerry", "SlowBerry", "UltraBerry", "SpeedyBerry", "RandomBerry", "AntiBerry" };
            foreach (string tag in berryTags)
            {
                foreach (var berry in GameObject.FindGameObjectsWithTag(tag))
                {
                    UnityEngine.Vector3 dir = berry.transform.position - bird.transform.position;
                    if (dir.magnitude < 3f)
                    {
                        berry.GetComponent<Rigidbody2D>().linearVelocity = dir.normalized * 5f;
                        ClampPosition(berry, false);
                    }
                }
            }
        }
    }

    void ClampPosition(GameObject obj, bool modifyY = true)
    {
        var cam = Camera.main;
        var pos = obj.transform.position;
        var bounds = obj.GetComponent<Renderer>().bounds.extents;

        float zDist = Mathf.Abs(cam.transform.position.z - pos.z);

        UnityEngine.Vector3 min = cam.ViewportToWorldPoint(new UnityEngine.Vector3(0, 0, zDist));
        UnityEngine.Vector3 max = cam.ViewportToWorldPoint(new UnityEngine.Vector3(1, 1, zDist));

        pos.x = Mathf.Clamp(pos.x, min.x + bounds.x, max.x - bounds.x);
        if (modifyY) pos.y = Mathf.Clamp(pos.y, min.y + bounds.y, max.y - bounds.y);

        obj.transform.position = pos;
    }

    void FixedUpdate()
    {
        SpawnBerries();
        if (!pausePanel.activeSelf)
        {
            MoveBird();
            if (boostLeft > 0f)
            {
                boostLeft -= Time.deltaTime;
                boostText.text = "Boost expires in " + string.Format("{0:0.0}", boostLeft) + "s";
            }
            else if (slownessLeft > 0f)
            {
                slownessLeft -= Time.deltaTime;
                boostText.text = "Slowness expires in " + string.Format("{0:0.0}", slownessLeft) + "s";
            }
            else if (speedyLeft > 0f)
            {
                speedyLeft -= Time.deltaTime;
                boostText.text = "Speed expires in " + string.Format("{0:0.0}", speedyLeft) + "s";
            }
            else if (antiLeft > 0f)
            {
                antiLeft -= Time.deltaTime;
                boostText.text = "Berry repellent expires in " + string.Format("{0:0.0}", antiLeft) + "s";
            }
            else
            {
                boostText.text = "";
            }
        }
    }

    void SpawnBerries()
    {
        if (Time.time < nextSpawnTime)
        {
            return;
        }
        if (speedyLeft > 0)
        {
            nextSpawnTime = Time.time + 1f / (spawnRate * 1.875f);
        }
        else
        {
            nextSpawnTime = Time.time + 1f / spawnRate;
        }
        float spawnProbability = Random.value;
        if (!pausePanel.activeSelf)
        {
            GameObject newBerry = new("Berry");
            newBerry.transform.SetParent(berryParent.transform);
            SpriteRenderer spriteRenderer = newBerry.AddComponent<SpriteRenderer>();
            if (spawnProbability <= 0.475f)
            {
                spriteRenderer.sprite = Resources.Load<Sprite>("Berries/Berry");
                newBerry.tag = "NormalBerry";
            }
            else if (spawnProbability <= 0.60f)
            {
                spriteRenderer.sprite = Resources.Load<Sprite>("Berries/PoisonBerry");
                newBerry.tag = "PoisonBerry";
            }
            else if (spawnProbability <= 0.70f)
            {
                spriteRenderer.sprite = Resources.Load<Sprite>("Berries/SlowBerry");
                newBerry.tag = "SlowBerry";
            }
            else if (spawnProbability <= 0.80f)
            {
                spriteRenderer.sprite = Resources.Load<Sprite>("Berries/UltraBerry");
                newBerry.tag = "UltraBerry";
            }
            else if (spawnProbability <= 0.90f)
            {
                spriteRenderer.sprite = Resources.Load<Sprite>("Berries/SpeedyBerry");
                newBerry.tag = "SpeedyBerry";
            }
            else if (spawnProbability <= 0.95f)
            {
                spriteRenderer.sprite = Resources.Load<Sprite>("Berries/BerryNoColor");
                newBerry.tag = "RandomBerry";
                RainbowSpriteRender randomBerryRainbowImage = newBerry.AddComponent<RainbowSpriteRender>();
                randomBerryRainbowImage.frequency = 5f;
            }
            else
            {
                spriteRenderer.sprite = Resources.Load<Sprite>("Berries/AntiBerry");
                newBerry.tag = "AntiBerry";
            }
            spriteRenderer.sortingOrder = -5;

            float screenWidth = Camera.main.orthographicSize * 2 * Camera.main.aspect;
            float spawnPositionX = Random.Range(-screenWidth / 2.17f, screenWidth / 2.17f);
            newBerry.transform.position = new UnityEngine.Vector3(spawnPositionX, Camera.main.orthographicSize + 1f, 0f);

            Rigidbody2D rb = newBerry.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.linearVelocity = new UnityEngine.Vector2(0f, -4f);
        }
    }

    void Update()
    {
        foreach (AudioSource audio in FindObjectsByType<AudioSource>(FindObjectsSortMode.None))
        {
            audio.pitch = speedyLeft > 0f ? 1.3f : 1f;
        }
        if (screenWidth != Camera.main.orthographicSize * 2f * Camera.main.aspect)
        {
            screenWidth = Camera.main.orthographicSize * 2f * Camera.main.aspect;
            ClampPosition(bird);
            ClampPosition(bird2);
            GameObject[] allberries = GameObject.FindGameObjectsWithTag("NormalBerry")
                .Concat(GameObject.FindGameObjectsWithTag("PoisonBerry"))
                .Concat(GameObject.FindGameObjectsWithTag("SlowBerry"))
                .Concat(GameObject.FindGameObjectsWithTag("UltraBerry"))
                .Concat(GameObject.FindGameObjectsWithTag("SpeedyBerry"))
                .Concat(GameObject.FindGameObjectsWithTag("RandomBerry"))
                .Concat(GameObject.FindGameObjectsWithTag("AntiBerry"))
                .ToArray();
            foreach (GameObject berry in allberries)
            {
                ClampPosition(berry, false);
            }
        }
        GameObject[] normalBerries = GameObject.FindGameObjectsWithTag("NormalBerry");
        GameObject[] poisonBerries = GameObject.FindGameObjectsWithTag("PoisonBerry");
        GameObject[] slowBerries = GameObject.FindGameObjectsWithTag("SlowBerry");
        GameObject[] ultraBerries = GameObject.FindGameObjectsWithTag("UltraBerry");
        GameObject[] speedyBerries = GameObject.FindGameObjectsWithTag("SpeedyBerry");
        GameObject[] randomBerries = GameObject.FindGameObjectsWithTag("RandomBerry");
        GameObject[] antiBerries = GameObject.FindGameObjectsWithTag("AntiBerry");

        GameObject[] bullets = GameObject.FindGameObjectsWithTag("Bullet");

        if (!pausePanel.activeSelf)
        {
            CheckIfGrounded();
            foreach (GameObject normalBerry in normalBerries)
            {
                if (normalBerry.transform.position.y < 0f - Camera.main.orthographicSize - 1f)
                {
                    Destroy(normalBerry);
                }
                else if (UnityEngine.Vector3.Distance(bird.transform.position, normalBerry.transform.position) < 1.5f || UnityEngine.Vector3.Distance(bird2.transform.position, normalBerry.transform.position) < 1.5f)
                {
                    totalNormalBerries++;
                    DoNormalBerry(normalBerry);
                }
                else
                {
                    foreach (GameObject bullet in bullets)
                    {
                        if (UnityEngine.Vector3.Distance(bullet.transform.position, normalBerry.transform.position) < 0.5f)
                        {
                            totalNormalBerries++;
                            DoNormalBerry(normalBerry);
                            Destroy(bullet);
                            break;
                        }
                    }
                }
                if (speedyLeft > 0)
                {
                    normalBerry.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, -7.5f);
                }
                else
                {
                    normalBerry.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, -4f);
                }
            }
            foreach (GameObject poisonBerry in poisonBerries)
            {
                if (poisonBerry.transform.position.y < 0f - Camera.main.orthographicSize - 1f)
                {
                    Destroy(poisonBerry);
                }
                else if (UnityEngine.Vector3.Distance(bird.transform.position, poisonBerry.transform.position) < 1.5f || UnityEngine.Vector3.Distance(bird2.transform.position, poisonBerry.transform.position) < 1.5f)
                {
                    totalPoisonBerries++;
                    DoPoisonBerry();
                }
                else
                {
                    foreach (GameObject bullet in bullets)
                    {
                        if (UnityEngine.Vector3.Distance(bullet.transform.position, poisonBerry.transform.position) < 0.5f)
                        {
                            totalPoisonBerries++;
                            Destroy(poisonBerry);
                            Destroy(bullet);
                            break;
                        }
                    }
                }
                if (speedyLeft > 0)
                {
                    poisonBerry.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, -7.5f);
                }
                else
                {
                    poisonBerry.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, -4f);
                }
            }
            foreach (GameObject slowBerry in slowBerries)
            {
                if (slowBerry.transform.position.y < 0f - Camera.main.orthographicSize - 1f)
                {
                    Destroy(slowBerry);
                }
                else if (UnityEngine.Vector3.Distance(bird.transform.position, slowBerry.transform.position) < 1.5f || UnityEngine.Vector3.Distance(bird2.transform.position, slowBerry.transform.position) < 1.5f)
                {
                    totalSlowBerries++;
                    DoSlowBerry(slowBerry);
                }
                else
                {
                    foreach (GameObject bullet in bullets)
                    {
                        if (UnityEngine.Vector3.Distance(bullet.transform.position, slowBerry.transform.position) < 0.5f)
                        {
                            totalSlowBerries++;
                            Destroy(slowBerry);
                            Destroy(bullet);
                            break;
                        }
                    }
                }
                if (speedyLeft > 0)
                {
                    slowBerry.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, -7.5f);
                }
                else
                {
                    slowBerry.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, -4f);
                }
            }
            foreach (GameObject ultraBerry in ultraBerries)
            {
                if (ultraBerry.transform.position.y < 0f - Camera.main.orthographicSize - 1f)
                {
                    Destroy(ultraBerry);
                }
                else if (UnityEngine.Vector3.Distance(bird.transform.position, ultraBerry.transform.position) < 1.5f || UnityEngine.Vector3.Distance(bird2.transform.position, ultraBerry.transform.position) < 1.5f)
                {
                    totalUltraBerries++;
                    DoUltraBerry(ultraBerry);
                }
                else
                {
                    foreach (GameObject bullet in bullets)
                    {
                        if (UnityEngine.Vector3.Distance(bullet.transform.position, ultraBerry.transform.position) < 0.5f)
                        {
                            totalUltraBerries++;
                            DoUltraBerry(ultraBerry);
                            Destroy(bullet);
                            break;
                        }
                    }
                }
                if (speedyLeft > 0)
                {
                    ultraBerry.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, -7.5f);
                }
                else
                {
                    ultraBerry.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, -4f);
                }
            }
            foreach (GameObject speedyBerry in speedyBerries)
            {
                if (speedyBerry.transform.position.y < 0f - Camera.main.orthographicSize - 1f)
                {
                    Destroy(speedyBerry);
                }
                else if (UnityEngine.Vector3.Distance(bird.transform.position, speedyBerry.transform.position) < 1.5f || UnityEngine.Vector3.Distance(bird2.transform.position, speedyBerry.transform.position) < 1.5f)
                {
                    totalSpeedyBerries++;
                    DoSpeedyBerry(speedyBerry);
                }
                else
                {
                    foreach (GameObject bullet in bullets)
                    {
                        if (UnityEngine.Vector3.Distance(bullet.transform.position, speedyBerry.transform.position) < 0.5f)
                        {
                            totalSpeedyBerries++;
                            DoSpeedyBerry(speedyBerry);
                            Destroy(bullet);
                            break;
                        }
                    }
                }
                if (speedyLeft > 0)
                {
                    speedyBerry.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, -7.5f);
                }
                else
                {
                    speedyBerry.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, -4f);
                }
            }
            foreach (GameObject randomBerry in randomBerries)
            {
                if (randomBerry.transform.position.y < 0f - Camera.main.orthographicSize - 1f)
                {
                    Destroy(randomBerry);
                }
                else if (UnityEngine.Vector3.Distance(bird.transform.position, randomBerry.transform.position) < 1.5f || UnityEngine.Vector3.Distance(bird2.transform.position, randomBerry.transform.position) < 1.5f)
                {
                    totalRandomBerries++;
                    System.Action[] funcs = {
                        () => DoNormalBerry(randomBerry),
                        () => DoSlowBerry(randomBerry),
                        () => DoUltraBerry(randomBerry),
                        () => DoSpeedyBerry(randomBerry),
                        () => DoAntiBerry(randomBerry)
                    };
                    funcs[Random.Range(0, funcs.Length)]();
                }
                else
                {
                    foreach (GameObject bullet in bullets)
                    {
                        if (UnityEngine.Vector3.Distance(bullet.transform.position, randomBerry.transform.position) < 0.5f)
                        {
                            totalRandomBerries++;
                            System.Action[] funcs = {
                                () => DoNormalBerry(randomBerry),
                                () => DoUltraBerry(randomBerry),
                                () => DoSpeedyBerry(randomBerry),
                                () => DoAntiBerry(randomBerry)
                            };
                            funcs[Random.Range(0, funcs.Length)]();
                            Destroy(bullet);
                            break;
                        }
                    }
                }
                if (speedyLeft > 0)
                {
                    randomBerry.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, -7.5f);
                }
                else
                {
                    randomBerry.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, -4f);
                }
            }
            foreach (GameObject antiBerry in antiBerries)
            {
                if (antiBerry.transform.position.y < 0f - Camera.main.orthographicSize - 1f)
                {
                    Destroy(antiBerry);
                }
                else if (UnityEngine.Vector3.Distance(bird.transform.position, antiBerry.transform.position) < 1.5f || UnityEngine.Vector3.Distance(bird2.transform.position, antiBerry.transform.position) < 1.5f)
                {
                    totalAntiBerries++;
                    DoAntiBerry(antiBerry);
                }
                else
                {
                    foreach (GameObject bullet in bullets)
                    {
                        if (UnityEngine.Vector3.Distance(bullet.transform.position, antiBerry.transform.position) < 0.5f)
                        {
                            totalAntiBerries++;
                            DoAntiBerry(antiBerry);
                            Destroy(bullet);
                            break;
                        }
                    }
                }
                if (speedyLeft > 0)
                {
                    antiBerry.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, -7.5f);
                }
                else
                {
                    antiBerry.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, -4f);
                }
            }
        }
        else
        {
            rb.gravityScale = 0f;
            rb.linearVelocity = UnityEngine.Vector2.zero;
            GameObject[] allberries = normalBerries
                .Concat(poisonBerries)
                .Concat(slowBerries)
                .Concat(ultraBerries)
                .Concat(speedyBerries)
                .Concat(randomBerries)
                .Concat(antiBerries)
                .ToArray();
            foreach (GameObject berry in allberries)
            {
                berry.GetComponent<Rigidbody2D>().linearVelocity = UnityEngine.Vector2.zero;
            }
        }
        if ((Application.platform == RuntimePlatform.Android && Keyboard.current.escapeKey.wasPressedThisFrame) || !Application.isMobilePlatform && (Keyboard.current.escapeKey.wasPressedThisFrame || (Gamepad.current != null && (Gamepad.current.startButton.wasPressedThisFrame || Gamepad.current.buttonEast.wasPressedThisFrame)))) TogglePause();
    }

    void Respawn()
    {
        bird.transform.position = new UnityEngine.Vector3(0f, -4.3f, 0f);
        bird2.transform.position = new UnityEngine.Vector3(0f, -4.3f, 0f);
        rb.gravityScale = 0f;
        rb.linearVelocity = UnityEngine.Vector2.zero;
        rb2.gravityScale = 0f;
        rb2.linearVelocity = UnityEngine.Vector2.zero;
        score = 0;
        boostLeft = 0f;
        slownessLeft = 0f;
        speedyLeft = 0f;
        antiLeft = 0f;
        UpdateStats(0, 1);

        GameObject[] allberries = GameObject.FindGameObjectsWithTag("NormalBerry")
            .Concat(GameObject.FindGameObjectsWithTag("PoisonBerry"))
            .Concat(GameObject.FindGameObjectsWithTag("SlowBerry"))
            .Concat(GameObject.FindGameObjectsWithTag("UltraBerry"))
            .Concat(GameObject.FindGameObjectsWithTag("SpeedyBerry"))
            .Concat(GameObject.FindGameObjectsWithTag("RandomBerry"))
            .Concat(GameObject.FindGameObjectsWithTag("AntiBerry"))
            .ToArray();
        foreach (GameObject berry in allberries)
        {
            Destroy(berry);
        }
    }

    void UpdateStats(BigInteger scoreAddAmount, BigInteger attemptAddAmount)
    {
        var prefix = "";
        var suffix = "";
        score += scoreAddAmount;
        totalAttempts += attemptAddAmount;
        attempts += attemptAddAmount;
        if (score >= highscore)
        {
            highscore = score;
            prefix = "<color=#7FFFD4>";
            suffix = "</color>";
        }
        BazookaManager.Instance.SetGameStoreHighScore(highscore);
        BazookaManager.Instance.SetGameStoreTotalNormalBerries(totalNormalBerries);
        BazookaManager.Instance.SetGameStoreTotalPoisonBerries(totalPoisonBerries);
        BazookaManager.Instance.SetGameStoreTotalSlowBerries(totalSlowBerries);
        BazookaManager.Instance.SetGameStoreTotalUltraBerries(totalUltraBerries);
        BazookaManager.Instance.SetGameStoreTotalSpeedyBerries(totalSpeedyBerries);
        BazookaManager.Instance.SetGameStoreTotalRandomBerries(totalRandomBerries);
        BazookaManager.Instance.SetGameStoreTotalAntiBerries(totalAntiBerries);
        BazookaManager.Instance.SetGameStoreTotalAttepts(totalAttempts);
        scoreText.text = $"Score: {Tools.FormatWithCommas(score)} \\u2022 Attempts: {Tools.FormatWithCommas(attempts)}";
        highScoreText.text = prefix + $"High Score: {Tools.FormatWithCommas(highscore) + suffix} \\u2022 Total Attempts: {Tools.FormatWithCommas(totalAttempts)}";
        if (Application.isMobilePlatform) restartButton.interactable = score != 0;
    }

    void CheckIfGrounded()
    {
        isGrounded = bird.transform.position.y <= -4.1299996f;

        rb.gravityScale = isGrounded ? 0f : 1.5f;

        if (bird.transform.position.y < -4.1359f)
        {
            bird.transform.position = new UnityEngine.Vector2(bird.transform.position.x, -4.1359f);
            rb.linearVelocity = new UnityEngine.Vector2(rb.linearVelocity.x, 0f);
        }
        isGrounded2 = bird2.transform.position.y <= -4.1299996f;
        rb2.gravityScale = isGrounded2 ? 0f : 1.5f;

        if (bird2.transform.position.y < -4.1359f)
        {
            bird2.transform.position = new UnityEngine.Vector2(bird2.transform.position.x, -4.1359f);
            rb2.linearVelocity = new UnityEngine.Vector2(rb2.linearVelocity.x, 0f);
        }
        if (Application.isMobilePlatform) jumpButton.transform.GetChild(0).GetComponent<TMP_Text>().color = isGrounded ? Color.white : Color.red;
    }

    internal void TogglePause()
    {
        if (statsPanel.activeSelf) return;
        if (pausePanel.activeSelf)
        {
            DisablePause();
        }
        else
        {
            EnablePause();
        }
    }

    internal void EnablePause()
    {
        backgroundMusic.GetComponent<GameMusicHandler>().PauseMusic();
        pausePanel.SetActive(true);
    }

    internal void DisablePause()
    {
        backgroundMusic.GetComponent<GameMusicHandler>().ResumeMusic();
        pausePanel.SetActive(false);
    }

    void OnApplicationPause(bool pause)
    {
        if (pause) EnablePause();
    }

    void DoNormalBerry(GameObject berry)
    {
        AudioSource.PlayClipAtPoint(Resources.Load<AudioClip>("Sounds/Eat"), Camera.main.transform.position, 1.2f * BazookaManager.Instance.GetSettingSFXVolume());
        Destroy(berry);
        UpdateStats(1, 0);
    }

    void DoPoisonBerry()
    {
        AudioSource.PlayClipAtPoint(Resources.Load<AudioClip>("Sounds/Death"), Camera.main.transform.position, 1.2f * BazookaManager.Instance.GetSettingSFXVolume());
        Respawn();
        UpdateStats(0, 0);
    }

    void DoSlowBerry(GameObject berry)
    {
        AudioSource.PlayClipAtPoint(Resources.Load<AudioClip>("Sounds/Downgrade"), Camera.main.transform.position, 0.35f * BazookaManager.Instance.GetSettingSFXVolume());
        Destroy(berry);
        boostLeft = 0f;
        slownessLeft = 10f;
        speedyLeft = 0f;
        antiLeft = 0f;
        if (score > 0)
        {
            UpdateStats(-1, 0);
        }
    }

    void DoUltraBerry(GameObject berry)
    {
        AudioSource.PlayClipAtPoint(Resources.Load<AudioClip>("Sounds/Powerup"), Camera.main.transform.position, 0.35f * BazookaManager.Instance.GetSettingSFXVolume());
        Destroy(berry);
        speedyLeft = 0f;
        antiLeft = 0f;
        if (slownessLeft > 0f)
        {
            slownessLeft = 0f;
            UpdateStats(1, 0);
        }
        else
        {
            boostLeft += 10f;
            UpdateStats(5, 0);
        }
    }

    void DoSpeedyBerry(GameObject berry)
    {
        AudioSource.PlayClipAtPoint(Resources.Load<AudioClip>("Sounds/SpeedyPowerup"), Camera.main.transform.position, 0.35f * BazookaManager.Instance.GetSettingSFXVolume());
        Destroy(berry);
        boostLeft = 0f;
        slownessLeft = 0f;
        speedyLeft = 10f;
        antiLeft = 0f;
        UpdateStats(10, 0);
    }

    void DoAntiBerry(GameObject berry)
    {
        AudioSource.PlayClipAtPoint(Resources.Load<AudioClip>("Sounds/Downgrade"), Camera.main.transform.position, 0.35f * BazookaManager.Instance.GetSettingSFXVolume());
        Destroy(berry);
        boostLeft = 0f;
        slownessLeft = 0f;
        speedyLeft = 0f;
        antiLeft = 10f;
        UpdateStats(0, 0);
    }
}