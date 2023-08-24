using DeepSound.Helpers.Model;

namespace DeepSound
{
    //For the accuracy of the icon and logo, please use this website " https://appicon.co " and add images according to size in folders " mipmap " 

    internal static class AppSettings
    {
        /// <summary>
        /// you should add your website without http in the analytic.xml file >> ../values/analytic.xml .. line 5
        /// <string name="ApplicationUrlWeb">deepsoundscript.com</string>
        /// </summary>  
        public static string Cert = "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";
        
        //Main Settings >>>>>
        //*********************************************************
        public static string ApplicationName = "Warrior Tunes";
        public static string DatabaseName = "DeepSoundMusic"; 
        public static string Version = "2.5";

        //Main Colors >>
        //*********************************************************
        public static string MainColor = "#FF8216";

        public static string BackgroundGradationColor1 = "#3033c6";  
        public static string BackgroundGradationColor2 = "#fb0049";  

        //Language Settings >> http://www.lingoes.net/en/translator/langcode.htm
        //*********************************************************
        public static bool FlowDirectionRightToLeft = false;
        public static string Lang = ""; //Default language ar_AE

        //Error Report Mode
        //*********************************************************
        public static bool SetApisReportMode = false;

        //Notification Settings >>
        //*********************************************************
        public static bool ShowNotification = true;
        public static string OneSignalAppId = "bfc2e195-6c3c-4123-97ce-1e122f5809a9";
         
        //AdMob >> Please add the code ads in the Here and analytic.xml 
        //*********************************************************
        public static ShowAds ShowAds = ShowAds.AllUsers;

        //Three times after entering the ad is displayed
        public static int ShowAdInterstitialCount = 3;
        public static int ShowAdRewardedVideoCount = 3;
        public static int ShowAdNativeCount = 5;
        public static int ShowAdAppOpenCount = 2;
          
        public static bool ShowAdMobBanner = false;
        public static bool ShowAdMobInterstitial = false;
        public static bool ShowAdMobRewardVideo = false;
        public static bool ShowAdMobNative = false;
        public static bool ShowAdMobAppOpen = false;  
        public static bool ShowAdMobRewardedInterstitial = false;  

        public static string AdInterstitialKey = "ca-app-pub-5135691635931982/6646750931";
        public static string AdRewardVideoKey = "ca-app-pub-5135691635931982/6981792857";
        public static string AdAdMobNativeKey = "ca-app-pub-5135691635931982/1394424252";
        public static string AdAdMobAppOpenKey = "ca-app-pub-5135691635931982/1906896275";  
        public static string AdRewardedInterstitialKey = "ca-app-pub-5135691635931982/4054725070";
         
        //FaceBook Ads >> Please add the code ad in the Here and analytic.xml 
        //*********************************************************
        public static bool ShowFbBannerAds = false; 
        public static bool ShowFbInterstitialAds = false;  
        public static bool ShowFbRewardVideoAds = false;  
        public static bool ShowFbNativeAds = false; 

        //YOUR_PLACEMENT_ID
        public static string AdsFbBannerKey = "250485588986218_554026418632132"; 
        public static string AdsFbInterstitialKey = "250485588986218_554026125298828";  
        public static string AdsFbRewardVideoKey = "250485588986218_554072818627492"; 
        public static string AdsFbNativeKey = "250485588986218_554706301897477";

        //Colony Ads >> Please add the code ad in the Here 
        //*********************************************************  
        public static bool ShowColonyBannerAds = false; 
        public static bool ShowColonyInterstitialAds = false; 
        public static bool ShowColonyRewardAds = false; 

        public static string AdsColonyAppId = "appc1a3a39f4257436fb0";
        public static string AdsColonyBannerId = "vzf3427a794942477a91";
        public static string AdsColonyInterstitialId = "vz0df8be89b80d41a9ba";
        public static string AdsColonyRewardedId = "vzd163d9467cbc4ab681";
        //*********************************************************   

        //Social Logins >>
        //If you want login with facebook or google you should change id key in the analytic.xml file  
        //Facebook >> ../values/analytic.xml .. line 10 - 11
        //Google >> ../values/analytic.xml .. line 15
        //*********************************************************
        public static bool EnableSmartLockForPasswords = false;
        
        public static bool ShowFacebookLogin = false;
        public static bool ShowGoogleLogin = false; 
        public static bool ShowWoWonderLogin = false;  

        public static string ClientId = "104516058316-9vjdctmsk63o35nbpp872is04qqa84vc.apps.googleusercontent.com";

        public static string AppNameWoWonder = "WoWonder";
        public static readonly string WoWonderDomainUri = "https://demo.wowonder.com";
        public static readonly string WoWonderAppKey = "131c471c8b4edf662dd0ebf7adf3c3d7365838b9";

        //*********************************************************
        public static bool ShowPrice = true;
        public static bool ShowSkipButton = true;

