using Npgsql;
using UnityEngine;
using Utilities;

namespace DatabaseScripts
{
    public class DatabaseConnection : MonoBehaviour
    {
        public bool EditDatabaseSettings = false;
        [ConditionalShowInInspector("EditDatabaseSettings",true)] public string Server= "lawvrdatabase.cou5tvcgh993.us-east-2.rds.amazonaws.com";
        [ConditionalShowInInspector("EditDatabaseSettings",true)] public int Port=5432;
        [ConditionalShowInInspector("EditDatabaseSettings",true)] public string DatabaseName= "postgres";
        [ConditionalShowInInspector("EditDatabaseSettings",true)] public string UserID= "postgres";
        [ConditionalShowInInspector("EditDatabaseSettings",true)] public string Password= "postgres";
        // Start is called before the first frame update
        void Start()
        {
            
            NpgsqlConnection baglanti = new NpgsqlConnection($"Server={Server}; Port={Port}; Database={DatabaseName}; User Id={UserID}; Password={Password}; Timeout=45;");
            baglanti.Open();
            var cmd = new NpgsqlCommand();
            cmd.Connection = baglanti;

            /*************************************************************************************
            
            Non-query executions like Create,Alter,Drop,Insert,Update,Delete vs works with ExecuteNonQuery.
            
            *************************************************************************************/
            /*string sorgu = "insert into users(user_id,name,password) values(1234,'kaan','12345') ";
            
            cmd.Connection = baglanti;
            cmd.CommandText = sorgu;
            cmd.ExecuteNonQuery();*/


            /*************************************************************************************

            Query executions with return like select will work with ExecuteReader .

            *************************************************************************************/
            cmd.CommandText = "SELECT * FROM users";

            NpgsqlDataReader tables = cmd.ExecuteReader();

            //print rows
            while(tables.Read())
            {
                string rowResult="";
                for(int i = 0; i < tables.FieldCount; i++)
                {
                    rowResult += tables[i] + "\t";
                }
                Debug.Log(rowResult);
            }
                
            
        }
    }
}
