using Npgsql;
using System.ComponentModel.DataAnnotations;
using UnityEngine;
using Utilities;

namespace DatabaseScripts
{

    public enum SignInStatus
    {
        SuccesfulLogin,
        UserDoesntExist,
        WrongPassword
    }
    public enum SignUpStatus
    {
        SuccesfulCreation,
        UserExists,
        InvalidMail
    }


    public class DatabaseConnection : MonoBehaviour
    {
        public bool EditDatabaseSettings = false;
        [ConditionalShowInInspector("EditDatabaseSettings",true)] private static string Server= "lawvrdb.cou5tvcgh993.us-east-2.rds.amazonaws.com";
        [ConditionalShowInInspector("EditDatabaseSettings",true)] private static int Port =5432;
        [ConditionalShowInInspector("EditDatabaseSettings",true)] private static string DatabaseName = "lawvr";
        [ConditionalShowInInspector("EditDatabaseSettings",true)] private static string UserID = "lawvr";
        [ConditionalShowInInspector("EditDatabaseSettings",true)] private static string Password = "lawvr123";

        public static NpgsqlConnection PostgreConnection;
        public static NpgsqlCommand SqlCommand;

        public static void ConnectDatabase()
        {
            PostgreConnection = new NpgsqlConnection($"Server={Server}; Port={Port}; Database={DatabaseName}; User Id={UserID}; Password={Password}; Timeout=45;");
            PostgreConnection.Open();
            SqlCommand = new NpgsqlCommand();
            SqlCommand.Connection = PostgreConnection;
        }
        
        
        public static SignInStatus SignIn(string username, string password)
        {
            SqlCommand.CommandText = "SELECT password FROM users where name='"+username+"' ";
            NpgsqlDataReader passwordTable = SqlCommand.ExecuteReader();            
            if(passwordTable.HasRows)
            {
                passwordTable.Read();
                if (passwordTable[0].Equals(password))
                {
                    return SignInStatus.SuccesfulLogin;
                }
                return SignInStatus.WrongPassword;
            }
            return SignInStatus.UserDoesntExist;

        }

        public static SignUpStatus SignUp(string username, string usermail, string userpassword, bool ismale) 
        {
            SqlCommand.CommandText = "SELECT * FROM users where name='" + username + "' or user_email ='"+usermail+"'";
            NpgsqlDataReader passwordTable = SqlCommand.ExecuteReader();
            if (passwordTable.HasRows)
            {
                return SignUpStatus.UserExists;
            }

            bool validMail;
            try
            {
                var addr = new System.Net.Mail.MailAddress(usermail);
                validMail=(addr.Address == usermail)?true:false ;
            }
            catch
            {
                validMail = false;
            }
            
            if(validMail)
            {
                SqlCommand.CommandText = "Insert into users(user_id,name,password,user_email,ismale) values( 1, '" +
                                         username + "' , '" + userpassword + "' , '" + usermail + "' , " + ismale + " )";
                Debug.Log(SqlCommand.CommandText);
                SqlCommand.ExecuteNonQuery();

                return SignUpStatus.SuccesfulCreation;
            }
            return SignUpStatus.InvalidMail;

        }
        
    }
}
