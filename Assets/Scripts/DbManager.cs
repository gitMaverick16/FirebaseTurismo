using System.Collections;
using System.Collections.Generic;
using Firebase;
using Firebase.Database;
using UnityEngine;

public class DbManager : MonoBehaviour
{
    DatabaseReference reference;
    // Start is called before the first frame update
    void Start()
    {
        reference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*

    public void guardarUsuario(string username, string email, string usuarioId, string rolId)
    {
        User usuario = new User(username, email, usuarioId, rolId);
        string json = JsonUtility.ToJson(usuario);
        reference.Child("usuarios").Child(usuarioId).SetRawJsonValueAsync(json);
    }
    */
}
