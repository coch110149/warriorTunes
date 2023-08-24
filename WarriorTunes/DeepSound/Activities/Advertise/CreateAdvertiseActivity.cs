using System;
using System.Collections.Generic;
using System.Linq;
using MaterialDialogsCore;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Ads.DoubleClick;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidHUD;
using AndroidX.AppCompat.Content.Res;
using AndroidX.AppCompat.Widget;
using AndroidX.Core.Content;
using Bumptech.Glide;
using Bumptech.Glide.Request;
using TheArtOfDev.Edmodo.Cropper;
using Java.IO;
using DeepSound.Activities.Base;
using DeepSound.Helpers.Ads;
using DeepSound.Helpers.CacheLoaders;
using DeepSound.Helpers.Controller;
using DeepSound.Helpers.Utils;
using DeepSoundClient.Classes.Advertise;
using DeepSoundClient.Requests;
using Exception = System.Exception;
using Toolbar = AndroidX.AppCompat.Widget.Toolbar;
using Uri = Android.Net.Uri;

namespace DeepSound.Activities.Advertise
{
    [Activity(Icon = "@mipmap/icon", Theme = "@style/MyTheme", ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.UiMode | ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class CreateAdvertiseActivity : BaseActivity, MaterialDialog.IListCallback, MaterialDialog.ISingleButtonCallback, MaterialDialog.IListCallbackMultiChoice
    { 
        #region Variables Basic

        private ImageView ImageAd, BtnSelectImage;
        private EditText TxtName, TxtTitle, TxtUrl, TxtDescription, TxtAudience, TxtPlacement, TxtPricing, TxtSpending, TxtType;
        private string PathFile, TotalIdAudienceChecked, PlacementStatus, PricingStatus, TypeStatus, TypeDialog;
        private PublisherAdView PublisherAdView;
        private AppCompatButton BtnApply;

        #endregion

        #region General

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                SetTheme(DeepSoundTools.IsTabDark() ? Resource.Style.MyTheme_Dark : Resource.Style.MyTheme);

                Methods.App.FullScreenApp(this);

                // Create your application here
                SetContentView(Resource.Layout.CreateAdvertiseLayout);
                 
                //Get Value And Set Toolbar
                InitComponent();
                InitToolbar();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        protected override void OnResume()
        {
            try
            {
                base.OnResume();
                AddOrRemoveEvent(true);
                PublisherAdView?.Resume();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        protected override void OnPause()
        {
            try
            {
                base.OnPause();
                AddOrRemoveEvent(false);
                PublisherAdView?.Pause();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public override void OnTrimMemory(TrimMemory level)
        {
            try
            {
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
                base.OnTrimMemory(level);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public override void OnLowMemory()
        {
            try
            {
                GC.Collect(GC.MaxGeneration);
                base.OnLowMemory();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }
        protected override void OnDestroy()
        {
            try
            {
                DestroyBasic();
                base.OnDestroy();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        #endregion

        #region Menu

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    Finish();
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        #endregion

        #region Functions

        private void InitComponent()
        {
            try
            {
                ImageAd = FindViewById<ImageView>(Resource.Id.image);
                BtnSelectImage = FindViewById<ImageView>(Resource.Id.ChooseImageText);

                TxtName = FindViewById<EditText>(Resource.Id.NameEditText);
                TxtTitle = FindViewById<EditText>(Resource.Id.TitleEditText);
                TxtUrl = FindViewById<EditText>(Resource.Id.UrlEditText);
                TxtDescription = FindViewById<EditText>(Resource.Id.DescriptionEditText);
                TxtAudience = FindViewById<EditText>(Resource.Id.TargetAudienceEditText);
                TxtPlacement = FindViewById<EditText>(Resource.Id.PlacementEditText);
                TxtPricing = FindViewById<EditText>(Resource.Id.PricingEditText);
                TxtSpending = FindViewById<EditText>(Resource.Id.SpendingEditText);
                TxtType = FindViewById<EditText>(Resource.Id.TypeEditText);

                BtnApply = FindViewById<AppCompatButton>(Resource.Id.ApplyButton);

                PublisherAdView = FindViewById<PublisherAdView>(Resource.Id.multiple_ad_sizes_view);
                AdsGoogle.InitPublisherAdView(PublisherAdView);

                Methods.SetColorEditText(TxtName, DeepSoundTools.IsTabDark() ? Color.White : Color.Black);
                Methods.SetColorEditText(TxtTitle, DeepSoundTools.IsTabDark() ? Color.White : Color.Black);
                Methods.SetColorEditText(TxtUrl, DeepSoundTools.IsTabDark() ? Color.White : Color.Black);
                Methods.SetColorEditText(TxtDescription, DeepSoundTools.IsTabDark() ? Color.White : Color.Black);
                Methods.SetColorEditText(TxtAudience, DeepSoundTools.IsTabDark() ? Color.White : Color.Black);
                Methods.SetColorEditText(TxtPlacement, DeepSoundTools.IsTabDark() ? Color.White : Color.Black);
                Methods.SetColorEditText(TxtPricing, DeepSoundTools.IsTabDark() ? Color.White : Color.Black);
                Methods.SetColorEditText(TxtSpending, DeepSoundTools.IsTabDark() ? Color.White : Color.Black);
                Methods.SetColorEditText(TxtType, DeepSoundTools.IsTabDark() ? Color.White : Color.Black);

                Methods.SetFocusable(TxtAudience);
                Methods.SetFocusable(TxtPlacement);
                Methods.SetFocusable(TxtPricing);
                Methods.SetFocusable(TxtType);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void InitToolbar()
        {
            try
            {
                var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
                if (toolbar != null)
                {
                    toolbar.Title = GetString(Resource.String.Lbl_Create_Ad);
                    toolbar.SetTitleTextColor(DeepSoundTools.IsTabDark() ? Color.White : Color.Black);
                    SetSupportActionBar(toolbar);
                    SupportActionBar.SetDisplayShowCustomEnabled(true);
                    SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                    SupportActionBar.SetHomeButtonEnabled(true);
                    SupportActionBar.SetDisplayShowHomeEnabled(true);

                    var icon = AppCompatResources.GetDrawable(this, AppSettings.FlowDirectionRightToLeft ? Resource.Drawable.icon_back_arrow_right : Resource.Drawable.icon_back_arrow_left);
                    icon?.SetTint(DeepSoundTools.IsTabDark() ? Color.White : Color.Black);
                    SupportActionBar.SetHomeAsUpIndicator(icon);

                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void AddOrRemoveEvent(bool addEvent)
        {
            try
            {
                switch (addEvent)
                {
                    // true +=  // false -=
                    case true:
                        BtnApply.Click += TxtAddOnClick;
                        BtnSelectImage.Click += BtnSelectImageOnClick;
                        TxtAudience.Touch += TxtAudienceOnTouch;
                        TxtPlacement.Touch += TxtPlacementOnTouch;
                        TxtPricing.Touch += TxtPricingOnTouch;
                        TxtType.Touch += TxtTypeOnTouch;
                        break;
                    default:
                        BtnApply.Click -= TxtAddOnClick;
                        BtnSelectImage.Click -= BtnSelectImageOnClick;
                        TxtAudience.Touch -= TxtAudienceOnTouch;
                        TxtPlacement.Touch -= TxtPlacementOnTouch;
                        TxtPricing.Touch -= TxtPricingOnTouch;
                        TxtType.Touch -= TxtTypeOnTouch;
                        break;
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void DestroyBasic()
        {
            try
            {
                PublisherAdView?.Destroy();

                TxtName = null!;
                ImageAd = null!;
                BtnSelectImage = null!;
                TxtTitle = null!;
                TxtDescription = null!;
                TxtAudience = null!;
                TxtPlacement = null!;
                TxtPricing = null!;

                PublisherAdView = null!;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region Events

        //Add Image
        private void BtnSelectImageOnClick(object sender, EventArgs e)
        {
            try
            {
                if (TypeStatus == "banner")
                {
                    OpenDialogGallery();
                }
                else if (TypeStatus == "audio")
                {
                    OpenDialogAudio(); 
                }
                else
                {
                    Toast.MakeText(this, GetString(Resource.String.Lbl_Please_select_Type), ToastLength.Short)?.Show();
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void TxtTypeOnTouch(object sender, View.TouchEventArgs e)
        {
            try
            {
                if (e?.Event?.Action != MotionEventActions.Up) return;

                TypeDialog = "Type";

                var arrayAdapter = new List<string>();
                var dialogList = new MaterialDialog.Builder(this).Theme(DeepSoundTools.IsTabDark() ? MaterialDialogsTheme.Dark : MaterialDialogsTheme.Light);

                arrayAdapter.Add(GetText(Resource.String.Lbl_TypeBanners)); //banner
                arrayAdapter.Add(GetText(Resource.String.Lbl_TypeAudio)); //audio

                dialogList.Title(GetText(Resource.String.Lbl_Type)).TitleColorRes(Resource.Color.primary);
                dialogList.Items(arrayAdapter);
                dialogList.NegativeText(GetText(Resource.String.Lbl_Close)).OnNegative(new MyMaterialDialog());
                dialogList.AlwaysCallSingleChoiceCallback();
                dialogList.ItemsCallback(this).Build().Show();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void TxtPricingOnTouch(object sender, View.TouchEventArgs e)
        {
            try
            {
                if (e?.Event?.Action != MotionEventActions.Up) return;

                TypeDialog = "Pricing";

                var arrayAdapter = new List<string>();
                var dialogList = new MaterialDialog.Builder(this).Theme(DeepSoundTools.IsTabDark() ? MaterialDialogsTheme.Dark : MaterialDialogsTheme.Light);

                arrayAdapter.Add(GetText(Resource.String.Lbl_PricingClick)); //1
                arrayAdapter.Add(GetText(Resource.String.Lbl_PricingViews)); //2

                dialogList.Title(GetText(Resource.String.Lbl_Pricing)).TitleColorRes(Resource.Color.primary);
                dialogList.Items(arrayAdapter);
                dialogList.NegativeText(GetText(Resource.String.Lbl_Close)).OnNegative(new MyMaterialDialog());
                dialogList.AlwaysCallSingleChoiceCallback();
                dialogList.ItemsCallback(this).Build().Show();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void TxtPlacementOnTouch(object sender, View.TouchEventArgs e)
        {
            try
            {
                if (e?.Event?.Action != MotionEventActions.Up) return;

                TypeDialog = "Placement";

                var arrayAdapter = new List<string>();
                var dialogList = new MaterialDialog.Builder(this).Theme(DeepSoundTools.IsTabDark() ? MaterialDialogsTheme.Dark : MaterialDialogsTheme.Light);

                arrayAdapter.Add(GetText(Resource.String.Lbl_Placement_TrackPage)); //1
                arrayAdapter.Add(GetText(Resource.String.Lbl_Placement_AllPage)); //2

                dialogList.Title(GetText(Resource.String.Lbl_Placement)).TitleColorRes(Resource.Color.primary);
                dialogList.Items(arrayAdapter);
                dialogList.NegativeText(GetText(Resource.String.Lbl_Close)).OnNegative(new MyMaterialDialog());
                dialogList.AlwaysCallSingleChoiceCallback();
                dialogList.ItemsCallback(this).Build().Show();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void TxtAudienceOnTouch(object sender, View.TouchEventArgs e)
        {
            try
            {
                if (e?.Event?.Action != MotionEventActions.Up) return;

                TypeDialog = "Audience";

                var arrayIndexAdapter = new int[] { };
                var countriesArray = DeepSoundTools.GetCountryList(this);

                var arrayAdapter = countriesArray.Select(item => item.Value).ToList();

                var dialogList = new MaterialDialog.Builder(this).Theme(DeepSoundTools.IsTabDark() ? MaterialDialogsTheme.Dark : MaterialDialogsTheme.Light);
                dialogList.Title(GetText(Resource.String.Lbl_TargetAudience)).TitleColorRes(Resource.Color.primary)
                    .Items(arrayAdapter)
                    .ItemsCallbackMultiChoice(arrayIndexAdapter, this)
                    .AlwaysCallMultiChoiceCallback()
                    .AutoDismiss(false)
                    .PositiveText(GetText(Resource.String.Lbl_Close)).OnPositive(this)
                    .NeutralText(Resource.String.Lbl_SelectAll).OnNeutral((dialog, action) =>
                    {
                        try
                        {
                            dialog.SelectAllIndices();

                            TotalIdAudienceChecked = "";
                            foreach (var item in countriesArray)
                            {
                                TotalIdAudienceChecked += item.Key + ",";
                            }

                            TxtAudience.Text = TypeDialog == "Audience" && !string.IsNullOrEmpty(TotalIdAudienceChecked) ? GetText(Resource.String.Lbl_Selected) : TxtAudience.Text;

                            dialog.Dismiss();
                        }
                        catch (Exception ex)
                        {
                            Methods.DisplayReportResultTrack(ex);
                        }
                    })
                    .Build().Show();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }
        
        //Save 
        private async void TxtAddOnClick(object sender, EventArgs e)
        {
            try
            {
                if (!Methods.CheckConnectivity())
                {
                    Toast.MakeText(this, GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)?.Show();
                }
                else
                { 
                    if (string.IsNullOrEmpty(TxtName.Text) || string.IsNullOrWhiteSpace(TxtName.Text))
                    {
                        Toast.MakeText(this, GetString(Resource.String.Lbl_PleaseEnterName), ToastLength.Short)?.Show();
                        return;
                    }

                    if (string.IsNullOrEmpty(TxtTitle.Text) || string.IsNullOrWhiteSpace(TxtTitle.Text))
                    {
                        Toast.MakeText(this, GetString(Resource.String.Lbl_PleaseEnterTitle), ToastLength.Short)?.Show();
                        return;
                    }

                    if (string.IsNullOrEmpty(TxtUrl.Text) || string.IsNullOrWhiteSpace(TxtUrl.Text))
                    {
                        Toast.MakeText(this, GetString(Resource.String.Lbl_PleaseEnterUrl), ToastLength.Short)?.Show();
                        return;
                    }
                      
                    if (string.IsNullOrEmpty(TxtDescription.Text) || string.IsNullOrWhiteSpace(TxtDescription.Text))
                    {
                        Toast.MakeText(this, GetString(Resource.String.Lbl_PleaseEnterDescription), ToastLength.Short)?.Show();
                        return;
                    }
                      
                    if (string.IsNullOrEmpty(TxtAudience.Text) || string.IsNullOrWhiteSpace(TxtAudience.Text) || string.IsNullOrEmpty(TotalIdAudienceChecked))
                    {
                        Toast.MakeText(this, GetString(Resource.String.Lbl_Please_select_Audience), ToastLength.Short)?.Show();
                        return;
                    }
                     
                    if (string.IsNullOrEmpty(TxtPlacement.Text) || string.IsNullOrWhiteSpace(TxtPlacement.Text))
                    {
                        Toast.MakeText(this, GetString(Resource.String.Lbl_Please_select_Placement), ToastLength.Short)?.Show();
                        return;
                    }
                       
                    if (string.IsNullOrEmpty(TxtPricing.Text) || string.IsNullOrWhiteSpace(TxtPricing.Text))
                    {
                        Toast.MakeText(this, GetString(Resource.String.Lbl_Please_select_Pricing), ToastLength.Short)?.Show();
                        return;
                    }
                       
                    if (string.IsNullOrEmpty(TxtSpending.Text) || string.IsNullOrWhiteSpace(TxtSpending.Text))
                    {
                        Toast.MakeText(this, GetString(Resource.String.Lbl_Please_select_Spending), ToastLength.Short)?.Show();
                        return;
                    }
                       
                    if (string.IsNullOrEmpty(TxtType.Text) || string.IsNullOrWhiteSpace(TxtType.Text))
                    {
                        Toast.MakeText(this, GetString(Resource.String.Lbl_Please_select_Type), ToastLength.Short)?.Show();
                        return;
                    }
                       
                    //Show a progress
                    AndHUD.Shared.Show(this, GetText(Resource.String.Lbl_Loading));

                    TotalIdAudienceChecked = TotalIdAudienceChecked.Length switch
                    {
                        > 0 => TotalIdAudienceChecked.Remove(TotalIdAudienceChecked.Length - 1, 1),
                        _ => TotalIdAudienceChecked
                    };

                    var dictionary = new Dictionary<string, string>
                    {
                        {"name", TxtName.Text},
                        {"url",TxtUrl.Text},
                        {"title",TxtTitle.Text},
                        {"desc", TxtDescription.Text},
                        {"audience-list", TotalIdAudienceChecked},
                        {"cost",PricingStatus},
                        {"placement", PlacementStatus},
                        {"type", TypeStatus},
                    };

                    var (apiStatus, respond) = await RequestsAsync.Advertise.CreateAdvertiseAsync(dictionary, TypeStatus, PathFile);
                    if (apiStatus == 200)
                    {
                        if (respond is CreateAdvertiseObject result)
                        {
                            AndHUD.Shared.Dismiss(this);
                            Toast.MakeText(this, GetString(Resource.String.Lbl_CreatedSuccessfully), ToastLength.Short)?.Show();
                             
                            Finish();
                        }
                    }
                    else
                        Methods.DisplayAndHudErrorResult(this, respond);
                }
            }
            catch (Exception exception)
            {
                AndHUD.Shared.Dismiss(this);
                Methods.DisplayReportResultTrack(exception);
            }
        }

        #endregion

        #region Permissions && Result

        //Result
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            try
            {
                base.OnActivityResult(requestCode, resultCode, data);
                if (requestCode == CropImage.CropImageActivityRequestCode)
                {
                    var result = CropImage.GetActivityResult(data); 
                    if (result.IsSuccessful)
                    {
                        var resultUri = result.Uri;

                        if (!string.IsNullOrEmpty(resultUri.Path))
                        {
                            PathFile = resultUri.Path;
                            File file2 = new File(resultUri.Path);
                            var photoUri = FileProvider.GetUriForFile(this, PackageName + ".fileprovider", file2);
                            Glide.With(this).Load(photoUri).Apply(new RequestOptions()).Into(ImageAd);

                            //GlideImageLoader.LoadImage(this, resultUri.Path, ImgCover, ImageStyle.CenterCrop, ImagePlaceholders.Drawable);
                        }
                        else
                        {
                            Toast.MakeText(this, GetString(Resource.String.Lbl_something_went_wrong), ToastLength.Short)?.Show();
                        }
                    }
                }
                else if (requestCode == 505 && resultCode == Result.Ok) //==> Audio
                {
                    var filepath = Methods.AttachmentFiles.GetActualPathFromFile(this, data.Data);
                    if (filepath != null)
                    {
                        PathFile = filepath; 
                        GlideImageLoader.LoadImage(this, "Audio_File", ImageAd, ImageStyle.CenterCrop, ImagePlaceholders.Drawable);
                    }
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        //Permissions
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            try
            {
                base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

                switch (requestCode)
                {
                    case 108 when grantResults.Length > 0 && grantResults[0] == Permission.Granted:
                        OpenDialogGallery();
                        break;
                    case 108:
                        Toast.MakeText(this, GetString(Resource.String.Lbl_Permission_is_denied), ToastLength.Short)?.Show();
                        break;
                    case 100 when grantResults.Length > 0 && grantResults[0] == Permission.Granted:
                        OpenDialogAudio();
                        break;
                    case 100:
                        Toast.MakeText(this, GetString(Resource.String.Lbl_Permission_is_denied), ToastLength.Short)?.Show();
                        break;
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region MaterialDialog

        public void OnSelection(MaterialDialog dialog, View itemView, int position, string itemString)
        {
            try
            {
                switch (TypeDialog)
                {
                    case "Placement":
                        {
                            if (itemString == GetText(Resource.String.Lbl_Placement_TrackPage))
                            {
                                PlacementStatus = "1";
                            }
                            else if (itemString == GetText(Resource.String.Lbl_Placement_AllPage))
                            {
                                PlacementStatus = "2";
                            }

                            TxtPlacement.Text = itemString;
                            break;
                        }
                    case "Pricing":
                        {
                            if (itemString == GetText(Resource.String.Lbl_PricingClick))
                            {
                                PricingStatus = "1";
                            }
                            else if (itemString == GetText(Resource.String.Lbl_PricingViews))
                            {
                                PricingStatus = "2";
                            }

                            TxtPricing.Text = itemString;
                            break;
                        }
                    case "Type":
                        {
                            if (itemString == GetText(Resource.String.Lbl_TypeBanners))
                            {
                                TypeStatus = "banner";
                            }
                            else if (itemString == GetText(Resource.String.Lbl_TypeAudio))
                            {
                                TypeStatus = "audio";
                            }

                            PathFile = "";
                            ImageAd.SetImageResource(0);

                            TxtType.Text = itemString;
                            break;
                        }
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        bool MaterialDialog.IListCallbackMultiChoice.OnSelection(MaterialDialog dialog, int[] which, string[] text)
        {
            try
            {
                switch (TypeDialog)
                {
                    case "Audience":
                        {
                            TotalIdAudienceChecked = "";
                            var countriesArray = DeepSoundTools.GetCountryList(this);
                            for (int i = 0; i < which.Length; i++)
                            {
                                var itemString = text[i];
                                var check = countriesArray.FirstOrDefault(a => a.Value == itemString).Key;
                                if (check != null)
                                {
                                    TotalIdAudienceChecked += check + ",";
                                }
                            }

                            break;
                        }
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return true;
            }
            return true;
        }

        public void OnClick(MaterialDialog p0, DialogAction p1)
        {
            try
            {
                if (p1 == DialogAction.Positive)
                {
                    TxtAudience.Text = TypeDialog == "Audience" && !string.IsNullOrEmpty(TotalIdAudienceChecked) ? GetText(Resource.String.Lbl_Selected) : TxtAudience.Text;

                    p0.Dismiss();
                }
                else if (p1 == DialogAction.Negative)
                {
                    p0.Dismiss();
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        private void OpenDialogGallery()
        {
            try
            {
                if (!DeepSoundTools.CheckAllowedFileUpload())
                {
                    Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), GetText(Resource.String.Lbl_Error_AllowedFileUpload), GetText(Resource.String.Lbl_Ok));
                    return;
                }

                switch ((int)Build.VERSION.SdkInt)
                {
                    // Check if we're running on Android 5.0 or higher
                    case < 23:
                        {
                            Methods.Path.Chack_MyFolder();

                            //Open Image 
                            var myUri = Uri.FromFile(new File(Methods.Path.FolderDiskImage, Methods.GetTimestamp(DateTime.Now) + ".jpg"));
                            CropImage.Activity()
                                .SetInitialCropWindowPaddingRatio(0)
                                .SetAutoZoomEnabled(true)
                                .SetMaxZoom(4)
                                .SetGuidelines(CropImageView.Guidelines.On)
                                .SetCropMenuCropButtonTitle(GetText(Resource.String.Lbl_Crop))
                                .SetOutputUri(myUri).Start(this);
                            break;
                        }
                    default:
                        {
                            if (!CropImage.IsExplicitCameraPermissionRequired(this) && PermissionsController.CheckPermissionStorage() && CheckSelfPermission(Manifest.Permission.Camera) == Permission.Granted)
                            {
                                Methods.Path.Chack_MyFolder();

                                //Open Image 
                                var myUri = Uri.FromFile(new File(Methods.Path.FolderDiskImage, Methods.GetTimestamp(DateTime.Now) + ".jpg"));
                                CropImage.Activity()
                                    .SetInitialCropWindowPaddingRatio(0)
                                    .SetAutoZoomEnabled(true)
                                    .SetMaxZoom(4)
                                    .SetGuidelines(CropImageView.Guidelines.On)
                                    .SetCropMenuCropButtonTitle(GetText(Resource.String.Lbl_Crop))
                                    .SetOutputUri(myUri).Start(this);
                            }
                            else
                            {
                                new PermissionsController(this).RequestPermission(108);
                            }

                            break;
                        }
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void OpenDialogAudio()
        {
            try
            {
                if (!DeepSoundTools.CheckAllowedFileUpload())
                {
                    Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), GetText(Resource.String.Lbl_Error_AllowedFileUpload), GetText(Resource.String.Lbl_Ok));
                    return;
                }

                // Check if we're running on Android 5.0 or higher
                if ((int)Build.VERSION.SdkInt < 23)
                    new IntentController(this).OpenIntentAudio(); //505
                else
                {
                    if (PermissionsController.CheckPermissionStorage() )
                        new IntentController(this).OpenIntentAudio(); //505
                    else
                        new PermissionsController(this).RequestPermission(100);
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

    }
}