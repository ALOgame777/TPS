
public class StatusBase
{
    public float maxHp;
    public float speed;


    public float currentHP;


    public void Initialize(float maxHealth, float spd)
    {
        maxHp = maxHealth;
        speed = spd;
        currentHP = maxHp;
    }

    
}
