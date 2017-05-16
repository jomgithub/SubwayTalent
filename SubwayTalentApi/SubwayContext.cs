using SubwayTalent.Contracts.Interfaces;
using SubwayTalent.Core;
using SubwayTalent.Core.Interfaces;
using SubwayTalent.Data.Interfaces;
using SubwayTalentApi.Helpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace SubwayTalentApi
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

        #region "private tool objects"
        private IThumbnailGenerator _generator;
        private ILogger _logger;
        private IEmailProvider _emailProvider;
        private IAPNPushNotification _appleNotification;
        private NotificationHelper _notification;
        private IPayPalProcessor _paypalProcessor;
        private IBraintreeProcessor _braintreeProcessor;
        #endregion
        
        private string[] _imageMimeTypes;
        private string[] _videoMimeTypes;
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

        internal IAPNPushNotification AppleNotification 
        { 
            get 
            {
                if (_appleNotification == null)
                    _appleNotification = ProviderFactory.CreateProvider<IAPNPushNotification>();
                return _appleNotification;
            }
        }

        internal IThumbnailGenerator ThumbnailGenerator
        {
            get
            { 
                if(_generator == null)
                    _generator = ProviderFactory.CreateProvider<IThumbnailGenerator>();
                return _generator;
            }
        }

        internal ILogger Logger
        {
            get
            {
                if (_logger == null)
                    _logger = ProviderFactory.CreateProvider<ILogger>();
                return _logger;
            }
        }

        internal IEmailProvider EmailProvider
        {
            get
            {
                if (_emailProvider == null)
                {
                    _emailProvider = ProviderFactory.CreateProvider<IEmailProvider>();
                    _emailProvider.Username = ConfigurationManager.AppSettings["mailUsername"];
                    _emailProvider.Password = ConfigurationManager.AppSettings["mailPassword"];
                }
                return _emailProvider;
            }
        }

        internal NotificationHelper Notifications
        {
            get
            {
                if (_notification == null)
                    _notification = new NotificationHelper();
                return _notification;
            }
        }

        internal string[] VideMimeTypesList
        {
            get
            {
                if (_videoMimeTypes == null)
                    _videoMimeTypes = ConfigurationManager.AppSettings["VideoMimeTypes"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                return _videoMimeTypes;
            }
        }

        internal string[] ImageMimeTypesList
        {
            get
            {
                if (_imageMimeTypes == null)
                    _imageMimeTypes = ConfigurationManager.AppSettings["ImageMimeTypes"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                return _imageMimeTypes;
            }
        }

        internal IPayPalProcessor PayPalProcessor
        {
            get
            {
                if (_paypalProcessor == null)
                    _paypalProcessor = ProviderFactory.CreateProvider<IPayPalProcessor>();
                return _paypalProcessor;
            }
        }

        internal IBraintreeProcessor BraintreeProcessor
        {
            get
            {
                if (_braintreeProcessor == null)
                    _braintreeProcessor = ProviderFactory.CreateProvider<IBraintreeProcessor>();
                return _braintreeProcessor;
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