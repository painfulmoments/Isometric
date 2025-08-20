using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float turnSensitivity = 2f;
    public float turnSmoothness = 5f;
    public float deathHeight = -5f;

    [Header("死亡面板")]
    public GameObject deathPanel;
    public Button restartButton;
    public Button mainMenuButton;

    [Header("胜利面板")]
    public GameObject winPanel;              // <- 在 Inspector 拖一个 WinPanel 进来
    [Tooltip("达到该分数即触发胜利状态")]
    public int targetScore = 60;             // <- 可在 Inspector 修改

    private Rigidbody rb;
    private Vector3 moveDirection = Vector3.forward;
    private bool isDead = false;
    private bool isWon = false;              // <- 新增：胜利标记，避免重复触发

    [Header("计分系统")]
    public TMP_Text scoreText;
    public int scorePerSecond = 10;

    private float scoreTimer = 0f;
    private int currentScore = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // 防止翻滚

        restartButton.onClick.AddListener(RestartGame);
        mainMenuButton.onClick.AddListener(BackToMainMenu);

        if (deathPanel != null) deathPanel.SetActive(false);
        if (winPanel != null) winPanel.SetActive(false); // <- 确保开始时隐藏
        UpdateScoreUI();
    }

    void Update()
    {
        if (isDead || isWon) return; // <- 胜利/死亡后停止一切输入与计分

        // —— 计分 —— //
        scoreTimer += Time.deltaTime;
        if (scoreTimer >= 1f)
        {
            currentScore += scorePerSecond;
            UpdateScoreUI();
            scoreTimer = 0f;

            // 胜利判定
            if (currentScore >= targetScore)
            {
                Win();
                return;
            }
        }

        // —— 掉落死亡判定 —— //
        if (transform.position.y < deathHeight)
        {
            Die();
            return;
        }

        // —— 转向逻辑 —— //
        float mouseX = Input.GetAxis("Mouse X");
        if (Mathf.Abs(mouseX) > 0.01f)
        {
            Quaternion rotation = Quaternion.AngleAxis(mouseX * turnSensitivity, Vector3.up);
            moveDirection = rotation * moveDirection;
        }
        moveDirection = Vector3.Lerp(moveDirection, moveDirection.normalized, Time.deltaTime * turnSmoothness);

        // 更新角色朝向
        if (moveDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(moveDirection),
                Time.deltaTime * 10f
            );
        }
    }

    void FixedUpdate()
    {
        if (isDead || isWon) return; // <- 胜利/死亡后不再移动

        Vector3 velocity = moveDirection.normalized * moveSpeed;
        velocity.y = rb.velocity.y; // 保留当前 Y 轴速度（重力）
        rb.velocity = velocity;
    }

    void Die()
    {
        if (isDead || isWon) return;
        isDead = true;
        rb.velocity = Vector3.zero;
        rb.isKinematic = true; // 停止物理
        Time.timeScale = 0f;

        if (deathPanel != null) deathPanel.SetActive(true);
    }

    // —— 新增：胜利处理 —— //
    void Win()
    {
        if (isWon || isDead) return;
        isWon = true;
        rb.velocity = Vector3.zero;
        rb.isKinematic = true;
        Time.timeScale = 0f;

        if (winPanel != null) winPanel.SetActive(true);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        currentScore = 0;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Start");
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = " " + currentScore;
        }
    }
}
