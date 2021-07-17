using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Usuario : MonoBehaviour
{
    public string username;
    public string email;
    public string userId;
    public string rolId;
    public Usuario()
    {
    }
    public Usuario(string username, string email, string userId, string rolId)
    {
        this.username = username;
        this.email = email;
        this.userId = userId;
        this.rolId = rolId;
    }

}
