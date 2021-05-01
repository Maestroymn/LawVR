using Managers;
using Npgsql;
using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;
using Utilities;
using LitJson;
using Data;
using System.IO;
using UI;


namespace DatabaseScripts
{

    public static class JsonHelper
    {
        public static T[] FromJson<T>(string json)
        {
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
            return wrapper.Items;
        }

        public static string ToJson<T>(T[] array)
        {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.Items = array;
            return JsonUtility.ToJson(wrapper);
        }

        public static string ToJson<T>(T[] array, bool prettyPrint)
        {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.Items = array;
            return JsonUtility.ToJson(wrapper, prettyPrint);
        }

        [Serializable]
        private class Wrapper<T>
        {
            public T[] Items;
        }
    }

    [Serializable]
    public class ParticipantUser
    {
        public string name;
        public string role;
    }
    public enum SignInStatus
    {
        AlreadyLoggedIn,
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
    public enum SendInvitationStatus
    {
        Sent,
        UserDoesntExist,
        AlreadyExistingInvitation,
        AlreadyFriend
    }

    public struct Friend
    {
        public string UserID;
        public string Name;
        public bool IsOnline;

        public Friend(string userID, string name, bool isOnline)
        {
            UserID = userID;
            Name = name;
            IsOnline = isOnline;
        }
    }

    public class DatabaseConnection : MonoBehaviour
    {
        public bool EditDatabaseSettings = false;

        [ConditionalShowInInspector("EditDatabaseSettings", true)]
        private static string Server = "lawvrdb.cou5tvcgh993.us-east-2.rds.amazonaws.com";

        [ConditionalShowInInspector("EditDatabaseSettings", true)]
        private static int Port = 5432;

        [ConditionalShowInInspector("EditDatabaseSettings", true)]
        private static string DatabaseName = "lawvr";

        [ConditionalShowInInspector("EditDatabaseSettings", true)]
        private static string UserID = "lawvr";

        [ConditionalShowInInspector("EditDatabaseSettings", true)]
        private static string Password = "lawvr123";

        public static NpgsqlConnection PostgreConnection;
        public static NpgsqlCommand SqlCommand;

        public static void ConnectDatabase()
        {
            PostgreConnection =
                new NpgsqlConnection(
                    $"Server={Server}; Port={Port}; Database={DatabaseName}; User Id={UserID}; Password={Password}; Timeout=45;");
            PostgreConnection.Open();
            SqlCommand = new NpgsqlCommand();
            SqlCommand.Connection = PostgreConnection;
        }

        public static SignInStatus SignIn(string username, string password)
        {
            SqlCommand.CommandText = "SELECT password FROM users where name='" + username + "' ";
            NpgsqlDataReader passwordTable = SqlCommand.ExecuteReader();
            if (passwordTable.HasRows)
            {
                
                passwordTable.Read();
                if (passwordTable[0].Equals(password))
                {
                    SqlCommand.CommandText = "SELECT is_online FROM users where name='" + username + "' ";
                    NpgsqlDataReader alreadyOnlineCheck = SqlCommand.ExecuteReader();
                    alreadyOnlineCheck.Read();
                    if (alreadyOnlineCheck[0].ToString().ToLower().Equals("true"))
                    {
                        return SignInStatus.AlreadyLoggedIn;
                    }
                    GameManager.GameSettings.NickName = username;
                    return SignInStatus.SuccesfulLogin;
                }

                return SignInStatus.WrongPassword;
            }

            return SignInStatus.UserDoesntExist;

        }

        public static SignUpStatus SignUp(string username, string usermail, string userpassword, bool ismale)
        {
            SqlCommand.CommandText =
                "SELECT * FROM users where name='" + username + "' or user_email ='" + usermail + "'";
            NpgsqlDataReader passwordTable = SqlCommand.ExecuteReader();
            if (passwordTable.HasRows)
            {
                return SignUpStatus.UserExists;
            }

            bool validMail;
            try
            {
                var addr = new System.Net.Mail.MailAddress(usermail);
                validMail = (addr.Address == usermail) ? true : false;
            }
            catch
            {
                validMail = false;
            }

            if (validMail)
            {
                SqlCommand.CommandText =
                    "Insert into users(user_id,name,password,user_email,is_female,is_online) values( " +
                    "'" + PhotonNetwork.LocalPlayer.UserId + "' , '" + username + "' , '" + userpassword + "' , '" +
                    usermail + "' , " + ismale + ", true )";
                GameManager.GameSettings.NickName = username;
                SqlCommand.ExecuteNonQuery();

                return SignUpStatus.SuccesfulCreation;
            }

            return SignUpStatus.InvalidMail;

        }

