using UnityEngine;

public class NewPlayerAnimation : MonoBehaviour
{
    [Header("参照設定")]
    [SerializeField] private Animator _animator;

    private PlayerMove _playerMove;
    private PlayerState _playerState;
    private Rigidbody _rb;

    // Animator パラメータ ID
    private int _animIDSpeed;
    private int _animIDGrounded;
    private int _animIDJump;

    private bool _wasGrounded;

    void Start()
    {
        if (_animator == null) _animator = GetComponent<Animator>();

        // 親オブジェクト (PlayerModel) からコンポーネントを取得
        _playerMove = GetComponentInParent<PlayerMove>();
        _playerState = GetComponentInParent<PlayerState>();
        _rb = GetComponentInParent<Rigidbody>();

        if (_playerMove == null || _playerState == null || _rb == null)
        {
            Debug.LogError("必須コンポーネントが親に見つかりません。");
        }

        AssignAnimationIDs();

        // 初期の接地状態を記録
        if (_playerState != null) _wasGrounded = _playerState.isGrounded;
    }

    private void AssignAnimationIDs()
    {
        _animIDSpeed = Animator.StringToHash("Speed");
        _animIDGrounded = Animator.StringToHash("Grounded");
        _animIDJump = Animator.StringToHash("Jump");
    }

    void Update()
    {
        if (_animator == null || _playerState == null || _rb == null) return;

        UpdateMoveAnimation();
        UpdateGroundingAndJumpAnimation();
    }

    private void UpdateMoveAnimation()
    {
        // 1. 移動速度の即時反映
        // 入力値（MoveInput）と物理速度の大きい方を採用すると、動き出しのレスポンスが良くなります
        float inputSpeed = Mathf.Abs(_playerMove.MoveInput);
        float physicalSpeed = Mathf.Abs(_rb.linearVelocity.x);
        float targetSpeed = Mathf.Max(inputSpeed, physicalSpeed);

        _animator.SetFloat(_animIDSpeed, targetSpeed);

        // 2. 向きの制御 (即時回転)
        if (_playerState.playerDirectionRight)
            transform.localRotation = Quaternion.Euler(0, 90, 0);
        else
            transform.localRotation = Quaternion.Euler(0, -90, 0);
    }

    private void UpdateGroundingAndJumpAnimation()
    {
        bool isGrounded = _playerState.isGrounded;

        // 接地状態を常に同期
        _animator.SetBool(_animIDGrounded, isGrounded);

        // --- 状態が切り替わった瞬間の即時処理 ---

        // A. 地面を離れた瞬間 (ジャンプまたは落下開始)
        if (!isGrounded && _wasGrounded)
        {
            if (_rb.linearVelocity.y > 0.1f)
            {
                // ジャンプ開始
                _animator.SetBool(_animIDJump, true);
                // 必要であればここで直接ステートを叩くことで遷移の遅延をゼロにします
                _animator.CrossFadeInFixedTime("Jump", 0.05f);
            }
        }

        // B. 地面に着いた瞬間
        if (isGrounded && !_wasGrounded)
        {
            _animator.SetBool(_animIDJump, false);
            // 着地した瞬間に即座に Idle/Move 系のブレンドツリーへ戻す
            _animator.CrossFadeInFixedTime("Grounded", 0.05f);
        }

        _wasGrounded = isGrounded;
    }
}