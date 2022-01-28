using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    #region ����ֵ
/******************************************************************************************/
    [HideInInspector]public float  current_health;
    [HideInInspector]public float  current_energy;
    [Header("��̺���")]public float  energy_cost_sprint;
    [Header("��Ծ����")]public float  energy_cost_jump;
    [Header("�����ͻ�ÿ�����")][Range(1,20)]public float energy_cost_slash;
    [Header("��ͨ�������")]public float energy_cost_shoot;
    [Header("���к���")]public float energy_cost_shoot2;
    [Header("�Զ�������")]public float  energy_recovery_value;
    [Header("�ظ�����ʱ����")]public float energy_recovery_time;
    public float  health;
    public float  energy;
    public float  speed;
    [Header("��Ծ����")][Range(1,5)]public int jumpTimes;
    [Range(200,1000)]public float jumpHeight;
    #endregion
    #region ����
/*****************************************************************************/
    [Header("����ٶ�")]public float sprint_speed;
    [Header("���ʱ��")]public float sprint_time;
    [Header("�����ȴ")]public float sprint_cd;
    float sprint_current_cd;
    [Header("������ȴ")]public float shoot2_cd;
    [Header("�����и�ʱ��")]public float sprint2_time;
    float shoot2_current_cd;
    #endregion
    #region ��������
/***************************************************************************************************/
    Rigidbody2D rb;
    //��Ծ
    [HideInInspector]public int jumpTimer;
    [HideInInspector]public bool is_onFloor;
    //�ƶ�
    public bool is_againstingWall;
    //���
    [HideInInspector]public float sprint_timer;
    bool is_sprinting;
    bool isReady_Sprint;
    Vector3 sprint_dir;
    Vector3 mouse_pos;
    //���
    float shoot_timer;
    [Header("������")][Range(0.1f,3)]public float shoot_time;
    //�����и�
    bool is_sprinting_b2;
    bool is_ready_b2 = false;
    bool has_arrived = false;
    bool begin_sprint2 = false;
    bool begin_first_sprint2 = false;
    float sprint2_timer;
    Vector3 target;
    Vector3 target0;
    Vector3 dir0;
    GameObject[] emyObjs;
    GameObject target_emyObj;
    //�����ͻ�
    Vector3 slash_dir;
    bool is_static;             //�Ƿ�ֹ
    float slash_timer;
    [Header("������")][Range(0.1f,3)]public float slash_time;
    //����
    [HideInInspector]public bool is_invincible = false;
    //����
    float energy_covery_timer;
    //����
    Animator anim;
    //��ײ
    float trigger_timer;
    float trigger_time = 0.5f;
    bool trigger_manager = false;
    bool trigger_manager2 = false;

    //TimeScale

    #endregion
    #region ��Ч
    [Header("�����Ч")]public GameObject sprintEffect;
    [Header("���е�����Ч")]public GameObject slashEffect;
    [Header("��Ծ��Ч")]public GameObject jumpEffect;

    [Header("�и�켣��Ч")]public GameObject player_trailEffect;
    [Header("�и���Ч")]public GameObject shootEffect2;
    [Header("�����ӵ�")]public GameObject player_bullet2;
    [Header("��ͨ�ӵ�")]public GameObject player_bullet;
    #endregion
    #region UI
    public Slider health_slider;
    public Slider energy_slider;
    public Image jump_img;
    public Image sprint_img;
    public Image shoot2_img;        //����һ��
    public GameObject shoot2_img_2;//���ж���
    
    #endregion
/**********************************************************************************/
/****************************      Start      *************************************/
/**********************************************************************************/
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        current_energy = energy;
        current_health = health;
        #region ��ʱ��
        //��ʱ��
        jumpTimer = jumpTimes;
        sprint_timer = sprint_time;
        energy_covery_timer = energy_recovery_time;
        shoot_timer = shoot_time;
        slash_timer = slash_time;
        sprint2_timer = sprint2_time;
        trigger_timer = trigger_time;
        //����
        emyObjs = GameObject.FindGameObjectsWithTag("enemy");
        //��ȴʱ��
        sprint_current_cd = sprint_cd;
        #endregion
    }
