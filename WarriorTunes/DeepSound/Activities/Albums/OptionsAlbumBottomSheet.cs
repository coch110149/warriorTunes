using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using DeepSound.Activities.Tabbes;
using DeepSound.Adapters;
using DeepSound.Helpers.CacheLoaders;
using DeepSound.Helpers.Controller;
using DeepSound.Helpers.Model;
using DeepSound.Helpers.Utils;
using DeepSound.Library.Anjo.Share.Abstractions;
using DeepSound.Library.Anjo.Share;
using DeepSoundClient.Classes.Albums;
using DeepSoundClient.Requests;
using Google.Android.Material.BottomSheet;
using MaterialDialogsCore;
using Newtonsoft.Json;
using Exception = System.Exception;

namespace DeepSound.Activities.Albums
{
    public class OptionsAlbumBottomSheet : BottomSheetDialogFragment, MaterialDialog.ISingleButtonCallback
    {
        #region Variables Basic

        private HomeActivity GlobalContext;
        private ImageView Image, IconHeart;
        private TextView TxtTitle, TxtSeconderText;

        private RecyclerView MRecycler;
        private LinearLayoutManager LayoutManager;
        private ItemOptionAdapter MAdapter;

        private DataAlbumsObject AlbumsObject;

        #endregion

        #region General

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your fragment here
            GlobalContext = (HomeActivity)Activity;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            try
            {
                Context contextThemeWrapper = DeepSoundTools.IsTabDark() ? new ContextThemeWrapper(Activity, Resource.Style.MyTheme_Dark) : new ContextThemeWrapper(Activity, Resource.Style.MyTheme);
                // clone the inflater using the ContextThemeWrapper
                LayoutInflater localInflater = inflater.CloneInContext(contextThemeWrapper);
                View view = localInflater?.Inflate(Resource.Layout.BottomSheetDefaultLayout, container, false);
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
                InitComponent(view);
                SetRecyclerViewAdapters(view);

                LoadDataChat();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
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
                IconHeart.Visibility = ViewStates.Gone;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void SetRecyclerViewAdapters(View view)
        {
            try
            {
                MRecycler = (RecyclerView)view.FindViewById(Resource.Id.recyler);

                MAdapter = new ItemOptionAdapter(Activity)
                {
                    ItemOptionList = new ObservableCollection<Classes.ItemOptionObject>()
                };
                MAdapter.ItemClick += MAdapterItemClick;
                LayoutManager = new LinearLayoutManager(Context);
                MRecycler.SetLayoutManager(LayoutManager);
                MRecycler.SetAdapter(MAdapter);
                MRecycler.HasFixedSize = true;
                MRecycler.SetItemViewCacheSize(50);
                MRecycler.GetLayoutManager().ItemPrefetchEnabled = true;
                MRecycler.GetRecycledViewPool().Clear();
                MRecycler.SetAdapter(MAdapter);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region Event

        private void MAdapterItemClick(object sender, ItemOptionAdapterClickEventArgs e)
        {
            try
            {
                var position = e.Position;
                if (position > -1)
                {
                    var item = MAdapter.GetItem(position);
                    if (item?.Id == "1") //Edit
                    {
                        OnMenuEditAlbumOnClick();
                        Dismiss();
                    }
                    else if (item?.Id == "2") //Delete
                    {
                        OnMenuDeleteAlbumOnClick();
                    }
                    else if (item?.Id == "3") //Share
                    {
                        ShareAlbum();
                        Dismiss();
                    }
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private async void ShareAlbum()
        {
            try
            {
                //Share Plugin same as Song
                if (!CrossShare.IsSupported)
                {
                    return;
                }

                await CrossShare.Current.Share(new ShareMessage
                {
                    Title = AlbumsObject?.Title,
                    Text = "",
                    Url = AlbumsObject?.Url
                });
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void OnMenuDeleteAlbumOnClick()
        {
            try
            {
                if (!UserDetails.IsLogin)
                {
                    PopupDialogController dialog = new PopupDialogController(Activity, null, "Login");
                    dialog.ShowNormalDialog(Activity.GetText(Resource.String.Lbl_Login), Activity.GetText(Resource.String.Lbl_Message_Sorry_signin), Activity.GetText(Resource.String.Lbl_Yes), Activity.GetText(Resource.String.Lbl_No));
                    return;
                }

                if (Methods.CheckConnectivity())
                {
                    var dialog = new MaterialDialog.Builder(Activity).Theme(DeepSoundTools.IsTabDark() ? MaterialDialogsTheme.Dark : MaterialDialogsTheme.Light);
                    dialog.Title(GlobalContext.GetText(Resource.String.Lbl_DeleteAlbum));
                    dialog.Content(GlobalContext.GetText(Resource.String.Lbl_AreYouSureDeleteAlbum));
                    dialog.PositiveText(Activity.GetText(Resource.String.Lbl_YesButKeepSongs)).OnPositive(this);
                    dialog.NegativeText(Activity.GetText(Resource.String.Lbl_YesDeleteEverything)).OnNegative(this);
                    dialog.NegativeText(Activity.GetText(Resource.String.Lbl_No)).OnNegative(new MyMaterialDialog());
                    dialog.AlwaysCallSingleChoiceCallback();
                    dialog.Build().Show();
                }
                else
                {
                    Toast.MakeText(Activity, Activity.GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)?.Show();
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void OnMenuEditAlbumOnClick()
        {
            try
            {
                if (!UserDetails.IsLogin)
                {
                    PopupDialogController dialog = new PopupDialogController(Activity, null, "Login");
                    dialog.ShowNormalDialog(Activity.GetText(Resource.String.Lbl_Login), Activity.GetText(Resource.String.Lbl_Message_Sorry_signin), Activity.GetText(Resource.String.Lbl_Yes), Activity.GetText(Resource.String.Lbl_No));
                    return;
                }

                //Bundle bundle = new Bundle();
                //bundle.PutString("ItemData", JsonConvert.SerializeObject(AlbumsObject));
                //bundle.PutString("AlbumId", AlbumsObject.Id.ToString());

                //EditAlbumBottomSheet editAlbumBottomSheet = new EditAlbumBottomSheet()
                //{
                //    Arguments = bundle
                //};
                //editAlbumBottomSheet.Show(GlobalContext.SupportFragmentManager, editAlbumBottomSheet.Tag);

                Intent intent = new Intent(GlobalContext, typeof(EditAlbumActivity));
                intent.PutExtra("ItemData", JsonConvert.SerializeObject(AlbumsObject));
                GlobalContext.StartActivity(intent);
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        #endregion


        public void OnClick(MaterialDialog p0, DialogAction p1)
        {
            try
            {
                if (p1 == DialogAction.Positive)
                {
                    var dataAlbumFragment = GlobalContext?.HomeFragment?.LatestHomeTab?.AlbumsAdapter;
                    var list2 = dataAlbumFragment?.AlbumsList;
                    var dataMyAlbum = list2?.FirstOrDefault(a => a.Id == AlbumsObject?.Id);
                    if (dataMyAlbum != null)
                    {
                        int index = list2.IndexOf(dataMyAlbum);
                        if (index >= 0)
                        {
                            list2?.Remove(dataMyAlbum);
                            dataAlbumFragment?.NotifyItemRemoved(index);
                        }
                    }

                    Toast.MakeText(GlobalContext, GlobalContext.GetText(Resource.String.Lbl_AlbumSuccessfullyDeleted), ToastLength.Short)?.Show();

                    //Sent Api >>
                    PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Albums.DeleteAlbumAsync("single", AlbumsObject?.Id.ToString()) });
                    Dismiss();
                }
                else if (p1 == DialogAction.Negative) //Yes, Delete Everything
                {
                    var dataAlbumFragment = GlobalContext?.HomeFragment?.LatestHomeTab?.AlbumsAdapter;
                    var list2 = dataAlbumFragment?.AlbumsList;
                    var dataMyAlbum = list2?.FirstOrDefault(a => a.Id == AlbumsObject?.Id);
                    if (dataMyAlbum != null)
                    {
                        int index = list2.IndexOf(dataMyAlbum);
                        if (index >= 0)
                        {
                            list2?.Remove(dataMyAlbum);
                            dataAlbumFragment?.NotifyItemRemoved(index);
                        }
                    }

                    Toast.MakeText(GlobalContext, GlobalContext.GetText(Resource.String.Lbl_AlbumSuccessfullyDeleted), ToastLength.Short)?.Show();

                    //Sent Api >>
                    PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Albums.DeleteAlbumAsync("all", AlbumsObject?.Id.ToString()) });
                    Dismiss();
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


        private void LoadDataChat()
        {
            try
            {
                AlbumsObject = JsonConvert.DeserializeObject<DataAlbumsObject>(Arguments?.GetString("ItemData") ?? "");
                if (AlbumsObject != null)
                {
                    GlideImageLoader.LoadImage(Activity, AlbumsObject.Thumbnail, Image, ImageStyle.CenterCrop, ImagePlaceholders.Drawable);

                    TxtTitle.Text = Methods.FunString.SubStringCutOf(Methods.FunString.DecodeString(AlbumsObject.Title), 80);

                    var count = !string.IsNullOrEmpty(AlbumsObject.CountSongs) ? AlbumsObject.CountSongs : AlbumsObject.SongsCount ?? "0";
                    var text = count + " " + Context.GetText(Resource.String.Lbl_Songs);
                    if (AppSettings.ShowCountPurchases)
                        text = text + " - " + AlbumsObject.Purchases + " " + Context.GetText(Resource.String.Lbl_Purchases);

                    TxtSeconderText.Text = text;
                     
                    if (AlbumsObject.IsOwner != null && AlbumsObject.IsOwner.Value && UserDetails.IsLogin)
                    {
                        MAdapter.ItemOptionList.Add(new Classes.ItemOptionObject()
                        {
                            Id = "1",
                            Text = GetText(Resource.String.Lbl_EditAlbum),
                            Icon = Resource.Drawable.icon_edit_vector,
                        });

                        MAdapter.ItemOptionList.Add(new Classes.ItemOptionObject()
                        {
                            Id = "2",
                            Text = GetText(Resource.String.Lbl_DeleteAlbum),
                            Icon = Resource.Drawable.icon_delete_vector,
                        });
                    }

                    MAdapter.ItemOptionList.Add(new Classes.ItemOptionObject()
                    {
                        Id = "3",
                        Text = GetText(Resource.String.Lbl_Share),
                        Icon = Resource.Drawable.icon_send_vector,
                    });
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }
    }
}