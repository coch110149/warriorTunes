using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MaterialDialogsCore;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Text;
using Android.Views;
using Android.Widget;
using DeepSound.Activities.Albums;
using DeepSound.Activities.Playlist;
using DeepSound.Activities.Tabbes;
using DeepSound.Activities.Upload;
using DeepSound.Helpers.Controller;
using DeepSound.Helpers.Model;
using DeepSound.Helpers.Utils;
using DeepSound.Library.Anjo.Share;
using DeepSound.Library.Anjo.Share.Abstractions;
using DeepSound.SQLite;
using DeepSoundClient.Classes.Global;
using DeepSoundClient.Requests;
using Newtonsoft.Json;
using ClipboardManager = Android.Content.ClipboardManager;
using Exception = System.Exception;
using Android.Util;
using DeepSound.Activities.Songs;
using Java.Util;

namespace DeepSound.Helpers.MediaPlayerController
{
    public class SocialIoClickListeners : Java.Lang.Object, MaterialDialog.IListCallbackMultiChoice, MaterialDialog.ISingleButtonCallback
    {
        private readonly Activity MainContext;
        private readonly HomeActivity GlobalContext;
        private string TypeDialog, NamePage, TotalIdPlaylistChecked, TotalIdAlbumChecked;
        private MoreClickEventArgs MoreSongArgs;

        public SocialIoClickListeners(Activity context)
        {
            try
            {
                MainContext = context;
                GlobalContext = (HomeActivity)MainContext ?? HomeActivity.GetInstance();
                TypeDialog = string.Empty;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void SetPlaySongs(ImageView playButton , bool isPlay)
        {
            try
            {
                int dimensionInPixel = 35;
                int dimensionInPixel2 = 27;

                if (isPlay)
                {
                    int dimensionInDp = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, dimensionInPixel2, playButton.Context.Resources.DisplayMetrics);
                    playButton.SetImageResource(Resource.Drawable.new_icon_pause);
                    playButton.Tag = "playing";
                    playButton.LayoutParameters.Height = dimensionInDp;
                    playButton.LayoutParameters.Width = dimensionInDp;

                    RelativeLayout.LayoutParams lp = (RelativeLayout.LayoutParams)playButton.LayoutParameters;
                    lp.RightMargin = 30;
                    //lp.MarginEnd = 20;

                    playButton.LayoutParameters = lp;
                    playButton.RequestLayout();
                }
                else
                {
                    playButton.SetImageResource(Resource.Drawable.new_ic_play);

                    int dimensionInDp = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, dimensionInPixel, playButton.Context.Resources.DisplayMetrics);

                    playButton.LayoutParameters.Height = dimensionInDp;
                    playButton.LayoutParameters.Width = dimensionInDp;

                    RelativeLayout.LayoutParams lp = (RelativeLayout.LayoutParams)playButton.LayoutParameters;
                    lp.RightMargin = 26;
                    //lp.MarginEnd = 13;

                    playButton.LayoutParameters = lp;
                    playButton.RequestLayout();
                    playButton.Tag = "play";
                }
            }
            catch (Exception ex)
            {
                Methods.DisplayReportResultTrack(ex);

            }
        }

