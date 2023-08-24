using Android;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Media;
using Android.Provider;
using Android.Widget;
using AndroidX.Core.Content;
using System;
using System.IO;
using System.Net;
using System.Linq;
using Android.Graphics;
using DeepSound.Activities.Tabbes;
using DeepSound.Helpers.Utils;
using DeepSoundClient.Classes.Global;
using Google.Android.Material.BottomSheet;
using Newtonsoft.Json;
using DeepSound.Helpers.Model;
using DeepSound.Helpers.CacheLoaders;
using DeepSound.Helpers.MediaPlayerController;
using DeepSound.Helpers.Controller;
using DeepSoundClient;
using File = Java.IO.File;
using IOException = Java.IO.IOException;
using Exception = System.Exception;
using Path = System.IO.Path;

namespace DeepSound.Activities.Songs
{
    public class SongOptionBottomDialogFragment : BottomSheetDialogFragment 
    {
        #region Variables Basic

        private HomeActivity GlobalContext;
        private ImageView Image, IconHeart;
        private TextView TxtTitle, TxtSeconderText;

        private LinearLayout PlayNextLayout, PlayingQueueLayout, EditSongLayout, AddPlaylistLayout, GoToAlbumLayout, GoToArtistLayout, DetailsLayout, RePostLayout, ReportLayout, ReportCopyrightLayout, SetRingtoneLayout, AddToBlackListLayout, ShareLayout, DeleteLayout;

        private SoundDataObject SongsClass;
        private string NamePage;
        private SocialIoClickListeners ClickListeners;
        public static SongOptionBottomDialogFragment Instance;

        #endregion