/**********************************************************************************/
/****************************      Update      ************************************/
/**********************************************************************************/
    void Update()
    {
        if (is_static) rb.constraints = RigidbodyConstraints2D.FreezePosition;

        #region ʹ�ü���
        if (!is_static)
        {
            Sprint();
        }
        if (!is_static && !is_sprinting)
        {
            Jump();
            Shoot();
            ShootPlus();
        }
        if(!is_sprinting)
            Slash();



        #endregion
        #region ����ƶ�
/*******************************************************************************************************/
        if (!is_sprinting && !is_static)
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            if (is_againstingWall)
                rb.velocity = new Vector2(0, rb.velocity.y);
            else rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);

        }
        #endregion
        #region UI����,�����ظ�
/***************************************************************************************************/
        energy_slider.value = current_energy / energy;
        health_slider.value = current_health / health;
        jump_img.fillAmount = 1 - (float)(jumpTimes - jumpTimer) / (float)jumpTimes;
        sprint_img.fillAmount = 1 - sprint_current_cd / sprint_cd;
        shoot2_img.fillAmount = 1 - shoot2_current_cd / shoot2_cd;
        EnergyRecovery();

        #endregion
        ColliderManager();
        //if (!is_static) rb.constraints = ~RigidbodyConstraints2D.FreezePosition;
    }

/**********************************************************************************/
/***************************      Function      ***********************************/
/**********************************************************************************/
    #region �ı�����ֵ
/********************************************************************************************************/
    public enum Variables
    {
        current_health,
        current_energy,
        health,
        energy,
        speed
    }
    public void ChangeVariables(float value,Variables variable)
    {
        switch (variable)
        {
            case Variables.current_health:
                if (is_invincible)
                    current_health = Mathf.Clamp(current_health, 0, health);
                else
                    current_health = Mathf.Clamp(current_health + value, 0, health);
                break;
            case Variables.current_energy:
                current_energy = Mathf.Clamp(current_energy + value, 0, energy);
                break;
            case Variables.health:
                health = Mathf.Clamp(health + value, 1, 1000);
                break;
            case Variables.energy:
                energy = Mathf.Clamp(energy + value, 1, 1000);
                break;
            case Variables.speed:
                speed = Mathf.Clamp(speed + value, 1, 100);
                break;
            default:
                break;
        }
    }

    #endregion
    #region ����
/************************************************************************************/
    private void Jump()
    {
        if (is_onFloor)
        {
            if (Input.GetKeyDown(KeyCode.Space) && jumpTimer >= 1)
            {
                rb.velocity = new Vector2(rb.velocity.x, 0);
                rb.AddForce(new Vector2(0, jumpHeight));
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Space) && jumpTimer >= 1 && (current_energy > energy_cost_jump))
            {
                rb.velocity = new Vector2(rb.velocity.x, 0);
                rb.AddForce(new Vector2(0, jumpHeight));
                GameObject effect = Instantiate(jumpEffect, transform.position, Quaternion.identity);
                jumpTimer -= 1;
                ChangeVariables(-energy_cost_jump, Variables.current_energy);//���ǵ�һ�����������
            }
        }
    }//��Ծ
    private void Sprint()
    {
        sprint_current_cd = Mathf.Clamp(sprint_current_cd - Time.deltaTime, 0, sprint_cd);
        isReady_Sprint = sprint_current_cd == 0;
        if (Input.GetKeyDown(KeyCode.LeftShift) && isReady_Sprint && (current_energy>energy_cost_sprint))
        {
            is_sprinting = true;
            sprint_current_cd = sprint_cd;
        }
        if (is_sprinting)
        {
            anim.SetBool("StartSprint", true);
            if (sprint_current_cd == sprint_cd)//��̵�һ֡
            {
                ChangeVariables(-energy_cost_sprint, Variables.current_energy);
                mouse_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                sprint_dir = (new Vector3(mouse_pos.x - transform.position.x, mouse_pos.y - transform.position.y, 0)).normalized;

            }
            sprint_timer -= Time.deltaTime;



            rb.velocity = new Vector2(sprint_dir.x * sprint_speed, sprint_dir.y * sprint_speed);

            if (sprint_timer <= 0)
            {
                rb.velocity = Vector2.zero;
                is_sprinting = false;
                sprint_timer = sprint_time;
                anim.SetBool("StartSprint", false);
                GameObject sprint_effect = Instantiate(sprintEffect,transform.position, Quaternion.identity);
                Destroy(sprint_effect, 1);
            }
        }
    }//���
    private void Shoot()
    {
        if (Input.GetMouseButton(0)&&current_energy>=energy_cost_shoot)
        {
            shoot_timer -= Time.deltaTime;
            if (shoot_timer <= 0)
            {
                ChangeVariables(-energy_cost_shoot, Variables.current_energy);
                GameObject bullet = Instantiate(player_bullet, transform.position, Quaternion.identity);
                shoot_timer = shoot_time;
            }

        }

    }//���
    private void ShootPlus()
    {
        
        //һ�Σ����ӵ�
        shoot2_current_cd = Mathf.Clamp(shoot2_current_cd - Time.deltaTime, 0, shoot2_cd);
        if (Input.GetKeyDown(KeyCode.R) && current_energy >= energy_cost_shoot2 && shoot2_current_cd == 0)
        {
            shoot2_current_cd = shoot2_cd;
            ChangeVariables(-energy_cost_shoot2, Variables.current_energy);
            for (int i = 0; i < 6; i++)
            {
                GameObject bullet2 = Instantiate(player_bullet2, transform.position, Quaternion.Euler(0, 0, (i-3) * 30));
            }
        }
        //����,�и�
























                        //���ߴ����Ұ���R    ,�����и�ʱ��//
        GameObject lineObj = GameObject.FindGameObjectWithTag("Line");
        if (lineObj != null&&Input.GetKeyDown(KeyCode.R))       //ִֻ��һ֡
        {
            is_invincible = true;
            is_ready_b2 = true;                             //�����и�ʱ��
            player_trailEffect.gameObject.SetActive(true);
            //is_sprinting_b2 = true;                         //���������ƶ�
            target_emyObj = emyObjs[lineObj.GetComponent<LineEffect>().index];
            GameObject.Destroy(lineObj);
            target0 = target_emyObj.transform.position;
            dir0 = (target0 - transform.position).normalized;
            begin_first_sprint2 = true;
        }
        if (begin_first_sprint2)
        {


            /***************************   ��һ���и�   ****************************/
            //��������
            target = target_emyObj.transform.position;
            rb.gravityScale = 0;
            //�ж��Ƿ񵽴����
            if (Vector3.Distance(transform.position, target) <= 0.5f)
            {
                //������ˣ����г��
                has_arrived = true;
                //Vector3 dir0 = (target0 - transform.position).normalized;
                //rb.velocity = (new Vector2(dir0.x, dir0.y)) * sprint_speed;


            }
            else if (!has_arrived)//û�е�������ƶ�
            {

                transform.up = Vector3.Slerp(transform.up
                            , target - transform.position
                            , 5f / Vector2.Distance(transform.position, target));
                transform.position += transform.up * sprint_speed * Time.deltaTime;
            }

            //����������
            if (has_arrived)
            {
                sprint_timer -= Time.deltaTime;
                transform.position += (new Vector3(dir0.x, dir0.y, 0)) * sprint_speed * Time.deltaTime;
                if (sprint_timer <= 0)//�и���̽���
                {
                    rb.velocity = Vector2.zero;
                    sprint_timer = sprint_time;
                    is_invincible = false;
                    has_arrived = false;
                    begin_first_sprint2 = false;
                    player_trailEffect.gameObject.SetActive(false);
                    rb.gravityScale = 3;
                    transform.eulerAngles = Vector3.zero;
                }
            }
        }



                            //�и�ʱ���ʱ��//
        if (is_ready_b2)
        {
            shoot2_img_2.gameObject.SetActive(true);
            shoot2_img_2.GetComponent<Image>().fillAmount = sprint2_timer / sprint2_time;
            sprint2_timer -= Time.deltaTime;
            if (sprint2_timer <= 0)
            {
                StartCoroutine(Invincible());                  //�Ƿ��޵�
                is_ready_b2 = false;
                shoot2_img_2.gameObject.SetActive(false);
                player_trailEffect.gameObject.SetActive(false);
                sprint2_timer = sprint2_time;
                transform.eulerAngles = Vector3.zero;
                rb.gravityScale = 3;
                transform.position = target0;
            }
        }




                        //�и�ʱ��//
        if (Input.GetKeyDown(KeyCode.R) && is_ready_b2 && begin_sprint2 == false && begin_first_sprint2 == false)//ִ��һ֡
        {
            target0 = target_emyObj.transform.position;
            dir0 = (target0 - transform.position).normalized;
            is_invincible = true;
            player_trailEffect.gameObject.SetActive(true);
            begin_sprint2 = true;
        }
        if (!is_ready_b2)
        {
            begin_sprint2 = false;
        }


        if (begin_sprint2)
        {
                        /***************************   ÿ���и�   ****************************/
            //��������
            target = target_emyObj.transform.position;
            rb.gravityScale = 0;
            //�ж��Ƿ񵽴����
            if (Vector3.Distance(transform.position, target) <= 0.5f)
            {
                //������ˣ����г��
                has_arrived = true;
                //Vector3 dir0 = (target0 - transform.position).normalized;
                //rb.velocity = (new Vector2(dir0.x, dir0.y)) * sprint_speed;


            }
            else if(!has_arrived)//û�е�������ƶ�
            {

                transform.up = Vector3.Slerp(transform.up
                            , target - transform.position
                            , 5f / Vector2.Distance(transform.position, target));
                transform.position += transform.up * sprint_speed * Time.deltaTime;
                //rb.velocity = (new Vector2((target - transform.position).x
                //               , (target - transform.position).y)).normalized * sprint_speed;
            }

            //����������
            if (has_arrived)
            {
                sprint_timer -= Time.deltaTime;
                transform.position += (new Vector3(dir0.x, dir0.y, 0)) * sprint_speed * Time.deltaTime;

                if (sprint_timer <= 0)//�и���̽���
                {
                    rb.velocity = Vector2.zero;
                    sprint_timer = sprint_time;
                    has_arrived = false;
                    player_trailEffect.gameObject.SetActive(false);
                    rb.gravityScale = 3;
                    begin_sprint2 = false;
                    transform.eulerAngles = Vector3.zero;
                }
            }
        }





































/****************************************************************************************************************************/
        //GameObject lineObj = GameObject.FindGameObjectWithTag("Line");
        //if (lineObj!=null && !is_ready_b2)//ֻҪ�ҵ�line�򴥷�������R
        //{
        //    is_ready_b2 = true;
        //    Debug.Log("����");
        //}
        //if (is_ready_b2)
        //{
        //    /*!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!*/
        //    LineRenderer line = lineObj.GetComponent<LineRenderer>();

        //    shoot2_img_2.gameObject.SetActive(true);
        //    if (Input.GetKeyDown(KeyCode.R) && !is_sprinting_b2)//����R�򴥷�
        //    {
        //        //�и��һ֡
        //        is_sprinting_b2 = true;
        //        Debug.Log("��ʼ�и�");
        //    }
        //    if (is_sprinting_b2)
        //    {
        //        bool has_arrived = false;
        //        target = line.GetPosition(1);
        //        Vector3 target0;
        //        if (Vector3.Distance(transform.position,target)<=0.5f)
        //        {
        //            //��ȡ����Ŀ����һ֡����
        //            target0 = line.GetPosition(1);
        //            Vector3 dir0 = (target0 - transform.position).normalized;
        //            rb.velocity = (new Vector2(dir0.x, dir0.y)) * sprint_speed;
        //            has_arrived = true;
        //            Debug.Log("����Ŀ�꣬��ʼ���");
        //        }
        //        else
        //        {
        //            target0 = Vector3.zero;
        //            Debug.Log("û�е���,�����ƶ�");
        //        }


        //        if (!has_arrived)
        //        {
        //            /*!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!*/
                    
        //             //rb.velocity = (new Vector2((target - transform.position).x
        //             //               , (target - transform.position).y)).normalized * sprint_speed;
        //            transform.up = Vector3.Slerp(transform.up
        //            , target - transform.position
        //            , 0.5f / Vector2.Distance(transform.position, target));

        //            transform.position += transform.up * sprint_speed * Time.deltaTime;
                    
        //            /*!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!*/
        //        }
        //        else
        //        {
        //            rb.velocity = (new Vector2((target0 - transform.position).x
        //                            , (target0 - transform.position).y)).normalized * sprint_speed;
        //            sprint_timer -= Time.deltaTime;

        //            if (sprint_timer <= 0)//�и���̽���
        //            {
        //                Debug.Log("��̽���");
        //                rb.velocity = Vector2.zero;
        //                sprint_timer = sprint_time;
        //                is_ready_b2 = false;
        //                is_sprinting_b2 = false;
        //                has_arrived = false;
        //            }
        //        }
        //    }
        //}
        //else
        //{
        //    shoot2_img_2.gameObject.SetActive(false);
        //}
/****************************************************************************************************************************/













        
    }//����
    private void Slash()
    {
        if (!is_onFloor && Input.GetMouseButton(1) && current_energy >= 5)
        {
            mouse_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            slash_dir = (new Vector3(mouse_pos.x - transform.position.x, mouse_pos.y - transform.position.y, 0)).normalized;
            //��Ч����ʱ��ת�Ƕ�
            float effect_rotation = Mathf.Atan2(slash_dir.y,slash_dir.x)*Mathf.Rad2Deg;
            is_static = true;
            if (slash_timer == slash_time)//��һ֡
            {
                GameObject effect = Instantiate(slashEffect,
                    transform.position + slash_dir,
                    Quaternion.Euler(0, 0, effect_rotation+180));
            }
            if (slash_timer < 0)
            {
                GameObject effect = Instantiate(slashEffect,
                    transform.position + slash_dir,
                    Quaternion.Euler(0, 0, effect_rotation+180)); 
                slash_timer = slash_time;
            }
            ChangeVariables(-Time.deltaTime * energy_cost_slash, Variables.current_energy);
            slash_timer -= Time.deltaTime;
        }
        else
        {
            is_static = false;
            rb.constraints = ~RigidbodyConstraints2D.FreezePosition;
            rb.AddForce(new Vector2(0, -0.01f));
        }
    }//�����ͻ�
    #endregion
    #region Ѫ���������ظ�
/*******************************************************************************************************/
    void EnergyRecovery()
    {
        energy_covery_timer -= Time.deltaTime;
        if (energy_covery_timer<=0)
        {
            ChangeVariables(energy_recovery_value,Variables.current_energy);
            energy_covery_timer = energy_recovery_time;
        }
    }

    #endregion
    #region ��ײ
    private void ColliderManager()
    {
        if (trigger_manager)
        {
            trigger_timer -= Time.deltaTime;
            if (trigger_timer<=0)
            {
                trigger_manager = false;
            }
        }
    }
    #endregion
    /*
    private void OnGUI()
    {
        if (GUILayout.Button("����")) StartCoroutine(Wait());
    }
     */
    private void OnTriggerEnter2D(Collider2D other)//trigger_manager
    {
        //trigger_managerΪtrue����������
        trigger_manager = true;
        trigger_timer = trigger_time;
        if (other.tag == "enemy"&& is_ready_b2)
        {
            trigger_manager2 = true;
            
            GameObject effect = Instantiate(shootEffect2, other.transform.position, Quaternion.identity);
        }
        if (trigger_manager2)
        {
            
            StartCoroutine(Wait());
        }



    }

    /***************************    Э��    ***************************/
    IEnumerator Wait()
    {
        Time.timeScale = 0.1f;
        yield return new WaitForSecondsRealtime(0.3f);
        Time.timeScale = 1;
        trigger_manager2 = false;
        yield return null;
    }
    IEnumerator Invincible()
    {
        yield return new WaitForSecondsRealtime(1);
        is_invincible = false;
        yield return null;
    }
}