        //Add Like Or Remove  
        public void OnLikeSongsClick(MoreClickEventArgs e, string name = "")
        {
            try
            {
                if (!UserDetails.IsLogin)
                {
                    PopupDialogController dialog = new PopupDialogController(MainContext, null, "Login");
                    dialog.ShowNormalDialog(MainContext.GetText(Resource.String.Lbl_Login), MainContext.GetText(Resource.String.Lbl_Message_Sorry_signin), MainContext.GetText(Resource.String.Lbl_Yes), MainContext.GetText(Resource.String.Lbl_No));
                    return;
                }

                if (Methods.CheckConnectivity())
                {
                    var refs = SetLike(e.Button);
                    e.SongsClass.IsLiked = refs;

                    //add to Liked
                    if (refs)
                    {
                        ListUtils.LikedSongs.Add(e.SongsClass);
                    }
                    else
                    {
                        var removeItem = ListUtils.LikedSongs.Select(x => x).FirstOrDefault(x => x.Id == e.SongsClass.Id);
                        ListUtils.LikedSongs.Remove(removeItem);
                    }

                    if (name == "AlbumsFragment")
                    {
                        var list = AlbumsFragment.Instance?.MAdapter?.SoundsList;
                        var dataSong = list?.FirstOrDefault(a => a.Id == e.SongsClass.Id);
                        if (dataSong != null)
                        {
                            dataSong.IsLiked = refs;
                            int index = list.IndexOf(dataSong);
                            AlbumsFragment.Instance.MAdapter?.NotifyItemChanged(index);
                        }
                    }
                    else if (name == "PlaylistProfileFragment")
                    {  
                        var list = PlaylistProfileFragment.Instance?.MAdapter?.SoundsList;
                        var dataSong = list?.FirstOrDefault(a => a.Id == e.SongsClass.Id);
                        if (dataSong != null)
                        {
                            dataSong.IsLiked = refs;
                            int index = list.IndexOf(dataSong);
                            PlaylistProfileFragment.Instance?.MAdapter?.NotifyItemChanged(index);
                        }
                    }
                    else if (name == "SongsByGenresFragment")
                    {
                        var list = GlobalContext?.HomeFragment?.LatestHomeTab?.SongsByGenresFragment?.MAdapter?.SoundsList;
                        var dataSong = list?.FirstOrDefault(a => a.Id == e.SongsClass.Id);
                        if (dataSong != null)
                        {
                            dataSong.IsLiked = refs;
                            int index = list.IndexOf(dataSong);
                            GlobalContext?.HomeFragment?.LatestHomeTab?.SongsByGenresFragment?.MAdapter?.NotifyItemChanged(index);
                        }
                    }
                    else if (name == "PlaylistProfileFragment")
                    {
                        var list = GlobalContext?.TrendingFragment?.PlaylistProfileFragment?.MAdapter?.SoundsList;
                        var dataSong = list?.FirstOrDefault(a => a.Id == e.SongsClass.Id);
                        if (dataSong != null)
                        {
                            dataSong.IsLiked = refs;
                            int index = list.IndexOf(dataSong);
                            GlobalContext?.TrendingFragment?.PlaylistProfileFragment?.MAdapter?.NotifyItemChanged(index);
                        }
                    }
                    else if (name == "SearchSongsFragment")
                    {
                        var list = GlobalContext?.HomeFragment?.SearchFragment?.SongsTab?.MAdapter?.SoundsList;
                        var dataSong = list?.FirstOrDefault(a => a.Id == e.SongsClass.Id);
                        if (dataSong != null)
                        {
                            dataSong.IsLiked = refs;
                            int index = list.IndexOf(dataSong);
                            GlobalContext?.HomeFragment?.SearchFragment?.SongsTab?.MAdapter?.NotifyItemChanged(index);
                        }
                    }
                    else if (name == "FavoritesFragment")
                    {
                        var list = GlobalContext?.LibraryFragment.FavoritesFragment?.MAdapter?.SoundsList;
                        var dataSong = list?.FirstOrDefault(a => a.Id == e.SongsClass.Id);
                        if (dataSong != null)
                        {
                            dataSong.IsLiked = refs;
                            int index = list.IndexOf(dataSong);
                            GlobalContext?.LibraryFragment.FavoritesFragment?.MAdapter?.NotifyItemChanged(index);
                        }
                    }
                    else if (name == "LatestDownloadsFragment")
                    {
                        var list = GlobalContext?.LibraryFragment.LatestDownloadsFragment?.MAdapter?.SoundsList;
                        var dataSong = list?.FirstOrDefault(a => a.Id == e.SongsClass.Id);
                        if (dataSong != null)
                        {
                            dataSong.IsLiked = refs;
                            int index = list.IndexOf(dataSong);
                            GlobalContext?.LibraryFragment.LatestDownloadsFragment?.MAdapter?.NotifyItemChanged(index);
                        }
                    }
                    else if (name == "LikedFragment")
                    {
                        var list = GlobalContext?.LibraryFragment.LikedFragment?.MAdapter?.SoundsList;
                        var dataSong = list?.FirstOrDefault(a => a.Id == e.SongsClass.Id);
                        if (dataSong != null)
                        {
                            dataSong.IsLiked = refs;
                            int index = list.IndexOf(dataSong);
                            GlobalContext?.LibraryFragment.LikedFragment?.MAdapter?.NotifyItemChanged(index);
                        }
                    }
                    else if (name == "PurchasesFragment") 
                    {
                        //var list = GlobalContext?.LibraryFragment.PurchasesFragment?.MAdapter?.PurchasesList;
                        //var dataSong = list?.FirstOrDefault(a => a.Id == e.SongsClass.Id.ToString());
                        //if (dataSong != null)
                        //{
                        //    //dataSong.IsLiked = refs;
                        //    int index = list.IndexOf(dataSong);
                        //    GlobalContext?.LibraryFragment.PurchasesFragment?.MAdapter?.NotifyItemChanged(index);
                        //}
                    }
                    else if (name == "RecentlyPlayedFragment")
                    {
                        var list = GlobalContext?.LibraryFragment.RecentlyPlayedFragment?.MAdapter?.SoundsList;
                        var dataSong = list?.FirstOrDefault(a => a.Id == e.SongsClass.Id);
                        if (dataSong != null)
                        {
                            dataSong.IsLiked = refs;
                            int index = list.IndexOf(dataSong);
                            GlobalContext?.LibraryFragment.RecentlyPlayedFragment?.MAdapter?.NotifyItemChanged(index);
                        }
                    }
                    else if (name == "SharedFragment")
                    {
                        var list = GlobalContext?.LibraryFragment.SharedFragment?.MAdapter?.SoundsList;
                        var dataSong = list?.FirstOrDefault(a => a.Id == e.SongsClass.Id);
                        if (dataSong != null)
                        {
                            dataSong.IsLiked = refs;
                            int index = list.IndexOf(dataSong);
                            GlobalContext?.LibraryFragment.SharedFragment?.MAdapter?.NotifyItemChanged(index);
                        }
                    } 
                    else if (name == "SongsByTypeFragment")
                    {
                        var list = SongsByTypeFragment.Instance?.MAdapter?.SoundsList;
                        var dataSong = list?.FirstOrDefault(a => a.Id == e.SongsClass.Id);
                        if (dataSong != null)
                        {
                            dataSong.IsLiked = refs;
                            int index = list.IndexOf(dataSong);
                            SongsByTypeFragment.Instance?.MAdapter?.NotifyItemChanged(index);
                        }
                    }
                    else if (name == "UserLikedFragment")
                    {
                        var list = GlobalContext?.ProfileFragment.LikedFragment.MAdapter?.SoundsList;
                        var dataSong = list?.FirstOrDefault(a => a.Id == e.SongsClass.Id);
                        if (dataSong != null)
                        {
                            dataSong.IsLiked = refs;
                            int index = list.IndexOf(dataSong);
                            GlobalContext?.ProfileFragment.LikedFragment.MAdapter?.NotifyItemChanged(index);
                        }
                    }
                    else if (name == "UserSongsFragment")
                    {
                        var list = GlobalContext?.ProfileFragment.SongsFragment.MAdapter?.SoundsList;
                        var dataSong = list?.FirstOrDefault(a => a.Id == e.SongsClass.Id);
                        if (dataSong != null)
                        {
                            dataSong.IsLiked = refs;
                            int index = list.IndexOf(dataSong);
                            GlobalContext?.ProfileFragment.SongsFragment.MAdapter?.NotifyItemChanged(index);
                        }
                    }

                    //Sent Api
                    PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Tracks.LikeUnLikeTrackAsync(e.SongsClass.AudioId) });
                }
                else
                {
                    Toast.MakeText(MainContext, MainContext.GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)?.Show();
                }

            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //Add NotInterested  
        public void OnNotInterestedSongsClick(MoreClickEventArgs e, string name = "")
        {
            try
            {
                if (!UserDetails.IsLogin)
                {
                    PopupDialogController dialog = new PopupDialogController(MainContext, null, "Login");
                    dialog.ShowNormalDialog(MainContext.GetText(Resource.String.Lbl_Login), MainContext.GetText(Resource.String.Lbl_Message_Sorry_signin), MainContext.GetText(Resource.String.Lbl_Yes), MainContext.GetText(Resource.String.Lbl_No));
                    return;
                }

                if (Methods.CheckConnectivity())
                {
                    var data = ListUtils.GlobalNotInterestedList.FirstOrDefault(a => a.Id == e.SongsClass.Id);
                    if (data == null)
                    {
                        ListUtils.GlobalNotInterestedList.Add(e.SongsClass);
                    }

                    var sqlEntity = new SqLiteDatabase();
                    sqlEntity.Insert_NotInterestedSound(e.SongsClass);

                    if (name == "AlbumsFragment")
                    {
                        var list = AlbumsFragment.Instance?.MAdapter;
                        var dataSong = list?.SoundsList?.FirstOrDefault(a => a.Id == e.SongsClass.Id);
                        if (dataSong != null)
                        {
                            int index = list.SoundsList.IndexOf(dataSong); 
                            list.SoundsList.Remove(dataSong); 
                            AlbumsFragment.Instance.MAdapter?.NotifyItemChanged(index);
                        }
                    }
                    else if (name == "PlaylistProfileFragment")
                    {  
                        var list = PlaylistProfileFragment.Instance?.MAdapter;
                        var dataSong = list?.SoundsList?.FirstOrDefault(a => a.Id == e.SongsClass.Id);
                        if (dataSong != null)
                        {
                            int index = list.SoundsList.IndexOf(dataSong);
                            list.SoundsList.Remove(dataSong);
                            PlaylistProfileFragment.Instance?.MAdapter?.NotifyItemChanged(index);
                        }
                    }
                    else if (name == "SongsByGenresFragment")
                    {
                        var list = GlobalContext?.HomeFragment?.LatestHomeTab?.SongsByGenresFragment?.MAdapter;
                        var dataSong = list?.SoundsList?.FirstOrDefault(a => a.Id == e.SongsClass.Id);
                        if (dataSong != null)
                        {
                            int index = list.SoundsList.IndexOf(dataSong);
                            list.SoundsList.Remove(dataSong);
                            GlobalContext?.HomeFragment?.LatestHomeTab?.SongsByGenresFragment?.MAdapter?.NotifyItemChanged(index);
                        }
                    }
                    else if (name == "PlaylistProfileFragment")
                    {
                        var list = GlobalContext?.TrendingFragment?.PlaylistProfileFragment?.MAdapter;
                        var dataSong = list?.SoundsList?.FirstOrDefault(a => a.Id == e.SongsClass.Id);
                        if (dataSong != null)
                        {
                            int index = list.SoundsList.IndexOf(dataSong);
                            list.SoundsList.Remove(dataSong);
                            GlobalContext?.TrendingFragment?.PlaylistProfileFragment?.MAdapter?.NotifyItemChanged(index);
                        }
                    }
                    else if (name == "SearchSongsFragment")
                    {
                        var list = GlobalContext?.HomeFragment?.SearchFragment?.SongsTab?.MAdapter;
                        var dataSong = list?.SoundsList?.FirstOrDefault(a => a.Id == e.SongsClass.Id);
                        if (dataSong != null)
                        {
                            int index = list.SoundsList.IndexOf(dataSong);
                            list.SoundsList.Remove(dataSong);
                            GlobalContext?.HomeFragment?.SearchFragment?.SongsTab?.MAdapter?.NotifyItemChanged(index);
                        }
                    }
                    else if (name == "FavoritesFragment")
                    {
                        var list = GlobalContext?.LibraryFragment.FavoritesFragment?.MAdapter;
                        var dataSong = list?.SoundsList?.FirstOrDefault(a => a.Id == e.SongsClass.Id);
                        if (dataSong != null)
                        {
                            int index = list.SoundsList.IndexOf(dataSong);
                            list.SoundsList.Remove(dataSong);
                            GlobalContext?.LibraryFragment.FavoritesFragment?.MAdapter?.NotifyItemChanged(index);
                        }
                    }
                    else if (name == "LatestDownloadsFragment")
                    {
                        var list = GlobalContext?.LibraryFragment.LatestDownloadsFragment?.MAdapter;
                        var dataSong = list?.SoundsList?.FirstOrDefault(a => a.Id == e.SongsClass.Id);
                        if (dataSong != null)
                        {
                            int index = list.SoundsList.IndexOf(dataSong);
                            list.SoundsList.Remove(dataSong);
                            GlobalContext?.LibraryFragment.LatestDownloadsFragment?.MAdapter?.NotifyItemChanged(index);
                        }
                    }
                    else if (name == "LikedFragment")
                    {
                        var list = GlobalContext?.LibraryFragment.LikedFragment?.MAdapter;
                        var dataSong = list?.SoundsList?.FirstOrDefault(a => a.Id == e.SongsClass.Id);
                        if (dataSong != null)
                        {
                            int index = list.SoundsList.IndexOf(dataSong);
                            list.SoundsList.Remove(dataSong);
                            GlobalContext?.LibraryFragment.LikedFragment?.MAdapter?.NotifyItemChanged(index);
                        }
                    }
                    else if (name == "PurchasesFragment") 
                    {
                        var list = GlobalContext?.LibraryFragment.PurchasesFragment?.MAdapter?.PurchasesList;
                        var dataSong = list?.FirstOrDefault(a => a.Id == e.SongsClass.Id.ToString());
                        if (dataSong != null)
                        {
                            //dataSong.IsLiked = refs;
                            int index = list.IndexOf(dataSong);
                            GlobalContext?.LibraryFragment.PurchasesFragment?.MAdapter?.NotifyItemChanged(index);
                        }
                    }
                    else if (name == "RecentlyPlayedFragment")
                    {
                        var list = GlobalContext?.LibraryFragment.RecentlyPlayedFragment?.MAdapter;
                        var dataSong = list?.SoundsList?.FirstOrDefault(a => a.Id == e.SongsClass.Id);
                        if (dataSong != null)
                        {
                            int index = list.SoundsList.IndexOf(dataSong);
                            list.SoundsList.Remove(dataSong);
                            GlobalContext?.LibraryFragment.RecentlyPlayedFragment?.MAdapter?.NotifyItemChanged(index);
                        }
                    }
                    else if (name == "SharedFragment")
                    {
                        var list = GlobalContext?.LibraryFragment.SharedFragment?.MAdapter;
                        var dataSong = list?.SoundsList?.FirstOrDefault(a => a.Id == e.SongsClass.Id);
                        if (dataSong != null)
                        {
                            int index = list.SoundsList.IndexOf(dataSong);
                            list.SoundsList.Remove(dataSong);
                            GlobalContext?.LibraryFragment.SharedFragment?.MAdapter?.NotifyItemChanged(index);
                        }
                    }
                    else if (name == "SongsByTypeFragment")
                    {
                        var list = SongsByTypeFragment.Instance?.MAdapter;
                        var dataSong = list?.SoundsList?.FirstOrDefault(a => a.Id == e.SongsClass.Id);
                        if (dataSong != null)
                        {
                            int index = list.SoundsList.IndexOf(dataSong);
                            list.SoundsList.Remove(dataSong);
                            SongsByTypeFragment.Instance?.MAdapter?.NotifyItemChanged(index);
                        }
                    }
                    else if (name == "UserLikedFragment")
                    {
                        var list = GlobalContext?.ProfileFragment.LikedFragment.MAdapter;
                        var dataSong = list?.SoundsList?.FirstOrDefault(a => a.Id == e.SongsClass.Id);
                        if (dataSong != null)
                        {
                            int index = list.SoundsList.IndexOf(dataSong);
                            list.SoundsList.Remove(dataSong);
                            GlobalContext?.ProfileFragment.LikedFragment.MAdapter?.NotifyItemChanged(index);
                        }
                    }
                    else if (name == "UserSongsFragment")
                    {
                        var list = GlobalContext?.ProfileFragment.SongsFragment.MAdapter;
                        var dataSong = list?.SoundsList?.FirstOrDefault(a => a.Id == e.SongsClass.Id);
                        if (dataSong != null)
                        {
                            int index = list.SoundsList.IndexOf(dataSong);
                            list.SoundsList.Remove(dataSong);
                            GlobalContext?.ProfileFragment.SongsFragment.MAdapter?.NotifyItemChanged(index);
                        }
                    }

                    //Sent Api after add new api
                    //PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Tracks.LikeUnLikeTrackAsync(e.SongsClass.AudioId) });
                }
                else
                {
                    Toast.MakeText(MainContext, MainContext.GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)?.Show();
                }

            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //Add Dislike Or Remove 
        public void OnDislikeSongsClick(MoreClickEventArgs e, string name = "")
        {
            try
            {
                if (!UserDetails.IsLogin)
                {
                    PopupDialogController dialog = new PopupDialogController(MainContext, null, "Login");
                    dialog.ShowNormalDialog(MainContext.GetText(Resource.String.Lbl_Login), MainContext.GetText(Resource.String.Lbl_Message_Sorry_signin), MainContext.GetText(Resource.String.Lbl_Yes), MainContext.GetText(Resource.String.Lbl_No));
                    return;
                }

                if (Methods.CheckConnectivity())
                {
                    var refs = SetDislike(e.Button);
                    e.SongsClass.IsDisLiked = refs;

                    //add to Disliked
                    //if (refs)
                    //    GlobalContext?.LibrarySynchronizer.AddToLiked(e.SongsClass);

                    if (name == "AlbumsFragment")
                    {
                        var list = AlbumsFragment.Instance?.MAdapter?.SoundsList;
                        var dataSong = list?.FirstOrDefault(a => a.Id == e.SongsClass.Id);
                        if (dataSong != null)
                        {
                            dataSong.IsDisLiked = refs;
                            int index = list.IndexOf(dataSong);
                            AlbumsFragment.Instance?.MAdapter?.NotifyItemChanged(index);
                        }
                    }
                    else if (name == "PlaylistProfileFragment")
                    { 
                        var list = PlaylistProfileFragment.Instance?.MAdapter?.SoundsList;
                        var dataSong = list?.FirstOrDefault(a => a.Id == e.SongsClass.Id);
                        if (dataSong != null)
                        {
                            dataSong.IsDisLiked = refs;
                            int index = list.IndexOf(dataSong);
                            PlaylistProfileFragment.Instance?.MAdapter?.NotifyItemChanged(index);
                        }
                    }

                    //Sent Api 
                    PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Tracks.DislikeUnDislikeTrackAsync(e.SongsClass.AudioId) });
                }
                else
                {
                    Toast.MakeText(MainContext, MainContext.GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)?.Show();
                }

            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //Add Favorite Or Remove 
        public void OnFavoriteSongsClick(MoreClickEventArgs e)
        {
            try
            {
                if (!UserDetails.IsLogin)
                {
                    PopupDialogController dialog = new PopupDialogController(MainContext, null, "Login");
                    dialog.ShowNormalDialog(MainContext.GetText(Resource.String.Lbl_Login), MainContext.GetText(Resource.String.Lbl_Message_Sorry_signin), MainContext.GetText(Resource.String.Lbl_Yes), MainContext.GetText(Resource.String.Lbl_No));
                    return;
                }


                if (Methods.CheckConnectivity())
                {
                    var refs = SetFav(e.Button);
                    e.SongsClass.IsFavoriated = refs;
                     
                    var page = GlobalContext?.FavoritesFragment;
                    if (page?.MAdapter != null)
                    {
                        //add to Favorites
                        if (refs)
                        {
                            var dataSong = page?.MAdapter?.SoundsList?.FirstOrDefault(a => a.Id == e.SongsClass.Id);
                            if (dataSong == null)
                            {
                                page.MAdapter?.SoundsList?.Add(e.SongsClass);
                                page.MAdapter?.NotifyDataSetChanged();

                                ListUtils.FavoritesList.Add(e.SongsClass);
                            }
                        }
                        else
                        {
                            var dataSong = page?.MAdapter?.SoundsList?.FirstOrDefault(a => a.Id == e.SongsClass.Id);
                            if (dataSong != null)
                            {
                                int index = page.MAdapter.SoundsList.IndexOf(dataSong);
                                page.MAdapter.SoundsList.Remove(dataSong);
                                page.MAdapter?.NotifyItemChanged(index);

                                ListUtils.FavoritesList.Remove(dataSong);
                            }
                        }
                    }

                    //Sent Api
                    PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Tracks.AddOrRemoveFavoriteTrackAsync(e.SongsClass.AudioId) });
                }
                else
                {
                    Toast.MakeText(MainContext, MainContext.GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)?.Show();
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //Share
        public async void OnShareClick(MoreClickEventArgs args)
        {
            try
            {
                if (!CrossShare.IsSupported)
                    return;

                await CrossShare.Current.Share(new ShareMessage
                {
                    Title = args.SongsClass.Title,
                    Text = args.SongsClass.Description,
                    Url = args.SongsClass.Url
                });

                SqLiteDatabase dbDatabase = new SqLiteDatabase();
                dbDatabase.InsertOrUpdate_SharedSound(args.SongsClass);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        //Event Show More :  DeleteSong , EditSong , GoSong , Copy Link  , Report .. 
        public void OnMoreClick(MoreClickEventArgs args, string namePage = "")
        {
            try
            {
                NamePage = namePage;
                MoreSongArgs = args;

                Bundle bundle = new Bundle();
                bundle.PutString("NamePage", namePage);
                bundle.PutString("ItemDataSong", JsonConvert.SerializeObject(args.SongsClass));

                SongOptionBottomDialogFragment songOptionBottomDialogFragment = new SongOptionBottomDialogFragment()
                {
                    Arguments = bundle
                };
                songOptionBottomDialogFragment.Show(GlobalContext.SupportFragmentManager, songOptionBottomDialogFragment.Tag); 
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        #region MaterialDialog

        public void OnClick(MaterialDialog p0, DialogAction p1)
        {
            try
            {
                if (p1 == DialogAction.Positive)
                {
                    if (TypeDialog == "AddPlaylist")
                    {
                        TotalIdPlaylistChecked = TotalIdPlaylistChecked.Remove(TotalIdPlaylistChecked.Length - 1, 1);
                        if (Methods.CheckConnectivity())
                        {
                            var item = Constant.ArrayListPlay[Constant.PlayPos];
                            if (item != null)
                            {
                                //Sent Api
                                PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Playlist.AddToPlaylistAsync(item.Id.ToString(), TotalIdPlaylistChecked) });
                            }
                        }
                        else
                        {
                            Toast.MakeText(MainContext, MainContext.GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)?.Show();
                        }
                    }
                    else if (TypeDialog == "AddAlbum")
                    {
                        TotalIdAlbumChecked = TotalIdAlbumChecked.Remove(TotalIdAlbumChecked.Length - 1, 1);
                        if (Methods.CheckConnectivity())
                        {
                            var item = Constant.ArrayListPlay[Constant.PlayPos];
                            if (item != null)
                            {
                                //wael add Api after update 
                                //PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Albums.AddToAlbumAsync(item.Id.ToString(), TotalIdAlbumChecked) });
                            }
                        }
                        else
                        {
                            Toast.MakeText(MainContext, MainContext.GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)?.Show();
                        }
                    } 
                }
                else if (p1 == DialogAction.Neutral)
                {
                    if (TypeDialog == "AddPlaylist")
                    {
                        CreatePlaylistBottomSheet createPlaylistBottomSheet = new CreatePlaylistBottomSheet();
                        createPlaylistBottomSheet.Show(GlobalContext.SupportFragmentManager, createPlaylistBottomSheet.Tag);
                    }
                    else if (TypeDialog == "AddAlbum")
                    {
                        MainContext.StartActivity(new Intent(MainContext, typeof(UploadAlbumActivity)));
                    }
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

        public bool OnSelection(MaterialDialog dialog, int[] which, string[] text)
        {
            try
            {
                if (TypeDialog == "AddPlaylist")
                {
                    var list = ListUtils.PlaylistList;
                    if (list?.Count > 0)
                    {
                        for (int i = 0; i < which.Length; i++)
                        {
                            TotalIdPlaylistChecked += list[i].Id + ",";
                        }
                    }
                }
                else if (TypeDialog == "AddAlbum")
                {
                    var list = ListUtils.AlbumList;
                    if (list?.Count > 0)
                    {
                        for (int i = 0; i < which.Length; i++)
                        {
                            TotalIdAlbumChecked += list[i].Id + ",";
                        }
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

        public bool RemoveDiskSoundFile(string fileName, long id)
        {
            try
            {
                var filePath = "";

                if (Build.VERSION.SdkInt >= BuildVersionCodes.Q)
                {
                    /*
                     * this changes is due to scoped storage introduce in Android 10
                     * https://developer.android.com/preview/privacy/storage
                     */

                    var file = Application.Context.GetExternalFilesDir(Android.OS.Environment.DirectoryMusic);
                    filePath = Methods.MultiMedia.CheckFileIfExits(file + "/" + fileName);
                }
                else
                {
                    filePath = Methods.Path.FolderDcimSound + fileName;
                }

                if (File.Exists(filePath))
                {
                    var sqlEntity = new SqLiteDatabase();
                    sqlEntity.Remove_LatestDownloadsSound(id);

                    File.Delete(filePath);

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
                return false;
            }
        }

        #endregion

        //DeleteSong
        public void OnMenuDeleteSongOnClick(MoreClickEventArgs song)
        {
            try
            {
                if (!UserDetails.IsLogin)
                {
                    PopupDialogController dialog = new PopupDialogController(MainContext, null, "Login");
                    dialog.ShowNormalDialog(MainContext.GetText(Resource.String.Lbl_Login), MainContext.GetText(Resource.String.Lbl_Message_Sorry_signin), MainContext.GetText(Resource.String.Lbl_Yes), MainContext.GetText(Resource.String.Lbl_No));
                    return;
                }
                 
                if (Methods.CheckConnectivity())
                {
                    TypeDialog = "DeleteSong";
                    MoreSongArgs = song;

                    var showDeleteFromCacheDialog = MoreSongArgs.SongsClass.UserId != UserDetails.UserId && AppSettings.AllowDeletingDownloadedSongs && NamePage == "LatestDownloadsFragment";
                    var content = showDeleteFromCacheDialog ? MainContext.GetText(Resource.String.Lbl_Do_You_want_to_remove_Song) : MainContext.GetText(Resource.String.Lbl_AreYouSureDeleteSong);

                    var dialog = new MaterialDialog.Builder(MainContext).Theme(DeepSoundTools.IsTabDark() ? MaterialDialogsTheme.Dark : MaterialDialogsTheme.Light);
                    dialog.Title(MainContext.GetText(Resource.String.Lbl_DeleteSong));
                    dialog.Content(content);
                    dialog.PositiveText(MainContext.GetText(Resource.String.Lbl_Yes)).OnPositive((materialDialog, action) =>
                    {
                        try
                        {
                            MainContext?.RunOnUiThread(() =>
                            {
                                try
                                {
                                    if (Methods.CheckConnectivity())
                                    {
                                        SoundDataObject dataSong = null!;
                                        dynamic mAdapter = null!;

                                        switch (NamePage)
                                        {
                                            //Delete Song from list
                                            case "FavoritesFragment":
                                                dataSong = GlobalContext?.LibraryFragment?.FavoritesFragment?.MAdapter?.SoundsList?.FirstOrDefault(a => a.Id == MoreSongArgs.SongsClass.Id);
                                                mAdapter = GlobalContext?.LibraryFragment?.FavoritesFragment?.MAdapter;
                                                break;
                                            case "LatestDownloadsFragment":
                                                dataSong = GlobalContext?.LibraryFragment?.LatestDownloadsFragment?.MAdapter?.SoundsList?.FirstOrDefault(a => a.Id == MoreSongArgs.SongsClass.Id);
                                                mAdapter = GlobalContext?.LibraryFragment?.LatestDownloadsFragment?.MAdapter;
                                                break;
                                            case "LikedFragment":
                                                dataSong = GlobalContext?.LibraryFragment?.LikedFragment?.MAdapter?.SoundsList?.FirstOrDefault(a => a.Id == MoreSongArgs.SongsClass.Id);
                                                mAdapter = GlobalContext?.LibraryFragment?.LikedFragment?.MAdapter;
                                                break;
                                            case "RecentlyPlayedFragment":
                                                dataSong = GlobalContext?.LibraryFragment?.RecentlyPlayedFragment?.MAdapter?.SoundsList?.FirstOrDefault(a => a.Id == MoreSongArgs.SongsClass.Id);
                                                mAdapter = GlobalContext?.LibraryFragment?.RecentlyPlayedFragment?.MAdapter;
                                                break;
                                            case "SharedFragment":
                                                dataSong = GlobalContext?.LibraryFragment?.SharedFragment?.MAdapter?.SoundsList?.FirstOrDefault(a => a.Id == MoreSongArgs.SongsClass.Id);
                                                mAdapter = GlobalContext?.LibraryFragment?.SharedFragment?.MAdapter;
                                                break;
                                            case "PurchasesFragment":
                                                //dataSong = GlobalContext?.LibraryFragment?.PurchasesFragment?.MAdapter?.SoundsList?.FirstOrDefault(a => a.Id == MoreSongArgs.SongsClass.Id);
                                                //mAdapter = GlobalContext?.LibraryFragment?.PurchasesFragment?.MAdapter;
                                                break;
                                            case "SongsByGenresFragment":
                                                dataSong = GlobalContext?.HomeFragment?.LatestHomeTab?.SongsByGenresFragment?.MAdapter?.SoundsList?.FirstOrDefault(a => a.Id == MoreSongArgs.SongsClass.Id);
                                                mAdapter = GlobalContext?.HomeFragment?.LatestHomeTab?.SongsByGenresFragment?.MAdapter;
                                                break;
                                            case "SongsByTypeFragment"://wael
                                                dataSong = SongsByTypeFragment.Instance?.MAdapter?.SoundsList?.FirstOrDefault(a => a.Id == MoreSongArgs.SongsClass.Id);
                                                mAdapter = SongsByTypeFragment.Instance?.MAdapter;
                                                break;
                                            case "SearchSongsFragment":
                                                dataSong = GlobalContext?.TrendingFragment?.SearchFragment?.SongsTab?.MAdapter?.SoundsList?.FirstOrDefault(a => a.Id == MoreSongArgs.SongsClass.Id);
                                                mAdapter = GlobalContext?.TrendingFragment?.SearchFragment?.SongsTab?.MAdapter;
                                                break;
                                        }

                                        if (mAdapter != null && dataSong != null)
                                        {
                                            int index = mAdapter.SoundsList.IndexOf(dataSong);
                                            mAdapter.SoundsList.Remove(dataSong);

                                            if (index >= 0)
                                            {
                                                mAdapter.NotifyItemRemoved(index);
                                            }
                                        }

                                        Toast.MakeText(MainContext, MainContext.GetText(Resource.String.Lbl_SongSuccessfullyDeleted), ToastLength.Short)?.Show();

                                        var showingDeleteOptionForOtherUsers = MoreSongArgs.SongsClass.UserId != UserDetails.UserId &&
                                                                            AppSettings.AllowDeletingDownloadedSongs &&
                                                                            NamePage == "LatestDownloadsFragment";
                                        if (showingDeleteOptionForOtherUsers)
                                        {
                                            RemoveDiskSoundFile(dataSong.Title, dataSong.Id);
                                        }
                                        else
                                        {
                                            //Sent Api >>
                                            PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Tracks.DeleteTrackAsync(MoreSongArgs.SongsClass.Id.ToString()) });
                                        }
                                    }
                                    else
                                    {
                                        Toast.MakeText(MainContext, MainContext.GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)?.Show();
                                    }
                                }
                                catch (Exception e)
                                {
                                    Methods.DisplayReportResultTrack(e);
                                }
                            });
                        }
                        catch (Exception e)
                        {
                            Methods.DisplayReportResultTrack(e);
                        }
                    });
                    dialog.NegativeText(MainContext.GetText(Resource.String.Lbl_No)).OnNegative(new MyMaterialDialog());
                    dialog.AlwaysCallSingleChoiceCallback();
                    dialog.Build().Show();
                }
                else
                {
                    Toast.MakeText(MainContext, MainContext.GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)?.Show();
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        //Edit Song 
        public void OnMenuEditSongOnClick(MoreClickEventArgs song)
        {
            try
            {
                if (!UserDetails.IsLogin)
                {
                    PopupDialogController dialog = new PopupDialogController(MainContext, null, "Login");
                    dialog.ShowNormalDialog(MainContext.GetText(Resource.String.Lbl_Login), MainContext.GetText(Resource.String.Lbl_Message_Sorry_signin), MainContext.GetText(Resource.String.Lbl_Yes), MainContext.GetText(Resource.String.Lbl_No));
                    return;
                }
                 
                Intent intent = new Intent(MainContext, typeof(EditSongActivity));
                intent.PutExtra("ItemDataSong", JsonConvert.SerializeObject(song.SongsClass));
                intent.PutExtra("NamePage", NamePage);
                MainContext.StartActivity(intent);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        //RePost
        public void OnMenuRePostOnClick(MoreClickEventArgs song)
        {
            try
            {
                if (!UserDetails.IsLogin)
                {
                    PopupDialogController dialog = new PopupDialogController(MainContext, null, "Login");
                    dialog.ShowNormalDialog(MainContext.GetText(Resource.String.Lbl_Login), MainContext.GetText(Resource.String.Lbl_Message_Sorry_signin), MainContext.GetText(Resource.String.Lbl_Yes), MainContext.GetText(Resource.String.Lbl_No));
                    return;
                }

                if (Methods.CheckConnectivity())
                {
                    Toast.MakeText(MainContext, MainContext.GetText(Resource.String.Lbl_RePost_Message), ToastLength.Short)?.Show();
                    //Sent Api >>
                    PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Tracks.RePostTrackAsync(song.SongsClass.Id.ToString()) });
                }
                else
                {
                    Toast.MakeText(MainContext, MainContext.GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)?.Show();
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }
         
        //ReportSong
        public void OnMenuReportSongOnClick(MoreClickEventArgs song)
        {
            try
            {
                if (!UserDetails.IsLogin)
                {
                    PopupDialogController dialog = new PopupDialogController(MainContext, null, "Login");
                    dialog.ShowNormalDialog(MainContext.GetText(Resource.String.Lbl_Login), MainContext.GetText(Resource.String.Lbl_Message_Sorry_signin), MainContext.GetText(Resource.String.Lbl_Yes), MainContext.GetText(Resource.String.Lbl_No));
                    return;
                }

                var dialogBuilder = new MaterialDialog.Builder(MainContext).Theme(DeepSoundTools.IsTabDark() ? MaterialDialogsTheme.Dark : MaterialDialogsTheme.Light);
                dialogBuilder.Title(Resource.String.Lbl_ReportSong).TitleColorRes(Resource.Color.primary);
                dialogBuilder.Input(0, 0, false, (materialDialog, s) =>
                {
                    try
                    {
                        if (s.Length <= 0) return;

                        if (Methods.CheckConnectivity())
                        {
                            Toast.MakeText(MainContext, MainContext.GetText(Resource.String.Lbl_YourReportSong), ToastLength.Short)?.Show();
                            //Sent Api >>
                            PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Tracks.ReportUnReportTrackAsync(song.SongsClass.Id.ToString(), s.ToString(), true) });


                        }
                        else
                        {
                            Toast.MakeText(MainContext, MainContext.GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)?.Show();
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e); 
                    }
                });
                dialogBuilder.InputType(InputTypes.TextFlagImeMultiLine);
                dialogBuilder.PositiveText(MainContext.GetText(Resource.String.Lbl_Submit)).OnPositive(new MyMaterialDialog());
                dialogBuilder.NegativeText(MainContext.GetText(Resource.String.Lbl_Cancel)).OnNegative(new MyMaterialDialog());
                dialogBuilder.AlwaysCallSingleChoiceCallback();
                dialogBuilder.Build().Show(); 
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }
         
        //Report Copyright Song
        public void OnMenuReportCopyrightSongOnClick(MoreClickEventArgs song)
        {
            try
            {
                if (!UserDetails.IsLogin)
                {
                    PopupDialogController dialog = new PopupDialogController(MainContext, null, "Login");
                    dialog.ShowNormalDialog(MainContext.GetText(Resource.String.Lbl_Login), MainContext.GetText(Resource.String.Lbl_Message_Sorry_signin), MainContext.GetText(Resource.String.Lbl_Yes), MainContext.GetText(Resource.String.Lbl_No));
                    return;
                }

                var dialogBuilder = new MaterialDialog.Builder(MainContext).Theme(DeepSoundTools.IsTabDark() ? MaterialDialogsTheme.Dark : MaterialDialogsTheme.Light);
                dialogBuilder.Title(Resource.String.Lbl_ReportCopyright).TitleColorRes(Resource.Color.primary);
                dialogBuilder.Input(0, 0, false, (materialDialog, s) =>
                {
                    try
                    {
                        if (s.Length <= 0) return;

                        if (!materialDialog.PromptCheckBoxChecked)
                        {
                            Toast.MakeText(MainContext, MainContext.GetText(Resource.String.Lbl_copyright_Error), ToastLength.Short)?.Show();
                        }
                        else
                        {
                            if (Methods.CheckConnectivity())
                            {
                                Toast.MakeText(MainContext, MainContext.GetText(Resource.String.Lbl_YourReportSong), ToastLength.Short)?.Show();
                                //Sent Api >>
                                PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Common.CreateCopyrightAsync(song.SongsClass.Id.ToString(), s.ToString()) });
                            }
                            else
                            {
                                Toast.MakeText(MainContext, MainContext.GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)?.Show();
                            }
                        } 
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                });
                dialogBuilder.InputType(InputTypes.TextFlagImeMultiLine);
                dialogBuilder.CheckBoxPromptRes(Resource.String.Lbl_copyright_Des, false, null);
                dialogBuilder.PositiveText(MainContext.GetText(Resource.String.Lbl_Submit)).OnPositive(new MyMaterialDialog());
                dialogBuilder.NegativeText(MainContext.GetText(Resource.String.Lbl_Cancel)).OnNegative(new MyMaterialDialog());
                dialogBuilder.AlwaysCallSingleChoiceCallback();
                dialogBuilder.Build().Show();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        //Copy
        public void OnMenuCopyOnClick(string urlClipboard)
        {
            try
            {
                ClipboardManager clipboard = (ClipboardManager)MainContext.GetSystemService(Context.ClipboardService);
                ClipData clip = ClipData.NewPlainText("clipboard", urlClipboard);
                clipboard.PrimaryClip = clip;

                Toast.MakeText(MainContext, MainContext.GetText(Resource.String.Lbl_Text_copied), ToastLength.Short)?.Show();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        //Add To Playlist
        public void AddPlaylistOnClick(MoreClickEventArgs song)
        {
            try
            {
                if (!UserDetails.IsLogin)
                {
                    PopupDialogController dialog = new PopupDialogController(MainContext, null, "Login");
                    dialog.ShowNormalDialog(MainContext.GetText(Resource.String.Lbl_Login), MainContext.GetText(Resource.String.Lbl_Message_Sorry_signin), MainContext.GetText(Resource.String.Lbl_Yes), MainContext.GetText(Resource.String.Lbl_No));
                    return;
                }

                TypeDialog = "AddPlaylist";

                var arrayAdapter = new List<string>();
                var arrayIndexAdapter = new int[] { };
                var dialogList = new MaterialDialog.Builder(MainContext).Theme(DeepSoundTools.IsTabDark() ? MaterialDialogsTheme.Dark : MaterialDialogsTheme.Light);

                if (ListUtils.PlaylistList?.Count > 0) arrayAdapter.AddRange(ListUtils.PlaylistList.Select(playlistDataObject => Methods.FunString.DecodeString(playlistDataObject.Name)));

                dialogList.Title(MainContext.GetText(Resource.String.Lbl_SelectPlaylist))
                    .Items(arrayAdapter)
                    .ItemsCallbackMultiChoice(arrayIndexAdapter, this)
                    .AlwaysCallMultiChoiceCallback()
                    .NegativeText(MainContext.GetText(Resource.String.Lbl_Close)).OnNegative(this)
                    .PositiveText(MainContext.GetText(Resource.String.Lbl_Done)).OnPositive(new MyMaterialDialog())
                    .NeutralText(MainContext.GetText(Resource.String.Lbl_Create)).OnNeutral(new MyMaterialDialog())
                    .Build().Show();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //Add To Album
        public void AddAlbumOnClick(MoreClickEventArgs song)
        {
            try
            {
                if (!UserDetails.IsLogin)
                {
                    PopupDialogController dialog = new PopupDialogController(MainContext, null, "Login");
                    dialog.ShowNormalDialog(MainContext.GetText(Resource.String.Lbl_Login), MainContext.GetText(Resource.String.Lbl_Message_Sorry_signin), MainContext.GetText(Resource.String.Lbl_Yes), MainContext.GetText(Resource.String.Lbl_No));
                    return;
                }

                TypeDialog = "AddAlbum";
                var arrayAdapter = new List<string>();
                var arrayIndexAdapter = new int[] { };
                var dialogList = new MaterialDialog.Builder(MainContext).Theme(DeepSoundTools.IsTabDark() ? MaterialDialogsTheme.Dark : MaterialDialogsTheme.Light);

                if (ListUtils.AlbumList?.Count > 0) arrayAdapter.AddRange(ListUtils.AlbumList.Select(albumDataObject => Methods.FunString.DecodeString(albumDataObject.Title)));

                dialogList.Title(MainContext.GetText(Resource.String.Lbl_SelectAlbum))
                    .Items(arrayAdapter)
                    .ItemsCallbackMultiChoice(arrayIndexAdapter, this)
                    .AlwaysCallMultiChoiceCallback()
                    .NegativeText(MainContext.GetText(Resource.String.Lbl_Close)).OnNegative(this)
                    .PositiveText(MainContext.GetText(Resource.String.Lbl_Done)).OnPositive(new MyMaterialDialog())
                    .NeutralText(MainContext.GetText(Resource.String.Lbl_Create)).OnNeutral(new MyMaterialDialog())
                    .Build().Show();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        public bool SetLike(ImageView likeButton)
        {
            try
            {
                if (likeButton?.Tag?.ToString() == "Liked")
                {
                    likeButton.SetImageResource(Resource.Drawable.icon_player_heart);
                    likeButton.ClearColorFilter();
                    likeButton.Tag = "Like";
                    return false;
                }
                else
                {
                    likeButton.SetImageResource(Resource.Drawable.icon_heart_filled_post_vector);
                    likeButton.SetColorFilter(Color.ParseColor("#f55a4e"));
                    likeButton.Tag = "Liked";
                    return true;
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
                return false;
            }
        }

        public bool SetDislike(ImageView likeButton)
        {
            try
            {
                if (likeButton?.Tag?.ToString() == "Disliked")
                {
                    likeButton.SetImageResource(Resource.Drawable.icon_player_dislike);
                    likeButton.SetColorFilter(Color.Argb(255, 255, 255, 255));
                    likeButton.Tag = "Dislike";
                    return false;
                }
                else
                {
                    likeButton.SetImageResource(Resource.Drawable.icon_player_dislike);
                    likeButton.SetColorFilter(Color.ParseColor("#f55a4e"));
                    likeButton.Tag = "Disliked";
                    return true;
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
                return false;
            }
        }

        public bool SetFav(ImageView favButton)
        {
            try
            {
                if (favButton?.Tag?.ToString() == "Added")
                {
                    favButton.SetImageResource(Resource.Drawable.icon_player_star);
                    favButton.SetColorFilter(Color.Argb(255, 255, 255, 255));
                    favButton.Tag = "Add";
                    return false;
                }
                else
                {
                    favButton.SetImageResource(Resource.Drawable.icon_star_filled_post_vector);
                    favButton.SetColorFilter(Color.ParseColor("#ffa142"));
                    favButton.Tag = "Added";
                    return true;
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
                return false;
            }
        } 
    }

    public class MoreClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public SoundDataObject SongsClass { get; set; }

        public ImageView Button { get; set; }
    }  
}