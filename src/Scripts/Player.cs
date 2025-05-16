using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Player : MonoBehaviour
{

    [SerializeField] float speed;
    [SerializeField] int health;
    [HideInInspector] public int maxHealth;

    Rigidbody2D rb;
    Vector2 moveVelocity;

    [SerializeField] GameObject bullet;
    [SerializeField] Transform shootPos;

    [SerializeField] Transform[] shootSuperPos;

    [SerializeField] float timeBtwShoot = 2;
    float shootTimer;
    
    [SerializeField] float timeBtwSuperShoot = 2;
    float shootSuperTimer;

    Animator anim;
    SpriteRenderer spR;

    [SerializeField] TextMeshProUGUI text;

    public static Player instance;

    [SerializeField] GameObject hitEffect;

    [SerializeField] Sprite[] spritesMuzzleFlash;
    [SerializeField] SpriteRenderer muzzleFlashSpR;

    [SerializeField] float dashForce, timeBtwDash, dashTime;
    float dashTimer;
    bool isDashing = false;

    [SerializeField] Slider healthSlider;
    [SerializeField] Slider dashSlider;

    [SerializeField] ParticleSystem footParticle;
    [SerializeField] GameObject deathPanel;

    bool canBeDamaged = true;

    [SerializeField] AudioClip shootClip, superShootClip, heartClip, deathClip, dashSound;
    [SerializeField] AudioClip[] footClips;
    AudioSource audS;

    Vector2 moveInput;
    private void Awake()
    {
        instance = this;

        Shop.instance.buySeconPosition += UpdateTimeBtwShoot;
    }

    // Запускается в старте
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spR = GetComponent<SpriteRenderer>();
        audS = GetComponent<AudioSource>();

        shootTimer = timeBtwShoot;
        dashTimer = timeBtwDash;
        maxHealth = health;

        UpdateHealthUI();

    }

    // Update is called once per frame
    void Update()
    {
        shootTimer += Time.deltaTime;

        if (Input.GetMouseButtonDown(0) && shootTimer >= timeBtwShoot)
        {
            Shoot();
            shootTimer = 0;
        }

        shootSuperTimer += Time.deltaTime;

        if (Input.GetMouseButtonDown(1) && shootSuperTimer >= timeBtwSuperShoot && PlayerPrefs.GetInt("Position1") == 1)
        {
            SuperShoot();
            shootSuperTimer = 0;
        }

        dashTimer += Time.deltaTime;

        dashSlider.value = dashTimer / timeBtwDash;

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {

            if(dashTimer >= timeBtwDash)
            {
                dashTimer = 0;
                ActivateDash();
            }
        }

        if (timeBtwShoot - shootTimer < 0) return;

        text.text = ((int)
            ((timeBtwShoot - shootTimer)
            * 100) 
            / 100f).ToString();
    }

    private void FixedUpdate()
    {
        Move();

        if (isDashing) Dash();
    }

    void UpdateTimeBtwShoot()
    {
        timeBtwShoot -= 0.1f;
        timeBtwSuperShoot -= 0.5f;
    }

    #region Base Function

    void Dash()
    {
        rb.AddForce(moveInput * Time.fixedDeltaTime * dashForce * 100);
    }

    void ActivateDash()
    {
        isDashing = true;
        canBeDamaged = false;

        SoundManager.instance.PlayerSound(dashSound);

        Invoke(nameof(DeActivateDash), dashTime);
    }
    
    void DeActivateDash()
    {
        isDashing = false;
        canBeDamaged = true;
    }

    void Move()
    {
        moveInput = new Vector2(Input.GetAxisRaw("Horizontal"),
             Input.GetAxisRaw("Vertical"));

        if(moveInput != Vector2.zero)
        {
            anim.SetBool("run", true);
            footParticle.Pause();
            footParticle.Play();

            var emission = footParticle.emission;

            emission.rateOverTime = 10;


            if (!audS.isPlaying)
            {
                audS.clip = footClips[Random.Range(0, footClips.Length)];
                audS.Play();
            }
        }
        else
        {
            anim.SetBool("run", false);

            var emission = footParticle.emission;

            emission.rateOverTime = 0;
        }



        ScalePlayer(moveInput.x);

        moveVelocity = moveInput.normalized * speed;

        rb.MovePosition(rb.position + moveVelocity * Time.fixedDeltaTime);
    }

    void ScalePlayer(float x)
    {
        if (x == 1)
            spR.flipX = false;
        else if (x == -1)
            spR.flipX = true;
    }

    #endregion

    void Shoot()
    {
        Instantiate(bullet, shootPos.position, shootPos.rotation);

        SoundManager.instance.PlayerSound(shootClip);

        StartCoroutine(nameof(SetMuzzleFlash));
    }
    
    void SuperShoot()
    {
        for (int i = 0; i < shootSuperPos.Length; i++)
        {
            Instantiate(bullet, shootSuperPos[i].position, shootSuperPos[i].rotation);
        }
        SoundManager.instance.PlayerSound(superShootClip);
        CameraFollow.instance.CamShake();

        StartCoroutine(nameof(SetMuzzleFlash));
    }

    IEnumerator SetMuzzleFlash()
    {
        muzzleFlashSpR.enabled = true;
        muzzleFlashSpR.sprite = spritesMuzzleFlash[Random.Range(0, spritesMuzzleFlash.Length)];

        yield return new WaitForSeconds(0.1f);

        muzzleFlashSpR.enabled = false;
    }


    public void Damage(int damage)
    {
        if (!canBeDamaged) return;

        health -= damage;

        Instantiate(hitEffect, transform.position, Quaternion.identity);

        CameraFollow.instance.CamShake();

        SoundManager.instance.PlayerSound(heartClip);

        UpdateHealthUI();

        if (health <= 0 && deathPanel.activeInHierarchy == false) {

            SoundManager.instance.PlayerSound(deathClip);

            deathPanel.SetActive(true);

            gameObject.SetActive(false);
        }
    }

    public void AddHealth(int value)
    {
        if (health <= 0) health = 0;

        health += value;
        if (health > maxHealth) health = maxHealth;

        UpdateHealthUI();
    }


    void UpdateHealthUI()
    {
        healthSlider.value = (float)health / maxHealth;
    }

    [HideInInspector] public int currentMoney;
    [SerializeField] TextMeshProUGUI coinsText;
    public void AddMoney(int value)
    {
        currentMoney += value;
        coinsText.text = "У вас " + currentMoney.ToString() + " монеток";
    }
}
