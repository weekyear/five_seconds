using Android.Content;
using Android.InputMethodServices;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Text;
using Android.Views;
using Android.Views.InputMethods;
using Five_Seconds.CustomControls;
using Five_Seconds.Droid.CustomRenderers;
using Five_Seconds.Views;
using Plugin.CurrentActivity;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using static Android.Support.V7.Widget.SearchView;
using static Android.Views.View;
using SearchView = Android.Support.V7.Widget.SearchView;
using Toolbar = Android.Support.V7.Widget.Toolbar;

[assembly: ExportRenderer(typeof(RecordPage), typeof(SearchPageRenderer))]
namespace Five_Seconds.Droid.CustomRenderers
{
    public class SearchPageRenderer : PageRenderer, IOnFocusChangeListener, IMenuItemOnActionExpandListener
    {
        SearchView SearchView { get; set; }

        public SearchPageRenderer(Context context) : base(context)
        {

        }

        protected override void OnAttachedToWindow()
        {
            base.OnAttachedToWindow();

            if (Element is ISearchPage && Element is Page page && page.Parent is NavigationPage navigationPage)
            {
                //Workaround to re-add the SearchView when navigating back to an ISearchPage, because Xamarin.Forms automatically removes it
                navigationPage.Popped += HandleNavigationPagePopped;
            }
        }

        //Adding the SearchBar in OnSizeChanged ensures the SearchBar is re-added after the device is rotated, because Xamarin.Forms automatically removes it
        protected override void OnSizeChanged(int w, int h, int oldw, int oldh)
        {
            base.OnSizeChanged(w, h, oldw, oldh);

            if (Element is ISearchPage && Element is Page page)
            {
                AddSearchToToolbar(page.Title);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (GetToolbar() is Toolbar toolBar)
                toolBar.Menu?.RemoveItem(Resource.Menu.MainMenu);

            base.Dispose(disposing);
        }

        //Workaround to re-add the SearchView when navigating back to an ISearchPage, because Xamarin.Forms automatically removes it
        void HandleNavigationPagePopped(object sender, NavigationEventArgs e)
        {
            if (sender is NavigationPage navigationPage
                && navigationPage.CurrentPage is ISearchPage)
            {
                AddSearchToToolbar(navigationPage.CurrentPage.Title);
            }
        }

        void AddSearchToToolbar(string pageTitle)
        {
            if (GetToolbar() is Toolbar toolBar
                && toolBar.Menu?.FindItem(Resource.Id.ActionSearch)?.ActionView?.JavaCast<SearchView>().GetType() != typeof(SearchView))
            {
                toolBar.Title = pageTitle;
                toolBar.InflateMenu(Resource.Menu.MainMenu);

                var menu = toolBar.Menu?.FindItem(Resource.Id.ActionSearch);

                menu.SetOnActionExpandListener(this);

                if (toolBar.Menu?.FindItem(Resource.Id.ActionSearch)?.ActionView?.JavaCast<SearchView>() is SearchView searchView)
                {
                    SearchView = searchView;
                    SearchView.SetOnQueryTextFocusChangeListener(this);
                    SearchView.QueryTextChange += HandleQueryTextChange; ;
                    SearchView.QueryTextSubmit += HandleQueryTextSubmit;
                    SearchView.ImeOptions = (int)ImeAction.Done;
                    SearchView.InputType = (int)InputTypes.TextVariationFilter;
                    SearchView.MaxWidth = int.MaxValue; //Set to full width - http://stackoverflow.com/questions/31456102/searchview-doesnt-expand-full-width
                }
            }
        }


        public void OnFocusChange(Android.Views.View v, bool hasFocus)
        {
            if (v is SearchView && Element is ISearchPage searchPage)
            {
                if (hasFocus)
                {
                    searchPage.IsSearching = true;
                }
            }
        }
        private void HandleQueryTextChange(object sender, QueryTextChangeEventArgs e)
        {
            if (Element is ISearchPage searchPage)
            {
                searchPage.OnSearchBarTextChanged(e.NewText);
            }
        }

        void HandleQueryTextSubmit(object sender, QueryTextSubmitEventArgs e)
        {
            InputMethodManager imm = CrossCurrentActivity.Current.Activity.GetSystemService(Context.InputMethodService) as InputMethodManager;
            imm.HideSoftInputFromWindow(SearchView.WindowToken, HideSoftInputFlags.None);
        }

        Toolbar GetToolbar() => CrossCurrentActivity.Current.Activity.FindViewById<Toolbar>(Resource.Id.toolbar);


        public bool OnMenuItemActionCollapse(IMenuItem item)
        {
            if (Element is ISearchPage searchPage)
                searchPage.IsSearching = false;
            return true;
        }

        public bool OnMenuItemActionExpand(IMenuItem item)
        {
            return true;
        }
    }
}