        public static List<Friend> RetrieveFriendList(string username)
        {
            List<Friend> Friends = new List<Friend>();
            SqlCommand.CommandText = "select f.friend_user_name , " +
                                     "(select user_id from users where name = f.friend_user_name) , " +
                                     "(select is_online from users where name = f.friend_user_name) " +
                                     "from users as u " +
                                     "left join friendship_list as f on u.name = f.user_name " +
                                     "where f.friend_user_name is not null and u.name = '" + username + "'";
            
            NpgsqlDataReader FriendList = SqlCommand.ExecuteReader();
            while (FriendList.Read())
            {
                Friends.Add(new Friend(FriendList[1].ToString(),FriendList[0].ToString() ,FriendList[2].ToString().ToLower() == "true"));
            }

            return Friends;
        }

        public static bool RemoveFriend(string FriendName)
        {

            try
            {
                SqlCommand.CommandText = "delete from friendship_list where user_name = '" +
                                         GameManager.GameSettings.NickName + "'" +
                                         "and friend_user_name = '" + FriendName + "'";
                SqlCommand.ExecuteNonQuery();


                SqlCommand.CommandText = "delete from friendship_list where user_name = '" + FriendName + "'" +
                                         "and friend_user_name = '" + GameManager.GameSettings.NickName + "'";
                SqlCommand.ExecuteNonQuery();
                return true;
            }
            catch (NpgsqlException e)
            {
                Debug.Log("Recejtion failed: " + e.ToString());
                return false;
            }
        }

        public static Dictionary<string, bool> ListFriendshipRequests(string username)
        {

            SqlCommand.CommandText =
                "select i.sender_user_name, (select is_online from users where name=i.sender_user_name) " +
                "from users as u left join invitation_list as i " +
                "on u.name = i.user_name " +
                "where i.sender_user_name is not null and i.user_name = '" + username + "'";
            Dictionary<string, bool> WaitingInvitations = new Dictionary<string, bool>();

            NpgsqlDataReader Invitations = SqlCommand.ExecuteReader();

            while (Invitations.Read())
            {
                WaitingInvitations.Add(Invitations[0].ToString(),
                    Invitations[1].ToString().ToLower() == "true" ? true : false);
            }

            return WaitingInvitations;
        }

        public static SendInvitationStatus SendFriendshipRequest(string NewFriendName)
        {
            if (NewFriendName != GameManager.GameSettings.NickName);
            {
                NewFriendName = NewFriendName.Substring(0, NewFriendName.Length - 1);


                bool UserExists = CheckUserExistence(NewFriendName);
                if (UserExists)
                {
                    SqlCommand.CommandText = $"select count(user_name) from (select user_name from friendship_list " +
                                             "where user_name = '" + GameManager.GameSettings.NickName +
                                             "' and friend_user_name = '" + NewFriendName + "') as friend_check";
                    NpgsqlDataReader friendCheck = SqlCommand.ExecuteReader();
                    friendCheck.Read();
                    if (friendCheck[0].ToString().Equals("0"))
                    {
                        SqlCommand.CommandText =
                            "insert into invitation_list(user_name, sender_user_name, invitation_send_date) " +
                            "values ('" + NewFriendName + "', '" + GameManager.GameSettings.NickName + "', now())";
                        try
                        {
                            SqlCommand.ExecuteNonQuery();
                            return SendInvitationStatus.Sent;
                        }
                        catch (NpgsqlException e)
                        {
                            return SendInvitationStatus.AlreadyExistingInvitation;
                        }
                    }
                    else
                    {
                        return SendInvitationStatus.AlreadyFriend;
                    }
                }
                else
                {
                    return SendInvitationStatus.UserDoesntExist;
                }
            }
        }

        public static bool AcceptFriendshipRequest(string RequestFriendName)
        {

            try
            {
                SqlCommand.CommandText = "delete from invitation_list where user_name ='" + GameManager.GameSettings.NickName + "'" +
                " and sender_user_name ='" + RequestFriendName + "'";

                SqlCommand.ExecuteNonQuery();

                SqlCommand.CommandText = "insert into friendship_list(user_name, friend_user_name, friendship_start_date) " +
                "values('" + GameManager.GameSettings.NickName + "', '" + RequestFriendName + "', now())";

                SqlCommand.ExecuteNonQuery();

                SqlCommand.CommandText = "insert into friendship_list(user_name, friend_user_name, friendship_start_date) " +
                "values('" + RequestFriendName  + "', '" + GameManager.GameSettings.NickName + "', now())";

                SqlCommand.ExecuteNonQuery();
                return true;
            }
            catch (NpgsqlException e)
            {
                Debug.Log("Recejtion failed: " + e.ToString());
                return false;
            }
        }

