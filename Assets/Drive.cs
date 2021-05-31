using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drive : MonoBehaviour {
    // declarações
	float speed = 20.0F;
    float rotationSpeed = 120.0F;
    public GameObject bulletPrefab;
    public Transform bulletSpawn;

    void Update() { // iniciando o update
        float translation = Input.GetAxis("Vertical") * speed; // velocidade 
        float rotation = Input.GetAxis("Horizontal") * rotationSpeed; // rotação para outro lado, no caso horizontal
        translation *= Time.deltaTime; // tempo de reação para translação
        rotation *= Time.deltaTime; // tempo de reação para rotação
        transform.Translate(0, 0, translation); // posição da translação
        transform.Rotate(0, rotation, 0); // posição da rotação

        if(Input.GetKeyDown("space")) // se apertar espaçõ
        {
            GameObject bullet = GameObject.Instantiate(bulletPrefab, bulletSpawn.transform.position, bulletSpawn.transform.rotation); // instancia projeteis de bala
            bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward*2000); // força e rigidBody das balas
        }
    }
}
