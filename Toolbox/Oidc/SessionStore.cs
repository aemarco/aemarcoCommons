using System;
using System.Threading.Tasks;

namespace aemarcoCommons.Toolbox.Oidc
{


    /// <summary>
    /// external party holding the access and refresh token
    /// </summary>
    public interface ISessionStore
    {
        event EventHandler AccessTokenChanged;

        Task<Session> GetSession();
        Task SetSession(Session session);
        Task EndSession();
    }



    /// <summary>
    /// default in memory session store
    /// </summary>
    public class SessionStore : ISessionStore
    {
        public event EventHandler AccessTokenChanged;
        protected void OnAccessTokenChanged()
        {
            AccessTokenChanged?.Invoke(this, EventArgs.Empty);
        }

        private Session _session;
        public virtual Task<Session> GetSession()
        {
            return Task.FromResult(_session);
        }

        public virtual Task SetSession(Session session)
        {
            _session = session;
            OnAccessTokenChanged();
            return Task.CompletedTask;
        }

        public virtual Task EndSession()
        {
            _session = null;
            return Task.CompletedTask;
        }
    }

    public class Session
    {
        public string IdToken { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }

}