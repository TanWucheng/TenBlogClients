using AndroidX.RecyclerView.Widget;

namespace TenBlogDroidApp.Listeners
{
    public class FabScrollListener : RecyclerView.OnScrollListener
    {
        private const int Threshold = 20;
        private readonly IFabDisplayListener _displayListener;
        private int _distance;
        private bool _visible;

        public FabScrollListener(IFabDisplayListener displayListener)
        {
            _displayListener = displayListener;
            _distance = 0;
            _visible = true;
        }

        public override void OnScrolled(RecyclerView recyclerView, int dx, int dy)
        {
            base.OnScrolled(recyclerView, dx, dy);
            switch (_distance)
            {
                case > Threshold when _visible:
                    _visible = false;
                    _displayListener.FabHide();
                    _distance = 0;
                    break;
                case < -20 when !_visible:
                    _visible = true;
                    _displayListener.FabShow();
                    _distance = 0;
                    break;
            }

            if (_visible && dy > 0 || (!_visible && dy < 0))
            {
                _distance += dy;
            }
        }
    }
}