        public static bool RejectFriendshipRequest(string RequestFriendName)
        {
            SqlCommand.CommandText = "delete from invitation_list where user_name ='" + GameManager.GameSettings.NickName + "'" +
            " and sender_user_name ='" + RequestFriendName + "'";

            try
            {
                SqlCommand.ExecuteNonQuery();
                return true;
            }
            catch(NpgsqlException e)
            {
                Debug.Log("Recejtion failed: "+ e.ToString());
                return false;
            }
        }

        private static bool CheckUserExistence(string NewFriendName)
        {
            SqlCommand.CommandText = "select count(name) from users where name ='" + NewFriendName + "'";
            NpgsqlDataReader UserExistControl = SqlCommand.ExecuteReader();
            UserExistControl.Read();
            Debug.Log("user count " + UserExistControl[0].ToString());
            if (Int64.Parse(UserExistControl[0].ToString()) == 1)
                return true;
            else
                return false;
        }

        public static void SetUserOnline(string username)
        {
            SqlCommand.CommandText = "update users set is_online = true where name='" + username + "'";
            SqlCommand.ExecuteNonQuery();

        }

        public static void SetUserOffline(string username)
        {
            
            SqlCommand.CommandText = "update users set is_online = false where name='" + username + "'";
            SqlCommand.ExecuteNonQuery();
        }
    
        public static void UploadSpeech(int SessionID , string SpeakerID, string SpeakerRole, string Speech, string StartTime, string SpeechDuration)
        {
            SqlCommand.CommandText = "insert into speech_log(session_id,speaker_id,speaker_role,speech,start_time,speech_duration) " +
               "values(" + SessionID + ", '" + SpeakerID + "', '"+ SpeakerRole + "', '"+Speech+"', '"+ StartTime+"' , "+ SpeechDuration.Replace(",",".")+" )";
            Debug.Log(SqlCommand.CommandText);
            SqlCommand.ExecuteNonQuery();

        }
        
        public static int CreateSessionLog( string CaseID, string StartTime , string SimulationType)
        {
            SqlCommand.CommandText = "insert into court_session(case_id, start_time, simulation_type) " +
            "values('" + CaseID + "', '" + StartTime + "' , '" + SimulationType + "' ) returning session_id";
            Debug.Log(SqlCommand.CommandText);
            NpgsqlDataReader SessionID = SqlCommand.ExecuteReader();
            SessionID.Read();
            return (int)SessionID[0];
        }
        
        public static void UpdateSessionLog(int SessionID , string EndTime , string Feedback)
        {
            SqlCommand.CommandText = "update court_session set end_time = '" + EndTime + 
            "' , feedback = '" + Feedback +"' where session_id = " +SessionID;
            Debug.Log(SqlCommand.CommandText);
            SqlCommand.ExecuteNonQuery();
        }

        public static List<SessionHistory> GetSessionHistories(string UserName)
        {
            
            SqlCommand.CommandText = "select session_ids from users where name ='"+UserName+"' ";
            NpgsqlDataReader SessionIDs = SqlCommand.ExecuteReader();
            try 
            {
                SessionIDs.Read();
                string[] sessionID = SessionIDs[0].ToString().Split('&');
                List<SessionHistory> UserHistory = new List<SessionHistory>();
                foreach (string id in sessionID)
                {
                    
                    SqlCommand.CommandText = "select * from court_session where session_id = " + id;
                    NpgsqlDataReader SessionLogs = SqlCommand.ExecuteReader();
                    SessionLogs.Read();
                    SessionHistory newHistory = new SessionHistory();
                    newHistory.CaseID = Int32.Parse(SessionLogs[0].ToString());
                    newHistory.StartTime = SessionLogs[1].ToString();
                    newHistory.EndTime = SessionLogs[2].ToString();
                    newHistory.Feedback = SessionLogs[3].ToString();
                    newHistory.SessionID = Int32.Parse(SessionLogs[4].ToString());
                    newHistory.SimulationType = SessionLogs[5].ToString();
                    newHistory.SpeechText = GetSessionSpeechLog(id);
                    UserHistory.Add(newHistory);
                }

                foreach( var u in UserHistory)
                {
                    Debug.Log(u.CaseID + " " + u.EndTime + " " + u.StartTime + " " + u.SimulationType + " " + u.SessionID + " " + u.Feedback + " " + u.SpeechText);

                }

                return UserHistory;
            } 
            catch (Exception e) 
            { 
                Debug.Log(e.Message + " NO HISTORY");
            }

            return null;

        }

