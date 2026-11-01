using UnityEngine;

public class MechVeinPickup : MonoBehaviour
{
    public Transform player;
    public float healRate;
    public float energyRate;
    public float attractSpeed;
    public float attractDistance;

    void Start()
    {
        player = FindFirstObjectByType<MechControl>().transform;
    }

    // void Update()
    // {
    //     if(player == null)
    //         return;

    //     transform.position = Vector3.MoveTowards(transform.position, player.position, attractSpeed * Time.deltaTime);
    // }
    // void Update()
    // {
    //     if(player == null)
    //         return;

    //     float distance = Vector3.Distance(transform.position, player.position);

    //     if(distance < 5f)
    //     {
    //         transform.position = Vector3.MoveTowards(transform.position, player.position, attractSpeed * Time.deltaTime);
    //     }
    // }
    void Update()
    {
        if(player == null)
            return;

        float distance = Vector3.Distance(transform.position, player.position);

        if(distance < attractDistance)
        {
            transform.position = Vector3.MoveTowards(transform.position, player.position, attractSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        MechControl player = col.GetComponent<MechControl>();

        if(player == null)
            return;

        player.currentHP += player.maxHP * healRate;

        player.currentHP = Mathf.Min(player.currentHP, player.maxHP);

        player.boostEnergy += player.boostEnergyMax * energyRate;

        player.boostEnergy = Mathf.Min(player.boostEnergy, player.boostEnergyMax);

        Destroy(gameObject);
    }
}