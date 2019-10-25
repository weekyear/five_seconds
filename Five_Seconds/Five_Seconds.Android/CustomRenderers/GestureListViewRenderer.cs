using System;
using Android.Content;
using Android.Views;
using Five_Seconds.CustomControls;
using Five_Seconds.Droid.CustomRenderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(GestureListView), typeof(GestureListViewRenderer))]
namespace Five_Seconds.Droid.CustomRenderers
{
    class GestureListViewRenderer : ListViewRenderer
    {
        readonly CustomGestureListener _listener;
        readonly GestureDetector _detector;

        public GestureListViewRenderer(Context context) : base(context)
        {
            _listener = new CustomGestureListener();
            _detector = new GestureDetector(context, _listener);
        }

        public override bool DispatchTouchEvent(MotionEvent e)
        {
            if (_detector != null)
            {
                _detector.OnTouchEvent(e);
                base.DispatchTouchEvent(e);
                return true;
            }

            return base.DispatchTouchEvent(e);
        }

        public override bool OnTouchEvent(MotionEvent ev)
        {
            base.OnTouchEvent(ev);

            if (_detector != null)
                return _detector.OnTouchEvent(ev);

            return false;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.ListView> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement == null)
            {
                _listener.OnSwipeLeft -= HandleOnSwipeLeft;
                _listener.OnSwipeRight -= HandleOnSwipeRight;
            }

            if (e.OldElement == null)
            {
                _listener.OnSwipeLeft += HandleOnSwipeLeft;
                _listener.OnSwipeRight += HandleOnSwipeRight;
            }
        }

        void HandleOnSwipeLeft(object sender, EventArgs e) =>
            ((GestureListView)Element).OnSwipeLeft();

        void HandleOnSwipeRight(object sender, EventArgs e) =>
            ((GestureListView)Element).OnSwipeRight();
    }
}