        public static string GetSessionSpeechLog(string SessionID)
        {
            SqlCommand.CommandText = "Select speaker_id,speaker_role,speech from speech_log where session_id = " + SessionID + " order by start_time asc";
            NpgsqlDataReader speeches = SqlCommand.ExecuteReader();
            string speech ="";
            while (speeches.Read())
            {
                speech += speeches[0].ToString() + "[" + speeches[1].ToString() + "]:" + speeches[2].ToString() + "\n";
            }
            return speech;
        }


        public static void UpdateUserSessionID(string username , int session_id)
        {
            SqlCommand.CommandText = "select session_ids from users where name = '" + username+"'";
            NpgsqlDataReader sessions = SqlCommand.ExecuteReader();
            bool isEmpty = true;

            while(sessions.Read())
            {
                string existingSessions = sessions[0].ToString();
                if (existingSessions.Length == 0)
                    break;
                
                isEmpty = false;
                existingSessions += "&" + session_id;
                SqlCommand.CommandText = "update users set session_ids = '" + existingSessions + "'where name = '" + username + "'";
                SqlCommand.ExecuteNonQuery();
            }
            if(isEmpty)
            {
                SqlCommand.CommandText = "update users set session_ids = '" + session_id + "'where name = '" + username + "'";
                SqlCommand.ExecuteNonQuery();
            }

        }
        
        public static List<CourtCase> GetCourtCases()
        {
            SqlCommand.CommandText ="select * from court_case";
            NpgsqlDataReader cases = SqlCommand.ExecuteReader();
            List<CourtCase> ListedCases = new List<CourtCase>();
            while(cases.Read())
            {
                CourtCase temp = new CourtCase();
                temp.CaseID = Int32.Parse(cases[0].ToString());
                temp.CaseName = cases[1].ToString();
                temp.CaseText = cases[2].ToString();
                temp.CaseDate = cases[3].ToString();
                ListedCases.Add(temp);
            }

            return ListedCases;


        }
        
        public static string GetEmail()
        {
            SqlCommand.CommandText = "select user_email from users where name ='"+GameManager.GameSettings.NickName+"'";
            NpgsqlDataReader UserEmail =   SqlCommand.ExecuteReader();
            UserEmail.Read();
            return UserEmail[0].ToString();
        }
        
        public static string GetUserID()
        {
            SqlCommand.CommandText = "select user_id from users where name ='"+GameManager.GameSettings.NickName+"'";
            NpgsqlDataReader UserID =   SqlCommand.ExecuteReader();
            UserID.Read();
            return UserID[0].ToString();
        }
        
        public static bool GetIsFemale()
        {
            SqlCommand.CommandText = "select is_female from users where name ='" + GameManager.GameSettings.NickName + "'";
            NpgsqlDataReader UserIsFemale = SqlCommand.ExecuteReader();
            UserIsFemale.Read();
            return UserIsFemale[0].ToString().ToLower() =="true"?true:false;
        }

        public static void SetEmail(string NewEmail)
        {
            SqlCommand.CommandText = "update users set user_email ='" + NewEmail + "' where name ='" + GameManager.GameSettings.NickName + "'";
            SqlCommand.ExecuteNonQuery();
        }
        
        public static void SetIsFemale(bool IsFemale)
        {
            SqlCommand.CommandText = "update users set is_female =" + IsFemale + " where name ='" + GameManager.GameSettings.NickName + "'";
            SqlCommand.ExecuteNonQuery();
        }

        public static void SetPassword(string NewPassword)
        {
            SqlCommand.CommandText = "update users set password ='" + NewPassword + "' where name ='" + GameManager.GameSettings.NickName + "'";
            SqlCommand.ExecuteNonQuery();
        }

        public static void SetName(string NewName)
        {
            SqlCommand.CommandText = "update users set name ='" + NewName + "' where name ='" + GameManager.GameSettings.NickName + "'";
            SqlCommand.ExecuteNonQuery();
        }

        public static string GetAvatarPreferences()
        {
            SqlCommand.CommandText = "select preference from users where name ='" + GameManager.GameSettings.NickName + "'";
            NpgsqlDataReader UserPreference = SqlCommand.ExecuteReader();
            UserPreference.Read();
            return UserPreference[0].ToString();
        }

        public static void SaveAvatarPreference(string json)
        {
            SqlCommand.CommandText = "update users set preference ='" + json + "' where name ='" + GameManager.GameSettings.NickName + "'";
            SqlCommand.ExecuteNonQuery();
        }
    }
}
