using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace demo2.Helpers
{
    public class SessionManager
    {
        private static SessionManager _instance;
        public int LoggedInUserId { get; private set; }
        public string LoggedInUsername { get; private set; }

        private SessionManager() { }

        public static SessionManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SessionManager();
                }
                return _instance;
            }
        }

        public void SetUser(int userId, string username)
        {
            LoggedInUserId = userId;
            LoggedInUsername = username;
        }

        public void ClearSession()
        {
            LoggedInUserId = 0;
            LoggedInUsername = null;
        }
    }

}



