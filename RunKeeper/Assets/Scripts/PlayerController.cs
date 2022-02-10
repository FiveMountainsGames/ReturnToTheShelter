using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    const float startShotDelay = 0.28f;

    private Rigidbody2D playerRb;
    private Animator anim;
    private AudioSource audio;
    [SerializeField] ParticleSystem dirtParticles;
    [SerializeField] List<AudioClip> steps;
    [SerializeField] List<AudioClip> fx;
    [SerializeField] Transform shotPoint;
    [SerializeField] LineRenderer shotLine;
    [SerializeField] List<GameObject> hitFX;
    public GameObject viewEnemy;

    [Range(0, 100)] public int hp = 100;
    private int adrenaline = 0;
    private int ammoCount = 7;
    private int magazines = 5;

    private float jumpForce = 12f;
    private float gravityModifier = 1.5f;
    private float shotDelay;
    public int cupsValue = 0;
    private float playerDeep = 0.1f;
    public float speedUp = 1.5f;
    private float stamina = 100;
    private float startGeneralSpeed;

    public bool isGround = false;
    private bool isLowDash = false;
    public bool isSpeedUp = false;
    private bool firstGroundContact = false;
    private bool isReload = false;
    public bool isUnderAttack = false;

    public float speed;
    
    public GameObject ground;
    public GameObject shootEffect;
    public List<AudioClip> audioClips;

    Coroutine stepCor;

    private void Start()
    {
        playerRb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        audio = GetComponent<AudioSource>();
        Physics.gravity *= gravityModifier;
        shotDelay = startShotDelay;
        GameManager.Instance.SetCups(cupsValue);
        startGeneralSpeed = GameManager.Instance.generalSpeed;
    }

    private void Update()
    {
        if (hp <= 0)
        {
            GameManager.Instance.GameOver();
        }
        else
        {
            GameManager.Instance.SetHP(hp);
        }

        if (!GameManager.Instance.isGameOver)
        {
            PcControls();
        }

        if (!isGround || isLowDash)
        {
            if (stepCor != null)
            {
                StopCoroutine(stepCor);
                stepCor = null;
            }
        }

        if (isUnderAttack)
        {
            anim.SetBool("isShoot", false);
            anim.SetBool("isUnderAttack", true);
            GameManager.Instance.generalSpeed = 0.5f;
        }
        else
        {
            anim.SetBool("isUnderAttack", false);
            GameManager.Instance.generalSpeed = GameManager.Instance.currGeneralSpeed;
        }
    }

    private void PcControls()
    {
        if (Input.GetButtonDown("Jump") && isGround && !isLowDash && !isUnderAttack)
        {
            firstGroundContact = false;
            playerRb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            audio.PlayOneShot(fx[0]);
            dirtParticles.Stop();
            anim.SetBool("isJump", true);
            isGround = false;
            firstGroundContact = true;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && !isUnderAttack)
        {
            isSpeedUp = !isSpeedUp;
        }

        SpeedUp();

        if (Input.GetKey(KeyCode.W) && ground.transform.position.y < 2 && isGround && !isLowDash)
        {
            UpDownMove(Vector3.up);
        }
        else if (Input.GetKey(KeyCode.S) && ground.transform.position.y > 0 && isGround && !isLowDash)
        {
            UpDownMove(-Vector3.up);
        }

        if (Input.GetKeyDown(KeyCode.LeftControl) && isGround && !isLowDash && !isUnderAttack)
        {
            StartCoroutine(LowDash());
        }

        if (Input.GetMouseButton(0) && isGround && !isLowDash)
        {
            Shoot(true);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            Shoot(false);
            shotDelay = startShotDelay;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            AdrenalineAction();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(Reload());
        }

        if (isUnderAttack)
        {
            anim.SetBool("isUnderAttack", true);
        }
        else
        {
            anim.SetBool("isUnderAttack", false);
        }
    }

    void AdrenalineAction()
    {
        if (adrenaline > 0)
        {
            adrenaline--;
            GameManager.Instance.SetAdrenaline(adrenaline);

            if (hp < 100)
            {
                hp += 25;
            }

            if (stamina < 100)
            {
                stamina += 50;
            }
            StartCoroutine(SlowMotion());
        }
    }

    IEnumerator SlowMotion()
    {
        Time.timeScale = 0.5f;
        yield return new WaitForSeconds(2.5f);
        Time.timeScale = 1f;
    }

    private void SpeedUp()
    {
        if (stamina > 0)
        {
            if (isSpeedUp)
            {
                stamina -= Time.deltaTime * 10;
                GameManager.Instance.generalSpeed = GameManager.Instance.generalSpeed * speedUp;
            }
        }
        else
        {
            isSpeedUp = false;
        }

        if (stamina < 100 && !isSpeedUp)
        {
            stamina += Time.deltaTime * 5;
            GameManager.Instance.generalSpeed = startGeneralSpeed;
        }

        GameManager.Instance.SetStamina(stamina);
    }

    private void Shoot(bool state)
    {
        if (state && ammoCount > 0 || magazines > 0)
        {
            if (!isUnderAttack)
            {
                anim.SetBool("isShoot", state);
            }
            ShotTime();
        }
        if (!state || ammoCount == 0 && magazines == 0)
        {
            anim.SetBool("isShoot", false);
            shootEffect.GetComponent<Animator>().SetBool("isShoot", false);
        }
    }

    private void ShotTime()
    {
        if (shotDelay > 0 || isReload)
        {
            shotDelay -= Time.deltaTime;
            shootEffect.GetComponent<Animator>().SetBool("isShoot", false);
        }
        else if (shotDelay <= 0)
        {
            if (ammoCount > 0)
            {
                shootEffect.GetComponent<Animator>().SetBool("isShoot", true);
                audio.PlayOneShot(audioClips[0]);
                if (EnemyPosition() != Vector2.zero)
                {
                    RaycastHit2D hitInfo = Physics2D.Raycast(shotPoint.localPosition, EnemyPosition(), LayerMask.GetMask("Enemies"));
                    if (hitInfo)
                    {
                        if (hitInfo.transform.CompareTag("Enemy"))
                        {
                            shotLine.SetPosition(0, shotPoint.position);
                            shotLine.SetPosition(1, hitInfo.transform.position);
                            hitInfo.collider.gameObject.GetComponent<RatEnemy>().hp -= 10;
                            StartCoroutine(HitFxPlay(hitFX[0], hitInfo.transform.position));

                        }
                    }
                }
                else
                {
                    shotLine.SetPosition(0, shotPoint.position);
                    shotLine.SetPosition(1, (Vector2)shotPoint.position + Vector2.right * 20);
                }
                StartCoroutine(ShotLineOnOff());

                ammoCount--;
                GameManager.Instance.SetAmmo(ammoCount);
                shotDelay = startShotDelay;
            }
            else
            {
                StartCoroutine(Reload());
            }
        }
    }

    IEnumerator Reload()
    {
        if (magazines > 0 && ammoCount < 7)
        {
            anim.SetBool("isShoot", false);
            isReload = true;
            anim.SetBool("isReload", isReload);
            audio.PlayOneShot(fx[5]);
            yield return new WaitForSeconds(0.6f);
            isReload = false;
            anim.SetBool("isReload", isReload);
            magazines--;
            ammoCount = 7;
            GameManager.Instance.SetMagazines(magazines);
            GameManager.Instance.SetAmmo(ammoCount);
        }
    }

    IEnumerator HitFxPlay(GameObject hitFxReciver, Vector3 position)
    {
        hitFxReciver.SetActive(true);
        hitFxReciver.transform.position = (Vector2)position + new Vector2(0, 0.4f);
        yield return new WaitForSeconds(0.2f);
        hitFxReciver.SetActive(false);
    }

    IEnumerator ShotLineOnOff()
    {
        shotLine.enabled = true;
        yield return new WaitForSeconds(0.02f);
        shotLine.enabled = false;
    }

    void UpDownMove(Vector3 dir)
    {
        if (dir != Vector3.zero)
        {
            transform.Translate(dir * Time.deltaTime * speed);
            ground.transform.Translate(dir * Time.deltaTime * speed);
        }
    }

    IEnumerator Steps()
    {
        while (!GameManager.Instance.isGameOver)
        {
            yield return new WaitForSeconds(0.35f);
            int stepRandom = UnityEngine.Random.Range(0, steps.Count);
            audio.PlayOneShot(steps[stepRandom]);

        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGround = true;
            if (firstGroundContact)
            {
                audio.PlayOneShot(fx[1]);
            }
            dirtParticles.Play();
            if (stepCor == null)
            {
                stepCor = StartCoroutine(Steps());
            }
            anim.SetBool("isJump", false);
        }

        if (collision.gameObject.CompareTag("Obstacle"))
        {
            audio.PlayOneShot(fx[2]);
            dirtParticles.Stop();
            anim.SetTrigger("isCollision");
            GameManager.Instance.GameOver();
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Cups"))
        {
            cupsValue += UnityEngine.Random.Range(1, 15);
            GameManager.Instance.SetCups(cupsValue);
            audio.PlayOneShot(fx[4]);
            ObjectPool.Instance.DeleteObject(collision.gameObject);
        }

        if (collision.CompareTag("Adrenaline"))
        {
            adrenaline++;
            GameManager.Instance.SetAdrenaline(adrenaline);
            ObjectPool.Instance.DeleteObject(collision.gameObject);
        }
    }


    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Obstacle"))
        {
            if (isLowDash)
            {
                collision.GetComponent<PolygonCollider2D>().isTrigger = true;
                collision.GetComponent<SpriteRenderer>().sortingOrder = 8;
            }
        }
    }

    private IEnumerator LowDash()
    {
        anim.SetTrigger("lowDash");
        isLowDash = true;
        audio.PlayOneShot(fx[3]);
        var main = dirtParticles.main;
        main.maxParticles = 15;
        yield return new WaitForSeconds(1.0f);
        main.maxParticles = 5;
        isLowDash = false;
        if (stepCor == null)
        {
            stepCor = StartCoroutine(Steps());
        }
    }

    private Vector2 EnemyPosition()
    {
        if (viewEnemy != null)
        {
            if (Vector2.Distance(transform.position, viewEnemy.transform.position) < 20)
            {
                return viewEnemy.transform.position;
            }
        }

        return Vector2.zero;
    }
}