        //in album
        public static bool ShowCountPurchases = true; 

        //Show Title Album Only on song
        public static bool ShowTitleAlbumOnly = false;  

        //Set Theme Full Screen App
        //*********************************************************
        public static bool EnableFullScreenApp = false;

        public static bool EnableOptimizationApp = false;  

        public static bool ShowSettingsRateApp = true;  
        public static int ShowRateAppCount = 5;

        public static bool ShowSettingsHelp = true; 
        public static bool ShowSettingsTermsOfUse = true; 
        public static bool ShowSettingsAbout = true; 
        public static bool ShowSettingsDeleteAccount = true; 
         
        public static bool ShowSettingsUpdateManagerApp = false; 

        public static bool ShowTextWithSpace = false; 
         
        //Set Blur Screen Comment
        //*********************************************************
        public static readonly bool EnableBlurBackgroundComment = true;

        //Set the radius of the Blur. Supported range 0 < radius <= 25
        public static readonly float BlurRadiusComment = 25f;

        //Import && Upload Videos >>  
        //*********************************************************
        public static bool ShowButtonUploadSingleSong { get; set; } = true;
        public static bool ShowButtonUploadAlbum { get; set; } = true;  
        public static bool ShowButtonImportSong { get; set; } = true;

        //Tap profile
        //*********************************************************
        public static bool ShowStore = true;  
        public static bool ShowStations = true;  
        public static bool ShowPlaylist = true;
        public static bool ShowEvent = true; //#New
        public static bool ShowProduct = true; //#New
        public static bool ShowAdvertise = true; //#New

        //Offline Sound >>  
        //*********************************************************
        public static bool AllowOfflineDownload = true;
         
        //Profile >>  
        //*********************************************************
        public static bool ShowEmail = true; 

        public static bool ShowForwardTrack = true; 
        public static bool ShowBackwardTrack = true; 

        //Settings Page >>  
        //*********************************************************
        public static bool ShowEditPassword = true; 
        public static bool ShowWithdrawals = true; 
        public static bool ShowGoPro = true; 
        public static bool ShowBlockedUsers = true; 
        public static bool ShowBlog = true;  
        public static bool ShowSettingsTwoFactor = true; 
        public static bool ShowSettingsManageSessions = true;  

        //Last_Messages Page >>
        //********************************************************* 
        public static bool RunSoundControl = true; 
        public static int RefreshAppAPiSeconds = 6000; // 6 Seconds
        public static int MessageRequestSpeed = 3000; // 3 Seconds

        //Set Theme App >> Color - Tab
        //*********************************************************
        public static TabTheme SetTabDarkTheme = TabTheme.Light;

        //Bypass Web Erros 
        //*********************************************************
        public static bool TurnTrustFailureOnWebException = true;
        public static bool TurnSecurityProtocolType3072On = true;

        //*********************************************************
        public static bool RenderPriorityFastPostLoad = true;

        //Payment System
        //*********************************************************
        /// <summary>
        /// if you want this feature enabled go to Properties -> AndroidManefist.xml and remove comments from below code
        /// <uses-permission android:name="com.android.vending.BILLING" />
        /// </summary>
        public static bool ShowInAppBilling = false;

        /// <summary>
        /// Paypal and google pay using Braintree Gateway https://www.braintreepayments.com/
        /// 
        /// Add info keys in Payment Methods : https://prnt.sc/1z5bffc & https://prnt.sc/1z5b0yj
        /// To find your merchant ID :  https://prnt.sc/1z59dy8
        ///
        /// Tokenization Keys : https://prnt.sc/1z59smv
        /// </summary>
        public static bool ShowPaypal = true;
        public static string MerchantAccountId = "test"; 

        public static string SandboxTokenizationKey = "sandbox_kt2f6mdh_hf4c******"; 
        public static string ProductionTokenizationKey = "production_t2wns2y2_dfy45******";  

        public static bool ShowBankTransfer = true;
        public static bool ShowCreditCard = true; 
         
        public static bool ShowCashFree = true;

        /// <summary>
        /// Currencies : http://prntscr.com/u600ok
        /// </summary>
        public static string CashFreeCurrency = "INR";
         
        /// <summary>
        /// If you want RazorPay you should change id key in the analytic.xml file
        /// razorpay_api_Key >> .. line 24 
        /// </summary>
        public static bool ShowRazorPay = true;

        /// <summary>
        /// Currencies : https://razorpay.com/accept-international-payments
        /// </summary>
        public static string RazorPayCurrency = "USD";

        public static bool ShowPayStack = true;
        public static bool ShowPaySera = false;

        public static bool ShowPayUmoney = true;
        public static bool ShowAuthorizeNet = true;
        public static bool ShowSecurionPay = true;
        public static bool ShowIyziPay = true;//#New
        public static bool ShowAamarPay = true;//#New

        //********************************************************* 
        public static bool AllowDeletingDownloadedSongs = true; 

        
    }
} 