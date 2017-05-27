using SubwayTalent.Contracts.Interfaces;
using SubwayTalent.Core;
using SubwayTalent.Core.Interfaces;
using SubwayTalent.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace SubwayTalent.PaymentJob
{
    public class SubwayContext
    {
        private static SubwayContext _current = null;
        private static object _contextLock = new object();

        #region "private repo objects"
        private IUserAccount _userRepo;
        private IEvent _eventRepo;
        private ILookUpValues _lookupValuesRepo;
        private ITokenRepo _tokenRepo;
        private ISettings _settingsRepo;
        private IPaymentRepo _paymentRepo;
        #endregion    
       
        private ILogger _logger;
        private Dictionary<string, string> _subwaySettings;       


        public static SubwayContext Current
        {
            get
            {
                lock (_contextLock)
                {
                    if (_current == null)
                        _current = new SubwayContext();
                }
                return _current;
            }
        }

        internal Dictionary<string, string> SubwaySettings
        {
            get 
            {
                if (_subwaySettings == null)
                    _subwaySettings = SubwayContext.Current.SettingsRepo.GetSettings();
                return _subwaySettings;
            }
        }

        #region Tool(s)

        internal ILogger Logger
        {
            get
            {
                if (_logger == null)
                    _logger = ProviderFactory.CreateProvider<ILogger>();
                return _logger;
            }
        }

        #endregion

        #region repositories
        internal IUserAccount UserRepo 
        { 
            get 
            {
                if (_userRepo == null)
                    _userRepo = ProviderFactory.CreateProvider<IUserAccount>();

                return _userRepo;
            } 
        }

        internal IEvent EventRepo
        {
            get
            {
                if (_eventRepo == null)
                    _eventRepo = ProviderFactory.CreateProvider<IEvent>();

                return _eventRepo;
            }
        }

        internal ILookUpValues LookUpValuesRepo
        {
            get
            {
                if (_lookupValuesRepo == null)
                    _lookupValuesRepo = ProviderFactory.CreateProvider<ILookUpValues>();

                return _lookupValuesRepo;
            }
        }

        internal ITokenRepo TokenRepo
        {
            get
            {
                if (_tokenRepo == null)
                    _tokenRepo = ProviderFactory.CreateProvider<ITokenRepo>();

                return _tokenRepo;
            }
        }

        internal ISettings SettingsRepo
        {
            get
            {
                if (_settingsRepo == null)
                    _settingsRepo = ProviderFactory.CreateProvider<ISettings>();

                return _settingsRepo;
            }
        }

        internal IPaymentRepo PaymentRepo
        {
            get
            {
                if (_paymentRepo == null)
                    _paymentRepo = ProviderFactory.CreateProvider<IPaymentRepo>();

                return _paymentRepo;
            }
        }
        #endregion

    }
}