        #region General

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your fragment here
            GlobalContext = HomeActivity.GetInstance();
            Instance = this;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            try
            {
                // create ContextThemeWrapper from the original Activity Context with the custom theme
                Context contextThemeWrapper = DeepSoundTools.IsTabDark() ? new ContextThemeWrapper(Activity, Resource.Style.MyTheme_Dark) : new ContextThemeWrapper(Activity, Resource.Style.MyTheme);
                // clone the inflater using the ContextThemeWrapper
                LayoutInflater localInflater = inflater.CloneInContext(contextThemeWrapper);
                View view = localInflater?.Inflate(Resource.Layout.BottomSheetSongOption, container, false);
                return view;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return null!;
            }
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            try
            {
                base.OnViewCreated(view, savedInstanceState);

                //Get Value And Set Toolbar
                InitComponent(view);
                SetData();

                ClickListeners = new SocialIoClickListeners(GlobalContext);
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

        public override void OnDestroy()
        {
            Instance = null;
            base.OnDestroy();
        }

        #endregion

        #region Functions

        private void InitComponent(View view)
        {
            try
            {
                Image = view.FindViewById<ImageView>(Resource.Id.Image);
                TxtTitle = view.FindViewById<TextView>(Resource.Id.title);
                TxtSeconderText = view.FindViewById<TextView>(Resource.Id.brief);
                IconHeart = view.FindViewById<ImageView>(Resource.Id.heart);

                PlayNextLayout = view.FindViewById<LinearLayout>(Resource.Id.PlayNextLayout);
                PlayingQueueLayout = view.FindViewById<LinearLayout>(Resource.Id.PlayingQueueLayout);
                EditSongLayout = view.FindViewById<LinearLayout>(Resource.Id.EditSongLayout);
                AddPlaylistLayout = view.FindViewById<LinearLayout>(Resource.Id.AddPlaylistLayout);
                GoToAlbumLayout = view.FindViewById<LinearLayout>(Resource.Id.GoToAlbumLayout);
                GoToArtistLayout = view.FindViewById<LinearLayout>(Resource.Id.GoToArtistLayout);
                DetailsLayout = view.FindViewById<LinearLayout>(Resource.Id.DetailsLayout);
                RePostLayout = view.FindViewById<LinearLayout>(Resource.Id.RePostLayout);
                ReportLayout = view.FindViewById<LinearLayout>(Resource.Id.ReportLayout);
                ReportCopyrightLayout = view.FindViewById<LinearLayout>(Resource.Id.ReportCopyrightLayout);
                SetRingtoneLayout = view.FindViewById<LinearLayout>(Resource.Id.SetRingtoneLayout);
                AddToBlackListLayout = view.FindViewById<LinearLayout>(Resource.Id.AddToBlackListLayout);
                ShareLayout = view.FindViewById<LinearLayout>(Resource.Id.ShareLayout);
                DeleteLayout = view.FindViewById<LinearLayout>(Resource.Id.DeleteLayout);

                IconHeart.Click += IconHeartOnClick;
                PlayNextLayout.Click += PlayNextLayoutOnClick;
                PlayingQueueLayout.Click += PlayingQueueLayoutOnClick;
                EditSongLayout.Click += EditSongLayoutOnClick;
                AddPlaylistLayout.Click += AddPlaylistLayoutOnClick;
                GoToAlbumLayout.Click += GoToAlbumLayoutOnClick;
                GoToArtistLayout.Click += GoToArtistLayoutOnClick;
                DetailsLayout.Click += DetailsLayoutOnClick;
                RePostLayout.Click += RePostLayoutOnClick;
                ReportLayout.Click += ReportLayoutOnClick;
                ReportCopyrightLayout.Click += ReportCopyrightLayoutOnClick;
                SetRingtoneLayout.Click += SetRingtoneLayoutOnClick;
                AddToBlackListLayout.Click += AddToBlackListLayoutOnClick;
                ShareLayout.Click += ShareLayoutOnClick;
                DeleteLayout.Click += DeleteLayoutOnClick;

                //wael change to Visible after update 
                GoToAlbumLayout.Visibility = ViewStates.Gone;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region Events

        private void IconHeartOnClick(object sender, EventArgs e)
        {
            try
            {
                ClickListeners.OnLikeSongsClick(new MoreClickEventArgs { Button = IconHeart, SongsClass = SongsClass }, NamePage);
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void PlayNextLayoutOnClick(object sender, EventArgs e)
        {
            try
            {
                if (Constant.ArrayListPlay.Count > 1)
                {
                    Constant.ArrayListPlay.Insert(1, SongsClass);
                }
                else
                {
                    Constant.ArrayListPlay.Add(SongsClass);
                }
                Dismiss();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void PlayingQueueLayoutOnClick(object sender, EventArgs e)
        {
            try
            {   
                if (Constant.ArrayListPlay.Count > 0)
                    Constant.ArrayListPlay.Add(SongsClass);

                Dismiss();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void EditSongLayoutOnClick(object sender, EventArgs e)
        {
            try
            {
                ClickListeners.OnMenuEditSongOnClick(new MoreClickEventArgs() { SongsClass = SongsClass });
                Dismiss();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }
          
        private void AddPlaylistLayoutOnClick(object sender, EventArgs e)
        {
            try
            {
                ClickListeners.AddPlaylistOnClick(new MoreClickEventArgs() { SongsClass = SongsClass });
                Dismiss();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void GoToAlbumLayoutOnClick(object sender, EventArgs e)
        {
            try
            { 
                ClickListeners.AddAlbumOnClick(new MoreClickEventArgs() { SongsClass = SongsClass });
                Dismiss();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void GoToArtistLayoutOnClick(object sender, EventArgs e)
        {
            try
            { 
                GlobalContext?.OpenProfile(SongsClass.Publisher.Id, SongsClass.Publisher);
                Dismiss();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void DetailsLayoutOnClick(object sender, EventArgs e)
        {
            try
            {
                new DialogInfoSong(Activity).Display(SongsClass);
                Dismiss();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void RePostLayoutOnClick(object sender, EventArgs e)
        {
            try
            {
                ClickListeners.OnMenuRePostOnClick(new MoreClickEventArgs() { SongsClass = SongsClass });
                Dismiss();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void ReportLayoutOnClick(object sender, EventArgs e)
        {
            try
            {
                ClickListeners.OnMenuReportSongOnClick(new MoreClickEventArgs() { SongsClass = SongsClass });
                Dismiss();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void ReportCopyrightLayoutOnClick(object sender, EventArgs e)
        {
            try
            {
                ClickListeners.OnMenuReportCopyrightSongOnClick(new MoreClickEventArgs() { SongsClass = SongsClass });
                Dismiss();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void SetRingtoneLayoutOnClick(object sender, EventArgs e)
        {
            try
            {
                // Check if we're running on Android 5.0 or higher
                if ((int)Build.VERSION.SdkInt < 23)
                { 
                    SetRingtone();
                }
                else
                {
                    if (PermissionsController.CheckPermissionStorage())
                    { 
                        SetRingtone();
                    }
                    else
                    {
                        GlobalContext.RequestPermissions(new[]
                        {
                            Manifest.Permission.ReadExternalStorage,
                            Manifest.Permission.WriteExternalStorage,
                            Manifest.Permission.ManageExternalStorage,
                            Manifest.Permission.AccessMediaLocation,
                            Manifest.Permission.WriteSettings,
                        }, 1325);
                    }
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        public void SetRingtone()
        {
            try
            {
                Methods.Path.Chack_MyFolder();
                if (SongsClass.AudioLocation.Contains("http"))
                {
                    string folderDestination = Methods.Path.FolderDcimSound;

                    string filePath = Path.Combine(folderDestination);

                    string filename = SongsClass.Title;
                    if (!SongsClass.Title.Contains(".mp3"))
                        filename = SongsClass.Title + ".mp3";

                    string mediaFile = filePath + "/" + filename;

                    if (System.IO.File.Exists(mediaFile))
                    {
                        File file2 = new File(mediaFile);
                        var uri = FileProvider.GetUriForFile(Activity, Activity?.PackageName + ".fileprovider", file2);

                        if (CheckSystemWritePermission())
                        {
                            ContentValues values = new ContentValues();
                            values.Put("_data", uri.ToString());
                            values.Put("title", SongsClass.Title);
                            //values.Put("_size", 215454);
                            values.Put("mime_type", MimeTypeMap.GetMimeType(SongsClass.AudioLocation.Split('.').LastOrDefault()));
                            values.Put(MediaStore.Audio.IAudioColumns.Artist, DeepSoundTools.GetNameFinal(SongsClass.Publisher));
                            //values.Put(MediaStore.Audio.IAudioColumns.Duration, 230);
                            values.Put(MediaStore.Audio.IAudioColumns.IsRingtone, true);
                            values.Put(MediaStore.Audio.IAudioColumns.IsNotification, false);
                            values.Put(MediaStore.Audio.IAudioColumns.IsAlarm, true);
                            values.Put(MediaStore.Audio.IAudioColumns.IsMusic, false);

                            // Setting ringtone....
                            Activity.ContentResolver?.Delete(MediaStore.Audio.Media.InternalContentUri, "title" + " = \"Sonify\"", null);
                            // To avoid duplicate inserts
                            var ringUri = Activity.ContentResolver?.Insert(MediaStore.Audio.Media.InternalContentUri, values);
                            RingtoneManager.SetActualDefaultRingtoneUri(Activity, RingtoneType.Ringtone, ringUri);

                            Toast.MakeText(Context, Context.GetText(Resource.String.Lbl_SetRingtoneSuccessfully), ToastLength.Short)?.Show();
                        }

                        return;
                    }

                    WebClient webClient = new WebClient();
                    webClient.DownloadDataAsync(new Uri(SongsClass.AudioLocation));
                    webClient.DownloadDataCompleted += (o, args) =>
                    {
                        try
                        {
                            //Downloading Cancelled
                            if (args.Cancelled)
                                return;

                            if (args.Error != null)
                            {
                                Console.WriteLine(args.Error);
                                return;
                            }

                            if (!System.IO.File.Exists(mediaFile))
                            {
                                using FileStream fs = new FileStream(mediaFile, FileMode.CreateNew, FileAccess.Write, FileShare.Read);
                                fs.Write(args.Result, 0, args.Result.Length);
                                 
                                MediaScannerConnection.ScanFile(Activity, new []{ mediaFile }, null, null);
                                //File.WriteAllBytes(mediaFile, e.Result);
                            }

                            File file2 = new File(mediaFile);
                            var uri = FileProvider.GetUriForFile(Activity, Activity?.PackageName + ".fileprovider", file2);

                            if (CheckSystemWritePermission())
                            {
                                ContentValues values = new ContentValues();
                                values.Put("_data", uri.ToString());
                                values.Put("title", SongsClass.Title);
                                //values.Put("_size", 215454);
                                values.Put("mime_type", MimeTypeMap.GetMimeType(SongsClass.AudioLocation.Split('.').LastOrDefault()));
                                values.Put(MediaStore.Audio.IAudioColumns.Artist, DeepSoundTools.GetNameFinal(SongsClass.Publisher));
                                //values.Put(MediaStore.Audio.IAudioColumns.Duration, 230);
                                values.Put(MediaStore.Audio.IAudioColumns.IsRingtone, true);
                                values.Put(MediaStore.Audio.IAudioColumns.IsNotification, false);
                                values.Put(MediaStore.Audio.IAudioColumns.IsAlarm, true);
                                values.Put(MediaStore.Audio.IAudioColumns.IsMusic, false);

                                // Setting ringtone....
                                Activity.ContentResolver?.Delete(MediaStore.Audio.Media.InternalContentUri, "title" + " = \"Sonify\"", null);
                                // To avoid duplicate inserts
                                var ringUri = Activity.ContentResolver?.Insert(MediaStore.Audio.Media.InternalContentUri, values);
                                RingtoneManager.SetActualDefaultRingtoneUri(Activity, RingtoneType.Ringtone, ringUri);

                                Toast.MakeText(Context, Context.GetText(Resource.String.Lbl_SetRingtoneSuccessfully), ToastLength.Short)?.Show();
                            } 
                        }
                        catch (IOException xed)
                        {
                            Methods.DisplayReportResultTrack(xed);
                        }
                        catch (Exception xed)
                        {
                            Methods.DisplayReportResultTrack(xed);
                        }
                    };
                }
                Dismiss();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private bool CheckSystemWritePermission()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
            {
                if (Settings.System.CanWrite(Activity))
                    return true;
                else
                {
                    Intent intent = new Intent(Settings.ActionManageWriteSettings);
                    intent.SetData(Android.Net.Uri.Parse("package:" + Activity?.PackageName));
                    Activity.StartActivity(intent);
                }
            }
            return false;
        }
         
        private void AddToBlackListLayoutOnClick(object sender, EventArgs e)
        {
            try
            {
                ClickListeners.OnNotInterestedSongsClick(new MoreClickEventArgs { Button = IconHeart, SongsClass = SongsClass }, NamePage);
                Toast.MakeText(Context, Context.GetText(Resource.String.Lbl_SongRemoved), ToastLength.Short)?.Show();

                Dismiss();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void ShareLayoutOnClick(object sender, EventArgs e)
        {
            try
            {
                ClickListeners.OnShareClick(new MoreClickEventArgs(){SongsClass = SongsClass});
                Dismiss();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void DeleteLayoutOnClick(object sender, EventArgs e)
        {
            try
            {
                ClickListeners.OnMenuDeleteSongOnClick(new MoreClickEventArgs() { SongsClass = SongsClass });
                Dismiss();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }
         
        #endregion
          
        private void SetData()
        {
            try
            {
                NamePage = Arguments?.GetString("NamePage") ?? "";
                SongsClass = JsonConvert.DeserializeObject<SoundDataObject>(Arguments?.GetString("ItemDataSong") ?? "");
                if (SongsClass != null)
                {
                    if (SongsClass.UserId == UserDetails.UserId && UserDetails.IsLogin)
                    {
                        EditSongLayout.Visibility = ViewStates.Visible;
                        DeleteLayout.Visibility = ViewStates.Visible;
                    }
                    else
                    {
                        EditSongLayout.Visibility = ViewStates.Gone;
                        DeleteLayout.Visibility = ViewStates.Gone;
                    }

                    if (UserDetails.IsLogin)
                    {
                        RePostLayout.Visibility = ViewStates.Visible;
                        ReportLayout.Visibility = ViewStates.Visible;
                        ReportCopyrightLayout.Visibility = ViewStates.Visible;
                    }
                    else
                    {
                        RePostLayout.Visibility = ViewStates.Gone;
                        ReportLayout.Visibility = ViewStates.Gone;
                        ReportCopyrightLayout.Visibility = ViewStates.Gone;
                    }

                    GlideImageLoader.LoadImage(Activity, SongsClass.Thumbnail, Image, ImageStyle.CenterCrop, ImagePlaceholders.Drawable);

                    TxtTitle.Text = Methods.FunString.SubStringCutOf(Methods.FunString.DecodeString(SongsClass.Title), 25);

                    string seconderText;
                    if (SongsClass.Publisher != null)
                        seconderText = DeepSoundTools.GetNameFinal(SongsClass.Publisher);
                    else
                        seconderText = SongsClass.CategoryName + " " + Activity.GetText(Resource.String.Lbl_Music);

                    if (SongsClass.Src == "radio")
                    {

                    }
                    else
                        seconderText += "   |   " + SongsClass.Duration + " " + Activity.GetText(Resource.String.Lbl_CutMinutes);

                    TxtSeconderText.Text = seconderText;

                    if (SongsClass.IsLiked != null && SongsClass.IsLiked.Value)
                    {
                        IconHeart.SetImageResource(Resource.Drawable.icon_heart_filled_post_vector);
                        IconHeart.SetColorFilter(Color.ParseColor("#f55a4e"));
                        IconHeart.Tag = "Liked";
                    }
                    else
                    {
                        IconHeart.SetImageResource(Resource.Drawable.icon_player_heart);
                        IconHeart.ClearColorFilter();
                        IconHeart.Tag = "Like";
                    }
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }
         
    }
}