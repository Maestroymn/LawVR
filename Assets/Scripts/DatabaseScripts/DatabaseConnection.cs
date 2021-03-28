using Npgsql;
using UnityEngine;
using Utilities;

namespace DatabaseScripts
{
    public class DatabaseConnection : MonoBehaviour
    {
        public bool EditDatabaseSettings = false;
        [ConditionalShowInInspector("EditDatabaseSettings",true)] public string Server="127.0.0.1";
        [ConditionalShowInInspector("EditDatabaseSettings",true)] public int Port=5432;
        [ConditionalShowInInspector("EditDatabaseSettings",true)] public string DatabaseName="test";
        [ConditionalShowInInspector("EditDatabaseSettings",true)] public string UserID="postgres";
        [ConditionalShowInInspector("EditDatabaseSettings",true)] public string Password="postgres";
        // Start is called before the first frame update
        void Start()
        {
            NpgsqlConnection baglanti = new NpgsqlConnection($"Server={Server}; Port={Port}; Database={DatabaseName}; User Id={UserID}; Password={Password}; Timeout=15;");
            baglanti.Open();
            Debug.Log("selam");
            string sorgu = "insert into newtable(username) values ('at123')";

            var cmd = new NpgsqlCommand() ;

            cmd.Connection = baglanti;
            cmd.CommandText = sorgu;
            cmd.ExecuteNonQuery();
        }
    }
}
