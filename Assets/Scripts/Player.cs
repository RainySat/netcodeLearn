using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Player : NetworkBehaviour
{
    Rigidbody rb;
    [SerializeField]
    float moveSpeed = 10f;
    
    [SerializeField]
    float turnSpeed = 100f;

    public Text nameLabel;
    
    private NetworkVariable<Vector3> networkPlayerPos = new NetworkVariable<Vector3>(Vector3.zero);
    private NetworkVariable<Quaternion> networkPlayerRot = new NetworkVariable<Quaternion>(Quaternion.identity);
    private NetworkVariable<int> clientId = new NetworkVariable<int>(0);
    
    private Color[] playerColors = new Color[] { Color.red, Color.green, Color.blue ,Color.yellow ,Color.cyan , Color.magenta };
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("Player Start");   //test
        rb = GetComponent<Rigidbody>();

        if (IsClient && IsOwner)
        {
            transform.position = new Vector3(Random.Range(-5,5), 0.5f, Random.Range(-5,5));
        }
        nameLabel.text = clientId.Value.ToString();
        
        Renderer renderer = GetComponent<Renderer>();
        renderer.material.color = playerColors[clientId.Value % playerColors.Length];
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            clientId.Value = (int)OwnerClientId;
            Debug.Log("Player OnNetworkSpawn id :" + OwnerClientId);
        }
        
        //nameLabel.text = OwnerClientId + "";
    }

    // Update is called once per frame
    void Update()
    {
        if (IsClient && IsOwner)
        {
            float v = Input.GetAxis("Vertical");
            float h = Input.GetAxis("Horizontal");

            Vector3 pos = GetTargetPos(v);
            Quaternion rot = GetTargetRot(h);
        
            UpdatePosAndRotServerRpc(pos, rot);
            
            Move(pos);
            Turn(rot);
        }
        else
        {
            Move(networkPlayerPos.Value);
            Turn(networkPlayerRot.Value);
        }
        
    }

    [ServerRpc]
    public void UpdatePosAndRotServerRpc(Vector3 pos, Quaternion rot)
    {
        networkPlayerPos.Value = pos;
        networkPlayerRot.Value = rot;
    }

    private void Turn(Quaternion rot)
    {
        rb.MoveRotation(rot);
    }

    private Quaternion GetTargetRot(float f)
    {
        Quaternion delta = Quaternion.Euler(0, f * turnSpeed * Time.deltaTime, 0);
        Quaternion rot = rb.rotation * delta;
        return rot;
    }

    private void Move(Vector3 pos)
    {
        rb.MovePosition(pos);
    }

    private Vector3 GetTargetPos(float f)
    {
        Vector3 delta = transform.forward * f * moveSpeed * Time.deltaTime;
        Vector3 pos = rb.position + delta;
        return pos;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Coin"))
        {
            Debug.Log("Coin");
            other.gameObject.SetActive(false);
        }
    }
}
