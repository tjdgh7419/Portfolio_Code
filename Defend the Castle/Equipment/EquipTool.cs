using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class EquipTool : Equip
{
    public float attackRate;
    public float attackDistance;

    [Header("Combat")]
    public bool doesDealDamage;
    public int damage;

    private Animator animator;
    private Camera cam;
    private bool attacking;
    private bool isHit;

    public ParticleSystem swordEffect;
    private readonly int AnimAttack = Animator.StringToHash("Attack");

    private int count = 0;

    protected virtual void Awake()
    {
        cam = Camera.main;
        animator = GetComponent<Animator>();
    }

	private void Start()
	{
        swordEffect.Stop();
	}

	public override void OnAttackInput()
	{
        if (!attacking && !UIManager.Instance.IsOnUI)
        {
            attacking = true;
            animator.SetTrigger(AnimAttack);
            Invoke(nameof(AttackDelay), attackRate);
            SkillOn();
        }
	}

    private void SkillOn()
    {
        swordEffect.Play();
        if (gameObject.name == "Equip_MagicSword(Clone)")
        {
            GameObject go = GraphicManager.Instance.Effacts.transform.GetChild(count).gameObject;
            Vector3 dir = GraphicManager.Instance.SpawnPoint.transform.position - new Vector3(GameManager.Instance.player.transform.position.x, GameManager.Instance.player.transform.position.y + 1.5f, GameManager.Instance.player.transform.position.z);

            go.transform.position = GraphicManager.Instance.SpawnPoint.transform.position;
            go.GetComponent<Electro>().UseSkill(dir);
            count++;
            if (count >= GraphicManager.Instance.Effacts.transform.childCount)
            {
                count = 0;
            }
        }
    }

    public void OnHit()
    {
		Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        isHit = Physics.Raycast(ray, out RaycastHit hit, attackDistance);
        if (isHit)
        {
			if (hit.collider.TryGetComponent(out Monster monster))
            {
				if (monster != null)
                {
                    monster.TakeDamage(damage);
                    GameManager.Instance.garphicManager.MonsterHit(hit.point);
                }
            }
        }
    }

    private void AttackDelay()
    {
        attacking = false;
    }
}
