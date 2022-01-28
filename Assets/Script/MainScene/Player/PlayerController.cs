using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    #region 属性值
/******************************************************************************************/
    [HideInInspector]public float  current_health;
    [HideInInspector]public float  current_energy;
    [Header("冲刺耗蓝")]public float  energy_cost_sprint;
    [Header("跳跃耗蓝")]public float  energy_cost_jump;
    [Header("空中猛击每秒耗蓝")][Range(1,20)]public float energy_cost_slash;
    [Header("普通射击耗蓝")]public float energy_cost_shoot;
    [Header("大招耗蓝")]public float energy_cost_shoot2;
    [Header("自动回蓝量")]public float  energy_recovery_value;
    [Header("回复蓝量时间间隔")]public float energy_recovery_time;
    public float  health;
    public float  energy;
    public float  speed;
    [Header("跳跃次数")][Range(1,5)]public int jumpTimes;
    [Range(200,1000)]public float jumpHeight;
    #endregion
    #region 技能
/*****************************************************************************/
    [Header("冲刺速度")]public float sprint_speed;
    [Header("冲刺时间")]public float sprint_time;
    [Header("冲刺冷却")]public float sprint_cd;
    float sprint_current_cd;
    [Header("大招冷却")]public float shoot2_cd;
    [Header("大招切割时间")]public float sprint2_time;
    float shoot2_current_cd;
    #endregion
    #region 其它变量
/***************************************************************************************************/
    Rigidbody2D rb;
    //跳跃
    [HideInInspector]public int jumpTimer;
    [HideInInspector]public bool is_onFloor;
    //移动
    public bool is_againstingWall;
    //冲刺
    [HideInInspector]public float sprint_timer;
    bool is_sprinting;
    bool isReady_Sprint;
    Vector3 sprint_dir;
    Vector3 mouse_pos;
    //射击
    float shoot_timer;
    [Header("射击间隔")][Range(0.1f,3)]public float shoot_time;
    //大招切割
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
    //空中猛击
    Vector3 slash_dir;
    bool is_static;             //是否静止
    float slash_timer;
    [Header("刀光间隔")][Range(0.1f,3)]public float slash_time;
    //生命
    [HideInInspector]public bool is_invincible = false;
    //能量
    float energy_covery_timer;
    //动画
    Animator anim;
    //碰撞
    float trigger_timer;
    float trigger_time = 0.5f;
    bool trigger_manager = false;
    bool trigger_manager2 = false;

    //TimeScale

    #endregion
    #region 特效
    [Header("冲刺特效")]public GameObject sprintEffect;
    [Header("空中刀光特效")]public GameObject slashEffect;
    [Header("跳跃特效")]public GameObject jumpEffect;

    [Header("切割轨迹特效")]public GameObject player_trailEffect;
    [Header("切割特效")]public GameObject shootEffect2;
    [Header("大招子弹")]public GameObject player_bullet2;
    [Header("普通子弹")]public GameObject player_bullet;
    #endregion
    #region UI
    public Slider health_slider;
    public Slider energy_slider;
    public Image jump_img;
    public Image sprint_img;
    public Image shoot2_img;        //大招一段
    public GameObject shoot2_img_2;//大招二段
    
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
        #region 计时器
        //计时器
        jumpTimer = jumpTimes;
        sprint_timer = sprint_time;
        energy_covery_timer = energy_recovery_time;
        shoot_timer = shoot_time;
        slash_timer = slash_time;
        sprint2_timer = sprint2_time;
        trigger_timer = trigger_time;
        //敌人
        emyObjs = GameObject.FindGameObjectsWithTag("enemy");
        //冷却时间
        sprint_current_cd = sprint_cd;
        #endregion
    }
