using UnityEngine;

public class DemonMeleeAttackState : IState
{
    private DemonAI demon;

    public DemonMeleeAttackState(DemonAI demon)
    {
        this.demon = demon;
    }

    public void Enter()
    {
        demon.animator?.SetBool("IsAttacking", true);

        // Gợi ý: Có thể trigger sát thương tại đây hoặc bằng Animation Event
    }

    public void Update()
    {
        // Không làm gì, đợi animation tự quyết định khi nào quay lại chase
        // Bạn có thể thêm kiểm tra AnimatorState ở đây nếu muốn tự chuyển
    }

    public void Exit()
    {
        demon.animator?.SetBool("IsAttacking", false);
    }
}
