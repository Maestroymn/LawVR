using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Npgsql;
using System.Data;

public class DatabaseConnection : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
        NpgsqlConnection baglanti = new NpgsqlConnection("Server=127.0.0.1; Port=5432; Database=test; User Id=postgres; Password=postgres; Timeout=15;");
        baglanti.Open();
        Debug.Log("selam");
        string sorgu = "insert into newtable(username) values ('at123')";

        var cmd = new NpgsqlCommand() ;

        cmd.Connection = baglanti;
        cmd.CommandText = sorgu;
        cmd.ExecuteNonQuery();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