/**********************************************************************************/
/****************************      Update      ************************************/
/**********************************************************************************/
    void Update()
    {
        if (is_static) rb.constraints = RigidbodyConstraints2D.FreezePosition;

        #region 使用技能
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
        #region 玩家移动
/*******************************************************************************************************/
        if (!is_sprinting && !is_static)
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            if (is_againstingWall)
                rb.velocity = new Vector2(0, rb.velocity.y);
            else rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);

        }
        #endregion
        #region UI更新,能量回复
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
    #region 改变属性值
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
    #region 技能
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
                ChangeVariables(-energy_cost_jump, Variables.current_energy);//不是第一次跳，则耗蓝
            }
        }
    }//跳跃
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
            if (sprint_current_cd == sprint_cd)//冲刺第一帧
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
    }//冲刺
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

    }//射击
    private void ShootPlus()
    {
        
        //一段，射子弹
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
        //二段,切割
























                        //射线存在且按下R    ,进入切割时间//
        GameObject lineObj = GameObject.FindGameObjectWithTag("Line");
        if (lineObj != null&&Input.GetKeyDown(KeyCode.R))       //只执行一帧
        {
            is_invincible = true;
            is_ready_b2 = true;                             //进入切割时间
            player_trailEffect.gameObject.SetActive(true);
            //is_sprinting_b2 = true;                         //锁定其它移动
            target_emyObj = emyObjs[lineObj.GetComponent<LineEffect>().index];
            GameObject.Destroy(lineObj);
            target0 = target_emyObj.transform.position;
            dir0 = (target0 - transform.position).normalized;
            begin_first_sprint2 = true;
        }
        if (begin_first_sprint2)
        {


            /***************************   第一次切割   ****************************/
            //敌人坐标
            target = target_emyObj.transform.position;
            rb.gravityScale = 0;
            //判断是否到达敌人
            if (Vector3.Distance(transform.position, target) <= 0.5f)
            {
                //到达敌人，进行冲刺
                has_arrived = true;
                //Vector3 dir0 = (target0 - transform.position).normalized;
                //rb.velocity = (new Vector2(dir0.x, dir0.y)) * sprint_speed;


            }
            else if (!has_arrived)//没有到达，继续移动
            {

                transform.up = Vector3.Slerp(transform.up
                            , target - transform.position
                            , 5f / Vector2.Distance(transform.position, target));
                transform.position += transform.up * sprint_speed * Time.deltaTime;
            }

            //如果到达敌人
            if (has_arrived)
            {
                sprint_timer -= Time.deltaTime;
                transform.position += (new Vector3(dir0.x, dir0.y, 0)) * sprint_speed * Time.deltaTime;
                if (sprint_timer <= 0)//切割过程结束
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



                            //切割时间计时器//
        if (is_ready_b2)
        {
            shoot2_img_2.gameObject.SetActive(true);
            shoot2_img_2.GetComponent<Image>().fillAmount = sprint2_timer / sprint2_time;
            sprint2_timer -= Time.deltaTime;
            if (sprint2_timer <= 0)
            {
                StartCoroutine(Invincible());                  //是否无敌
                is_ready_b2 = false;
                shoot2_img_2.gameObject.SetActive(false);
                player_trailEffect.gameObject.SetActive(false);
                sprint2_timer = sprint2_time;
                transform.eulerAngles = Vector3.zero;
                rb.gravityScale = 3;
                transform.position = target0;
            }
        }




                        //切割时间//
        if (Input.GetKeyDown(KeyCode.R) && is_ready_b2 && begin_sprint2 == false && begin_first_sprint2 == false)//执行一帧
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
                        /***************************   每次切割   ****************************/
            //敌人坐标
            target = target_emyObj.transform.position;
            rb.gravityScale = 0;
            //判断是否到达敌人
            if (Vector3.Distance(transform.position, target) <= 0.5f)
            {
                //到达敌人，进行冲刺
                has_arrived = true;
                //Vector3 dir0 = (target0 - transform.position).normalized;
                //rb.velocity = (new Vector2(dir0.x, dir0.y)) * sprint_speed;


            }
            else if(!has_arrived)//没有到达，继续移动
            {

                transform.up = Vector3.Slerp(transform.up
                            , target - transform.position
                            , 5f / Vector2.Distance(transform.position, target));
                transform.position += transform.up * sprint_speed * Time.deltaTime;
                //rb.velocity = (new Vector2((target - transform.position).x
                //               , (target - transform.position).y)).normalized * sprint_speed;
            }

            //如果到达敌人
            if (has_arrived)
            {
                sprint_timer -= Time.deltaTime;
                transform.position += (new Vector3(dir0.x, dir0.y, 0)) * sprint_speed * Time.deltaTime;

                if (sprint_timer <= 0)//切割过程结束
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
        //if (lineObj!=null && !is_ready_b2)//只要找到line则触发，允许按R
        //{
        //    is_ready_b2 = true;
        //    Debug.Log("触发");
        //}
        //if (is_ready_b2)
        //{
        //    /*!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!*/
        //    LineRenderer line = lineObj.GetComponent<LineRenderer>();

        //    shoot2_img_2.gameObject.SetActive(true);
        //    if (Input.GetKeyDown(KeyCode.R) && !is_sprinting_b2)//按下R则触发
        //    {
        //        //切割第一帧
        //        is_sprinting_b2 = true;
        //        Debug.Log("开始切割");
        //    }
        //    if (is_sprinting_b2)
        //    {
        //        bool has_arrived = false;
        //        target = line.GetPosition(1);
        //        Vector3 target0;
        //        if (Vector3.Distance(transform.position,target)<=0.5f)
        //        {
        //            //获取到达目标后第一帧坐标
        //            target0 = line.GetPosition(1);
        //            Vector3 dir0 = (target0 - transform.position).normalized;
        //            rb.velocity = (new Vector2(dir0.x, dir0.y)) * sprint_speed;
        //            has_arrived = true;
        //            Debug.Log("到达目标，开始冲刺");
        //        }
        //        else
        //        {
        //            target0 = Vector3.zero;
        //            Debug.Log("没有到达,继续移动");
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

        //            if (sprint_timer <= 0)//切割过程结束
        //            {
        //                Debug.Log("冲刺结束");
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













        
    }//大招
    private void Slash()
    {
        if (!is_onFloor && Input.GetMouseButton(1) && current_energy >= 5)
        {
            mouse_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            slash_dir = (new Vector3(mouse_pos.x - transform.position.x, mouse_pos.y - transform.position.y, 0)).normalized;
            //特效生成时旋转角度
            float effect_rotation = Mathf.Atan2(slash_dir.y,slash_dir.x)*Mathf.Rad2Deg;
            is_static = true;
            if (slash_timer == slash_time)//第一帧
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
    }//空中猛击
    #endregion
    #region 血量，能量回复
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
    #region 碰撞
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
        if (GUILayout.Button("测试")) StartCoroutine(Wait());
    }
     */
    private void OnTriggerEnter2D(Collider2D other)//trigger_manager
    {
        //trigger_manager为true不发生交互
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

    /***************************    协程    ***************************/
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
