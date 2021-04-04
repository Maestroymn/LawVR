using Managers;
using Npgsql;
using Photon.Pun;
using System;
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
                    GameManager.GameSettings.NickName = username;
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
                SqlCommand.CommandText = "Insert into users(user_id,name,password,user_email,isfemale,is_online) values( "+
                "'"+PhotonNetwork.LocalPlayer.UserId +"' , '" +username + "' , '" + userpassword + "' , '" + usermail + "' , " + ismale + ", true )";
                GameManager.GameSettings.NickName = username;
                SqlCommand.ExecuteNonQuery();

                return SignUpStatus.SuccesfulCreation;
            }
            return SignUpStatus.InvalidMail;

        }

        public static string RetrieveFriendList(string username)
        {
            SqlCommand.CommandText = "Select friend_list from users where name ='" + username + "'";
            NpgsqlDataReader FriendList = SqlCommand.ExecuteReader();
            FriendList.Read();
            return FriendList[0].ToString();

        }

        public static bool[] RetrieveFriendStatus(string[] friendlist)
        {

            bool[] FriendsOnlineList = new bool[friendlist.Length];
            for (var i = 0; i < friendlist.Length; i++)
            {
                
                SqlCommand.CommandText = "Select is_online from users where name ='" + friendlist[i] + "'";
                Debug.Log(SqlCommand.CommandText);
                NpgsqlDataReader isonline = SqlCommand.ExecuteReader();
                isonline.Read();
                
                FriendsOnlineList[i] = isonline[0].ToString() =="True"?true:false;
            }
            return FriendsOnlineList;
        }

        public static void RemoveFriend(string friendName)
        {

            SqlCommand.CommandText = "Select friend_list from users where name ='" + GameManager.GameSettings.NickName + "'";
            NpgsqlDataReader FriendList = SqlCommand.ExecuteReader();
            FriendList.Read();
            string FriendListText  = FriendList[0].ToString();
            FriendListText = FriendListText.Substring(1, FriendListText.Length - 2);

            string[] friends = FriendListText.Split(',');

            string NewFriendList = "{";
            for(var i  = 0; i< friends.Length; i++)
            {
                Debug.Log(i + ": "+friends[i]);
                if(!friends[i].Equals(friendName))
                    NewFriendList += friends[i]+ ",";
            }

            NewFriendList = NewFriendList.Substring(0, NewFriendList.Length - 1) + "}";


            SqlCommand.CommandText = "Update users set friend_list ='"+NewFriendList+"' where name ='" + GameManager.GameSettings.NickName + "'";
            Debug.Log(SqlCommand.CommandText);

            SqlCommand.ExecuteNonQuery();





        }

        public static void AddFriend(string NewFriendName)
        {

            SqlCommand.CommandText = "select count(name) from users where name ='" + NewFriendName.Substring(0, NewFriendName.Length - 1) + "'";
            NpgsqlDataReader IsUserExist = SqlCommand.ExecuteReader();
            Debug.Log(SqlCommand.CommandText);
            IsUserExist.Read();
            Debug.Log("user count " + IsUserExist[0].ToString());

            if(Int64.Parse(IsUserExist[0].ToString()) == 1 )
            {
                SqlCommand.CommandText = "select waiting_invitations from users where name = '" + NewFriendName.Substring(0, NewFriendName.Length - 1) + "'";
                Debug.Log(SqlCommand.CommandText);
                NpgsqlDataReader WaitingInvitations = SqlCommand.ExecuteReader();
                WaitingInvitations.Read();

                    if(WaitingInvitations[0].ToString().Trim()!="")
                    {
                        string ExistingInvitations = WaitingInvitations[0].ToString();
                        ExistingInvitations = ExistingInvitations.Substring(0, ExistingInvitations.Length - 1) + "," + GameManager.GameSettings.NickName + "}";
                        SqlCommand.CommandText = "update users set waiting_invitations = '" + ExistingInvitations + "' where name ='" + NewFriendName.Substring(0, NewFriendName.Length - 1) + "' ";
                        Debug.Log(SqlCommand.CommandText);
                        SqlCommand.ExecuteNonQuery();
                    }
                    else
                    {
                        SqlCommand.CommandText = "update users set waiting_invitations = '{" + GameManager.GameSettings.NickName + "}' where name ='" + NewFriendName.Substring(0, NewFriendName.Length - 1) + "' ";
                        SqlCommand.ExecuteNonQuery();
                        Debug.Log(SqlCommand.CommandText);
                    }

            }
            else
            {
                Debug.Log("user doesn't exist");
            }
                
                
                


        }

        public static void RetrieveSpecificUser()
        {



        }
    }
}
